#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.PlateFiles
{
    /// <summary>
    /// An implementation of <see cref="Stream"/> that allows for slicing of another stream given
    /// an initial offset and length.
    /// </summary>
    public class StreamSlice : Stream
    {
        private readonly long _offset;
        private readonly long _length;

        private Stream _baseStream;

        /// <summary>
        /// Attempts to create a <see cref="StreamSlice"/>. Any exception is wrapped in
        /// a <see cref=" PlateTileException"/>.
        /// </summary>
        public static Stream Create(Stream baseStream, long offset, long length)
        {
            try
            {
                return new StreamSlice(baseStream, offset, length);
            }
            catch (Exception e)
            {
                throw new PlateTileException(e.Message, e);
            }
        }

        public StreamSlice(Stream baseStream, long offset, long length)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _length = length;
            _offset = offset;

            if (!baseStream.CanRead)
            {
                throw new ArgumentException("Stream must support read.");
            }

            if (!baseStream.CanSeek)
            {
                throw new ArgumentException("Stream must support seek.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (offset + length > baseStream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            baseStream.Seek(offset, SeekOrigin.Begin);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer.AsSpan(offset, count));

        public override int ReadByte()
        {
            Span<byte> buffer = stackalloc byte[1];
            return Read(buffer) == 0 ? -1 : buffer[0];
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            var remaining = _length - Position;

            if (remaining <= 0)
            {
                return 0;
            }

            if (remaining < buffer.Length)
            {
                buffer = buffer[..(int)remaining];
            }

            return await _baseStream.ReadAsync(buffer, cancellationToken);
        }

        public override int Read(Span<byte> buffer)
        {
            CheckDisposed();
            var remaining = _length - Position;

            if (remaining <= 0)
            {
                return 0;
            }

            if (remaining < buffer.Length)
            {
                buffer = buffer[..(int)remaining];
            }

            return _baseStream.Read(buffer);
        }

        private void CheckDisposed()
        {
            if (_baseStream == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public override long Length
        {
            get
            {
                CheckDisposed();
                return _length;
            }
        }

        public override bool CanRead
        {
            get
            {
                CheckDisposed();
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                CheckDisposed();
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                CheckDisposed();
                return _baseStream.CanSeek;
            }
        }

        public override long Position
        {
            get
            {
                CheckDisposed();
                return _baseStream.Position - _offset;
            }
            set => Seek(value, SeekOrigin.Begin);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();

            var basePosition = origin switch
            {
                SeekOrigin.Begin => offset + _offset,
                SeekOrigin.Current => _baseStream.Position + offset,
                SeekOrigin.End => _offset + _length - offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin)),
            };

            _baseStream.Seek(basePosition, SeekOrigin.Begin);

            return Position;
        }

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Flush()
        {
            CheckDisposed();
            _baseStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            return base.FlushAsync(cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_baseStream != null)
                {
                    try { _baseStream.Dispose(); }
                    catch { }
                    _baseStream = null;
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotImplementedException();
    }
}
