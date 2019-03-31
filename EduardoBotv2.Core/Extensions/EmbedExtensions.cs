using Discord;

namespace EduardoBotv2.Core.Extensions
{
    public static class EmbedExtensions
    {
        public static EmbedBuilder AddConditionalField(this EmbedBuilder builder, string name, object value, bool condition, bool inline = false)
        {
            if (condition)
            {
                builder.AddField(name, value, inline);
            }

            return builder;
        }
    }
}