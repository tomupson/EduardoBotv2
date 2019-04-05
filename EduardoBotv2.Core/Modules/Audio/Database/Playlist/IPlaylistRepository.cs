using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist.Results;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public interface IPlaylistRepository
    {
        Task<List<Models.Playlist>> GetPlaylistsAsync(long discordUserId);

        Task<Models.Playlist> GetPlaylistAsync(long discordUserId, string playlistName);

        Task<AddSongResult> AddSongToPlaylistAsync(long discordUserId, string playlistName, PlaylistSong song);

        Task<RemoveSongResult> RemoveSongFromPlaylistAsync(long discordUserId, string playlistName, string songName);

        Task<RemoveSongResult> RemoveSongFromPlaylistByIndexAsync(long discordUserId, string playlistName, int index);

        Task<CreatePlaylistResult> CreatePlaylistAsync(long discordUserId, string playlistName);

        Task<DeletePlaylistResult> DeletePlaylistAsync(long discordUserId, string playlistName);
    }
}