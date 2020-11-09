using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface IUserActivityService
    {
        Task<UserActivity> GetActivityAsync(string id);
        Task AddActivityAsync(UserActivity activity);
        Task<bool> DeleteActivityAsync(string id, string userId);
        Task<long> DeleteActivitiesAsync(IList<string> ids, string userId);
        Task<bool> UpdatePayloadAsync(string id, string payload);
        Task<bool> UpdateOptionAsync(string id, string option);
        Task<bool> UpdateStatusAsync(string id, string status);
        Task<bool> UpdateExportedAsync(string id, UpdateExportedModel export);
        Task<bool> UpdateCallbackAsync(string id, JobCompletionSummary job);

        Task<IList<UserActivity>> GetActivitiesByUserAsync(string userId);
        Task<long> GetActivityCountByUserAsync(string userId);
        Task<IList<UserActivity>> GetActivitiesPagingByUserAsync(string userId, int pageNo, int pageSize);
        
        Task<IList<UserActivity>> GetActivitiesBySubjectAsync(string userId, string subjectName, string productName);
        Task<long> GetActivitiCountBySubjectAsync(string userId, string subjectName, string productName);
        Task<IList<UserActivity>> GetActivitiesPagingBySubjectAsync(string userId, string subjectName, string productName, int pageNo, int pageSize);
    }
}