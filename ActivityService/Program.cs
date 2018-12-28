using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace ActivityService
{
    public class Program
    {
        /// <summary>
        /// References for Serilog configuration:
        ///     https://github.com/serilog/serilog-settings-configuration#serilogsettingsconfiguration
        ///     https://nblumhardt.com/2017/08/use-serilog/
        ///     https://nblumhardt.com/2016/03/reading-logger-configuration-from-appsettings-json/
        ///     https://github.com/serilog/serilog-aspnetcore/blob/dev/samples/SimpleWebSample/Program.cs
        ///     https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .UseSerilog()
                .ConfigureServices(services =>
                {
                    services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
                });

            Log.Information("Starting web host");
            webHostBuilder.Build().Run();
        }
    }
}