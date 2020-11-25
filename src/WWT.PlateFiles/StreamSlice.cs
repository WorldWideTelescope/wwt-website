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
        private Stream _baseStream;
        private long _position;

        private readonly long _length;

        public StreamSlice(Stream baseStream, long offset, long length)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _length = length;

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
            => ReadInternalAsync(buffer, offset, count, true, cancellationToken).AsTask();

        public override int Read(byte[] buffer, int offset, int count)
            => ReadInternalAsync(buffer, offset, count, isAsync: false, default).Result;

        /// <summary>
        /// A read implementation that can be either async or synchronous to reduce duplication of code. The synchronous pathway will always be completed.
        /// </summary>
        private async ValueTask<int> ReadInternalAsync(byte[] buffer, int offset, int count, bool isAsync, CancellationToken cancellationToken)
        {
            CheckDisposed();
            var remaining = _length - _position;

            if (remaining <= 0)
            {
                return 0;
            }

            if (remaining < count)
            {
                count = (int)remaining;
            }

            var read = isAsync
                ? await _baseStream.ReadAsync(buffer, offset, count, cancellationToken)
                : _baseStream.Read(buffer, offset, count);

            _position += read;

            return read;
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
                return false;
            }
        }

        public override long Position
        {
            get
            {
                CheckDisposed();
                return _position;
            }
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

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
