using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using EduardoBotv2.Common.Data.Enums;
using System;
using System.Threading.Tasks;

namespace EduardoBotv2.Common.Utilities.Helpers
{
    public static class GoogleHelper
    {
        public static YouTubeService CreateYouTubeService(string apiKey)
        {
            return new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "EduardoBot"
            });
        }

        public static UrlshortenerService CreateShortenerService(string apiKey)
        {
            return new UrlshortenerService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "EduardoBot"
            });
        }

        public static async Task<SearchListResponse> SearchYouTubeAsync(string apiKey, string part, string searchQuery, int maxResults, YouTubeRequestType type)
        {
            return await Task.Run(() =>
            {
                var service = CreateYouTubeService(apiKey);

                var searchVideoRequest = service.Search.List(part);
                searchVideoRequest.Q = searchQuery;
                searchVideoRequest.MaxResults = maxResults;
                searchVideoRequest.Type = Enum.GetName(typeof(YouTubeRequestType), type);

                var response = searchVideoRequest.ExecuteAsync();
                return response;
            });
        }

        public static async Task<VideoListResponse> GetVideoFromYouTubeAsync(string apiKey, string part, string videoId, int maxResults)
        {
            return await Task.Run(() =>
            {
                var service = CreateYouTubeService(apiKey);

                var getVideoByIdRequest = service.Videos.List(part);
                getVideoByIdRequest.MaxResults = 1;
                getVideoByIdRequest.Id = videoId;

                var response = getVideoByIdRequest.ExecuteAsync();
                return response;
            });
        }

        public static Task<string> ShortenUrlAsync(string apiKey, string longUrl)
        {
            return Task.Run(() =>
            {
                var service = CreateShortenerService(apiKey);

                var original = new Url();
                original.LongUrl = longUrl;
                var shorten = service.Url.Insert(original).Execute().Id;

                return shorten;
            });
        }

        public static Task<string> UnshortenUrlAsync(string apiKey, string shortUrl)
        {
            return Task.Run(() =>
            {
                var service = CreateShortenerService(apiKey);

                var unshorten = service.Url.Get(shortUrl).Execute().LongUrl;

                return unshorten;
            });
        }
    }
}