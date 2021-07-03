using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using ActivityService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace ActivityService.Controllers
{
    [ApiController]
    [EnableCors(nameof(AccessScope.RestrictedAccess))]
    [Route("api/subjects")]
    public class SubjectsController: ControllerBase
    {
        private ISubjectService subjectService;

        public SubjectsController(ISubjectService subjectService, ISubjectFetcherFactory fetcherFactory)
        {
            this.subjectService = subjectService;
        }

        [HttpGet("product-listing/{userId}")]
        public async Task<ActionResult<IList<Subject>>> LoadSubjectDetail(string userId,
            [FromQuery(Name = "v")] string version, [FromQuery(Name = "domain")] string domain)
        {
            IList<EducationLevel> subjectsOfAllLevels = new List<EducationLevel>();
            try
            {
                subjectsOfAllLevels = await Task.Run(() =>
                    subjectService.GetProductListing(version, userId, domain)).ConfigureAwait(false);
                Log.Information("listed products for {Version} of {Domain} by {UserId}", version, domain, userId);
            }
            catch (Exception e)
            {
                Log.Error("failed to list products for {Version} of {Domain} by {UserId}. {Message}", 
                    version, domain, userId, e.Message);
            }

            return Ok(subjectsOfAllLevels);
        }

        [HttpDelete("cache")]
        public IActionResult RemoveCache(
            [FromServices] IMemoryCache cache, [FromServices] IOptionsMonitor<JsonLocationOptions> configAccessor)
        {
            var config = configAccessor.CurrentValue;
            
            cache.Remove(config.CacheName.TestGoVersionCacheName);
            cache.Set(config.CacheName.EducationLevel, new Dictionary<string, IList<EducationLevel>>());
            
            Log.Information("cache removed");
            return NoContent();
        }
    }
}