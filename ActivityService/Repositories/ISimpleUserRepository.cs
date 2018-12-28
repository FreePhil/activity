using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Repositories
{
    public interface ISimpleUserRepository: IRepository<SimpleUser>
    {
        void CreateIndex();
        Task<SimpleUser> GetByUserNameAsync(string userName);
        Task<SimpleUser> AddAsync(string userName);
    }
}