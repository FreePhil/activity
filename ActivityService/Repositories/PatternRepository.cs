using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class PatternRepository: ServiceRepository<QuestionPattern>, IPatternRepository
    {
        public PatternRepository(IContext context) : base(context)
        {
        }

        public void CreateIndex()
        {
            var indexBuilder = Builders<QuestionPattern>.IndexKeys;
            
            var indexModel = new CreateIndexModel<QuestionPattern>(indexBuilder
                .Ascending(pattern => pattern.UserId)
                .Ascending(pattern => pattern.UpdatedAt)
                .Ascending(pattern => pattern.ProductName));
            Context.GetCollection<QuestionPattern>().Indexes.CreateOne(indexModel);
        }

        public async Task<IList<QuestionPattern>> GetAllAsync(string userId, string subjectName, string productName)
        {
            var patterns = await Context.GetCollection<QuestionPattern>()
                .Find(u => u.UserId == userId && u.SubjectName == subjectName && u.ProductName == productName)
                .SortByDescending(u => u.UserId)
                .ToListAsync();
            return patterns;
        }

        public async Task<IList<QuestionPattern>> GetAllWithPublicAsync(string userId, string subjectName, string productName)
        {
            var patterns = await Context.GetCollection<QuestionPattern>()
                .Find(u => (u.UserId == null || u.UserId == userId) && u.SubjectName == subjectName && u.ProductName == productName)
                .SortBy(u => u.UserId)
                .ToListAsync();
            return patterns;   
        }
    }
}