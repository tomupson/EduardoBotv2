using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Models.Enums;
using EduardoBotv2.Core.Models.Games;
using EduardoBotv2.Core.Models.Games.Pokemon;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Services
{
    public class GamesService
    {
        private static readonly Dictionary<Pokemon, int> _pokemonInventory = new Dictionary<Pokemon, int>();

        private readonly PokemonData pokemonData;
        private readonly EightBallData eightBallData;

        public GamesService()
        {
            pokemonData = JsonConvert.DeserializeObject<PokemonData>(File.ReadAllText("data/pokemon.json"));
            eightBallData = JsonConvert.DeserializeObject<EightBallData>(File.ReadAllText("data/eightball.json"));
        }

        public async Task GetPokemon(EduardoContext context)
        {
            // .Next() lower bound is inclusive, upper bound is exclusive.
            int roll = new Random().Next(0, pokemonData.PokemonCount);

            IMessage waitingMessage = await context.Channel.SendMessageAsync($"{context.User.Username.Boldify()} is looking for a Pokemon...");

            Pokemon pokemonRoll = await GetPokemonFromApi(roll + 1);

            await waitingMessage.DeleteAsync();

            if (pokemonRoll.Id != 0)
            {
                using (Stream stream = await NetworkHelper.GetStream(pokemonRoll.Sprites.FrontDefaultSpriteUrl))
                {
                    await context.Channel.SendFileAsync(stream, $"{pokemonRoll.Name}.png", $"{context.User.Username.Boldify()} has found a wild {pokemonRoll.Name.UpperFirstChar()}!");
                }

                if (_pokemonInventory.ContainsKey(pokemonRoll))
                {
                    _pokemonInventory[pokemonRoll] += 1;
                } else
                {
                    _pokemonInventory.Add(pokemonRoll, 1);
                }
            } else
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "Eduardo", $"Error fetching Pokemon with id {roll}"));
            }
        }

        public async Task ShowInventory(EduardoContext context)
        {
            if (_pokemonInventory.Count > 0)
            {
                List<Embed> pageEmbeds = new List<Embed>();
                for (int i = 0; i < _pokemonInventory.Count; i += pokemonData.MaxPokemonPerPage)
                {
                    Dictionary<Pokemon, int> pokemonPage = _pokemonInventory.Skip(i).Take(Math.Min(pokemonData.MaxPokemonPerPage, _pokemonInventory.Count - i - 1)).ToDictionary(x => x.Key, x => x.Value);
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                    foreach ((Pokemon pokemon, int amount) in pokemonPage)
                    {
                        fields.Add(new EmbedFieldBuilder
                        {
                            Name = $"{pokemon.Name.UpperFirstChar()} (x{amount})",
                            Value = pokemon.Sprites.FrontDefaultSpriteUrl
                        });
                    }

                    pageEmbeds.Add(new EmbedBuilder
                    {
                        Author = new EmbedAuthorBuilder
                        {
                            IconUrl = context.User.GetAvatarUrl(),
                            Name = $"{context.User.Username}'s Pokemon"
                        },
                        Color = new Color(255, 255, 0),
                        Fields = fields,
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = @"https://maxcdn.icons8.com/Share/icon/color/Gaming//pokeball1600.png",
                            Text = $"Page {i / pokemonData.MaxPokemonPerPage + 1} | Pokemon via pokeapi.co"
                        }
                    }.Build());
                }
                
                await context.SendPaginatedMessageAsync(new PaginatedMessage
                {
                    Embeds = pageEmbeds,
                    Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                    TimeoutBehaviour = TimeoutBehaviour.Delete
                });
            } else
            {
                await context.Channel.SendMessageAsync($"**{context.User.Mention} You dont have any Pokemon! Use `$pokemon` to find a wild Pokemon!**");
            }
        }

        public async Task FlipCoin(EduardoContext context)
        {
            string result = new Random().Next(0, 2) == 0 ? "Heads" : "Tails";
            await context.Channel.SendMessageAsync(result);
        }

        public async Task DisplayEightBall(EduardoContext context)
        {
            await context.Channel.TriggerTypingAsync();
            string answer = eightBallData.Words[new Random().Next(0, eightBallData.Words.Count)];
            await context.Channel.SendMessageAsync(answer);
        }

        private static async Task<Pokemon> GetPokemonFromApi(int roll)
        {
            string json = await NetworkHelper.GetString($"http://pokeapi.co/api/v2/pokemon/{roll}");
            return JsonConvert.DeserializeObject<Pokemon>(json);
        }
    }
}