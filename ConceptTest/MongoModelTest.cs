using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;
using MongoDB.Driver;
using NSwag;
using Xunit;

namespace ConceptTest
{
    public class MongoModelTest
    {
        [Fact]
        public async Task SaveActivity()
        {
            IContext context = new ActivityContext();
            IRepository activityRepo = new ActivityRepository(context);

            var activity = new UserActivity()
            {
                UserId = "userid",
                UpdatedAt = DateTime.UtcNow
            };
               
            await activityRepo.AddAsync(activity);
            var activityFromMongo = await activityRepo.GetAsync(activity.Id);
            
            Assert.Equal("userid", activityFromMongo.UserId);
        }

        [Fact]
        public async Task DeleteActivity()
        {
            IContext context = new ActivityContext();
            IRepository activityRepo = new ActivityRepository(context);

            var activity = new UserActivity()
            {
                UserId = "userid"
            };
               
            await activityRepo.AddAsync(activity);
            DeleteResult result = await activityRepo.DeleteAsync(activity.Id);
            
            Assert.True(result.IsAcknowledged);
        }
    }
}