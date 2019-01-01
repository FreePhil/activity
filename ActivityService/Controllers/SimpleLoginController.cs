using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [Obsolete("Will be replace after Hanin ID establshed")]
    [Route("api/login")]
    public class SimpleLoginController: ControllerBase
    {
        public ISimpleUserService Service { get; }
        public SimpleLoginController(ISimpleUserService service)
        {
            Service = service;
        }
        
        [HttpPost]
        public async Task<ActionResult<string>> Login([FromForm] string userName)
        {
            SimpleUser user = await Service.LoginAsync(userName);

            if (user == null)
            {
                return BadRequest($"{userName} login failed");
            }
            return user.Id;
        }
    }
}