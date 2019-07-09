using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

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
        public async Task<ActionResult<object>> Add([FromBody] UserActivity activity)
        {
            await Service.AddActivityAsync(activity);
            
            // wrap returning json
            //
            dynamic idObject = new JObject();
            idObject.activityId = activity.Id;
            
            return CreatedAtAction(nameof(Get), new {id = activity.Id}, idObject);
        }
      
        [HttpPost("{id}/status", Name = "status")]
        public async Task<ActionResult<object>> UpdateStatus(string id, [FromBody] JobCompletionSummary job)
        {
            Log.Information("Updating status for job {JobId}", job.TestId);
            var result = await Service.UpdateCallbackAsync(id, job);

            // wrap returning json
            //
            dynamic info = new JObject();
            info.result = result;
            
            return info;
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IList<UserActivity>>> GetByUser(string userId)
        {
            var activities = await Service.GetActivitiesByUserAsync(userId);
            return activities.ToList();
        }
        
                
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<object>> Export(string userId, 
            [FromServices] IHttpClientFactory clientFactory,
            [FromServices] LinkGenerator linkGenerator,
            [FromServices] ExportModuleOptions exporter)
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
            string callbackUrl = linkGenerator.GetUriByRouteValues(HttpContext, "status", new {activity.Id}); 
            var extract = InjectPayload(rawPayload, activity.Id, callbackUrl);
            var payload = extract.PayloadString;
            

            // call export api
            //
            Log.Information("Exporting job {JobId} for user {UserId}", activity.Id, userId);
            var client = clientFactory.CreateClient();
            var message = await client.PostAsync($"{exporter.Host}/{exporter.EndPoint}", new StringContent(payload, Encoding.UTF8, "application/json"));
            message.EnsureSuccessStatusCode();

            var updatingJob = new UpdateExportedModel
            {
                Status = "accepted", 
                TestName = extract.TestName,
                Volume = extract.Volume,
                SubjectName = extract.SubjectName
            };

            // update calling result
            //
            Log.Information("Updating job {JobId} for user {UserId} for activity repository", activity.Id, userId);
            await Service.UpdateExportedAsync(activity.Id, updatingJob);

            // wrap returning json
            //
            dynamic idObject = new JObject();
            idObject.activityId = activity.Id;
            
            return CreatedAtAction(nameof(Get), new {id = activity.Id}, idObject);
        }

        private async Task<string> ReadFromBodyAsync()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                return await reader.ReadToEndAsync();
            }
        }

        private DynamicPayloadRetrieval InjectPayload(string jsonString, string activityId, string callbackUrl)
        {
            // modify raw payload
            //
            dynamic payload = JsonConvert.DeserializeObject(jsonString);
            payload.testSpec.testId = activityId;
            payload.callback = new JObject();
            payload.callback.onJobFinish = callbackUrl;
            
            // update activity from payload
            //
            return new DynamicPayloadRetrieval
            {
                TestName = payload.testSpec.metadata.testName,
                SubjectName = payload.testSpec.metadata.subjectName,
                Volume = payload.testSpec.metadata.volume,
                PayloadString = JsonConvert.SerializeObject(payload)
            };
        }
    }
}