using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;

namespace EduardoBotv2.Core.Modules.Shorten.Helpers
{
    public static class ShortenHelper
    {
        private static UrlshortenerService urlShortenerService;

        public static UrlshortenerService CreateShortenerService(string apiKey) => urlShortenerService = new UrlshortenerService(new BaseClientService.Initializer
        {
            ApiKey = apiKey,
            ApplicationName = "EduardoBot"
        });

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