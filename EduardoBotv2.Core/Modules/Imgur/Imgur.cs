using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Imgur.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Imgur
{
    [Group("imgur")]
    public class Imgur : EduardoModule
    {
        private readonly ImgurService _service;

        public Imgur(ImgurService service)
        {
            _service = service;
        }

        [Command("search")]
        [Name("imgur search")]
        [Summary("Search for an image on Imgur")]
        [Remarks("teddy bear")]
        public async Task FindCommand([Remainder, Summary("The image to search for. Leaving blank will fetch a random image")] string searchQuery = null)
        {
            await _service.SearchImgur(Context, searchQuery);
        }

        [Command("subreddit")]
        [Alias("sub")]
        [Name("imgur subreddit")]
        [Summary("Fetches a random image from an Imgur subreddit")]
        [Remarks("pubattlegrounds")]
        public async Task SubredditCommand([Summary("The subreddit you wish to fetch a random image from")] string subredditName)
        {
            await _service.FetchSubredditImage(Context, subredditName);
        }
    }
}