using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class ActivityRepository: IRepository
    {
        public ActivityRepository(IContext context)
        {
            this.Context = context;
        }
        
        public IContext Context { get; }
        
        public Task<List<UserActivity>> GetAll(int pageNo, int pageSize)
        {
            return Context.Activities.Find(_ => true).ToListAsync();
        }

        public Task AddAsync(UserActivity activity)
        {
            activity.UpdatedAt = DateTime.UtcNow;
            return Context.Activities.InsertOneAsync(activity);
        }

        public Task<UserActivity> GetAsync(ObjectId id)
        {
            return Context.Activities.Find(activity => activity.Id == id).FirstOrDefaultAsync();
        }

        public Task<DeleteResult> DeleteAsync(ObjectId id)
        {
            return Context.Activities.DeleteOneAsync(activity => activity.Id == id);
        }

        public Task<UpdateResult> UpdateAsync(ObjectId id, Expression<Func<UserActivity, string>> updater, string value)
        {
            var update = Builders<UserActivity>.Update
                .Set(updater, value)
                .Set(activity => activity.UpdatedAt, DateTime.UtcNow);
            return Context.Activities.UpdateOneAsync(activity => activity.Id == id, update);
        }
    }
}