using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

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
        
        public async Task<IList<EducationLevel>> ReadCache(string eduVersion)
        {
            // String currentVersion = string.Empty;
            var currentVersion = cache.Get<String>(jsonUri.CacheName.EduVersionCacheName);
            if (eduVersion != currentVersion)
            {
                cache.Remove(jsonUri.CacheName.EducationLevel);
            }
            IList<EducationLevel> levels = cache.GetOrCreate<IList<EducationLevel>>(jsonUri.CacheName.EducationLevel, entry =>
            {
                try
                {
                    filler.Load(eduVersion);
                    Task<IList<EducationLevel>> task =
                        Task.Run<IList<EducationLevel>>(async () => await filler.Load(eduVersion));
                    cache.Set(jsonUri.CacheName.EduVersionCacheName, eduVersion);
                    
                    Log.Information("read education levels from json files and cache version set to {EduVersion}", eduVersion);
                    return task.Result;
                }
                catch (Exception)
                {
                    Log.Error("failed to read education levels from json files for version {EduVersion}", eduVersion);
                    return new List<EducationLevel>();
                }
            });

            return levels; // always from cache
        }
    }
}