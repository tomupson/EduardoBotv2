using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Spotify.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Spotify
{
    [Group("spotify")]
    [Name("Spotify")]
    public class Spotify : EduardoModule<SpotifyService>
    {
        public Spotify(SpotifyService service)
            : base(service) { }

        [Command("song")]
        [Summary("Search spotify for a song")]
        public async Task SongCommand([Summary("The name of the song"), Remainder] string searchSong)
        {
            await _service.GetSongAsync(Context, searchSong);
        }

        [Command("album")]
        [Summary("Search spotify for an artist")]
        public async Task AlbumCommand([Summary("The name of the album"), Remainder] string album)
        {
            await _service.GetAlbumAsync(Context, album);
        }

        [Command("artist")]
        [Summary("Search spotify for an artist")]
        public async Task ArtistCommand([Summary("The name of the artist"), Remainder] string artist)
        {
            await _service.GetArtistAsync(Context, artist);
        }

        [Command("playlist")]
        [Summary("Search spotify for a playlist")]
        public async Task PlaylistCommand([Summary("The name of the playlist"), Remainder] string playlist)
        {
            await _service.GetPlaylist(Context, playlist);
        }
    }
}