using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Modules.Audio.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Audio
{
    [Group("queue")]
    public class Queue : EduardoModule
    {
        private readonly AudioService _service;

        public Queue(AudioService service)
        {
            _service = service;
        }

        [Command("play", RunMode = RunMode.Async)]
        [Name("queue play")]
        [Summary("Play the queue")]
        public async Task PlayCommand()
        {
            await _service.StartQueue(Context);
        }

        [Command("add")]
        [Name("queue add")]
        [Summary("Add a song to the queue")]
        public async Task AddCommand([Remainder, Summary("The url or name of the song to play")]
            string song)
        {
            await _service.AddSongToQueue(Context, song);
        }

        [Command("remove")]
        [Name("queue remove")]
        [Summary("Remove a specific item from the queue")]
        [Remarks("3")]
        public async Task RemoveCommand([Summary("The number of the song in the queue you want to remove")]
            int queueNum)
        {
            await _service.RemoveSongFromQueue(Context, queueNum);
        }

        [Command("clear")]
        [Name("queue clear")]
        [Summary("Clear the queue")]
        public Task ClearCommand()
        {
            _service.ClearQueue();
            return Task.CompletedTask;
        }

        [Command]
        [Name("queue")]
        [Summary("View the queue")]
        public async Task ViewCommand()
        {
            await _service.ViewQueue(Context);
        }

        [Command("skip")]
        [Name("queue skip")]
        [Summary("Skip the current item in the queue")]
        public async Task SkipCommand()
        {
            await _service.Skip(Context);
        }
    }
}