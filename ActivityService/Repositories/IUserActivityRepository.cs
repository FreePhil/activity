using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Repositories
{
    public interface IUserActivityRepository: IRepository<UserActivity>
    {
        void CreateIndex();
        Task<IList<UserActivity>> GetByUserAsync(string userId);
        Task<bool> UpdateExportedAsync(string id, UpdateExportedModel export);
        Task<bool> UpdateCallbackAsync(string id, JobCompletionSummary job);
    }
}