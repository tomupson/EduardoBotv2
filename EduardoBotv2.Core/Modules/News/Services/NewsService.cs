using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.News.Models;
using EduardoBotv2.Core.Modules.Shorten.Helpers;
using EduardoBotv2.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EduardoBotv2.Core.Modules.News.Services
{
    public class NewsService : IEduardoService
    {
        private readonly NewsData _newsData;
        private readonly Credentials _credentials;

        public NewsService(Credentials credentials)
        {
            _newsData = JsonConvert.DeserializeObject<NewsData>(File.ReadAllText("data/news.json"));
            _credentials = credentials;
        }

        public async Task GetNewsHeadlines(EduardoContext context, string source)
        {
            if (!_newsData.NewsSources.Contains(source))
            {
                await context.Channel.SendMessageAsync($"**{source} is not a valid. Type `sources` to view available news sources**");
                return;
            }

            using (Stream responseStream = await NetworkHelper.GetStreamAsync($"https://newsapi.org/v1/articles?source={source}&sortBy=top&apiKey={_credentials.NewsApiKey}"))
            using (StreamReader sr = new StreamReader(responseStream))
            {
                string json = sr.ReadToEnd();

                JObject jResult = JObject.Parse(json);

                JArray jHeadlines = (JArray)jResult["articles"];
                List<EmbedFieldBuilder> headlines = new List<EmbedFieldBuilder>();

                int maxHeadlines = Math.Min(_newsData.MaxHeadlines, jHeadlines.Count - 1);
                for (int i = 0; i < maxHeadlines; i++)
                {
                    string shorten = await ShortenHelper.ShortenUrlAsync(_credentials.GoogleShortenerApiKey, jHeadlines[i]["url"].ToString());

                    headlines.Add(new EmbedFieldBuilder
                    {
                        Name = jHeadlines[i]["title"].ToString(),
                        Value = $"{jHeadlines[i]["description"]}\n({shorten})"
                    });
                }

                await context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .WithAuthor($"Latest News from {source.Replace('-', ' ').ToUpper()}",
                        @"http://shmector.com/_ph/18/412122157.png")
                    .WithThumbnailUrl(jResult["articles"][0]["urlToImage"].ToString())
                    .WithFields(headlines)
                    .WithFooter("News via newsapi.org",
                        @"https://pbs.twimg.com/profile_images/815237522641092609/6IeO3WLV.jpg")
                    .Build());
            }
        }

        public async Task ShowNewsSources(EduardoContext context)
        {
            await context.Channel.SendMessageAsync($"Available sources for the news command are:\n{string.Join(", ", _newsData.NewsSources)}");
        }
    }
}