using Newtonsoft.Json;

namespace EduardoBotv2.Core.Models.Pokemon
{
    public class Pokemon
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sprites")]
        public PokemonSprite Sprites { get; set; }
    }
}