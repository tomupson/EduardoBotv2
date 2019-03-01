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

        public string GoogleYouTubeApiKey { get; set; }

        public string GoogleShortenerApiKey { get; set; }

        public string ImgurClientId { get; set; }

        public string ImgurClientSecret { get; set; }

        public string NewsApiKey { get; set; }

        public string PUBGApiKey { get; set; }

        public Credentials()
        {
            string credentialsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");
            if (!File.Exists(credentialsFilePath))
            {
                Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo", "credentials.json is missing"));
            }

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(credentialsFilePath);

            IConfigurationRoot data = configurationBuilder.Build();

            Token = data[nameof(Token)];
            if (string.IsNullOrWhiteSpace(Token))
            {
                Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo", "Token is missing from credentials.json"));
                Environment.Exit(0);
            }

            OwnerIds = data.GetSection(nameof(OwnerIds)).GetChildren().Select(id => ulong.Parse(id.Value)).ToArray();
            GoogleYouTubeApiKey = data[nameof(GoogleYouTubeApiKey)];
            GoogleShortenerApiKey = data[nameof(GoogleShortenerApiKey)];
            ImgurClientId = data[nameof(ImgurClientId)];
            ImgurClientSecret = data[nameof(ImgurClientSecret)];
            NewsApiKey = data[nameof(NewsApiKey)];
            PUBGApiKey = data[nameof(PUBGApiKey)];
        }
    }
}