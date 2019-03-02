using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Services
{
    public class UserService
    {
        public async Task DisplayUserInfo(EduardoContext context, IGuildUser user = null)
        {
            SocketUser targetSocketUser;
            SocketGuildUser targetGuildUser;

            if (user == null)
            {
                targetSocketUser = context.User;
                targetGuildUser = context.User as SocketGuildUser;
            } else
            {
                targetSocketUser = user as SocketUser;
                targetGuildUser = user as SocketGuildUser;
            }

            string targetSocketUserGame = targetSocketUser?.Activity.Name ?? "N/A";
            string targetGuildUserNickname = targetGuildUser?.Nickname ?? "N/A";

            string userInfo = $"Created: {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", targetSocketUser?.CreatedAt, CommonHelper.GetDaySuffix(DateTime.Now.Day))}\n" +
                           $"Status: {targetSocketUser?.Status}\n" +
                           $"Game: {targetSocketUserGame}\n";

            string memberInfo = $"Joined: {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", targetGuildUser?.JoinedAt, CommonHelper.GetDaySuffix(DateTime.Now.Day))}\n" +
                             $"Nickname: {targetGuildUserNickname}\n";

            string roleInfo = string.Join(", ", targetGuildUser?.Roles.Where(x => !x.IsEveryone));

            EmbedBuilder builder = new EmbedBuilder
            {
                Color = Color.Orange,
                ThumbnailUrl = targetSocketUser?.GetAvatarUrl(),
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = context.Guild.CurrentUser.GetAvatarUrl(),
                    Text = $"Eduardo | {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                },
                Author = new EmbedAuthorBuilder
                {
                    Name = $"Summary for {targetSocketUser?.Username}#{targetSocketUser?.Discriminator} ({targetSocketUser?.Id})",
                    IconUrl = targetSocketUser?.GetAvatarUrl()
                },
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "User Info",
                        Value = userInfo
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Member Info",
                        Value = memberInfo
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Roles",
                        Value = roleInfo
                    }
                }
            };

            await context.Channel.SendMessageAsync("", false, builder.Build());
        }

        public async Task GetAvatar(EduardoContext context, IUser targetUser = null)
        {
            if (targetUser == null)
            {
                await context.Channel.SendMessageAsync(context.User.GetAvatarUrl());
            } else
            {
                await context.Channel.SendMessageAsync(targetUser.GetAvatarUrl());
            }
        }
    }
}