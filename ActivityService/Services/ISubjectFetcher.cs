using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ISubjectFetcher
    {
        IList<EducationLevel> Load(string userId);
    }
}