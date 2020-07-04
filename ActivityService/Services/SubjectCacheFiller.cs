using System.Collections;
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
    public class SubjectCacheFiller: ICacheFiller
    {
        private IHttpClientFactory httpClientFactory;
        private JsonLocationOptions jsonUri;
        private IMemoryCache cache;

        public SubjectCacheFiller(IHttpClientFactory httpClientFactory, IOptionsMonitor<JsonLocationOptions> configAccessor, IMemoryCache cache)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
            this.cache = cache;
        }
   
        public async Task<IList<EducationLevel>> Load()
        {
            IList<EducationLevel> allLevels = null;
            var httpClient = httpClientFactory.CreateClient();
            using (var subjectResponse = await httpClient.GetAsync(jsonUri.SubjectUri))
            {
                var subjectJson = await subjectResponse.Content.ReadAsStringAsync();
                allLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(subjectJson);
                CreateLookupCache(allLevels);
            }

            return allLevels;
        }
        
        private void CreateLookupCache(IList<EducationLevel> levels)
        {
            cache.Set<IDictionary<string, string>>("versions-lookup", new Dictionary<string, string>
            {
                {"H", "翰林"},
                {"K", "康輯"},
                {"N", "南一"}
            });
            
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
                        
                        // append default version
                        product.Versions.Add(new LookupModel() { Id = "H", Name = "翰林"});
                    }
                }
            }
            
            cache.Set<IDictionary<string, string>>("subjects-lookup", subjectDictionary);
            cache.Set<IDictionary<string, string>>("products-lookup", productDictionary);
        }
    }
}