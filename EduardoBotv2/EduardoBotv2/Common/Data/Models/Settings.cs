namespace EduardoBot.Common.Data.Models
{
    public class Settings
    {
        // General
        public string Token { get; set; }
        public ulong[] OwnerIds { get; set; }
        public string MongoDBConnectionString { get; set; }
        public string DatabaseName { get; set; }

        // Google
        public string GoogleYouTubeApiKey { get; set; }
        public string GoogleShortenerApiKey { get; set; }

        // Imgur
        public string ImgurClientId { get; set; }
        public string ImgurClientSecret { get; set; }

        // NewsApi.org
        public string NewsApiKey { get; set; }
    }
}