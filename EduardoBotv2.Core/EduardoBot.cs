using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core
{
    public class EduardoBot
    {
        private readonly DiscordSocketClient client;
        private readonly Credentials credentials;

        public EduardoBot()
        {
            credentials = new Credentials();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Info
            });

            client.Log += Logger.Log;
        }

        public async Task RunAsync()
        {
            IServiceProvider services = InitializeServices();
            await services.GetRequiredService<CommandHandler>().InitializeAsync(services);

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

        private IServiceProvider InitializeServices() => new ServiceCollection()
            // Base
            .AddSingleton(client)
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandler>()
            // Services
            .AddSingleton(new AudioService())
            .AddSingleton(new FinanceService())
            .AddSingleton(new FortniteService())
            .AddSingleton(new GamesService())
            .AddSingleton(new GeneralService())
            .AddSingleton(new ImgurService())
            .AddSingleton(new ModerationService())
            .AddSingleton(new MoneyService())
            .AddSingleton(new NewsService())
            .AddSingleton(new PUBGService())
            .AddSingleton(new ShortenService())
            .AddSingleton(new UserService())
            .AddSingleton(new UtilityService())
            .AddSingleton(new YouTubeModuleService())
            .AddSingleton(credentials)
            .BuildServiceProvider();
    }
}