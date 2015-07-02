namespace Butler.Schema.Internal {

    internal sealed class SuppressCloseStream : System.IO.Stream, ICloneableStream {

        public SuppressCloseStream(System.IO.Stream sourceStream) {
            if (sourceStream == null)
                throw new System.ArgumentNullException(nameof(sourceStream));
            _sourceStream = sourceStream;
        }

        public override bool CanRead => _sourceStream != null && _sourceStream.CanRead;
        public override bool CanWrite => _sourceStream != null && _sourceStream.CanWrite;
        public override bool CanSeek => _sourceStream != null && _sourceStream.CanSeek;

        public override long Length {
            get {
                this.AssertOpen();
                return _sourceStream.Length;
            }
        }

        public override long Position {
            get {
                this.AssertOpen();
                return _sourceStream.Position;
            }
            set {
                this.AssertOpen();
                _sourceStream.Position = value;
            }
        }

        public System.IO.Stream Clone() {
            this.AssertOpen();
            if (this.CanWrite)
                throw new System.NotSupportedException();
            var cloneableStream = _sourceStream as ICloneableStream;
            if (cloneableStream != null)
                return new SuppressCloseStream(cloneableStream.Clone());
            if (!_sourceStream.CanSeek)
                throw new System.NotSupportedException();
            _sourceStream = new AutoPositionReadOnlyStream(_sourceStream, false);
            cloneableStream = (ICloneableStream) _sourceStream;
            return new SuppressCloseStream(cloneableStream.Clone());
        }

        public override int Read(byte[] buffer, int offset, int count) {
            this.AssertOpen();
            return _sourceStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            this.AssertOpen();
            _sourceStream.Write(buffer, offset, count);
        }

        public override void Flush() {
            this.AssertOpen();
            _sourceStream.Flush();
        }

        public override void SetLength(long value) {
            this.AssertOpen();
            _sourceStream.SetLength(value);
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            this.AssertOpen();
            return _sourceStream.Seek(offset, origin);
        }

        private void AssertOpen() {
            if (_sourceStream == null)
                throw new System.ObjectDisposedException("SuppressCloseStream");
        }

        protected override void Dispose(bool disposing) {
            _sourceStream = null;
            base.Dispose(disposing);
        }

        private System.IO.Stream _sourceStream;

    }

}