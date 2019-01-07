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
using Microsoft.AspNetCore.Routing;
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
        public async Task<ActionResult<object>> UpdateStatus(string id, [FromBody] string status)
        {
            var result = await Service.UpdateStatusAsync(id, status);

            // wrap returning json
            //
            dynamic info = new JObject();
            info.result = result;
            
            return info;
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IList<UserActivity>>> GetByUser(string userId)
        {
            var activities = await Service.GetByUserAsync(userId);
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
            var client = clientFactory.CreateClient();
            var message = await client.PostAsync($"{exporter.Host}/{exporter.EndPoint}", new StringContent(payload, Encoding.UTF8, "application/json"));
            message.EnsureSuccessStatusCode();
            
            var transferredJob = await message.Content.ReadAsAsync<ExportJobModel>();
            var updatingJob = new UpdateExportedModel
            {
                Export = transferredJob,
                TestName = extract.TestName,
                SubjectName = extract.SubjectName
            };

            // update calling result
            //
            await Service.UpdateCallbackAsync(activity.Id, updatingJob);

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
            var retrieval = new DynamicPayloadRetrieval();
            
            // modify raw payload
            //
            dynamic payload = JsonConvert.DeserializeObject(jsonString);
            payload.testSpec.testId = activityId;
            payload.callback = new JObject();
            payload.callback.onJobFinish = callbackUrl;
            
            // update activity from payload
            //
            retrieval.TestName = payload.testSpec.heading.testName;
            retrieval.SubjectName = payload.testSpec.heading.subjectName;
            
            retrieval.PayloadString = JsonConvert.SerializeObject(payload);
            return retrieval;
        }
    }
}