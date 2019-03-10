using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace EduardoBotv2.Core.Services
{
    public class SpotifyService
    {
        private readonly Credentials credentials;
        private readonly SpotifyWebAPI spotify;

        private int[] acessTokenErrorStatusCodes = {400, 401};
        
        public SpotifyService(SpotifyWebAPI spotify, Credentials credentials)
        {
            this.spotify = spotify;
            this.credentials = credentials;
        }

        public async Task GetArtist(EduardoContext context, string searchArtist)
        {
            SearchItem searchItem = await GetAsync(() => spotify.SearchItemsAsync(searchArtist, SearchType.Artist));
            if (searchItem.Artists.Total > 0)
            {
                FullArtist artist = searchItem.Artists.Items[0];
                await context.Channel.SendMessageAsync(embed: new EmbedBuilder
                {
                    Title = artist.Name,
                    Color = Color.DarkGreen,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Followers",
                            Value = artist.Followers.Total
                        }
                    },
                    ImageUrl = artist.Images.Count > 0 ? artist.Images[0].Url : "",
                    Url = artist.ExternalUrls["spotify"],
                }.Build());
            } else
            {
                await context.Channel.SendMessageAsync($"No artists found from search term \"{searchArtist}\"");
            }
        }

        private async Task<T> GetAsync<T>(Func<Task<T>> func) where T : BasicModel
        {
            T result = await func();
            if (result.HasError() && acessTokenErrorStatusCodes.Contains(result.Error.Status))
            {
                AuthorizationCodeAuth auth = new AuthorizationCodeAuth(credentials.SpotifyClientId, credentials.SpotifyClientSecret, credentials.SpotifyRedirectUri, "");
                Token token = await auth.RefreshToken(credentials.SpotifyRefreshToken);
                spotify.AccessToken = token.AccessToken;
                spotify.TokenType = token.TokenType;
            }

            return await func();
        }

        private T Get<T>(Func<T> func) where T : BasicModel
        {
            T result = func();
            if (result.HasError() && acessTokenErrorStatusCodes.Contains(result.Error.Status))
            {
                AuthorizationCodeAuth auth = new AuthorizationCodeAuth(credentials.SpotifyClientId, credentials.SpotifyClientSecret, credentials.SpotifyRedirectUri, "");
                Token token = auth.RefreshToken(credentials.SpotifyRefreshToken).Result;
                spotify.AccessToken = token.AccessToken;
                spotify.TokenType = token.TokenType;
            }

            return func();
        }
    }
}