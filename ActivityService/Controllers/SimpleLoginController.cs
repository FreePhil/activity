using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;

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
                return BadRequest($"{login.UserName} login failed");
            }
            return user;
        }
    }
}