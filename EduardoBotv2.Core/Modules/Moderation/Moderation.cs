using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Moderation.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Moderation
{
    public class Moderation : EduardoModule<ModerationService>
    {
        public Moderation(ModerationService service)
            : base(service) { }

        [Command("ban")]
        [Alias("banish", "hammer")]
        [Summary("Ban a user")]
        [Remarks("uppy")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanCommand([Summary("User to ban")] IGuildUser user, [Remainder, Summary("Ban reason")] string reason = null)
        {
            await _service.BanUser(Context, user, reason);
        }

        [Command("kick")]
        [Summary("Kick a user")]
        [Remarks("uppy")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickCommand([Summary("User to kick")] IGuildUser user, [Remainder, Summary("Kick reason")] string reason = null)
        {
            await _service.KickUser(Context, user, reason);
        }
    }
}