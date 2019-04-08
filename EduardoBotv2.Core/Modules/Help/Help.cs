using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Help.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Help
{
    public class Help : EduardoModule<HelpService>
    {
        private readonly CommandService _commandService;

        public Help(HelpService service, CommandService commandService)
            : base(service)
        {
            _commandService = commandService;
        }

        [Command("help")]
        [Alias("command")]
        [Summary("View usage information about a command")]
        [Remarks("money donate")]
        public async Task HelpCommand([Remainder] string commandOrModule = null)
        {
            await _service.DisplayHelpAsync(Context, _commandService, commandOrModule);
        }
    }
}