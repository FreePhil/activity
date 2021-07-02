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
        private ICacheLoader loader;

        public EduSubjectFetcher(ICacheLoader loader)
        {
            this.loader = loader;
        }

        public async Task<IList<EducationLevel>> Load(string version, string userId)
        {
            IList<EducationLevel> levels = loader.ReadCache(version);
            return levels;
        }
    }
}