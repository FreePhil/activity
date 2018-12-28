using System.IO;
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
        public ActivitiesController(IRepository<UserActivity> repository)
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
        [HttpPost("{id}/export")]
        public async Task<ActionResult<string>> UpdatePayload(string id, [FromServices] IPayloadValidator validator)
        {
            string callbackHref = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host}{Url.RouteUrl("status", new {id })}";

            string payloadString = await ReadFromBodyAsync();
            dynamic payload = JsonConvert.DeserializeObject(payloadString);

            if (!validator.IsValid())
            {
                return "blablabla...";
            }

            payload.callbackHref = callbackHref;
            
            return JsonConvert.SerializeObject(payload);
        }
        
        [HttpPost("{id}/status", Name = "status")]
        public async Task<ActionResult<bool>> UpdateStatus(string id, [FromBody] string status)
        {
            var result = await Repository.UpdateAsync(id, ac => ac.Status, status);

            return result;
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