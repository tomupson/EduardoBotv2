using EduardoBotv2.Core.Modules.Audio.Models;

namespace EduardoBotv2.Core.Modules.Audio.Extensions
{
    public static class SongExtensions
    {
        public static Song ToPlaylistSong(this SongInfo song)
        {
            return new Song();
        }
    }
}