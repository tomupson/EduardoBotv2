using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;
using YoutubeExplode.Models;

namespace EduardoBotv2.Core.Modules.YouTube.Services
{
    public class YouTubeService : IEduardoService
    {
        public async Task SearchYouTubeAsync(EduardoContext context, string searchQuery = null)
        {
            if (searchQuery != null)
            {
                List<Video> videos = await VideoHelper.GetOrSearchVideoAsync(searchQuery);

                List<Embed> pageEmbeds = videos.Select((video, index) => new EmbedBuilder()
                    .WithTitle(video.Title)
                    .WithColor(Color.Red)
                    .WithDescription(video.Description)
                    .WithThumbnailUrl(video.Thumbnails.StandardResUrl)
                    .WithUrl($"http://youtu.be/{video.Id}")
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