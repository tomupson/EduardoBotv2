using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private IServiceProvider serviceProvider;

        public CommandHandler(DiscordSocketClient client, CommandService commandService)
        {
            _client = client;
            _commandService = commandService;
            _commandService.Log += Logger.Log;

            _client.MessageReceived += OnMessageReceviedAsync;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            serviceProvider = provider;
            await _commandService.AddModulesAsync(GetType().GetTypeInfo().Assembly, serviceProvider);
        }

        private async Task OnMessageReceviedAsync(SocketMessage sm)
        {
            if (!(sm is SocketUserMessage msg)) return;

            EduardoContext context = new EduardoContext(_client, msg, serviceProvider);

            int argPos = 0;

            if (msg.HasStringPrefix(Constants.CMD_PREFIX, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                IResult result = await _commandService.ExecuteAsync(context, argPos, serviceProvider);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    if (result.Error == CommandError.BadArgCount)
                    {
                        await context.Channel.SendMessageAsync($"Incorrect command usage. Use `{Constants.CMD_PREFIX}help <command>` to show usage".Boldify());
                    } else
                    {
                        Console.WriteLine(result.ErrorReason);
                        await context.Channel.SendMessageAsync("Something went wrong while executing that command...");
                    }
                }
            }
        }
    }
}