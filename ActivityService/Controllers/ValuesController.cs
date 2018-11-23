using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityService.Services;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace ActivityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ITopic topic;
        public ValuesController(ITopic topic)
        {
            this.topic = topic;
        }
        
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            topic.Bus.Publish("message", topic.Name);
            return new string[] {"value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}