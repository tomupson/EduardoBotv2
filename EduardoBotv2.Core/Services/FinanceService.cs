using System.Threading.Tasks;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Services
{
    public class FinanceService
    {
        public async Task ConvertCurrency(EduardoContext context, decimal amount, string convertFrom, string convertTo)
        {
            //string url = $"http://www.google.com/finance/converter?a={amount}&from={convertFrom}&to={convertTo}";
            await context.Channel.SendMessageAsync("**NO LONGER SUPPORTED**");
        }
    }
}