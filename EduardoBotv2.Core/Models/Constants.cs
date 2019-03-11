namespace EduardoBotv2.Core.Models
{
    public static class Constants
    {
        public const string CMD_PREFIX = ".";
        public const int MAX_DESCRIPTION_LENGTH = 250;

        public const string YOUTUBE_LINK_REGEX = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";

        public const int PAGINATION_TIMEOUT_SECONDS = 15;

        public static string PUBG_PLAYER_LOOKUP(string platform, string region, string username) => $"https://api.playbattlegrounds.com/shards/{platform}-{region}/players?filter[playerNames]={username}";
        public static string PUBG_PLAYER_LOOKUP_WITH_ID(string platform, string region, string id) => $"https://api.playbattlegrounds.com/shards/{platform}-{region}/players/{id}";
        public static string PUBG_MATCH_LOOKUP(string platform, string region, string matchId) => $"https://api.playbattlegrounds.com/shards/{platform}-{region}/matches/{matchId}";
        public const string PUBG_API_STATUS = "https://api.playbattlegrounds.com/status";
    }
}