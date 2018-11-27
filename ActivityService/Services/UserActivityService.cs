using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Services
{
    public class UserActivityService: IUserActivityService
    {
        private IRepository Repository { get; }
        public UserActivityService(IRepository repository)
        {
            Repository = repository;
        }

        public Task<UserActivity> GetActivity(string id)
        {
            return Repository.GetAsync(id);
        }

        public Task AddActivity(UserActivity activity)
        {
            return Repository.AddAsync(activity);
        }

        public Task<bool> UpdatePayload(string id, string payload)
        {
            return Repository.UpdateAsync(id, ac => ac.Payload, payload);
        }

        public Task<bool> UpdateOption(string id, string option)
        {
            return Repository.UpdateAsync(id, ac => ac.Option, option);
        }

        public Task<bool> UpdateStatus(string id, string status)
        {
            return Repository.UpdateAsync(id, ac => ac.Status, status);
        }
    }
}