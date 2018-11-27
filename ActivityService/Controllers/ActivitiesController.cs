using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;
using ActivityService.Services;
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
            Repository = repository;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> Get(string id)
        {
            var result = await Repository.GetAsync(id);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Add([FromBody] UserActivity activity)
        {
            await Repository.AddAsync(activity);
            return activity.Id;
        }

        [HttpPost("{id}/option")]
        public async Task<ActionResult<bool>> UpdateOption(string id, [FromBody] string option)
        {
            var result = await Repository.UpdateAsync(id, ac => ac.Option, option);

            return result;
        }
        
        [HttpPost("{id}/payload")]
        public async Task<ActionResult<bool>> UpdatePayload(string id, [FromBody] string payload)
        {
            var result = await Repository.UpdateAsync(id, ac => ac.Payload, payload);

            return result;
        }
        
        [HttpPost("{id}/status")]
        public async Task<ActionResult<bool>> UpdateStatus(string id, [FromBody] string status)
        {
            var result = await Repository.UpdateAsync(id, ac => ac.Status, status);

            return result;
        }
    }
}