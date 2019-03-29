using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.YouTube.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.YouTube
{
    [Group("youtube")]
    public class YouTube : EduardoModule
    {
        private readonly YouTubeService _service;

        public YouTube(YouTubeService service)
        {
            _service = service;
        }

        [Command("search")]
        [Name("youtube search")]
        [Summary("Search YouTube for a video")]
        [Remarks("harlem shake compilation")]
        public async Task SearchCommand([Remainder, Summary("The search query. If left blank, fetches a random video")] string searchQuery = null)
        {
            await _service.SearchYouTube(Context, searchQuery);
        }
    }
}