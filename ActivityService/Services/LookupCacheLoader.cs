using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ActivityService.Services
{
    public class LookupCacheLoader: ILookupCacheLoader
    {
        private IHttpClientFactory httpClientFactory;
        private JsonLocationOptions jsonUri;
        private IMemoryCache cache;
        
        public LookupCacheLoader(IHttpClientFactory httpClientFactory, IOptionsMonitor<JsonLocationOptions> configAccessor, IMemoryCache cache)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
            this.cache = cache;
        }
        
        public void ReadCache(string testGoVersion)
        {
            var currentVersion = cache.Get<string>(jsonUri.CacheName.TestGoVersionCacheName);
            if (testGoVersion != currentVersion)
            {
                var testgoUri = $"{jsonUri.TestGoSubjectUri}/{testGoVersion}/{jsonUri.TestGoSubjectFilename}";
                
                try
                {
                    Task task =
                        Task.Run(async () =>
                        {
                            var client = httpClientFactory.CreateClient();
                            using (var response = await client.GetAsync(testgoUri))
                            {
                                var testgoJson = await response.Content.ReadAsStringAsync();
                                var allLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(testgoJson);
                                CreateLookupCache(allLevels);
                            }
                        });
                    task.Wait();
                }
                catch (Exception)
                {
                }
                cache.Set<string>(jsonUri.CacheName.TestGoVersionCacheName, testGoVersion);
            }
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
    }
}