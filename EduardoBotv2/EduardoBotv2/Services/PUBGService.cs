using Discord;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Data.Enums;
using EduardoBotv2.Common.Data.Models;
using EduardoBotv2.Common.Extensions;
using EduardoBotv2.Common.Utilities;
using EduardoBotv2.Common.Utilities.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EduardoBotv2.Services
{
    public class PUBGService
    {
        public async Task GetPlayer(EduardoContext c, string username, string platformRegionString)
        {
            PUBGPlatformRegion platformRegion;

            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out platformRegion))
            {
                if (username != string.Empty && username != null)
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject playerJson = await GetPlayerFromApi(c, username, platform, region);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Color = Color.Orange,
                        Description = "You can view your recent matches by using the `$pubgmatches` command",
                        Fields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder()
                            {
                                IsInline = true,
                                Name = "Account ID",
                                Value = playerJson["data"][0]["attributes"]["id"]
                            },
                            new EmbedFieldBuilder()
                            {
                                IsInline = true,
                                Name = "Last Updated",
                                Value = playerJson["data"][0]["attributes"]["updatedAt"]
                            }
                        },
                        Footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400",
                            Text = $"Player Profile for {playerJson["data"][0]["attributes"]["name"]}"
                        },
                        Title = playerJson["data"][0]["attributes"]["name"].ToString()
                    };

                    await c.Channel.SendMessageAsync("", false, builder.Build());
                }
            } else
            {
                await c.Channel.SendMessageAsync("Invalid platform-region. You can view valid options with `$pubgvalids`");
            }
        }

        public async Task GetMatches(EduardoContext c, string username, string platformRegionString)
        {
            PUBGPlatformRegion platformRegion;

            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out platformRegion))
            {
                if (username != string.Empty && username != null)
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject lookupJson = await GetPlayerFromApi(c, username, platform, region);
                    File.WriteAllText(@"D:\Thomas\Documents\ExamplePlayer.json", lookupJson.ToString());
                    JToken matchesJson = lookupJson["data"][0]["relationships"]["matches"]["data"];

                    List<Embed> pageEmbeds = new List<Embed>();

                    foreach (JToken match in matchesJson)
                    {
                        if (match["type"].ToString() == "match")
                        {
                            JObject matchJson = await GetMatchFromApi(c, match["id"].ToString(), platform, region);
                            TimeSpan t = TimeSpan.FromSeconds((int)matchJson["data"]["attributes"]["duration"]);
                            List<JToken> participants = matchJson["included"].ToObject<JArray>().Where(x => x["type"].ToString() == "participant").ToList();
                            JToken me = participants.Where(x => x["attributes"]["stats"]["playerId"].ToString() == lookupJson["data"][0]["id"].ToString()).FirstOrDefault();
                            JToken winner = participants.Where(x => (int)x["attributes"]["stats"]["winPlace"] == 1).FirstOrDefault();
                            string winnerShard = winner["attributes"]["shardId"].ToString();
                            DateTime startDate = DateTime.Parse(matchJson["data"]["attributes"]["createdAt"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);
                            //JArray telemetryData = await GetTelemetryDataFromMatch(matchJson);
                            TimeSpan timeSurvived = TimeSpan.FromSeconds((int)me["attributes"]["stats"]["timeSurvived"]);
                            string deathReason = string.Empty;
                            switch (me["attributes"]["stats"]["deathType"].ToString())
                            {
                                default:
                                case "byplayer":
                                    deathReason = "Player";
                                    break;
                                case "suicide":
                                    deathReason = "Suicide";
                                    break;
                                case "alive":
                                    break;
                            }

                            EmbedBuilder builder = new EmbedBuilder()
                            {
                                Color = Color.Orange,
                                Fields = new List<EmbedFieldBuilder>()
                                {
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Started",
                                        Value = startDate
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Duration",
                                        Value = string.Format("{0:D2}m {1:D2}s", t.Minutes, t.Seconds)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Player Count",
                                        Value = matchJson["data"]["relationships"]["rosters"]["data"].ToObject<JArray>().Count
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Placement",
                                        Value = $"#{me["attributes"]["stats"]["winPlace"].ToString()}"
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Death Reason",
                                        Value = deathReason
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Kills",
                                        Value = me["attributes"]["stats"]["kills"]
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Damage Dealt",
                                        Value = Math.Round((decimal)me["attributes"]["stats"]["damageDealt"], 2)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Headshot %",
                                        Value = ((float)me["attributes"]["stats"]["headshotKills"] / (float)me["attributes"]["stats"]["kills"]) * 100
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Time Survived",
                                        Value = string.Format("{0:D2}m {1:D2}s", timeSurvived.Minutes, timeSurvived.Seconds)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Distance Travelled",
                                        Value = Math.Round((float)me["attributes"]["stats"]["rideDistance"] + (float)me["attributes"]["stats"]["walkDistance"], 2)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "DBNOs",
                                        Value = me["attributes"]["stats"]["DBNOs"]
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Heals Used",
                                        Value = me["attributes"]["stats"]["heals"]
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Boosts Used",
                                        Value = me["attributes"]["stats"]["boosts"]
                                    }
                                },
                                Footer = new EmbedFooterBuilder()
                                {
                                    IconUrl = "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400",
                                    Text = $"Match Data for {me["attributes"]["stats"]["name"]} for match {match["id"].ToString()}"
                                },
                                Title = matchJson["data"]["attributes"]["gameMode"].ToString().Replace("-", " ").ToUpper()
                            };
                            
                            if (deathReason == string.Empty)
                                builder.Fields.Remove(builder.Fields.Where(x => x.Name == "Death Reason").FirstOrDefault());

                            pageEmbeds.Add(builder.Build());
                        } else
                        {
                            await c.Channel.SendMessageAsync($"Type of match is {match["type"].ToString()}??");
                        }
                    }

                    await c.SendPaginatedMessageAsync(new PaginatedMessage()
                    {
                        Embeds = pageEmbeds,
                        Timeout = Config.PAGINATION_TIMEOUT_TIME,
                        TimeoutBehaviour = TimeoutBehaviour.Default
                    });
                }
            }
        }

        public async Task GetMatch(EduardoContext c, string matchId, string platformRegionString)
        {
            PUBGPlatformRegion platformRegion;

            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out platformRegion))
            {
                if (!string.IsNullOrEmpty(matchId))
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject matchJson = await GetMatchFromApi(c, matchId, platform, region);
                }
            }
        }

        public async Task GetTelemetry(EduardoContext c, string username, string platformRegionString)
        {
            PUBGPlatformRegion platformRegion;

            if (Enum.TryParse(platformRegionString.Replace("-", "_"), out platformRegion))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject lookupJson = await GetPlayerFromApi(c, username, platform, region);
                    string matchId = lookupJson["data"][0]["relationships"]["matches"]["data"][0]["id"].ToString();
                    JObject matchJson = await GetMatchFromApi(c, matchId, platform, region);
                    JArray telemetry = await GetTelemetryDataFromMatch(matchJson);
                    foreach (JObject obj in telemetry)
                    {
                        string logEvent = obj["_T"].ToString();
                        TelemetryEvents evnt;

                        if (Enum.TryParse(logEvent, out evnt))
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
                                        case "Damage_Gun":
                                            Console.WriteLine($"{obj["killer"]["name"]} killed {obj["victim"]["name"]}");
                                            break;
                                    }
                                    break;
                                case TelemetryEvents.LogCarePackageSpawn:
                                    List<string> items = new List<string>();
                                    JArray itemJson = JArray.Parse(obj["itemPackage"]["items"].ToString());
                                    foreach (JObject item in itemJson)
                                    {
                                        items.Add($"{item["itemId"]} x {item["stackCount"]}");
                                    }
                                    Console.WriteLine("Care Package Spawned with: " + string.Join(", ", items));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public async Task ShowValidOptions(EduardoContext c)
        {
            string[] values = Enum.GetNames(typeof(PUBGPlatformRegion));
            await c.Channel.SendMessageAsync($"Valid options for platform-region are:\n{string.Join(", ", values)}");
        }

        private async Task<JObject> GetPlayerFromApi(EduardoContext c, string username, string platform, string region)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.PUBG_PLAYER_LOOKUP(platform, region, username));
                request.Headers.Add("Authorization", "bearer " + c.EduardoSettings.PUBGApiKey);
                request.Headers.Add("Accept", "application/vnd.api+json");
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            } catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG Player from API.\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetPlayerFromApiWithId(EduardoContext c, string id, string platform, string region)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.PUBG_PLAYER_LOOKUP_WITH_ID(platform, region, id));
                request.Headers.Add("Authorization", "bearer " + c.EduardoSettings.PUBGApiKey);
                request.Headers.Add("Accept", "application/vnd.api+json");
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            } catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG player from API.\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetMatchFromApi(EduardoContext c, string matchId, string platform, string region)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.PUBG_MATCH_LOOKUP(platform, region, matchId));
                request.Headers.Add("Authorization", "bearer " + c.EduardoSettings.PUBGApiKey);
                request.Headers.Add("Accept", "application/vnd.api+json");
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            } catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG match from API.\n{e}"));
                return null;
            }
        }

        private async Task<JArray> GetTelemetryDataFromMatch(JObject matchJson)
        {
            string telemetryURL = matchJson["included"].ToObject<JArray>().Where(x => x["type"].ToString() == "asset" && x["id"].ToString() == matchJson["data"]["relationships"]["assets"]["data"][0]["id"].ToString()).FirstOrDefault()["attributes"]["URL"].ToString();
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, telemetryURL);
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JArray.Parse(responseString);
            } catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG Telemetry Data for match.\n{e}"));
                return null;
            }
        }
    }
}