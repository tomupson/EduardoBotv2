using Newtonsoft.Json;

namespace EduardoBotv2.Core.Models.Games.Pokemon
{
    public class PokemonSprite
    {
        [JsonProperty("back_female")]
        public string BackFemaleSpriteUrl { get; set; }

        [JsonProperty("back_shiny_female")]
        public string BackShinyFemaleSpriteUrl { get; set; }

        [JsonProperty("back_default")]
        public string BackDefaultUrl { get; set; }

        [JsonProperty("front_female")]
        public string FrontFemaleSpriteUrl { get; set; }

        [JsonProperty("front_shiny_female")]
        public string FrontShinyFemaleSpriteUrl { get; set; }

        [JsonProperty("back_shiny")]
        public string BackShinySpriteUrl { get; set; }

        [JsonProperty("front_default")]
        public string FrontDefaultSpriteUrl { get; set; }

        [JsonProperty("front_shiny")]
        public string FrontShinySpriteUrl { get; set; }
    }
}