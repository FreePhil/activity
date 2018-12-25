using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public interface IRepository
    {
        IContext Context { get; }
        
        Task<List<UserActivity>> GetAllAsync(int pageNo, int pageSize);

        Task AddAsync(UserActivity activity);
        Task<UserActivity> GetAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(string id, Expression<Func<UserActivity, string>> updater, string value);
    }
}