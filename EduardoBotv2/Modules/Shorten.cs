using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    public class Shorten : ModuleBase<EduardoContext>
    {
        private readonly ShortenService service;
        public Shorten(ShortenService service)
        {
            this.service = service;
        }

        [Command("shorten", RunMode = RunMode.Async)]
        [Summary("Shorten any url into a goo.gl link.")]
        [Remarks("http://www.404errorpages.com/")]
        public async Task ShortenCommand(string url)
        {
            await service.Shorten(Context, url);
        }

        [Command("shortenyt", RunMode = RunMode.Async), Alias("shortenyoutube")]
        [Summary("Shortens YouTube links.")]
        [Remarks("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
        public async Task YouTubeCommand(string url)
        {
            await service.ShortenYouTube(Context, url);
        }

        [Command("unshorten", RunMode = RunMode.Async), Alias("us")]
        [Summary("Unshortens a shortened goo.gl url.")]
        [Remarks("https://goo.gl/MY6gDD")]
        public async Task UnshortenCommand(string url)
        {
            await service.Unshorten(Context, url);
        }
    }
}