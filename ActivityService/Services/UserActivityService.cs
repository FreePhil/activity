using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Services
{
    public class UserActivityService: IUserActivityService
    {
        private IUserActivityRepository Repository { get; }
        public UserActivityService(IUserActivityRepository repository)
        {
            Repository = repository;
        }

        public Task<UserActivity> GetActivityAsync(string id)
        {
            return Repository.GetAsync(id);
        }

        public Task AddActivityAsync(UserActivity activity)
        {
            return Repository.AddAsync(activity);
        }

        public Task<bool> UpdatePayloadAsync(string id, string payload)
        {
            return Repository.UpdateAsync(id, ac => ac.Payload, payload);
        }

        public Task<bool> UpdateOptionAsync(string id, string option)
        {
            return Repository.UpdateAsync(id, ac => ac.Option, option);
        }

        public Task<bool> UpdateStatusAsync(string id, string status)
        {
            return Repository.UpdateAsync(id, ac => ac.Status, status);
        }

        public Task<bool> UpdateExportedAsync(string id, UpdateExportedModel export)
        {
            return Repository.UpdateExportedAsync(id, export);
        }
        
        public Task<bool> UpdateCallbackAsync(string id, JobCompletionSummary job)
        {
            return Repository.UpdateCallbackAsync(id, job);
        }

        public Task<IList<UserActivity>> GetActivitiesByUserAsync(string userId)
        {
            return Repository.GetByUserAsync(userId);
        }
        
        public Task<IList<UserActivity>> GetActivitiesBySubjectAsync(string userId, string subjectName, string productName)
        {
            return Repository.GetBySubjectAsync(userId, subjectName, productName);
        }
    }
}