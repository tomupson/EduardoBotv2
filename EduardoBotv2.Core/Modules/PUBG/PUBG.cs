using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.PUBG.Services;
using EduardoBotv2.Core.Services;
using Pubg.Net;

namespace EduardoBotv2.Core.Modules.PUBG
{
    [Group("pubg")]
    public class Pubg : EduardoModule
    {
        private readonly PubgService _service;

        public Pubg(PubgService service)
        {
            _service = service;
        }

        [Command("player")]
        [Summary("Get Player info for a user")]
        [Remarks("uppy steam")]
        public async Task PlayerCommand(
            [Summary("The username of the player to get info from")] string username,
            [Summary("The platform and region to search in")] PubgPlatform platform)
        {
            await _service.GetPlayer(Context, username, platform);
        }

        [Command("matches")]
        [Summary("Get match info for a user")]
        [Remarks("uppy steam")]
        public async Task MatchesCommand(
            [Summary("The username of the player to get matches from")] string username,
            [Summary("The platform and region to search in")] PubgPlatform platform)
        {
            await _service.GetMatches(Context, username, platform);
        }
    }
}