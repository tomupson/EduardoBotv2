﻿using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Services
{
    public class ModerationService
    {
        public async Task BanUser(EduardoContext context, IGuildUser banUser, string reason = null)
        {
            await context.Channel.SendMessageAsync($"{context.User.Mention.Boldify()} has banned {banUser.Mention}.\nReason: {reason}");
            await context.Guild.AddBanAsync(banUser, 0, reason);
        }

        public async Task KickUser(EduardoContext context, IGuildUser kickUser, string reason = null)
        {
            await context.Channel.SendMessageAsync($"{context.User.Mention.Boldify()} has kicked {kickUser.Mention}.\nReason: {reason}");
            // Add kick code here.
        }
    }
}