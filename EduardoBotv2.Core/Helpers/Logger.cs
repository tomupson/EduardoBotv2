using System;
using System.Threading.Tasks;
using Discord;

namespace EduardoBotv2.Core.Helpers
{
    public static class Logger
    {
        public static Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public static Task Log(string message, LogSeverity severity) =>
            Log(new LogMessage(severity, "EduardoBotv2", message));

        public static Task Log(string message, Exception ex, LogSeverity severity) =>
            Log(new LogMessage(severity, "EduardoBotv2", message, ex));
    }
}