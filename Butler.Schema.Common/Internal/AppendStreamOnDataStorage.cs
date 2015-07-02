namespace Butler.Schema.Data.Internal {

    internal class AppendStreamOnDataStorage : StreamOnDataStorage {

        public AppendStreamOnDataStorage(ReadableWritableDataStorage storage) {
            storage.AddRef();
            this.ReadableWritableStorage = storage;
            position = storage.Length;
        }

        public override DataStorage Storage => (DataStorage) this.ReadableWritableStorage;
        public override long Start => 0L;
        public override long End => position;
        public ReadableWritableDataStorage ReadableWritableStorage { get; private set; }
        public override bool CanRead => false;
        public override bool CanWrite => this.ReadableWritableStorage != null;
        public override bool CanSeek => false;

        public override long Length {
            get {
                throw new System.NotSupportedException();
            }
        }

        public override long Position {
            get {
                throw new System.NotSupportedException();
            }
            set {
                throw new System.NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            throw new System.NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (this.ReadableWritableStorage == null)
                throw new System.ObjectDisposedException("AppendStreamOnDataStorage");
            if (buffer == null)
                throw new System.ArgumentNullException(nameof(buffer));
            if (offset > buffer.Length || offset < 0)
                throw new System.ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.OffsetOutOfRange);
            if (count > buffer.Length || count < 0)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountOutOfRange);
            if (count + offset > buffer.Length)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountTooLarge);
            this.ReadableWritableStorage.Write(position, buffer, offset, count);
            position += count;
        }

        public override void SetLength(long value) {
            throw new System.NotSupportedException();
        }

        public override void Flush() {
            if (this.ReadableWritableStorage == null)
                throw new System.ObjectDisposedException("AppendStreamOnDataStorage");
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            throw new System.NotSupportedException();
        }

        protected override void Dispose(bool disposing) {
            if (disposing && this.ReadableWritableStorage != null) {
                this.ReadableWritableStorage.Release();
                this.ReadableWritableStorage = null;
            }
            base.Dispose(disposing);
        }

        private long position;

    }

}