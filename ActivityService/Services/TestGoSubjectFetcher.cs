using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public class TestGoSubjectFetcher: ISubjectFetcher
    {
        public IList<Subject> Load(string userId)
        {
            throw new System.NotImplementedException();
        }
    }
}