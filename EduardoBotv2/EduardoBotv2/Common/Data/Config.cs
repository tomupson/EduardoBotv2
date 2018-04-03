using System;
using System.Collections.Generic;

namespace EduardoBotv2.Common.Data
{
    public static class Config
    {
        public const string DEFAULT_PREFIX = "$";
        public const int MIN_CHAR_LENGTH = 7;
        public const int MAX_DESCRIPTION_LENGTH = 250;

        public const string YOUTUBE_LINK_REGEX = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";

        public static readonly TimeSpan PAGINATION_TIMEOUT_TIME = new TimeSpan(0, 0, 15);
        public static readonly List<string> NEWS_SOURCES = new List<string>()
        {
            "ars-technica", "associated-press", "bbc-news", "bbc-sport", "bloomberg", "breitbart-news", "business-insider", "business-insider-uk", "buzzfeed",
            "cnbc", "cnn", "daily-mail", "espn", "financial-times", "google-news", "ign", "independent", "mashable", "metro", "mirror", "mtv-news", "mtv-news-uk",
            "national-geographic", "polygon", "reddit-r-all", "techcrunch", "techradar", "the-economist", "the-guardian-uk", "the-huffington-post", "the-lad-bible",
            "the-new-york-times", "the-sport-bible", "the-telegraph", "the-verge", "the-wall-street-journal", "the-washington-post", "time", "usa-today"
        };

        public const int MAX_HEADLINES = 5;

        //**GAMES**//
        public const int ALL_POKEMON_COUNT = 721;
        public const int MAX_POKEMON_PER_PAGE = 6;

        public static readonly List<string> SPEEDTEST_WORDS = new List<string> { "speedtest", "typeveryfast", "word", "quickly!", "gg" };
        public static readonly List<string> EIGHT_BALL_WORDS = new List<string>
        {
            "It is certain.",
            "It is decidedly so.",
            "Without a doubt.",
            "Yes definitely.",
            "You may rely on it.",
            "As I see it, yes.",
            "Most likely.",
            "Outlook good.",
            "Yes.",
            "Signs point to yes.",
            "Reply hazy try again.",
            "Ask again later.",
            "Better not tell you now.",
            "Cannot predict now.",
            "Concentrate and ask again.",
            "Don't count on it.",
            "My reply is no.",
            "My sources say no.",
            "Outlook not so good.",
            "Very doubtful."
        };

        // Fortnite API
        public const string FORTNITE_LAUNCHER_TOKEN = "MzRhMDJjZjhmNDQxNGUyOWIxNTkyMTg3NmRhMzZmOWE6ZGFhZmJjY2M3Mzc3NDUwMzlkZmZlNTNkOTRmYzc2Y2Y==";
        public const string FORTNITE_CLIENT_TOKEN = "ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ==";
        // OAUTH URLS
        public const string FORTNITE_OAUTH_TOKEN = "https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/token";
        public const string FORTNITE_OAUTH_EXCHANGE = "https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/exchange";
        public const string FORTNITE_OAUTH_VERIFY = "https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/verify?includePerms=true";
        // API URLS
        public const string FORTNITE_STORE = "https://fortnite-public-service-prod11.ol.epicgames.com/fortnite/api/storefront/v2/catalog";
        public const string FORTNITE_SERVER_STATUS = "https://lightswitch-public-service-prod06.ol.epicgames.com/lightswitch/api/service/bulk/status?serviceId=Fortnite";
        public static string FORTNITE_PLAYER_LOOKUP(string username) => "https://persona-public-service-prod06.ol.epicgames.com/persona/api/public/account/lookup?q=" + username;
        public const string FORTNITE_NEWS = "https://fortnitecontent-website-prod07.ol.epicgames.com/content/api/pages/fortnite-game";
        // PVP
        public static string FORTNITE_STATS_BR(string accountId) => "https://fortnite-public-service-prod11.ol.epicgames.com/fortnite/api/stats/accountId/" + accountId + "/bulk/window/alltime";
        // PVE
        public const string FORTNITE_PVE_INFO = "https://fortnite-public-service-prod11.ol.epicgames.com/fortnite/api/game/v2/world/info";
        public static string FORTNITE_PROFILE_LOOKUP(string accountId) => "https://fortnite-public-service-prod11.ol.epicgames.com/fortnite/api/game/v2/profile/" + accountId + "/public/QueryProfile";

        // PUBG API
        public static string PUBG_PLAYER_LOOKUP(string platform, string region, string username) => "https://api.playbattlegrounds.com/shards/" + platform + "-" + region + "/players?filter[playerNames]=" + username;
        public static string PUBG_MATCH_LOOKUP(string platform, string region, string matchId) => "https://api.playbattlegrounds.com/shards/" + platform + "-" + region + "/matches/" + matchId;
        public const string PUBG_API_STATUS = "https://api.playbattlegrounds.com/status";
    }
}