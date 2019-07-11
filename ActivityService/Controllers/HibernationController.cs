using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [ApiController]
    [Route("api/hibernation")]
    public class HibernationController
    {
        public IHibernationService Service { get; }
        public HibernationController(IHibernationService service)
        {
            Service = service;
        }
        
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserActivity>> Get(string userId, 
            [FromQuery] string subjectName, [FromQuery] string productName, [FromQuery] int pageSize, [FromQuery] int pageNo)
        {
//            var result = await Service.GetActivityAsync(id);
//            return result;

            return null;
        }
    }
    
    
}