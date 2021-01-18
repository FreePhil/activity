using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ActivityService.Services
{
    public class CacheLoader: ICacheLoader
    {
        private ICacheFiller filler;
        private JsonLocationOptions jsonUri;
        private IMemoryCache cache;

        public CacheLoader(ICacheFiller cacheFiller, IOptionsMonitor<JsonLocationOptions> configAccessor, IMemoryCache cache)
        {
            this.filler = cacheFiller;
            this.jsonUri = configAccessor.CurrentValue;
            this.cache = cache;
        }
        
        public IList<EducationLevel> ReadCache(string version)
        {
            // remove version info
            //
            if (cache.TryGetValue(jsonUri.CacheName.VersionCacheName, out string cacheVersion))
            {
                if (cacheVersion != version)
                {
                    cache.Remove(jsonUri.CacheName.VersionCacheName);
                    cache.Remove(jsonUri.CacheName.EducationLevel);
                }
            }
            IList<EducationLevel> levels = cache.GetOrCreate<IList<EducationLevel>>(jsonUri.CacheName.EducationLevel, entry =>
            {
                cache.Set(jsonUri.CacheName.VersionCacheName, version);
                try
                {
                    Task<IList<EducationLevel>> task =
                        Task.Run<IList<EducationLevel>>(async () => await filler.Load());
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