using System.IO;

namespace ImageCombinerChannelExtractor.Components.Stream
{
    public class FileExportingStreamer : System.IO.Stream
    {
        private readonly System.IO.Stream _baseStream;
        private readonly Action<long> _onBytesWritten;
        private long _totalBytesWritten;

        public FileExportingStreamer(System.IO.Stream baseStream, Action<long> onBytesWritten)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _onBytesWritten = onBytesWritten;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
            _totalBytesWritten += count;
            _onBytesWritten?.Invoke(_totalBytesWritten);
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => _baseStream.Length;
        public override long Position { get => _baseStream.Position; set => _baseStream.Position = value; }
        public override void Flush() => _baseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);
        public override void SetLength(long value) => _baseStream.SetLength(value);
    }
}
