using System;
using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public class SubjectFetcherFactory: ISubjectFetcherFactory
    {
        private const string EduDomain = "edu";
        private const string TestGoDemain = "testgo";
        
        public ISubjectFetcher Create(string userDomain)
        {
            var normalizedDomain = userDomain.ToLower();
            switch (normalizedDomain)
            {
                case EduDomain:
                    return new EduSubjectFetcher();
                    break;
                case TestGoDemain:
                    return new TestGoSubjectFetcher();
                    break;
                default:
                    throw new Exception($"{userDomain} did not implement");
            }
        }
    }
}