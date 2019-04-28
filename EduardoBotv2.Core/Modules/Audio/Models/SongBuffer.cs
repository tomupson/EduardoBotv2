using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EduardoBotv2.Core.Modules.Audio.Models
{
    public class SongBuffer : IDisposable
    {
        private readonly Process _process;
        private readonly byte[] _buffer;
        private readonly Stream _stream;

        private bool prebuffered;
        private bool stopped;

        public TaskCompletionSource<bool> PrebufferingCompleted { get; }

        public int ReadPosition { get; private set; }
        public int WritePosition { get; private set; }

        public int ContentLength => WritePosition >= ReadPosition
            ? WritePosition - ReadPosition
            : _buffer.Length - ReadPosition + WritePosition;

        public int FreeSpace => _buffer.Length - ContentLength;

        public SongBuffer(string songUrl, int bufferSize = 0)
        {
            _process = CreateFfmpegProcess(songUrl);
            _stream = _process.StandardOutput.BaseStream;

            if (bufferSize == 0)
            {
                bufferSize = 10 * 1024 * 1024;
            }

            _buffer = new byte[bufferSize];

            PrebufferingCompleted = new TaskCompletionSource<bool>();
        }

        public void StartBuffering(CancellationToken cancelToken)
        {
            cancelToken.Register(() => PrebufferingCompleted.SetResult(true));
            Task.Run(async () =>
            {
                byte[] output = new byte[38400];
                int read;
                while (!stopped && (read = await _stream.ReadAsync(output, 0, 38400, cancelToken).ConfigureAwait(false)) > 0)
                {
                    while (_buffer.Length - ContentLength <= read)
                    {
                        if (!prebuffered)
                        {
                            prebuffered = true;
                            PrebufferingCompleted.SetResult(true);
                        }

                        await Task.Delay(100, cancelToken).ConfigureAwait(false);
                    }

                    Write(output, read);
                }
            }, cancelToken);
        }

        public ReadOnlySpan<byte> Read(int count)
        {
            int toRead = Math.Min(ContentLength, count);
            int wp = WritePosition;

            if (ContentLength == 0)
                return ReadOnlySpan<byte>.Empty;

            if (wp > ReadPosition || ReadPosition + toRead <= _buffer.Length)
            {
                Span<byte> toReturn = ((Span<byte>)_buffer).Slice(ReadPosition, toRead);
                ReadPosition += toRead;
                return toReturn;
            } else
            {
                byte[] toReturn = new byte[toRead];
                int toEnd = _buffer.Length - ReadPosition;
                Buffer.BlockCopy(_buffer, ReadPosition, toReturn, 0, toEnd);

                int fromStart = toRead - toEnd;
                Buffer.BlockCopy(_buffer, 0, toReturn, toEnd, fromStart);
                ReadPosition = fromStart;
                return toReturn;
            }
        }

        public void Dispose()
        {
            _process.StandardOutput.Dispose();

            if (!_process.HasExited)
            {
                _process.Kill();
            }

            stopped = true;
            _stream.Dispose();
            _process.Dispose();
        }

        private void Write(byte[] input, int writeCount)
        {
            if (WritePosition + writeCount < _buffer.Length)
            {
                Buffer.BlockCopy(input, 0, _buffer, WritePosition, writeCount);
                WritePosition += writeCount;
                return;
            }

            int wroteNormally = _buffer.Length - WritePosition;
            Buffer.BlockCopy(input, 0, _buffer, WritePosition, wroteNormally);
            int wroteFromStart = writeCount - wroteNormally;
            Buffer.BlockCopy(input, wroteNormally, _buffer, 0, wroteFromStart);
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