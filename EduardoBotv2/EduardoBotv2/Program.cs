using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using EduardoBot.Services;
using EduardoBot.Common.Utilities;
using EduardoBot.Common.Data.Models;
using System;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Eduardo Bot created by Thomas Upson.
/// </summary>

namespace EduardoBot
{
    public class Program
    {
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private readonly DiscordSocketClient _client;
        private readonly Settings _settings;

        public Program()
        {
            try
            {
                using (StreamReader file = File.OpenText($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EduardoBotConfig\settings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    _settings = (Settings)serializer.Deserialize(file, typeof(Settings));
                }
            }
            catch
            {
                Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", "Error loading file 'settings.json'.\n" +
                    "Make sure your documents contains a folder called \"EduardoBotConfig\" with that contains the settings file.\n" +
                    "Please fix this issue and restart the bot.".Replace("\n", Environment.NewLine)));
                Console.ReadLine();
                Environment.Exit(0);
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Info
            });

            _client.Log += Logger.Log;
        }

        public async Task MainAsync()
        {
            var services = InitializeServices();
            await services.GetRequiredService<CommandHandler>().InitializeAsync(services);

            try
            {
                await _client.LoginAsync(TokenType.Bot, _settings.Token);
            } catch (HttpException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", "Failed to log in: " + e.Message));
                Console.ReadLine();
                Environment.Exit(0);
            }
            
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider InitializeServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                // Services
                .AddSingleton(new AudioService())
                .AddSingleton(new GamesService())
                .AddSingleton(new GeneralService())
                .AddSingleton(new ImgurService())
                .AddSingleton(new ModerationService())
                .AddSingleton(new MoneyService())
                .AddSingleton(new NewsService())
                .AddSingleton(new ShortenService())
                .AddSingleton(new UserService())
                .AddSingleton(new UtilityService())
                .AddSingleton(new YouTubeModuleService())
                .AddSingleton(_settings)
                .BuildServiceProvider();
        }
    }
}