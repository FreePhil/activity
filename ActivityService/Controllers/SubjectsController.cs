using System.Collections.Generic;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        public ActionResult<IList<Subject>> LoadSubjectDetail(string userId, [FromQuery(Name = "domain")] string domain)
        {
            var subjectsOfAllLevels = subjectService.GetProductListing(userId, domain);
            return Ok(subjectsOfAllLevels);
        }

        [HttpDelete("cache")]
        public IActionResult RemoveCache([FromServices] IMemoryCache cache)
        {
            cache.Remove("education-level");
            cache.Remove("subjects-lookup");
            cache.Remove("products-lookup");
            
            Log.Information("cache removed");
            
            return NoContent();
        }
    }
}