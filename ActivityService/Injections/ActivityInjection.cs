using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActivityService.Models;
using ActivityService.Repositories;
using ActivityService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace ActivityService.Injections
{
    public static class ActivityInjection
    {
        public static IServiceCollection AddActivity(this IServiceCollection services)
        {
            services.AddScoped<IContext, ServiceContext>(provider =>
            {
                var mapper = provider.GetService<IDictionary<Type, string>>();
                var optionAccessor = provider.GetService<IOptionsMonitor<MongoDbOptions>>();
                var options = optionAccessor.CurrentValue;
                
                Log.Information("Obtaining database {host}:{database}", options.Hosts, options.Database);

                var client = provider.GetService<MongoClient>();
                var database = client.GetDatabase(options.Database);

                return new ServiceContext(database, mapper);
            });

            services.AddScoped(typeof(IRepository<>), typeof(ServiceRepository<>));
            services.AddScoped<IUserActivityRepository, UserActivityRepository>();
            services.AddScoped<ISimpleUserRepository, SimpleUserRepository>();
            services.AddScoped<ISimpleUserService, SimpleUserService>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            
            return services;
        }
    }
}