using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist;
using EduardoBotv2.Core.Modules.Audio.Services;
using EduardoBotv2.Core.Modules.Draw.Services;
using EduardoBotv2.Core.Modules.Games.Database;
using EduardoBotv2.Core.Modules.Games.Services;
using EduardoBotv2.Core.Modules.General.Services;
using EduardoBotv2.Core.Modules.Imgur.Services;
using EduardoBotv2.Core.Modules.Memes.Services;
using EduardoBotv2.Core.Modules.Moderation.Services;
using EduardoBotv2.Core.Modules.Money.Services;
using EduardoBotv2.Core.Modules.News.Services;
using EduardoBotv2.Core.Modules.PUBG.Services;
using EduardoBotv2.Core.Modules.Shorten.Services;
using EduardoBotv2.Core.Modules.Spotify.Services;
using EduardoBotv2.Core.Modules.User.Services;
using EduardoBotv2.Core.Modules.Utility.Services;
using EduardoBotv2.Core.Modules.YouTube.Services;
using EduardoBotv2.Core.Services;
using Pubg.Net;
using RedditSharp;
using SpotifyAPI.Web;

namespace EduardoBotv2.Core
{
    public class EduardoBot : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly Models.Credentials _credentials;

        public EduardoBot()
        {
            _credentials = new Models.Credentials();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Verbose
            });

            _client.Log += Logger.Log;
        }

        public async Task RunAsync()
        {
            IServiceCollection services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_credentials)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
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
                .AddSingleton<IPokemonRepository, DatabasePokemonRepository>()
                .AddSingleton<IPlaylistRepository, DatabasePlaylistRepository>();

            services.AddSingleton(new Reddit(new RefreshTokenWebAgent(_credentials.RedditRefreshToken, _credentials.RedditClientId, _credentials.RedditClientSecret, _credentials.RedditRedirectUri)));

            PubgApiConfiguration.Configure(config =>
            {
                config.ApiKey = _credentials.PUBGApiKey;
            });

            IServiceProvider provider = services.BuildServiceProvider();

            await provider.GetRequiredService<CommandHandler>().InitializeAsync(provider);

            try
            {
                await _client.LoginAsync(TokenType.Bot, _credentials.Token);
            } catch (HttpException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo", $"Failed to log in: {e.Message}"));
                Console.ReadLine();
                Environment.Exit(0);
            }
            
            await _client.StartAsync();

            await _client.SetGameAsync("chat", "", ActivityType.Listening);
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);

            await Task.Delay(-1);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}