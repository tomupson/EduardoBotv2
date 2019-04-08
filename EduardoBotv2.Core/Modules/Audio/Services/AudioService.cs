using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Audio.Models;
using EduardoBotv2.Core.Services;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace EduardoBotv2.Core.Modules.Audio.Services
{
    public class AudioService : IEduardoService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private readonly List<SongInfo> _queue = new List<SongInfo>();

        private float volume = 1.0f;
        private CancellationTokenSource queueCts;
        private CancellationTokenSource audioCts;

        private TaskCompletionSource<bool> PauseTaskSource { get; set; }

        public async Task PlaySong(EduardoContext context, string input)
        {
            SongInfo song = await GetVideoInfo(context, input);

            if (song != null)
            {
                audioCts?.Cancel();
                queueCts?.Cancel();
                _queue.Insert(0, song);
                await StartQueue(context);
            } else
            {
                await context.Channel.SendMessageAsync($"Could not find video like \"{input}\"");
            }
        }

        public async Task StartQueue(EduardoContext context)
        {
            queueCts = new CancellationTokenSource();
            if (_queue.Count == 0)
            {
                await context.Channel.SendMessageAsync($"There are no songs in the queue! Use `{Constants.CMD_PREFIX}queue add <song>` to add items to the queue");
                return;
            }

            await JoinAudio(context);

            if (_connectedChannels.TryGetValue(context.Guild.Id, out IAudioClient client))
            {
                using (AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music, bufferMillis: 1))
                {
                    while (_queue.Count > 0 && !queueCts.IsCancellationRequested)
                    {
                        await SendAudioAsync(context, _queue[0], stream);
                        _queue.RemoveAt(0);
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
            SongInfo requestedSong = await GetVideoInfo(context, input);

            if (requestedSong != null)
            {
                _queue.Add(requestedSong);
                await context.Channel.SendMessageAsync($"Added {requestedSong.Name.Boldify()} to the queue");
            } else
            {
                await context.Channel.SendMessageAsync($"Could not find video like \"{input}\"");
            }
        }

        public async Task RemoveSongFromQueue(EduardoContext context, int queueNum)
        {
            if (queueNum < 1 || queueNum > _queue.Count) return;

            if (queueNum == 1)
            {
                await Skip(context);
            }

            SongInfo requestedSong = _queue[queueNum - 1];

            // Skip current song if queue is running and queueNum == 1?

            _queue.Remove(requestedSong);
            await context.Channel.SendMessageAsync($"Removed {requestedSong.Name.Boldify()} from the queue");
        }

        public async Task Skip(EduardoContext context)
        {
            if (_queue.Count > 0)
            {
                IUserMessage skipMessage = await context.Channel.SendMessageAsync("Skipping song...");
                audioCts?.Cancel();
                await skipMessage.DeleteAsync();
            }
        }

        public async Task ShowCurrentSong(EduardoContext context)
        {
            if (_queue.Count > 0 && !(audioCts.IsCancellationRequested || queueCts.IsCancellationRequested))
            {
                await context.Channel.SendMessageAsync("Currently playing:", false, BuildSongEmbed(_queue[0]));
            } else
            {
                await context.Channel.SendMessageAsync("There is no song playing");
            }
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        public void SetVolume(int newVolume)
        {
            if (newVolume < 0 || newVolume > 100)
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
            if (_queue.Count > 0)
            {
                string queueInfo = "Queue:";
                for (int i = 0; i < _queue.Count; i++)
                {
                    queueInfo += $"\n{i + 1}. {_queue[i].Name}";
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
            if (_connectedChannels.ContainsKey(context.Guild.Id))
            {
                await LeaveAudio(context.Guild);
            }

            // Connect to new audio channel
            IAudioClient audioClient = await target.ConnectAsync();

            // Register connection in connected channels
            if (_connectedChannels.TryAdd(context.Guild.Id, audioClient))
            {
                await Logger.Log(new LogMessage(LogSeverity.Info, "Eduardov2", $"Connected to voice channel on {context.Guild.Name}"));
            } else
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "Eduardov2", $"Failed to join voice channel on {context.Guild.Name}"));
            }
        }

        private async Task LeaveAudio(IGuild guild)
        {
            // Unregister from connected channels
            if (_connectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                // Disconnect from connected channel
                await client.StopAsync();

                await Logger.Log(new LogMessage(LogSeverity.Debug, "Eduardov2", $"Disconncted from voice in {guild.Name}"));
            }
        }

        private async Task SendAudioAsync(EduardoContext context, SongInfo song, AudioOutStream discordStream)
        {
            audioCts = new CancellationTokenSource();

            await ShowCurrentSong(context);

            await Logger.Log(new LogMessage(LogSeverity.Debug, "Eduardov2", $"Starting playback of {song.Name} in {context.Guild.Name}"));

            SongBuffer songBuffer = null;

            try
            {
                songBuffer = new SongBuffer(song.StreamUrl);
                songBuffer.StartBuffering();

                await Task.WhenAny(Task.Delay(10000), songBuffer.PrebufferingCompleted.Task);

                while (true)
                {
                    byte[] buffer = songBuffer.Read(3840).ToArray();
                    if (buffer.Length == 0) break;

                    AdjustVolume(ref buffer, volume);

                    await discordStream.WriteAsync(buffer, 0, buffer.Length, audioCts.Token);

                    await (PauseTaskSource?.Task ?? Task.CompletedTask);
                }
            } catch (Exception ex)
            {
                if (ex is OperationCanceledException cancelledException && !cancelledException.CancellationToken.IsCancellationRequested) throw;
            } finally
            {
                await discordStream.FlushAsync();
                songBuffer?.Dispose();
            }
        }

        private static async Task<SongInfo> GetVideoInfo(EduardoContext context, string input)
        {
            List<Video> videos = await VideoHelper.GetOrSearchVideoAsync(input);

            if (videos == null || videos.Count == 0) return null;

            Video chosenVideo = videos.FirstOrDefault();

            if (chosenVideo == null) return null;

            MediaStreamInfoSet streamInfo = await VideoHelper.GetMediaStreamInfoAsync(chosenVideo);
            AudioStreamInfo stream = streamInfo.Audio.OrderByDescending(a => a.Bitrate).FirstOrDefault();

            if (stream == null) return null;

            return new SongInfo
            {
                Name = chosenVideo.Title,
                Duration = chosenVideo.Duration,
                VideoId = chosenVideo.Id,
                Url = chosenVideo.GetShortUrl(),
                StreamUrl = stream.Url,
                RequestedBy = context.User as IGuildUser,
                Description = chosenVideo.Description,
                ThumbnailUrl = chosenVideo.Thumbnails.MediumResUrl
            };
        }

        private static Embed BuildSongEmbed(SongInfo song) => new EmbedBuilder()
            .WithTitle(song.Name)
            .WithColor(Color.Red)
            .WithThumbnailUrl(song.ThumbnailUrl)
            .WithUrl(song.Url)
            .AddField("Video Description", !string.IsNullOrEmpty(song.Description) ? song.Description.Length < Constants.MAX_DESCRIPTION_LENGTH ?
                song.Description :
                song.Description.Substring(0, Constants.MAX_DESCRIPTION_LENGTH) : "-")
            .AddField("Requested by", song.RequestedBy.Boldify())
            .WithFooter($"{song.TimePassed.ToDurationString()} / {song.Duration?.ToDurationString()}", @"https://i.imgur.com/Fsaf4OW.png")
            .Build();

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