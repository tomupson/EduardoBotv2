using Imgur.API.Models.Impl;
using System.Threading.Tasks;
using EduardoBotv2.Helpers;
using EduardoBotv2.Models;
using Imgur.API.Models;

namespace EduardoBotv2.Services
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
                    case GalleryImage _:
                        await c.Channel.SendMessageAsync(((GalleryImage)img).Link);
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
                    case GalleryAlbum _:
                        await c.Channel.SendMessageAsync(((GalleryAlbum)img).Link);
                        break;
                    case GalleryImage _:
                        await c.Channel.SendMessageAsync(((GalleryImage)img).Link);
                        break;
                }
            } else
            {
                await c.Channel.SendMessageAsync($"**Could not find subreddit with name {subredditName}.**");
            }
        }
    }
}