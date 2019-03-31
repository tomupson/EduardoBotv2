using System.Threading.Tasks;

namespace EduardoBotv2.Core.Modules.Money.Database
{
    public interface IMoneyRepository
    {
        Task GetMoneyAsync(ulong discordUserId);

        Task AddMoneyAsync(ulong discordUserId, int money);
    }
}