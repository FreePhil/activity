using System;
using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public class SubjectFetcherFactory: ISubjectFetcherFactory
    {
        private const string EduDomain = "edu";
        private const string TestGoDemain = "testgo";

        private ISubjectFetcher eduFetcher;
        private ISubjectFetcher testGoFetcher;

        public SubjectFetcherFactory(EduSubjectFetcher eduFetcher, TestGoSubjectFetcher testGoFetcher)
        {
            this.eduFetcher = eduFetcher;
            this.testGoFetcher = testGoFetcher;
        }
        
        public ISubjectFetcher Create(string userDomain)
        {
            var normalizedDomain = userDomain.ToLower();
            switch (normalizedDomain)
            {
                case EduDomain:
                    return eduFetcher;
                    break;
                case TestGoDemain:
                    return testGoFetcher;
                    break;
                default:
                    throw new Exception($"{userDomain} did not implement");
            }
        }
    }
}