using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist.Results;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public interface IPlaylistRepository
    {
        Task<List<Models.Playlist>> GetPlaylistsAsync(long discordUserId);

        Task<Models.Playlist> GetPlaylistAsync(ulong discordUserId, string playlistName);

        Task<AddSongResult> AddSongToPlaylistAsync(ulong discordUserId, string playlistName, PlaylistSong song);

        Task<RemoveSongResult> RemoveSongFromPlaylistAsync(ulong discordUserId, string playlistName, string songName);

        Task<RemoveSongResult> RemoveSongFromPlaylistByIndexAsync(ulong discordUserId, string playlistName, int index);

        Task<CreatePlaylistResult> CreatePlaylistAsync(ulong discordUserId, string playlistName);
    }
}