using Discord;
using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    [Group("money"), Alias("bal", "balance")]
    public class Money : ModuleBase<EduardoContext>
    {
        private readonly MoneyService _service;

        public Money(MoneyService service)
        {
            this._service = service;
        }

        [Command(RunMode = RunMode.Async), Name("money")]
        [Summary("Show the balance of a user.")]
        [Remarks("UppyMeister();")]
        public async Task ShowCommand([Summary("The user to get the balance of.")] IGuildUser user = null)
        {
            await _service.ShowMoney(Context, user);
        }

        [Command("set", RunMode = RunMode.Async), Name("money set")]
        [Summary("Set the money of a user.")]
        [Remarks("UppyMeister(); 1629")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetCommand([Summary("The target user")] IGuildUser user, [Summary("The balance to set.")] int amount)
        {
            await _service.SetMoney(Context, user, amount);
        }

        [Command("donate", RunMode = RunMode.Async), Name("money donate"), Alias("append", "give")]
        [Summary("Donate money to a user.")]
        [Remarks("UppyMeister(); 1443")]
        public async Task GiveCommand([Summary("The target user")] IGuildUser user, [Summary("The amount to append")] int amount)
        {
            await _service.DonateMoney(Context, user, amount);
        }
    }
}