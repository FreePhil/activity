using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ActivityService.Controllers
{
    [Route("api/activities")]
    [ApiController]
    public class ActivitiesController
    {
        public IRepository Repository { get; }
        public ActivitiesController(IRepository repository)
        {
            this.Repository = repository;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> Get(string id)
        {
            var result = await Repository.GetAsync(ObjectId.Parse(id));
            return result;
        }
    }
}