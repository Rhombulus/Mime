namespace Butler.Schema.Internal {

    internal class ReadableWritableDataStorageOnStream : ReadableWritableDataStorage {

        public ReadableWritableDataStorageOnStream(System.IO.Stream stream, bool ownsStream) {
            if (stream == null)
                throw new System.ArgumentNullException(nameof(stream));
            Stream = stream;
            OwnsStream = ownsStream;
        }

        public override long Length {
            get {
                this.ThrowIfDisposed();
                return Stream.Length;
            }
        }

        public override int Read(long position, byte[] buffer, int offset, int count) {
            this.ThrowIfDisposed();
            int num;
            if (isReadOnly) {
                readOnlySemaphore.Wait();
                try {
                    num = this.InternalRead(position, buffer, offset, count);
                } finally {
                    readOnlySemaphore.Release();
                }
            } else
                num = this.InternalRead(position, buffer, offset, count);
            return num;
        }

        public override void Write(long position, byte[] buffer, int offset, int count) {
            this.ThrowIfDisposed();
            if (isReadOnly)
                throw new System.InvalidOperationException("Write to read-only DataStorage");
            Stream.Position = position;
            Stream.Write(buffer, offset, count);
        }

        public override void SetLength(long length) {
            this.ThrowIfDisposed();
            if (isReadOnly)
                throw new System.InvalidOperationException("Write to read-only DataStorage");
            Stream.SetLength(length);
        }

        protected override void Dispose(bool disposing) {
            if (!this.IsDisposed) {
                if (disposing && OwnsStream)
                    Stream.Dispose();
                Stream = null;
            }
            base.Dispose(disposing);
        }

        private int InternalRead(long position, byte[] buffer, int offset, int count) {
            Stream.Position = position;
            return Stream.Read(buffer, offset, count);
        }

        protected bool OwnsStream;
        protected System.IO.Stream Stream;

    }

}