using System.Collections.Generic;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.General.Models
{
    public class Urban
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("list")]
        public List<UrbanList> List { get; set; }
    }
}