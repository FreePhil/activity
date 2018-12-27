using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class UserActivityRepository: ServiceRepository<UserActivity>, IUserActivityRepository
    {
        public IContext Context { get; }
        public UserActivityRepository(IContext context): base(context)
        {
            Context = context;
        }

        public Task<List<UserActivity>> GetByUserAsync(string userId)
        {
            return Context.GetCollection<UserActivity>().
                Find(u => u.UserId == userId).
                SortByDescending(u => u.Id).
                ToListAsync();
        }
    }
}