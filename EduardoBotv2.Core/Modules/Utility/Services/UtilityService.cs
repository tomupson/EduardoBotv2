using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;
using Format = Discord.Format;

namespace EduardoBotv2.Core.Modules.Utility.Services
{
    public class UtilityService : IEduardoService
    {
        public async Task CleanMessages(EduardoContext context, int count)
        {
            if (count <= 0) return;

            IEnumerable<IMessage> messagesToDelete = await context.Channel.GetMessagesAsync(count + 1).FlattenAsync();
            await ((ITextChannel) context.Channel).DeleteMessagesAsync(messagesToDelete);
            string plural = count > 1 ? "s" : "";
            RestUserMessage finishedMessage = await context.Channel.SendMessageAsync($"Successfully cleared {count} message{plural} :ok_hand:");
            await Task.Delay(3000);
            await finishedMessage.DeleteAsync();
        }

        public async Task DisplayInvite(EduardoContext context)
        {
            await context.Channel.SendMessageAsync($"{Format.Bold(context.User.Mention)}, you can invite me to your server with this link!:\n{Format.EscapeUrl("https://discordapp.com/oauth2/authorize?client_id=488384686826192916&scope=bot&permissions=8")}");
        }
    }
}