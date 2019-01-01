using Newtonsoft.Json;

namespace ActivityService.Models
{
    public class CallbackModel
    {
        [JsonProperty("onJobFinish")]
        public string OnJobFinish { get; set; }
    }
}