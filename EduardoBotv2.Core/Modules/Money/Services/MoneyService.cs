using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Modules.Money.Services
{
    public class MoneyService
    {
        private static int money;

        public async Task ShowMoney(EduardoContext context, IGuildUser user)
        {
            if (user == null)
            {
                await context.Channel.SendMessageAsync($"${money}"); // Get your own money.
            } else
            {
                await context.Channel.SendMessageAsync($"${money}"); // Get money for a specific user
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