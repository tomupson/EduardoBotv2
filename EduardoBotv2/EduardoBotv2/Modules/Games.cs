using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Data.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduardoBotv2.Modules
{
    public class Games : ModuleBase<EduardoContext>
    {
        private readonly GamesService _service;
        private static Dictionary<Pokemon, int> pokemonInventory = new Dictionary<Pokemon, int>();

        public Games(GamesService service)
        {
            this._service = service;
        }

        [Command("pokemon", RunMode = RunMode.Async), Alias("poke")]
        [Summary("Discover a wild Pokemon!")]
        [Remarks("")]
        public async Task PokemonCommand()
        {
            await _service.GetPokemon(Context);
        }

        [Command("inventory", RunMode = RunMode.Async), Alias("inv")]
        [Summary("View your Pokemon.")]
        [Remarks("")]
        public async Task InventoryCommand()
        {
            await _service.ShowInventory(Context);
        }

        [Command("coin", RunMode = RunMode.Async), Alias("toss")]
        [Summary("Flip a coin.")]
        [Remarks("")]
        public async Task CoinCommand()
        {
            await _service.FlipCoin(Context);
        }

        [Command("8ball", RunMode = RunMode.Async)]
        [Summary("Determine your fate.")]
        [Remarks("Will I die tomorrow?")]
        public async Task EightBallCommand([Summary("The (optional) question or statement you want an answer to.")] string question = null)
        {
            await _service.DisplayEightBall(Context);
        }
    }
}