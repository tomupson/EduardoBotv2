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
        public async Task SongCommand([Remainder, Summary("The name of the song")] string searchSong)
        {
            await _service.GetSong(Context, searchSong);
        }

        [Command("album")]
        [Summary("Search spotify for an artist")]
        public async Task AlbumCommand([Remainder, Summary("The name of the album")] string album)
        {
            await _service.GetAlbum(Context, album);
        }

        [Command("artist")]
        [Summary("Search spotify for an artist")]
        public async Task ArtistCommand([Remainder, Summary("The name of the artist")] string artist)
        {
            await _service.GetArtist(Context, artist);
        }

        [Command("playlist")]
        [Summary("Search spotify for a playlist")]
        public async Task PlaylistCommand([Remainder, Summary("The name of the playlist")] string playlist)
        {
            await _service.GetPlaylist(Context, playlist);
        }
    }
}