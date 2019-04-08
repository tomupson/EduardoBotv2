using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Extensions;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace EduardoBotv2.Core.Modules.Spotify.Services
{
    public class SpotifyService : IEduardoService
    {
        private readonly Credentials _credentials;
        private readonly SpotifyWebAPI _spotify;

        private readonly int[] _accessTokenErrorStatusCodes = { 400, 401 };
        
        public SpotifyService(SpotifyWebAPI spotify, Credentials credentials)
        {
            _spotify = spotify;
            _credentials = credentials;
        }

        public async Task GetSongAsync(EduardoContext context, string searchSong)
        {
            SearchItem searchItem = await GetAsync(() => _spotify.SearchItemsAsync(searchSong, SearchType.Track));

            if (searchItem.Tracks.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No songs found from search term \"{searchSong}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (FullTrack track in searchItem.Tracks.Items)
            {
                embeds.Add(new EmbedBuilder()
                    .WithTitle(track.Name)
                    .WithColor(Color.DarkGreen)
                    .WithUrl(track.ExternUrls["spotify"])
                    .WithImageUrl(track.Album.Images.Count > 0 ? track.Album.Images[0].Url : "")
                    .AddField("Duration", TimeSpan.FromMilliseconds(track.DurationMs).ToString(@"mm\:ss"))
                    .AddField("Album", $"{track.Album.Name} - {track.Album.AlbumType}")
                    .AddField("Released", track.Album.ReleaseDate)
                    .AddField("Explcit", track.Explicit ? "Yes": "No")
                    .Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        public async Task GetAlbumAsync(EduardoContext context, string searchAlbum)
        {
            SearchItem searchItem = await GetAsync(() => _spotify.SearchItemsAsync(searchAlbum, SearchType.Album));

            if (searchItem.Albums.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No albums found from search term \"{searchAlbum}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (SimpleAlbum album in searchItem.Albums.Items)
            {
                embeds.Add(new EmbedBuilder()
                    .WithTitle(album.Name)
                    .WithColor(Color.DarkGreen)
                    .WithUrl(album.ExternalUrls["spotify"])
                    .WithImageUrl(album.Images.Count > 0 ? album.Images[0].Url : "")
                    .AddField("Album Type", album.AlbumType)
                    .AddField("Released", album.ReleaseDate)
                    .Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        public async Task GetArtistAsync(EduardoContext context, string searchArtist)
        {
            SearchItem searchItem = await GetAsync(() => _spotify.SearchItemsAsync(searchArtist, SearchType.Artist));

            if (searchItem.Artists.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No artists found from search term \"{searchArtist}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (FullArtist artist in searchItem.Artists.Items)
            {
                embeds.Add(new EmbedBuilder()
                    .WithTitle(artist.Name)
                    .WithColor(Color.DarkGreen)
                    .WithUrl(artist.ExternalUrls["spotify"])
                    .WithImageUrl(artist.Images.Count > 0 ? artist.Images[0].Url : "")
                    .AddField("Followers", artist.Followers.Total)
                    .Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        public async Task GetPlaylist(EduardoContext context, string searchPlaylist)
        {
            SearchItem searchItem = await GetAsync(() => _spotify.SearchItemsAsync(searchPlaylist, SearchType.Playlist));

            if (searchItem.Playlists.Total == 0)
            {
                await context.Channel.SendMessageAsync($"No playlists found from search term \"{searchPlaylist}\"");
            }

            List<Embed> embeds = new List<Embed>();

            foreach (SimplePlaylist playlist in searchItem.Playlists.Items)
            {
                embeds.Add(new EmbedBuilder()
                    .Build());
            }

            await context.SendMessageOrPaginatedAsync(embeds);
        }

        private async Task<T> GetAsync<T>(Func<Task<T>> func) where T : BasicModel
        {
            T result = await func();
            if (result.HasError() && _accessTokenErrorStatusCodes.Contains(result.Error.Status))
            {
                AuthorizationCodeAuth auth = new AuthorizationCodeAuth(_credentials.SpotifyClientId, _credentials.SpotifyClientSecret, _credentials.SpotifyRedirectUri, "");
                Token token = await auth.RefreshToken(_credentials.SpotifyRefreshToken);
                _spotify.AccessToken = token.AccessToken;
                _spotify.TokenType = token.TokenType;
            }

            return await func();
        }
    }
}