using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface IUserActivityService
    {
        Task<UserActivity> GetActivity(string id);
        Task AddActivity(UserActivity activity);
        
        Task<bool> UpdatePayload(string id, string payload);
        Task<bool> UpdateOption(string id, string option);
        Task<bool> UpdateStatus(string id, string status);
    }
}