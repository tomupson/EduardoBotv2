using Discord.Commands;
using EduardoBot.Services;
using EduardoBot.Common.Data;
using System.Threading.Tasks;

namespace EduardoBot.Modules
{
    [Group("imgur")]
    public class Imgur : ModuleBase<EduardoContext>
    {
        private readonly ImgurService _service;

        public Imgur(ImgurService service)
        {
            this._service = service;
        }

        [Command("find", RunMode = RunMode.Async), Alias("search"), Name("imgur find")]
        [Summary("Search for an image on Imgur.")]
        [Remarks("teddy bear")]
        public async Task FindCommand([Remainder, Summary("The Image you would like to search for. Leaving blank will fetch with a random image.")] string searchQuery = null)
        {
            await _service.SearchImgur(Context, searchQuery);
        }

        [Command("subreddit", RunMode = RunMode.Async), Alias("sr"), Name("imgur subreddit")]
        [Summary("Fetches a random image from an Imgur subreddit.")]
        [Remarks("pubattlegrounds")]
        public async Task SubredditCommand([Summary("The subreddit you wish to fetch a random image from.")] string subredditName)
        {
            await _service.FetchSubredditImage(Context, subredditName);
        }
    }
}