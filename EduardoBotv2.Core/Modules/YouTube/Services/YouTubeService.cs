using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.YouTube.Helpers;
using EduardoBotv2.Core.Modules.YouTube.Models;
using Google.Apis.YouTube.v3.Data;

namespace EduardoBotv2.Core.Modules.YouTube.Services
{
    public class YouTubeService
    {
        private readonly Credentials _credentials;

        public YouTubeService(Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task SearchYouTube(EduardoContext context, string searchQuery = null)
        {
            if (searchQuery != null)
            {
                SearchListResponse searchVideosResponse = await YouTubeHelper.SearchYouTubeAsync(_credentials.GoogleYouTubeApiKey, "snippet", searchQuery, 5, YouTubeRequestType.Video);
                
                List<Embed> pageEmbeds = searchVideosResponse.Items.Select((video, index) => new EmbedBuilder
                {
                    Color = Color.Red,
                    Title = video.Snippet.Title,
                    Description = video.Snippet.Description,
                    ThumbnailUrl = video.Snippet.Thumbnails.Default__.Url,

                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = @"https://seeklogo.com/images/Y/youtube-icon-logo-521820CDD7-seeklogo.com.png",
                        Text = $"Page {index + 1}"
                    },
                    Url = $"http://youtu.be/{video.Id.VideoId}"
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