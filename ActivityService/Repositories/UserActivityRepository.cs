using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class UserActivityRepository: ServiceRepository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(IContext context): base(context)
        {
        }

        public void CreateIndex()
        {
            var indexBuilder = Builders<UserActivity>.IndexKeys;
            var indexModel = new CreateIndexModel<UserActivity>(indexBuilder.Ascending(a => a.UserId));
            
            Context.GetCollection<UserActivity>().Indexes.CreateOne(indexModel);
        }

        public async Task<IList<UserActivity>> GetByUserAsync(string userId)
        {
            var activities = await Context.GetCollection<UserActivity>().
                Find(u => u.UserId == userId).
                SortByDescending(u => u.Id).
                ToListAsync();
            return activities;
        }
    }
}