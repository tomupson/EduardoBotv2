using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Draw.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Draw
{
    public class Draw : EduardoModule
    {
        private readonly DrawService _service;

        public Draw(DrawService service)
        {
            _service = service;
        }

        [Command("draw")]
        [Summary("Draw an emoji, in emoji")]
        public async Task DrawCommand([Summary("The emoji to draw")] string emoji, [Summary("The size, in emoji, of the output drawing")] int size = 30)
        {
            await _service.DrawAsync(Context, emoji, size);
        }
    }
}