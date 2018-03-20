using Discord.Commands;
using EduardoBot.Services;
using EduardoBot.Common.Data;
using System.Threading.Tasks;

namespace EduardoBot.Modules
{
    public class Shortenn : ModuleBase<EduardoContext>
    {
        private readonly ShortenService _service;
        public Shortenn(ShortenService service)
        {
            this._service = service;
        }

        [Command("shorten", RunMode = RunMode.Async)]
        [Summary("Shorten any url into a goo.gl link.")]
        [Remarks("http://www.404errorpages.com/")]
        public async Task ShortenCommand(string url)
        {
            await _service.Shorten(Context, url);
        }

        [Command("shortenyt", RunMode = RunMode.Async), Alias("shortenyoutube")]
        [Summary("Shortens YouTube links.")]
        [Remarks("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
        public async Task YouTubeCommand(string url)
        {
            await _service.ShortenYouTube(Context, url);
        }

        [Command("unshorten", RunMode = RunMode.Async), Alias("us")]
        [Summary("Unshortens a shortened goo.gl url.")]
        [Remarks("https://goo.gl/MY6gDD")]
        public async Task UnshortenCommand(string url)
        {
            await _service.Unshorten(Context, url);
        }
    }
}
