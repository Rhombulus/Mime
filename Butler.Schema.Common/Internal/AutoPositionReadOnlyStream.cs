namespace Butler.Schema.Internal {

    internal sealed class AutoPositionReadOnlyStream : System.IO.Stream, ICloneableStream {

        public AutoPositionReadOnlyStream(System.IO.Stream wrapped, bool ownsStream) {
            storage = new ReadableDataStorageOnStream(wrapped, ownsStream);
            position = wrapped.Position;
        }

        private AutoPositionReadOnlyStream(AutoPositionReadOnlyStream original) {
            original.storage.AddRef();
            storage = original.storage;
            position = original.position;
        }

        public override bool CanRead => storage != null;
        public override bool CanWrite => false;
        public override bool CanSeek => storage != null;

        public override long Length {
            get {
                if (storage == null)
                    throw new System.ObjectDisposedException("AutoPositionReadOnlyStream");
                return storage.Length;
            }
        }

        public override long Position {
            get {
                if (storage == null)
                    throw new System.ObjectDisposedException("AutoPositionReadOnlyStream");
                return position;
            }
            set {
                if (storage == null)
                    throw new System.ObjectDisposedException("AutoPositionReadOnlyStream");
                if (value < 0L)
                    throw new System.ArgumentOutOfRangeException(nameof(value), Resources.SharedStrings.CannotSeekBeforeBeginning);
                position = value;
            }
        }

        public System.IO.Stream Clone() {
            if (storage == null)
                throw new System.ObjectDisposedException("AutoPositionReadOnlyStream");
            return new AutoPositionReadOnlyStream(this);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (storage == null)
                throw new System.ObjectDisposedException("AutoPositionReadOnlyStream");
            var num = storage.Read(position, buffer, offset, count);
            position += num;
            return num;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new System.NotSupportedException();
        }

        public override void SetLength(long value) {
            throw new System.NotSupportedException();
        }

        public override void Flush() {
            throw new System.NotSupportedException();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            if (storage == null)
                throw new System.ObjectDisposedException("AutoPositionReadOnlyStream");
            switch (origin) {
                case System.IO.SeekOrigin.Begin:
                    if (0L > offset)
                        throw new System.ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.CannotSeekBeforeBeginning);
                    position = offset;
                    return position;
                case System.IO.SeekOrigin.Current:
                    offset += position;
                    goto case 0;
                case System.IO.SeekOrigin.End:
                    offset += this.Length;
                    goto case 0;
                default:
                    throw new System.ArgumentException("origin");
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing && storage != null) {
                storage.Release();
                storage = null;
            }
            base.Dispose(disposing);
        }

        private long position;
        private ReadableDataStorage storage;

    }

}