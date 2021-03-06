﻿using System;
using Discord;

namespace EduardoBotv2.Core.Modules.Audio.Models
{
    public class SongInfo : SongBase
    {
        public string VideoId { get; set; }

        public string Description { get; set; }

        public TimeSpan CurrentTime { get; set; }

        public TimeSpan Duration { get; set; }

        public string StreamUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public IGuildUser RequestedBy { get; set; }
    }
}
