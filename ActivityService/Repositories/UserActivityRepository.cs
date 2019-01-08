using System;
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

        public async Task<bool> UpdateCallbackAsync(string id, UpdateExportedModel updated)
        {
            var update = Builders<UserActivity>.Update
                .Set(m => m.Name, updated.TestName)
                .Set(m => m.SubjectName, updated.SubjectName)
                .Set(m => m.Status, updated.Export.Status)
                .Set(m => m.Manifest, updated.Export.Manifest)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);
            
            var result = await Context.GetCollection<UserActivity>().UpdateOneAsync(entity => entity.Id == id, update);

            return result.IsAcknowledged;
        }
    }
}