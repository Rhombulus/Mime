namespace Butler.Schema.Internal {

    internal static class ScratchPad {

        public static void Begin() {
            if (_pad == null)
                _pad = new ScratchPadContainer();
            else
                _pad.AddRef();
        }

        public static void End() {
            if (_pad == null || !_pad.Release())
                return;
            _pad = null;
        }

        public static byte[] GetByteBuffer(int size) {
            return _pad == null ? new byte[size] : _pad.GetByteBuffer(size);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReleaseByteBuffer() {
            _pad?.ReleaseByteBuffer();
        }

        public static char[] GetCharBuffer(int size) {
            return _pad == null ? new char[size] : _pad.GetCharBuffer(size);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReleaseCharBuffer() {
            _pad?.ReleaseCharBuffer();
        }

        public static System.Text.StringBuilder GetStringBuilder() {
            return ScratchPad.GetStringBuilder(16);
        }

        public static System.Text.StringBuilder GetStringBuilder(int initialCapacity) {
            return _pad == null ? new System.Text.StringBuilder(initialCapacity) : _pad.GetStringBuilder(initialCapacity);
        }

        public static void ReleaseStringBuilder() {
            _pad?.ReleaseStringBuilder();
        }

        [System.ThreadStatic]
        private static ScratchPadContainer _pad;


        private class ScratchPadContainer {

            public ScratchPadContainer() {
                _refCount = 1;
            }

            public void AddRef() {
                ++_refCount;
            }

            public bool Release() {
                --_refCount;
                return _refCount == 0;
            }

            public byte[] GetByteBuffer(int size) {
                if (_byteBuffer == null || _byteBuffer.Length < size)
                    _byteBuffer = new byte[size];
                return _byteBuffer;
            }

            public void ReleaseByteBuffer() {}

            public char[] GetCharBuffer(int size) {
                if (_charBuffer == null || _charBuffer.Length < size)
                    _charBuffer = new char[size];
                return _charBuffer;
            }

            public void ReleaseCharBuffer() {}

            public System.Text.StringBuilder GetStringBuilder(int initialCapacity) {
                if (initialCapacity > 512)
                    return new System.Text.StringBuilder(initialCapacity);
                if (_stringBuilder == null)
                    _stringBuilder = new System.Text.StringBuilder(512);
                else
                    _stringBuilder.Length = 0;
                return _stringBuilder;
            }

            public void ReleaseStringBuilder() {
                if (_stringBuilder == null || _stringBuilder.Capacity <= 512 && _stringBuilder.Length*2 < _stringBuilder.Capacity + 1)
                    return;
                _stringBuilder = null;
            }

            public const int SCRATCH_STRING_BUILDER_CAPACITY = 512;
            private byte[] _byteBuffer;
            private char[] _charBuffer;
            private int _refCount;
            private System.Text.StringBuilder _stringBuilder;

        }

    }

}