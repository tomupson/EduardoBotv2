using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Audio.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Audio
{
    [Group("playlist")]
    public class Playlist : EduardoModule
    {
        private readonly PlaylistService _service;

        public Playlist(PlaylistService service)
        {
            _service = service;
        }

        [Command("create")]
        [Remarks("My Favourite Songs")]
        public async Task CreatePlaylistCommand([Summary("The name of the playlist")] string playlistName)
        {
            await _service.CreatePlaylistAsync(Context, playlistName);
        }
    }
}
