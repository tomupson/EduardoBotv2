using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using Imgur.API.Models;
using Imgur.API.Models.Impl;

namespace EduardoBotv2.Core.Services
{
    public class ImgurService
    {
        public async Task SearchImgur(EduardoContext c, string searchQuery = null)
        {
            IGalleryItem img = await ImgurHelper.SearchImgur(c.EduardoSettings.ImgurClientId, c.EduardoSettings.ImgurClientSecret, searchQuery);

            if (img != null)
            {
                switch (img)
                {
                    case GalleryAlbum album:
                        await c.Channel.SendMessageAsync(album.Link);
                        break;
                    case GalleryImage image:
                        await c.Channel.SendMessageAsync(image.Link);
                        break;
                }
            } else
            {
                await c.Channel.SendMessageAsync($"**Could not find any images that match {searchQuery}**");
            }
        }

        public async Task FetchSubredditImage(EduardoContext c, string subredditName)
        {
            IGalleryItem img = await ImgurHelper.SearchImgurSubreddit(c.EduardoSettings.ImgurClientId, c.EduardoSettings.ImgurClientSecret, subredditName);

            if (img != null)
            {
                switch (img)
                {
                    case GalleryAlbum album:
                        await c.Channel.SendMessageAsync(album.Link);
                        break;
                    case GalleryImage image:
                        await c.Channel.SendMessageAsync(image.Link);
                        break;
                }
            } else
            {
                await c.Channel.SendMessageAsync($"**Could not find subreddit with name {subredditName}.**");
            }
        }
    }
}