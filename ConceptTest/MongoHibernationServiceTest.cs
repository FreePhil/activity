using System;
using System.Threading.Tasks;
using ActivityService.Injections;
using ActivityService.Models;
using ActivityService.Repositories;
using ActivityService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConceptTest
{
    public class MongoHibernationServiceTest
    {
        private ServiceProvider Injector { get; }
        public MongoHibernationServiceTest()
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
        public async Task TestHibernationServerUpdateOnTheSameStageInSucess()
        {
            IHibernationService hibernationService = Injector.GetService<IHibernationService>();
            
            var dormancy = await hibernationService.GetHibernationAsync("xxx", "ch", "00");
            StagePayload stage = new StagePayload()
            {
                Name = "4",
                Payload = @"{""name"": ""john""}"
            };

            Hibernation updatedDormancy = null;
            var randomGenerator = new Random();
            string randomPayload = null;
            for (int i = 0; i < 10; i++)
            {
                randomPayload = $"{randomGenerator.Next(100): 00}";
                stage.Payload = randomPayload;
                updatedDormancy = await hibernationService.UpdateOnTheSameStageAsync(dormancy.Id, stage);
            }


            var target = await hibernationService.GetHibernationAsync(updatedDormancy?.Id);
            
            Assert.Equal("xxx", target.UserId);
            Assert.Equal("ch", target.SubjectName);
            Assert.Equal("00", target.ProductName);
            Assert.Equal("4", target.Stage.Name);
            Assert.Equal(randomPayload, target.Stage.Payload);
        }
        
        [Fact]
        public async Task TestHibernationCreate()
        {
            IHibernationService hibernationService = Injector.GetService<IHibernationService>();

            var dormancy = new Hibernation()
            {
                UserId = "xxz",
                SubjectName = "ch",
                ProductName = "00",
                Stage = new StagePayload()
                {
                    Name = "2",
                    Payload = @"{""name"": ""john""}"
                }
            };

            var createdDormancy = await hibernationService.CreateOrUpdateHibernationForwardAsync(dormancy);
            var target = await hibernationService.GetHibernationAsync(createdDormancy?.Id);
            
            Assert.Equal("xxy", target.UserId);
            Assert.Equal("ch", target.SubjectName);
            Assert.Equal("00", target.ProductName);
            Assert.Equal("2", target.Stage.Name);
        }

        [Fact]
        public async Task TestHibernationBackward()
        {
            IHibernationService hibernationService = Injector.GetService<IHibernationService>();

            var dormancy = new Hibernation()
            {
                UserId = "xxy",
                SubjectName = "ch",
                ProductName = "00",
                Stage = new StagePayload()
                {
                    Name = "1",
                    Payload = @"{""name"": ""tom""}"
                }
            };  
            var updatedDormancy = await hibernationService.CreateOrUpdateHibernationBackwardAsync(dormancy);
            var target = await hibernationService.GetHibernationAsync(updatedDormancy?.Id);
            
            Assert.Equal("xxy", target.UserId);
            Assert.Equal("ch", target.SubjectName);
            Assert.Equal("00", target.ProductName); 
            Assert.Equal("1", target.Stage.Name);
            
        }
        
        [Fact]
        public async Task TestHibernationPatchHistory()
        {
            IHibernationService hibernationService = Injector.GetService<IHibernationService>();

            var dormancy = await hibernationService.GetHibernationAsync("5d2dc557602ade946f5b65a1");
            var updatedDormancy = await hibernationService.UpdateHistoryOnTheSameStageAsync(
                dormancy.Id, new StagePayload() {Name = "3", Payload = "patching history 3"});
            updatedDormancy = await hibernationService.UpdateHistoryOnTheSameStageAsync(
                dormancy.Id, new StagePayload() {Name = "4", Payload = "patching history 4"});
            await hibernationService.UpdateHistoryOnTheSameStageAsync(
                dormancy.Id, new StagePayload() {Name = "5", Payload = "patching history 5"});
            var target = await hibernationService.GetHibernationAsync(updatedDormancy?.Id);

        }
    }
}