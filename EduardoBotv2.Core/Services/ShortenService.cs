using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Services
{
    public class ShortenService
    {
        public async Task ShortenYouTube(EduardoContext context, string url)
        {
            string videoId = string.Empty;
            List<string> alreadyShortened = new List<string>
            {
                "youtu.be", "www.youtu.be"
            };

            if (alreadyShortened.Any(url.Contains) && !url.Contains("&feature=youtu.be"))
            {
                await context.Channel.SendMessageAsync($"**{context.User.Username}, This url is already shortened!**");
                return;
            }

            List<string> validAuthorities = new List<string>
            {
                "youtube.com", "www.youtube.com"
            };

            if (validAuthorities.Any(url.Contains))
            {
                Regex regexExtractId = new Regex(Constants.YOUTUBE_LINK_REGEX, RegexOptions.Compiled);
                Uri uri = new Uri(url);
                try
                {
                    string authority = new UriBuilder(uri).Uri.Authority.ToLower();
                    if (validAuthorities.Contains(authority))
                    {
                        Match regRes = regexExtractId.Match(uri.ToString());
                        if (regRes.Success)
                        {
                            videoId = regRes.Groups[1].Value;
                        }
                    }

                    await context.Channel.SendMessageAsync($"**{context.User.Mention}, your shortened url is: \"http://youtu.be/{videoId}\"**");
                }
                catch
                {
                    await context.Channel.SendMessageAsync("**Failed to parse url.**");
                }
            } else
            {
                await context.Channel.SendMessageAsync($"**{context.User.Username}, This is not a valid YouTube url.");
            }
        }

        public async Task Shorten(EduardoContext context, string url)
        {
            if (url.Contains("goo.gl"))
            {
                await context.Channel.SendMessageAsync($"**{context.User.Username}, This Url is already shortened**");
                return;
            }

            string shorten = await GoogleHelper.ShortenUrlAsync(context.EduardoCredentials.GoogleShortenerApiKey, url);

            await context.Channel.SendMessageAsync($"**{context.User.Mention}, your shortened url is: \"{shorten}\"**");
        }

        public async Task Unshorten(EduardoContext context, string url)
        {
            if (!url.Contains("goo.gl"))
            {
                await context.Channel.SendMessageAsync($"**{context.User.Username}, This Url is not shortened**");
                return;
            }

            string unshortened = await GoogleHelper.UnshortenUrlAsync(context.EduardoCredentials.GoogleShortenerApiKey, url);

            await context.Channel.SendMessageAsync($"**{context.User.Mention}, your unshortened url is: \"{unshortened}\"**");
        }
    }
}