using System.Collections.Generic;
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
        public ActionResult<IList<Subject>> LoadSubjectDetail(string userId,
            [FromQuery(Name = "v")] string version, [FromQuery(Name = "domain")] string domain)
        {
            var subjectsOfAllLevels = subjectService.GetProductListing(version, userId, domain);
            return Ok(subjectsOfAllLevels);
        }

        [HttpDelete("cache")]
        public IActionResult RemoveCache(
            [FromServices] IMemoryCache cache, [FromServices] IOptionsMonitor<JsonLocationOptions> configAccessor)
        {
            var config = configAccessor.CurrentValue;
            
            cache.Remove(config.CacheName.VersionCacheName);
            cache.Remove(config.CacheName.EducationLevel);

            Log.Information("cache removed");
            
            return NoContent();
        }
    }
}