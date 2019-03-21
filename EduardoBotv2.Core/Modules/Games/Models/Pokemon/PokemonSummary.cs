using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.Games.Models.Pokemon
{
    public class PokemonSummary
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public PokemonSummary(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public PokemonSummary() { }
    }
}