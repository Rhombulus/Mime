using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public partial class MimePart {

        internal class CountingWriteStream : System.IO.Stream {

            internal CountingWriteStream(System.IO.Stream stream) {
                _stream = stream;
            }

            public bool IsCountingOnly => _stream == Null;

            public long Count { get; private set; }

            public override bool CanRead => false;

            public override bool CanWrite => true;

            public override bool CanSeek => false;

            public override long Length {
                get {
                    throw new NotSupportedException();
                }
            }

            public override long Position {
                get {
                    throw new NotSupportedException();
                }
                set {
                    throw new NotSupportedException();
                }
            }

            public void Add(long value) {
                this.Count += value;
            }

            public override int Read(byte[] array, int offset, int count) {
                throw new NotSupportedException();
            }

            public override void Write(byte[] array, int offset, int count) {
                this.Count += count;
                _stream.Write(array, offset, count);
            }

            public override void SetLength(long value) {
                throw new NotSupportedException();
            }

            public override void Flush() {
                _stream.Flush();
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin) {
                throw new NotSupportedException();
            }

            private readonly System.IO.Stream _stream;

        }

    }

}