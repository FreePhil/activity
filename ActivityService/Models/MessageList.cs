using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityService.Models
{
    public class MessageList
    {
        [JsonProperty("items")]
        public List<string> Items { get; set; }
    }
}