using Discord;
using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Extensions;
using System.Threading.Tasks;

namespace EduardoBotv2.Services
{
    public class UtilityService
    {
        public async Task CleanMessages(EduardoContext c, uint count)
        {
            if (count <= 0) return;
            else
            {
                var messagesToDelete = await c.Channel.GetMessagesAsync((int)count + 1).FlattenAsync();
                await (c.Channel as ITextChannel).DeleteMessagesAsync(messagesToDelete);
                string plural = count > 1 ? "s" : "";
                var finishedMessage = await c.Channel.SendMessageAsync($"Successfully cleared {count} message{plural} :ok_hand:");
                await Task.Delay(3000);
                await finishedMessage.DeleteAsync();
            }
        }

        public async Task DisplayInvite(EduardoContext c)
        {
            await c.Channel.SendMessageAsync($"{c.User.Mention.Boldify()}, you can invite me to your server with this link!:\nhttps://discordapp.com/oauth2/authorize?client_id=360500527869460480&scope=bot&permissions=8");
        }
    }
}