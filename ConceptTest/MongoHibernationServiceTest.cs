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
                Payload = ""
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
    }
}