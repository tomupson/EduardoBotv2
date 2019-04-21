using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Money.Database.Results;

namespace EduardoBotv2.Core.Modules.Money.Database
{
    public interface IMoneyRepository
    {
        Task<int> GetMoneyAsync(long discordUserId, long guildId);

        Task SetMoneyAsync(long discordGuildId, long guildId, int money);

        Task<DonateMoneyResult> DonateMoneyAsync(long donorDiscordUserId, long doneeDiscordUserId, long guildId, int amount);
    }
}