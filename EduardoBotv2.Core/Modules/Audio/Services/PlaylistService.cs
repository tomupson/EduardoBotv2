using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist.Results;
using EduardoBotv2.Core.Modules.Audio.Models;
using EduardoBotv2.Core.Services;
using YoutubeExplode.Models;

namespace EduardoBotv2.Core.Modules.Audio.Services
{
    public class PlaylistService : IEduardoService
    {
        private readonly IPlaylistRepository _playlistRepository;

        public PlaylistService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        public async Task ViewPlaylistAsync(EduardoContext context, string playlistName)
        {
            if (string.IsNullOrWhiteSpace(playlistName)) return;

            Models.Playlist playlist = await _playlistRepository.GetPlaylistAsync((long)context.User.Id, playlistName);

            if (playlist == null)
            {
                await context.Channel.SendMessageAsync($"No playlist found with name \"{playlistName}\"");
                return;
            }

            EmbedBuilder playlistBuilder = new EmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithTitle(playlist.Name)
                .WithCurrentTimestamp();

            if (playlist.Songs.Count > 0)
            {
                playlistBuilder = playlistBuilder
                    .WithFieldsForList(playlist.Songs, x => x.Name, x => x.Url);
            } else
            {
                playlistBuilder = playlistBuilder
                    .WithDescription($"This playlist doesn't contain any songs. Use ```{Constants.CMD_PREFIX}playlist add <playlist name> <song or url>``` to add a song");
            }

            await context.Channel.SendMessageAsync(embed: playlistBuilder.Build());
        }

        public async Task CreatePlaylistAsync(EduardoContext context, string playlistName)
        {
            if (string.IsNullOrWhiteSpace(playlistName)) return;

            CreatePlaylistResult result = await _playlistRepository.CreatePlaylistAsync((long)context.User.Id, playlistName);

            switch (result)
            {
                case CreatePlaylistResult.PlaylistExists:
                    await context.Channel.SendMessageAsync("You have already created a playlist with this name");
                    break;
                case CreatePlaylistResult.MaxPlaylistsReached:
                    await context.Channel.SendMessageAsync("You have reached the maximum number of playlists (5)");
                    break;
                default:
                    await context.Channel.SendMessageAsync($"Playlist \"{playlistName}\" created successfully!");
                    break;
            }
        }

        public async Task DeletePlaylistAsync(EduardoContext context, string playlistName)
        {
            if (string.IsNullOrWhiteSpace(playlistName)) return;

            DeletePlaylistResult result = await _playlistRepository.DeletePlaylistAsync((long)context.User.Id, playlistName);

            switch (result)
            {
                case DeletePlaylistResult.PlaylistNotFound:
                    await context.Channel.SendMessageAsync("Playlist not found");
                    break;
                default:
                    await context.Channel.SendMessageAsync($"Playlist \"{playlistName}\" deleted successfully!");
                    break;
            }
        }

        public async Task AddSongToPlaylistAsync(EduardoContext context, string playlistName, string query)
        {
            if (string.IsNullOrWhiteSpace(playlistName) || string.IsNullOrWhiteSpace(query)) return;

            List<Video> videos = await VideoHelper.GetOrSearchVideoAsync(query);

            if (videos == null || videos.Count == 0)
            {
                await context.Channel.SendMessageAsync($"Could not find any videos related to \"{query}\"");
                return;
            }

            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            if (videos.Count == 1)
            {
                tcs.TrySetResult(videos[0].GetShortUrl());
            } else
            {
                await context.SendPaginatedMessageAsync(new PaginatedMessage
                {
                    Text = "Select which song to add to your playlist",
                    Embeds = videos.Take(5).Select(video => new EmbedBuilder()
                        .WithTitle(video.Title)
                        .WithColor(Color.Red)
                        .WithDescription(video.Description)
                        .WithUrl(video.GetShortUrl())
                        .WithThumbnailUrl(video.Thumbnails.StandardResUrl)
                        .Build()).ToList(),
                    Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS),
                    Reactions = new List<PaginatedMessageReaction>
                    {
                        new PaginatedMessageReaction(new Emoji("\u2795"), (embed, paginatedMsgCts) =>
                        {
                            tcs.TrySetResult(embed.Url);
                            paginatedMsgCts.Cancel();
                        })
                    },
                    TimeoutCallback = () => tcs.TrySetResult("")
                });
            }

            await tcs.Task;

            Video chosenVideo = videos.FirstOrDefault(v => v.GetShortUrl() == tcs.Task.Result);

            if (chosenVideo == null) return;

            AddSongResult result = await _playlistRepository.AddSongToPlaylistAsync((long)context.User.Id, playlistName, new PlaylistSong
            {
                Name = chosenVideo.Title,
                Url = chosenVideo.GetShortUrl()
            });

            switch (result)
            {
                case AddSongResult.PlaylistNotFound:
                    await context.Channel.SendMessageAsync("Could not find the specified playlist");
                    break;
                case AddSongResult.SongAlreadyInPlaylist:
                    await context.Channel.SendMessageAsync("Song already exists in playlist");
                    break;
                default:
                    await context.Channel.SendMessageAsync($"Successfully added \"{chosenVideo.Title}\" to playlist \"{playlistName}\"");
                    break;
            }
        }

        public async Task RemoveSongFromPlaylistAsync(EduardoContext context, string playlistName, string songName)
        {
            if (string.IsNullOrWhiteSpace(playlistName) || string.IsNullOrWhiteSpace(songName)) return;

            RemoveSongResult result = await _playlistRepository.RemoveSongFromPlaylistAsync((long)context.User.Id, playlistName, songName);

            switch (result)
            {
                case RemoveSongResult.PlaylistNotFound:
                    await context.Channel.SendMessageAsync("Could not find the specified playlist");
                    break;
                case RemoveSongResult.SongNotInPlaylist:
                    await context.Channel.SendMessageAsync("Specified song is not in playlist");
                    break;
                default:
                    await context.Channel.SendMessageAsync($"Successfully removed \"{songName}\" from playlist \"{playlistName}\"");
                    break;
            }
        }

        public async Task RemoveSongFromPlaylistByIndexAsync(EduardoContext context, string playlistName, int index)
        {
            if (string.IsNullOrWhiteSpace(playlistName) || index <= 0) return;

            RemoveSongResult result = await _playlistRepository.RemoveSongFromPlaylistByIndexAsync((long)context.User.Id, playlistName, index);

            switch (result)
            {
                case RemoveSongResult.PlaylistNotFound:
                    await context.Channel.SendMessageAsync("Could not find the specified playlist");
                    break;
                case RemoveSongResult.SongNotInPlaylist:
                    await context.Channel.SendMessageAsync("Specified song is not in playlist");
                    break;
                default:
                    await context.Channel.SendMessageAsync($"Successfully removed item {index} from playlist {playlistName}");
                    break;
            }
        }
    }
}