using System.Threading.Tasks;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Imgur.Helpers;
using Imgur.API.Models;
using Imgur.API.Models.Impl;

namespace EduardoBotv2.Core.Modules.Imgur.Services
{
    public class ImgurService
    {
        private readonly Credentials credentials;

        public ImgurService(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public async Task SearchImgur(EduardoContext context, string searchQuery = null)
        {
            IGalleryItem img = await ImgurHelper.SearchImgur(credentials.ImgurClientId, credentials.ImgurClientSecret, searchQuery);

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
            IGalleryItem img = await ImgurHelper.SearchImgurSubreddit(credentials.ImgurClientId, credentials.ImgurClientSecret, subredditName);

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