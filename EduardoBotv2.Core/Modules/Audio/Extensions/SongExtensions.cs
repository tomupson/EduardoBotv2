using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Extensions
{
    public static class SongExtensions
    {
        public static PlaylistSong ToPlaylistSong(this Song song)
        {
            return new PlaylistSong();
        }
    }
}