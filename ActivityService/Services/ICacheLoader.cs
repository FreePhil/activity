using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ICacheLoader
    {
        Task<IList<EducationLevel>> ReadCache(string version);
    }
}