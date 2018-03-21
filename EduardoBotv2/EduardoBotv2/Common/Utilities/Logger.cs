using Discord;
using System;
using System.Threading.Tasks;

namespace EduardoBotv2.Common.Utilities
{
    public static class Logger
    {
        public static Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}