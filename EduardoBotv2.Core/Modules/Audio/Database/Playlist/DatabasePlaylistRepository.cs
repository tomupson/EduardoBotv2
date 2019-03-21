using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Database.Playlist
{
    public class DatabasePlaylistRepository : IPlaylistRepository
    {
        public async Task<List<Models.Playlist>> GetPlaylist(long discordUserId) => throw new NotImplementedException();

        public async Task AddSongToPlaylist(long playlistId, Song song) => throw new NotImplementedException();
    }
}