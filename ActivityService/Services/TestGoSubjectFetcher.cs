using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ActivityService.Services
{
    public class TestGoSubjectFetcher: ISubjectFetcher
    {
        private IHttpClientFactory httpClientFactory;
        private JsonLocationOptions jsonUri;
        private ICacheFiller cacheFiller;
        private IMemoryCache cache;

        public TestGoSubjectFetcher(
            IHttpClientFactory httpClientFactory, 
            IOptionsMonitor<JsonLocationOptions> configAccessor, 
            ICacheFiller cacheFiller,
            IMemoryCache cache)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
            this.cacheFiller = cacheFiller;
            this.cache = cache;
        }

        public IList<EducationLevel> Load(string userId)
        {
            EnsureCacheLoaded();
            Task<string> task = Task.Run<string>(async () =>
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{jsonUri.TestGoUri}/{userId}");
                return await response.Content.ReadAsStringAsync();
            });

            var subjectJson = task.Result;
            var subjectContainer = JsonConvert.DeserializeObject<TestGoSubject>(subjectJson);

            return ConvertToEducationLevel(subjectContainer);
        }

        private void EnsureCacheLoaded()
        {
            IList<EducationLevel> levels = cache.GetOrCreate<IList<EducationLevel>>("education-level", entry =>
            {
                try
                {
                    Task<IList<EducationLevel>> task =
                        Task.Run<IList<EducationLevel>>(async () => await cacheFiller.Load());
                    return task.Result;
                }
                catch (Exception)
                {
                    return new List<EducationLevel>();
                }
            });
        }

        private IList<EducationLevel> ConvertToEducationLevel(TestGoSubject subjectContainer)
        {
            var levelsDictionary = new Dictionary<string, EducationLevel>
            {
                {"E", new EducationLevel { Id = 0, SchoolType = "國小"}},
                {"J", new EducationLevel { Id = 1, SchoolType = "國中"}},
                {"H", new EducationLevel { Id = 2, SchoolType = "高中"}},
                {"V", new EducationLevel { Id = 3, SchoolType = "技術高中 (高職)"}},
            };
            var subjectsLookupTable = cache.Get<IDictionary<string, string>>("subjects-lookup");
            var productsLookupTable = cache.Get<IDictionary<string, string>>("products-lookup");
            var versionsLookupTable = cache.Get<IDictionary<string, string>>("versions-lookup");
            
            
            foreach (var subject in subjectContainer.Subjects)
            {
                subject.Name = subjectsLookupTable[subject.Id];
                
                foreach (var product in subject.Products)
                {
                    product.Name = productsLookupTable[product.Id];
                    
                    foreach (var version in product.Versions)
                    {
                        version.Name = versionsLookupTable[version.Id];
                    }
                }
                
                levelsDictionary[subject.Id.Substring(0, 1)].Subjects.Add(subject);
            }

            return levelsDictionary.Values.Where(level => level.Subjects.Count > 0).ToList();
        }
    }
}