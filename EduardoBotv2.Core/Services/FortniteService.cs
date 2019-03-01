using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Models.Enums;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace EduardoBotv2.Core.Services
{
    public class FortniteService
    {
        private string refreshToken;
        private string accessToken;
        private string accountId;
        private string code;
        private string expiresAt;
        private bool loggedIn;

        public FortniteService()
        {
            StartTokenChecker();
            loggedIn = false;
        }

        private async void StartTokenChecker()
        {
            await CommonHelper.SetInterval(async () =>
            {
                if (expiresAt != string.Empty)
                {
                    DateTime actualDate = new DateTime();
                    if (DateTime.TryParseExact(expiresAt, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime expireDate))
                    {
                        expireDate -= TimeSpan.FromMilliseconds(15);
                    }

                    if (expireDate < actualDate)
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Constants.FORTNITE_OAUTH_TOKEN)
                        {
                            Content = new FormUrlEncodedContent(new Dictionary<string, string>
                            {
                                { "grant_type", "refresh_token" },
                                { "refresh_token", refreshToken },
                                { "includePerms", "true" }
                            })
                        };
                        request.Headers.Add("Authorization", "basic " + Constants.FORTNITE_CLIENT_TOKEN);
                        HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                        string responseString = await response.Content.ReadAsStringAsync();
                    }
                }
            }, TimeSpan.FromSeconds(1));
        }

        private async Task Login(Credentials settings)
        {
            Console.WriteLine("Logging in...");
            HttpRequestMessage oauthTokenRequest = new HttpRequestMessage(HttpMethod.Post, Constants.FORTNITE_OAUTH_TOKEN)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    //{ "username", settings.FortniteLoginEmail },
                    //{ "password", settings.FortniteLoginPassword },
                    { "includePerms", "true" }
                })
            };
            oauthTokenRequest.Headers.Add("Authorization", "basic " + Constants.FORTNITE_LAUNCHER_TOKEN);
            HttpResponseMessage oauthTokenResponse = await NetworkHelper.MakeRequest(oauthTokenRequest);
            string oauthTokenResponseString = await oauthTokenResponse.Content.ReadAsStringAsync();
            JObject oauthTokenJson = JObject.Parse(oauthTokenResponseString);
            accessToken = oauthTokenJson["access_token"].ToString();
            accountId = oauthTokenJson["account_id"].ToString();

            HttpRequestMessage oauthExchangeRequest = new HttpRequestMessage(HttpMethod.Get, Constants.FORTNITE_OAUTH_EXCHANGE);
            oauthExchangeRequest.Headers.Add("Authorization", "bearer " + accessToken);
            HttpResponseMessage oauthExchangeResponse = await NetworkHelper.MakeRequest(oauthExchangeRequest);
            string oauthExchangeResponseString = await oauthExchangeResponse.Content.ReadAsStringAsync();
            JObject oauthExchangeJson = JObject.Parse(oauthExchangeResponseString);
            code = oauthExchangeJson["code"].ToString();

            HttpRequestMessage finalRequest = new HttpRequestMessage(HttpMethod.Post, Constants.FORTNITE_OAUTH_TOKEN)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "exchange_code" },
                    { "exchange_code", code },
                    { "includePerms", "true" },
                    { "token_type", "egl" }
                })
            };
            finalRequest.Headers.Add("Authorization", "basic " + Constants.FORTNITE_CLIENT_TOKEN);
            HttpResponseMessage finalResponse = await NetworkHelper.MakeRequest(finalRequest);
            string finalResponseString = await finalResponse.Content.ReadAsStringAsync();
            JObject finalJson = JObject.Parse(finalResponseString);
            expiresAt = finalJson["expires_at"].ToString();
            accessToken = finalJson["access_token"].ToString();
            refreshToken = finalJson["refresh_token"].ToString();

            loggedIn = true;
            Console.WriteLine("Logged in!");
        }

        private async Task<JObject> Lookup(string username)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.FORTNITE_PLAYER_LOOKUP(username));
            request.Headers.Add("Authorization", "bearer " + accessToken);
            HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
            string responseString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseString);
        }

        public async Task GetStats(EduardoContext c, string username, Gamemode gamemode)
        {
            if (!loggedIn)
            {
                await Login(c.EduardoCredentials);
            }

            if (username != string.Empty)
            {
                JArray statsJson = await GetStatsFromApi(username);

                List<int> indices;
                string title;
                string second;
                string third;

                switch (gamemode)
                {
                    default:
                        indices = new List<int> { 8, 10, 7, 4, 5, 11, 17, 19 };
                        title = "Solo";
                        second = "Top 10s";
                        third = "Top 25s";
                        break;
                    case Gamemode.Duo:
                        indices = new List<int> { 2, 16, 14, 0, 21, 15, 20, 13 };
                        title = "Duo";
                        second = "Top 5s";
                        third = "Top 12s";
                        break;
                    case Gamemode.Squad:
                        indices = new List<int> { 12, 18, 6, 1, 9, 23, 3, 22 };
                        title = "Squad";
                        second = "Top 3s";
                        third = "Top 6s";
                        break;
                }

                if (statsJson != null)
                {
                    EmbedBuilder builder = new EmbedBuilder
                    {
                        Author = new EmbedAuthorBuilder
                        {
                            IconUrl = "https://i.pinimg.com/originals/72/54/44/725444874a4c0bda8ea17fd6f6332c11.jpg",
                            Name = "Fortnite Battle Royale Stats"
                        },
                        Color = Color.Purple,
                        Fields = new List<EmbedFieldBuilder>
                        {
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Matches Played",
                                Value = statsJson[indices[0]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Minutes Played",
                                Value = statsJson[indices[1]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Last Modified",
                                Value = string.Format("{0:dddd d/M/yy} at {0:HH:mm}", new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((double)statsJson[indices[2]]["value"]))
                                //Value = string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:HH:mm}", new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((double)statsJson[indices[2]]["value"]), CommonHelper.GetDaySuffix(DateTime.Now.Day))
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Score",
                                Value = statsJson[indices[3]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Kills",
                                Value = statsJson[indices[4]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Kills per Match",
                                Value = (float)statsJson[indices[4]]["value"] / (float)statsJson[indices[0]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Wins",
                                Value = statsJson[indices[5]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = second,
                                Value = statsJson[indices[6]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = third,
                                Value = statsJson[indices[7]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Win Percentage",
                                Value = (float)statsJson[indices[5]]["value"] / (float)statsJson[indices[0]]["value"] * 100
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Avg. Minutes per Match",
                                Value = (float)statsJson[indices[1]]["value"] / (float)statsJson[indices[0]]["value"]
                            },
                            new EmbedFieldBuilder
                            {
                                IsInline = true,
                                Name = "Kills per Minute",
                                Value = (float)statsJson[indices[4]]["value"] / (float)statsJson[indices[1]]["value"]
                            }
                        },
                        Footer = new EmbedFooterBuilder
                        {
                            Text = $"Fortnite Stats for {username} as of {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                        },
                        Title = title
                    };

                    await c.Channel.SendMessageAsync("", false, builder.Build());
                }
                else
                {
                    await c.Channel.SendMessageAsync($"Unable to get Fortnite BR Stats for \"{username}\"");
                }
            }
        }

        public async Task GetWeeklyStoreItems(EduardoContext c)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://stormshield.one/pvp/sales");
            HtmlNodeCollection saleItems = document.DocumentNode.SelectNodes("//div[contains(@class, 'sale__items')]");

            List<string> itemNames = saleItems[0].SelectNodes(".//p").Select(name => name.InnerText).ToList();

            List<string> itemUrls = saleItems[0].SelectNodes(".//img").Select(image => image.GetAttributeValue("src", null)).ToList();

            List<Embed> pageEmbeds = itemNames.Select((t, i) => new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = "https://i.pinimg.com/originals/72/54/44/725444874a4c0bda8ea17fd6f6332c11.jpg",
                    Name = "Fortnite Battle Royale Weekly Store"
                },
                Color = Color.Purple,
                Title = t.Boldify(),
                ImageUrl = itemUrls[i],
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Fortnite Weekly Store as of {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                }
            }.Build()).ToList();

            await c.SendPaginatedMessageAsync(new PaginatedMessage
            {
                Embeds = pageEmbeds,
                Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                TimeoutBehaviour = TimeoutBehaviour.Default
            });
        }

        public async Task GetDailyStoreItems(EduardoContext c)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://stormshield.one/pvp/sales");
            HtmlNodeCollection saleItems = document.DocumentNode.SelectNodes("//div[contains(@class, 'sale__items')]");

            List<string> itemNames = saleItems[1].SelectNodes(".//p").Select(name => name.InnerText).ToList();
            List<string> itemUrls = saleItems[1].SelectNodes(".//img").Select(image => image.GetAttributeValue("src", null)).ToList();

            List<Embed> pageEmbeds = itemNames.Select((t, i) => new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = "https://i.pinimg.com/originals/72/54/44/725444874a4c0bda8ea17fd6f6332c11.jpg",
                    Name = "Fortnite Battle Royale Daily Store"
                },
                Color = Color.Purple,
                Title = t.Boldify(),
                ImageUrl = itemUrls[i],
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Fortnite Daily Store as of {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                }
            }.Build()).ToList();

            await c.SendPaginatedMessageAsync(new PaginatedMessage
            {
                Embeds = pageEmbeds,
                Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                TimeoutBehaviour = TimeoutBehaviour.Default
            });
        }

        public async Task GetStoreItems(EduardoContext c)
        {
            if (!loggedIn)
            {
                await Login(c.EduardoCredentials);
            }

            JObject storeItemsJson = await GetStoreItemsFromApi();

            if (storeItemsJson != null)
            {

            }
        }

        public async Task GetNews(EduardoContext c)
        {
            if (!loggedIn)
            {
                await Login(c.EduardoCredentials);
            }

            JObject newsJson = await GetNewsFromApi();

            if (newsJson != null)
            {
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                JToken brNews = newsJson["battleroyalenews"]["news"]["messages"];

                List<Embed> pageEmbeds = brNews.Select(x => new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        IconUrl = "https://i.pinimg.com/originals/72/54/44/725444874a4c0bda8ea17fd6f6332c11.jpg",
                        Name = "Fortnite Battle Royale News"
                    },
                    Color = Color.Purple,
                    Title = x["title"].ToString(),
                    Description = x["body"].ToString(),
                    ImageUrl = x["image"].ToString(),
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"Fortnite News as of {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                    }
                }.Build()).ToList();

                await c.SendPaginatedMessageAsync(new PaginatedMessage
                {
                    Embeds = pageEmbeds,
                    Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                    TimeoutBehaviour = TimeoutBehaviour.Default
                });
            }
            else
            {
                await c.Channel.SendMessageAsync("Failed to load Fortnite News.");
            }
        }

        public async Task GetServerStatus(EduardoContext c)
        {
            if (!loggedIn)
            {
                await Login(c.EduardoCredentials);

                JArray statusJson = await GetServerStatusFromApi();
                Console.WriteLine(statusJson);

                if (statusJson != null)
                {
                    string status = statusJson[0]["status"].ToString();
                    string maintenanceUrl = statusJson[0]["maintenanceUri"].ToString();
                    await c.Channel.SendMessageAsync($"Fortnite Servers are {statusJson[0]["status"].Boldify()}");
                    if (status != "UP" && maintenanceUrl != string.Empty)
                    {
                        await c.Channel.SendMessageAsync($"Maintenance URL: {maintenanceUrl}");
                    }
                }
            }
        }

        public async Task ReportBug(EduardoContext c)
        {
            EmbedBuilder builder = new EmbedBuilder
            {
                Color = Color.Purple,
                Description = "How do I submit a bug report for Fortnite?",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = "Report the bug in-game",
                        Value = "• Open the game menu\n\n• Select *Feedback*\n\n• Select *Bug*\n\n• Fill in the *Subject* and *Body* fields with your feedback\n\n• Select *Send*"
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = "Report the bug online",
                        Value = "Additionally you can post in the \"Bug Reporting\" section of the forums."
                    }
                },
                Title = "Epic Games Support",
                Url = "http://fortnitehelp.epicgames.com/customer/en/portal/articles/2841545-how-do-i-submit-a-bug-report-for-fortnite-"
            };

            await c.Channel.SendMessageAsync("", false, builder.Build());
        }

        private async Task<JArray> GetStatsFromApi(string username)
        {
            try
            {
                JObject lookupDataJson = await Lookup(username);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.FORTNITE_STATS_BR(lookupDataJson["id"].ToString()));
                request.Headers.Add("Authorization", "bearer " + accessToken);
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JArray.Parse(responseString);
            }
            catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo", $"Error fetching Fortnite Stats from API.\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetStoreItemsFromApi()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.FORTNITE_STORE);
                request.Headers.Add("Authorization", "bearer " + accessToken);
                request.Headers.Add("X-EpicGames-Language", "en");
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching Fortnite BR Store Items from API\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetNewsFromApi()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.FORTNITE_NEWS);
                request.Headers.Add("Authorization", "bearer " + accessToken);
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo", $"Error fetching Fortnite News from API.\n{e}"));
                return null;
            }
        }

        private async Task<JArray> GetServerStatusFromApi()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.FORTNITE_SERVER_STATUS);
                request.Headers.Add("Authorization", "bearer " + accessToken);
                HttpResponseMessage response = await NetworkHelper.MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JArray.Parse(responseString);
            }
            catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo", $"Error fetching Fortnite Server Status from API.\n{e}"));
                return null;
            }
        }
    }
}