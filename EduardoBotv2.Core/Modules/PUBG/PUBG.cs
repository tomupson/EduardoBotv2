using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.PUBG.Services;
using Pubg.Net;

namespace EduardoBotv2.Core.Modules.PUBG
{
    [Group("pubg")]
    public class Pubg : ModuleBase<EduardoContext>
    {
        private readonly PubgService service;

        public Pubg(PubgService service)
        {
            this.service = service;
        }

        [Command("player", RunMode = RunMode.Async)]
        [Summary("Get Player info for a user")]
        [Remarks("uppy steam")]
        public async Task PlayerCommand(
            [Summary("The username of the player to get info from")] string username,
            [Summary("The platform and region to search in")] PubgPlatform platform)
        {
            await service.GetPlayer(Context, username, platform);
        }

        [Command("matches", RunMode = RunMode.Async)]
        [Summary("Get match info for a user")]
        [Remarks("uppy steam")]
        public async Task MatchesCommand(
            [Summary("The username of the player to get matches from")] string username,
            [Summary("The platform and region to search in")] PubgPlatform platform)
        {
            await service.GetMatches(Context, username, platform);
        }
    }
}