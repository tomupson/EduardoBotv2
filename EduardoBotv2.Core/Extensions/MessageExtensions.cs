using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace EduardoBotv2.Core.Extensions
{
    public static class MessageExtensions
    {
        public static SocketUserMessage AsSocketUserMessage(this IMessage message)
            => message as SocketUserMessage;

        public static SocketMessage AsSocketMessage(this IMessage message)
            => message as SocketMessage;

        public static RestUserMessage AsRestUserMessage(this IMessage message)
            => message as RestUserMessage;

        public static RestMessage AsRestMessage(this IMessage message)
            => message as RestMessage;
    }
}