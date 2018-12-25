using ActivityService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace ActivityService.Injections
{
    public static class MongoDbInjection
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<MongoDbOptions>(configuration.GetSection("MongoDB"));

            // Typically you only create one MongoClient instance for a given cluster and use it across your application.
            // See: http://mongodb.github.io/mongo-csharp-driver/2.7/getting_started/quick_tour/
            serviceCollection.AddSingleton(provider =>
            {
                var optionAccessor = provider.GetService<IOptionsMonitor<MongoDbOptions>>();
                var options = optionAccessor.CurrentValue;
                var connectionString = $"mongodb://{options.Hosts}";

                Log.Information("Creating MongoClient using connection string: {connectionString}", connectionString);

                return new MongoClient(connectionString);
            });

            return serviceCollection;
        }
    }
}
