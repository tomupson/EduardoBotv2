using Discord;
using Discord.Audio;
using Newtonsoft.Json.Linq;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Utilities;
using EduardoBotv2.Common.Data.Enums;
using EduardoBotv2.Common.Extensions;
using EduardoBotv2.Common.Data.Models;
using EduardoBotv2.Common.Utilities.Helpers;
using System;
using System.Xml;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace EduardoBotv2.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private readonly List<Song> _queue = new List<Song>();
        private static Song currentSong;

        private static bool queueRunning = false;
        private static bool songPlaying = false;

        public async Task AddSongToQueue(EduardoContext c, string input)
        {
            Song requestedSong = await GetVideoInfo(c, input);

            if (requestedSong != null)
            {
                _queue.Add(requestedSong);
                await c.Channel.SendMessageAsync($"Added **{requestedSong.Title}** to the queue.");
            } else
            {
                await c.Channel.SendMessageAsync($"**Could not find video like '{input}'**");
            }
        }

        public async Task RemoveSongFromQueue(EduardoContext c, int queueIndex)
        {
            if (queueIndex < 1 || queueIndex > _queue.Count) return;

            Song requestedSong = _queue[queueIndex-1];

            if (queueIndex == 1 && queueRunning)
            {
                await c.Channel.SendMessageAsync("**Cannot remove the current song in the queue! Use `$queue skip` to move onto the next song.**");
                return;
            }

            _queue.Remove(requestedSong);
            await c.Channel.SendMessageAsync($"Removed **{requestedSong.Title}** from the queue.");
        }

        private EmbedBuilder BuildPlayingSongEmbed()
        {
            string desc = currentSong.Description.Length < Config.MAX_DESCRIPTION_LENGTH ? currentSong.Description : currentSong.Description.Substring(0, Config.MAX_DESCRIPTION_LENGTH);
            return new EmbedBuilder()
            {
                Title = currentSong.Title,
                Color = Color.Red,
                Url = currentSong.Url,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Video Description",
                        Value = desc
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Requested by",
                        Value = currentSong.RequestedBy.Boldify()
                    }
                },
                Footer = new EmbedFooterBuilder()
                {
                    IconUrl = @"https://i.imgur.com/Fsaf4OW.png",
                    Text = $"{currentSong.TimePassed.ToDurationString()} / {currentSong.Duration.Value.ToDurationString()}"
                },
                ThumbnailUrl = currentSong.ThumbnailUrl
            };
        }

        public async Task ShowCurrentlyPlayingSong(EduardoContext c)
        {
            if (currentSong != null)
            {
                EmbedBuilder builder = BuildPlayingSongEmbed();
                await c.Channel.SendMessageAsync("**Currently Playing:**", false, builder.Build());

            } else
            {
                await c.Channel.SendMessageAsync("**There is no song currently playing.**");
            }
        }

        private async Task ShowCurrentlyPlaying(EduardoContext c)
        {
            EmbedBuilder builder = BuildPlayingSongEmbed();
            await c.Channel.SendMessageAsync("**Now Playing:**", false, builder.Build());
        }

        private async Task SendAudioAsync(EduardoContext c, Song song)
        {
            await ShowCurrentlyPlaying(c);

            IAudioClient client;
            if (ConnectedChannels.TryGetValue(c.Guild.Id, out client))
            {
                await Logger.Log(new LogMessage(LogSeverity.Debug, "Eduardo Bot", $"Starting playback of {song.Title} in {c.Guild.Name}"));

                var output = CreateStream(song.StreamUrl).StandardOutput.BaseStream;

                var stream = client.CreatePCMStream(AudioApplication.Mixed);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }

        public async Task ClearQueue()
        {
            _queue.Clear();
            await Task.CompletedTask;
        }

        public async Task PlaySong(EduardoContext c, string input)
        {
            if (songPlaying)
            {
                await LeaveAudio(c);
                songPlaying = false;
                await PlaySong(c, input);
            }

            if (queueRunning)
            {
                await c.Channel.SendMessageAsync("**Cannot play song while queue is running! Use `$stop` to stop the queue.**");
                return;
            }

            songPlaying = true;
            Song song = await GetVideoInfo(c, input);
            if (song != null)
            {
                currentSong = song;
                await JoinAudio(c);
                await SendAudioAsync(c, song);
                songPlaying = false;
                currentSong = null;
                await LeaveAudio(c);
            } else
            {
                await c.Channel.SendMessageAsync($"**Could not find video like '{input}'**");
            }
        }

        public async Task StartQueue(EduardoContext c)
        {
            if (_queue.Count == 0)
            {
                await c.Channel.SendMessageAsync("**There are no songs in the queue! Use `$queue add <song>` to add items to the queue!**");
                return;
            }

            if (songPlaying)
            {
                await c.Channel.SendMessageAsync("**Cannot play queue while song is playing! Use `$stop` to stop the song.**");
                return;
            }

            if (queueRunning)
            {
                await c.Channel.SendMessageAsync($"**Queue already running! Use `$stop` to stop the queue.**");
                return;
            } else
            {
                queueRunning = true;
                await JoinAudio(c);
                await PlayQueue(c);
            }
        }

        public async Task PlayQueue(EduardoContext c)
        {
            currentSong = _queue[0];
            await SendAudioAsync(c, _queue[0]);
            _queue.RemoveAt(0);

            if (_queue.Count > 0)
            {
                await PlayQueue(c);
            }
            else
            {
                queueRunning = false;
                currentSong = null;
                await LeaveAudio(c);
            }
        }

        public async Task ViewQueue(EduardoContext c)
        {
            if (_queue.Count > 0)
            {
                string queueInfo = "**Queue:**";
                for (int i = 0; i < _queue.Count; i++)
                {
                    queueInfo += $"\n{i + 1}. **{_queue[i].Title}**";
                    if (i == 0 && queueRunning) queueInfo += " => **[CURRENTLY PLAYING]**";
                }
                await c.Channel.SendMessageAsync(queueInfo);
            } else
            {
                await c.Channel.SendMessageAsync("**There are no songs in the queue! Use `$queue add <song>` to add songs to the queue.**");
            }
        }

        public async Task SkipQueueSong(EduardoContext c)
        {
            if (songPlaying)
            {
                await c.Channel.SendMessageAsync("**A song is already playing! Use `$stop` to stop the song.**");
                return;
            }

            var skipMsg = await c.Channel.SendMessageAsync("**Skipping Song...**");
            _queue.RemoveAt(0);
            await skipMsg.DeleteAsync();
            await LeaveAudio(c);
            if (_queue.Count > 0)
            {
                await JoinAudio(c);
                await PlayQueue(c);
            }
        }

        public async Task Stop(EduardoContext c)
        {
            queueRunning = false;
            songPlaying = false;
            currentSong = null;
            await LeaveAudio(c);
        }

        private async Task JoinAudio(EduardoContext c)
        {
            IVoiceChannel target = (c.User as IVoiceState).VoiceChannel;
            IAudioClient client;

            if (ConnectedChannels.TryGetValue(c.Guild.Id, out client)) return;
            if (target.Guild.Id != c.Guild.Id) return;

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(c.Guild.Id, audioClient))
            {
                await Logger.Log(new LogMessage(LogSeverity.Info, "Eduardo Bot", $"Connected to voice channel on {c.Guild.Name}"));
            } else
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "Eduardo Bot", $"Failed to join a voice channel on {c.Guild.Name}"));
            }
        }

        private async Task LeaveAudio(EduardoContext c)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(c.Guild.Id, out client))
            {
                await client.StopAsync();
                await Logger.Log(new LogMessage(LogSeverity.Info, "Eduardo Bot", $"Disconnected from voice channel on {c.Guild.Name}"));
            }
        }

        private Process CreateStream(string url)
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -err_detect ignore_err -i {url} -vol 75 -f s16le -ar 48000 -vn -ac 2 pipe:1 -loglevel panic",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        private async Task<Song> GetVideoInfo(EduardoContext c, string input)
        {
            Song outputSong = null;

            string videoId = string.Empty;

            List<string> validAuthorities = new List<string>(){ "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };

            if (validAuthorities.Any(s => input.Contains(s)))
            {
                Regex regexExtractId = new Regex(Config.YOUTUBE_LINK_REGEX, RegexOptions.Compiled);
                Uri uri = new Uri(input);
                try
                {
                    string authority = new UriBuilder(uri).Uri.Authority.ToLower();
                    if (validAuthorities.Contains(authority))
                    {
                        var regRes = regexExtractId.Match(uri.ToString());
                        if (regRes.Success)
                        {
                            videoId = regRes.Groups[1].Value;
                        }
                    }
                }
                catch { }

            } else
            {
                var searchVideoResponse = await GoogleHelper.SearchYouTubeAsync(c.EduardoSettings.GoogleYouTubeApiKey, "snippet", input, 1, YouTubeRequestType.video);

                if (searchVideoResponse.Items.Count > 0)
                {
                    videoId = searchVideoResponse.Items[0].Id.VideoId;
                }
                else return null;
            }

            var getVideoByIdResponse = await GoogleHelper.GetVideoFromYouTubeAsync(c.EduardoSettings.GoogleYouTubeApiKey, "snippet,contentDetails", videoId, 1);

            if (getVideoByIdResponse.Items.Count == 0) return null;

            outputSong = new Song()
            {
                Title = getVideoByIdResponse.Items[0].Snippet.Title,
                Duration = XmlConvert.ToTimeSpan(getVideoByIdResponse.Items[0].ContentDetails.Duration),
                VideoId = videoId,
                Url = $"http://youtu.be/{videoId}",
                StreamUrl = GetStreamUrl($"http://youtu.be/{videoId}"),
                RequestedBy = c.User as IGuildUser,
                Description = getVideoByIdResponse.Items[0].Snippet.Description,
                ThumbnailUrl = getVideoByIdResponse.Items[0].Snippet.Thumbnails.Default__.Url
            };

            return outputSong;
        }

        private string GetStreamUrl(string url)
        {
            Process youtubedl;

            ProcessStartInfo youtubedlGetStreamUrl = new ProcessStartInfo()
            {
                FileName = "youtube-dl",
                Arguments = $"-url {url} -p \"\" -s -J -i -q --no-warnings --geo-bypass --no-check-certificate --no-call-home",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            youtubedl = Process.Start(youtubedlGetStreamUrl);
            
            var output = youtubedl.StandardOutput.ReadToEnd();
            var songJson = JObject.Parse(output);

            dynamic entryJson = songJson;
            string streamUrl = entryJson.url ?? entryJson.requested_formats[1]?.url;
            if (string.IsNullOrEmpty(streamUrl))
            {
                return string.Empty;
            }

            return streamUrl;
        }
    }
}