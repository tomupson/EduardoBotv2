using Imgur.API.Models.Impl;
using EduardoBot.Common.Data;
using EduardoBot.Common.Utilities.Helpers;
using System.Threading.Tasks;

namespace EduardoBot.Services
{
    public class ImgurService
    {
        public async Task SearchImgur(EduardoContext c, string searchQuery = null)
        {
            var img = await ImgurHelper.SearchImgur(c.EduardoSettings.ImgurClientId, c.EduardoSettings.ImgurClientSecret, searchQuery);

            if (img != null)
            {
                if (img is GalleryAlbum)
                {
                    await c.Channel.SendMessageAsync(((GalleryAlbum)img).Link);
                } else if (img is GalleryImage)
                {
                    await c.Channel.SendMessageAsync(((GalleryImage)img).Link);
                }
            } else
            {
                await c.Channel.SendMessageAsync($"**Could not find any images that match {searchQuery}**");
            }
        }

        public async Task FetchSubredditImage(EduardoContext c, string subredditName)
        {
            var img = await ImgurHelper.SearchImgurSubreddit(c.EduardoSettings.ImgurClientId, c.EduardoSettings.ImgurClientSecret, subredditName);

            if (img != null)
            {
                if (img is GalleryAlbum)
                {
                    await c.Channel.SendMessageAsync(((GalleryAlbum)img).Link);
                } else if (img is GalleryImage)
                {
                    await c.Channel.SendMessageAsync(((GalleryImage)img).Link);
                }
            } else
            {
                await c.Channel.SendMessageAsync($"**Could not find subreddit with name {subredditName}.**");
            }
        }
    }
}