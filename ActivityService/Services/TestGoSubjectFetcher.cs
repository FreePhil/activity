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

        public TestGoSubjectFetcher(
            IHttpClientFactory httpClientFactory, 
            IOptionsMonitor<JsonLocationOptions> configAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.jsonUri = configAccessor.CurrentValue;
        }

        public IList<EducationLevel> Load(string testGoVersion, string userId)
        {
            Task<string> task = Task.Run<string>(async () =>
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{jsonUri.TestGoPermissibleUri}/{userId}?v={testGoVersion}");
                return await response.Content.ReadAsStringAsync();
            });

            var subjectJson = task.Result;
            var subjectContainer = JsonConvert.DeserializeObject<TestGoSubject>(subjectJson);

            return ConvertToEducationLevel(subjectContainer);
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