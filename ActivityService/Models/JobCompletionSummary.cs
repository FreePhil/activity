using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityService.Models
{
    public class JobCompletionSummary
    {
        [JsonProperty("testId")]
        public string TestId { get; set; }
        
        [JsonProperty("exportJobId")]
        public string ExportJobId { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("manifest")]
        public List<DownloadModel> Manifest { get; set; }
        
        [JsonProperty("messages")]
        public string[] Messages { get; set; }
    }
}