using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using EduardoBotv2.Helpers;
using EduardoBotv2.Models;

namespace EduardoBotv2.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commandService;
        private readonly Settings settings;
        private IServiceProvider serviceProvider;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider, Settings settings)
        {
            this.settings = settings;
            this.client = client;
            this.commandService = commandService;
            this.commandService.Log += Logger.Log;
            serviceProvider = provider;

            this.client.MessageReceived += OnMessageReceviedAsync;
            //_client.UserJoined += OnUserJoinedAsync;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            serviceProvider = provider;
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task OnMessageReceviedAsync(SocketMessage sm)
        {
            if (!(sm is SocketUserMessage msg)) return;

            EduardoContext context = new EduardoContext(client, msg, serviceProvider, settings);

            int argPos = 0;

            if (msg.HasStringPrefix(Constants.DEFAULT_PREFIX, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))
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

        //private async Task OnUserJoinedAsync(SocketGuildUser user)
        //{
        //    if (user.DiscriminatorValue == 1917 || user.DiscriminatorValue == 5353)
        //    {
        //        await user.ModifyAsync(p => p.Nickname = "GANDALF THE GAY LMAO");
        //    }
        //    else if (user.DiscriminatorValue == 875)
        //    {
        //        await user.ModifyAsync(p => p.Nickname = "Kurt");
        //    }
        //}
    }
}