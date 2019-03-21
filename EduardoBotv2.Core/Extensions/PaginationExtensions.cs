using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task SendMessageOrPaginatedAsync(this EduardoContext context, List<Embed> embeds)
        {
            PaginatedMessage paginatedMsg = PaginatedMessage.Default;
            paginatedMsg.Embeds = embeds;
            await SendMessageOrPaginatedAsync(context, paginatedMsg);
        }

        public static async Task SendMessageOrPaginatedAsync(this EduardoContext context, PaginatedMessage paginatedMsg)
        {
            if (paginatedMsg.Embeds.Count == 1)
            {
                await context.Channel.SendMessageAsync(embed: paginatedMsg.Embeds[0]);
            } else
            {
                await context.SendPaginatedMessageAsync(paginatedMsg);
            }
        }

        public static async Task SendPaginatedMessageAsync(this EduardoContext context, PaginatedMessage paginatedMsg)
        {
            if (paginatedMsg.Embeds.Count == 0)
            {
                throw new ArgumentException("No pages provided");
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            CancellationTokenSource cts = new CancellationTokenSource(paginatedMsg.Timeout);
            cts.Token.Register(() => tcs.TrySetResult(true));

            string text = "";
            if (paginatedMsg.TimeoutBehaviour == TimeoutBehaviour.Delete)
            {
                text = $"This message will automatically delete in {paginatedMsg.Timeout.TotalSeconds} seconds";
            }

            RestUserMessage message = await context.Channel.SendMessageAsync(text, embed: paginatedMsg.Embeds[paginatedMsg.CurrentIndex]);
            await AddPaginationReactionsAsync(message);

            ulong botUserId = context.Client.CurrentUser.Id;

            async Task ProcessReactionAsync(Cacheable<IUserMessage, ulong> cachedReactionMessage, ISocketMessageChannel channel, SocketReaction reaction)
            {
                IUserMessage reactionMessage = await cachedReactionMessage.GetOrDownloadAsync();

                if (reactionMessage.Id == message.Id && reaction.User.Value.Id != botUserId)
                {
                    ProcessPaginationReaction(paginatedMsg, reaction.Emote, cts);
                    await ChangePage(paginatedMsg, message, cts);
                }
            }

            context.Client.ReactionAdded += ProcessReactionAsync;
            context.Client.ReactionRemoved += ProcessReactionAsync;

            await tcs.Task;

            switch(paginatedMsg.TimeoutBehaviour)
            {
                case TimeoutBehaviour.Default:
                case TimeoutBehaviour.Ignore:
                    await message.RemoveAllReactionsAsync();
                    break;
                case TimeoutBehaviour.Delete:
                    await message.DeleteAsync();
                    break;
            }
        }

        private static async Task AddPaginationReactionsAsync(IUserMessage message)
        {
            const int delay = 1300; // Delay as to not reach the reaction limit.
            await message.AddReactionAsync(new Emoji("◀")); // :arrow_backward:
            await Task.Delay(delay);
            await message.AddReactionAsync(new Emoji("❌")); // :x:
            await Task.Delay(delay);
            await message.AddReactionAsync(new Emoji("▶")); // :arrow_forward:
        }

        private static void ProcessPaginationReaction(PaginatedMessage paginatedMsg, IEmote emoji, CancellationTokenSource cts)
        {
            switch (emoji.Name)
            {
                case "◀":
                    paginatedMsg.CurrentIndex = paginatedMsg.CurrentIndex - 1 >= 0 ? paginatedMsg.CurrentIndex - 1 : paginatedMsg.Embeds.Count - 1;
                    cts.CancelAfter(paginatedMsg.Timeout);
                    break;
                case "❌":
                    cts.Cancel();
                    break;
                case "▶":
                    paginatedMsg.CurrentIndex = paginatedMsg.CurrentIndex + 1 <= paginatedMsg.Embeds.Count - 1 ? paginatedMsg.CurrentIndex + 1 : 0;
                    cts.CancelAfter(paginatedMsg.Timeout);
                    break;
            }
        }

        private static async Task ChangePage(PaginatedMessage paginatedMsg, IUserMessage message, CancellationTokenSource cts)
        {
            if (!cts.IsCancellationRequested && paginatedMsg.CurrentIndex != paginatedMsg.PreviousIndex)
            {
                await message.ModifyAsync(n =>
                {
                    n.Embed = paginatedMsg.Embeds[paginatedMsg.CurrentIndex];
                });

                paginatedMsg.PreviousIndex = paginatedMsg.CurrentIndex;
            }
        }
    }
}