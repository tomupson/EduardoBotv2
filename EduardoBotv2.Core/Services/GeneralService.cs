using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Services
{
    public class GeneralService
    {
        public async Task EchoText(EduardoContext context, string echo)
        {
            await context.Channel.SendMessageAsync($"{context.User.Mention} {echo}");
        }

        public async Task DisplayHelp(CommandService service, EduardoContext context, string commandOrModule = null)
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
                    if (module.Name.ToLower() == commandOrModule || module.Aliases.Any(x => x.ToLower() == commandOrModule))
                    {
                        int longestInModule = module.Commands.Select(cmd => cmd.Aliases.First().Length).Concat(new[] { 0 }).Max();

                        string moduleInfo = $"**{module.Name} Commands **: ```asciidoc\n";
                        moduleInfo = module.Commands.Aggregate(moduleInfo, (current, cmd) => current + $"{Constants.CMD_PREFIX}{cmd.Aliases.First()}{new string(' ', longestInModule + 1 - cmd.Aliases.First().Length)} :: {cmd.Summary}\n");
                        moduleInfo += "\nUse the $help command for more information on any of these commands.```";
                        await context.Channel.SendMessageAsync(moduleInfo);
                        return;
                    }

                    CommandInfo command = module.Commands.FirstOrDefault(x => x.Aliases.Any(y => y.ToLower() == commandOrModule));
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

                    EmbedBuilder builder = new EmbedBuilder
                    {
                        Author = new EmbedAuthorBuilder
                        {
                            IconUrl = context.Client.CurrentUser.GetAvatarUrl(),
                            Name = $"Command Summary for \"{commandOrModule}\""
                        },
                        Color = Color.DarkPurple,
                        Fields = fields
                    };

                    //await c.Channel.SendMessageAsync($"**Description:** {command.Summary}\n\n" +
                    //$"**Usage:** `{Config.DEFAULT_PREFIX}{commandOrModule}{command.GetUsage()}`\n\n" +
                    //$"**Example:** {example}");
                    await context.Channel.SendMessageAsync(embed: builder.Build());
                    return;
                }

                await context.Channel.SendMessageAsync("This command/module does not exist.");
            } else
            {
                IDMChannel userDm = await context.User.GetOrCreateDMChannelAsync();

                string modulesAndCommands = string.Empty;

                foreach (ModuleInfo module in service.Modules)
                {
                    modulesAndCommands += $"\n{module.Name.UpperFirstChar().Boldify()}\n";
                    modulesAndCommands = module.Commands.Aggregate(modulesAndCommands, (current, command) => current + $"${command.Name} :: {command.Summary}\n");
                }

                await userDm.SendMessageAsync($"\nEduardo is a multi-purpose Discord Bot. This command can be used the view the usage of a specific command.\nHere are the commands you can use:\n {modulesAndCommands}\n\nUse `$help <command>` to view the usage of any command!");

                await context.Channel.SendMessageAsync($"{context.User.Mention} - you have been DMed with all the command information!");
            }
        }

        public async Task Ping(EduardoContext context)
        {
            SocketUserMessage ping = context.Message;
            RestUserMessage pong = await context.Channel.SendMessageAsync(embed: new EmbedBuilder
            {
                Title = "Measuring Latency...",
                Color = Color.DarkRed
            }.Build());

            TimeSpan delta = pong.CreatedAt - ping.CreatedAt;
            double diffMs = delta.TotalSeconds * 1000;

            await pong.ModifyAsync(n =>
            {
                n.Embed = new EmbedBuilder
                {
                    Color = Color.DarkRed,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Round Trip",
                            Value = $"{diffMs} ms"
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Web Socket Latency",
                            Value = $"{context.Client.Latency} ms"
                        }
                    },
                    Title = "Latency Results"
                }.Build();
            });
        }

        public async Task Choose(EduardoContext context, string[] words)
        {
            await context.Channel.SendMessageAsync(words[new Random().Next(0, words.Length)]);
        }

        public async Task GoogleForYou(EduardoContext context, string searchQuery)
        {
            await context.Channel.SendMessageAsync($"**Your special URL: **<http://lmgtfy.com/?q={ Uri.EscapeUriString(searchQuery) }>");
        }

        public async Task SearchUrbanDictionary(EduardoContext context, string searchQuery)
        {
            string json = await NetworkHelper.GetString($"http://api.urbandictionary.com/v0/define?term={searchQuery.Replace(' ', '+')}");
            Urban data = JsonConvert.DeserializeObject<Urban>(json);

            if (!data.List.Any())
            {
                await context.Channel.SendMessageAsync($"**Couldn't find anything related to {searchQuery}**");
                return;
            }

            List termInfo = data.List[new Random().Next(0, data.List.Count)];
            EmbedBuilder builder = new EmbedBuilder
            {
                Color = Color.Gold,
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Related Terms: {string.Join(", ", data.Tags) ?? "No related terms."}"
                },
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = $"Definition of {termInfo.Word}",
                        Value = termInfo.Definition
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Example",
                        Value = termInfo.Example
                    }
                }
            };

            await context.Channel.SendMessageAsync(embed: builder.Build());
        }

        public async Task RoboMe(EduardoContext context, string username)
        {
            username = username.Replace(" ", "");
            await context.Channel.SendMessageAsync($"**https://robohash.org/{username}**");
        }
    }
}