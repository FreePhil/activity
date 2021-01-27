using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ActivityService.Services
{
    public class EduCacheLoader: ICacheLoader
    {
        private ICacheFiller filler;
        private JsonLocationOptions jsonUri;
        private IMemoryCache cache;

        public EduCacheLoader(ICacheFiller cacheFiller, IOptionsMonitor<JsonLocationOptions> configAccessor, IMemoryCache cache)
        {
            this.filler = cacheFiller;
            this.jsonUri = configAccessor.CurrentValue;
            this.cache = cache;
        }
        
        public IList<EducationLevel> ReadCache(string eduVersion)
        {
            // String currentVersion = string.Empty;
            var currentVersion = cache.Get<String>(jsonUri.CacheName.EduVersionCacheName);
            if (eduVersion != currentVersion)
            {
                cache.Remove(jsonUri.CacheName.EducationLevel);
            }
            IList<EducationLevel> levels = cache.GetOrCreate<IList<EducationLevel>>(jsonUri.CacheName.EducationLevel, entry =>
            {
                cache.Set(jsonUri.CacheName.EduVersionCacheName, eduVersion);
                try
                {
                    Task<IList<EducationLevel>> task =
                        Task.Run<IList<EducationLevel>>(async () => await filler.Load(eduVersion));
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