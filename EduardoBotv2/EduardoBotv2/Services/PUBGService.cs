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
                    Console.WriteLine(playerJson);
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
                    JToken matchesJson = lookupJson["data"][0]["relationships"]["matches"]["data"];

                    List<Embed> pageEmbeds = new List<Embed>();

                    foreach (JToken match in matchesJson)
                    {
                        if (match["type"].ToString() == "match")
                        {
                            JObject matchJson = await GetMatchFromApi(c, match["id"].ToString(), platform, region);
                            TimeSpan t = TimeSpan.FromSeconds((int)matchJson["data"]["attributes"]["duration"]);
                            pageEmbeds.Add(new EmbedBuilder()
                            {
                                Color = Color.Orange,
                                Fields = new List<EmbedFieldBuilder>()
                                {
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Started",
                                        Value = DateTime.Parse(matchJson["data"]["attributes"]["createdAt"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Duration",
                                        Value = string.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "GameMode",
                                        Value = matchJson["data"]["attributes"]["gameMode"].ToString().Replace("-", " ").ToUpper()
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        IsInline = true,
                                        Name = "Player Count",
                                        Value = matchJson["data"]["relationships"]["rosters"]["data"].ToObject<JArray>().Count
                                    }
                                },
                                Title = "Match Info"
                            }.Build());
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
                if (matchId != string.Empty && matchId != null)
                {
                    string name = Enum.GetName(typeof(PUBGPlatformRegion), platformRegion);
                    string platform = name.Split("_")[0];
                    string region = name.Split("_")[1];
                    JObject matchJson = await GetMatchFromApi(c, matchId, platform, region);
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

        private async Task<JObject> GetMatchFromApi(EduardoContext c, string matchId, string platform, string region)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.PUBG_MATCH_LOOKUP(platform, region, matchId));
                request.Headers.Add("Authorization", "bearer " + c.EduardoSettings.PUBGApiKey);
                request.Headers.Add("Accept", "application/vnd.api+json");
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JObject.Parse(responseString));
                return JObject.Parse(responseString);
            } catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching PUBG match from API.\n{e}"));
                return null;
            }
        }
    }
}