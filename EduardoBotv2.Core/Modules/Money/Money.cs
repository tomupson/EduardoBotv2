using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Money.Services;
using EduardoBotv2.Core.Preconditions;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Money
{
    [Group("money")]
    [Name("Money")]
    [Alias("bal", "balance")]
    public class Money : EduardoModule<MoneyService>
    {
        public Money(MoneyService service)
            : base(service) { }

        [Command("")]
        [Summary("Show the balance of a user")]
        [Remarks("uppy")]
        public async Task ShowCommand([Summary("The user to get the balance of")] IGuildUser user = null)
        {
            await _service.ShowMoney(Context, user);
        }

        [Command("set")]
        [Summary("Set the money of a user")]
        [Remarks("uppy 1629")]
        //[RequireRole("Banker")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetCommand([Summary("The target user")] IGuildUser user, [Summary("The balance to set")] int amount)
        {
            await _service.SetMoney(Context, user, amount);
        }

        [Command("donate")]
        [Alias("give")]
        [Summary("Donate money to a user")]
        [Remarks("uppy 1443")]
        public async Task GiveCommand([Summary("The target user")] IGuildUser user, [Summary("The amount to append")] int amount)
        {
            await _service.DonateMoney(Context, user, amount);
        }
    }
}