using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Memes.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Memes
{
    public class Memes : EduardoModule<MemesService>
    {
        public Memes(MemesService service)
            : base(service) { }

        [Command("dank")]
        [Summary("Posts a random dank meme from hot posts on r/dankmemes")]
        public async Task DankCommand()
        {
            await _service.PostDankMeme(Context);
        }
    }
}