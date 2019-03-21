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
using Newtonsoft.Json;
using Pubg.Net;
using Pubg.Net.Models.Participants;

namespace EduardoBotv2.Core.Modules.PUBG.Services
{
    public class PubgService
    {
        private readonly PubgData pubgData;

        public PubgService()
        {
            pubgData = JsonConvert.DeserializeObject<PubgData>(File.ReadAllText("data/pubg.json"));
        }

        public async Task GetPlayer(EduardoContext context, string username, PubgPlatform platform)
        {
            PubgPlayer player = await GetPlayerFromApiAsync(username, platform);

            if (player == null)
            {
                await context.Channel.SendMessageAsync($"No player found with username \"{username}\"");
                return;
            }

            EmbedBuilder builder = new EmbedBuilder
            {
                Color = Color.Orange,
                Description = $"You can view recent matches for this player using `{Constants.CMD_PREFIX}pubg matches {player.Name} {platform}`",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Account ID",
                        Value = player.Id
                    }
                },
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400",
                    Text = $"Player Profile for {player.Name}"
                },
                Title = player.Name
            };

            await context.Channel.SendMessageAsync(embed: builder.Build());
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
            foreach (string matchId in player.MatchIds.Take(pubgData.MaxMatches))
            {
                PubgMatch match = await GetMatchFromApiAsync(matchId, platform);

                TimeSpan matchDuration = TimeSpan.FromSeconds(match.Duration);
                PubgRoster roster = match.Rosters.FirstOrDefault(r => r.Participants.Any(p => p.Stats.PlayerId == player.Id));
                PubgParticipant participant = match.Rosters.SelectMany(r => r.Participants).FirstOrDefault(p => p.Stats.PlayerId == player.Id);
                TimeSpan timeSurvived = TimeSpan.FromSeconds(participant?.Stats.TimeSurvived ?? 0);

                EmbedBuilder matchEmbedBuilder = new EmbedBuilder
                {
                    Color = Color.Orange,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Started",
                            Value = DateTime.Parse(match.CreatedAt)
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Duration",
                            Value = $"{matchDuration.Minutes:D2}m {matchDuration.Seconds:D2}s"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Player Count",
                            Value = match.Rosters.Count()
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Placement",
                            Value = roster != null
                                ? $"#{roster.Stats.Rank}"
                                : "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Death Reason",
                            Value = participant?.Stats.DeathType.ToString() ?? "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Kills",
                            Value = participant?.Stats.Kills.ToString() ?? "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Damage Dealt",
                            Value = participant != null
                                ? Math.Round(participant.Stats.DamageDealt, 2).ToString(CultureInfo.InvariantCulture)
                                : "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Headshot %",
                            Value = participant != null
                                ? participant.Stats.Kills > 0
                                    ? (participant.Stats.HeadshotKills / participant.Stats.Kills * 100).ToString()
                                    : "0%"
                                : "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Time Survived",
                            Value = participant != null
                                ? $"{timeSurvived.Minutes:D2}m {timeSurvived.Seconds:D2}s"
                                : "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Distance Travelled",
                            Value = participant != null
                                ? Math.Round(participant.Stats.RideDistance + participant.Stats.WalkDistance, 2).ToString(CultureInfo.InvariantCulture)
                                : "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "DBNOs",
                            Value = participant?.Stats.DBNOs.ToString() ?? "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Heals Used",
                            Value = participant?.Stats.Heals.ToString() ?? "Unknown"
                        },
                        new EmbedFieldBuilder
                        {
                            IsInline = true,
                            Name = "Boosts Used",
                            Value = participant?.Stats.Boosts.ToString() ?? "Unknown"
                        }
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = "https://steemit-production-imageproxy-thumbnail.s3.amazonaws.com/U5dt5ZoFC4oMbrTPSSvVHVfyGSakHWV_1680x8400",
                        Text = $"Match Data for {participant?.Stats.Name ?? "Unknown"} for match {matchId}"
                    },
                    Title = match.GameMode.ToString()
                };

                if (participant != null && participant.Stats.DeathType == PubgParticipantDeathType.Alive)
                {
                    matchEmbedBuilder.Fields.RemoveAll(x => x.Name.ToLower() == "death reason");
                }

                embeds.Add(matchEmbedBuilder.Build());
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