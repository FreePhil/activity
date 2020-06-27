using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ISubjectFetcher
    {
        IList<Subject> Load(string userId);
    }
}