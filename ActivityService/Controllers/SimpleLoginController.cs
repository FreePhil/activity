using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ActivityService.Controllers
{
    [Obsolete("Will be replace after Hanin ID establshed")]
    [ApiController]
    [Route("api/login")]
    public class SimpleLoginController: ControllerBase
    {
        public ISimpleUserService Service { get; }
        public SimpleLoginController(ISimpleUserService service)
        {
            Service = service;
        }
        
        [HttpPost]
        public async Task<ActionResult<SimpleUser>> Login([FromBody] LoginUser login)
        {
            SimpleUser user = await Service.LoginAsync(login.UserName);

            if (user == null)
            {
                dynamic msg = new JObject();
                msg.errorInfo = $"{login.UserName} login failed: internal error";
                return BadRequest(msg);
            }
            return user;
        }
    }
}