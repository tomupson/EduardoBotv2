using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Models.Enums;
using Google.Apis.YouTube.v3.Data;

namespace EduardoBotv2.Core.Services
{
    public class YouTubeModuleService
    {
        public async Task SearchYouTube(EduardoContext context, string searchQuery = null)
        {
            if (searchQuery != null)
            {
                SearchListResponse searchVideosResponse = await GoogleHelper.SearchYouTubeAsync(context.EduardoCredentials.GoogleYouTubeApiKey, "snippet", searchQuery, 5, YouTubeRequestType.Video);
                
                List<Embed> pageEmbeds = searchVideosResponse.Items.Select((t, i) => new EmbedBuilder
                {
                    Color = Color.Red,
                    Title = t.Snippet.Title,
                    Description = t.Snippet.Description,
                    ThumbnailUrl = t.Snippet.Thumbnails.Default__.Url,

                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = @"https://seeklogo.com/images/Y/youtube-icon-logo-521820CDD7-seeklogo.com.png",
                        Text = $"Page {i + 1}"
                    },
                    Url = $"http://youtu.be/{t.Id.VideoId}"
                }.Build()).ToList();

                await context.SendPaginatedMessageAsync(new PaginatedMessage
                {
                    Embeds = pageEmbeds,
                    Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                    TimeoutBehaviour = TimeoutBehaviour.Delete
                });
            }
        }
    }
}