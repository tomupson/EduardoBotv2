using System.Threading.Tasks;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Imgur.Helpers;
using EduardoBotv2.Core.Services;
using Imgur.API.Models;
using Imgur.API.Models.Impl;

namespace EduardoBotv2.Core.Modules.Imgur.Services
{
    public class ImgurService : IEduardoService
    {
        private readonly Credentials _credentials;

        public ImgurService(Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task SearchImgur(EduardoContext context, string searchQuery = null)
        {
            IGalleryItem img = await ImgurHelper.SearchImgur(_credentials.ImgurClientId, _credentials.ImgurClientSecret, searchQuery);

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
            IGalleryItem img = await ImgurHelper.SearchImgurSubreddit(_credentials.ImgurClientId, _credentials.ImgurClientSecret, subredditName);

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