﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using EduardoBotv2.Core.Database;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist;
using EduardoBotv2.Core.Modules.Audio.Services;
using EduardoBotv2.Core.Modules.Draw.Services;
using EduardoBotv2.Core.Modules.Games.Database.Pokemon;
using EduardoBotv2.Core.Modules.Games.Services;
using EduardoBotv2.Core.Modules.General.Services;
using EduardoBotv2.Core.Modules.Help.Services;
using EduardoBotv2.Core.Modules.Imgur.Services;
using EduardoBotv2.Core.Modules.Memes.Services;
using EduardoBotv2.Core.Modules.Moderation.Services;
using EduardoBotv2.Core.Modules.Money.Database;
using EduardoBotv2.Core.Modules.Money.Services;
using EduardoBotv2.Core.Modules.News.Services;
using EduardoBotv2.Core.Modules.PUBG.Services;
using EduardoBotv2.Core.Modules.Shorten.Services;
using EduardoBotv2.Core.Modules.Spotify.Services;
using EduardoBotv2.Core.Modules.User.Services;
using EduardoBotv2.Core.Modules.Utility.Services;
using EduardoBotv2.Core.Modules.YouTube.Services;
using EduardoBotv2.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Pubg.Net;
using RedditSharp;
using SpotifyAPI.Web;

namespace EduardoBotv2.Core
{
    public class EduardoBot : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly Models.Credentials _credentials;
        private readonly CancellationToken _appCancellationToken;

        public EduardoBot(CancellationToken cancelToken = default)
        {
            _credentials = new Models.Credentials();
            _appCancellationToken = cancelToken;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Verbose
            });

            _client.Log += Logger.Log;
            _client.Ready += ClientReady;
        }

        public async Task RunAsync()
        {
            IServiceCollection services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_credentials)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<GuildHandler>()
                .AddSingleton<AudioService>()
                .AddSingleton<MemesService>()
                .AddSingleton<DrawService>()
                .AddSingleton<GamesService>()
                .AddSingleton<GeneralService>()
                .AddSingleton<ImgurService>()
                .AddSingleton<ModerationService>()
                .AddSingleton<MoneyService>()
                .AddSingleton<NewsService>()
                .AddSingleton<PubgService>()
                .AddSingleton<ShortenService>()
                .AddSingleton<UserService>()
                .AddSingleton<UtilityService>()
                .AddSingleton<YouTubeService>()
                .AddSingleton<SpotifyWebAPI>()
                .AddSingleton<SpotifyService>()
                .AddSingleton<PlaylistService>()
                .AddSingleton<PokemonService>()
                .AddSingleton<HelpService>()
                .AddSingleton<IPokemonRepository, DatabasePokemonRepository>()
                .AddSingleton<IPlaylistRepository, DatabasePlaylistRepository>()
                .AddSingleton<IMoneyRepository, DatabaseMoneyRepository>()
                .AddSingleton<IGuildRepository, DatabaseGuildRepository>();

            services.AddSingleton(new Reddit(new RefreshTokenWebAgent(
                _credentials.RedditRefreshToken,
                _credentials.RedditClientId,
                _credentials.RedditClientSecret,
                _credentials.RedditRedirectUri)));

            PubgApiConfiguration.Configure(config => config.ApiKey = _credentials.PUBGApiKey);

            IServiceProvider provider = services.BuildServiceProvider();

            await provider.GetRequiredService<CommandHandler>().InitializeAsync(provider);

            try
            {
                await _client.LoginAsync(TokenType.Bot, _credentials.Token);
            }
            catch (HttpException e)
            {
                await Logger.Log("Failed to log in", e, LogSeverity.Critical);
                Console.ReadLine();
                Environment.Exit(0);
            }

            await _client.StartAsync();

            await _client.SetStatusAsync(UserStatus.Online);

            await Task.Delay(-1, _appCancellationToken);
        }

        private async Task ClientReady()
        {
            await _client.SetGameAsync($"chat in {_client.Guilds.Count} servers.", "", ActivityType.Listening);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}