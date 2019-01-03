using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using ActivityService.Repositories;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace ActivityService.Controllers
{
    [ApiController]
    [Route("api/activities")]
    public class ActivitiesController: ControllerBase
    {
        public IUserActivityService Service { get; }
        public ActivitiesController(IUserActivityService service)
        {
            Service = service;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> Get(string id)
        {
            var result = await Service.GetActivityAsync(id);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Add([FromBody] UserActivity activity)
        {
            await Service.AddActivityAsync(activity);
            return CreatedAtAction(nameof(Get), new {id = activity.Id}, activity.Id);
        }
      
        [HttpPost("{id}/status")]
        public async Task<ActionResult<bool>> UpdateStatus(string id, [FromBody] string status)
        {
            var result = await Service.UpdateStatusAsync(id, status);

            return result;
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IList<UserActivity>>> GetByUser(string userId)
        {
            var activities = await Service.GetByUserAsync(userId);
            return activities.ToList();
        }
        
                
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<string>> Export(string userId, [FromServices] IHttpClientFactory clientFactory, [FromServices] ExportModuleOptions exporter)
        {
            string rawPayload = await ReadFromBodyAsync();

            // save to activity db
            //
            var activity = new UserActivity
            {
                UserId = userId,
                Payload = rawPayload
            };
            await Service.AddActivityAsync(activity);
            
            // inject payload for export service
            //
            string callbackUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host}{Url.Action(nameof(UpdateStatus), new { activity.Id })}";
            string payload = InjectPayload(rawPayload, activity.Id, callbackUrl);

            // call export api
            //
            var client = clientFactory.CreateClient();
            
            var message = await client.PostAsync($"{exporter.Host}/{exporter.EndPoint}", new StringContent(payload, Encoding.UTF8, "application/json"));
            message.EnsureSuccessStatusCode();
            var transferredJob = await message.Content.ReadAsAsync<ExportJobModel>();

            // update calling result
            //
            await Service.UpdateStatusAsync(activity.Id, transferredJob.Status);
            
            return CreatedAtAction(nameof(Get), new {id = activity.Id}, activity.Id);
        }

        private async Task<string> ReadFromBodyAsync()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                return await reader.ReadToEndAsync();
            }
        }

        private string InjectPayload(string jsonString, string activityId, string callbackUrl)
        {
            dynamic payload = JsonConvert.DeserializeObject(jsonString);
            payload.testSpec.testId = activityId;
            payload.callback = new JObject();
            payload.callback.onJobFinish = callbackUrl;
            string relayedPayload = JsonConvert.SerializeObject(payload);
            
            return relayedPayload;
        }
    }
}