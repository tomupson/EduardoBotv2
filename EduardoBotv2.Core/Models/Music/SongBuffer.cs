using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EduardoBotv2.Core.Models.Music
{
    public class SongBuffer : IDisposable
    {
        private readonly Process process;
        private readonly StreamBuffer buffer;
        private Stream stream;

        public TaskCompletionSource<bool> PrebufferingCompleted { get; }

        public SongBuffer(string songUrl)
        {
            process = CreateFfmpegProcess(songUrl);
            stream = process.StandardOutput.BaseStream;
            buffer = new StreamBuffer(stream);

            PrebufferingCompleted = new TaskCompletionSource<bool>();
        }

        private static Process CreateFfmpegProcess(string url) => Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -err_detect ignore_err -i \"{url}\" -vol 100 -f s16le -ar 48000 -vn -ac 2 pipe:1 -loglevel error",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            RedirectStandardError = false
        });

        public byte[] Read(int count) => buffer.Read(count).ToArray();

        public void Dispose()
        {
            process.StandardOutput.Dispose();

            if (!process.HasExited)
            {
                process.Kill();
            }

            buffer.Stop();
            stream.Dispose();
            process.Dispose();
            buffer.PrebufferingCompleted += OnPrebufferingCompleted;
        }

        public void StartBuffering()
        {
            buffer.StartBuffering();
            buffer.PrebufferingCompleted += OnPrebufferingCompleted;
        }

        private void OnPrebufferingCompleted()
        {
            PrebufferingCompleted.SetResult(true);
        }
    }
}