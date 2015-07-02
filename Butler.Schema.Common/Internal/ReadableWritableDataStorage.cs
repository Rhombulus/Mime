namespace Butler.Schema.Internal {

    internal abstract class ReadableWritableDataStorage : ReadableDataStorage {

        public abstract void Write(long position, byte[] buffer, int offset, int count);
        public abstract void SetLength(long length);

        public virtual StreamOnDataStorage OpenWriteStream(bool append) {
            this.ThrowIfDisposed();
            if (append)
                return new AppendStreamOnDataStorage(this);
            return new ReadWriteStreamOnDataStorage(this);
        }

    }

}