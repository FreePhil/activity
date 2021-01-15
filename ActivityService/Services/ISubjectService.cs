using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ISubjectService
    {
        IList<EducationLevel> GetProductListing(string version, string userId, string userDomain);
    }
}