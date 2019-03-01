using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class Audio : ModuleBase<EduardoContext>
    {
        private readonly AudioService service;

        public Audio(AudioService service)
        {
            this.service = service;
        }

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Play an individual song, or the queue")]
        [Remarks("despacito 2")]
        public async Task PlayCommand([Remainder, Summary("The url or name of the song to play, or empty for the queue")]
            string url = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                await service.StartQueue(Context);
            }
            else
            {
                await service.PlaySong(Context, url);
            }
        }

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stop playing all songs")]
        public async Task StopCommand()
        {
            service.Stop();
            await Task.CompletedTask;
        }

        [Command("playing", RunMode = RunMode.Async)]
        [Summary("View the current song")]
        public async Task PlayingCommand()
        {
            await service.ShowCurrentSong(Context);
        }

        [Group("queue")]
        public class Queue : ModuleBase<EduardoContext>
        {
            private readonly AudioService service;

            public Queue(AudioService service)
            {
                this.service = service;
            }

            [Command("play", RunMode = RunMode.Async)]
            [Name("queue play")]
            [Summary("Play the queue")]
            public async Task PlayCommand()
            {
                await service.StartQueue(Context);
            }

            [Command("add", RunMode = RunMode.Async)]
            [Name("queue add")]
            [Summary("Add a song to the queue")]
            [Remarks("")]
            public async Task AddCommand([Remainder, Summary("The url or name of the song to play")]
                string song)
            {
                await service.AddSongToQueue(Context, song);
            }

            [Command("remove", RunMode = RunMode.Async)]
            [Name("queue remove")]
            [Summary("Remove a specific item from the queue")]
            [Remarks("3")]
            public async Task RemoveCommand([Summary("The number of the song in the queue you want to remove")]
                int queueNum)
            {
                await service.RemoveSongFromQueue(Context, queueNum);
            }

            [Command("clear", RunMode = RunMode.Async)]
            [Name("queue clear")]
            [Summary("Clear the queue")]
            public async Task ClearCommand()
            {
                service.ClearQueue();
                await Task.CompletedTask;
            }

            [Command(RunMode = RunMode.Async)]
            [Name("queue")]
            [Summary("View the queue")]
            [Alias("queue view")]
            public async Task ViewCommand()
            {
                await service.ViewQueue(Context);
            }

            [Command("skip", RunMode = RunMode.Async)]
            [Name("queue skip")]
            [Summary("Skip the current item in the queue")]
            public async Task SkipCommand()
            {
                await service.Skip(Context);
            }
        }
    }
}