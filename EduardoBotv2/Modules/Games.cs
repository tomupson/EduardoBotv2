using Discord.Commands;
using EduardoBotv2.Services;
using System.Threading.Tasks;
using EduardoBotv2.Models;

namespace EduardoBotv2.Modules
{
    public class Games : ModuleBase<EduardoContext>
    {
        private readonly GamesService service;

        public Games(GamesService service)
        {
            this.service = service;
        }

        [Command("pokemon", RunMode = RunMode.Async), Alias("poke")]
        [Summary("Discover a wild Pokemon!")]
        [Remarks("")]
        public async Task PokemonCommand()
        {
            await service.GetPokemon(Context);
        }

        [Command("inventory", RunMode = RunMode.Async), Alias("inv")]
        [Summary("View your Pokemon.")]
        [Remarks("")]
        public async Task InventoryCommand()
        {
            await service.ShowInventory(Context);
        }

        [Command("coin", RunMode = RunMode.Async), Alias("toss")]
        [Summary("Flip a coin.")]
        [Remarks("")]
        public async Task CoinCommand()
        {
            await service.FlipCoin(Context);
        }

        [Command("8ball", RunMode = RunMode.Async)]
        [Summary("Determine your fate.")]
        [Remarks("Will I die tomorrow?")]
        public async Task EightBallCommand([Summary("The (optional) question or statement you want an answer to.")] string question = null)
        {
            await service.DisplayEightBall(Context);
        }
    }
}