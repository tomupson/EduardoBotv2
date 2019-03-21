using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.General.Models
{
    public class UrbanList
    {
        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }
    }
}