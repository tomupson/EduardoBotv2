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

            [Command("create")]
            [Remarks("My Favourite Songs")]
            public async Task CreatePlaylistCommand([Summary("The name of the playlist")] string playlistName)
            {
                await _service.CreatePlaylistAsync(Context, playlistName);
            }
        }
    }
}