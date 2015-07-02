using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public class MacBinaryHeader {

        public MacBinaryHeader() {
            version = 130;
            minimumVersion = 129;
        }

        public MacBinaryHeader(byte[] bytes) {
            if (bytes == null)
                throw new System.ArgumentNullException(nameof(bytes));
            if (bytes.Length != 128)
                throw new System.ArgumentException(Resources.EncodersStrings.MacBinHeaderMustBe128Long, nameof(bytes));
            if (bytes[0] != 0 || bytes[74] != 0 || bytes[82] != 0)
                throw new ByteEncoderException(Resources.EncodersStrings.MacBinInvalidData);
            if (bytes[1] > 63)
                throw new ByteEncoderException(Resources.EncodersStrings.MacBinInvalidData);
            this.FileNameLength = bytes[1];
            fileName = System.Text.Encoding.ASCII.GetString(bytes, 2, this.FileNameLength);
            this.FileType = BinHexUtils.UnmarshalInt32(bytes, 65);
            this.FileCreator = BinHexUtils.UnmarshalInt32(bytes, 69);
            this.FinderFlags = bytes[73];
            iconXOffset = BinHexUtils.UnmarshalUInt16(bytes, 75);
            iconYOffset = BinHexUtils.UnmarshalUInt16(bytes, 77);
            this.Protected = 1 == bytes[81];
            this.DataForkLength = BinHexUtils.UnmarshalInt32(bytes, 83);
            this.ResourceForkLength = BinHexUtils.UnmarshalInt32(bytes, 87);
            version = bytes[122];
            minimumVersion = bytes[123];
        }

        public int OldVersion => 0;
        public int FileNameLength { get; private set; }

        public string FileName {
            get {
                return fileName;
            }
            set {
                var bytes = System.Text.Encoding.ASCII.GetBytes(value);
                if (bytes.Length > 63)
                    throw new System.ArgumentException(Resources.EncodersStrings.MacBinFileNameTooLong, nameof(value));
                fileName = value;
                this.FileNameLength = bytes.Length;
            }
        }

        public int FileType { get; set; }
        public int FileCreator { get; set; }
        public int FinderFlags { get; set; }

        public int XIcon {
            get {
                return iconXOffset;
            }
            set {
                if (ushort.MaxValue < (uint) value)
                    throw new System.ArgumentOutOfRangeException(nameof(value), Resources.EncodersStrings.MacBinIconOffsetTooLarge(ushort.MaxValue));
                iconXOffset = value;
            }
        }

        public int YIcon {
            get {
                return iconYOffset;
            }
            set {
                if (ushort.MaxValue < (uint) value)
                    throw new System.ArgumentOutOfRangeException(nameof(value), Resources.EncodersStrings.MacBinIconOffsetTooLarge(ushort.MaxValue));
                iconYOffset = value;
            }
        }

        public int FileId { get; set; }
        public bool Protected { get; set; }
        public long DataForkLength { get; set; }
        public long ResourceForkLength { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.DateTime ModificationDate { get; set; }
        public int GetInfoLength { get; set; }
        public int UnpackedSize { get; set; }
        public int SecondaryHeaderLength { get; set; }

        public int Version {
            get {
                return version;
            }
            set {
                if (value != 0 && 129 != value && 130 != value)
                    throw new System.ArgumentOutOfRangeException(nameof(value), Resources.EncodersStrings.MacBinBadVersion);
                version = value;
            }
        }

        public int MinimumVersion {
            get {
                return minimumVersion;
            }
            set {
                if (value != 0 && 129 != value && 130 != value)
                    throw new System.ArgumentOutOfRangeException(nameof(value), Resources.EncodersStrings.MacBinBadVersion);
                minimumVersion = value;
            }
        }

        public short CheckSum {
            get {
                this.GetBytes();
                return (short) headerCRC;
            }
        }

        internal byte[] GetBytes() {
            const int num1 = 0;
            var numArray1 = new byte[128];
            var numArray2 = numArray1;
            const int index1 = num1;
            const int num2 = 1;
            var num3 = index1 + num2;
            const int num4 = 0;
            numArray2[index1] = num4;
            var numArray3 = numArray1;
            var index2 = num3;
            const int num5 = 1;
            var dstOffset = index2 + num5;
            int num6 = (byte) this.FileNameLength;
            numArray3[index2] = (byte) num6;
            System.Buffer.BlockCopy(this.FileNameAsByteArray(), 0, numArray1, dstOffset, this.FileNameLength);
            const int offset1 = 65;
            var offset2 = offset1 + BinHexUtils.MarshalInt32(numArray1, offset1, this.FileType);
            var num7 = offset2 + BinHexUtils.MarshalInt32(numArray1, offset2, this.FileCreator);
            var numArray4 = numArray1;
            var index3 = num7;
            const int num8 = 1;
            var num9 = index3 + num8;
            int num10 = (byte) ((65280 & this.FinderFlags) >> 8);
            numArray4[index3] = (byte) num10;
            var numArray5 = numArray1;
            var index4 = num9;
            const int num11 = 1;
            var offset3 = index4 + num11;
            const int num12 = 0;
            numArray5[index4] = num12;
            var offset4 = offset3 + BinHexUtils.MarshalUInt16(numArray1, offset3, (ushort) iconXOffset);
            var offset5 = offset4 + BinHexUtils.MarshalUInt16(numArray1, offset4, (ushort) iconYOffset);
            var num13 = offset5 + BinHexUtils.MarshalUInt16(numArray1, offset5, 0);
            var numArray6 = numArray1;
            var index5 = num13;
            const int num14 = 1;
            var num15 = index5 + num14;
            var num16 = this.Protected ? 1 : 0;
            numArray6[index5] = (byte) num16;
            var numArray7 = numArray1;
            var index6 = num15;
            const int num17 = 1;
            var offset6 = index6 + num17;
            const int num18 = 0;
            numArray7[index6] = num18;
            var offset7 = offset6 + BinHexUtils.MarshalInt32(numArray1, offset6, this.DataForkLength);
            var offset8 = offset7 + BinHexUtils.MarshalInt32(numArray1, offset7, this.ResourceForkLength);
            var offset9 = offset8 + BinHexUtils.MarshalInt32(numArray1, offset8, 0L);
            var offset10 = offset9 + BinHexUtils.MarshalInt32(numArray1, offset9, 0L);
            var num19 = offset10 + BinHexUtils.MarshalUInt16(numArray1, offset10, (ushort) this.GetInfoLength);
            var numArray8 = numArray1;
            var index7 = num19;
            const int num20 = 1;
            var num21 = index7 + num20;
            int num22 = (byte) (byte.MaxValue & this.FinderFlags);
            numArray8[index7] = (byte) num22;
            var offset11 = num21 + 18;
            var num23 = offset11 + BinHexUtils.MarshalUInt16(numArray1, offset11, (ushort) this.SecondaryHeaderLength);
            var numArray9 = numArray1;
            var index8 = num23;
            const int num24 = 1;
            var num25 = index8 + num24;
            int num26 = (byte) version;
            numArray9[index8] = (byte) num26;
            var numArray10 = numArray1;
            var index9 = num25;
            const int num27 = 1;
            var offset12 = index9 + num27;
            int num28 = (byte) minimumVersion;
            numArray10[index9] = (byte) num28;
            headerCRC = BinHexUtils.CalculateHeaderCrc(numArray1, 124);
            var num29 = offset12 + BinHexUtils.MarshalUInt16(numArray1, offset12, headerCRC);
            var numArray11 = numArray1;
            var index10 = num29;
            const int num30 = 1;
            var num31 = index10 + num30;
            const int num32 = 0;
            numArray11[index10] = num32;
            var numArray12 = numArray1;
            var index11 = num31;
            const int num35 = 0;
            numArray12[index11] = num35;
            return numArray1;
        }

        private byte[] FileNameAsByteArray() {
            return System.Text.Encoding.ASCII.GetBytes(fileName);
        }

        private string fileName;
        private ushort headerCRC;
        private int iconXOffset;
        private int iconYOffset;
        private int minimumVersion;
        private int version;


        [System.Flags]
        internal enum FinderFlagsFields {

            OnDesk = 1,
            Color = 14,
            ColorReserved = 16,
            SwitchLaunch = 32,
            Shared = 64,
            NoInits = 128,
            Initialized = 256,
            Reserved = 512,
            CustomIcon = 1024,
            Stationary = 2048,
            NameLocked = 4096,
            Bundle = 8192,
            Invisible = 16384,
            Alias = 32768

        }

    }

}