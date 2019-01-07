using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ActivityService.Models
{
    public class DownloadList
    {
        [JsonProperty("items")]
        public List<DownloadModel> Items { get; set; }
    }
}