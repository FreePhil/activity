using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface IUserActivityService
    {
        Task<UserActivity> GetActivityAsync(string id);
       
        Task AddActivityAsync(UserActivity activity);
        
        Task<bool> UpdatePayloadAsync(string id, string payload);
        Task<bool> UpdateOptionAsync(string id, string option);
        Task<bool> UpdateStatusAsync(string id, string status);
        Task<bool> UpdateExportedAsync(string id, UpdateExportedModel export);
        Task<bool> UpdateCallbackAsync(string id, JobCompletionSummary job);

        Task<IList<UserActivity>> GetActivitiesByUserAsync(string userId);
        Task<IList<UserActivity>> GetActivitiesBySubjectAsync(string userId, string subjectName, string productName);
    }
}