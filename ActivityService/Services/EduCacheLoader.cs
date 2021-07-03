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
        
        public IList<EducationLevel> ReadCache(string eduVersion)
        {
            var educationLevels =
                cache.Get<IDictionary<string, IList<EducationLevel>>>(jsonUri.CacheName.EducationLevel);
            IList<EducationLevel> educationLevel;
            if (!educationLevels.TryGetValue(eduVersion, out educationLevel))
            {
                Task<IList<EducationLevel>> task = 
                    Task.Run<IList<EducationLevel>>(async () => await filler.Load(eduVersion).ConfigureAwait(false));
                educationLevel = task.Result;
                try
                {
                    educationLevels.Add(eduVersion, educationLevel);
                }
                catch (Exception e)
                {
                    Log.Error("version {Version} of education level existing can not be appended to cache. {Message}", eduVersion, e.Message);
                }
            }

            return educationLevel;
        }
    }
}