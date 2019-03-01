using System.Text.RegularExpressions;

namespace EduardoBotv2.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Boldify(this object obj)
            => $"**{obj.ToString().Replace("*", "").Replace("_", " ").Replace("~", "").Replace("`", "")}**";

        public static string UpperFirstChar(this string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            char[] chars = str.ToLower().ToCharArray();
            chars[0] = char.ToUpper(chars[0]);

            return new string(chars);
        }

        public static string LowerFirstChar(this string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            char[] chars = str.ToCharArray();
            chars[0] = char.ToLower(chars[0]);

            return new string(chars);
        }

        public static bool Like(this string toSearch, string toFind) => new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\")
            .Replace(toFind, ch => @"\" + ch).Replace('_', '.')
            .Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
    }
}