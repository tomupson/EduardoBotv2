using Discord;
using EduardoBotv2.Common.Data.Enums;
using System;
using System.Collections.Generic;

namespace EduardoBotv2.Common.Data.Models
{
    public class PaginatedMessage
    {
        public int CurrentIndex { get; set; }
        public int PreviousIndex { get; set; }
        public List<Embed> Embeds { get; set; }
        public TimeSpan Timeout { get; set; }
        public TimeoutBehaviour TimeoutBehaviour { get; set; }
    }
}