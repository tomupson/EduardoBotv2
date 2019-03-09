using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Audio;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Models.Enums;
using EduardoBotv2.Core.Models.Music;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json.Linq;

namespace EduardoBotv2.Core.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private readonly List<Song> queue = new List<Song>();
        private float volume = 1.0f;
        private CancellationTokenSource queueCts;
        private CancellationTokenSource audioCts;

        private TaskCompletionSource<bool> PauseTaskSource { get; set; }

        public async Task PlaySong(EduardoContext context, string input)
        {
            Song song = await GetVideoInfo(context, input);

            if (song != null)
            {
                audioCts?.Cancel();
                queueCts?.Cancel();
                queue.Insert(0, song);
                await StartQueue(context);
            } else
            {
                await context.Channel.SendMessageAsync($"Could not find video like \"{input}\"");
            }
        }

        public async Task StartQueue(EduardoContext context)
        {
            queueCts = new CancellationTokenSource();
            if (queue.Count == 0)
            {
                await context.Channel.SendMessageAsync($"There are no songs in the queue! Use `{Constants.CMD_PREFIX}queue add <song>` to add items to the queue");
                return;
            }

            await JoinAudio(context);

            if (connectedChannels.TryGetValue(context.Guild.Id, out IAudioClient client))
            {
                using (AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music, bufferMillis: 1))
                {
                    while (queue.Count > 0 && !queueCts.IsCancellationRequested)
                    {
                        await SendAudioAsync(context, queue[0], stream);
                        queue.RemoveAt(0);
                    }
                }
            }

            await LeaveAudio(context.Guild);
        }

        public void Stop()
        {
            queueCts?.Cancel();
            audioCts?.Cancel();
        }

        public async Task AddSongToQueue(EduardoContext context, string input)
        {
            Song requestedSong = await GetVideoInfo(context, input);

            if (requestedSong != null)
            {
                queue.Add(requestedSong);
                await context.Channel.SendMessageAsync($"Added **{requestedSong.Title}** to the queue");
            } else
            {
                await context.Channel.SendMessageAsync($"Could not find video like \"{input}");
            }
        }

        public async Task RemoveSongFromQueue(EduardoContext context, int queueNum)
        {
            if (queueNum < 1 || queueNum > queue.Count) return;

            if (queueNum == 1)
            {
                await Skip(context);
            }

            Song requestedSong = queue[queueNum - 1];

            // Skip current song if queue is running and queueNum == 1?

            queue.Remove(requestedSong);
            await context.Channel.SendMessageAsync($"Removed {requestedSong.Title.Boldify()} from the queue");
        }

        public async Task Skip(EduardoContext context)
        {
            if (queue.Count > 0)
            {
                IUserMessage skipMessage = await context.Channel.SendMessageAsync("Skipping song...");
                audioCts?.Cancel();
                await skipMessage.DeleteAsync();
            }
        }

        public async Task ShowCurrentSong(EduardoContext context)
        {
            if (queue.Count > 0 && !(audioCts.IsCancellationRequested || queueCts.IsCancellationRequested))
            {
                await context.Channel.SendMessageAsync("Currently playing:", false, BuildSongEmbed(queue[0]));
            } else
            {
                await context.Channel.SendMessageAsync("There is no song playing");
            }
        }

        public void ClearQueue()
        {
            queue.Clear();
        }

        public void SetVolume(int newVolume)
        {
            if (newVolume < 0 || volume > 100)
                throw new ArgumentOutOfRangeException(nameof(newVolume));

            volume = (float) newVolume / 100;
        }

        public void TogglePause()
        {
            if (PauseTaskSource == null)
                PauseTaskSource = new TaskCompletionSource<bool>();
            else
            {
                PauseTaskSource?.TrySetResult(true);
                PauseTaskSource = null;
            }
        }

        public async Task ViewQueue(EduardoContext context)
        {
            if (queue.Count > 0)
            {
                string queueInfo = "Queue:";
                for (int i = 0; i < queue.Count; i++)
                {
                    queueInfo += $"\n{i + 1}. {queue[i].Title}";
                }

                await context.Channel.SendMessageAsync(queueInfo);
            } else
            {
                await context.Channel.SendMessageAsync($"There are no songs in the queue! Use {$"`{Constants.CMD_PREFIX}queue add <song>`".Boldify()} to add a song to the queue");
            }
        }

        private async Task JoinAudio(EduardoContext context)
        {
            // Get connected voice channel of requesting user
            IVoiceChannel target = (context.User as IVoiceState)?.VoiceChannel;

            // Don't join audio if not correct guild.
            if (target?.Guild.Id != context.Guild.Id) return;

            // Leave current audio channel on guild (if any)
            if (connectedChannels.ContainsKey(context.Guild.Id))
            {
                await LeaveAudio(context.Guild);
            }

            // Connect to new audio channel
            IAudioClient audioClient = await target.ConnectAsync();

            // Register connection in connected channels
            if (connectedChannels.TryAdd(context.Guild.Id, audioClient))
            {
                await Logger.Log(new LogMessage(LogSeverity.Info, "EduardoRemastered", $"Connected to voice channel on {context.Guild.Name}"));
            } else
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "EduardoRemastered", $"Failed to join voice channel on {context.Guild.Name}"));
            }
        }

        private async Task LeaveAudio(IGuild guild)
        {
            // Unregister from connected channels
            if (connectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                // Disconnect from connected channel
                await client.StopAsync();

                await Logger.Log(new LogMessage(LogSeverity.Debug, "EduardoRemastered", $"Disconncted from voice in {guild.Name}"));
            }
        }

        private async Task SendAudioAsync(EduardoContext context, Song song, AudioOutStream discordStream)
        {
            audioCts = new CancellationTokenSource();

            await ShowCurrentSong(context);

            await Logger.Log(new LogMessage(LogSeverity.Debug, "EduardoRemastered", $"Starting playback of {song.Title} in {context.Guild.Name}"));

            SongBuffer songBuffer = null;

            try
            {
                songBuffer = new SongBuffer(song.StreamUrl);
                songBuffer.StartBuffering();

                await Task.WhenAny(Task.Delay(10000), songBuffer.PrebufferingCompleted.Task);

                while (true)
                {
                    byte[] buffer = songBuffer.Read(3840);
                    if (buffer.Length == 0) break;

                    AdjustVolume(ref buffer, volume);

                    await discordStream.WriteAsync(buffer, 0, buffer.Length, audioCts.Token);

                    await (PauseTaskSource?.Task ?? Task.CompletedTask);
                }
            } catch (Exception ex)
            {
                if (ex is OperationCanceledException ocEx && !ocEx.CancellationToken.IsCancellationRequested) throw;
            } finally
            {
                await discordStream.FlushAsync();
                songBuffer?.Dispose();
            }
        }

        private static async Task<Song> GetVideoInfo(EduardoContext context, string input)
        {
            string videoId = "";

            List<string> validAuthorities = new List<string> {"youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be"};

            // Check if user input is any form of youtube url
            if (validAuthorities.Any(input.ToLower().Contains))
            {
                Uri uri = new Uri(input);

                if (validAuthorities.Contains(uri.Authority.ToLower()))
                {
                    // If so, get the video id query parameter using regex
                    Regex idExtractionRegex = new Regex(Constants.YOUTUBE_LINK_REGEX, RegexOptions.Compiled);
                    Match idMatch = idExtractionRegex.Match(uri.ToString());
                    if (idMatch.Success)
                    {
                        videoId = idMatch.Groups[1].Value;
                    }
                }
            } else
            {
                // Otherwise, search youtube for the user input and get the video id that way
                SearchListResponse response = await GoogleHelper.SearchYouTubeAsync(context.EduardoCredentials.GoogleYouTubeApiKey, "snippet", input, 1, YouTubeRequestType.Video);

                if (response.Items.Count > 0)
                {
                    videoId = response.Items[0].Id.VideoId;
                } else return null;
            }

            // Get the video by the previously determined id
            VideoListResponse getVideoResponse = await GoogleHelper.GetVideoFromYouTubeAsync(context.EduardoCredentials.GoogleYouTubeApiKey, "snippet,contentDetails", videoId, 1);

            if (getVideoResponse.Items.Count == 0) return null;

            string youtubeUrl = $"http://youtu.be/{videoId}";

            return new Song
            {
                Title = getVideoResponse.Items[0].Snippet.Title,
                Duration = XmlConvert.ToTimeSpan(getVideoResponse.Items[0].ContentDetails.Duration),
                VideoId = videoId,
                Url = youtubeUrl,
                StreamUrl = GetStreamUrl(youtubeUrl),
                RequestedBy = context.User as IGuildUser,
                Description = getVideoResponse.Items[0].Snippet.Description,
                ThumbnailUrl = getVideoResponse.Items[0].Snippet.Thumbnails.Default__.Url
            };
        }

        private static string GetStreamUrl(string url)
        {
            // Use youtube-dl to get the url of the video stream, from the youtube url
            Process youtubeDl = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "youtube-dl",
                    Arguments = $"-j --geo-bypass --no-call-home {url}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.CurrentDirectory
                }
            };

            youtubeDl.Start();

            string output = youtubeDl.StandardOutput.ReadToEnd();
            dynamic songJson = JObject.Parse(output);
            return songJson.url ?? songJson.requested_formats[1]?.url ?? "";
        }

        private static Embed BuildSongEmbed(Song song)
        {
            return new EmbedBuilder
            {
                Title = song.Title,
                Color = Color.Red,
                Url = song.Url,
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "Video Description",
                        Value = !string.IsNullOrEmpty(song.Description) ? song.Description.Length < Constants.MAX_DESCRIPTION_LENGTH ?
                            song.Description :
                            song.Description.Substring(0, Constants.MAX_DESCRIPTION_LENGTH) : "-"
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Requested by",
                        Value = song.RequestedBy.Boldify()
                    }
                },
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = @"https://i.imgur.com/Fsaf4OW.png",
                    Text = $"{song.TimePassed.ToDurationString()} / {song.Duration?.ToDurationString()}"
                },
                ThumbnailUrl = song.ThumbnailUrl
            }.Build();
        }

        private static unsafe void AdjustVolume(ref byte[] audioSamples, float volume)
        {
            if (Math.Abs(volume - 1f) < 0.0001f) return;

            // 16-bit precision for the multiplication
            int volumeFixed = (int) Math.Round(volume * 65536d);

            int count = audioSamples.Length / 2;

            fixed (byte* srcBytes = audioSamples)
            {
                short* src = (short*) srcBytes;

                for (int i = count; i != 0; i--, src++)
                {
                    *src = (short) ((*src * volumeFixed) >> 16);
                }
            }
        }
    }
}