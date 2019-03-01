using System.Collections.Generic;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Models
{
    public class List
    {
        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }
    }

    public class Urban
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("list")]
        public List<List> List { get; set; }
    }
}