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
        private ICacheLoader loader;
        private IMemoryCache cache;

        public TestGoSubjectFetcher(
            IHttpClientFactory httpClientFactory, 
            ICacheLoader loader,
            IMemoryCache cache,
            IOptionsMonitor<JsonLocationOptions> configAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
            this.cache = cache;
            this.loader = loader;
        }

        public IList<EducationLevel> Load(string testgoVersion, string userId)
        {
            Task<string> task = Task.Run<string>(async () =>
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{jsonUri.TestGoPermissibleUri}/{userId}?v={testgoVersion}");
                return await response.Content.ReadAsStringAsync();
            });

            var subjectJson = task.Result;
            var subjectContainer = JsonConvert.DeserializeObject<TestGoSubject>(subjectJson);

            var currentVersion = cache.Get<String>(jsonUri.CacheName.TestGoVersionCacheName);
            if (testgoVersion != currentVersion)
            {
                var testgoUri = $"{jsonUri.TestGoSubjectUri}/{testgoVersion}/{jsonUri.TestGoSubjectFilename}";
                var httpClient = httpClientFactory.CreateClient();

                var response = httpClient.GetAsync(testgoUri).ConfigureAwait(false).GetAwaiter().GetResult();
                using (response)
                {
                    var testgoJson = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    var allLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(testgoJson);
                    CreateLookupCache(allLevels);
                }
            }

            return ConvertToEducationLevel(subjectContainer);
        }
        private void CreateLookupCache(IList<EducationLevel> levels)
        {
            if (levels == null) return;
        
            IDictionary<string, string> subjectDictionary = new Dictionary<string, string>(); 
            IDictionary<string, string> productDictionary = new Dictionary<string, string>();

            foreach (var level in levels)
            {
                foreach (var subject in level.Subjects)
                {
                    if (!subjectDictionary.ContainsKey(subject.Id))
                    {
                        subjectDictionary.Add(subject.Id, subject.Name);
                    }
                    foreach (var product in subject.Products)
                    {
                        if (!productDictionary.ContainsKey(product.Id))
                        {
                            productDictionary.Add(product.Id, product.Name);
                        }
                    }
                }
            }
        
            cache.Set<IDictionary<string, string>>(jsonUri.CacheName.SubjectsLookup, subjectDictionary);
            cache.Set<IDictionary<string, string>>(jsonUri.CacheName.ProductsLookup, productDictionary);
        }


        private IList<EducationLevel> ConvertToEducationLevel(TestGoSubject container)
        {
            var levelsDictionary = new Dictionary<string, EducationLevel>
            {
                {"E", new EducationLevel { Id = 0, SchoolType = "國小"}},
                {"J", new EducationLevel { Id = 1, SchoolType = "國中"}},
                {"H", new EducationLevel { Id = 2, SchoolType = "高中"}},
                {"V", new EducationLevel { Id = 3, SchoolType = "技術高中 (高職)"}},
            };

            foreach (var subject in container.Subjects)
            {
                levelsDictionary[subject.Id.Substring(0, 1)].Subjects.Add(subject);
            }

            return levelsDictionary.Values.Where(level => level.Subjects.Count > 0).ToList();
        }
    }
}