using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Services;
using Octokit;
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
                .AddSingleton<FinanceService>()
                .AddSingleton<GamesService>()
                .AddSingleton<GeneralService>()
                .AddSingleton<ImgurService>()
                .AddSingleton<ModerationService>()
                .AddSingleton<MoneyService>()
                .AddSingleton<NewsService>()
                .AddSingleton<PUBGService>()
                .AddSingleton<ShortenService>()
                .AddSingleton<UserService>()
                .AddSingleton<UtilityService>()
                .AddSingleton<YouTubeModuleService>()
                .AddSingleton<SpotifyWebAPI>()
                .AddSingleton<SpotifyService>();

            services.AddSingleton(new Reddit(new RefreshTokenWebAgent(credentials.RedditRefreshToken, credentials.RedditClientId, credentials.RedditClientSecret, credentials.RedditRedirectUri)));

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