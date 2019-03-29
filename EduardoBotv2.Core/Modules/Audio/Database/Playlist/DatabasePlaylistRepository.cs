using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public class DatabasePlaylistRepository : IPlaylistRepository
    {
        public Task<List<Models.Playlist>> GetPlaylistAsync(long discordUserId)
        {
            return Task.FromResult(new List<Models.Playlist>());
        }

        public Task AddSongToPlaylistAsync(long playlistId, SongInfo song)
        {
            return Task.CompletedTask;
        }

        public Task CreatePlaylistAsync(ulong discordUserId, string playlistName)
        {
            return Task.CompletedTask;
        }
    }
}