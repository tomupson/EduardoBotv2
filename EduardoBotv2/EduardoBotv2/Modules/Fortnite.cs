using Discord.Commands;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Data.Enums;
using EduardoBotv2.Services;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    public class Fortnite : ModuleBase<EduardoContext>
    {
        private readonly FortniteService _service;

        public Fortnite(FortniteService service)
        {
            _service = service;
        }

        [Command("fnstats", RunMode = RunMode.Async)]
        [Summary("Get Fortnite Battle Royale stats for a player.")]
        [Remarks("UppyMeister")]
        public async Task FortniteStatsCommand([Summary("The username of the player to get stats for.")] string username, [Summary("The platform to get the stats from")] Platform platform)
        {
            await _service.GetStats(Context, username, platform);
        }
    }
}
