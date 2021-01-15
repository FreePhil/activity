using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ICacheLoader
    {
        IList<EducationLevel> ReadCache(string version);
    }
}