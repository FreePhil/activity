using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using MongoDB.Driver;

namespace ActivityService.Repositories
{
    public class UserActivityRepository: ServiceRepository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(IContext context): base(context)
        {
        }

        public void CreateIndex()
        {
            var indexBuilder = Builders<UserActivity>.IndexKeys;
            
            var indexModel = new CreateIndexModel<UserActivity>(indexBuilder.Ascending(user => user.UserId));
            Context.GetCollection<UserActivity>().Indexes.CreateOne(indexModel);
            
            var subjectModel = new CreateIndexModel<UserActivity>(indexBuilder
                .Ascending(uid => uid.UserId)
                .Ascending(usub => usub.SubjectName)
                .Ascending(psub => psub.ProductName));
            Context.GetCollection<UserActivity>().Indexes.CreateOne(subjectModel);
        }

        public async Task<IList<UserActivity>> GetByUserAsync(string userId)
        {
            var activities = await Context.GetCollection<UserActivity>()
                .Find(u => u.UserId == userId)
                .SortByDescending(u => u.Id)
                .ToListAsync();
            return activities;
        }

        public async Task<IList<UserActivity>> GetBySubjectAsync(string userId, string subjectName, string productName)
        {
            var activitiesBySubject = await Context.GetCollection<UserActivity>()
                .Find(ac => ac.UserId == userId && ac.SubjectName == subjectName && ac.ProductName == productName)
                .SortByDescending(ac => ac.Id)
                .ToListAsync();

            return activitiesBySubject;
        }
        
        public async Task<bool> UpdateExportedAsync(string id, UpdateExportedModel export)
        {
            var exportUpdatingQuery = Builders<UserActivity>.Update
                .Set(m => m.Name, export.TestName)
                .Set(m => m.SubjectName, export.SubjectName)
                .Set(m => m.Status, export.Status)
                .Set(m => m.Volume, export.Volume)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);
            
            var result = await Context.GetCollection<UserActivity>().UpdateOneAsync(entity => entity.Id == id, exportUpdatingQuery);

            return result.IsAcknowledged;  
        }

        public async Task<bool> UpdateCallbackAsync(string id, JobCompletionSummary job)
        {
            var jobUpdatingQuery = Builders<UserActivity>.Update
                .Set(m => m.Status, job.Status)
                .Set(m => m.Manifest, job.Manifest)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);
            
            var result = await Context.GetCollection<UserActivity>().UpdateOneAsync(entity => entity.Id == id, jobUpdatingQuery);

            return result.IsAcknowledged;
        }

    }
}