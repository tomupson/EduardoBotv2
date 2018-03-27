using Discord;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Utilities;
using EduardoBotv2.Common.Data.Enums;
using EduardoBotv2.Common.Extensions;
using EduardoBotv2.Common.Data.Models;
using EduardoBotv2.Common.Utilities.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduardoBotv2.Services
{
    public class FortniteService
    {
        HttpClient httpClient = new HttpClient();

        private string refreshToken = string.Empty;
        private string accessToken = string.Empty;
        private string accountId = string.Empty;
        private string code = string.Empty;
        private string expiresAt = string.Empty;

        private bool loggedIn = false;

        public FortniteService()
        {
            StartTokenChecker();
            loggedIn = false;
        }

        private async Task<HttpResponseMessage> MakeRequest(HttpRequestMessage request)
        {
            try
            {
                return await httpClient.SendAsync(request);
            }
            catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", $"Error fetching fortnite stats.\n{e}"));
                return null;
            }
        }

        private async void StartTokenChecker()
        {
            await CommonHelper.SetInterval(async () =>
            {
                DateTime actualDate = new DateTime();
                TimeSpan difference = DateTime.Now - DateTime.Parse(this.expiresAt);
                DateTime expireDate = new DateTime(difference.Subtract(TimeSpan.FromMilliseconds(15)).Multiply(60000).Milliseconds);

                if (this.expiresAt != string.Empty && expireDate < actualDate)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Config.FORTNITE_OAUTH_TOKEN);
                    request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", this.refreshToken },
                    { "includePerms", "true" }
                });
                    request.Headers.Add("Authorization", "basic " + Config.FORTNITE_CLIENT_TOKEN);
                    HttpResponseMessage response = await MakeRequest(request);
                    string responseString = await response.Content.ReadAsStringAsync();
                }
            }, TimeSpan.FromSeconds(1));
        }

        private async Task Login(Settings settings)
        {
            Console.WriteLine("Logging in...");
            HttpRequestMessage oauthTokenRequest = new HttpRequestMessage(HttpMethod.Post, Config.FORTNITE_OAUTH_TOKEN);
            oauthTokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", settings.FortniteLoginEmail },
                { "password", settings.FortniteLoginPassword },
                { "includePerms", "true" }
            });
            oauthTokenRequest.Headers.Add("Authorization", "basic " + Config.FORTNITE_LAUNCHER_TOKEN);
            HttpResponseMessage oauthTokenResponse = await MakeRequest(oauthTokenRequest);
            string oauthTokenResponseString = await oauthTokenResponse.Content.ReadAsStringAsync();
            JObject oauthTokenJson = JObject.Parse(oauthTokenResponseString);
            this.accessToken = oauthTokenJson["access_token"].ToString();
            this.accountId = oauthTokenJson["account_id"].ToString();

            HttpRequestMessage oauthExchangeRequest = new HttpRequestMessage(HttpMethod.Get, Config.FORTNITE_OAUTH_EXCHANGE);
            oauthExchangeRequest.Headers.Add("Authorization", "bearer " + this.accessToken);
            HttpResponseMessage oauthExchangeResponse = await MakeRequest(oauthExchangeRequest);
            string oauthExchangeResponseString = await oauthExchangeResponse.Content.ReadAsStringAsync();
            JObject oauthExchangeJson = JObject.Parse(oauthExchangeResponseString);
            this.code = oauthExchangeJson["code"].ToString();

            HttpRequestMessage finalRequest = new HttpRequestMessage(HttpMethod.Post, Config.FORTNITE_OAUTH_TOKEN);
            finalRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "exchange_code" },
                { "exchange_code", this.code },
                { "includePerms", "true" },
                { "token_type", "egl" }
            });
            finalRequest.Headers.Add("Authorization", "basic " + Config.FORTNITE_CLIENT_TOKEN);
            HttpResponseMessage finalResponse = await MakeRequest(finalRequest);
            string finalResponseString = await finalResponse.Content.ReadAsStringAsync();
            JObject finalJson = JObject.Parse(finalResponseString);
            this.expiresAt = finalJson["expires_at"].ToString();
            this.accessToken = finalJson["access_token"].ToString();
            this.refreshToken = finalJson["refresh_token"].ToString();

            loggedIn = true;
            Console.WriteLine("Logged in!");
        }

        private async Task<JObject> Lookup(string username)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.FORTNITE_PLAYER_LOOKUP(username));
            request.Headers.Add("Authorization", "bearer " + this.accessToken);
            HttpResponseMessage response = await MakeRequest(request);
            string responseString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseString);
        }

        public async Task GetStats(EduardoContext c, string username)
        {
            if (!loggedIn)
            {
                await Login(c.EduardoSettings);
            }

            if (username != string.Empty)
            {
                JObject lookupJson = await Lookup(username);
                JArray statsJson = await GetStatsFromApi(username);
                Console.WriteLine(statsJson);

                if (statsJson != null)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Author = new EmbedAuthorBuilder()
                        {
                            IconUrl = @"https://d1u5p3l4wpay3k.cloudfront.net/fortnite_gamepedia/6/64/Favicon.ico",
                            Name = $"Info for account {lookupJson["account_id"]}"
                        },
                        Color = Color.Purple
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
            var web = new HtmlWeb();
            var document = web.Load("https://stormshield.one/pvp/sales");
            var saleItems = document.DocumentNode.SelectNodes("//div[contains(@class, 'sale__items')]");

            List<string> itemNames = new List<string>();
            List<string> itemUrls = new List<string>();

            foreach (HtmlNode name in saleItems[0].SelectNodes(".//p"))
            {
                itemNames.Add(name.InnerText);
            }

            foreach (HtmlNode image in saleItems[0].SelectNodes(".//img"))
            {
                itemUrls.Add(image.GetAttributeValue("src", null));
            }

            List<Embed> pageEmbeds = new List<Embed>();
            for (int i = 0; i < itemNames.Count; i++)
            {
                pageEmbeds.Add(new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        IconUrl = "https://i.pinimg.com/originals/72/54/44/725444874a4c0bda8ea17fd6f6332c11.jpg",
                        Name = "Fortnite Battle Royale Weekly Store"
                    },
                    Color = Color.Purple,
                    Title = itemNames[i].Boldify(),
                    ImageUrl = itemUrls[i],
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = $"Fortnite Weekly Store as of {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                    }
                }.Build());
            }

            await c.SendPaginatedMessageAsync(new PaginatedMessage()
            {
                Embeds = pageEmbeds,
                Timeout = Config.PAGINATION_TIMEOUT_TIME,
                TimeoutBehaviour = TimeoutBehaviour.Default
            });
        }

        public async Task GetDailyStoreItems(EduardoContext c)
        {
            var web = new HtmlWeb();
            var document = web.Load("https://stormshield.one/pvp/sales");
            var saleItems = document.DocumentNode.SelectNodes("//div[contains(@class, 'sale__items')]");

            List<string> itemNames = new List<string>();
            List<string> itemUrls = new List<string>();

            foreach (HtmlNode name in saleItems[1].SelectNodes(".//p"))
            {
                itemNames.Add(name.InnerText);
            }

            foreach (HtmlNode image in saleItems[1].SelectNodes(".//img"))
            {
                itemUrls.Add(image.GetAttributeValue("src", null));
            }

            List<Embed> pageEmbeds = new List<Embed>();
            for (int i = 0; i < itemNames.Count; i++)
            {
                pageEmbeds.Add(new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        IconUrl = "https://i.pinimg.com/originals/72/54/44/725444874a4c0bda8ea17fd6f6332c11.jpg",
                        Name = "Fortnite Battle Royale Daily Store"
                    },
                    Color = Color.Purple,
                    Title = itemNames[i].Boldify(),
                    ImageUrl = itemUrls[i],
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = $"Fortnite Daily Store as of {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                    }
                }.Build());
            }

            await c.SendPaginatedMessageAsync(new PaginatedMessage()
            {
                Embeds = pageEmbeds,
                Timeout = Config.PAGINATION_TIMEOUT_TIME,
                TimeoutBehaviour = TimeoutBehaviour.Default
            });
        }

        //public async Task GetStoreItems(EduardoContext c)
        //{
        //    if (!loggedIn)
        //    {
        //        await Login(c.EduardoSettings);
        //    }

        //    JObject storeItemsJson = await GetStoreItemsFromApi();
            
        //    if (storeItemsJson != null)
        //    {

        //    }
        //}

        public async Task GetNews(EduardoContext c)
        {
            if (!loggedIn)
            {
                await Login(c.EduardoSettings);
            }

            JObject newsJson = await GetNewsFromApi();

            if (newsJson != null)
            {
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                var brNews = newsJson["battleroyalenews"]["news"]["messages"];

                List<Embed> pageEmbeds = new List<Embed>();

                foreach (JObject newsItem in brNews)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Author = new EmbedAuthorBuilder()
                        {
                            IconUrl = "https://i.pinimg.com/originals/72/54/44/725444874a4c0bda8ea17fd6f6332c11.jpg",
                            Name = "Fortnite Battle Royale News"
                        },
                        Color = Color.Purple,
                        Title = newsItem["title"].ToString(),
                        Description = newsItem["body"].ToString(),
                        ThumbnailUrl = newsItem["image"].ToString(),
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = $"Fortnite News as of {string.Format("{0:dddd MMM d}{1} {0:yyyy} at {0:h:m tt}", DateTime.Now, CommonHelper.GetDaySuffix(DateTime.Now.Day))}"
                        }
                    };

                    pageEmbeds.Add(builder.Build());
                }

                await c.SendPaginatedMessageAsync(new PaginatedMessage()
                {
                    Embeds = pageEmbeds,
                    Timeout = Config.PAGINATION_TIMEOUT_TIME,
                    TimeoutBehaviour = TimeoutBehaviour.Default
                });
            } else
            {
                await c.Channel.SendMessageAsync("Failed to load Fortnite News.");
            }
        }

        public async Task GetServerStatus(EduardoContext c)
        {
            if (!loggedIn)
            {
                await Login(c.EduardoSettings);

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
            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = Color.Purple,
                Description = "How do I submit a bug report for Fortnite?",
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        IsInline = false,
                        Name = "Report the bug in-game",
                        Value = "• Open the game menu\n\n• Select *Feedback*\n\n• Select *Bug*\n\n• Fill in the *Subject* and *Body* fields with your feedback\n\n• Select *Send*"
                    },
                    new EmbedFieldBuilder()
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
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.FORTNITE_STATS_BR(lookupDataJson["id"].ToString()));
                request.Headers.Add("Authorization", "bearer " + this.accessToken);
                HttpResponseMessage response = await MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JArray.Parse(responseString);
            }
            catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", $"Error fetching Fortnite Stats from API.\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetStoreItemsFromApi()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.FORTNITE_STORE);
                request.Headers.Add("Authorization", "bearer " + this.accessToken);
                request.Headers.Add("X-EpicGames-Language", "en");
                HttpResponseMessage response = await MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error fetching Fortnite BR Store Items from API\n{e}"));
                return null;
            }
        }

        private async Task<JObject> GetNewsFromApi()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.FORTNITE_NEWS);
                request.Headers.Add("Authorization", "bearer " + this.accessToken);
                HttpResponseMessage response = await MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", $"Error fetching Fortnite News from API.\n{e}"));
                return null;
            }
        }

        private async Task<JArray> GetServerStatusFromApi()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Config.FORTNITE_SERVER_STATUS);
                request.Headers.Add("Authorization", "bearer " + this.accessToken);
                HttpResponseMessage response = await MakeRequest(request);
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
                return JArray.Parse(responseString);
            }
            catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", $"Error fetching Fortnite Server Status from API.\n{e}"));
                return null;
            }
        }
    }
}