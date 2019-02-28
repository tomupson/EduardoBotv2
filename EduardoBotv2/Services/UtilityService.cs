using System.Collections.Generic;
using Discord;
using System.Threading.Tasks;
using Discord.Rest;
using EduardoBotv2.Extensions;
using EduardoBotv2.Models;

namespace EduardoBotv2.Services
{
    public class UtilityService
    {
        public async Task CleanMessages(EduardoContext c, uint count)
        {
            if (count <= 0) return;

            IEnumerable<IMessage> messagesToDelete = await c.Channel.GetMessagesAsync((int)count + 1).FlattenAsync();
            await ((ITextChannel) c.Channel).DeleteMessagesAsync(messagesToDelete);
            string plural = count > 1 ? "s" : "";
            RestUserMessage finishedMessage = await c.Channel.SendMessageAsync($"Successfully cleared {count} message{plural} :ok_hand:");
            await Task.Delay(3000);
            await finishedMessage.DeleteAsync();
        }

        public async Task DisplayInvite(EduardoContext c)
        {
            await c.Channel.SendMessageAsync($"{c.User.Mention.Boldify()}, you can invite me to your server with this link!:\nhttps://discordapp.com/oauth2/authorize?client_id=360500527869460480&scope=bot&permissions=8");
        }
    }
}