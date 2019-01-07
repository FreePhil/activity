using System.Collections.Generic;
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
        
        [JsonProperty("manifest")]
        public DownloadList Manifest { get; set; }
        
        [JsonProperty("messages")]
        public MessageList Messages { get; set; }
    }
}