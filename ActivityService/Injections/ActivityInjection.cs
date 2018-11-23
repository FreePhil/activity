using ActivityService.Services;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;

namespace ActivityService.Injections
{
    public static class ActivityInjection
    {
        public static IServiceCollection AddActivity(this IServiceCollection services)
        {
            services.AddSingleton<IBus>(RabbitHutch.CreateBus("host=localhost"));
            services.AddSingleton<ITopic, Topic>();
            return services;
        }
    }
}