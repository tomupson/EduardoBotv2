using Discord.Commands;
using Discord.WebSocket;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Utilities;
using EduardoBotv2.Common.Data.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EduardoBotv2.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly Settings _settings;

        private IServiceProvider _serviceProvider;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider, Settings settings)
        {
            _settings = settings;
            _client = client;
            _commandService = commandService;
            _commandService.Log += Logger.Log;
            _serviceProvider = provider;

            _client.MessageReceived += OnMessageReceviedAsync;
            //_client.UserJoined += OnUserJoinedAsync;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _serviceProvider = provider;
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task OnMessageReceviedAsync(SocketMessage sm)
        {
            SocketUserMessage msg = sm as SocketUserMessage;
            if (msg == null) return;

            var context = new EduardoContext(_client, msg, _serviceProvider, _settings);

            int argPos = 0;

            if (msg.HasStringPrefix(Config.DEFAULT_PREFIX, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    if (result.Error == CommandError.BadArgCount)
                    {
                        await context.Channel.SendMessageAsync($"**Incorrect command usage. Use `$help <command>` to show usage**");
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
