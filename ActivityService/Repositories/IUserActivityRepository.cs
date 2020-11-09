using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Repositories
{
    public interface IUserActivityRepository: IRepository<UserActivity>
    {
        void CreateIndex();
        Task<IList<UserActivity>> GetByUserAsync(string userId);
        Task<long> GetDocumentCountByUser(string userId);
        Task<IList<UserActivity>> GetByUserAsync(string userId, int pageNo, int pageSize);
        Task<IList<UserActivity>> GetBySubjectAsync(string userId, string subjectName, string productName);

        Task<long> GetDocumentCountBySubject(string userId, string subjectName, string productName);
        Task<IList<UserActivity>> GetBySubjectAsync(string userId, string subjectName, string productName, int pageNo, int pageSize);

        Task<bool> DeleteUserActivityAsync(string id, string userId);
        Task<long> DeleteManyUserActivitiesAsync(IList<string> ids, string userId);
        
        Task<bool> UpdateExportedAsync(string id, UpdateExportedModel export);
        Task<bool> UpdateCallbackAsync(string id, JobCompletionSummary job);
    }
}