using System.Threading.Tasks;
using Discord.WebSocket;
using EduardoBotv2.Core.Database;

namespace EduardoBotv2.Core.Services
{
    public class GuildHandler
    {
        private readonly IGuildRepository _guildRepository;

        public GuildHandler(DiscordSocketClient client, IGuildRepository guildRepository)
        {
            client.JoinedGuild += OnJoinedGuildAsync;
            client.LeftGuild += OnLeftGuildAsync;

            _guildRepository = guildRepository;
        }

        private async Task OnJoinedGuildAsync(SocketGuild guild) =>
            await _guildRepository.AddGuildAsync((long)guild.Id);

        private async Task OnLeftGuildAsync(SocketGuild guild) =>
            await _guildRepository.RemoveGuildAsync((long)guild.Id);
    }
}