using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Modules.Utility.Services
{
    public class UtilityService
    {
        public async Task CleanMessages(EduardoContext context, uint count)
        {
            if (count <= 0) return;

            IEnumerable<IMessage> messagesToDelete = await context.Channel.GetMessagesAsync((int)count + 1).FlattenAsync();
            await ((ITextChannel) context.Channel).DeleteMessagesAsync(messagesToDelete);
            string plural = count > 1 ? "s" : "";
            RestUserMessage finishedMessage = await context.Channel.SendMessageAsync($"Successfully cleared {count} message{plural} :ok_hand:");
            await Task.Delay(3000);
            await finishedMessage.DeleteAsync();
        }

        public async Task DisplayInvite(EduardoContext context)
        {
            await context.Channel.SendMessageAsync($"{context.User.Mention.Boldify()}, you can invite me to your server with this link!:\nhttps://discordapp.com/oauth2/authorize?client_id=360500527869460480&scope=bot&permissions=8");
        }
    }
}