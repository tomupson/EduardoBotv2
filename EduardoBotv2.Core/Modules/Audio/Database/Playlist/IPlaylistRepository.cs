using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public interface IPlaylistRepository
    {
        Task<List<Models.Playlist>> GetPlaylist(long discordUserId);

        Task AddSongToPlaylist(long playlistId, Song song);
    }
}