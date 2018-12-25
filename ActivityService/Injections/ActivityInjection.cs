using ActivityService.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace ActivityService.Injections
{
    public static class ActivityInjection
    {
        public static IServiceCollection AddActivity(this IServiceCollection services)
        {
            services.AddScoped<IContext, ActivityContext>(provider =>
            {
                var optionAccessor = provider.GetService<IOptionsMonitor<MongoDbOptions>>();
                var options = optionAccessor.CurrentValue;
                
                Log.Information("Obtaining database {host}//{database}", options.Hosts, options.Database);

                var client = provider.GetService<MongoClient>();
                var database = client.GetDatabase(options.Database);

                return new ActivityContext(database);
            });

            services.AddScoped<IRepository, ActivityRepository>();

            return services;
        }
    }
}