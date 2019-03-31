using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public interface IPlaylistRepository
    {
        Task<List<Models.Playlist>> GetPlaylistsAsync(long discordUserId);

        Task<Models.Playlist> GetPlaylistAsync(ulong discordUserId, string playlistName);

        Task AddSongToPlaylistAsync(long playlistId, SongInfo song);

        Task CreatePlaylistAsync(ulong discordUserId, string playlistName);
    }
}