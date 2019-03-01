using System.Text.RegularExpressions;

namespace EduardoBotv2.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Boldify(this object obj) => $"**{obj.ToString().Replace("*", string.Empty).Replace("_", " ").Replace("~", string.Empty).Replace("`", string.Empty)}**";

        public static string UpperFirstChar(this string s)
        {
            if (string.IsNullOrEmpty(s)) return null;

            char[] characters = s.ToLower().ToCharArray();
            characters[0] = char.ToUpper(characters[0]);
            return new string(characters);
        }

        public static string LowerFirstChar(this string s)
        {
            if (string.IsNullOrEmpty(s)) return null;

            char[] characters = s.ToCharArray();
            characters[0] = char.ToLower(characters[0]);
            return new string(characters);
        }

        public static bool Like(this string toSearch, string toFind) => new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\")
            .Replace(toFind, ch => @"\" + ch).Replace('_', '.')
            .Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
    }
}