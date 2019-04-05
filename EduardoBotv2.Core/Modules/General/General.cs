using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.General.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.General
{
    public class General : EduardoModule<GeneralService>
    {
        private readonly CommandService _commandService;

        public General(GeneralService service, CommandService commandService)
            : base(service)
        {
            _commandService = commandService;
        }

        [Command("echo")]
        [Summary("Echo a message")]
        [Remarks("I am a bot!")]
        public async Task EchoCommand([Summary("The text to echo"), Remainder] string echo)
        {
            await _service.EchoTextAsync(Context, echo);
        }

        [Command("help")]
        [Alias("command")]
        [Summary("View usage information about a command")]
        [Remarks("money donate")]
        public async Task HelpCommand([Remainder] string commandOrModule = null)
        {
            await _service.DisplayHelpAsync(_commandService, Context, commandOrModule);
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
        public async Task LetMeGoogleThatForYouCommand([Summary("What you're searching for"), Remainder] string searchQuery)
        {
            await _service.GoogleForYou(Context, searchQuery);
        }

        [Command("urban")]
        [Summary("Look up word on Urban Dictionary")]
        [Remarks("Eduardo")]
        public async Task UrbanCommand([Summary("The words you want to look up"), Remainder] string searchQuery)
        {
            await _service.SearchUrbanDictionary(Context, searchQuery);
        }

        [Command("robome")]
        [Summary("Shows what you would look like as a robot")]
        [Remarks("Eduardo")]
        public async Task RoboMeCommand([Summary("Your name"), Remainder] string name)
        {
            await _service.RoboMe(Context, name);
        }
    }
}