using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EduardoBotv2.Core.Database;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist.Results;
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
            AsyncDataReader dr = new AsyncDataReader("PLAYLIST_GetPlaylists", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);

            List<Models.Playlist> playlists = new List<Models.Playlist>();
            await dr.ExecuteReaderAsync(reader =>
            {
                playlists.Add(GetPlaylistFromReader(reader));

                return Task.CompletedTask;
            });

            return playlists;
        }

        public async Task<Models.Playlist> GetPlaylistAsync(long discordUserId, string playlistName)
        {
            AsyncDataReader dr = new AsyncDataReader("PLAYLIST_GetPlaylist", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@PlaylistName", playlistName);

            Models.Playlist playlist = null;
            await dr.ExecuteReaderAsync(reader =>
            {
                GetPlaylistWithSongsFromReader(reader, ref playlist);

                return Task.CompletedTask;
            });

            return playlist;
        }

        public async Task<AddSongResult> AddSongToPlaylistAsync(long discordUserId, string playlistName, PlaylistSong song)
        {
            AsyncDataReader dr = new AsyncDataReader("PLAYLIST_AddSong", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@PlaylistName", playlistName);
            dr.AddParameter("@SongName", song.Name);
            dr.AddParameter("@SongUrl", song.Url);

            AddSongResult result = (AddSongResult)await dr.ExecuteScalarAsync();

            return result;
        }

        public async Task<RemoveSongResult> RemoveSongFromPlaylistAsync(long discordUserId, string playlistName, string songName)
        {
            AsyncDataReader dr = new AsyncDataReader("PLAYLIST_RemoveSong", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@PlaylistName", playlistName);
            dr.AddParameter("@SongName", songName);

            RemoveSongResult result = (RemoveSongResult)await dr.ExecuteScalarAsync();

            return result;
        }

        public async Task<RemoveSongResult> RemoveSongFromPlaylistByIndexAsync(long discordUserId, string playlistName, int index)
        {
            AsyncDataReader dr = new AsyncDataReader("PLAYLIST_RemoveSongByIndex", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@PlaylistName", playlistName);
            dr.AddParameter("@Index", index);

            RemoveSongResult result = (RemoveSongResult)await dr.ExecuteScalarAsync();

            return result;
        }

        public async Task<CreatePlaylistResult> CreatePlaylistAsync(long discordUserId, string playlistName)
        {
            AsyncDataReader dr = new AsyncDataReader("PLAYLIST_CreatePlaylist", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@PlaylistName", playlistName);

            CreatePlaylistResult result = (CreatePlaylistResult)await dr.ExecuteScalarAsync();

            return result;
        }

        public async Task<DeletePlaylistResult> DeletePlaylistAsync(long discordUserId, string playlistName)
        {
            AsyncDataReader dr = new AsyncDataReader("PLAYLIST_DeletePlaylist", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@PlaylistName", playlistName);

            DeletePlaylistResult result = (DeletePlaylistResult)await dr.ExecuteScalarAsync();

            return result;
        }

        private static Models.Playlist GetPlaylistFromReader(IDataReader reader) => new Models.Playlist
        {
            Id = reader.GetInt64(reader.GetOrdinal("PLAYLIST_ID")),
            Name = reader.GetString(reader.GetOrdinal("PLAYLIST_NAME"))
        };

        private static void GetPlaylistWithSongsFromReader(IDataReader reader, ref Models.Playlist playlist)
        {
            if (playlist == null)
            {
                playlist = GetPlaylistFromReader(reader);
            }

            if (reader.IsDBNull(reader.GetOrdinal("SONG_ID"))) return;

            playlist.Songs.Add(GetSongFromReader(reader));
        }

        private static PlaylistSong GetSongFromReader(IDataReader reader) => new PlaylistSong
        {
            Name = reader.GetString(reader.GetOrdinal("SONG_NAME")),
            Id = reader.GetInt64(reader.GetOrdinal("SONG_ID")),
            Url = reader.GetString(reader.GetOrdinal("SONG_URL"))
        };
    }
}