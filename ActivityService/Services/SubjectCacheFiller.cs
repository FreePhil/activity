using System;
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
using Serilog;

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
            // use edu subject to generate default subjects for schools
            //
            var httpClient = httpClientFactory.CreateClient();
            var eduUri = $"{jsonUri.EduSubjectUri}/{version}/{jsonUri.EduSubjectFilename}";
            IList<EducationLevel> eduLevels = null;
            
            Log.Information("read data from {eduUri}", eduUri);
            using (var eduResponse = await httpClient.GetAsync(eduUri))
            {
                var eduJson = await eduResponse.Content.ReadAsStringAsync();
                    eduLevels = JsonConvert.DeserializeObject<IList<EducationLevel>>(eduJson);
            }

            return eduLevels;
        }
    }
}