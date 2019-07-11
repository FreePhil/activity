using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;

namespace ActivityService.Controllers
{
    [ApiController]
    [Route("api/hibernation")]
    public class HibernationController
    {
        public IHibernationService Service { get; }
        public HibernationController(IHibernationService service)
        {
            Service = service;
        }
        
        [HttpGet("{userId}")]
        public async Task<ActionResult<Hibernation>> Get(string userId, [FromQuery(Name="subject")] string subjectName, [FromQuery(Name="product")] string productName)
        {
            var result = await Service.GetHibernationAsync(userId, subjectName, productName);

            return result;
        }
        
        [HttpPost]
        public async Task<ActionResult<Hibernation>> CreateOrUpdateHibernation(Hibernation dormancy)
        {
            return await Service.CreateOrUpdateHibernationAsync(dormancy);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Hibernation>> UpdateHibernationOnTheSameStage(string id, StagePayload stage)
        {
            return await Service.UpdateOnTheSameStageAsync(id, stage);
        }
    }
    
    
}