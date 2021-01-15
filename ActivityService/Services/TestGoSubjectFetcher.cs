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
        private IMemoryCache cache;
        private ICacheLoader loader;

        public TestGoSubjectFetcher(
            IHttpClientFactory httpClientFactory, 
            ICacheLoader loader,
            IMemoryCache cache,
            IOptionsMonitor<JsonLocationOptions> configAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
            this.loader = loader;
            this.cache = cache;
        }

        public IList<EducationLevel> Load(string version, string userId)
        {
            EnsureCacheLoaded(version);
            Task<string> task = Task.Run<string>(async () =>
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{jsonUri.TestGoUri}/{userId}?v={version}");
                return await response.Content.ReadAsStringAsync();
            });

            var subjectJson = task.Result;
            var subjectContainer = JsonConvert.DeserializeObject<TestGoSubject>(subjectJson);

            return ConvertToEducationLevel(subjectContainer);
        }

        private void EnsureCacheLoaded(string version)
        {
            IList<EducationLevel> levels = loader.ReadCache(version);
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
            
            
            foreach (var subject in subjectContainer.Subjects)
            {
                subject.Name = subjectsLookupTable[subject.Id];
                
                foreach (var product in subject.Products)
                {
                    product.Name = productsLookupTable[product.Id];
                }
                
                levelsDictionary[subject.Id.Substring(0, 1)].Subjects.Add(subject);
            }

            return levelsDictionary.Values.Where(level => level.Subjects.Count > 0).ToList();
        }
    }
}