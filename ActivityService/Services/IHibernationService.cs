using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface IHibernationService
    {
        Task<Hibernation> GetHibernationAsync(string id);
        Task<Hibernation> GetHibernationAsync(string userId, string subjectName, string productName);
        Task<Hibernation> CreateOrUpdateHibernationForwardAsync(Hibernation dormancy);
        Task<Hibernation> CreateOrUpdateHibernationBackwardAsync(Hibernation dormancy);
        Task<Hibernation> UpdateOnTheSameStageAsync(string Id, StagePayload stageOnly);

        Task DeleteHibernationAsync(string id);
    }
}