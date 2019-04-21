using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Games.Database.Pokemon;
using EduardoBotv2.Core.Modules.Games.Helpers;
using EduardoBotv2.Core.Modules.Games.Models.Pokemon;
using EduardoBotv2.Core.Services;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.Games.Services
{
    public class PokemonService : IEduardoService
    {
        private readonly PokemonData _pokemonData;
        private readonly IPokemonRepository _pokemonRepository;

        public PokemonService(IPokemonRepository pokemonRepository)
        {
            _pokemonData = JsonConvert.DeserializeObject<PokemonData>(File.ReadAllText("data/pokemon.json"));
            _pokemonRepository = pokemonRepository;
        }

        public async Task DiscoverPokemonAsync(EduardoContext context)
        {
            int roll = new Random().Next(0, _pokemonData.PokemonCount);

            IMessage waitingMessage = await context.Channel.SendMessageAsync($"{Format.Bold(context.User.Username)} is looking for a Pokemon...");

            Pokemon pokemonRoll = await PokemonHelper.GetPokemonFromApiAsync(roll + 1);

            await waitingMessage.DeleteAsync();

            if (pokemonRoll.Id != 0)
            {
                using (Stream stream = await NetworkHelper.GetStreamAsync(pokemonRoll.Sprites.FrontDefaultSpriteUrl))
                {
                    await context.Channel.SendFileAsync(stream, $"{pokemonRoll.Name}.png", $"{Format.Bold(context.User.Username)} has found a wild {Format.Bold(pokemonRoll.Name.UpperFirstChar())}!");
                }

                await _pokemonRepository.AddPokemonAsync((long)context.Message.Author.Id,
                    (long)((context.Message.Channel as SocketGuildChannel)?.Guild.Id ?? 0), pokemonRoll);
            } else
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "Eduardov2", $"Error fetching Pokemon with id {roll}"));
            }
        }

        public async Task ShowPokemonInventoryAsync(EduardoContext context)
        {
            Dictionary<PokemonSummary, int> pokemonInventory = await _pokemonRepository.GetPokemonAsync((long)context.Message.Author.Id,
                (long)((context.Message.Channel as SocketGuildChannel)?.Guild.Id ?? 0));

            if (pokemonInventory.Count > 0)
            {
                List<Embed> pageEmbeds = new List<Embed>();
                for (int i = 0; i < pokemonInventory.Count; i += _pokemonData.MaxPokemonPerPage)
                {
                    List<KeyValuePair<PokemonSummary, int>> pokemonPage = pokemonInventory
                        .Skip(i)
                        .Take(Math.Min(_pokemonData.MaxPokemonPerPage, pokemonInventory.Count - i)).ToList();

                    pageEmbeds.Add(new EmbedBuilder()
                        .WithColor(new Color(255, 255, 0))
                        .WithAuthor($"{context.User.Username}'s Pokemon",
                            context.User.GetAvatarUrl())
                        .WithFieldsForList(pokemonPage, x => x.Key.Name.UpperFirstChar(), x => x.Value)
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
                await context.Channel.SendMessageAsync($"{context.User.Mention} You don't have any Pokemon! Use `{Constants.CMD_PREFIX}pokemon` to find a wild Pokemon!");
            }
        }
    }
}