using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ISimpleUserService
    {
        Task<SimpleUser> LoginAsync(string userName);
    }
}