using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using EduardoBot.Common.Data;
using EduardoBot.Common.Extensions;
using EduardoBot.Common.Data.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduardoBot.Services
{
    public class GeneralService
    {
        public async Task EchoText(EduardoContext c, string echo)
        {
            await c.Channel.SendMessageAsync(string.Format("{0} {1}", c.User.Mention, echo));
        }

        public async Task DisplayHelp(CommandService _service, EduardoContext c, string commandOrModule = null)
        {
            if (commandOrModule != null)
            {
                commandOrModule = commandOrModule.ToLower();
                if (commandOrModule.StartsWith(Config.DEFAULT_PREFIX))
                {
                    commandOrModule = commandOrModule.Remove(0, Config.DEFAULT_PREFIX.Length);
                }

                foreach (var module in _service.Modules)
                {
                    if (module.Name.ToLower() == commandOrModule || module.Aliases.Any(x => x.ToLower() == commandOrModule))
                    {
                        var longestInModule = 0;
                        foreach (var cmd in module.Commands)
                        {
                            if (cmd.Aliases.First().Length > longestInModule)
                            {
                                longestInModule = cmd.Aliases.First().Length;
                            }
                        }

                        var moduleInfo = $"**{module.Name} Commands **: ```asciidoc\n";
                        foreach (var cmd in module.Commands)
                        {
                            moduleInfo += $"{Config.DEFAULT_PREFIX}{cmd.Aliases.First()}{new string(' ', (longestInModule + 1) - cmd.Aliases.First().Length)} :: {cmd.Summary}\n";
                        }
                        moduleInfo += "\nUse the $help command for more information on any of these commands.```";
                        await c.Channel.SendMessageAsync(moduleInfo);
                        return;
                    }

                    var command = module.Commands.FirstOrDefault(x => x.Aliases.Any(y => y.ToLower() == commandOrModule));
                    if (command != default(CommandInfo))
                    {
                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder()
                            {
                                Name = "Description",
                                Value = command.Summary
                            },
                            new EmbedFieldBuilder()
                            {
                                Name = "Usage",
                                Value = $"`{Config.DEFAULT_PREFIX}{commandOrModule}{command.GetUsage()}`"
                            }
                        };

                        if (command.Parameters.Count > 0)
                        {
                            fields.Add(new EmbedFieldBuilder()
                            {
                                Name = "Example",
                                Value = $"`{Config.DEFAULT_PREFIX}{commandOrModule} {command.Remarks}`"
                            });
                        }

                        EmbedBuilder builder = new EmbedBuilder()
                        {
                            Author = new EmbedAuthorBuilder()
                            {
                                IconUrl = c.Client.CurrentUser.GetAvatarUrl(),
                                Name = $"Command Summary for \"{commandOrModule}\""
                            },
                            Color = Color.DarkPurple,
                            Fields = fields
                        };

                        //await c.Channel.SendMessageAsync($"**Description:** {command.Summary}\n\n" +
                        //$"**Usage:** `{Config.DEFAULT_PREFIX}{commandOrModule}{command.GetUsage()}`\n\n" +
                        //$"**Example:** {example}");
                        await c.Channel.SendMessageAsync("", false, builder.Build());
                        return;
                    }
                }

                await c.Channel.SendMessageAsync("This command/module does not exist.");
            }
            else
            {
                var userDm = await c.User.GetOrCreateDMChannelAsync();

                string modules = string.Empty;
                string modulesAndCommands = string.Empty;

                foreach (var module in _service.Modules)
                {
                    modules += $"{module.Name}, ";
                    modulesAndCommands += $"\n{module.Name.UpperFirstChar().Boldify()}\n";
                    foreach (var command in module.Commands)
                    {
                        modulesAndCommands += $"${command.Name} :: {command.Summary}\n";
                    }
                }

                await userDm.SendMessageAsync($"\nEduardo is a multi-purpose Discord Bot. This command can be used the view the usage of a specific command.\nHere are the commands you can use:\n {modulesAndCommands}\n\nUse `$help <command>` to view the usage of any command!");

                await c.Channel.SendMessageAsync(string.Format("{0}, {1}", c.User.Mention, "You have been DMed with all the command information!"));
            }
        }

        public async Task Ping(EduardoContext c)
        {
            await c.Channel.SendMessageAsync("Pong!");
        }

        public async Task Choose(EduardoContext c, string[] words)
        {
            await c.Channel.SendMessageAsync(words[new Random().Next(0, words.Length)]);
        }

        public async Task GoogleForYou(EduardoContext c, string searchQuery)
        {
            await c.Channel.SendMessageAsync($"**Your special URL: **<http://lmgtfy.com/?q={ Uri.EscapeUriString(searchQuery) }>");
        }

        public async Task SearchUrbanDictionary(EduardoContext c, string searchQuery)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"http://api.urbandictionary.com/v0/define?term={searchQuery.Replace(' ', '+')}");
                if (!response.IsSuccessStatusCode)
                {
                    await c.Channel.SendFileAsync("**Failed to communicate with Urban's API.");
                    return;
                }

                var data = JToken.Parse(await response.Content.ReadAsStringAsync()).ToObject<Urban>();
                if (!data.List.Any())
                {
                    await c.Channel.SendMessageAsync($"**Couldn't find anything related to {searchQuery}**");
                    return;
                }

                var termInfo = data.List[new Random().Next(0, data.List.Count)];
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Color = Color.Gold,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = $"Related Terms: {string.Join(", ", data.Tags)}" ?? "No related terms."
                    },
                    Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = $"Definition of {termInfo.Word}",
                            Value = termInfo.Definition
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "Example",
                            Value = termInfo.Example
                        }
                    }
                };

                await c.Channel.SendMessageAsync("", false, builder.Build());
            }
        }

        public async Task RoboMe(EduardoContext c, string username)
        {
            username = username.Replace(" ", "");
            await c.Channel.SendMessageAsync($"**https://robohash.org/{username}**");
        }
    }
}