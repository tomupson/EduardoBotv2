using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.General.Models;
using EduardoBotv2.Core.Services;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.General.Services
{
    public class GeneralService : IEduardoService
    {
        public async Task EchoTextAsync(EduardoContext context, string echo)
        {
            await context.Channel.SendMessageAsync($"{context.User.Mention} {echo}");
        }

        public async Task Ping(EduardoContext context)
        {
            IMessage ping = context.Message;
            RestUserMessage pong = await context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithTitle("Measuring Latency...")
                .WithColor(Color.DarkRed)
                .Build());

            TimeSpan delta = pong.CreatedAt - ping.CreatedAt;
            double diffMs = delta.TotalSeconds * 1000;

            await pong.ModifyAsync(n =>
            {
                n.Embed = new EmbedBuilder()
                    .WithTitle("Latency Results")
                    .WithColor(Color.DarkRed)
                    .AddField("Round Trip", $"{diffMs} ms")
                    .AddField("Web Socket Latency", $"{context.Client.Latency} ms")
                    .Build();
            });
        }

        public async Task Choose(EduardoContext context, string[] words)
        {
            await context.Channel.SendMessageAsync(words[new Random().Next(0, words.Length)]);
        }

        public async Task GoogleForYou(EduardoContext context, string searchQuery)
        {
            await context.Channel.SendMessageAsync($"{"Your special URL: ".Boldify()}<http://lmgtfy.com/?q={ Uri.EscapeUriString(searchQuery) }>");
        }

        public async Task SearchUrbanDictionary(EduardoContext context, string searchQuery)
        {
            string json = await NetworkHelper.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={searchQuery.Replace(' ', '+')}");
            Urban data = JsonConvert.DeserializeObject<Urban>(json);

            if (!data.List.Any())
            {
                await context.Channel.SendMessageAsync($"Couldn't find anything related to {searchQuery}".Boldify());
                return;
            }

            UrbanList termInfo = data.List[new Random().Next(0, data.List.Count)];

            await context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithColor(Color.Gold)
                .AddField($"Definition of {termInfo.Word}", termInfo.Definition)
                .AddField("Example", termInfo.Example)
                .WithFooter($"Related Terms: {string.Join(", ", data.Tags) ?? "No related terms."}")
                .Build());
        }

        public async Task RoboMe(EduardoContext context, string username)
        {
            username = username.Replace(" ", "");
            await context.Channel.SendMessageAsync($"https://robohash.org/{username}");
        }
    }
}