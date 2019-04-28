using System;
using System.IO;
using System.Linq;
using Discord;
using EduardoBotv2.Core.Helpers;
using Microsoft.Extensions.Configuration;

namespace EduardoBotv2.Core.Models
{
    public class Credentials
    {
        public string Token { get; set; }

        public ulong[] OwnerIds { get; set; }

        public string GoogleShortenerApiKey { get; set; }

        public string ImgurClientId { get; set; }

        public string ImgurClientSecret { get; set; }

        public string NewsApiKey { get; set; }

        public string PUBGApiKey { get; set; }

        public string RedditRefreshToken { get; set; }

        public string RedditClientId { get;set; }

        public string RedditClientSecret { get; set; }

        public string RedditRedirectUri { get; set; }

        public string SpotifyRefreshToken { get; set; }

        public string SpotifyClientId { get; set; }

        public string SpotifyClientSecret { get; set; }

        public string SpotifyRedirectUri { get; set; }

        public string DbConnectionString { get; set; }

        public Credentials()
        {
            string credentialsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");
            if (!File.Exists(credentialsFilePath))
            {
                Logger.Log("credentials.json is missing", LogSeverity.Critical);
            }

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(credentialsFilePath);

            IConfigurationRoot data = configurationBuilder.Build();

            Token = data[nameof(Token)];
            if (string.IsNullOrWhiteSpace(Token))
            {
                Logger.Log("Token is missing from credentials.json", LogSeverity.Critical);
                Environment.Exit(0);
            }

            OwnerIds = data.GetSection(nameof(OwnerIds)).GetChildren().Select(id => ulong.Parse(id.Value)).ToArray();

            GoogleShortenerApiKey = data[nameof(GoogleShortenerApiKey)];

            ImgurClientId = data[nameof(ImgurClientId)];
            ImgurClientSecret = data[nameof(ImgurClientSecret)];

            NewsApiKey = data[nameof(NewsApiKey)];

            PUBGApiKey = data[nameof(PUBGApiKey)];

            RedditRefreshToken = data[nameof(RedditRefreshToken)];
            RedditClientId = data[nameof(RedditClientId)];
            RedditClientSecret = data[nameof(RedditClientSecret)];
            RedditRedirectUri = data[nameof(RedditRedirectUri)];

            SpotifyRefreshToken = data[nameof(SpotifyRefreshToken)];
            SpotifyClientId = data[nameof(SpotifyClientId)];
            SpotifyClientSecret = data[nameof(SpotifyClientSecret)];
            SpotifyRedirectUri = data[nameof(SpotifyRedirectUri)];

            DbConnectionString = data[nameof(DbConnectionString)];
        }
    }
}