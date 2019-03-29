using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public interface IPlaylistRepository
    {
        Task<List<Models.Playlist>> GetPlaylistAsync(long discordUserId);

        Task AddSongToPlaylistAsync(long playlistId, SongInfo song);

        Task CreatePlaylistAsync(ulong discordUserId, string playlistName);
    }
}