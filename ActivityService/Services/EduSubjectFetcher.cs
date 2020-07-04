using System;
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
    public class EduSubjectFetcher : ISubjectFetcher
    {
        private ICacheFiller cacheFiller;
        private IMemoryCache cache;

        public EduSubjectFetcher(ICacheFiller cacheFiller, IMemoryCache cache)
        {
            this.cacheFiller = cacheFiller;
            this.cache = cache;
        }

        public IList<EducationLevel> Load(string userId)
        {
            IList<EducationLevel> levels = cache.GetOrCreate<IList<EducationLevel>>("education-level", entry =>
            {
                try
                {
                    Task<IList<EducationLevel>> task =
                        Task.Run<IList<EducationLevel>>(async () => await cacheFiller.Load());
                    return task.Result;
                }
                catch (Exception)
                {
                    return new List<EducationLevel>();
                }
            });

            return levels; // always from cache
        }
    }
}