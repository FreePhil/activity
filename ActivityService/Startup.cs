using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ActivityService.Injections;
using ActivityService.Models;
using ActivityService.Models.Options;
using ActivityService.Repositories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

// some comments 
//
[assembly: ApiController]
namespace ActivityService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddMemoryCache();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.Configure<JsonLocationOptions>(Configuration.GetSection("JsonLocation"));
            
            services.AddSwaggerDocument();

            services.AddMongoDb(Configuration);
            services.AddActivity();

            services.AddHealthChecks();
            services
                .AddCors(o =>
                {
                    o.AddPolicy(nameof(AccessScope.RestrictedAccess), builder =>
                    {
                        builder
                            .WithOrigins(new string[]
                            {
                                "https://testbank.hle.com.tw",
                                "https://qa-testbank.hle.com.tw",
                                "http://localhost:8080",
                                "http://localhost:8081"
                            })
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                    o.AddPolicy(nameof(AccessScope.OpenAccess), builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
        }

        private void CheckOrBuildIndexes(IApplicationBuilder app)
        {
            // Mongodb use createIndex to ensure index existing, otherwise create one.
            //
            IUserActivityRepository activityRepository = app.ApplicationServices.GetService<IUserActivityRepository>();
            activityRepository.CreateIndex();
            Log.Information("index of table 'activities' checked");
            
            ISimpleUserRepository userRepository = app.ApplicationServices.GetService<ISimpleUserRepository>();
            userRepository.CreateIndex();    
            Log.Information("index of table 'users' checked");
            
            IHibernationRepository hibernationRepository = app.ApplicationServices.GetService<IHibernationRepository>();
            hibernationRepository.CreateIndex();    
            Log.Information("index of table 'hibernation' checked");
            
            IPatternRepository patternRepository = app.ApplicationServices.GetService<IPatternRepository>();
            patternRepository.CreateIndex();    
            Log.Information("index of table 'patterns' checked");
        }

        private void InitializeCache(IApplicationBuilder app)
        {
            var cache = app.ApplicationServices.GetService<IMemoryCache>();
            var option = app.ApplicationServices.GetService<IOptionsMonitor<JsonLocationOptions>>();

            string educationLevelCacheName = option.CurrentValue.CacheName.EducationLevel;
            cache.Set(educationLevelCacheName, new Dictionary<string, IList<EducationLevel>>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            CheckOrBuildIndexes(app);
            InitializeCache(app);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            app.UseSwagger(config => config.PostProcess = (document, request) =>
            {
                if (request.Headers.ContainsKey("X-External-Host"))
                {
                    // Change document server settings to public
                    document.Host = request.Headers["X-External-Host"].First();
                    document.BasePath = request.Headers["X-External-Path"].First();
                }
            });

            app.UseSwaggerUi3(config => config.TransformToExternalPath = (internalUiRoute, request) =>
            {
                // The header X-External-Path is set in the nginx.conf file
                var externalPath = request.Headers.ContainsKey("X-External-Path") ? request.Headers["X-External-Path"].First() : "";
                return externalPath + internalUiRoute;
            });

            app.UseMvc();
            app.UseHealthChecks("/api/healthcheck");
        }
    }
}