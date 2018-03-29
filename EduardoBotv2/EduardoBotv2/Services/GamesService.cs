using Discord;
using Newtonsoft.Json;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Utilities;
using EduardoBotv2.Common.Data.Enums;
using EduardoBotv2.Common.Extensions;
using EduardoBotv2.Common.Data.Models;
using EduardoBotv2.Common.Utilities.Helpers;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduardoBotv2.Services
{
    public class GamesService
    {
        private static readonly Dictionary<Pokemon, int> pokemonInventory = new Dictionary<Pokemon, int>();

        public async Task GetPokemon(EduardoContext c)
        {
            // .Next() lower bound is inclusive, upper bound is exclusive.
            int roll = new Random().Next(1, Config.ALL_POKEMON_COUNT + 1);

            Pokemon pokemonRoll = await GetPokemonFromApi(roll);
            IMessage waitingMessage = await c.Channel.SendMessageAsync($"{c.User.Username.Boldify()} is looking for a Pokemon...");

            await waitingMessage.DeleteAsync();

            if (pokemonRoll.Id != 0)
            {
                await c.Channel.SendMessageAsync($"{c.User.Username.Boldify()} has found a wild {pokemonRoll.Name.UpperFirstChar()}!\n{pokemonRoll.Sprites.FrontDefaultSpriteUrl}");
                if (pokemonInventory.ContainsKey(pokemonRoll))
                {
                    pokemonInventory[pokemonRoll] += 1;
                }
                else pokemonInventory.Add(pokemonRoll, 1);
            }
            else await Logger.Log(new LogMessage(LogSeverity.Error, "Eduardo Bot", $"Error fetching Pokemon with id {roll}"));
        }

        public async Task ShowInventory(EduardoContext c)
        {
            if (pokemonInventory.Count > 0)
            {
                List<Embed> pageEmbeds = new List<Embed>();
                for (var i = 0; i < pokemonInventory.Count; i += Config.MAX_POKEMON_PER_PAGE)
                {
                    var pokemonPage = pokemonInventory.Skip(i).Take(Math.Max((pokemonInventory.Count / 4) - (i + 1), Config.MAX_POKEMON_PER_PAGE)).ToDictionary(x => x.Key, x => x.Value);
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                    foreach (KeyValuePair<Pokemon, int> poke in pokemonPage)
                    {
                        fields.Add(new EmbedFieldBuilder()
                        {
                            Name = $"{poke.Key.Name.UpperFirstChar()} (x{poke.Value})",
                            Value = poke.Key.Sprites.FrontDefaultSpriteUrl
                        });
                    }

                    pageEmbeds.Add(new EmbedBuilder()
                    {
                        Author = new EmbedAuthorBuilder()
                        {
                            IconUrl = c.User.GetAvatarUrl(),
                            Name = $"{c.User.Username}'s Pokemon"
                        },
                        Color = new Color(255, 255, 0),
                        Fields = fields,
                        Footer = new EmbedFooterBuilder()
                        {
                            IconUrl = @"https://maxcdn.icons8.com/Share/icon/color/Gaming//pokeball1600.png",
                            Text = $"Page {(i / Config.MAX_POKEMON_PER_PAGE) + 1} | Pokemon via pokeapi.co"
                        }
                    }.Build());
                }

                await c.SendPaginatedMessageAsync(new PaginatedMessage()
                {
                    Embeds = pageEmbeds,
                    Timeout = Config.PAGINATION_TIMEOUT_TIME,
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
            var result = new Random().Next(0, 2) == 0 ? "Heads" : "Tails";
            await c.Channel.SendMessageAsync(result);
        }

        public async Task DisplayEightBall(EduardoContext c)
        {
            await c.Channel.TriggerTypingAsync();
            var answer = Config.EIGHT_BALL_WORDS[new Random().Next(0, Config.EIGHT_BALL_WORDS.Count)];
            await c.Channel.SendMessageAsync(answer);
        }

        private async Task<Pokemon> GetPokemonFromApi(int roll)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://pokeapi.co/api/v2/pokemon/{roll}");
            HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Pokemon>(result);
        }
    }
}