using EduardoBotv2.Common.Data;
using EduardoBotv2.Common.Utilities.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EduardoBotv2.Services
{
    public class ShortenService
    {
        public async Task ShortenYouTube(EduardoContext c, string url)
        {
            string videoId = string.Empty;
            List<string> alreadyShortened = new List<string>() { "youtu.be", "www.youtu.be" };
            if (alreadyShortened.Any(s => url.Contains(s)) && !url.Contains("&feature=youtu.be"))
            {
                await c.Channel.SendMessageAsync($"**{c.User.Username}, This url is already shortened!**");
                return;
            }

            List<string> validAuthorities = new List<string>() { "youtube.com", "www.youtube.com" };
            if (validAuthorities.Any(s => url.Contains(s)))
            {
                Regex regexExtractId = new Regex(Config.YOUTUBE_LINK_REGEX, RegexOptions.Compiled);
                Uri uri = new Uri(url);
                try
                {
                    string authority = new UriBuilder(uri).Uri.Authority.ToLower();
                    if (validAuthorities.Contains(authority))
                    {
                        var regRes = regexExtractId.Match(uri.ToString());
                        if (regRes.Success)
                        {
                            videoId = regRes.Groups[1].Value;
                        }
                    }

                    await c.Channel.SendMessageAsync($"**{c.User.Mention}, your shortened url is: \"http://youtu.be/{videoId}\"**");
                }
                catch
                {
                    await c.Channel.SendMessageAsync("**Failed to parse url.**");
                }
            } else
            {
                await c.Channel.SendMessageAsync($"**{c.User.Username}, This is not a valid YouTube url.");
            }
        }

        public async Task Shorten(EduardoContext c, string url)
        {
            if (url.Contains("goo.gl"))
            {
                await c.Channel.SendMessageAsync($"**{c.User.Username}, This Url is already shortened**");
                return;
            }

            var shorten = await GoogleHelper.ShortenUrlAsync(c.EduardoSettings.GoogleShortenerApiKey, url);

            await c.Channel.SendMessageAsync($"**{c.User.Mention}, your shortened url is: \"{shorten}\"**");
        }

        public async Task Unshorten(EduardoContext c, string url)
        {
            if (!url.Contains("goo.gl"))
            {
                await c.Channel.SendMessageAsync($"**{c.User.Username}, This Url is not shortened**");
                return;
            }

            var unshortened = await GoogleHelper.UnshortenUrlAsync(c.EduardoSettings.GoogleShortenerApiKey, url);

            await c.Channel.SendMessageAsync($"**{c.User.Mention}, your unshortened url is: \"{unshortened}\"**");
        }
    }
}