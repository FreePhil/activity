using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ActivityService.Controllers
{
    [Route("api/activities")]
    public class ActivitiesController: Controller
    {
        public IRepository<UserActivity> Repository { get; }
        public IUserActivityService Service { get; }
        public ActivitiesController(IRepository<UserActivity> repository, IUserActivityService service)
        {
            Repository = repository;
            Service = service;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> Get(string id)
        {
            var result = await Service.GetActivityAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Add([FromBody] UserActivity activity)
        {
            await Service.AddActivityAsync(activity);
            return Created(nameof(Get), activity.Id);
        }
      
        [HttpPost("{id}/status", Name = "status")]
        public async Task<ActionResult<bool>> UpdateStatus(string id, [FromBody] string status)
        {
            var result = await Service.UpdateStatusAsync(id, status);

            return Ok(result);
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IList<UserActivity>>> GetByUser(string userId)
        {
            var activities = await Service.GetByUserAsync(userId);
            return Ok(activities.ToList());
        }
        
                
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<string>> Export(string userId, [FromServices] IHttpClientFactory clientFactory)
        {
            string payloadString = await ReadFromBodyAsync();
            dynamic payload = JsonConvert.DeserializeObject(payloadString);

            var activity = new UserActivity
            {
                UserId = userId,
                Payload = payloadString
            };
            
            // save to activity db
            //
            await Service.AddActivityAsync(activity);
            
            // decorate for export service
            //
            payload.testSpec.testId = activity.Id;
            string callbackUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host}{Url.RouteUrl("status", new { activity.Id })}";
            payload.callback = new { onJobFinish = callbackUrl};
            string relayedPayload = JsonConvert.SerializeObject(payload);

            // call export api
            //
            var client = clientFactory.CreateClient();
            var message = await client.PostAsync("", new StringContent(relayedPayload, Encoding.UTF8, "application/json"));
            var jsonString = await message.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ExportJobModel>(jsonString);

            // update calling result
            //
            await Service.UpdateStatusAsync(response.TestId, response.Status);
            
            return Created(nameof(Get), activity.Id);
        }

        private async Task<string> ReadFromBodyAsync()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                return await reader.ReadToEndAsync();
            }
        }
    }
}