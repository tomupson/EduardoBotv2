using System;
using System.Threading.Tasks;

namespace EduardoBotv2.Core.Modules.Money.Database
{
    public class DatabaseMoneyRepository : IMoneyRepository
    {
        public Task GetMoneyAsync(ulong discordUserId) => throw new NotImplementedException();

        public Task AddMoneyAsync(ulong discordUserId, int money) => throw new NotImplementedException();
    }
}