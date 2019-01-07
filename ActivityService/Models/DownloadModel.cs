using Newtonsoft.Json;

namespace ActivityService.Models
{
    public class DownloadModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}