using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Modules.Games.Models.Pokemon;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.Games.Helpers
{
    public static class PokemonHelper
    {
        public static async Task<Pokemon> GetPokemonFromApiAsync(int id)
        {
            string json = await NetworkHelper.GetStringAsync($"http://pokeapi.co/api/v2/pokemon/{id}");
            return JsonConvert.DeserializeObject<Pokemon>(json);
        }
    }
}