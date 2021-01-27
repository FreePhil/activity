using System;
using System.Collections.Generic;
using ActivityService.Models.Options;
using ActivityService.Repositories;
using ActivityService.Services;
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
            services.AddScoped<IHibernationRepository, HibernationRepository>();
            services.AddScoped<IPatternRepository, PatternRepository>();
            
            services.AddScoped<ISimpleUserService, SimpleUserService>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<IHibernationService, HibernationService>();
            services.AddScoped<IPatternService, PatternService>();
            
            services.AddScoped<IPayloadValidator, PayloadValidator>();

            services.AddTransient<ISubjectFetcherFactory, SubjectFetcherFactory>();
            services.AddTransient<ISubjectService, SubjectService>();
            services.AddTransient<TestGoSubjectFetcher>();
            services.AddTransient<EduSubjectFetcher>();
            services.AddTransient<ICacheLoader, EduCacheLoader>();
            services.AddTransient<ILookupCacheLoader, LookupCacheLoader>();
            services.AddTransient<ICacheFiller, SubjectCacheFiller>();

            services.AddHttpClient();
            
            return services;
        }
    }
}