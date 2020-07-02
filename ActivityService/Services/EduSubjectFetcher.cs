using System.Collections;
using System.Collections.Generic;
using ActivityService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ActivityService.Services
{
    public class EduSubjectFetcher: ISubjectFetcher
    {
        private IMemoryCache cache;

        public EduSubjectFetcher(IMemoryCache cache)
        {
            this.cache = cache;
        }
        
        public IList<Subject> Load(string userId)
        {
            IList<Subject> subjects = cache.GetOrCreate<IList<Subject>>("subjects", entry =>
            {
                cache.Set<IDictionary<string, string>>("subjects-lookup", null);
                cache.Set<IDictionary<string, string>>("products-lookup", null);
                cache.Set<IDictionary<string, string>>("verions-lookup", null);

                return null;
            });

            return subjects;
        }
    }
}