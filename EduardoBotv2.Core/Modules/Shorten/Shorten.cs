using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Shorten.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Shorten
{
    public class Shorten : EduardoModule
    {
        private readonly ShortenService _service;
        public Shorten(ShortenService service)
        {
            _service = service;
        }

        [Command("shorten")]
        [Summary("Shorten any url into a goo.gl link")]
        [Remarks("http://www.404errorpages.com/")]
        public async Task ShortenCommand(string url)
        {
            await _service.Shorten(Context, url);
        }

        [Command("shortenyt")]
        [Alias("shortenyoutube")]
        [Summary("Shortens YouTube links")]
        [Remarks("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
        public async Task YouTubeCommand(string url)
        {
            await _service.ShortenYouTube(Context, url);
        }

        [Command("unshorten")]
        [Summary("Unshortens a shortened goo.gl url")]
        [Remarks("https://goo.gl/MY6gDD")]
        public async Task UnshortenCommand(string url)
        {
            await _service.Unshorten(Context, url);
        }
    }
}