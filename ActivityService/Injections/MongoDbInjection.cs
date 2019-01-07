using System;
using System.Collections.Generic;
using System.Linq;
using ActivityService.Models.Options;
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
            serviceCollection.Configure<MongoEntityOptions>(configuration.GetSection("MongoEntity"));
            serviceCollection.Configure<ExportModuleOptions>(configuration.GetSection("ExportModule"));

            // Typically you only create one MongoClient instance for a given cluster and use it across your application.
            // See: http://mongodb.github.io/mongo-csharp-driver/2.7/getting_started/quick_tour/
            serviceCollection.AddSingleton(provider =>
            {
                var optionAccessor = provider.GetService<IOptionsMonitor<MongoDbOptions>>();
                var options = optionAccessor.CurrentValue;
                var connectionString = $"mongodb://{options.Hosts}";

                Log.Information("Creating MongoClient using connection string: {connectionString}", connectionString);

                var client = new MongoClient(connectionString);
                
                Log.Information("Create client successfully");

                return client;
            });

            serviceCollection.AddSingleton(provider =>
            {
                var accessor = provider.GetService<IOptionsMonitor<MongoEntityOptions>>();
                var options = accessor.CurrentValue;
                
                IDictionary<Type, string> typeCollectionMapper = new Dictionary<Type, string>();
                options.Mappers.ToList().ForEach(pair =>
                {
                    try
                    {
                        typeCollectionMapper.Add(Type.GetType(pair.FullTypeName), pair.CollectionName);
                        Log.Information("Maping type {TypeName} to collection '{Collection}'", pair.FullTypeName, pair.CollectionName);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to convert type {TypeName} to {Collection}, {Message}", pair.FullTypeName, pair.CollectionName, e.Message);
                    }
                });

                return typeCollectionMapper;
            });

            serviceCollection.AddSingleton(provider =>
            {
                var accessor = provider.GetService<IOptionsMonitor<ExportModuleOptions>>();
                var exporter = accessor.CurrentValue;
                
                Log.Information("Export service endpoint {EndPoint} of host {Host}", exporter.EndPoint, exporter.Host);
                
                return exporter;
            });

            return serviceCollection;
        }
    }
}
