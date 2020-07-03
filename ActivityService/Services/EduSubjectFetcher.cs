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
    public class EduSubjectFetcher: ISubjectFetcher
    {
        private IHttpClientFactory httpClientFactory;
        private JsonLocationOptions jsonUri;
        private IMemoryCache cache;

        public EduSubjectFetcher(IHttpClientFactory httpClientFactory, IOptionsMonitor<JsonLocationOptions> configAccessor, IMemoryCache cache)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
            this.cache = cache;
        }
        
        public IList<EducationLevel> Load(string userId)
        {
            IList<EducationLevel> levels = cache.GetOrCreate<IList<EducationLevel>>("subjects", entry =>
            {
                cache.Set<IDictionary<string, string>>("subjects-lookup", null);
                cache.Set<IDictionary<string, string>>("products-lookup", null);
                cache.Set<IDictionary<string, string>>("verions-lookup", null);

                return null;
            });

            return levels;
        }

        private async Task<IList<EducationLevel>> GetSubjects()
        {
            IList<EducationLevel> allSubjects = null;
            
            var httpClient = httpClientFactory.CreateClient();
            using (var subjectResponse = await httpClient.GetAsync(jsonUri.SubjectUri))
            using (var subjectJsonStream = await subjectResponse.Content.ReadAsStreamAsync())
            {
                allSubjects = JsonConvert.DeserializeObject<IList<EducationLevel>>(subjectJsonStream.ToString());
            }

            return allSubjects;
        }
            
    }
}