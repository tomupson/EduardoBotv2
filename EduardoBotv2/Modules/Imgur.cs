using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    [Group("imgur")]
    public class Imgur : ModuleBase<EduardoContext>
    {
        private readonly ImgurService service;

        public Imgur(ImgurService service)
        {
            this.service = service;
        }

        [Command("find", RunMode = RunMode.Async), Alias("search"), Name("imgur find")]
        [Summary("Search for an image on Imgur.")]
        [Remarks("teddy bear")]
        public async Task FindCommand([Remainder, Summary("The Image you would like to search for. Leaving blank will fetch with a random image.")] string searchQuery = null)
        {
            await service.SearchImgur(Context, searchQuery);
        }

        [Command("subreddit", RunMode = RunMode.Async), Alias("sr"), Name("imgur subreddit")]
        [Summary("Fetches a random image from an Imgur subreddit.")]
        [Remarks("pubattlegrounds")]
        public async Task SubredditCommand([Summary("The subreddit you wish to fetch a random image from.")] string subredditName)
        {
            await service.FetchSubredditImage(Context, subredditName);
        }
    }
}