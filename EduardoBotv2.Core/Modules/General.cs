using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class General : ModuleBase<EduardoContext>
    {
        private readonly GeneralService service;
        private readonly CommandService commandService;

        public General(GeneralService service, CommandService commandService)
        {
            this.service = service;
            this.commandService = commandService;
        }

        [Command("echo", RunMode = RunMode.Async), Alias("talk")]
        [Summary("Echo a message.")]
        [Remarks("I am a bot!")]
        public async Task EchoCommand([Remainder, Summary("The text to echo")] string echo)
        {
            await service.EchoText(Context, echo);
        }

        [Command("help", RunMode = RunMode.Async), Alias("command", "cmd")]
        [Summary("View Command Information.")]
        [Remarks("money donate")]
        public async Task HelpCommand([Remainder] string commandOrModule = null)
        {
            await service.DisplayHelp(commandService, Context, commandOrModule);
        }

        [Command("ping", RunMode = RunMode.Async), Alias("p")]
        [Summary("Responds 'Pong'")]
        [Remarks("")]
        public async Task PingCommand()
        {
            await service.Ping(Context);
        }

        [Command("choose", RunMode = RunMode.Async)]
        [Summary("Choose a random word from the ones provided.")]
        [Remarks("superman batman")]
        public async Task ChooseCommand(params string[] words)
        {
            await service.Choose(Context, words);
        }

        [Command("lmgtfy", RunMode = RunMode.Async)]
        [Summary("Googles something for that special person who is crippled and can't do it themselves.")]
        [Remarks("How do I use Eduardo Bot.")]
        public async Task LetMeGoogleThatForYouCommand([Remainder, Summary("What you're searching for.")] string searchQuery)
        {
            await service.GoogleForYou(Context, searchQuery);
        }

        [Command("urban", RunMode = RunMode.Async)]
        [Summary("Look up word on Urban Dictionary.")]
        [Remarks("Eduardo")]
        public async Task UrbanCommand([Remainder, Summary("Words you want to look up on Urban Dictionary.")] string searchQuery)
        {
            await service.SearchUrbanDictionary(Context, searchQuery);
        }

        [Command("robome", RunMode = RunMode.Async)]
        [Summary("Shows what you would look like as a robot!")]
        [Remarks("Eduardo")]
        public async Task RoboMeCommand([Remainder, Summary("Your name.")] string name)
        {
            await service.RoboMe(Context, name);
        }
    }
}