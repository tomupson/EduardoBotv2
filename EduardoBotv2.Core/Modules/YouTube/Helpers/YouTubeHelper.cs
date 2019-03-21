using System;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.YouTube.Models;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace EduardoBotv2.Core.Modules.YouTube.Helpers
{
    public static class YouTubeHelper
    {
        private static YouTubeService youTubeService;

        public static YouTubeService CreateYouTubeService(string apiKey) => youTubeService = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = apiKey,
            ApplicationName = "EduardoBot"
        });

        public static async Task<SearchListResponse> SearchYouTubeAsync(string apiKey, string part, string searchQuery, int maxResults, YouTubeRequestType type)
        {
            return await Task.Run(() =>
            {
                YouTubeService service = youTubeService ?? CreateYouTubeService(apiKey);
                SearchResource.ListRequest searchVideoRequest = service.Search.List(part);
                searchVideoRequest.Q = searchQuery;
                searchVideoRequest.MaxResults = maxResults;
                searchVideoRequest.Type = Enum.GetName(typeof(YouTubeRequestType), type).ToLower();

                return searchVideoRequest.ExecuteAsync();
            });
        }

        public static async Task<VideoListResponse> GetVideoFromYouTubeAsync(string apiKey, string part, string videoId)
        {
            return await Task.Run(() =>
            {
                YouTubeService service = youTubeService ?? CreateYouTubeService(apiKey);
                VideosResource.ListRequest getVideoByIdRequest = service.Videos.List(part);
                getVideoByIdRequest.MaxResults = 1;
                getVideoByIdRequest.Id = videoId;

                return getVideoByIdRequest.ExecuteAsync();
            });
        }
    }
}