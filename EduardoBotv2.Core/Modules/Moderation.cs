using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class Moderation : ModuleBase<EduardoContext>
    {
        private readonly ModerationService service;

        public Moderation(ModerationService service)
        {
            this.service = service;
        }

        [Command("ban", RunMode = RunMode.Async), Alias("banish", "hammer")]
        [Summary("Ban a user.")]
        [Remarks("UppyMeister();")]
        [RequireUserPermission(GuildPermission.BanMembers), RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanCommand([Summary("User to ban.")] IGuildUser user, [Remainder] string reason = null)
        {
            await service.BanUser(Context, user, reason);
        }

        [Command("kick", RunMode = RunMode.Async)]
        [Summary("Kick a user.")]
        [Remarks("UppyMeister();")]
        [RequireUserPermission(GuildPermission.KickMembers), RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickCommand([Summary("User to kick.")] IGuildUser user, [Remainder] string reason = null)
        {
            await service.KickUser(Context, user, reason);
        }
    }
}