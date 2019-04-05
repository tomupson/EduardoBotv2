using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Task SendPaginatedMessageAsync(this EduardoContext context, PaginatedMessage paginatedMsg)
        {
            if (paginatedMsg.Embeds.Count == 0)
            {
                throw new ArgumentException("No pages provided");
            }

            if (paginatedMsg.TimeoutBehaviour == TimeoutBehaviour.Delete)
            {
                paginatedMsg.Text += $" This message will automatically delete in {paginatedMsg.Timeout.TotalSeconds} seconds";
            }

            _ = Task.Run(async () =>
            {
                RestUserMessage message = await context.Channel.SendMessageAsync(paginatedMsg.Text, embed: paginatedMsg.Embeds[paginatedMsg.CurrentIndex]);
                await AddPaginationReactionsAsync(message, paginatedMsg);

                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                CancellationTokenSource cts = new CancellationTokenSource(paginatedMsg.Timeout);
                cts.Token.Register(() => tcs.TrySetResult(true));

                bool customReactionChosen = false;

                async Task ProcessReactionAsync(Cacheable<IUserMessage, ulong> cachedReactionMessage, ISocketMessageChannel channel, SocketReaction reaction)
                {
                    if (cachedReactionMessage.Id == message.Id)
                    {
                        if (paginatedMsg.Reactions.Select(r => r.Emote).Contains(reaction.Emote))
                        {
                            paginatedMsg.Reactions.First(x => x.Emote.Name == reaction.Emote.Name)
                                .ReactionChangedCallback(paginatedMsg.Embeds[paginatedMsg.CurrentIndex], cts);
                            customReactionChosen = true;
                        } else
                        {
                            ProcessPaginationReaction(paginatedMsg, reaction.Emote, cts);
                        }

                        await ChangePageIfRequired(paginatedMsg, message, cts);
                    }
                }

                context.Client.ReactionAdded += ProcessReactionAsync;
                context.Client.ReactionRemoved += ProcessReactionAsync;

                await tcs.Task;

                context.Client.ReactionAdded -= ProcessReactionAsync;
                context.Client.ReactionRemoved -= ProcessReactionAsync;

                switch (paginatedMsg.TimeoutBehaviour)
                {
                    case TimeoutBehaviour.Default:
                    case TimeoutBehaviour.Ignore:
                        await message.RemoveAllReactionsAsync();
                        break;
                    case TimeoutBehaviour.Delete:
                        await message.DeleteAsync();
                        break;
                }

                if (!customReactionChosen)
                {
                    paginatedMsg.TimeoutCallback?.Invoke();
                }
            });

            return Task.CompletedTask;
        }

        private static async Task AddPaginationReactionsAsync(IUserMessage message, PaginatedMessage paginatedMsg)
        {
            await message.AddReactionAsync(new Emoji("◀")); // :arrow_backward:
            await message.AddReactionAsync(new Emoji("❌")); // :x:

            foreach (PaginatedMessageReaction customReaction in paginatedMsg.Reactions)
            {
                await message.AddReactionAsync(customReaction.Emote);
            }

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

        private static async Task ChangePageIfRequired(PaginatedMessage paginatedMsg, IUserMessage message, CancellationTokenSource cts)
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