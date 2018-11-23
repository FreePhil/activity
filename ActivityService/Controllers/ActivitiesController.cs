using ActivityService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [Route("api/activities")]
    [ApiController]
    public class ActivitiesController
    {
    
        [HttpGet("{id}")]
        public ActionResult<UserActivity> Get(string id)
        {
            return null;
        }
    }
}