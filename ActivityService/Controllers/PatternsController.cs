using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [ApiController]
    [Route("api/patterns")]
    public class PatternsController: ControllerBase
    {
        public IPatternService Service { get; }
        public PatternsController(IPatternService service)
        {
            Service = service;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionPattern>> Get(string id)
        {
            var result = await Service.GetPatternAsync(id);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Create(QuestionPattern pattern)
        {
            await Service.CreatePatternAsync(pattern);
            return Ok();
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await Service.DeletePatternAsync(id);
            return Ok();
        }
        
        [HttpGet("subject")]
        public async Task<ActionResult<List<QuestionPattern>>> GetAll(
            [FromQuery(Name = "user_id")] string userId, [FromQuery(Name = "subject")] string subjectName, [FromQuery(Name = "product")] string productName)
        {
            var result = await Service.GetPatternsWithPublicAsync(userId, subjectName, productName);
            return result.ToList();
        }
    }
}