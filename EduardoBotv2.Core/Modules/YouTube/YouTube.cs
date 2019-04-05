using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.YouTube.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.YouTube
{
    [Group("youtube")]
    [Name("YouTube")]
    public class YouTube : EduardoModule<YouTubeService>
    {
        public YouTube(YouTubeService service)
            : base(service) { }

        [Command("search")]
        [Summary("Search YouTube for a video")]
        [Remarks("harlem shake compilation")]
        public async Task SearchCommand([Summary("The search query. If left blank, fetches a random video"), Remainder] string searchQuery = null)
        {
            await _service.SearchYouTubeAsync(Context, searchQuery);
        }
    }
}