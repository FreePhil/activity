using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public interface IRepository
    {
        IContext Context { get; }
        
        Task<List<UserActivity>> GetAll(int pageNo, int pageSize);

        Task AddAsync(UserActivity activity);
        Task<UserActivity> GetAsync(ObjectId id);
        Task<DeleteResult> DeleteAsync(ObjectId id);
        
        Task<bool> UpdateOption(string id, string option);
        Task<bool> UpdatePayload(string id, string payload);
        Task<bool> UpdateStatus(string id, string status);
    }
}