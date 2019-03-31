using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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

        public async Task DisplayHelpAsync(CommandService service, EduardoContext context, string commandOrModule = null)
        {
            if (commandOrModule != null)
            {
                commandOrModule = commandOrModule.ToLower();
                if (commandOrModule.StartsWith(Constants.CMD_PREFIX))
                {
                    commandOrModule = commandOrModule.Remove(0, Constants.CMD_PREFIX.Length);
                }

                foreach (ModuleInfo module in service.Modules)
                {
                    if (module.Name.ToLower() == commandOrModule || module.Aliases.Any(a => a.ToLower() == commandOrModule))
                    {
                        int longestAlias = module.Commands.Select(cmd => cmd.Aliases.First().Length).Concat(new[] { 0 }).Max();

                        StringBuilder moduleInfo = new StringBuilder($"{module.Name.Boldify()} Commands: ```asciidoc\n");

                        foreach (CommandInfo moduleCommand in module.Commands)
                        {
                            string alias = moduleCommand.Aliases.First();
                            moduleInfo.Append($"{Constants.CMD_PREFIX}{alias}{new string(' ', longestAlias + 1 - alias.Length)} :: {moduleCommand.Summary}\n");
                        }

                        moduleInfo.Append($"\nUse the {Constants.CMD_PREFIX}help command for more specific information on any of these commands.```");

                        await context.Channel.SendMessageAsync(moduleInfo.ToString());

                        return;
                    }

                    CommandInfo command = module.Commands.FirstOrDefault(c => c.Aliases.Any(a => a.ToLower() == commandOrModule));

                    if (command == default(CommandInfo)) continue;

                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Description",
                            Value = command.Summary
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Usage",
                            Value = $"`{Constants.CMD_PREFIX}{commandOrModule}{command.GetUsage()}`"
                        }
                    };

                    if (command.Parameters.Count > 0)
                    {
                        fields.Add(new EmbedFieldBuilder
                        {
                            Name = "Example",
                            Value = $"`{Constants.CMD_PREFIX}{commandOrModule} {command.Remarks}`"
                        });
                    }

                    await context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                        .WithColor(Color.DarkPurple)
                        .WithAuthor($"Command Summary for \"{command.Name}\"",
                            context.Client.CurrentUser.GetAvatarUrl())
                        .WithFields(fields)
                        .Build());

                    return;
                }

                await context.Channel.SendMessageAsync("The command / module does not exist.");
            } else
            {
                IDMChannel userDm = await context.User.GetOrCreateDMChannelAsync();

                List<string> messages = new List<string>();
                string currentMessage = "";

                foreach (ModuleInfo module in service.Modules.OrderBy(m => m.Name))
                {
                    StringBuilder currentModule = new StringBuilder($"\n{module.Name.Boldify()}\n");
                    foreach (CommandInfo command in module.Commands.OrderBy(c => c.Name))
                    {
                        if (!string.IsNullOrWhiteSpace(module.Group))
                        {
                            currentModule.Append(string.IsNullOrWhiteSpace(command.Name) ?
                                $"{Constants.CMD_PREFIX}{module.Group} :: {command.Summary}\n" :
                                $"{Constants.CMD_PREFIX}{module.Group} {command.Name} :: {command.Summary}\n");
                        } else
                        {
                            currentModule.Append($"{Constants.CMD_PREFIX}{command.Name} :: {command.Summary}\n");
                        }
                    }

                    if (currentMessage.Length + currentModule.Length <= 2000)
                    {
                        currentMessage += currentModule;
                        continue;
                    }

                    messages.Add(currentMessage);
                    currentMessage = "";
                }

                await userDm.SendMessageAsync("\nEduardo is a multi-purpose Discord Bot. This command can be used the view the usage of a specific command.\nHere are the commands you can use:");

                foreach (string message in messages)
                {
                    await userDm.SendMessageAsync($"\n{message}");
                }

                await userDm.SendMessageAsync($"\nUse `{Constants.CMD_PREFIX}help <command>` to view the usage of any specific command!");
            }
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