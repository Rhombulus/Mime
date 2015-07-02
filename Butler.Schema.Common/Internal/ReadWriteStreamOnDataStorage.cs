namespace Butler.Schema.Internal {

    internal class ReadWriteStreamOnDataStorage : StreamOnDataStorage, ICloneableStream {

        internal ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage) {
            storage.AddRef();
            _storage = storage;
        }

        private ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage, long position) {
            storage.AddRef();
            _storage = storage;
            _position = position;
        }

        public override DataStorage Storage {
            get {
                if (_storage == null)
                    throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
                return _storage;
            }
        }

        public override long Start => 0L;
        public override long End => long.MaxValue;
        public override bool CanRead => _storage != null;
        public override bool CanWrite => _storage != null;
        public override bool CanSeek => _storage != null;

        public override long Length {
            get {
                if (_storage == null)
                    throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
                return _storage.Length;
            }
        }

        public override long Position {
            get {
                if (_storage == null)
                    throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
                return _position;
            }
            set {
                if (_storage == null)
                    throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
                if (value < 0L)
                    throw new System.ArgumentOutOfRangeException(nameof(value), Resources.SharedStrings.CannotSeekBeforeBeginning);
                _position = value;
            }
        }

        System.IO.Stream ICloneableStream.Clone() {
            if (_storage == null)
                throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
            return new ReadWriteStreamOnDataStorage(_storage, _position);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (_storage == null)
                throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
            if (buffer == null)
                throw new System.ArgumentNullException(nameof(buffer));
            if (offset > buffer.Length || offset < 0)
                throw new System.ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.OffsetOutOfRange);
            if (count < 0)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountOutOfRange);
            if (count + offset > buffer.Length)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountTooLarge);
            var num = _storage.Read(_position, buffer, offset, count);
            _position += num;
            return num;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (_storage == null)
                throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
            if (buffer == null)
                throw new System.ArgumentNullException(nameof(buffer));
            if (offset > buffer.Length || offset < 0)
                throw new System.ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.OffsetOutOfRange);
            if (count > buffer.Length || count < 0)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountOutOfRange);
            if (count + offset > buffer.Length)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountTooLarge);
            _storage.Write(_position, buffer, offset, count);
            _position += count;
        }

        public override void SetLength(long value) {
            if (_storage == null)
                throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
            if (value < 0L)
                throw new System.ArgumentOutOfRangeException(nameof(value), Resources.SharedStrings.CannotSetNegativelength);
            _storage.SetLength(value);
        }

        public override void Flush() {
            if (_storage == null)
                throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            if (_storage == null)
                throw new System.ObjectDisposedException("ReadWriteStreamOnDataStorage");
            switch (origin) {
                case System.IO.SeekOrigin.Begin:
                    _position = offset;
                    break;
                case System.IO.SeekOrigin.Current:
                    offset = _position + offset;
                    break;
                case System.IO.SeekOrigin.End:
                    offset = _storage.Length + offset;
                    break;
                default:
                    throw new System.ArgumentException("Invalid Origin enumeration value", nameof(origin));
            }
            if (offset < 0L)
                throw new System.ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.CannotSeekBeforeBeginning);
            _position = offset;
            return _position;
        }

        protected override void Dispose(bool disposing) {
            if (disposing && _storage != null) {
                _storage.Release();
                _storage = null;
            }
            base.Dispose(disposing);
        }

        private long _position;
        private ReadableWritableDataStorage _storage;

    }

}