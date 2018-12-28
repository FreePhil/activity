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
            UserActivity activity;
            for (int i = 0; i < 3; i++)
            {
                activity = new UserActivity()
                {
                    UserId = "userid",
                    Payload = i.ToString(),
                    UpdatedAt = DateTime.UtcNow
                };
                await activityRepo.AddAsync(activity);
            }
            
            var activityFromMongo = await activityRepo.GetByUserAsync("userid");
            
            Assert.Equal(3, activityFromMongo.Count);
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
        public async Task LoginTest()
        {
            ISimpleUserService service = Injector.GetService<ISimpleUserService>();

            SimpleUser user = await service.LoginAsync("phil");
            
            Assert.NotNull(user.Id);
            Assert.Equal("phil", user.Name);
        }
    }
}