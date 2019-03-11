using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
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

        private readonly int[] acessTokenErrorStatusCodes = { 400, 401 };
        
        public SpotifyService(SpotifyWebAPI spotify, Credentials credentials)
        {
            this.spotify = spotify;
            this.credentials = credentials;
        }

        public async Task GetSong(EduardoContext context, string searchSong)
        {
            SearchItem searchItem = await GetAsync(() => spotify.SearchItemsAsync(searchSong, SearchType.Track));

            if (searchItem.Tracks.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No songs found from search term \"{searchSong}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (FullTrack track in searchItem.Tracks.Items)
            {
                embeds.Add(new EmbedBuilder
                {

                }.Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        public async Task GetAlbum(EduardoContext context, string searchAlbum)
        {
            SearchItem searchItem = await GetAsync(() => spotify.SearchItemsAsync(searchAlbum, SearchType.Album));

            if (searchItem.Albums.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No albums found from search term \"{searchAlbum}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (SimpleAlbum album in searchItem.Albums.Items)
            {
                embeds.Add(new EmbedBuilder
                {
                    Title = album.Name,
                    Color = Color.DarkGreen,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Album Type",
                            Value = album.AlbumType
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Released",
                            Value = album.ReleaseDate
                        }
                    },
                    ImageUrl = album.Images.Count > 0 ? album.Images[0].Url : "",
                    Url = album.ExternalUrls["spotify"]
                }.Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        public async Task GetArtist(EduardoContext context, string searchArtist)
        {
            SearchItem searchItem = await GetAsync(() => spotify.SearchItemsAsync(searchArtist, SearchType.Artist));

            if (searchItem.Artists.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No artists found from search term \"{searchArtist}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (FullArtist artist in searchItem.Artists.Items)
            {
                embeds.Add(new EmbedBuilder
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
                    Url = artist.ExternalUrls["spotify"]
                }.Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        public async Task GetPlaylist(EduardoContext context, string searchPlaylist)
        {
            SearchItem searchItem = await GetAsync(() => spotify.SearchItemsAsync(searchPlaylist, SearchType.Playlist));

            if (searchItem.Playlists.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No playlists found from search term \"{searchPlaylist}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (SimplePlaylist playlist in searchItem.Playlists.Items)
            {
                embeds.Add(new EmbedBuilder
                {

                }.Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
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