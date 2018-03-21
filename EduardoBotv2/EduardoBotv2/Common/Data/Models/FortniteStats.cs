using Newtonsoft.Json;

namespace EduardoBotv2.Common.Data.Models
{
    public class FortniteStats
    {
        [JsonProperty("accountId")]
        public string accountId { get; set; }
        [JsonProperty("platformNameLong")]
        public string platform { get; set; }
        [JsonProperty("epicUserHandle")]
        public string epicUsername { get; set; }
        [JsonProperty("stats")]
        public FortniteGamemodesStats gamemodes { get; set; }
    }

    public class FortniteGamemodesStats
    {
        [JsonProperty("p2")]
        public FortniteGamemodeStats lifetimeSolo { get; set; }
        [JsonProperty("curr_p2")]
        public FortniteGamemodeStats currentSeasonSolo { get; set; }
        [JsonProperty("p10")]
        public FortniteGamemodeStats lifetimeDuo { get; set; }
        [JsonProperty("curr_p10")]
        public FortniteGamemodeStats currentSeasonDuo { get; set; }
        [JsonProperty("p9")]
        public FortniteGamemodeStats lifetimeSquad { get; set; }
        [JsonProperty("curr_p9")]
        public FortniteGamemodeStats currentSeasonSquad { get; set; }
    }

    public class FortniteGamemodeStats
    {
        [JsonProperty("trnRating")]
        public FortniteFieldInfo trnRating { get; set; }
        [JsonProperty("score")]
        public FortniteFieldInfo score { get; set; }
        [JsonProperty("top1")]
        public FortniteFieldInfo wins { get; set; }
        [JsonProperty("top3")]
        public FortniteFieldInfo top3s { get; set; }
        [JsonProperty("top5")]
        public FortniteFieldInfo top5s { get; set; }
        [JsonProperty("top6")]
        public FortniteFieldInfo top6s { get; set; }
        [JsonProperty("top10")]
        public FortniteFieldInfo top10s { get; set; }
        [JsonProperty("top12")]
        public FortniteFieldInfo top12s { get; set; }
        [JsonProperty("top25")]
        public FortniteFieldInfo top25s { get; set; }
        [JsonProperty("kd")]
        public FortniteFieldInfo kdRatio { get; set; }
        [JsonProperty("matches")]
        public FortniteFieldInfo matchesPlayed { get; set; }
        [JsonProperty("kills")]
        public FortniteFieldInfo totalKills { get; set; }
        [JsonProperty("kpg")]
        public FortniteFieldInfo killsPerGame { get; set; }
        [JsonProperty("avgTimePlayed")]
        public FortniteFieldInfo averageTimePlayed { get; set; }
        [JsonProperty("scorePerMatch")]
        public FortniteFieldInfo scorePerGame { get; set; }
    }

    public class FortniteFieldInfo
    {
        [JsonProperty("label")]
        public string label { get; set; }
        [JsonProperty("field")]
        public string field { get; set; }
        [JsonProperty("valueInt")]
        public int value { get; set; }
        [JsonProperty("displayValue")]
        public string displayValue { get; set; }
    }
}