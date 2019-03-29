using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.News.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.News
{
    public class News : EduardoModule
    {
        private readonly NewsService _service;

        public News(NewsService service)
        {
            _service = service;
        }

        [Command("news")]
        [Summary("Get the top 5 headlines from a specific news source")]
        [Remarks("bbc-news")]
        public async Task NewsCommand([Summary("The specified news source")] string newsSource)
        {
            await _service.GetNewsHeadlines(Context, newsSource);
        }

        [Command("sources")]
        [Summary("View all available news sources")]
        public async Task SourcesCommand()
        {
            await _service.ShowNewsSources(Context);
        }
    }
}