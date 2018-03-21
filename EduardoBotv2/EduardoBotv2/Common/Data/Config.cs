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
    }
}