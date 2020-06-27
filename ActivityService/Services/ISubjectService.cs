using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ISubjectService
    {
        IList<Subject> GetProductListing(string userId, string userDomain);
    }
}