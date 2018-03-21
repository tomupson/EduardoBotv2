using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Data.Enums;
using EduardoBotv2.Common.Data.Models;
using EduardoBotv2.Common.Utilities;
using Newtonsoft.Json;

namespace EduardoBotv2.Services
{
    public class FortniteService
    {
        HttpClient httpClient = new HttpClient();

        public async Task GetStats(EduardoContext c, string username, Platform platform)
        {
            FortniteStats stats = await GetStatsFromApi(c.EduardoSettings.TRNApiKey, username, Enum.GetName(typeof(Platform), platform));
            if (stats != null)
            {
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        IconUrl = @"https://d1u5p3l4wpay3k.cloudfront.net/fortnite_gamepedia/6/64/Favicon.ico",
                        Name = $"Info for account {stats.accountId}"
                    },
                    Color = Color.Purple
                };

                await c.Channel.SendMessageAsync("", false, builder.Build());
            } else
            {
                await c.Channel.SendMessageAsync($"Unable to get fortnite stats for {username}");
            }
        }

        private async Task<FortniteStats> GetStatsFromApi(string apiKey, string username, string platform)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("TRN-Api-Key", apiKey);
                var response = await httpClient.GetAsync($"https://api.fortnitetracker.com/v1/profile/{platform}/{username}");
                var result = await response.Content.ReadAsStringAsync();
                File.WriteAllText(@"D:\Thomas\Documents\response.txt", result);

                return JsonConvert.DeserializeObject<FortniteStats>(result);
            }
            catch (IOException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", $"Error fetching fortnite stats.\n{e}"));
                return null;
            }
        }
    }
}
