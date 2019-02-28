using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    [Group("youtube")]
    public class YouTube : ModuleBase<EduardoContext>
    {
        private readonly YouTubeModuleService service;

        public YouTube(YouTubeModuleService service)
        {
            this.service = service;
        }

        [Command("search", RunMode = RunMode.Async), Alias("find"), Name("youtube search")]
        [Summary("Search YouTube for a video")]
        [Remarks("harlem shake compilation")]
        public async Task SearchCommand([Remainder, Summary("The search query. If left blank, fetches a random video.")] string searchQuery = null)
        {
            await service.SearchYouTube(Context, searchQuery);
        }
    }
}