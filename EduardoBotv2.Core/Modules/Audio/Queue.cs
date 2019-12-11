using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Audio.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Audio
{
    public partial class Audio
    {
        [Group("queue")]
        [Name("Queue")]
        public class Queue : EduardoModule<AudioService>
        {
            public Queue(AudioService service)
                : base(service) { }

            [Command("play", RunMode = RunMode.Async)]
            [Summary("Play the queue")]
            public async Task PlayCommand()
            {
                await _service.StartQueue(Context);
            }

            [Command("add")]
            [Summary("Add a song to the queue")]
            public async Task AddCommand([Summary("The url or name of the song to play"), Remainder] string song)
            {
                await _service.AddSongToQueue(Context, song);
            }

            [Command("remove")]
            [Summary("Remove a specific item from the queue")]
            [Remarks("3")]
            public async Task RemoveCommand([Summary("The number of the song in the queue you want to remove")] int queueNum)
            {
                await _service.RemoveSongFromQueue(Context, queueNum);
            }

            [Command("clear")]
            [Summary("Clear the queue")]
            public Task ClearCommand()
            {
                _service.ClearQueue(Context);
                return Task.CompletedTask;
            }

            [Command("")]
            [Summary("View the queue")]
            public async Task ViewCommand()
            {
                await _service.ViewQueue(Context);
            }

            [Command("skip")]
            [Summary("Skip the current item in the queue")]
            public async Task SkipCommand()
            {
                await _service.Skip(Context);
            }
        }
    }
}