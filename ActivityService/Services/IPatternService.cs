using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface IPatternService
    {
        Task<QuestionPattern> GetPatternAsync(string id);
        Task<QuestionPattern> CreatePatternAsync(QuestionPattern pattern);
        Task<bool> DeletePatternAsync(string id);
        Task<IList<QuestionPattern>> GetPatternsAsync(string userId, string subjectName, string productName);
        Task<IList<QuestionPattern>> GetPatternsWithPublicAsync(string userId, string subjectName, string prductName);
    }
}