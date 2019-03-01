using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class PUBG : ModuleBase<EduardoContext>
    {
        private readonly PUBGService service;

        public PUBG(PUBGService service)
        {
            this.service = service;
        }

        [Command("pubgplayer", RunMode = RunMode.Async)]
        [Summary("Get Player info for a user")]
        [Remarks("UppyMeister pc-eu")]
        public async Task PUBGPlayerCommand([Summary("The username of the player to get info from")] string username, [Summary("The platform and region to search in")] string platformRegion)
        {
            await service.GetPlayer(Context, username, platformRegion);
        }

        [Command("pubgmatches", RunMode = RunMode.Async)]
        [Summary("Get match info for a user")]
        [Remarks("UppyMeister pc-eu")]
        public async Task PUBGMatchesCommand([Summary("The username of the player to get matches from")] string username, [Summary("The platform and region to search in")] string platformRegion)
        {
            await service.GetMatches(Context, username, platformRegion);
        }

        [Command("pubgmatch", RunMode = RunMode.Async)]
        [Summary("Get match info for a specific match")]
        [Remarks("")]
        public async Task PUBGMatchCommand([Summary("The id of the match")] string matchId, [Summary("The platform and region to search in")] string platformRegion)
        {
            await service.GetMatch(Context, matchId, platformRegion);
        }

        [Command("pubgvalids", RunMode = RunMode.Async)]
        [Summary("Get valid platform-region options")]
        [Remarks("")]
        public async Task PUBGValidOptionsCommand()
        {
            await service.ShowValidOptions(Context);
        }

        [Command("telemtest", RunMode = RunMode.Async)]
        [Summary("Testing telemetry data analysis for a match")]
        [Remarks("")]
        public async Task PUBGTelemTestCommand([Summary("The username of the player to get matches from")] string username, [Summary("The platform and region to search in")] string platformRegion)
        {
            await service.GetTelemetry(Context, username, platformRegion);
        }
    }
}