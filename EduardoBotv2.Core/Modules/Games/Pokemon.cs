using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Games.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Games
{
    partial class Games
    {
        [Group("pokemon")]
        [Name("Pokemon")]
        public class Pokemon : EduardoModule<PokemonService>
        {
            public Pokemon(PokemonService service)
                : base(service) { }

            [Command("find")]
            [Alias("discover")]
            [Summary("Discover a wild Pokemon")]
            public async Task DiscoverPokemonCommand()
            {
                await _service.DiscoverPokemonAsync(Context);
            }

            [Command("inventory")]
            [Alias("inv")]
            [Summary("View your Pokemon")]
            public async Task PokemonInventoryCommand()
            {
                await _service.ShowPokemonInventoryAsync(Context);
            }
        }
    }
}