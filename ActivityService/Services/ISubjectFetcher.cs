using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ISubjectFetcher
    {
        IList<EducationLevel> Load(string eduVersion, string userId);
    }
}