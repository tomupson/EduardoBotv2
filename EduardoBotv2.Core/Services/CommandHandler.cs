using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commandService;
        private IServiceProvider serviceProvider;

        public CommandHandler(DiscordSocketClient client, CommandService commandService)
        {
            this.client = client;
            this.commandService = commandService;
            this.commandService.Log += Logger.Log;

            this.client.MessageReceived += OnMessageReceviedAsync;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            serviceProvider = provider;
            await commandService.AddModulesAsync(GetType().GetTypeInfo().Assembly, provider);
        }

        private async Task OnMessageReceviedAsync(SocketMessage sm)
        {
            if (!(sm is SocketUserMessage msg)) return;

            EduardoContext context = new EduardoContext(client, msg, serviceProvider);

            int argPos = 0;

            if (msg.HasStringPrefix(Constants.CMD_PREFIX, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                IResult result = await commandService.ExecuteAsync(context, argPos, serviceProvider);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    if (result.Error == CommandError.BadArgCount)
                    {
                        await context.Channel.SendMessageAsync("**Incorrect command usage. Use `$help <command>` to show usage**");
                    } else
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
        }
    }
}