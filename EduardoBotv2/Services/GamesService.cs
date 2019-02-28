using Discord;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using EduardoBotv2.Extensions;
using EduardoBotv2.Helpers;
using EduardoBotv2.Models;
using EduardoBotv2.Models.Enums;
using EduardoBotv2.Models.Pokemon;

namespace EduardoBotv2.Services
{
    public class GamesService
    {
        private static readonly Dictionary<Pokemon, int> _pokemonInventory = new Dictionary<Pokemon, int>();

        public async Task GetPokemon(EduardoContext c)
        {
            // .Next() lower bound is inclusive, upper bound is exclusive.
            int roll = new Random().Next(1, Constants.ALL_POKEMON_COUNT + 1);

            Pokemon pokemonRoll = await GetPokemonFromApi(roll);
            IMessage waitingMessage = await c.Channel.SendMessageAsync($"{c.User.Username.Boldify()} is looking for a Pokemon...");

            await waitingMessage.DeleteAsync();

            if (pokemonRoll.Id != 0)
            {
                await c.Channel.SendMessageAsync($"{c.User.Username.Boldify()} has found a wild {pokemonRoll.Name.UpperFirstChar()}!\n{pokemonRoll.Sprites.FrontDefaultSpriteUrl}");
                if (_pokemonInventory.ContainsKey(pokemonRoll))
                {
                    _pokemonInventory[pokemonRoll] += 1;
                }
                else _pokemonInventory.Add(pokemonRoll, 1);
            }
            else await Logger.Log(new LogMessage(LogSeverity.Error, "Eduardo Bot", $"Error fetching Pokemon with id {roll}"));
        }

        public async Task ShowInventory(EduardoContext c)
        {
            if (_pokemonInventory.Count > 0)
            {
                List<Embed> pageEmbeds = new List<Embed>();
                for (int i = 0; i < _pokemonInventory.Count; i += Constants.MAX_POKEMON_PER_PAGE)
                {
                    Dictionary<Pokemon, int> pokemonPage = _pokemonInventory.Skip(i).Take(Math.Max(_pokemonInventory.Count / 4 - (i + 1), Constants.MAX_POKEMON_PER_PAGE)).ToDictionary(x => x.Key, x => x.Value);
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
                            IconUrl = c.User.GetAvatarUrl(),
                            Name = $"{c.User.Username}'s Pokemon"
                        },
                        Color = new Color(255, 255, 0),
                        Fields = fields,
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = @"https://maxcdn.icons8.com/Share/icon/color/Gaming//pokeball1600.png",
                            Text = $"Page {i / Constants.MAX_POKEMON_PER_PAGE + 1} | Pokemon via pokeapi.co"
                        }
                    }.Build());
                }

                await c.SendPaginatedMessageAsync(new PaginatedMessage
                {
                    Embeds = pageEmbeds,
                    Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                    TimeoutBehaviour = TimeoutBehaviour.Delete
                });
            }
            else
            {
                await c.Channel.SendMessageAsync($"**{c.User.Mention} You dont have any Pokemon! Use `$pokemon` to find a wild Pokemon!**");
            }
        }

        public async Task FlipCoin(EduardoContext c)
        {
            string result = new Random().Next(0, 2) == 0 ? "Heads" : "Tails";
            await c.Channel.SendMessageAsync(result);
        }

        public async Task DisplayEightBall(EduardoContext c)
        {
            await c.Channel.TriggerTypingAsync();
            string answer = Constants.EIGHT_BALL_WORDS[new Random().Next(0, Constants.EIGHT_BALL_WORDS.Count)];
            await c.Channel.SendMessageAsync(answer);
        }

        private static async Task<Pokemon> GetPokemonFromApi(int roll)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://pokeapi.co/api/v2/pokemon/{roll}");
            HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Pokemon>(result);
        }
    }
}