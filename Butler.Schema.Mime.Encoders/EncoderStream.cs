using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public class EncoderStream : System.IO.Stream, Internal.ICloneableStream {

        public EncoderStream(System.IO.Stream stream, ByteEncoder encoder, EncoderStreamAccess access) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));
            if (access == EncoderStreamAccess.Read) {
                if (!stream.CanRead)
                    throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotRead);
            } else if (!stream.CanWrite)
                throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotWrite);
            this.stream = stream;
            this.encoder = encoder;
            this.access = access;
            ownsStream = true;
            length = long.MaxValue;
            buffer = new byte[4096];
        }

        internal EncoderStream(System.IO.Stream stream, ByteEncoder encoder, EncoderStreamAccess access, bool ownsStream) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));
            if (access == EncoderStreamAccess.Read) {
                if (!stream.CanRead)
                    throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotRead);
            } else if (!stream.CanWrite)
                throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotWrite);
            this.stream = stream;
            this.encoder = encoder;
            this.access = access;
            this.ownsStream = ownsStream;
            length = long.MaxValue;
            buffer = new byte[4096];
        }

        public override sealed bool CanRead {
            get {
                if (access == EncoderStreamAccess.Read)
                    return this.IsOpen;
                return false;
            }
        }

        public override sealed bool CanWrite {
            get {
                if (EncoderStreamAccess.Write == access)
                    return this.IsOpen;
                return false;
            }
        }

        public override sealed bool CanSeek {
            get {
                if (access == EncoderStreamAccess.Read && this.IsOpen)
                    return stream.CanSeek;
                return false;
            }
        }

        public override sealed long Length {
            get {
                this.AssertOpen();
                if (access != EncoderStreamAccess.Read)
                    return position;
                if (long.MaxValue == length) {
                    var stream = this.Clone();
                    var num1 = position;
                    var buffer = new byte[4096];
                    long num2;
                    do {
                        num2 = stream.Read(buffer, 0, 4096);
                        num1 += num2;
                    } while (num2 > 0L);
                    length = num1;
                }
                return length;
            }
        }

        public override sealed long Position {
            get {
                this.AssertOpen();
                return position;
            }
            set {
                this.AssertOpen();
                this.Seek(value, System.IO.SeekOrigin.Begin);
            }
        }

        public System.IO.Stream BaseStream {
            get {
                this.AssertOpen();
                return stream;
            }
        }

        public ByteEncoder ByteEncoder {
            get {
                this.AssertOpen();
                return encoder;
            }
        }

        private bool IsOpen => null != stream;

        public System.IO.Stream Clone() {
            this.AssertOpen();
            if (EncoderStreamAccess.Write == access)
                throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotCloneWriteableStream);
            var cloneableStream = stream as Internal.ICloneableStream;
            if (cloneableStream == null && stream.CanSeek) {
                stream = new Internal.AutoPositionReadOnlyStream(stream, ownsStream);
                ownsStream = true;
                cloneableStream = stream as Internal.ICloneableStream;
            }
            if (cloneableStream == null) {
                throw new NotSupportedException(
                    Resources.EncodersStrings.EncStrCannotCloneChildStream(
                        stream.GetType()
                              .ToString()));
            }
            var encoderStream = this.MemberwiseClone() as EncoderStream;
            encoderStream.buffer = buffer.Clone() as byte[];
            encoderStream.stream = cloneableStream.Clone();
            encoderStream.encoder = encoder.Clone();
            return encoderStream;
        }

        public override sealed int Read(byte[] array, int offset, int count) {
            this.AssertOpen();
            if (!this.CanRead)
                throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotRead);
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset + count > array.Length)
                throw new ArgumentOutOfRangeException("offset, count", Resources.EncodersStrings.EncStrLengthExceeded(offset + count, array.Length));
            if (0 > offset || 0 > count)
                throw new ArgumentOutOfRangeException(offset < 0 ? "offset" : "count");
            var num = 0;
            while (!endOfFile && count != 0) {
                if (bufferCount == 0) {
                    bufferPos = 0;
                    bufferCount = stream.Read(buffer, 0, 4096);
                }
                int inputUsed;
                int outputUsed;
                bool completed;
                encoder.Convert(buffer, bufferPos, bufferCount, array, offset, count, bufferCount == 0, out inputUsed, out outputUsed, out completed);
                if (bufferCount == 0 && completed)
                    endOfFile = true;
                count -= outputUsed;
                offset += outputUsed;
                num += outputUsed;
                position += outputUsed;
                bufferPos += inputUsed;
                bufferCount -= inputUsed;
            }
            return num;
        }

        public override sealed void Write(byte[] array, int offset, int count) {
            this.AssertOpen();
            if (!this.CanWrite)
                throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotWrite);
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (count + offset > array.Length)
                throw new ArgumentException(Resources.EncodersStrings.EncStrLengthExceeded(offset + count, array.Length), nameof(array));
            if (0 > offset || 0 > count)
                throw new ArgumentOutOfRangeException(offset < 0 ? "offset" : "count");
            while (count != 0) {
                int inputUsed;
                int outputUsed;
                bool completed;
                encoder.Convert(array, offset, count, buffer, 0, buffer.Length, false, out inputUsed, out outputUsed, out completed);
                count -= inputUsed;
                offset += inputUsed;
                position += inputUsed;
                stream.Write(buffer, 0, outputUsed);
            }
        }

        public override sealed long Seek(long offset, System.IO.SeekOrigin origin) {
            this.AssertOpen();
            if (!this.CanSeek)
                throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotSeek);
            if (origin == System.IO.SeekOrigin.Current)
                offset += position;
            else if (origin == System.IO.SeekOrigin.End) {
                if (length == long.MaxValue && offset == 0L)
                    offset = long.MaxValue;
                else
                    offset += this.Length;
            }
            if (offset < 0L)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset < position) {
                bufferPos = 0;
                bufferCount = 0;
                endOfFile = false;
                encoder.Reset();
                stream.Seek(0L, System.IO.SeekOrigin.Begin);
                position = 0L;
            }
            if (offset > position) {
                var val1 = offset - position;
                var buffer = new byte[4096];
                while (val1 > 0L) {
                    var count = (int) Math.Min(val1, 4096L);
                    var num = this.Read(buffer, 0, count);
                    if (num == 0) {
                        if (length == long.MaxValue)
                            length = position;
                        offset = position;
                        break;
                    }
                    val1 -= num;
                }
            }
            return offset;
        }

        public override sealed void SetLength(long value) {
            this.AssertOpen();
            throw new NotSupportedException();
        }

        public override sealed void Flush() {
            this.AssertOpen();
            if (!this.CanWrite)
                throw new NotSupportedException(Resources.EncodersStrings.EncStrCannotWrite);
            this.FlushEncoder(false);
            stream.Flush();
        }

        private void FlushEncoder(bool done) {
            var completed = false;
            int outputUsed;
            do {
                int inputUsed;
                encoder.Convert(null, 0, 0, buffer, 0, buffer.Length, done, out inputUsed, out outputUsed, out completed);
                stream.Write(buffer, 0, outputUsed);
            } while (0 < outputUsed && !completed);
        }

        private void AssertOpen() {
            if (!this.IsOpen)
                throw new ObjectDisposedException("EncoderStream");
        }

        protected override void Dispose(bool disposing) {
            if (disposing && this.IsOpen) {
                if (this.CanWrite)
                    this.FlushEncoder(true);
                if (stream != null) {
                    stream.Dispose();
                    stream = null;
                }
                if (encoder != null) {
                    encoder.Dispose();
                    encoder = null;
                }
            }
            base.Dispose(disposing);
        }

        private const int BlockSize = 4096;
        private readonly EncoderStreamAccess access;
        private byte[] buffer;
        private int bufferCount;
        private int bufferPos;
        private ByteEncoder encoder;
        private bool endOfFile;
        private long length;
        private bool ownsStream;
        private long position;
        private System.IO.Stream stream;

    }

}