using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Models.Enums;
using Newtonsoft.Json.Linq;

namespace EduardoBotv2.Core.Services
{
    public class PUBGService
    {
        private readonly Credentials credentials;

        public PUBGService(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public async Task GetPlayer(EduardoContext context, string username, string platformRegionString)
        {
            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out PUBGPlatformRegion platformRegion))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject playerJson = await GetPlayerFromApi(username, platform, region);
                    EmbedBuilder builder = new EmbedBuilder
                    {
                        Color = Color.Orange,
                        Description = "You can view your recent matches by using the `$pubgmatches` command",
                        Fields = new List<EmbedFieldBuilder>
                        {
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Account ID",
                                Value = playerJson["data"][0]["attributes"]["id"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Last Updated",
                                Value = playerJson["data"][0]["attributes"]["updatedAt"]
                            }
                        },
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400",
                            Text = $"Player Profile for {playerJson["data"][0]["attributes"]["name"]}"
                        },
                        Title = playerJson["data"][0]["attributes"]["name"].ToString()
                    };

                    await context.Channel.SendMessageAsync(embed: builder.Build());
                }
            } else
            {
                await context.Channel.SendMessageAsync("Invalid platform-region. You can view valid options with `$pubgvalids`");
            }
        }

        public async Task GetMatches(EduardoContext context, string username, string platformRegionString)
        {
            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out PUBGPlatformRegion platformRegion))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject lookupJson = await GetPlayerFromApi(username, platform, region);
                    File.WriteAllText(@"D:\Thomas\Documents\ExamplePlayer.json", lookupJson.ToString());
                    JToken matchesJson = lookupJson["data"][0]["relationships"]["matches"]["data"];

                    List<Embed> pageEmbeds = new List<Embed>();

                    foreach (JToken match in matchesJson)
                    {
                        if (match["type"].ToString() == "match")
                        {
                            JObject matchJson = await GetMatchFromApi(match["id"].ToString(), platform, region);
                            TimeSpan t = TimeSpan.FromSeconds((int)matchJson["data"]["attributes"]["duration"]);
                            List<JToken> participants = matchJson["included"].ToObject<JArray>().Where(x => x["type"].ToString() == "participant").ToList();
                            JToken me = participants.FirstOrDefault(x => x["attributes"]["stats"]["playerId"].ToString() == lookupJson["data"][0]["id"].ToString());
                            JToken winner = participants.FirstOrDefault(x => (int)x["attributes"]["stats"]["winPlace"] == 1);
                            string winnerShard = winner?["attributes"]["shardId"].ToString();
                            DateTime startDate = DateTime.Parse(matchJson["data"]["attributes"]["createdAt"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);
                            //JArray telemetryData = await GetTelemetryDataFromMatch(matchJson);
                            TimeSpan timeSurvived = TimeSpan.FromSeconds((int)me?["attributes"]["stats"]["timeSurvived"]);
                            string deathReason = string.Empty;
                            switch (me?["attributes"]["stats"]["deathType"].ToString())
                            {
                                default:
                                    deathReason = "Player";
                                    break;
                                case "suicide":
                                    deathReason = "Suicide";
                                    break;
                                case "alive":
                                    break;
                            }

                            EmbedBuilder builder = new EmbedBuilder
                            {
                                Color = Color.Orange,
                                Fields = new List<EmbedFieldBuilder>
                                {
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Started",
                                        Value = startDate
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Duration",
                                        Value = $"{t.Minutes:D2}m {t.Seconds:D2}s"
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Player Count",
                                        Value = matchJson["data"]["relationships"]["rosters"]["data"].ToObject<JArray>().Count
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Placement",
                                        Value = $"#{me?["attributes"]["stats"]["winPlace"]}"
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Death Reason",
                                        Value = deathReason
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Kills",
                                        Value = me?["attributes"]["stats"]["kills"]
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Damage Dealt",
                                        Value = Math.Round((decimal)me?["attributes"]["stats"]["damageDealt"], 2)
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Headshot %",
                                        Value = (float)me?["attributes"]["stats"]["headshotKills"] / (float)me?["attributes"]["stats"]["kills"] * 100
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Time Survived",
                                        Value = $"{timeSurvived.Minutes:D2}m {timeSurvived.Seconds:D2}s"
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Distance Travelled",
                                        Value = Math.Round((float)me?["attributes"]["stats"]["rideDistance"] + (float)me?["attributes"]["stats"]["walkDistance"], 2)
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "DBNOs",
                                        Value = me?["attributes"]["stats"]["DBNOs"]
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Heals Used",
                                        Value = me?["attributes"]["stats"]["heals"]
                                    },
                                    new EmbedFieldBuilder
                                    {
                                        IsInline = true,
                                        Name = "Boosts Used",
                                        Value = me?["attributes"]["stats"]["boosts"]
                                    }
                                },
                                Footer = new EmbedFooterBuilder
                                {
                                    IconUrl = "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400",
                                    Text = $"Match Data for {me?["attributes"]["stats"]["name"]} for match {match["id"]}"
                                },
                                Title = matchJson["data"]["attributes"]["gameMode"].ToString().Replace("-", " ").ToUpper()
                            };
                            
                            if (deathReason == string.Empty) builder.Fields.Remove(builder.Fields.FirstOrDefault(x => x.Name == "Death Reason"));

                            pageEmbeds.Add(builder.Build());
                        } else
                        {
                            await context.Channel.SendMessageAsync($"Type of match is {match["type"]}??");
                        }
                    }

                    await context.SendPaginatedMessageAsync(new PaginatedMessage
                    {
                        Embeds = pageEmbeds,
                        Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                        TimeoutBehaviour = TimeoutBehaviour.Default
                    });
                }
            }
        }

        public async Task GetMatch(EduardoContext context, string matchId, string platformRegionString)
        {
            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out PUBGPlatformRegion platformRegion))
            {
                if (!string.IsNullOrEmpty(matchId))
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject matchJson = await GetMatchFromApi(matchId, platform, region);
                }
            }
        }

        public async Task GetTelemetry(EduardoContext context, string username, string platformRegionString)
        {
            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out PUBGPlatformRegion platformRegion))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject lookupJson = await GetPlayerFromApi(username, platform, region);
                    string matchId = lookupJson["data"][0]["relationships"]["matches"]["data"][0]["id"].ToString();
                    JObject matchJson = await GetMatchFromApi(matchId, platform, region);
                    JArray telemetry = await GetTelemetryDataFromMatch(matchJson);
                    foreach (JToken jToken in telemetry)
                    {
                        JObject obj = (JObject) jToken;
                        string logEvent = obj["_T"].ToString();

                        if (Enum.TryParse(logEvent, out TelemetryEvents evnt))
                        {
                            switch (evnt)
                            {
                                case TelemetryEvents.LogPlayerKill:
                                    switch (obj["damageTypeCategory"].ToString())
                                    {
                                        case "Damage_BlueZone":
                                            Console.WriteLine($"BLUEZONE killed {obj["victim"]["name"]}");
                                            break;
                                        case "Damage_Explosion_RedZone":
                                            Console.WriteLine($"REDZONE killed {obj["victim"]["name"]}");
                                            break;
                                        default:
                                            Console.WriteLine($"{obj["killer"]["name"]} killed {obj["victim"]["name"]}");
                                            break;
                                    }
                                    break;
                                case TelemetryEvents.LogCarePackageSpawn:
                                    List<string> items = new List<string>();
                                    JArray itemJson = JArray.Parse(obj["itemPackage"]["items"].ToString());
                                    foreach (JToken jToken1 in itemJson)
                                    {
                                        JObject item = (JObject) jToken1;
                                        items.Add($"{item["itemId"]} x {item["stackCount"]}");
                                    }
                                    Console.WriteLine($"Care Package Spawned with: {string.Join(", ", items)}");
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public async Task ShowValidOptions(EduardoContext context)
        {
            string[] values = Enum.GetNames(typeof(PUBGPlatformRegion));
            await context.Channel.SendMessageAsync($"Valid options for platform-region are:\n{string.Join(", ", values)}");
        }

        private async Task<JObject> GetPlayerFromApi(string username, string platform, string region)
        {
            try
            {
                string json = await NetworkHelper.GetString(Constants.PUBG_PLAYER_LOOKUP(platform, region, username), new Dictionary<string, string>
                {
                    { "Authorization", $"bearer {credentials.PUBGApiKey}" },
                    { "Accept", "application/vnd.api+json" }
                });

                return JObject.Parse(json);
            } catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG Player from API.\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetPlayerFromApiWithId(string id, string platform, string region)
        {
            try
            {
                string json = await NetworkHelper.GetString(Constants.PUBG_PLAYER_LOOKUP_WITH_ID(platform, region, id), new Dictionary<string, string>
                {
                    { "Authorization", $"bearer {credentials.PUBGApiKey}" },
                    { "Accept", "application/vnd.api+json" }
                });

                return JObject.Parse(json);
            } catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG player from API.\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetMatchFromApi(string matchId, string platform, string region)
        {
            try
            {
                string json = await NetworkHelper.GetString(Constants.PUBG_MATCH_LOOKUP(platform, region, matchId), new Dictionary<string, string>
                {
                    { "Authorization", $"bearer {credentials.PUBGApiKey}" },
                    { "Accept", "application/vnd.api+json" }
                });

                return JObject.Parse(json);
            } catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG match from API.\n{e}"));
                return null;
            }
        }

        private static async Task<JArray> GetTelemetryDataFromMatch(JObject matchJson)
        {
            string telemetryUrl = matchJson["included"].ToObject<JArray>().FirstOrDefault(x => x["type"].ToString() == "asset" && x["id"].ToString() == matchJson["data"]["relationships"]["assets"]["data"][0]["id"].ToString())?["attributes"]["URL"].ToString();
            try
            {
                string json = await NetworkHelper.GetString(telemetryUrl);
                return JArray.Parse(json);
            } catch (Exception ex)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", "Error fetching PUBG Telemetry Data for match", ex));
                return null;
            }
        }
    }
}