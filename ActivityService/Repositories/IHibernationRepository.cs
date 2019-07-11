using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Repositories
{
    public interface IHibernationRepository: IRepository<Hibernation>
    {
        void CreateIndex();
        Task<Hibernation> GetAsync(string userId, string subjectName, string productName);
        Task UpdateAsync(string userId, string subjectName, string productName, StagePayload stage);

        Task<Hibernation> CreateOrUpdateAsync(string userId, string subjectName, string productName, StagePayload stage);
        Task<Hibernation> CreateOrUpdateAsync(Hibernation dormancy);
    }
}