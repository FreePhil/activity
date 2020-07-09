using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            // IList<EducationLevel> allLevels = null;
            var httpClient = httpClientFactory.CreateClient();
            using (var versionResponse = await httpClient.GetAsync(jsonUri.VersionUri))
            {
                var versionJson = await versionResponse.Content.ReadAsStringAsync();
                var versionDictionary = JsonConvert.DeserializeObject<IList<LookupModel>>(versionJson).ToDictionary(k => k.Id, n => n.Name);
                cache.Set<IDictionary<string, string>>("versions-lookup", versionDictionary);
            }
            using (var subjectResponse = await httpClient.GetAsync(jsonUri.SubjectUri))
            {
                var subjectJson = await subjectResponse.Content.ReadAsStringAsync();
                var allLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(subjectJson);
                CreateLookupCache(allLevels);
            }
            IList<EducationLevel> eduLevels = null;
            using (var eduResponse = await httpClient.GetAsync(jsonUri.EduSubjectUri))
            {
                var eduJson = await eduResponse.Content.ReadAsStringAsync();
                eduLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(eduJson);
                AddDefaultVersion(eduLevels);
            }

            return eduLevels;
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
            
            cache.Set<IDictionary<string, string>>("subjects-lookup", subjectDictionary);
            cache.Set<IDictionary<string, string>>("products-lookup", productDictionary);
        }

        private void AddDefaultVersion(IList<EducationLevel> levels)
        {
            foreach (var level in levels)
            {
                foreach (var subject in level.Subjects)
                {
                    foreach (var product in subject.Products)
                    {
                        product.Versions.Add(new LookupModel { Id = "H", Name = "翰林版"});
                    }
                }
            }
        }
    }
}