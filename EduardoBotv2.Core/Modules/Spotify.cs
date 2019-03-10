using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    [Group("spotify")]
    public class Spotify : ModuleBase<EduardoContext>
    {
        private readonly SpotifyService service;

        public Spotify(SpotifyService service)
        {
            this.service = service;
        }

        [Command("song")]
        [Summary("Search spotify for a song")]
        public async Task SongCommand([Remainder, Summary("The name of the song")] string searchSong)
        {
            await service.GetSong(Context, searchSong);
        }

        [Command("album")]
        [Summary("Search spotify for an artist")]
        public async Task AlbumCommand([Remainder, Summary("The name of the album")] string album)
        {
            await service.GetAlbum(Context, album);
        }

        [Command("artist")]
        [Summary("Search spotify for an artist")]
        public async Task ArtistCommand([Remainder, Summary("The name of the artist")] string artist)
        {
            await service.GetArtist(Context, artist);
        }

        [Command("playlist")]
        [Summary("Search spotify for a playlist")]
        public async Task PlaylistCommand([Remainder, Summary("The name of the playlist")] string playlist)
        {
            await service.GetPlaylist(Context, playlist);
        }
    }
}