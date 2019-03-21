using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
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
using Octokit;
using Pubg.Net;
using RedditSharp;
using SpotifyAPI.Web;

namespace EduardoBotv2.Core
{
    public class EduardoBot
    {
        private readonly DiscordSocketClient client;
        private readonly Models.Credentials credentials;

        public EduardoBot()
        {
            credentials = new Models.Credentials();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Verbose
            });

            client.Log += Logger.Log;
        }

        public async Task RunAsync()
        {
            IServiceCollection services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(credentials)
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
                .AddSingleton<YouTubeModuleService>()
                .AddSingleton<SpotifyWebAPI>()
                .AddSingleton<SpotifyService>()
                .AddSingleton<IPokemonRepository, DatabasePokemonRepository>();

            services.AddSingleton(new Reddit(new RefreshTokenWebAgent(credentials.RedditRefreshToken, credentials.RedditClientId, credentials.RedditClientSecret, credentials.RedditRedirectUri)));

            PubgApiConfiguration.Configure(config =>
            {
                config.ApiKey = credentials.PUBGApiKey;
            });

            IServiceProvider provider = services.BuildServiceProvider();

            await provider.GetRequiredService<CommandHandler>().InitializeAsync(provider);

            try
            {
                await client.LoginAsync(TokenType.Bot, credentials.Token);
            } catch (HttpException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo", $"Failed to log in: {e.Message}"));
                Console.ReadLine();
                Environment.Exit(0);
            }
            
            await client.StartAsync();

            await client.SetGameAsync("chat", "", ActivityType.Listening);
            await client.SetStatusAsync(UserStatus.DoNotDisturb);

            await Task.Delay(-1);
        }
    }
}