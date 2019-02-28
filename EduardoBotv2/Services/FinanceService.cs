﻿using System.Threading.Tasks;
using EduardoBotv2.Models;

namespace EduardoBotv2.Services
{
    public class FinanceService
    {
        public async Task ConvertCurrency(EduardoContext c, decimal amount, string convertFrom, string convertTo)
        {
            string url = $"http://www.google.com/finance/converter?a={amount}&from={convertFrom}&to={convertTo}";
            await c.Channel.SendMessageAsync("**NO LONGER SUPPORTED**");
        }
    }
}