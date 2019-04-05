using System;
using System.Collections.Generic;
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

        public static EmbedBuilder WithFieldsForList<T>(this EmbedBuilder builder, IEnumerable<T> lst, Func<T, string> nameSelector, Func<T, object> valueSelector)
        {
            foreach (T item in lst)
            {
                builder.AddField(nameSelector(item), valueSelector(item));
            }

            return builder;
        }
    }
}