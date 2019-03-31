using System;
using Discord.Commands;
using Discord.WebSocket;

namespace EduardoBotv2.Core.Models
{
    public class EduardoContext : SocketCommandContext
    {
        public IServiceProvider Provider { get; set; }

        public EduardoContext(DiscordSocketClient client, SocketUserMessage msg, IServiceProvider provider)
            : base(client, msg)
        {
            Provider = provider;
        }
    }
}