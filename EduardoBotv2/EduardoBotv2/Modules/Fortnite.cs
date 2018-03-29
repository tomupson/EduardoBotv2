using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Data.Enums;
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
        [Remarks("UppyMeister solo")]
        public async Task FortniteStatsCommand([Summary("The username of the player to get stats for.")] string username, [Summary("The gamemode you want to get stats for.")] GameMode gamemode)
        {
            await _service.GetStats(Context, username, gamemode);
        }

        [Command("fnnews", RunMode = RunMode.Async)]
        [Summary("Get Fortnite News.")]
        [Remarks("")]
        public async Task FortniteNewsCommand()
        {
            await _service.GetNews(Context);
        }

        //[Command("fnstore", RunMode = RunMode.Async)]
        //[Summary("Get Fortnite Battle Royale store items.")]
        //[Remarks("")]
        //public async Task FortniteStoreCommand()
        //{
        //    await _service.GetStoreItems(Context);
        //}

        [Command("fnserverstatus", RunMode = RunMode.Async), Alias("fnstatus")]
        [Summary("Get the Fortnite Server Status")]
        [Remarks("")]
        public async Task FortniteServerStatusCommand()
        {
            await _service.GetServerStatus(Context);
        }
        
        [Command("fnweekly", RunMode = RunMode.Async)]
        [Summary("Get Fortnite Battle Royale weekly store items.")]
        [Remarks("")]
        public async Task FortniteWeeklyStoreCommand()
        {
            await _service.GetWeeklyStoreItems(Context);
        }

        [Command("fndaily", RunMode = RunMode.Async)]
        [Summary("Get Fortnite Battle Royale daily store items.")]
        [Remarks("")]
        public async Task FortniteDailyStoreCommand()
        {
            await _service.GetDailyStoreItems(Context);
        }

        [Command("fnbug", RunMode = RunMode.Async)]
        [Summary("Report a bug in Fortnite.")]
        [Remarks("")]
        public async Task FortniteBugCommand()
        {
            await _service.ReportBug(Context);
        }
    }
}
