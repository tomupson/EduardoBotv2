using Discord;
using EduardoBot.Common.Data;
using EduardoBot.Common.Data.Enums;
using EduardoBot.Common.Extensions;
using EduardoBot.Common.Data.Models;
using EduardoBot.Common.Utilities.Helpers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduardoBot.Services
{
    public class YouTubeModuleService
    {
        public async Task SearchYouTube(EduardoContext c, string searchQuery = null)
        {
            if (searchQuery != null)
            {
                var searchVideosResponse = await GoogleHelper.SearchYouTubeAsync(c.EduardoSettings.GoogleYouTubeApiKey, "snippet", searchQuery, 5, YouTubeRequestType.video);
                
                List<Embed> pageEmbeds = new List<Embed>();
                for (var i = 0; i < searchVideosResponse.Items.Count; i++)
                {
                    pageEmbeds.Add(new EmbedBuilder()
                    {
                        Color = Color.Red,
                        Title = searchVideosResponse.Items[i].Snippet.Title,
                        Description = searchVideosResponse.Items[i].Snippet.Description,
                        ThumbnailUrl = searchVideosResponse.Items[i].Snippet.Thumbnails.Default__.Url,

                        Footer = new EmbedFooterBuilder()
                        {
                            IconUrl = @"https://seeklogo.com/images/Y/youtube-icon-logo-521820CDD7-seeklogo.com.png",
                            Text = $"Page {i + 1}"
                        },
                        Url = $"http://youtu.be/{searchVideosResponse.Items[i].Id.VideoId}"
                    }.Build());
                }

                await c.SendPaginatedMessageAsync(new PaginatedMessage()
                {
                    Embeds = pageEmbeds,
                    Timeout = Config.PAGINATION_TIMEOUT_TIME,
                    TimeoutBehaviour = TimeoutBehaviour.Delete
                });
            }
        }
    }
}