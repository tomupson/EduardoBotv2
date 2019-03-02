using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class Draw : ModuleBase<EduardoContext>
    {
        private readonly DrawService service;

        public Draw(DrawService service)
        {
            this.service = service;
        }

        [Command("draw", RunMode = RunMode.Async)]
        [Summary("Draw an emoji, in emoji")]
        public async Task DrawCommand([Summary("The emoji to draw")] string emoji, [Summary("The size, in emoji, of the output drawing")] int size = 30)
        {
            await service.Draw(Context, emoji, size);
        }
    }
}