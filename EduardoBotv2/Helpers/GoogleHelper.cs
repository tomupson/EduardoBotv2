using System;
using System.Threading.Tasks;
using EduardoBotv2.Models.Enums;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace EduardoBotv2.Helpers
{
    public static class GoogleHelper
    {
        public static YouTubeService CreateYouTubeService(string apiKey) => new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = apiKey,
            ApplicationName = "EduardoBot"
        });

        public static UrlshortenerService CreateShortenerService(string apiKey) => new UrlshortenerService(new BaseClientService.Initializer
        {
            ApiKey = apiKey,
            ApplicationName = "EduardoBot"
        });

        public static async Task<SearchListResponse> SearchYouTubeAsync(string apiKey, string part, string searchQuery, int maxResults, YouTubeRequestType type)
        {
            return await Task.Run(() =>
            {
                YouTubeService service = CreateYouTubeService(apiKey);

                SearchResource.ListRequest searchVideoRequest = service.Search.List(part);
                searchVideoRequest.Q = searchQuery;
                searchVideoRequest.MaxResults = maxResults;
                searchVideoRequest.Type = Enum.GetName(typeof(YouTubeRequestType), type).ToLower();

                Task<SearchListResponse> response = searchVideoRequest.ExecuteAsync();
                return response;
            });
        }

        public static async Task<VideoListResponse> GetVideoFromYouTubeAsync(string apiKey, string part, string videoId, int maxResults)
        {
            return await Task.Run(() =>
            {
                YouTubeService service = CreateYouTubeService(apiKey);

                VideosResource.ListRequest getVideoByIdRequest = service.Videos.List(part);
                getVideoByIdRequest.MaxResults = 1;
                getVideoByIdRequest.Id = videoId;

                Task<VideoListResponse> response = getVideoByIdRequest.ExecuteAsync();
                return response;
            });
        }

        public static Task<string> ShortenUrlAsync(string apiKey, string longUrl)
        {
            return Task.Run(() =>
            {
                UrlshortenerService service = CreateShortenerService(apiKey);

                var original = new Url
                {
                    LongUrl = longUrl
                };

                string shorten = service.Url.Insert(original).Execute().Id;

                return shorten;
            });
        }

        public static Task<string> UnshortenUrlAsync(string apiKey, string shortUrl)
        {
            return Task.Run(() =>
            {
                UrlshortenerService service = CreateShortenerService(apiKey);

                string unshorten = service.Url.Get(shortUrl).Execute().LongUrl;

                return unshorten;
            });
        }
    }
}