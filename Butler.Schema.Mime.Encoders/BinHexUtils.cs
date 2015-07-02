using System.Linq;

namespace Butler.Schema.Mime.Encoders {

    internal class BinHexUtils {

        private BinHexUtils() {}

        internal static ushort CalculateHeaderCrc(byte[] bytes, int count) {
            ushort num1 = 0;
            for (var index1 = 0; index1 < count; ++index1) {
                var num2 = bytes[index1];
                for (var index2 = 0; index2 < 8; ++index2) {
                    var num3 = (ushort) (num1 & 32768U);
                    num1 = (ushort) (num1 << 1 | num2 >> 7);
                    if (num3 != 0)
                        num1 ^= 4129;
                    num2 <<= 1;
                }
            }
            for (var index1 = 0; index1 < 2; ++index1) {
                for (var index2 = 0; index2 < 8; ++index2) {
                    var num2 = (ushort) (num1 & 32768U);
                    num1 <<= 1;
                    if (num2 != 0)
                        num1 ^= 4129;
                }
            }
            return num1;
        }

        internal static ushort CalculateCrc(byte[] bytes, int index, int size, ushort seed) {
            var num1 = seed;
            for (var index1 = index; index1 < index + size; ++index1) {
                var num2 = bytes[index1];
                for (var index2 = 0; index2 < 8; ++index2) {
                    var num3 = (ushort) (num1 & 32768U);
                    num1 = (ushort) (num1 << 1 | num2 >> 7);
                    if (num3 != 0)
                        num1 ^= 4129;
                    num2 <<= 1;
                }
            }
            return num1;
        }

        internal static ushort CalculateCrc(byte data, int count, ushort seed) {
            var num1 = seed;
            while (0 < count--) {
                var num2 = data;
                for (var index = 0; index < 8; ++index) {
                    var num3 = (ushort) (num1 & 32768U);
                    num1 = (ushort) (num1 << 1 | num2 >> 7);
                    if (num3 != 0)
                        num1 ^= 4129;
                    num2 <<= 1;
                }
            }
            return num1;
        }

        internal static ushort CalculateCrc(ushort seed) {
            var bytes = new byte[2];
            return BinHexUtils.CalculateCrc(bytes, 0, bytes.Length, seed);
        }

        internal static int MarshalInt32(byte[] array, int offset, long value) {
            array[offset] = (byte) ((value & 4278190080L) >> 24);
            array[offset + 1] = (byte) ((value & 16711680L) >> 16);
            array[offset + 2] = (byte) ((value & 65280L) >> 8);
            array[offset + 3] = (byte) ((ulong) value & byte.MaxValue);
            return 4;
        }

        internal static int UnmarshalInt32(byte[] array, int index) {
            return (int) ((((uint) array[index] << 8 | array[index + 1]) << 8 | array[index + 2]) << 8 | array[index + 3]);
        }

        internal static int MarshalUInt16(byte[] array, int offset, ushort value) {
            array[offset] = (byte) ((value & 65280) >> 8);
            array[offset + 1] = (byte) (value & (uint) byte.MaxValue);
            return 2;
        }

        internal static ushort UnmarshalUInt16(byte[] array, int index) {
            return (ushort) ((uint) array[index] << 8 | array[index + 1]);
        }


        internal enum State {

            Starting,
            Started,
            Prefix,
            HdrFileSize,
            Header,
            HeaderCRC,
            Data,
            DataCRC,
            Resource,
            ResourceCRC,
            Ending,
            Ended

        }

    }

}