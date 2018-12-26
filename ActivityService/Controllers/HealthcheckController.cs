using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [Obsolete("Will be replace by 2.2 features")]
    [Route("api/[controller]")]
    public class HealthcheckController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }
    }
}