using Discord.Commands;
using EduardoBotv2.Services;
using System.Threading.Tasks;
using EduardoBotv2.Models;

namespace EduardoBotv2.Modules
{
    public class Audio : ModuleBase<EduardoContext>
    {
        private readonly AudioService service;

        public Audio(AudioService service)
        {
            this.service = service;
        }

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Play an individual song, or the queue.")]
        [Remarks("rick astley")]
        public async Task PlayCommand([Remainder, Summary("The (optional) URL or name of the song you want to play.")] string url = null)
        {
            if (url == null)
            {
                await service.StartQueue(Context);
            }
            else
            {
                await service.PlaySong(Context, url);
            }
        }

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stop playing all songs.")]
        [Remarks("")]
        public async Task StopCommand()
        {
            await service.Stop(Context);
        }
        
        [Command("playing", RunMode = RunMode.Async)]
        [Summary("View the current song.")]
        [Remarks("")]
        public async Task PlayingCommand()
        {
            await service.ShowCurrentlyPlayingSong(Context);
        }

        [Group("queue")]
        public class Queue : ModuleBase<EduardoContext>
        {
            private readonly AudioService service;

            public Queue(AudioService service)
            {
                this.service = service;
            }

            [Command("play", RunMode = RunMode.Async), Name("queue play")]
            [Summary("Play an individual song, or the queue.")]
            [Remarks("")]
            public async Task PlayCommand()
            {
                await service.StartQueue(Context);
            }

            [Command("add", RunMode = RunMode.Async), Name("queue add")]
            [Summary("Queue a song to play.")]
            [Remarks("pepe the frog meme")]
            public async Task AddCommand([Remainder, Summary("The URL or name of the song you want to play.")] string song)
            {
                await service.AddSongToQueue(Context, song);
            }

            [Command("remove", RunMode = RunMode.Async), Name("queue remove")]
            [Summary("Remove a specific item from the queue.")]
            [Remarks("3")]
            public async Task RemoveCommand([Remainder, Summary("The number of the song in the queue you want to remove.")] int queueItem)
            {

                await service.RemoveSongFromQueue(Context, queueItem);
            }

            [Command("clear", RunMode = RunMode.Async), Name("queue clear")]
            [Summary("Clear the queue.")]
            [Remarks("")]
            public async Task ClearCommand()
            {
                await service.ClearQueue();
            }

            [Command(RunMode = RunMode.Async), Name("queue")]
            [Summary("View the queue.")]
            [Remarks("")]
            public async Task ViewCommand()
            {
                await service.ViewQueue(Context);
            }

            [Command("skip", RunMode = RunMode.Async), Name("queue skip")]
            [Summary("Skip the current item in the queue.")]
            [Remarks("")]
            public async Task SkipCommand()
            {
                await service.SkipQueueSong(Context);
            }
        }
    }
}