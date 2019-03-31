using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Audio.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Audio
{
    public partial class Audio : EduardoModule<AudioService>
    {
        public Audio(AudioService service)
            : base(service) { }

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Play an individual song, or the queue")]
        [Remarks("despacito 2")]
        public async Task PlayCommand([Remainder, Summary("The url or name of the song to play, or empty for the queue")] string url = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                await _service.StartQueue(Context);
            } else
            {
                await _service.PlaySong(Context, url);
            }
        }

        [Command("stop")]
        [Summary("Stop playing all songs")]
        public Task StopCommand()
        {
            _service.Stop();
            return Task.CompletedTask;
        }

        [Command("playing")]
        [Summary("View the current song")]
        public async Task PlayingCommand()
        {
            await _service.ShowCurrentSong(Context);
        }

        [Command("volume")]
        [Summary("Change the volume of the current song")]
        public Task VolumeCommand([Summary("The new volume")] int volume)
        {
            _service.SetVolume(volume);
            return Task.CompletedTask;
        }

        [Command("pause")]
        [Summary("Toggle the pause of the current song")]
        public Task PauseCommand()
        {
            _service.TogglePause();
            return Task.CompletedTask;
        }
    }
}