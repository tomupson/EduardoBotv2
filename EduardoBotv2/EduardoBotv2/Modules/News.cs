using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    public class News : ModuleBase<EduardoContext>
    {
        private readonly NewsService service;

        public News(NewsService service)
        {
            this.service = service;
        }

        [Command("news", RunMode = RunMode.Async)]
        [Summary("Get the top 5 headlines from a specific news source.")]
        [Remarks("bbc-news")]
        public async Task NewsCommand([Summary("The specified news source.")] string newsSource)
        {
            await service.GetNewsHeadlines(Context, newsSource);
        }

        [Command("sources", RunMode = RunMode.Async)]
        [Summary("View all available news sources.")]
        [Remarks("")]
        public async Task SourcesCommand()
        {
            await service.ShowNewsSources(Context);
        }
    }
}