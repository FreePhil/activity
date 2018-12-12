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
            //services.AddSingleton<IBus>(RabbitHutch.CreateBus("host=localhost"));
            //services.AddSingleton<ITopic, Topic>();
            services.AddScoped<IContext, ActivityContext>(provider =>
            {
                var optionAccessor = provider.GetService<IOptionsMonitor<MongoDbOptions>>();
                var options = optionAccessor.CurrentValue;

                var client = provider.GetService<MongoClient>();

                Log.Information("Obtaining database {database}", options.Database);

                var database = client.GetDatabase(options.Database);

                return new ActivityContext(database);
            });

            services.AddScoped<IRepository, ActivityRepository>();

            return services;
        }
    }
}