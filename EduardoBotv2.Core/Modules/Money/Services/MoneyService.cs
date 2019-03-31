using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Money.Database;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Money.Services
{
    public class MoneyService : IEduardoService
    {
        private static int money;

        private readonly IMoneyRepository _moneyRepository;

        public MoneyService(IMoneyRepository moneyRepository)
        {
            _moneyRepository = moneyRepository;
        }

        public async Task ShowMoney(EduardoContext context, IGuildUser user)
        {
            if (user == null)
            {
                //await _moneyRepository.GetMoneyAsync(context.User.Id);
                await context.Channel.SendMessageAsync($"${money}");
            } else
            {
                //await _moneyRepository.GetMoneyAsync(user.Id);
                await context.Channel.SendMessageAsync($"${money}");
            }
        }

        public async Task SetMoney(EduardoContext context, IGuildUser user, int amount)
        {
            money = amount;
            await context.Channel.SendMessageAsync($"Set the balance of {user.Mention} to {amount}");
        }

        public async Task DonateMoney(EduardoContext context, IGuildUser user, int amount)
        {
            money += amount;
            await context.Channel.SendMessageAsync($"{context.User.Username.Boldify()} has donated ${amount.Boldify()} to {user.Username.Boldify()}");
        }
    }
}