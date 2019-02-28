using Discord;
using Discord.Commands;

namespace EduardoBotv2.Extensions
{
    public static class CommandExtensions
    {
        public static string GetUsage(this CommandInfo cmd)
        {
            string usage = string.Empty;
            foreach (ParameterInfo param in cmd.Parameters)
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
    }
}