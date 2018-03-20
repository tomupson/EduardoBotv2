using Imgur.API.Models;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Authentication.Impl;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduardoBot.Common.Utilities.Helpers
{
    public static class ImgurHelper
    {
        public static GalleryEndpoint CreateImgurClientAndEndpoint(string clientId, string clientSecret)
        {
            var imgur =  new ImgurClient(clientId, clientSecret);
            return new GalleryEndpoint(imgur);
        }

        public static async Task<IGalleryItem> SearchImgur(string clientId, string clientSecret, string searchQuery)
        {
            return await Task.Run(() =>
            {
                var endpoint = CreateImgurClientAndEndpoint(clientId, clientSecret);

                List<IGalleryItem> gallery;
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    gallery = (endpoint.GetRandomGalleryAsync()).Result.ToList();
                }
                else
                {
                    gallery = (endpoint.SearchGalleryAsync(searchQuery)).Result.ToList();
                }

                var img = gallery.Any() ? gallery[new Random().Next(0, gallery.Count)] : null;

                return img;
            });
        }

        public static async Task<IGalleryItem> SearchImgurSubreddit(string clientId, string clientSecret, string subredditName)
        {
            return await Task.Run(() =>
            {
                var endpoint = CreateImgurClientAndEndpoint(clientId, clientSecret);
                var gallery = (endpoint.GetSubredditGalleryAsync(subredditName)).Result.ToList();

                var img = gallery.Any() ? gallery[new Random().Next(0, gallery.Count)] : null;

                return img;
            });
        }
    }
}