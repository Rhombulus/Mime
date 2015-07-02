namespace Butler.Schema.Data.Internal {

    internal class StreamOnReadableDataStorage : StreamOnDataStorage, ICloneableStream {

        public StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end) {
            if (baseStorage != null) {
                baseStorage.AddRef();
                _baseStorage = baseStorage;
            }
            _start = start;
            _end = end;
        }

        private StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end, long position) {
            if (baseStorage != null) {
                baseStorage.AddRef();
                _baseStorage = baseStorage;
            }
            _start = start;
            _end = end;
            _position = position;
        }

        public override DataStorage Storage {
            get {
                this.ThrowIfDisposed();
                return _baseStorage;
            }
        }

        public override long Start {
            get {
                this.ThrowIfDisposed();
                return _start;
            }
        }

        public override long End {
            get {
                this.ThrowIfDisposed();
                return _end;
            }
        }

        public override bool CanRead => !_disposed;
        public override bool CanWrite => false;
        public override bool CanSeek => !_disposed;

        public override long Length {
            get {
                this.ThrowIfDisposed();
                if (_end != long.MaxValue)
                    return _end - _start;
                return _baseStorage.Length - _start;
            }
        }

        public override long Position {
            get {
                this.ThrowIfDisposed();
                return _position;
            }
            set {
                this.ThrowIfDisposed();
                if (value < 0L)
                    throw new System.ArgumentOutOfRangeException(nameof(value), Resources.SharedStrings.CannotSeekBeforeBeginning);
                _position = value;
            }
        }

        public System.IO.Stream Clone() {
            this.ThrowIfDisposed();
            return new StreamOnReadableDataStorage(_baseStorage, _start, _end, _position);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            this.ThrowIfDisposed();
            if (buffer == null)
                throw new System.ArgumentNullException(nameof(buffer));
            if (offset > buffer.Length || offset < 0)
                throw new System.ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.OffsetOutOfRange);
            if (count < 0)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountOutOfRange);
            if (count + offset > buffer.Length)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountTooLarge);
            var num1 = 0;
            if ((_end == long.MaxValue || _position < _end - _start) && count != 0) {
                if (_end != long.MaxValue && count > _end - _start - _position)
                    count = (int) (_end - _start - _position);
                int num2;
                do {
                    num2 = _baseStorage.Read(_start + _position, buffer, offset, count);
                    count -= num2;
                    offset += num2;
                    _position += num2;
                    num1 += num2;
                } while (count != 0 && num2 != 0);
            }
            return num1;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new System.NotSupportedException();
        }

        public override void Flush() {
            throw new System.NotSupportedException();
        }

        public override void SetLength(long value) {
            throw new System.NotSupportedException();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            this.ThrowIfDisposed();
            switch (origin) {
                case System.IO.SeekOrigin.Begin:
                    if (offset < 0L)
                        throw new System.ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.CannotSeekBeforeBeginning);
                    _position = offset;
                    return _position;
                case System.IO.SeekOrigin.Current:
                    offset += _position;
                    goto case 0;
                case System.IO.SeekOrigin.End:
                    offset += this.Length;
                    goto case 0;
                default:
                    throw new System.ArgumentException("Invalid Origin enumeration value", nameof(origin));
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing && _baseStorage != null) {
                _baseStorage.Release();
                _baseStorage = null;
            }
            _disposed = true;
            base.Dispose(disposing);
        }

        private void ThrowIfDisposed() {
            if (_disposed)
                throw new System.ObjectDisposedException("StreamOnReadableDataStorage");
        }

        private readonly long _end;
        private readonly long _start;
        private ReadableDataStorage _baseStorage;
        private bool _disposed;
        private long _position;

    }

}