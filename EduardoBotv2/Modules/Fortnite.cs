using Discord.Commands;
using EduardoBotv2.Services;
using System.Threading.Tasks;
using EduardoBotv2.Models;
using EduardoBotv2.Models.Enums;

namespace EduardoBotv2.Modules
{
    public class Fortnite : ModuleBase<EduardoContext>
    {
        private readonly FortniteService service;

        public Fortnite(FortniteService service)
        {
            this.service = service;
        }

        [Command("fnstats", RunMode = RunMode.Async)]
        [Summary("Get Fortnite Battle Royale stats for a player.")]
        [Remarks("UppyMeister solo")]
        public async Task FortniteStatsCommand([Summary("The username of the player to get stats for.")] string username, [Summary("The gamemode you want to get stats for.")] GameMode gamemode)
        {
            await service.GetStats(Context, username, gamemode);
        }

        [Command("fnnews", RunMode = RunMode.Async)]
        [Summary("Get Fortnite News.")]
        [Remarks("")]
        public async Task FortniteNewsCommand()
        {
            await service.GetNews(Context);
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
            await service.GetServerStatus(Context);
        }
        
        [Command("fnweekly", RunMode = RunMode.Async)]
        [Summary("Get Fortnite Battle Royale weekly store items.")]
        [Remarks("")]
        public async Task FortniteWeeklyStoreCommand()
        {
            await service.GetWeeklyStoreItems(Context);
        }

        [Command("fndaily", RunMode = RunMode.Async)]
        [Summary("Get Fortnite Battle Royale daily store items.")]
        [Remarks("")]
        public async Task FortniteDailyStoreCommand()
        {
            await service.GetDailyStoreItems(Context);
        }

        [Command("fnbug", RunMode = RunMode.Async)]
        [Summary("Report a bug in Fortnite.")]
        [Remarks("")]
        public async Task FortniteBugCommand()
        {
            await service.ReportBug(Context);
        }
    }
}