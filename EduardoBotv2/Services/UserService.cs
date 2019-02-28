using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EduardoBotv2.Helpers;
using EduardoBotv2.Models;

namespace EduardoBotv2.Services
{
    public class UserService
    {
        public async Task DisplayUserInfo(EduardoContext c, IGuildUser user = null)
        {
            SocketUser targetSocketUser;
            SocketGuildUser targetGuildUser;

            if (user == null)
            {
                targetSocketUser = c.User;
                targetGuildUser = c.User as SocketGuildUser;
            }
            else
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
                    IconUrl = c.Guild.CurrentUser.GetAvatarUrl(),
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

            await c.Channel.SendMessageAsync("", false, builder.Build());
        }

        public async Task CheckInvisible(EduardoContext c)
        {
            await c.Channel.TriggerTypingAsync();
            List<SocketGuildUser> invis = c.Guild.Users.Where(x => x.Guild.IsConnected && x.Status == UserStatus.Offline).ToList();
            string plural = invis.Count > 1 ? "are" : "is";
            await c.Channel.SendMessageAsync(invis.Any() ? $"{string.Join(", ", invis.Select(x => x.Username))} {plural} probably invisible." : "Nobody appears to be invisible!");
        }

        public async Task GetAvatar(EduardoContext c, IUser targetUser = null)
        {
            if (targetUser == null)
            {
                await c.Channel.SendMessageAsync(c.User.GetAvatarUrl());
            } else
            {
                await c.Channel.SendMessageAsync(targetUser.GetAvatarUrl());
            }
        }
    }
}