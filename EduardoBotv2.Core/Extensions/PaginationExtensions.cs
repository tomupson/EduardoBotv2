using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Models.Enums;

namespace EduardoBotv2.Core.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task SendMessageOrPaginatedAsync(this EduardoContext context, List<Embed> embeds)
        {
            if (embeds.Count == 1)
            {
                await context.Channel.SendMessageAsync(embed: embeds[0]);
            } else
            {
                await context.SendPaginatedMessageAsync(new PaginatedMessage
                {
                    Embeds = embeds,
                    Timeout = TimeSpan.FromSeconds(Constants.PAGINATION_TIMEOUT_SECONDS)
                });
            }
        }

        public static async Task SendPaginatedMessageAsync(this EduardoContext context, PaginatedMessage paginatedMessage)
        {
            if (paginatedMessage.Embeds.Count == 0)
            {
                throw new ArgumentException("No pages provided");
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            CancellationTokenSource cts = new CancellationTokenSource(paginatedMessage.Timeout);
            cts.Token.Register(() => tcs.TrySetResult(true));

            RestUserMessage message = await context.Channel.SendMessageAsync(embed: paginatedMessage.Embeds[paginatedMessage.CurrentIndex]);
            await AddPaginationReactionsAsync(message);

            ulong botUserId = context.Client.CurrentUser.Id;

            async Task ProcessReactionAsync(Cacheable<IUserMessage, ulong> cachedReactionMessage, ISocketMessageChannel channel, SocketReaction reaction)
            {
                IUserMessage reactionMessage = await cachedReactionMessage.GetOrDownloadAsync();

                if (reactionMessage.Id == message.Id && reaction.User.Value.Id != botUserId)
                {
                    ProcessPaginationReaction(paginatedMessage, reaction.Emote, cts);
                    await ChangePage(paginatedMessage, message, cts);
                }
            }

            context.Client.ReactionAdded += ProcessReactionAsync;
            context.Client.ReactionRemoved += ProcessReactionAsync;

            await tcs.Task;

            switch(paginatedMessage.TimeoutBehaviour)
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

        private static void ProcessPaginationReaction(PaginatedMessage paginatedMessage, IEmote emoji, CancellationTokenSource cts)
        {
            switch (emoji.Name)
            {
                case "◀":
                    paginatedMessage.CurrentIndex = paginatedMessage.CurrentIndex - 1 >= 0 ? paginatedMessage.CurrentIndex - 1 : paginatedMessage.Embeds.Count - 1;
                    cts.CancelAfter(paginatedMessage.Timeout);
                    break;
                case "❌":
                    cts.Cancel();
                    break;
                case "▶":
                    paginatedMessage.CurrentIndex = paginatedMessage.CurrentIndex + 1 <= paginatedMessage.Embeds.Count - 1 ? paginatedMessage.CurrentIndex + 1 : 0;
                    cts.CancelAfter(paginatedMessage.Timeout);
                    break;
            }
        }

        private static async Task ChangePage(PaginatedMessage paginatedMessage, IUserMessage message, CancellationTokenSource cts)
        {
            if (!cts.IsCancellationRequested && paginatedMessage.CurrentIndex != paginatedMessage.PreviousIndex)
            {
                await message.ModifyAsync(n =>
                {
                    n.Embed = paginatedMessage.Embeds[paginatedMessage.CurrentIndex];
                });

                paginatedMessage.PreviousIndex = paginatedMessage.CurrentIndex;
            }
        }
    }
}