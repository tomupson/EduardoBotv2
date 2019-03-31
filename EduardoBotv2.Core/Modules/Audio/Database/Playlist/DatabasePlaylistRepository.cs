using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Database;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public class DatabasePlaylistRepository : IPlaylistRepository
    {
        private readonly Credentials _credentials;

        public DatabasePlaylistRepository(Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<List<Models.Playlist>> GetPlaylistsAsync(long discordUserId)
        {
            DataReader reader = new DataReader("PLAYLIST_GetPlaylists", _credentials.DbConnectionString);
            reader.AddParameter("@DiscordUserId", discordUserId);
            return await Task.FromResult(new List<Models.Playlist>());
        }

        public Task<Models.Playlist> GetPlaylistAsync(ulong discordUserId, string playlistName)
        {
            return Task.FromResult(new Models.Playlist());
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