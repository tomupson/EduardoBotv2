using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Audio.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Audio
{
    partial class Audio
    {
        [Group("playlist")]
        [Name("Playlist")]
        public class Playlist : EduardoModule<PlaylistService>
        {
            public Playlist(PlaylistService service)
                : base(service) { }

            [Command("")]
            [Summary("View a playlist")]
            [Remarks("\"My Favourite Songs\"")]
            public async Task ViewPlaylistCommand([Summary("The name of the playlist")] string playlistName)
            {
                await _service.ViewPlaylistAsync(Context, playlistName);
            }

            [Command("create")]
            [Remarks("\"My Favourite Songs\"")]
            public async Task CreatePlaylistCommand([Summary("The name of the playlist to create")] string playlistName)
            {
                await _service.CreatePlaylistAsync(Context, playlistName);
            }

            [Command("delete")]
            [Remarks("\"My Favourite Songs\"")]
            public async Task DeletePlaylistCommand([Summary("The name of the playlist to delete")] string playlistName)
            {
                await _service.DeletePlaylistAsync(Context, playlistName);
            }

            [Command("add", RunMode = RunMode.Async)]
            [Remarks("\"My Favourite Songs\" \"racecar\"")]
            public async Task AddSongCommand([Summary("The name of the playlist")] string playlistName,
                [Summary("The name or url of the song to add")] string query)
            {
                await _service.AddSongToPlaylistAsync(Context, playlistName, query);
            }

            [Command("remove")]
            [Remarks("\"My Favourite Songs\" \"racecar\"")]
            public async Task RemoveSongCommand([Summary("The name of the playlist")] string playlistName,
                [Summary("The name of the song to remove")] string songName)
            {
                await _service.RemoveSongFromPlaylistAsync(Context, playlistName, songName);
            }

            [Command("remove")]
            [Remarks("\"My Favourite Songs\" 1")]
            public async Task RemoveSongByIndexCommand([Summary("The name of the playlist")] string playlistName,
                [Summary("The item to remove")] int index)
            {
                await _service.RemoveSongFromPlaylistByIndexAsync(Context, playlistName, index);
            }
        }
    }
}