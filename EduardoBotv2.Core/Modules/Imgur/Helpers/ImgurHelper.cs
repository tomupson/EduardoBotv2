using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;

namespace EduardoBotv2.Core.Modules.Imgur.Helpers
{
    public static class ImgurHelper
    {
        public static GalleryEndpoint CreateImgurClientAndEndpoint(string clientId, string clientSecret) => new GalleryEndpoint(new ImgurClient(clientId, clientSecret));

        public static async Task<IGalleryItem> SearchImgur(string clientId, string clientSecret, string searchQuery)
        {
            GalleryEndpoint endpoint = CreateImgurClientAndEndpoint(clientId, clientSecret);

            return string.IsNullOrWhiteSpace(searchQuery) ?
                GetRandomGalleryItem(await endpoint.GetRandomGalleryAsync()) :
                GetRandomGalleryItem(await endpoint.SearchGalleryAsync(searchQuery));
        }

        public static async Task<IGalleryItem> SearchImgurSubreddit(string clientId, string clientSecret, string subredditName)
        {
            GalleryEndpoint endpoint = CreateImgurClientAndEndpoint(clientId, clientSecret);

            return GetRandomGalleryItem(await endpoint.GetSubredditGalleryAsync(subredditName));
        }

        private static IGalleryItem GetRandomGalleryItem(IEnumerable<IGalleryItem> gallery)
        {
            List<IGalleryItem> galleryItems = gallery.ToList();

            return galleryItems.Any() ? galleryItems[new Random().Next(0, galleryItems.Count)] : null;
        }
    }
}