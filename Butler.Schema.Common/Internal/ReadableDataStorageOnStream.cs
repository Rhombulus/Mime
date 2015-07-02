namespace Butler.Schema.Internal {

    internal class ReadableDataStorageOnStream : ReadableDataStorage {

        public ReadableDataStorageOnStream(System.IO.Stream stream, bool ownsStream) {
            if (stream == null)
                throw new System.ArgumentNullException(nameof(stream));
            _stream = stream;
            _ownsStream = ownsStream;
        }

        public override long Length => _stream.Length;

        public override int Read(long position, byte[] buffer, int offset, int count) {
            this.ThrowIfDisposed();
            var num = 0;
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

        protected override void Dispose(bool disposing) {
            if (!this.IsDisposed) {
                if (disposing && _ownsStream)
                    _stream.Dispose();
                _stream = null;
            }
            base.Dispose(disposing);
        }

        private int InternalRead(long position, byte[] buffer, int offset, int count) {
            _stream.Position = position;
            return _stream.Read(buffer, offset, count);
        }

        private readonly bool _ownsStream;
        private System.IO.Stream _stream;

    }

}