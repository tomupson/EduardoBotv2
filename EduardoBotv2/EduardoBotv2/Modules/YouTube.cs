using Discord.Commands;
using EduardoBot.Services;
using EduardoBot.Common.Data;
using System.Threading.Tasks;

namespace EduardoBot.Modules
{
    [Group("youtube")]
    public class YouTube : ModuleBase<EduardoContext>
    {
        private readonly YouTubeModuleService _service;

        public YouTube(YouTubeModuleService service)
        {
            this._service = service;
        }

        [Command("search", RunMode = RunMode.Async), Alias("find"), Name("youtube search")]
        [Summary("Search YouTube for a video")]
        [Remarks("harlem shake compilation")]
        public async Task SearchCommand([Remainder, Summary("The search query. If left blank, fetches a random video.")] string searchQuery = null)
        {
            await _service.SearchYouTube(Context, searchQuery);
        }
    }
}