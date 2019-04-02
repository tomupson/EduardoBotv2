using System.Collections.Generic;

namespace EduardoBotv2.Core.Modules.Audio.Models
{
    public class Playlist
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();
    }
}