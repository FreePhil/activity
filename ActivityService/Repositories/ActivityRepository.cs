using System;
using System.Collections.Generic;
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

        public Task<UpdateResult> UpdateOption(string id, string option)
        {
            var objectId = ObjectId.Parse(id);
            var update = Builders<UserActivity>.Update
                .Set(ac => ac.Payload, option)
                .Set(ac => ac.UpdatedAt, DateTime.UtcNow);
 
            var updateOption = new UpdateOptions { IsUpsert = true };
            return Context.Activities.UpdateOneAsync(ac => ac.Id == objectId, update, updateOption);
        }

        public Task<bool> UpdatePayload(string id, string payload)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStatus(string id, string status)
        {
            throw new System.NotImplementedException();
        }
    }
}