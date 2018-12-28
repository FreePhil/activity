using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [Obsolete("Will be replace after Hanin ID establshed")]
    [Route("api/login")]
    public class SimpleLoginController: Controller
    {
        public ISimpleUserService Service { get; }
        public SimpleLoginController(ISimpleUserService service)
        {
            Service = service;
        }
        
        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody] string userName)
        {
            SimpleUser user = await Service.LoginAsync(userName);
            return user.Id;
        }
    }
}