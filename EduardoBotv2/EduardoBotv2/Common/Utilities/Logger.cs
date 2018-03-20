using Discord;
using System;
using System.Threading.Tasks;

namespace EduardoBot.Common.Utilities
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