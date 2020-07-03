using System.Collections.Generic;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [ApiController]
    [EnableCors("OpenAccess")]
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
    }
}