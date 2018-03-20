using Discord.Commands;
using EduardoBot.Services;
using EduardoBot.Common.Data;
using System.Threading.Tasks;

namespace EduardoBot.Modules
{
    public class News : ModuleBase<EduardoContext>
    {
        private readonly NewsService _service;

        public News(NewsService service)
        {
            this._service = service;
        }

        [Command("news", RunMode = RunMode.Async)]
        [Summary("Get the top 5 headlines from a specific news source.")]
        [Remarks("bbc-news")]
        public async Task NewsCommand([Summary("The specified news source.")] string newsSource)
        {
            await _service.GetNewsHeadlines(Context, newsSource);
        }

        [Command("sources", RunMode = RunMode.Async)]
        [Summary("View all available news sources.")]
        [Remarks("")]
        public async Task SourcesCommand()
        {
            await _service.ShowNewsSources(Context);
        }
    }
}
