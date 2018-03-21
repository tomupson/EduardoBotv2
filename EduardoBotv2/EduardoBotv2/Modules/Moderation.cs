using Discord;
using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    public class Moderation : ModuleBase<EduardoContext>
    {
        private readonly ModerationService _service;

        public Moderation(ModerationService service)
        {
            this._service = service;
        }

        [Command("ban", RunMode = RunMode.Async), Alias("banish", "hammer")]
        [Summary("Ban a user.")]
        [Remarks("UppyMeister();")]
        [RequireUserPermission(GuildPermission.BanMembers), RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanCommand([Summary("User to ban.")] IGuildUser user, [Remainder] string reason = null)
        {
            await _service.BanUser(Context, user, reason);
        }

        [Command("kick", RunMode = RunMode.Async)]
        [Summary("Kick a user.")]
        [Remarks("UppyMeister();")]
        [RequireUserPermission(GuildPermission.KickMembers), RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickCommand([Summary("User to kick.")] IGuildUser user, [Remainder] string reason = null)
        {
            await _service.KickUser(Context, user, reason);
        }
    }
}