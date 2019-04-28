using System.Threading.Tasks;
using Discord;
using Discord.Net;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Moderation.Services
{
    public class ModerationService : IEduardoService
    {
        public async Task BanUser(EduardoContext context, IGuildUser banUser, string reason)
        {
            try
            {
                await banUser.SendMessageAsync($"You were banned from {Format.Bold(context.Guild.Name)} with reason: {Format.Bold(reason)}");
            } catch (HttpException ex) when (ex.DiscordCode == 50007) // https://discordapp.com/developers/docs/topics/opcodes-and-status-codes
            {
                // ignored
            }

            await context.Guild.AddBanAsync(banUser, 0, reason);
            await context.Channel.SendMessageAsync($"{Format.Bold(context.User.Mention)} has banned {banUser.Mention}: \"{Format.Bold(reason)}\"");
        }

        public async Task KickUser(EduardoContext context, IGuildUser kickUser, string reason)
        {
            try
            {
                await kickUser.SendMessageAsync($"You were kicked from {Format.Bold(context.Guild.Name)} with reason: {Format.Bold(reason)}");
            } catch (HttpException ex) when (ex.DiscordCode == 50007)
            {
                // ignored
            }

            await kickUser.KickAsync(reason);
            await context.Channel.SendMessageAsync($"{Format.Bold(context.User.Mention)} has kicked {kickUser.Mention}: \"{Format.Bold(reason)}\"");
        }
    }
}