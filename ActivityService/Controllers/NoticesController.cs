using System;
using ActivityService.Models;
using ActivityService.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ActivityService.Controllers
{
    [Obsolete("Notify feature will be implemented by TestExport projects")]
    [Route("api/notices")]
    public class NoticesController: Controller
    {
        private ITopic topic;
        private IErrorLimit limit;
//        private IUserActivityService service;
        
        public NoticesController(ITopic topic, IErrorLimit limit)
        {
            this.topic = topic;
            this.limit = limit;
        }

        [HttpPost]
        public void Add([FromBody] JobNotice job)
        {
            job.ErrorCounter++;
            if (limit.RetryCounter > job.ErrorCounter)
            {
                string body = JsonConvert.SerializeObject(job);
                topic.Bus.Publish(body, topic.Name);   
            }
            else
            {
                // TODO: no more retry
                //
            }
        }
    }
}