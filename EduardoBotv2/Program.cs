using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using EduardoBotv2.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using EduardoBotv2.Helpers;
using EduardoBotv2.Models;

/*
 * Eduardo Bot
 * Created by Thomas Upson
 */

namespace EduardoBotv2
{
    public class Program
    {
        private static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        private readonly DiscordSocketClient client;
        private readonly Settings settings;

        public Program()
        {
            try
            {
                using (StreamReader file = File.OpenText($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EduardoBotConfig\settings.json"))
                {
                    settings = (Settings) new JsonSerializer().Deserialize(file, typeof(Settings));
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

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Info
            });

            client.Log += Logger.Log;
        }

        public async Task MainAsync()
        {
            IServiceProvider services = InitializeServices();
            await services.GetRequiredService<CommandHandler>().InitializeAsync(services);

            try
            {
                await client.LoginAsync(TokenType.Bot, settings.Token);
            } catch (HttpException e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "Eduardo Bot", "Failed to log in: " + e.Message));
                Console.ReadLine();
                Environment.Exit(0);
            }
            
            await client.StartAsync();
            await client.SetGameAsync($"Running on {client.Guilds.Count} server" + (client.Guilds.Count > 1 ? "s" : "")); // Change this to change the bot's activity (playing / streaming / listening)
            await client.SetStatusAsync(UserStatus.DoNotDisturb); // Change this to change status (DND, Away e.t.c)

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
            .AddSingleton(settings)
            .BuildServiceProvider();
    }
}