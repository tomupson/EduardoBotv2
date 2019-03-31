using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.YouTube.Helpers;
using EduardoBotv2.Core.Modules.YouTube.Models;
using EduardoBotv2.Core.Services;
using Google.Apis.YouTube.v3.Data;

namespace EduardoBotv2.Core.Modules.YouTube.Services
{
    public class YouTubeService : IEduardoService
    {
        private readonly Credentials _credentials;

        public YouTubeService(Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task SearchYouTubeAsync(EduardoContext context, string searchQuery = null)
        {
            if (searchQuery != null)
            {
                SearchListResponse searchVideosResponse = await YouTubeHelper.SearchYouTubeAsync(_credentials.GoogleYouTubeApiKey, "snippet", searchQuery, 5, YouTubeRequestType.Video);

                List<Embed> pageEmbeds = searchVideosResponse.Items.Select((video, index) => new EmbedBuilder()
                    .WithTitle(video.Snippet.Title)
                    .WithColor(Color.Red)
                    .WithDescription(video.Snippet.Description)
                    .WithThumbnailUrl(video.Snippet.Thumbnails.Default__.Url)
                    .WithUrl($"http://youtu.be/{video.Id.VideoId}")
                    .WithFooter($"Page {index + 1}",
                        @"https://seeklogo.com/images/Y/youtube-icon-logo-521820CDD7-seeklogo.com.png")
                    .Build()).ToList();

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