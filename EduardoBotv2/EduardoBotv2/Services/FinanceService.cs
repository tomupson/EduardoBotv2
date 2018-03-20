using EduardoBot.Common.Data;
using System.Threading.Tasks;

namespace EduardoBot.Services
{
    public class FinanceService
    {
        public async Task ConvertCurrency(EduardoContext c, decimal amount, string convertFrom, string convertTo)
        {
            string url = $"http://www.google.com/finance/converter?a={amount}&from={convertFrom}&to={convertTo}";
            await c.Channel.SendMessageAsync("WIP");
        }
    }
}