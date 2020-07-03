using System.Collections.Generic;
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

        public TestGoSubjectFetcher(IHttpClientFactory httpClientFactory, IOptionsMonitor<JsonLocationOptions> configAccessor, IMemoryCache cache)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
            this.cache = cache;
        }

        public IList<EducationLevel> Load(string userId)
        {
            Task<string> task = Task.Run<string>(async () =>
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{jsonUri.TestGoUri}/{userId}");
                return await response.Content.ReadAsStringAsync();
            });

            var subjectJson = task.Result;
            var subjects = JsonConvert.DeserializeObject<TestGoSubject>(subjectJson);

            return null;
        }
    }
}