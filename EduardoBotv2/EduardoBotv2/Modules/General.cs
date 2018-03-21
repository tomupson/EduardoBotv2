using Discord.Commands;
using EduardoBotv2.Services;
using EduardoBotv2.Common.Data;
using System.Threading.Tasks;

namespace EduardoBotv2.Modules
{
    public class General : ModuleBase<EduardoContext>
    {
        private readonly GeneralService _service;
        private readonly CommandService _commandService;

        public General(GeneralService service, CommandService commandService)
        {
            this._service = service;
            this._commandService = commandService;
        }

        [Command("echo", RunMode = RunMode.Async), Alias("talk")]
        [Summary("Echo a message.")]
        [Remarks("I am a bot!")]
        public async Task EchoCommand([Remainder, Summary("The text to echo")] string echo)
        {
            await _service.EchoText(Context, echo);
        }

        [Command("help", RunMode = RunMode.Async), Alias("command", "cmd")]
        [Summary("View Command Information.")]
        [Remarks("money donate")]
        public async Task HelpCommand([Remainder] string commandOrModule = null)
        {
            await _service.DisplayHelp(_commandService, Context, commandOrModule);
        }

        [Command("ping", RunMode = RunMode.Async), Alias("p")]
        [Summary("Responds 'Pong'")]
        [Remarks("")]
        public async Task PingCommand()
        {
            await _service.Ping(Context);
        }

        [Command("choose", RunMode = RunMode.Async)]
        [Summary("Choose a random word from the ones provided.")]
        [Remarks("superman batman")]
        public async Task ChooseCommand(params string[] words)
        {
            await _service.Choose(Context, words);
        }

        [Command("lmgtfy", RunMode = RunMode.Async)]
        [Summary("Googles something for that special person who is crippled and can't do it themselves.")]
        [Remarks("How do I use Eduardo Bot.")]
        public async Task LetMeGoogleThatForYouCommand([Remainder, Summary("What you're searching for.")] string searchQuery) // xd the name
        {
            await _service.GoogleForYou(Context, searchQuery);
        }

        [Command("urban", RunMode = RunMode.Async)]
        [Summary("Look up word on Urban Dictionary.")]
        [Remarks("Eduardo")]
        public async Task UrbanCommand([Remainder, Summary("Words you want to look up on Urban Dictionary.")] string searchQuery)
        {
            await _service.SearchUrbanDictionary(Context, searchQuery);
        }

        [Command("robome", RunMode = RunMode.Async)]
        [Summary("Shows what you would look like as a robot!")]
        [Remarks("Eduardo")]
        public async Task RoboMeCommand([Remainder, Summary("Your name.")] string name)
        {
            await _service.RoboMe(Context, name);
        }
    }
}