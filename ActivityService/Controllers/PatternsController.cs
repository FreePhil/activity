using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [ApiController]
    [Route("api/patterns")]
    [EnableCors(nameof(AccessScope.RestrictedAccess))]
    public class PatternsController: ControllerBase
    {
        private IPatternService Service { get; }
        public PatternsController(IPatternService service)
        {
            Service = service;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionPattern>> Get(string id)
        {
            var result = await Service.GetPatternAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<QuestionPattern>> Create(QuestionPattern pattern)
        {
            await Service.CreatePatternAsync(pattern);
            return CreatedAtAction(nameof(Get), new {id = pattern.Id}, pattern);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var sucessfull = await Service.DeletePatternAsync(id);
            if (!sucessfull)
            {
                return BadRequest();
            }
            return NoContent();
        }
        
        [HttpGet("subject")]
        public async Task<ActionResult<List<QuestionPattern>>> GetAll(
            [FromQuery(Name = "user_id")] string userId, [FromQuery(Name = "subject")] string subjectName, [FromQuery(Name = "product")] string productName)
        {
            var result = await Service.GetPatternsWithPublicAsync(userId, subjectName, productName);
            if (result?.Count == 0)
            {
                return NoContent();
            }

            return Ok(result?.ToList());
        }
    }
}