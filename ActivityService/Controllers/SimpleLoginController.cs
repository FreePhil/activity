using System;
using System.Threading.Tasks;
using ActivityService.Models;
using ActivityService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [Obsolete("Will be replace after Hanin ID establshed")]
    [Route("api/login")]
    public class SimpleLoginController: Controller
    {
        public IRepository Repository { get; }
        public SimpleLoginController(IRepository repository)
        {
            Repository = repository;
        }
        
        [HttpPost]
        public async Task<ActionResult<string>> Add([FromBody] string userLoginName)
        {
            return String.Empty;
        }
    }
}