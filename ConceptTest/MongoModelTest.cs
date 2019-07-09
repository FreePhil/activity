using System;
using System.Threading.Tasks;
using ActivityService.Injections;
using ActivityService.Models;
using ActivityService.Repositories;
using ActivityService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using NSwag;
using Xunit;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace ConceptTest
{
    public class MongoModelTest
    {
        private ServiceProvider Injector { get; }
        public MongoModelTest()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();
            
            var services = new ServiceCollection();
            services.AddMongoDb(configuration);
            services.AddActivity();
            
            Injector = services.BuildServiceProvider();
        }
        
        [Fact]
        public async Task SaveActivity()
        {
            IUserActivityRepository activityRepo = Injector.GetService<IUserActivityRepository>();
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
        public async Task SortByIdForUser()
        {
            IUserActivityRepository activityRepo = Injector.GetService<IUserActivityRepository>();
            var activitiesFromMongo = await activityRepo.GetByUserAsync("userid");

            for (int i = 0; i < activitiesFromMongo.Count - 1; i++)
            {
                Assert.True(activitiesFromMongo[i].CreatedAt >= activitiesFromMongo[i+1].CreatedAt);
            }
        }

        [Fact]
        public async Task DeleteActivity()
        {
            IRepository<UserActivity> activityRepo = Injector.GetService<IRepository<UserActivity>>();

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
            IRepository<UserActivity> activityRepo = Injector.GetService<IRepository<UserActivity>>();
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

        [Fact]
        public async Task LoginExistingUserTest()
        {
            ISimpleUserService service = Injector.GetService<ISimpleUserService>();

            SimpleUser user = await service.LoginAsync("phil");
            
            Assert.Equal("5c2593550bf9a605288b2967", user.Id);
        }

        [Fact]
        public async Task LoginNonExistingUserTest()
        {
            ISimpleUserRepository repository = Injector.GetService<ISimpleUserRepository>();
            ISimpleUserService service = Injector.GetService<ISimpleUserService>();

            SimpleUser user = await service.LoginAsync("tom");
            
            Assert.NotNull(user.Id);
            Assert.Equal("tom", user.Name);

            await repository.DeleteAsync(user.Id);
        }

        [Fact]
        public async Task FindBySubjectTest()
        {
            IUserActivityService service = Injector.GetService<IUserActivityService>();

            var activities = await service.GetActivitiesBySubjectAsync("5c", "國中國文", "00");
            Assert.Equal(1, activities.Count);
        }
        
        [Fact]
        public async Task FindBySubjectTest_NoResult()
        {
            IUserActivityService service = Injector.GetService<IUserActivityService>();

            var activities = await service.GetActivitiesBySubjectAsync("5c", "國中國文", "01");
            Assert.Equal(0, activities.Count);
        }
    }
}