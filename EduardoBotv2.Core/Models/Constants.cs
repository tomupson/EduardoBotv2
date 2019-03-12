namespace EduardoBotv2.Core.Models
{
    public static class Constants
    {
        public const string CMD_PREFIX = ".";
        public const int MAX_DESCRIPTION_LENGTH = 250;

        public const string YOUTUBE_LINK_REGEX = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";

        public const int PAGINATION_TIMEOUT_SECONDS = 15;
    }
}