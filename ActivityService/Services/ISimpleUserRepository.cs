using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Services
{
    public interface ISimpleUserRepository: IRepository<SimpleUser>
    {
        Task<SimpleUser> GetByUserName(string userName);
    }
}