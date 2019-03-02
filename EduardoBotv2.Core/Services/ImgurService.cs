using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using Imgur.API.Models;
using Imgur.API.Models.Impl;

namespace EduardoBotv2.Core.Services
{
    public class ImgurService
    {
        public async Task SearchImgur(EduardoContext context, string searchQuery = null)
        {
            IGalleryItem img = await ImgurHelper.SearchImgur(context.EduardoCredentials.ImgurClientId, context.EduardoCredentials.ImgurClientSecret, searchQuery);

            if (img != null)
            {
                switch (img)
                {
                    case GalleryAlbum album:
                        await context.Channel.SendMessageAsync(album.Link);
                        break;
                    case GalleryImage image:
                        await context.Channel.SendMessageAsync(image.Link);
                        break;
                }
            } else
            {
                await context.Channel.SendMessageAsync($"**Could not find any images that match {searchQuery}**");
            }
        }

        public async Task FetchSubredditImage(EduardoContext context, string subredditName)
        {
            IGalleryItem img = await ImgurHelper.SearchImgurSubreddit(context.EduardoCredentials.ImgurClientId, context.EduardoCredentials.ImgurClientSecret, subredditName);

            if (img != null)
            {
                switch (img)
                {
                    case GalleryAlbum album:
                        await context.Channel.SendMessageAsync(album.Link);
                        break;
                    case GalleryImage image:
                        await context.Channel.SendMessageAsync(image.Link);
                        break;
                }
            } else
            {
                await context.Channel.SendMessageAsync($"**Could not find subreddit with name {subredditName}.**");
            }
        }
    }
}