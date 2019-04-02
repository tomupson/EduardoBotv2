using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Games.Database;
using EduardoBotv2.Core.Modules.Games.Helpers;
using EduardoBotv2.Core.Modules.Games.Models;
using EduardoBotv2.Core.Modules.Games.Models.Pokemon;
using EduardoBotv2.Core.Services;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.Games.Services
{
    public class GamesService : IEduardoService
    {
        private static readonly Random _prng = new Random();

        private readonly PokemonData _pokemonData;
        private readonly EightBallData _eightBallData;

        private readonly IPokemonRepository _pokemonRepository;

        public GamesService(IPokemonRepository pokemonRepository)
        {
            _pokemonData = JsonConvert.DeserializeObject<PokemonData>(File.ReadAllText("data/pokemon.json"));
            _eightBallData = JsonConvert.DeserializeObject<EightBallData>(File.ReadAllText("data/eightball.json"));

            _pokemonRepository = pokemonRepository;
        }

        public async Task GetPokemonAsync(EduardoContext context)
        {
            int roll = new Random().Next(0, _pokemonData.PokemonCount);

            IMessage waitingMessage = await context.Channel.SendMessageAsync($"{context.User.Username.Boldify()} is looking for a Pokemon...");

            Pokemon pokemonRoll = await PokemonHelper.GetPokemonFromApiAsync(roll + 1);

            await waitingMessage.DeleteAsync();

            if (pokemonRoll.Id != 0)
            {
                using (Stream stream = await NetworkHelper.GetStreamAsync(pokemonRoll.Sprites.FrontDefaultSpriteUrl))
                {
                    await context.Channel.SendFileAsync(stream, $"{pokemonRoll.Name}.png", $"{context.User.Username.Boldify()} has found a wild {pokemonRoll.Name.UpperFirstChar().Boldify()}!");
                }

                await _pokemonRepository.AddPokemonAsync(context.Message.Author.Id, (context.Message.Channel as SocketGuildChannel)?.Guild.Id ?? 0, pokemonRoll);
            } else
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "Eduardov2", $"Error fetching Pokemon with id {roll}"));
            }
        }

        public async Task ShowInventoryAsync(EduardoContext context)
        {
            Dictionary<PokemonSummary, int> pokemonInventory = await _pokemonRepository.GetPokemonAsync(context.Message.Author.Id, (context.Message.Channel as SocketGuildChannel)?.Guild.Id ?? 0);

            if (pokemonInventory.Count > 0)
            {
                List<Embed> pageEmbeds = new List<Embed>();
                for (int i = 0; i < pokemonInventory.Count; i += _pokemonData.MaxPokemonPerPage)
                {
                    Dictionary<PokemonSummary, int> pokemonPage = pokemonInventory.Skip(i).Take(Math.Min(_pokemonData.MaxPokemonPerPage, pokemonInventory.Count - i)).ToDictionary(x => x.Key, x => x.Value);
                    StringBuilder descriptionBuilder = new StringBuilder();
                    foreach ((PokemonSummary pokemon, int amount) in pokemonPage)
                    {
                        descriptionBuilder.AppendFormat("{0} (x{1}){2}", pokemon.Name.UpperFirstChar(), amount, Environment.NewLine);
                    }

                    pageEmbeds.Add(new EmbedBuilder()
                        .WithColor(new Color(255, 255, 0))
                        .WithAuthor($"{context.User.Username}'s Pokemon",
                            context.User.GetAvatarUrl())
                        .WithDescription(descriptionBuilder.ToString())
                        .WithFooter($"Page {i / _pokemonData.MaxPokemonPerPage + 1} of {Math.Ceiling(pokemonInventory.Count / (double)_pokemonData.MaxPokemonPerPage)} | Pokemon via pokeapi.co",
                            @"https://maxcdn.icons8.com/Share/icon/color/Gaming/pokeball1600.png")
                        .Build());
                }
                
                await context.SendMessageOrPaginatedAsync(new PaginatedMessage
                {
                    Embeds = pageEmbeds,
                    Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                    TimeoutBehaviour = TimeoutBehaviour.Delete
                });
            } else
            {
                await context.Channel.SendMessageAsync($"{context.User.Mention} You don't have any Pokemon! Use `{Constants.CMD_PREFIX}pokemon` to find a wild Pokemon!".Boldify());
            }
        }

        public async Task FlipCoin(EduardoContext context)
        {
            string result = new Random().Next(0, 2) == 0 ? "Heads" : "Tails";
            await context.Channel.SendMessageAsync(result);
        }

        public async Task DisplayEightBall(EduardoContext context, string question = null)
        {
            await context.Channel.TriggerTypingAsync();
            string answer = _eightBallData.Words[_prng.Next(0, _eightBallData.Words.Count)];
            await context.Channel.SendMessageAsync($"{question} -- {answer}");
        }
    }
}