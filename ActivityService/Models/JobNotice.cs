using Newtonsoft.Json;

namespace ActivityService.Models
{
    public class JobNotice
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("errorCounter")]
        public int ErrorCounter { get; set; }
    }
}