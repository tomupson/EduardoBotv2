using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.General.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.General
{
    public class General : EduardoModule
    {
        private readonly GeneralService _service;
        private readonly CommandService _commandService;

        public General(GeneralService service, CommandService commandService)
        {
            _service = service;
            _commandService = commandService;
        }

        [Command("echo")]
        [Summary("Echo a message")]
        [Remarks("I am a bot!")]
        public async Task EchoCommand([Remainder, Summary("The text to echo")] string echo)
        {
            await _service.EchoText(Context, echo);
        }

        [Command("help")]
        [Alias("command")]
        [Summary("View usage information about a command")]
        [Remarks("money donate")]
        public async Task HelpCommand([Remainder] string commandOrModule = null)
        {
            await _service.DisplayHelp(_commandService, Context, commandOrModule);
        }

        [Command("ping")]
        [Summary("Responds \"Pong\"")]
        public async Task PingCommand()
        {
            await _service.Ping(Context);
        }

        [Command("choose")]
        [Summary("Choose a random word from the ones provided")]
        [Remarks("superman batman")]
        public async Task ChooseCommand(params string[] words)
        {
            await _service.Choose(Context, words);
        }

        [Command("lmgtfy")]
        [Summary("Googles something for that special person who is crippled and can't do it themselves")]
        [Remarks("How do I use Eduardo Bot?")]
        public async Task LetMeGoogleThatForYouCommand([Remainder, Summary("What you're searching for")] string searchQuery)
        {
            await _service.GoogleForYou(Context, searchQuery);
        }

        [Command("urban")]
        [Summary("Look up word on Urban Dictionary")]
        [Remarks("Eduardo")]
        public async Task UrbanCommand([Remainder, Summary("The words you want to look up")] string searchQuery)
        {
            await _service.SearchUrbanDictionary(Context, searchQuery);
        }

        [Command("robome")]
        [Summary("Shows what you would look like as a robot")]
        [Remarks("Eduardo")]
        public async Task RoboMeCommand([Remainder, Summary("Your name")] string name)
        {
            await _service.RoboMe(Context, name);
        }
    }
}