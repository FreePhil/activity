using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;

namespace ActivityService.Services
{
    public class PatternService: IPatternService
    {
        private IPatternRepository Repository { get; }
        public PatternService(IPatternRepository repository)
        {
            Repository = repository;
        }
        
        public async Task<QuestionPattern> GetPatternAsync(string id)
        {
            return await Repository.GetAsync(id);
        }

        public async Task<QuestionPattern> CreatePatternAsync(QuestionPattern pattern)
        {
            await Repository.AddAsync(pattern);

            return pattern;
        }

        public async Task DeletePatternAsync(string id)
        {
            await Repository.DeleteAsync(id);
        }

        public async Task<IList<QuestionPattern>> GetPatternsAsync(string userId, string subjectName, string productName)
        {
            return await Repository.GetAllAsync(userId, subjectName, productName);
        }
        
        public async Task<IList<QuestionPattern>> GetPatternsWithPublicAsync(string userId, string subjectName, string productName)
        {
            var publicPatterns = await GetPatternsAsync(null, subjectName, productName);
            var privatePatterns = await GetPatternsAsync(userId, subjectName, productName);
            
            return publicPatterns.Concat(publicPatterns).ToList();
        }
    }
}