namespace Butler.Schema.Internal {

    internal abstract class DataStorage : RefCountable {

        internal bool IsReadOnly => isReadOnly;

        public static long CopyStreamToStream(System.IO.Stream srcStream, System.IO.Stream destStream, long lengthToCopy, ref byte[] scratchBuffer) {
            if (scratchBuffer == null || scratchBuffer.Length < 16384)
                scratchBuffer = new byte[16384];
            var num = 0L;
            while (lengthToCopy != 0L) {
                var count1 = (int) System.Math.Min(lengthToCopy, scratchBuffer.Length);
                var count2 = srcStream.Read(scratchBuffer, 0, count1);
                if (count2 != 0) {
                    destStream?.Write(scratchBuffer, 0, count2);
                    num += count2;
                    if (lengthToCopy != long.MaxValue)
                        lengthToCopy -= count2;
                } else
                    break;
            }
            return num;
        }

        public static System.IO.Stream NewEmptyReadStream() {
            return new StreamOnReadableDataStorage(null, 0L, 0L);
        }

        public abstract System.IO.Stream OpenReadStream(long start, long end);

        public virtual long CopyContentToStream(long start, long end, System.IO.Stream destStream, ref byte[] scratchBuffer) {
            this.ThrowIfDisposed();
            if (destStream == null && end != long.MaxValue)
                return end - start;
            using (var srcStream = this.OpenReadStream(start, end))
                return DataStorage.CopyStreamToStream(srcStream, destStream, long.MaxValue, ref scratchBuffer);
        }

        internal virtual void SetReadOnly(bool makeReadOnly) {
            this.ThrowIfDisposed();
            if (makeReadOnly == isReadOnly)
                return;
            readOnlySemaphore = !makeReadOnly ? null : new System.Threading.SemaphoreSlim(1);
            isReadOnly = makeReadOnly;
        }

        protected bool isReadOnly;
        protected System.Threading.SemaphoreSlim readOnlySemaphore;

    }

}