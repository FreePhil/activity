using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Models.Options;
using ActivityService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;

namespace ActivityService.Controllers
{
    [ApiController]
    [Route("api/activities")]
    public class ActivitiesController: ControllerBase
    {
        public IUserActivityService Service { get; }
        public IHibernationService HibernationService { get;  }
        public ActivitiesController(IUserActivityService service, IHibernationService hibernateService)
        {
            Service = service;
            HibernationService = hibernateService;
        }

        [EnableCors("RestrictedAccess")]
        [HttpGet("cors-test")]
        public IActionResult Get()
        {
            Log.Information("Access sucessfully");
            return Ok();
        }
        
        [EnableCors("RestrictedAccess")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> Get(string id)
        {
            Log.Information("Find activity by {activityId}", id);
            var result = await Service.GetActivityAsync(id);
            return result;
        }

        [EnableCors("RestrictedAccess")]
        [HttpGet("{id}/hibernation")]
        public async Task<ActionResult<Hibernation>> GetHibernation(string id)
        {
            var result = await Service.GetActivityAsync(id);
            if (result == null)
            {
                return null;
            }
            var dormancy = JsonConvert.DeserializeObject<Hibernation>(result.Hibernation);
            return dormancy;
        }

        [EnableCors("RestrictedAccess")]
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
        
        [EnableCors("OpenAcess")]
        [HttpOptions("{id}/users/{userId}")]
        public IActionResult PreflightRoute(string id, string userId)
        {
            return NoContent();
        }

        [EnableCors("OpenAcess")]
        [HttpDelete("{id}/users/{userId}")]
        public async Task<ActionResult<object>> Delete(string id, string userId)
        {
            Log.Information("Delete activity record {id} for user {userId}", id, userId);
            var result = await Service.DeleteActivityAsync(id, userId);
            
            // wrap returning json
            //
            dynamic info = new JObject();
            info.result = result;
            
            return info;
        }
        
        [EnableCors("OpenAcess")]
        [HttpOptions("users/{userId}")]
        public IActionResult PreflightRoute(string userId)
        {
            return NoContent();
        }

        [EnableCors("OpenAcess")]
        [HttpDelete("users/{userId}")]
        public async Task<ActionResult<object>> DeleteActivities([FromBody] IList<string> ids, string userId)
        {
            string idsString = ids.Aggregate((i, j) => i + "," + j);
            Log.Information("Delete activity records {idsString} for user {userId}", idsString, userId);
            var result = await Service.DeleteActivitiesAsync(ids, userId);
            
            // wrap returning json
            //
            dynamic info = new JObject();
            info.result = result;
            return info;
        }
      
        [EnableCors("OpenAcess")]
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
        
        [EnableCors("RestrictedAccess")]
        [HttpGet("subject/{userId}/count")]
        public async Task<ActionResult<long>> GetDocumentCountBySubject(string userId, 
            [FromQuery(Name = "subject")] string subjectName, [FromQuery(Name = "product")] string productName)
        {
            return await Service.GetActivitiCountBySubjectAsync(userId, subjectName, productName);
        }

        [EnableCors("RestrictedAccess")]
        [HttpGet("subject/{userId}/paging")]
        public async Task<ActionResult<IList<UserActivity>>> GetBySubject(string userId, 
            [FromQuery(Name = "subject")] string subjectName, [FromQuery(Name = "product")] string productName, 
            [FromQuery(Name = "page_no")] int pageNo, [FromQuery(Name = "page_size")] int pageSize)
        {
            var activities = await Service.GetActivitiesPagingBySubjectAsync(userId, subjectName, productName, pageNo, pageSize);
            return activities.ToList();
        }
        
        [EnableCors("RestrictedAccess")]
        [HttpGet("user/{userId}/count")]
        public async Task<ActionResult<long>> GetDocumentCountByUser(string userId)
        {
            return await Service.GetActivityCountByUserAsync(userId);
        }

        [EnableCors("RestrictedAccess")]
        [HttpGet("user/{userId}/paging")]
        public async Task<ActionResult<IList<UserActivity>>> GetByUser(string userId, 
            [FromQuery(Name = "page_no")] int pageNo, [FromQuery(Name = "page_size")] int pageSize)
        {
            var activities = await Service.GetActivitiesPagingByUserAsync(userId, pageNo, pageSize);
            return activities.ToList();    
        }
        
        [EnableCors("RestrictedAccess")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IList<UserActivity>>> GetByUser(string userId)
        {
            var activities = await Service.GetActivitiesByUserAsync(userId);
            return activities.ToList();
        }
                
        [EnableCors("RestrictedAccess")]
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
            string callbackUrl = linkGenerator.GetUriByRouteValues(HttpContext, "status", new {activity.Id}, "https"); 
            Log.Debug("Callback url: {url}", callbackUrl);
            
            var extract = InjectPayload(rawPayload, activity.Id, callbackUrl);
            var payload = extract.PayloadString;
            
            // call export api
            //
            bool isExportFailed = false;
            Exception error = null;
            Log.Information("Exporting job {JobId} for user {UserId}", activity.Id, userId);
            try
            {
                var client = clientFactory.CreateClient();
                var message = await client.PostAsync($"{exporter.Host}/{exporter.EndPoint}",
                    new StringContent(payload, Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                isExportFailed = true;
                error = e;
                Log.Warning("exporting job {JobId} of {UserId} failed with message {Message}", activity.Id, userId, e.Message);    
            }

            var dormancy = await HibernationService.GetHibernationAsync(userId, extract.SubjectName, extract.ProductName);
            Log.Information("Get hibernation with subject: {subject}, product: {product} and userid: {userId}", extract.SubjectName, extract.ProductName, userId);
            if (dormancy == null)
            {
                Log.Warning("No such hibernation exists");
            }
            
            string dormancyString = (dormancy == null? null: JsonConvert.SerializeObject(dormancy));
            var updatingJob = new UpdateExportedModel
            {
                TestName = extract.TestName,
                Volume = extract.Volume,
                SubjectName = extract.SubjectName,
                ProductName = extract.ProductName,
                SubjectId = extract.SubjectId,
                ProductId = extract.ProductId,
                Hibernation = dormancyString
            };
            if (isExportFailed)
            {
                updatingJob.Status = "failed";
            }
            else
            {
                updatingJob.Status = "accepted";
            }

            // update calling result
            //
            Log.Information("Updating job {JobId} for user {UserId} for activity repository", activity.Id, userId);
            await Service.UpdateExportedAsync(activity.Id, updatingJob);

            if (isExportFailed)
            {
                throw error;
            }
            else
            {
                // wrap returning json
                //
                dynamic idObject = new JObject();
                idObject.activityId = activity.Id;

                return CreatedAtAction(nameof(Get), new {id = activity.Id}, idObject);
            }
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
            payload.callbacks = new JObject();
            payload.callbacks.onJobFinish = callbackUrl;
            
            // update activity from payload
            //
            return new DynamicPayloadRetrieval
            {
                TestName = payload.testSpec.metadata.testName,
                SubjectName = payload.testSpec.metadata.subjectName,
                ProductName = payload.testSpec.metadata.productName,
                SubjectId = payload.testSpec.metadata.subjectId,
                ProductId = payload.testSpec.metadata.productId,
                Volume = payload.testSpec.metadata.volume,
                PayloadString = JsonConvert.SerializeObject(payload)
            };
        }
    }
}