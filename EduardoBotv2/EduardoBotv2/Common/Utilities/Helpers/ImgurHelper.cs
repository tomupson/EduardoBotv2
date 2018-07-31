using Imgur.API.Models;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Authentication.Impl;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduardoBotv2.Common.Utilities.Helpers
{
    public static class ImgurHelper
    {
        public static GalleryEndpoint CreateImgurClientAndEndpoint(string clientId, string clientSecret) => new GalleryEndpoint(new ImgurClient(clientId, clientSecret));

        public static async Task<IGalleryItem> SearchImgur(string clientId, string clientSecret, string searchQuery) => await Task.Run(() =>
        {
            GalleryEndpoint endpoint = CreateImgurClientAndEndpoint(clientId, clientSecret);

            List<IGalleryItem> gallery = string.IsNullOrWhiteSpace(searchQuery) ?
                endpoint.GetRandomGalleryAsync().Result.ToList() :
                endpoint.SearchGalleryAsync(searchQuery).Result.ToList();

            IGalleryItem img = gallery.Any() ? gallery[new Random().Next(0, gallery.Count)] : null;

            return img;
        });

        public static async Task<IGalleryItem> SearchImgurSubreddit(string clientId, string clientSecret, string subredditName) => await Task.Run(() =>
        {
            GalleryEndpoint endpoint = CreateImgurClientAndEndpoint(clientId, clientSecret);
            List<IGalleryItem> gallery = endpoint.GetSubredditGalleryAsync(subredditName).Result.ToList();

            IGalleryItem img = gallery.Any() ? gallery[new Random().Next(0, gallery.Count)] : null;

            return img;
        });
    }
}