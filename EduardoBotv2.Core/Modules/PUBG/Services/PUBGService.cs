using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.PUBG.Models;
using EduardoBotv2.Core.Services;
using Newtonsoft.Json;
using Pubg.Net;
using Pubg.Net.Models.Participants;

namespace EduardoBotv2.Core.Modules.PUBG.Services
{
    public class PubgService : IEduardoService
    {
        private readonly PubgData _pubgData;

        public PubgService()
        {
            _pubgData = JsonConvert.DeserializeObject<PubgData>(File.ReadAllText("data/pubg.json"));
        }

        public async Task GetPlayer(EduardoContext context, string username, PubgPlatform platform)
        {
            PubgPlayer player = await GetPlayerFromApiAsync(username, platform);

            if (player == null)
            {
                await context.Channel.SendMessageAsync($"No player found with username \"{username}\"");
                return;
            }

            await context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithTitle(player.Name)
                .WithColor(Color.Orange)
                .WithDescription($"You can view recent matches for this player using `{Constants.CMD_PREFIX}pubg matches {player.Name} {platform}`")
                .AddField("Account ID", player.Id, true)
                .WithFooter($"Player Profile for {player.Name}",
                    "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400")
                .Build());
        }

        public async Task GetMatches(EduardoContext context, string username, PubgPlatform platform)
        {
            PubgPlayer player = await GetPlayerFromApiAsync(username, platform);

            if (player == null)
            {
                await context.Channel.SendMessageAsync($"No player found with username \"{username}\"");
                return;
            }

            List<Embed> embeds = new List<Embed>();
            foreach (string matchId in player.MatchIds.Take(_pubgData.MaxMatches))
            {
                PubgMatch match = await GetMatchFromApiAsync(matchId, platform);

                TimeSpan matchDuration = TimeSpan.FromSeconds(match.Duration);
                PubgRoster roster = match.Rosters.FirstOrDefault(r => r.Participants.Any(p => p.Stats.PlayerId == player.Id));
                PubgParticipant participant = match.Rosters.SelectMany(r => r.Participants).FirstOrDefault(p => p.Stats.PlayerId == player.Id);
                TimeSpan timeSurvived = TimeSpan.FromSeconds(participant?.Stats.TimeSurvived ?? 0);

                embeds.Add(new EmbedBuilder()
                    .WithTitle(match.GameMode.ToString())
                    .WithColor(Color.Orange)
                    .AddField("Started", DateTime.Parse(match.CreatedAt), true)
                    .AddField("Duration", $"{matchDuration.Minutes:D2}m {matchDuration.Seconds:D2}s", true)
                    .AddField("Player Count", match.Rosters.Count(), true)
                    .AddField("Placement", roster != null
                        ? $"#{roster.Stats.Rank}"
                        : "Unknown", true)
                    .AddConditionalField("Death Reason", participant?.Stats.DeathType.ToString() ?? "Unknown",
                        participant != null && participant.Stats.DeathType == PubgParticipantDeathType.Alive, true)
                    .AddField("Kills", participant?.Stats.Kills.ToString() ?? "Unknown", true)
                    .AddField("Damage Dealt", participant != null
                        ? Math.Round(participant.Stats.DamageDealt, 2).ToString(CultureInfo.InvariantCulture)
                        : "Unknown", true)
                    .AddField("Headshot %", participant != null
                        ? participant.Stats.Kills > 0
                            ? (participant.Stats.HeadshotKills / participant.Stats.Kills * 100).ToString()
                            : "0%"
                        : "Unknown", true)
                    .AddField("Time Survived", participant != null
                        ? $"{timeSurvived.Minutes:D2}m {timeSurvived.Seconds:D2}s"
                        : "Unknown", true)
                    .AddField("Distance Travelled", participant != null
                        ? Math.Round(participant.Stats.RideDistance + participant.Stats.WalkDistance, 2).ToString(CultureInfo.InvariantCulture)
                        : "Unknown", true)
                    .AddField("DBNOs", participant?.Stats.DBNOs.ToString() ?? "Unknown", true)
                    .AddField("Heals Used", participant?.Stats.Heals.ToString() ?? "Unknown", true)
                    .AddField("Boosts Used", participant?.Stats.Boosts.ToString() ?? "Unknown", true)
                    .WithFooter($"Match Data for {participant?.Stats.Name ?? "Unknown"} for match {matchId}",
                        "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400")
                    .Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        private static async Task<PubgPlayer> GetPlayerFromApiAsync(string username, PubgPlatform platform)
        {
            PubgPlayerService service = new PubgPlayerService();

            List<PubgPlayer> players = (await service.GetPlayersAsync(platform, new GetPubgPlayersRequest
            {
                PlayerNames = new[] { username }
            })).ToList();

            return players.FirstOrDefault();
        }

        private static async Task<PubgMatch> GetMatchFromApiAsync(string matchId, PubgPlatform platform)
        {
            PubgMatchService service = new PubgMatchService();

            return await service.GetMatchAsync(platform, matchId);
        }
    }
}