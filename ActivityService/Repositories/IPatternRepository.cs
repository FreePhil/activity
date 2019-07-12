using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Repositories
{
    public interface IPatternRepository: IRepository<QuestionPattern>
    {
        void CreateIndex();

        Task<bool> DeleteWithUserAsync(string id);
        Task<IList<QuestionPattern>> GetAllAsync(string userId, string subjectName, string productName);
        Task<IList<QuestionPattern>> GetAllWithPublicAsync(string userId, string subjectName, string productName);
    }
}