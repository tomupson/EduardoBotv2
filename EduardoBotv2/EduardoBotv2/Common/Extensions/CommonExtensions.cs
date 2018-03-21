using Discord;
using Discord.Commands;
using System;

namespace EduardoBotv2.Common.Extensions
{
    public static class CommonExtensions
    {
        public static string GetUsage(this CommandInfo cmd)
        {
            string usage = string.Empty;
            foreach (var param in cmd.Parameters)
            {
                string before = "<";
                string after = ">";
                if (param.IsOptional)
                {
                    before = "[";
                    after = "]";
                }
                if (param.Type == typeof(IRole) || param.Type == typeof(IGuildUser))
                {
                    before += "@";
                }

                if (param.Type == typeof(ITextChannel))
                {
                    before += "#";
                }

                usage += $" {before}{param.Name}{after}";
            }
            return usage;
        }

        public static string ToDurationString(this TimeSpan ts)
        {
            return $"{(Math.Truncate(ts.TotalMinutes).ToString("00"))}:{(ts.Seconds.ToString("00"))}";
        }
    }
}