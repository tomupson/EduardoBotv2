using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Money.Database;
using EduardoBotv2.Core.Modules.Money.Database.Results;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Money.Services
{
    public class MoneyService : IEduardoService
    {
        private readonly IMoneyRepository _moneyRepository;

        public MoneyService(IMoneyRepository moneyRepository)
        {
            _moneyRepository = moneyRepository;
        }

        public async Task ShowMoney(EduardoContext context, IGuildUser user)
        {
            (string username, long userId) = user == null ?
                (context.User.Username, (long)context.User.Id) :
                (user.Username, (long)user.Id);

            int money = await _moneyRepository.GetMoneyAsync(userId, (long)context.Guild.Id);

            await context.Channel.SendMessageAsync($"{Format.Bold(username)} has ${money}");
        }

        public async Task SetMoney(EduardoContext context, IGuildUser user, int money)
        {
            await _moneyRepository.SetMoneyAsync((long)user.Id, (long)context.Guild.Id, money);
            await context.Channel.SendMessageAsync($"Set the balance of {user.Mention} to ${money}");
        }

        public async Task DonateMoney(EduardoContext context, IGuildUser user, int amount)
        {
            if (context.User.Id == user.Id)
            {
                await context.Channel.SendMessageAsync("Cannot donate money to yourself!");
                return;
            }

            if (amount <= 0)
            {
                await context.Channel.SendMessageAsync("Please enter a valid amount to donate. Value must be positive and greater than 0");
                return;
            }

            DonateMoneyResult result = await _moneyRepository.DonateMoneyAsync((long)user.Id, (long)context.User.Id, (long)context.Guild.Id, amount);

            await context.Channel.SendMessageAsync(result switch
            {
                DonateMoneyResult.NotEnoughMoney => "You do not have enough money to donate to this person",
                DonateMoneyResult.TransactionFailed => "Failed to donate money",
                _ => $"{Format.Bold(context.User.Username)} has donated ${Format.Bold(amount.ToString())} to {Format.Bold(user.Username)}"
            });
        }
    }
}