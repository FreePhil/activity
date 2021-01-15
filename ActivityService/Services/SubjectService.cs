using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public class SubjectService: ISubjectService
    {
        private ISubjectFetcherFactory fetcherFactory;

        public SubjectService(ISubjectFetcherFactory fetcherFactory)
        {
            this.fetcherFactory = fetcherFactory;
        }
        
        public IList<EducationLevel> GetProductListing(string version, string userId, string userDomain)
        {
            var fetcher = fetcherFactory.Create(userDomain);
            IList<EducationLevel> subjects = fetcher.Load(version, userId);

            return subjects;
        }
    }
}