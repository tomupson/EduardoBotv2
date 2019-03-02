using System;
using System.Threading.Tasks;
using EduardoBotv2.Core.Models.Enums;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace EduardoBotv2.Core.Helpers
{
    public static class GoogleHelper
    {
        private static YouTubeService youTubeService;
        private static UrlshortenerService urlShortenerService;

        public static YouTubeService CreateYouTubeService(string apiKey) => youTubeService = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = apiKey,
            ApplicationName = "EduardoBot"
        });

        public static UrlshortenerService CreateShortenerService(string apiKey) => urlShortenerService = new UrlshortenerService(new BaseClientService.Initializer
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

        public static async Task<VideoListResponse> GetVideoFromYouTubeAsync(string apiKey, string part, string videoId, int maxResults)
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

        public static async Task<string> ShortenUrlAsync(string apiKey, string longUrl)
        {
            return (await Task.Run(() =>
            {
                UrlshortenerService service = urlShortenerService ?? CreateShortenerService(apiKey);

                Url original = new Url
                {
                    LongUrl = longUrl
                };

                return service.Url.Insert(original).ExecuteAsync();
            })).Id;
        }

        public static async Task<string> UnshortenUrlAsync(string apiKey, string shortUrl)
        {
            return (await Task.Run(() =>
            {
                UrlshortenerService service = urlShortenerService ?? CreateShortenerService(apiKey);

                return service.Url.Get(shortUrl).ExecuteAsync();
            })).LongUrl;
        }
    }
}