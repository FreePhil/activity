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
   
        public async Task<IList<EducationLevel>> Load(string version)
        {
            // todo: add version info
            
            // use test go subjects to generate the cache of lookup table
            //
            // var testgoUri = $"{jsonUri.TestGoSubjectUri}/{version}/{jsonUri.TestGoSubjectFilename}";
            // var httpClient = httpClientFactory.CreateClient();
            // using (var testgoResponse = await httpClient.GetAsync(testgoUri))
            // {
            //     var testgoJson = await testgoResponse.Content.ReadAsStringAsync();
            //     var allLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(testgoJson);
            //     CreateLookupCache(allLevels);
            // }
            
            // use edu subject to generate default subjects for schools
            //
            var httpClient = httpClientFactory.CreateClient();
            var eduUri = $"{jsonUri.EduSubjectUri}/{version}/{jsonUri.EduSubjectFilename}";
            IList<EducationLevel> eduLevels = null;
            using (var eduResponse = await httpClient.GetAsync(eduUri))
            {
                var eduJson = await eduResponse.Content.ReadAsStringAsync();
                eduLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(eduJson);
            }

            return eduLevels;
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