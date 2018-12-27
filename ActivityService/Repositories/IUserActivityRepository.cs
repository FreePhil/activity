using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Repositories
{
    public interface IUserActivityRepository: IRepository<UserActivity>
    {
        Task<List<UserActivity>> GetByUserAsync(string userId);    
    }
}