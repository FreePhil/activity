using MongoDB.Bson.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ActivityService.Models
{
    public class ExportJobModel
    {
        [JsonProperty("testId")]
        public string TestId { get; set; }
        [JsonProperty("exportJobId")]
        public string ExportJobId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}