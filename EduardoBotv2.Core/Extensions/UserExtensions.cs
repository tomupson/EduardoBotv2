using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace EduardoBotv2.Core.Extensions
{
    public static class UserExtensions
    {
        public static SocketUser AsSocketUser(this IUser user)
            => user as SocketUser;

        public static SocketGuildUser AsSocketGuildUser(this IUser user)
            => user as SocketGuildUser;

        public static RestUser AsRestUser(this IUser user)
            => user as RestUser;

        public static RestGuildUser AsRestGuildUser(this IUser user)
            => user as RestGuildUser;
    }
}