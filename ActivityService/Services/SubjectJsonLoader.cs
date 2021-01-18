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
    public class SubjectJsonLoader: IJsonLoader
    {
        private IHttpClientFactory httpClientFactory;
        private JsonLocationOptions jsonUri;

        public SubjectJsonLoader(IHttpClientFactory httpClientFactory, IOptionsMonitor<JsonLocationOptions> configAccessor, IMemoryCache cache)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
        }
   
        public async Task<IList<EducationLevel>> Load()
        {
            IList<EducationLevel> eduLevels = null;
            
            var httpClient = httpClientFactory.CreateClient();
            using (var eduResponse = await httpClient.GetAsync(jsonUri.EduSubjectUri))
            {
                var eduJson = await eduResponse.Content.ReadAsStringAsync();
                eduLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(eduJson);
            }

            return eduLevels;
        }
    }
}