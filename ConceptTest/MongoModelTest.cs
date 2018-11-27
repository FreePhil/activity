using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using NSwag;
using Xunit;
using JsonConvert = Newtonsoft.Json.JsonConvert;

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
            bool result = await activityRepo.DeleteAsync(activity.Id);
            
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateOptionOfActivity()
        {
            IContext context = new ActivityContext();
            IRepository activityRepo = new ActivityRepository(context);
            DummyPayload payload = new DummyPayload()
            {
                Id = 10,
                Value = "value"
            };
            var payloadString = JsonConvert.SerializeObject(payload);

            var activity = new UserActivity()
            {
                UserId = "userid",
                Option = "old",
                Payload = payloadString
            };
            await activityRepo.AddAsync(activity);

            bool result = await activityRepo.UpdateAsync(activity.Id, ac => ac.Option, "new");
            UserActivity newActivity = await activityRepo.GetAsync(activity.Id);

            var newPayload = JsonConvert.DeserializeObject<DummyPayload>(newActivity.Payload);
            
            Assert.True(result);
            Assert.Equal("new", newActivity.Option);
            
            Assert.Equal(10, newPayload.Id);
            Assert.Equal("value", newPayload.Value);
        }
    }
}