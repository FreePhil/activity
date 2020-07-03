using System.Collections.Generic;
using ActivityService.Models;
using Microsoft.Extensions.Logging;

namespace ActivityService.Services
{
    public class TestGoSubjectFetcher: ISubjectFetcher
    {
        private ILogger<TestGoSubjectFetcher> logger;
        
        public TestGoSubjectFetcher(ILogger<TestGoSubjectFetcher> logger)
        {
            this.logger = logger;
        }
        public IList<EducationLevel> Load(string userId)
        {
            logger.LogInformation("hello");
            return null;
        }
    }
}