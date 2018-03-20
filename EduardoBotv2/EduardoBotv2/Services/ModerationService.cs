using Discord;
using EduardoBot.Common.Data;
using EduardoBot.Common.Extensions;
using System.Threading.Tasks;

namespace EduardoBot.Services
{
    public class ModerationService
    {
        public async Task BanUser(EduardoContext c, IGuildUser banUser, string reason = null)
        {
            await c.Channel.SendMessageAsync($"{c.User.Mention.Boldify()} has banned {banUser.Mention}.\nReason: {reason}");
            await c.Guild.AddBanAsync(banUser, 0, reason);
        }

        public async Task KickUser(EduardoContext c, IGuildUser kickUser, string reason = null)
        {
            await c.Channel.SendMessageAsync($"{c.User.Mention.Boldify()} has kicked {kickUser.Mention}.\nReason: {reason}");
            // Add kick code here.
        }
    }
}