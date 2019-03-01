using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class Finance : ModuleBase<EduardoContext>
    {
        private readonly FinanceService service;

        public Finance(FinanceService service)
        {
            this.service = service;
        }

        [Command("convert", RunMode = RunMode.Async)]
        [Summary("Convert one currency to another")]
        public async Task ConvertCommand([Summary("Amount to convert.")] decimal amount, [Summary("Currency to convert from.")] string convertFrom, [Summary("Currency to convert to.")] string convertTo)
        {
            await service.ConvertCurrency(Context, amount, convertFrom, convertTo);
        }
    }
}