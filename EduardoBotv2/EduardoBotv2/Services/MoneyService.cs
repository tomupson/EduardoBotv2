using Discord;
using EduardoBot.Common.Data;
using EduardoBot.Common.Extensions;
using System.Threading.Tasks;

namespace EduardoBot.Services
{
    public class MoneyService
    {
        private static int money;

        public async Task ShowMoney(EduardoContext c, IGuildUser user)
        {
            if (user == null) await c.Channel.SendMessageAsync(string.Format("${0}", money.ToString())); // Get your own money.
            else await c.Channel.SendMessageAsync(string.Format("${0}", money.ToString())); // Get money for a specific user
        }

        public async Task SetMoney(EduardoContext c, IGuildUser user, int amount)
        {
            money = amount;
            await c.Channel.SendMessageAsync(string.Format("Set the balance of {0} to {1}", user.Mention, amount));
        }

        public async Task DonateMoney(EduardoContext c, IGuildUser user, int amount)
        {
            money += amount;
            await c.Channel.SendMessageAsync($"{c.User.Username.Boldify()} has donated ${amount.Boldify()} to {user.Username.Boldify()}");
        }
    }
}