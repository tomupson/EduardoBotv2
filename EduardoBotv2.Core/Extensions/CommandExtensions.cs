using System.Text;
using Discord;
using Discord.Commands;

namespace EduardoBotv2.Core.Extensions
{
    public static class CommandExtensions
    {
        public static string GetUsage(this CommandInfo cmd)
        {
            StringBuilder sb = new StringBuilder();
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

                sb.Append($" {before}{param.Name}{after}");
            }

            return sb.ToString();
        }
    }
}