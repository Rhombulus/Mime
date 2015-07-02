using System.Linq;

namespace Butler.Schema.Mime {

    internal struct MimeString {

        public MimeString(byte[] data) {
            this = new MimeString(data, 0, data.Length);
        }

        public MimeString(byte[] data, int offset, int count) {
            if (data.Length > 268435455L)
                throw new MimeException(Resources.Strings.ValueTooLong);
            this.Data = data;
            this.Offset = offset;
            this.LengthAndMask = (uint) (count | -268435456);
        }

        public MimeString(byte[] data, int offset, int count, uint mask) {
            if (data.Length > 268435455L)
                throw new MimeException(Resources.Strings.ValueTooLong);
            this.Data = data;
            this.Offset = offset;
            this.LengthAndMask = (uint) count | mask;
        }

        public MimeString(string data) {
            if (data.Length > 268435455L)
                throw new MimeException(Resources.Strings.ValueTooLong);
            this.Data = Internal.ByteString.StringToBytes(data, true);
            this.Offset = 0;
            this.LengthAndMask = (uint) (this.Data.Length | -268435456);
        }

        internal MimeString(MimeString original, int offset, int count) {
            this.Data = original.Data;
            this.Offset = offset;
            this.LengthAndMask = (uint) (count | -268435456);
        }

        internal MimeString(MimeString original, int offset, int count, uint mask) {
            this.Data = original.Data;
            this.Offset = offset;
            this.LengthAndMask = (uint) count | mask;
        }

        internal MimeString(byte[] data, int offset, uint countAndMask) {
            this.Data = data;
            this.Offset = offset;
            this.LengthAndMask = countAndMask;
        }

        public int Length => (int) this.LengthAndMask & 268435455;
        public int Offset { get; }
        public byte[] Data { get; }

        public uint Mask {
            get {
                return this.LengthAndMask & 4026531840U;
            }
            set {
                this.LengthAndMask = this.LengthAndMask & 268435455U | value;
            }
        }

        public byte this[int index] => this.Data[this.Offset + index];
        internal uint LengthAndMask { get; set; }

        public static bool IsPureASCII(string str) {
            return (string.IsNullOrEmpty(str) || str.All(t => t < 128));
        }

        public static bool IsPureASCII(byte[] bytes) {
            var flag = true;
            if (bytes != null) {
                for (var index = 0; index < bytes.Length; ++index) {
                    if (bytes[index] >= 128) {
                        flag = false;
                        break;
                    }
                }
            }
            return flag;
        }

        public static bool IsPureASCII(MimeString str) {
            var flag = true;
            for (var index = 0; index < str.Length; ++index) {
                if (str[index] >= 128) {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public static bool IsPureASCII(MimeStringList str) {
            var flag = true;
            for (var index = 0; index < str.Count; ++index) {
                if (!MimeString.IsPureASCII(str[index])) {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public static MimeString CopyData(byte[] data, int offset, int count) {
            var numArray = new byte[count];
            Schema.Mime.Encoders.ByteEncoder.BlockCopy(data, offset, numArray, 0, count);
            return new MimeString(numArray, 0, count);
        }

        public override string ToString() {
            if (this.Data != null)
                return Internal.ByteString.BytesToString(this.Data, this.Offset, this.Length, true);
            return string.Empty;
        }

        public void TrimRight(int count) {
            this.LengthAndMask -= (uint) count;
        }

        public byte[] GetSz() {
            if (this.Data == null || this.Offset == 0 && this.Length == this.Data.Length)
                return this.Data;
            var destination = new byte[this.Length];
            this.CopyTo(destination, 0);
            return destination;
        }

        public unsafe MimeString CopyData() {
            if (this.Data == null)
                return new MimeString();
            var data = new byte[this.Length];
            fixed (byte* numPtr1 = this.Data) {
                fixed (byte* numPtr2 = data) {
                    var numPtr3 = numPtr1 + this.Offset;
                    var numPtr4 = numPtr1 + this.Offset + this.Length;
                    var numPtr5 = numPtr2;
                    while (numPtr3 != numPtr4)
                        *numPtr5++ = *numPtr3++;
                }
            }
            return new MimeString(data, 0, data.Length, this.Mask);
        }

        public int CopyTo(byte[] destination, int destinationIndex) {
            System.Buffer.BlockCopy(this.Data, this.Offset, destination, destinationIndex, this.Length);
            return this.Length;
        }

        public void WriteTo(System.IO.Stream stream) {
            stream.Write(this.Data, this.Offset, this.Length);
        }

        internal uint ComputeCrcI() {
            return Internal.ByteString.ComputeCrcI(this.Data, this.Offset, this.Length);
        }

        internal bool CompareEqI(string str2) {
            return Internal.ByteString.EqualsI(str2, this.Data, this.Offset, this.Length, true);
        }

        internal bool CompareEqI(byte[] str2) {
            return Internal.ByteString.EqualsI(this.Data, this.Offset, this.Length, str2, 0, str2.Length, true);
        }

        internal bool CompareEqI(MimeString str2) {
            return Internal.ByteString.EqualsI(this.Data, this.Offset, this.Length, str2.Data, str2.Offset, str2.Length, true);
        }

        internal bool HasPrefixEq(byte[] prefix, int start, int count) {
            if (count > this.Length)
                return false;
            var index = -1;
            var num = count;
            while (++index < num) {
                if (this[index] != prefix[start + index])
                    return false;
            }
            return true;
        }

        internal bool HasPrefixEqI(byte[] prefix, int start, int count) {
            if (count > this.Length)
                return false;
            return Internal.ByteString.EqualsI(this.Data, this.Offset, count, prefix, start, count, true);
        }

        internal byte[] GetData(out int offset, out int count) {
            offset = this.Offset;
            count = this.Length;
            return this.Data;
        }

        internal bool MergeIfAdjacent(MimeString refString) {
            if (this.Data != refString.Data || this.Offset + this.Length != refString.Offset || (int) this.Mask != (int) refString.Mask)
                return false;
            this.LengthAndMask += (uint) refString.Length;
            return true;
        }

        internal const string HdrReceived = "Received";
        internal const string HdrContentType = "Content-Type";
        internal const string HdrContentDisposition = "Content-Disposition";
        internal const string HrdDKIMSignature = "DKIM-Signature";
        internal const string HdrXConvertedToMime = "X-ConvertedToMime";
        internal const byte CARRIAGERETURN = 13;
        internal const byte LINEFEED = 10;
        internal const uint LINEFEEDMASK = 168430090U;
        internal const uint LengthMask = 268435455U;
        internal const uint MaskAny = 4026531840U;
        internal const bool AllowUTF8Value = true;
        internal static readonly byte[] EmptyByteArray = new byte[0];
        public static readonly MimeString Empty = new MimeString(EmptyByteArray);
        internal static readonly byte[] CrLf = Internal.ByteString.StringToBytes("\r\n", true);
        internal static readonly byte[] TwoDashes = Internal.ByteString.StringToBytes("--", true);
        internal static readonly byte[] TwoDashesCRLF = Internal.ByteString.StringToBytes("--\r\n", true);
        internal static readonly byte[] CRLF2Dashes = Internal.ByteString.StringToBytes("\r\n--", true);
        internal static readonly byte[] XParameterNamePrefix = Internal.ByteString.StringToBytes("x-", true);
        internal static readonly byte[] Colon = Internal.ByteString.StringToBytes(":", true);
        internal static readonly byte[] Comma = Internal.ByteString.StringToBytes(",", true);
        internal static readonly byte[] Semicolon = Internal.ByteString.StringToBytes(";", true);
        internal static readonly byte[] Space = Internal.ByteString.StringToBytes(" ", true);
        internal static readonly byte[] LessThan = Internal.ByteString.StringToBytes("<", true);
        internal static readonly byte[] GreaterThan = Internal.ByteString.StringToBytes(">", true);
        internal static readonly byte[] DoubleQuote = Internal.ByteString.StringToBytes("\"", true);
        internal static readonly byte[] Tabulation = Internal.ByteString.StringToBytes("\t", true);
        internal static readonly byte[] Backslash = Internal.ByteString.StringToBytes("\\", true);
        internal static readonly byte[] Asterisk = Internal.ByteString.StringToBytes("*", true);
        internal static readonly byte[] EqualTo = Internal.ByteString.StringToBytes("=", true);
        internal static readonly byte[] CommentInvalidDate = Internal.ByteString.StringToBytes("(invalid)", true);
        internal static readonly byte[] Base64 = Internal.ByteString.StringToBytes("base64", true);
        internal static readonly byte[] QuotedPrintable = Internal.ByteString.StringToBytes("quoted-printable", true);
        internal static readonly byte[] XUuencode = Internal.ByteString.StringToBytes("x-uuencode", true);
        internal static readonly byte[] Uue = Internal.ByteString.StringToBytes("x-uue", true);
        internal static readonly byte[] Uuencode = Internal.ByteString.StringToBytes("uuencode", true);
        internal static readonly byte[] MacBinhex = Internal.ByteString.StringToBytes("mac-binhex40", true);
        internal static readonly byte[] Binary = Internal.ByteString.StringToBytes("binary", true);
        internal static readonly byte[] Encoding8Bit = Internal.ByteString.StringToBytes("8bit", true);
        internal static readonly byte[] Encoding7Bit = Internal.ByteString.StringToBytes("7bit", true);
        internal static readonly byte[] Version1 = Internal.ByteString.StringToBytes("1.0", true);
        internal static readonly byte[] MimeVersion = Internal.ByteString.StringToBytes("MIME-Version: 1.0\r\n", true);
        internal static readonly byte[] TextPlain = Internal.ByteString.StringToBytes("text/plain", true);

        internal static readonly byte[] ConvertedToMimeUU = new byte[1] {
            49
        };

    }

}