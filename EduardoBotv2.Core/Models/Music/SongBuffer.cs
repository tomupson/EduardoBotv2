using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EduardoBotv2.Core.Models.Music
{
    public class SongBuffer : IDisposable
    {
        private readonly Process process;
        private readonly byte[] buffer;
        private readonly Stream stream;
        private bool prebuffered;
        private bool stopped;

        public TaskCompletionSource<bool> PrebufferingCompleted { get; }

        public int ReadPosition { get; private set; }
        public int WritePosition { get; private set; }

        public int ContentLength => WritePosition >= ReadPosition
            ? WritePosition - ReadPosition
            : buffer.Length - ReadPosition + WritePosition;

        public int FreeSpace => buffer.Length - ContentLength;

        public SongBuffer(string songUrl, int bufferSize = 0)
        {
            process = CreateFfmpegProcess(songUrl);
            stream = process.StandardOutput.BaseStream;

            if (bufferSize == 0)
            {
                bufferSize = 10 * 1024 * 1024;
            }

            buffer = new byte[bufferSize];

            PrebufferingCompleted = new TaskCompletionSource<bool>();
        }

        public void StartBuffering()
        {
            Task.Run(async () =>
            {
                byte[] output = new byte[38400];
                int read;
                while (!stopped && (read = await stream.ReadAsync(output, 0, 38400).ConfigureAwait(false)) > 0)
                {
                    while (buffer.Length - ContentLength <= read)
                    {
                        if (!prebuffered)
                        {
                            prebuffered = true;
                            PrebufferingCompleted.SetResult(true);
                        }

                        await Task.Delay(100).ConfigureAwait(false);
                    }

                    Write(output, read);
                }
            });
        }

        public ReadOnlySpan<byte> Read(int count)
        {
            int toRead = Math.Min(ContentLength, count);
            int wp = WritePosition;

            if (ContentLength == 0)
                return ReadOnlySpan<byte>.Empty;

            if (wp > ReadPosition || ReadPosition + toRead <= buffer.Length)
            {
                Span<byte> toReturn = ((Span<byte>)buffer).Slice(ReadPosition, toRead);
                ReadPosition += toRead;
                return toReturn;
            } else
            {
                byte[] toReturn = new byte[toRead];
                int toEnd = buffer.Length - ReadPosition;
                Buffer.BlockCopy(buffer, ReadPosition, toReturn, 0, toEnd);

                int fromStart = toRead - toEnd;
                Buffer.BlockCopy(buffer, 0, toReturn, toEnd, fromStart);
                ReadPosition = fromStart;
                return toReturn;
            }
        }

        public void Dispose()
        {
            process.StandardOutput.Dispose();

            if (!process.HasExited)
            {
                process.Kill();
            }

            stopped = true;
            stream.Dispose();
            process.Dispose();
        }

        private void Write(byte[] input, int writeCount)
        {
            if (WritePosition + writeCount < buffer.Length)
            {
                Buffer.BlockCopy(input, 0, buffer, WritePosition, writeCount);
                WritePosition += writeCount;
                return;
            }

            int wroteNormally = buffer.Length - WritePosition;
            Buffer.BlockCopy(input, 0, buffer, WritePosition, wroteNormally);
            int wroteFromStart = writeCount - wroteNormally;
            Buffer.BlockCopy(input, wroteNormally, buffer, 0, wroteFromStart);
            WritePosition = wroteFromStart;
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
    }
}