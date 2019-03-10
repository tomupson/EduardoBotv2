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

        [Command("artist")]
        [Summary("Search spotify for an artist")]
        public async Task ArtistCommand([Remainder, Summary("The name of the artist")] string artist)
        {
            await service.GetArtist(Context, artist);
        }
    }
}