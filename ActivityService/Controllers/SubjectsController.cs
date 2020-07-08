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
    [EnableCors("RestrictedAccess")]
    [Route("api/subjects")]
    public class SubjectsController: ControllerBase
    {
        private ISubjectFetcherFactory fetcherFactory;

        public SubjectsController(ISubjectFetcherFactory fetcherFactory)
        {
            this.fetcherFactory = fetcherFactory;
        }

        [HttpGet("product-listing/{userId}")]
        public ActionResult<IList<Subject>> LoadSubjectDetail(string userId, [FromQuery(Name = "domain")] string domain)
        {
            var fetcher = fetcherFactory.Create(domain);
            var subjects = fetcher.Load(userId);

            return Ok(subjects);
        }

        [HttpDelete("cache")]
        public IActionResult RemoveCache([FromServices] IMemoryCache cache)
        {
            cache.Remove("education-level");
            cache.Remove("subjects-lookup");
            cache.Remove("products-lookup");
            cache.Remove("versions-lookup");
            
            Log.Information("cache removed");
            
            return NoContent();
        }
    }
}