using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Utility.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Utility
{
    public class Utility : EduardoModule
    {
        private readonly UtilityService _service;

        public Utility(UtilityService service)
        {
            _service = service;
        }

        [Command("clear")]
        [Summary("Cleans messages")]
        [Remarks("10")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task CleanCommand([Summary("The number of messages to delete")] uint count)
        {
            await _service.CleanMessages(Context, count);
        }

        [Command("invite")]
        [Summary("Retrieves the invite link for the bot")]
        public async Task InviteCommand()
        {
            await _service.DisplayInvite(Context);
        }
    }
}