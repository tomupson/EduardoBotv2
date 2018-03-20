using Discord;
using EduardoBot.Common.Data;
using EduardoBot.Common.Data.Enums;
using EduardoBot.Common.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EduardoBot.Common.Extensions
{
    public static class PaginationExtension
    {
        public static async Task SendPaginatedMessageAsync(this EduardoContext c, PaginatedMessage pm)
        {
            if (pm.Embeds.Count == 0)
            {
                throw new ArgumentException("No pages provided");
            }

            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            var ct = new CancellationTokenSource(pm.Timeout); // Cancellation token automatically activates after timeout.
            ct.Token.Register(() => tcs.TrySetResult(null)); // Once it has been cancelled (token activates), it will set tcs to null, which will trigger await tcs.Task.

            var m = await c.Channel.SendMessageAsync("", false, pm.Embeds[pm.CurrentIndex]);
            await m.AddPaginationReactionsAsync();

            c.Client.ReactionAdded += async (e, channel, reaction) =>
            {
                var message = e.GetOrDownloadAsync().Result;

                // If the id of the message matches the id of the message we made...
                // ...and the id of the user is the same one as who posted the message.
                if (message.Id == m.Id && reaction.User.Value.Id != c.Client.CurrentUser.Id)
                {
                    pm.ProcessPaginationReaction(reaction.Emote, ct);
                }

                await Task.CompletedTask;
            };

            c.Client.ReactionRemoved += async (e, channel, reaction) =>
            {
                var message = e.GetOrDownloadAsync().Result;

                // If the id of the message matches the id of the message we made...
                // ...and the id of the user is the same one as who posted the message.
                if (message.Id == m.Id && reaction.User.Value.Id != c.Client.CurrentUser.Id)
                {
                    pm.ProcessPaginationReaction(reaction.Emote, ct);
                }

                await Task.CompletedTask;
            };

            TimerCallback callback = async x =>
            {
                var msg = x as IUserMessage;
                if (!ct.IsCancellationRequested && pm.CurrentIndex != pm.PreviousIndex && msg != null)
                {
                    await msg.ModifyAsync(n =>
                    {
                        n.Embed = pm.Embeds[pm.CurrentIndex];
                    });

                    pm.PreviousIndex = pm.CurrentIndex;
                }
            };

            var timer = new Timer(callback, m, 500, 1000);
            await tcs.Task;

            switch(pm.TimeoutBehaviour)
            {
                case TimeoutBehaviour.Default:
                case TimeoutBehaviour.Ignore:
                    await m.RemoveAllReactionsAsync();
                    break;
                case TimeoutBehaviour.Delete:
                    await m.DeleteAsync();
                    break;
            }

            timer.Dispose();
        }

        public static async Task AddPaginationReactionsAsync(this IUserMessage m)
        {
            int delay = 1300; // Delay as to not reach the reaction limit.
            await m.AddReactionAsync(new Emoji("◀")); // :arrow_backward:
            await Task.Delay(delay);
            await m.AddReactionAsync(new Emoji("❌")); // :x:
            await Task.Delay(delay);
            await m.AddReactionAsync(new Emoji("▶")); // :arrow_forward:
            await Task.CompletedTask;
        }

        public static void ProcessPaginationReaction(this PaginatedMessage pm, IEmote emoji, CancellationTokenSource ct)
        {
            switch(emoji.Name)
            {
                case "◀":
                    pm.CurrentIndex = Math.Max(pm.CurrentIndex - 1, 0);
                    ct.CancelAfter(pm.Timeout);
                    break;
                case "❌":
                    ct.Cancel();
                    break;
                case "▶":
                    pm.CurrentIndex = Math.Min(pm.CurrentIndex + 1, pm.Embeds.Count - 1);
                    ct.CancelAfter(pm.Timeout);
                    break;
            }
        }
    }
}