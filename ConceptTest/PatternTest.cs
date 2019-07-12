using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Injections;
using ActivityService.Models;
using ActivityService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConceptTest
{
    public class PatternTest
    {
        private ServiceProvider Injector { get; }
        public PatternTest()
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
        public async Task TestCreateObject()
        {
            IPatternRepository repository = Injector.GetService<IPatternRepository>();

            QuestionPattern pattern = new QuestionPattern()
            {
                PatternName = "a",
                SubjectName = "b",
                ProductName = "c",
                Items = new List<PatternItem>()
                {
                    new PatternItem()
                    {
                        QuestionType = "t1",
                        QuestionNumber = 2,
                        AnsweringNumber = 3
                    }
                }
            };

            await repository.AddAsync(pattern);

            var stored = await repository.GetAsync(pattern.Id);
            
            Assert.Equal(pattern.PatternName, stored.PatternName);
        }

        [Fact]
        public async void GetWithPublicPattern()
        {
            IPatternRepository repository = Injector.GetService<IPatternRepository>();

            var patterns = await repository.GetAllWithPublicAsync("aa", "s1", "p1");
            
            Assert.Equal(4, patterns.Count);
            Assert.Null(patterns[0].UserId);
        }

        [Fact]
        public async void TestPatternDeletion()
        {
            IPatternRepository repository = Injector.GetService<IPatternRepository>();            
            QuestionPattern pattern = new QuestionPattern()
            {
                PatternName = "a",
                SubjectName = "b",
                ProductName = "c",
                Items = new List<PatternItem>()
                {
                    new PatternItem()
                    {
                        QuestionType = "t1",
                        QuestionNumber = 2,
                        AnsweringNumber = 3
                    }
                }
            };

            await repository.AddAsync(pattern);
            
            await repository.DeleteAsync(pattern.Id);
            var result = await repository.GetAsync(pattern.Id);
            Assert.Null(result);
        }
    }
}