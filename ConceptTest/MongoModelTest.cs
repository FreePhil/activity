using System;
using System.Threading.Tasks;
using ActivityService.Injections;
using ActivityService.Models;
using ActivityService.Repositories;
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
            IRepository activityRepo = Injector.GetService<IRepository>();

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
            IRepository activityRepo = Injector.GetService<IRepository>();

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
            IRepository activityRepo = Injector.GetService<IRepository>();
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