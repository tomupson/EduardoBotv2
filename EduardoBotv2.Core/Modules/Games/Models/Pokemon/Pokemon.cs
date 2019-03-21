using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.Games.Models.Pokemon
{
    public class Pokemon : PokemonSummary
    {
        [JsonProperty("sprites")]
        public PokemonSprite Sprites { get; set; }
    }
}