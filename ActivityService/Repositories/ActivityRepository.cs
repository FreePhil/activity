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

        public Task<bool> UpdateOption(string id, string option)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePayload(string id, string option)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateStatus(string id, string status)
        {
            throw new System.NotImplementedException();
        }
    }
}