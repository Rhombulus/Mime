namespace Butler.Schema.Internal {

    internal abstract class ReadableDataStorage : DataStorage {

        public abstract long Length { get; }
        public abstract int Read(long position, byte[] buffer, int offset, int count);

        public override System.IO.Stream OpenReadStream(long start, long end) {
            this.ThrowIfDisposed();
            return new StreamOnReadableDataStorage(this, start, end);
        }

        public override long CopyContentToStream(long start, long end, System.IO.Stream destStream, ref byte[] scratchBuffer) {
            this.ThrowIfDisposed();
            if (scratchBuffer == null || scratchBuffer.Length < 16384)
                scratchBuffer = new byte[16384];
            var num = 0L;
            var val1 = end == long.MaxValue ? long.MaxValue : end - start;
            while (val1 != 0L) {
                var count1 = (int) System.Math.Min(val1, scratchBuffer.Length);
                var count2 = this.Read(start, scratchBuffer, 0, count1);
                if (count2 != 0) {
                    start += count2;
                    destStream.Write(scratchBuffer, 0, count2);
                    num += count2;
                    if (val1 != long.MaxValue)
                        val1 -= count2;
                } else
                    break;
            }
            return num;
        }

    }

}