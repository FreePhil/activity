using System.IO;
using System.Text;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Newtonsoft.Json;

namespace ActivityService.Controllers
{
    [ApiController]
    [Route("api/hibernation")]
    public class HibernationController: ControllerBase
    {
        public IHibernationService Service { get; }

        public HibernationController(IHibernationService service)
        {
            Service = service;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<Hibernation>> Get(string userId, [FromQuery(Name = "subject")] string subjectName,
            [FromQuery(Name = "product")] string productName)
        {
            var result = await Service.GetHibernationAsync(userId, subjectName, productName);

            return result;
        }

        [HttpPost("forward")]
        public async Task<ActionResult<Hibernation>> CreateOrUpdateHibernationForward(FlatHibernation flatDomancy)
        {
            var dormancy = new Hibernation()
            {
                SubjectName = flatDomancy.SubjectName,
                ProductName = flatDomancy.ProductName,
                UserId = flatDomancy.UserId,
                Stage = new StagePayload()
                {
                    Name = flatDomancy.Name,
                    Payload = flatDomancy.Payload
                }
            };
            
            return await Service.CreateOrUpdateHibernationForwardAsync(dormancy);
        }
        
        [HttpPost("backward")]
        public async Task<ActionResult<Hibernation>> CreateOrUpdateHibernationBackward(FlatHibernation flatDomancy)
        {
            var dormancy = new Hibernation()
            {
                SubjectName = flatDomancy.SubjectName,
                ProductName = flatDomancy.ProductName,
                UserId = flatDomancy.UserId,
                Stage = new StagePayload()
                {
                    Name = flatDomancy.Name,
                    Payload = flatDomancy.Payload
                }
            };
            
            return await Service.CreateOrUpdateHibernationBackwardAsync(dormancy);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Hibernation>> UpdateHibernationOnTheSameStage(string id, StagePayload stage)
        {
            return await Service.UpdateOnTheSameStageAsync(id, stage);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHibernation(string id)
        {
            await Service.DeleteHibernationAsync(id);
            return Ok();
        }
    }
}