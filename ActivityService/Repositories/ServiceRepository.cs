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
    public class ServiceRepository<T>: IRepository<T> where T: IServiceEntity
    {
        protected IContext Context { get; }
        public ServiceRepository(IContext context)
        {
            Context = context;
        }
        
        public Task<List<T>> GetAllAsync(int pageNo, int pageSize)
        {
            return Context.GetCollection<T>().Find(_ => true).ToListAsync();
        }

        public Task AddAsync(T entity)
        {
            return Context.GetCollection<T>().InsertOneAsync(entity);
        }

        public Task<T> GetAsync(string id)
        {
            return Context.GetCollection<T>().Find(entity => entity.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await Context.GetCollection<T>().DeleteOneAsync(activity => activity.Id == id);

            return result.IsAcknowledged;
        }

        public async Task<bool> UpdateAsync(string id, Expression<Func<T, string>> updater, string value)
        {
            var update = Builders<T>.Update
                .Set(updater, value)
                .Set(entity => entity.UpdatedAt, DateTime.UtcNow);
            var result = await Context.GetCollection<T>().UpdateOneAsync(entity => entity.Id == id, update);

            return result.IsAcknowledged;
        }
    }
}