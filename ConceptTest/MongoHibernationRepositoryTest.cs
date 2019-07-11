using System;
using System.Threading.Tasks;
using ActivityService.Injections;
using ActivityService.Models;
using ActivityService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConceptTest
{
    public class MongoHibernationRepositoryTest
    {
        private ServiceProvider Injector { get; }
        public MongoHibernationRepositoryTest()
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
        public async Task TestHibernationCreationForNew()
        {
            Random random = new Random();
            int product = random.Next(1000);  
            
            IHibernationRepository hibernationRepository = Injector.GetService<IHibernationRepository>();

            var dormancy = await hibernationRepository.CreateOrUpdateAsync("xxx", "ch", $"{product:000}", new StagePayload()
            {
                Name = "1",
                Payload = "{\"a\": 12}",
                History = null
            });

            var target = await hibernationRepository.GetAsync(dormancy.Id);
            
            Assert.Equal("xxx", target.UserId);
            Assert.Equal("ch", target.SubjectName);
            Assert.Equal($"{product:000}", target.ProductName);
            Assert.Equal("1", target.Stage.Name);

            await hibernationRepository.DeleteAsync(dormancy.Id);
        }
        
        [Fact]
        public async Task TestHibernationCreationForUpdate()
        {
            IHibernationRepository hibernationRepository = Injector.GetService<IHibernationRepository>();

            var dormancy = await hibernationRepository.CreateOrUpdateAsync("xxx", "ch", "00", new StagePayload()
            {
                Name = "4",
                Payload = "1234",
                History = new StagePayload()
                {
                    Name = "3",
                    Payload = "123",
                    History = new StagePayload()
                    {
                        Name = "2",
                        Payload = "12",
                        History = null
                    }
                }
            });

            var target = await hibernationRepository.GetAsync(dormancy.Id);
            
            Assert.Equal("xxx", target.UserId);
            Assert.Equal("ch", target.SubjectName);
            Assert.Equal("00", target.ProductName);
            Assert.Equal("4", target.Stage.Name);
            Assert.Equal("2", target.Stage.History.History.Name);
            Assert.Null(target.Stage.History.History.History);
        }
        
        [Fact]
        public async Task TestHibernationCreationForUpdateWithModel()
        {
            IHibernationRepository hibernationRepository = Injector.GetService<IHibernationRepository>();

            var dormancy = new Hibernation()
            {
                UserId = "xxx",
                SubjectName = "ch",
                ProductName = "00",
                Stage = new StagePayload()
                {
                    Name = "4",
                    Payload = "1234",
                    History = new StagePayload()
                    {
                        Name = "3",
                        Payload = "123",
                        History = new StagePayload()
                        {
                            Name = "2",
                            Payload = "12",
                            History = null
                        }
                    }
                }
            };

            var updatedDormancy = await hibernationRepository.CreateOrUpdateAsync(dormancy);

            var target = await hibernationRepository.GetAsync(updatedDormancy.Id);
            
            Assert.Equal("xxx", target.UserId);
            Assert.Equal("ch", target.SubjectName);
            Assert.Equal("00", target.ProductName);
            Assert.Equal("4", target.Stage.Name);
            Assert.Equal("2", target.Stage.History.History.Name);
            Assert.Null(target.Stage.History.History.History);
        }
    }
}