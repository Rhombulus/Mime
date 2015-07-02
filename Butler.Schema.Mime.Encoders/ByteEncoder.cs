using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public abstract class ByteEncoder : IDisposable {

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static ByteEncoder GetEncoder(string name) {
            if (string.Equals(Base64, name, StringComparison.OrdinalIgnoreCase))
                return new Base64Encoder();
            if (string.Equals(QuotedPrintable, name, StringComparison.OrdinalIgnoreCase))
                return new QPEncoder();
            if (string.Equals(UUEncode, name, StringComparison.OrdinalIgnoreCase) || string.Equals(UUEncodeAlt1, name, StringComparison.OrdinalIgnoreCase) || string.Equals(UUEncodeAlt2, name, StringComparison.OrdinalIgnoreCase))
                return new UUEncoder();
            if (string.Equals(BinHex, name, StringComparison.OrdinalIgnoreCase))
                return new BinHexEncoder();
            throw new NotSupportedException(name);
        }

        public static ByteEncoder GetDecoder(string name) {
            if (string.Equals(Base64, name, StringComparison.OrdinalIgnoreCase))
                return new Base64Decoder();
            if (string.Equals(QuotedPrintable, name, StringComparison.OrdinalIgnoreCase))
                return new QPDecoder();
            if (string.Equals(UUEncode, name, StringComparison.OrdinalIgnoreCase) || string.Equals(UUEncodeAlt1, name, StringComparison.OrdinalIgnoreCase) || string.Equals(UUEncodeAlt2, name, StringComparison.OrdinalIgnoreCase))
                return new UUDecoder();
            if (string.Equals(BinHex, name, StringComparison.OrdinalIgnoreCase))
                return new BinHexDecoder();
            throw new NotSupportedException(name);
        }

        public void Convert(System.IO.Stream sourceStream, System.IO.Stream destinationStream) {
            if (sourceStream == null)
                throw new ArgumentNullException(nameof(sourceStream));
            System.IO.Stream stream = new EncoderStream(destinationStream, this, EncoderStreamAccess.Write);
            var buffer = new byte[4096];
            while (true) {
                var count = sourceStream.Read(buffer, 0, buffer.Length);
                if (count != 0)
                    stream.Write(buffer, 0, count);
                else
                    break;
            }
            stream.Flush();
        }

        public abstract void Convert(byte[] input, int inputIndex, int inputSize, byte[] output, int outputIndex, int outputSize, bool done, out int inputUsed, out int outputUsed, out bool completed);
        public abstract int GetMaxByteCount(int dataCount);

        public virtual ByteEncoder Clone() {
            throw new NotSupportedException(
                Resources.EncodersStrings.ThisEncoderDoesNotSupportCloning(
                    this.GetType()
                        .ToString()));
        }

        public abstract void Reset();
        protected virtual void Dispose(bool disposing) {}

        internal static bool IsWhiteSpace(byte bT) {
            if (bT <= 32) {
                return
                    ~(Tables.CharClasses.WSp | Tables.CharClasses.QPEncode | Tables.CharClasses.QPUnsafe | Tables.CharClasses.QPWSp | Tables.CharClasses.QEncode | Tables.CharClasses.QPhraseUnsafe | Tables.CharClasses.QCommentUnsafe |
                      Tables.CharClasses.Token2047) != (Tables.CharacterTraits[bT] & Tables.CharClasses.WSp);
            }
            return false;
        }

        internal static bool BeginsWithNI(byte[] bytes, int offset, byte[] prefix, int length) {
            uint num;
            for (num = 0U; (long) num < (long) length && (long) num < (long) prefix.Length; ++num) {
                if (bytes[offset + num] != prefix[num] && ((prefix[num] | 32) < 97 || (prefix[num] | 32) > 122 || (bytes[offset + num] | 32) != (byte) (prefix[num] | 32U)))
                    return false;
            }
            return num == prefix.Length;
        }

        internal static unsafe void BlockCopy(byte[] data, int offset, byte[] newData, int newOffset, int count) {
            if (newOffset < 0 || count < 0 || newOffset + count > newData.Length)
                throw new ByteEncoderException("internal error");
            fixed (byte* numPtr1 = data) {
                fixed (byte* numPtr2 = newData) {
                    var numPtr3 = numPtr1 + offset;
                    var numPtr4 = numPtr2 + newOffset;
                    if (count >= 16) {
                        var num = count;
                        do {
                            *(int*) numPtr4 = *(int*) numPtr3;
                            *(int*) (numPtr4 + 4) = *(int*) (numPtr3 + 4);
                            *(int*) (numPtr4 + 8) = *(int*) (numPtr3 + 8);
                            *(int*) (numPtr4 + 12) = *(int*) (numPtr3 + 12);
                            numPtr4 += 16;
                            numPtr3 += 16;
                        } while ((num -= 16) >= 16);
                    }
                    if (count > 0) {
                        if ((count & 8) != 0) {
                            *(int*) numPtr4 = *(int*) numPtr3;
                            *(int*) (numPtr4 + 4) = *(int*) (numPtr3 + 4);
                            numPtr4 += 8;
                            numPtr3 += 8;
                        }
                        if ((count & 4) != 0) {
                            *(int*) numPtr4 = *(int*) numPtr3;
                            numPtr4 += 4;
                            numPtr3 += 4;
                        }
                        if ((count & 2) != 0) {
                            *(short*) numPtr4 = *(short*) numPtr3;
                            numPtr4 += 2;
                            numPtr3 += 2;
                        }
                        if ((count & 1) != 0)
                            *numPtr4 = *numPtr3;
                    }
                }
            }
        }

        internal const byte CarriageReturn = 13;
        internal const byte LineFeed = 10;

        internal static readonly byte[] LineWrap = new byte[2] {
            13,
            10
        };

        internal static readonly byte[] NibbleToHex = System.Text.Encoding.ASCII.GetBytes("0123456789ABCDEF");
        private static readonly string Base64 = "base64";
        private static readonly string QuotedPrintable = "quoted-printable";
        private static readonly string UUEncode = "uuencode";
        private static readonly string UUEncodeAlt1 = "x-uuencode";
        private static readonly string UUEncodeAlt2 = "x-uue";
        private static readonly string BinHex = "mac-binhex40";


        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Size = 1)]
        internal struct Tables {

            public static readonly byte[] ByteToBase64 = System.Text.Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/ ");

            public static readonly byte[] Base64ToByte = new byte[96] {
                64,
                64,
                64,
                64,
                64,
                64,
                64,
                64,
                64,
                64,
                64,
                62,
                64,
                64,
                64,
                63,
                52,
                53,
                54,
                55,
                56,
                57,
                58,
                59,
                60,
                61,
                64,
                64,
                64,
                64,
                64,
                64,
                64,
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                11,
                12,
                13,
                14,
                15,
                16,
                17,
                18,
                19,
                20,
                21,
                22,
                23,
                24,
                25,
                64,
                64,
                64,
                64,
                64,
                64,
                26,
                27,
                28,
                29,
                30,
                31,
                32,
                33,
                34,
                35,
                36,
                37,
                38,
                39,
                40,
                41,
                42,
                43,
                44,
                45,
                46,
                47,
                48,
                49,
                50,
                51,
                64,
                64,
                64,
                64,
                64
            };

            public static readonly byte[] NumFromHex = new byte[256] {
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                10,
                11,
                12,
                13,
                14,
                15,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                10,
                11,
                12,
                13,
                14,
                15,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue,
                byte.MaxValue
            };

            public static readonly CharClasses[] CharacterTraits = new CharClasses[128] {
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.WSp | CharClasses.QPWSp | CharClasses.QEncode,
                CharClasses.WSp | CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.WSp | CharClasses.QPWSp | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.WSp | CharClasses.QPWSp,
                CharClasses.QPUnsafe | CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.QCommentUnsafe,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPhraseUnsafe | CharClasses.QCommentUnsafe,
                CharClasses.QPhraseUnsafe | CharClasses.QCommentUnsafe,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.QPhraseUnsafe,
                CharClasses.Token2047,
                CharClasses.QPhraseUnsafe,
                ~(CharClasses.WSp | CharClasses.QPEncode | CharClasses.QPUnsafe | CharClasses.QPWSp | CharClasses.QEncode | CharClasses.QPhraseUnsafe | CharClasses.QCommentUnsafe | CharClasses.Token2047),
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.QPhraseUnsafe,
                CharClasses.QPhraseUnsafe,
                CharClasses.QPhraseUnsafe,
                CharClasses.QPEncode | CharClasses.QEncode,
                CharClasses.QPhraseUnsafe,
                CharClasses.QEncode,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QEncode | CharClasses.Token2047,
                CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPUnsafe | CharClasses.QPhraseUnsafe | CharClasses.Token2047,
                CharClasses.QPEncode | CharClasses.QEncode
            };


            [Flags]
            internal enum CharClasses : byte {

                WSp = (byte) 1,
                QPEncode = (byte) 2,
                QPUnsafe = (byte) 4,
                QPWSp = (byte) 8,
                QEncode = (byte) 16,
                QPhraseUnsafe = (byte) 32,
                QCommentUnsafe = (byte) 64,
                Token2047 = (byte) 128

            }

        }

    }

}