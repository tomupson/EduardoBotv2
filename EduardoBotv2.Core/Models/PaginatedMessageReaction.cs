using System;
using System.Threading;
using Discord;

namespace EduardoBotv2.Core.Models
{
    public class PaginatedMessageReaction
    {
        public Action<Embed, CancellationTokenSource> ReactionChangedCallback { get; set; }

        public IEmote Emote { get; set; }

        public PaginatedMessageReaction(IEmote emote, Action<Embed, CancellationTokenSource> callback)
        {
            Emote = emote;
            ReactionChangedCallback = callback;
        }
    }
}