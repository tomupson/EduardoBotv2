using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Imgur.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Imgur
{
    [Group("imgur")]
    [Name("Imgur")]
    public class Imgur : EduardoModule<ImgurService>
    {
        public Imgur(ImgurService service)
            : base(service) { }

        [Command("search")]
        [Summary("Search for an image on Imgur")]
        [Remarks("teddy bear")]
        public async Task FindCommand([Remainder, Summary("The image to search for. Leaving blank will fetch a random image")] string searchQuery = null)
        {
            await _service.SearchImgur(Context, searchQuery);
        }

        [Command("subreddit")]
        [Alias("sub")]
        [Summary("Fetches a random image from an Imgur subreddit")]
        [Remarks("pubattlegrounds")]
        public async Task SubredditCommand([Summary("The subreddit you wish to fetch a random image from")] string subredditName)
        {
            await _service.FetchSubredditImage(Context, subredditName);
        }
    }
}