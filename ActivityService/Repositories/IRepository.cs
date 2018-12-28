using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ActivityService.Repositories
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync(int pageNo, int pageSize);

        Task AddAsync(T entity);
        Task<T> GetAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(string id, Expression<Func<T, string>> updater, string value);
    }
}