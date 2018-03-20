using Discord.Commands;
using EduardoBot.Common.Data;
using EduardoBot.Services;
using System.Threading.Tasks;

namespace EduardoBot.Modules
{
    public class Finance : ModuleBase<EduardoContext>
    {
        private readonly FinanceService _service;

        public Finance(FinanceService service)
        {
            this._service = service;
        }

        [Command("convert", RunMode = RunMode.Async)]
        [Summary("Convert one currency to another")]
        public async Task ConvertCommand([Summary("Amount to convert.")] decimal amount, [Summary("Currency to convert from.")] string convertFrom, [Summary("Currency to convert to.")] string convertTo)
        {
            await _service.ConvertCurrency(Context, amount, convertFrom, convertTo);
        }
    }
}