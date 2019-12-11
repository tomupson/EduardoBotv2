using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.User.Services
{
    public class UserService : IEduardoService
    {
        public async Task DisplayUserInfo(EduardoContext context, IGuildUser user = null)
        {
            SocketGuildUser guildUser = user != null ? user.AsSocketGuildUser() : context.User.AsSocketGuildUser();

            string userInfo = $"Created: {guildUser.CreatedAt:dddd MMM d}{DateTime.Now.Day.GetDaySuffix()} {guildUser.CreatedAt:yyyy} at {guildUser.CreatedAt:h:m tt}\n" +
                           $"Status: {guildUser.Status}\n" +
                           $"Game: {guildUser.Activity?.Name ?? "N/A"}\n";

            string memberInfo = $"Joined: {guildUser.JoinedAt:dddd MMM d}{DateTime.Now.Day.GetDaySuffix()} {guildUser.JoinedAt:yyyy} at {guildUser.JoinedAt:h:m tt}\n" +
                             $"Nickname: {guildUser.Nickname ?? "N/A"}\n";

            string roleInfo = string.Join(", ", guildUser.Roles.Where(x => !x.IsEveryone));

            EmbedBuilder builder = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithAuthor($"Summary for {guildUser.Username}#{guildUser.Discriminator} ({guildUser.Id})",
                    guildUser.GetAvatarUrl())
                .WithThumbnailUrl(guildUser.GetAvatarUrl())
                .AddField("User Info", userInfo)
                .AddField("Member Info", memberInfo)
                .AddConditionalField("Roles", roleInfo, !string.IsNullOrWhiteSpace(roleInfo))
                .WithFooter($"Eduardo | {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, DateTime.Now.Day.GetDaySuffix())}",
                    context.User.AsSocketGuildUser().GetAvatarUrl());

            await context.Channel.SendMessageAsync(embed: builder.Build());
        }

        public async Task GetAvatar(EduardoContext context, IUser targetUser = null)
        {
            if (targetUser == null)
            {
                await context.Channel.SendMessageAsync(context.User.GetAvatarUrl());
            }
            else
            {
                await context.Channel.SendMessageAsync(targetUser.GetAvatarUrl());
            }
        }
    }
}
