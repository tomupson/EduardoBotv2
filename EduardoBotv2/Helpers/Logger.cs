using System;
using System.Threading.Tasks;
using Discord;

namespace EduardoBotv2.Helpers
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