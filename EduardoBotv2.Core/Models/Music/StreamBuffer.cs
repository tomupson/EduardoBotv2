using System;
using System.IO;
using System.Threading.Tasks;

namespace EduardoBotv2.Core.Models.Music
{
    public class StreamBuffer
    {
        private readonly Stream inStream;
        public event Action PrebufferingCompleted;
        private bool prebuffered;

        private readonly byte[] buffer;

        public int ReadPosition { get; private set; }
        public int WritePosition { get; private set; }

        public int ContentLength => WritePosition >= ReadPosition
            ? WritePosition - ReadPosition
            : (buffer.Length - ReadPosition) + WritePosition;

        public int FreeSpace => buffer.Length - ContentLength;

        public bool Stopped { get; private set; }

        public StreamBuffer(Stream inStream, int bufferSize = 0)
        {
            if (bufferSize == 0)
                bufferSize = 10 * 1024 * 1024;

            this.inStream = inStream;
            buffer = new byte[bufferSize];

            ReadPosition = 0;
            WritePosition = 0;

            this.inStream = inStream;
        }

        public void Stop() => Stopped = true;

        public void StartBuffering()
        {
            Task.Run(async () =>
            {
                byte[] output = new byte[38400];
                int read;
                while (!Stopped && (read = await inStream.ReadAsync(output, 0, 38400).ConfigureAwait(false)) > 0)
                {
                    while (buffer.Length - ContentLength <= read)
                    {
                        if (!prebuffered)
                        {
                            prebuffered = true;
                            PrebufferingCompleted?.Invoke();
                        }

                        await Task.Delay(100).ConfigureAwait(false);
                    }

                    Write(output, read);
                }
            });
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
    }
}