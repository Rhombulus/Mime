using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    internal class BinHexHeader {

        public BinHexHeader() {}

        public BinHexHeader(byte[] header) {
            if (header.Length < 23)
                throw new ArgumentException(Resources.EncodersStrings.BinHexHeaderTooSmall, nameof(header));
            int length = header[0];
            if (header.Length - 22 < length)
                throw new ByteEncoderException(Resources.EncodersStrings.BinHexHeaderIncomplete);
            if (63 < length || 1 > length)
                throw new ByteEncoderException(Resources.EncodersStrings.BinHexHeaderInvalidNameLength);
            var index = 2 + length;
            var num1 = BinHexUtils.CalculateHeaderCrc(header, index + 18);
            var num2 = BinHexUtils.UnmarshalUInt16(header, index + 18);
            if (num1 != num2)
                throw new ByteEncoderException(Resources.EncodersStrings.BinHexHeaderInvalidCrc);
            if (header[1 + length] != 0)
                throw new ByteEncoderException(Resources.EncodersStrings.BinHexHeaderUnsupportedVersion);
            this.FileNameLength = length;
            fileName = new byte[length];
            Buffer.BlockCopy(header, 1, fileName, 0, this.FileNameLength);
            version = header[this.FileNameLength + 1];
            fileType = BinHexUtils.UnmarshalInt32(header, index);
            fileCreator = BinHexUtils.UnmarshalInt32(header, index + 4);
            finderFlags = BinHexUtils.UnmarshalUInt16(header, index + 8);
            this.DataForkLength = BinHexUtils.UnmarshalInt32(header, index + 10);
            this.ResourceForkLength = BinHexUtils.UnmarshalInt32(header, index + 14);
            headerCRC = num2;
        }

        public BinHexHeader(MacBinaryHeader header) {
            if (63 < header.FileNameLength || 1 > header.FileNameLength)
                throw new ByteEncoderException(Resources.EncodersStrings.BinHexHeaderBadFileNameLength);
            this.FileName = header.FileName;
            version = 0;
            fileType = header.FileType;
            fileCreator = header.FileCreator;
            finderFlags = 0;
            this.DataForkLength = header.DataForkLength;
            this.ResourceForkLength = header.ResourceForkLength;
            this.GetBytes();
        }

        public int FileNameLength { get; private set; }

        public string FileName {
            get {
                return System.Text.Encoding.ASCII.GetString(fileName, 0, this.FileNameLength);
            }
            set {
                var bytes = System.Text.Encoding.ASCII.GetBytes(value);
                if (63 < bytes.Length || 1 > bytes.Length)
                    throw new ByteEncoderException(Resources.EncodersStrings.BinHexHeaderBadFileNameLength);
                fileName = bytes;
                this.FileNameLength = bytes.Length;
            }
        }

        public long DataForkLength { get; }
        public long ResourceForkLength { get; }

        public static implicit operator MacBinaryHeader(BinHexHeader rhs) {
            return new MacBinaryHeader {
                FileName = rhs.FileName,
                FileType = rhs.fileType,
                FileCreator = rhs.fileCreator,
                FinderFlags = rhs.finderFlags,
                DataForkLength = rhs.DataForkLength,
                ResourceForkLength = rhs.ResourceForkLength
            };
        }

        public byte[] GetBytes() {
            var num1 = 0;
            var numArray1 = new byte[1 + this.FileNameLength + 1 + 4 + 4 + 2 + 4 + 4 + 2];
            var numArray2 = numArray1;
            var index1 = num1;
            var num2 = 1;
            var dstOffset = index1 + num2;
            int num3 = (byte) this.FileNameLength;
            numArray2[index1] = (byte) num3;
            Buffer.BlockCopy(fileName, 0, numArray1, dstOffset, this.FileNameLength);
            var num4 = dstOffset + this.FileNameLength;
            var numArray3 = numArray1;
            var index2 = num4;
            var num5 = 1;
            var offset1 = index2 + num5;
            int num6 = (byte) version;
            numArray3[index2] = (byte) num6;
            var offset2 = offset1 + BinHexUtils.MarshalInt32(numArray1, offset1, fileType);
            var offset3 = offset2 + BinHexUtils.MarshalInt32(numArray1, offset2, fileCreator);
            var offset4 = offset3 + BinHexUtils.MarshalUInt16(numArray1, offset3, (ushort) finderFlags);
            var offset5 = offset4 + BinHexUtils.MarshalInt32(numArray1, offset4, this.DataForkLength);
            var num7 = offset5 + BinHexUtils.MarshalInt32(numArray1, offset5, this.ResourceForkLength);
            headerCRC = BinHexUtils.CalculateHeaderCrc(numArray1, num7);
            var num8 = num7 + BinHexUtils.MarshalUInt16(numArray1, num7, headerCRC);
            return numArray1;
        }

        public BinHexHeader Clone() {
            var binHexHeader = this.MemberwiseClone() as BinHexHeader;
            binHexHeader.fileName = fileName.Clone() as byte[];
            return binHexHeader;
        }

        private readonly int fileCreator;
        private readonly int fileType;
        private readonly int finderFlags;
        private readonly int version;
        private byte[] fileName;
        private ushort headerCRC;

    }

}