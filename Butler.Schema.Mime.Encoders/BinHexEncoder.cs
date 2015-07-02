using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public class BinHexEncoder : ByteEncoder {

        public BinHexEncoder() {
            state = BinHexUtils.State.Starting;
        }

        public BinHexEncoder(MacBinaryHeader header) {
            this.MacBinaryHeader = header;
        }

        public MacBinaryHeader MacBinaryHeader {
            get {
                if (header == null)
                    return new MacBinaryHeader();
                return header;
            }
            set {
                if (value != null) {
                    if (0L != value.ResourceForkLength)
                        throw new ArgumentException(Resources.EncodersStrings.BinHexEncoderDoesNotSupportResourceFork);
                    header = new BinHexHeader(value);
                }
                this.Reset();
            }
        }

        public override sealed void Convert(byte[] input, int inputIndex, int inputSize, byte[] output, int outputIndex, int outputSize, bool flush, out int inputUsed, out int outputUsed, out bool completed) {
            if (inputSize != 0) {
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                if (inputIndex < 0 || inputIndex >= input.Length)
                    throw new ArgumentOutOfRangeException(nameof(inputIndex));
                if (inputSize < 0 || inputSize > input.Length - inputIndex)
                    throw new ArgumentOutOfRangeException(nameof(inputSize));
            }
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (outputIndex < 0 || outputIndex >= output.Length)
                throw new ArgumentOutOfRangeException(nameof(outputIndex));
            if (outputSize < 1 || outputSize > output.Length - outputIndex)
                throw new ArgumentOutOfRangeException(nameof(outputSize));
            inputUsed = inputIndex;
            outputUsed = outputIndex;
            var inputTotal = 0;
            if (encodedSize != 0) {
                var count = Math.Min(encodedSize, outputSize);
                Buffer.BlockCopy(encoded, encodedIndex, output, outputIndex, count);
                outputSize -= count;
                outputIndex += count;
                encodedSize -= count;
                if (encodedSize != 0) {
                    encodedIndex += count;
                    outputUsed = outputIndex - outputUsed;
                    inputUsed = 0;
                    completed = false;
                    return;
                }
                encodedIndex = 0;
            }
            var flag = true;
            while (flag) {
                flag = false;
                switch (state) {
                    case BinHexUtils.State.Starting:
                        Buffer.BlockCopy(BinHexPrefix, 0, decoded, 0, BinHexPrefix.Length);
                        decodedIndex = 0;
                        decodedSize = BinHexPrefix.Length;
                        state = BinHexUtils.State.Prefix;
                        goto case 2;
                    case BinHexUtils.State.Prefix:
                        var count1 = Math.Min(decodedSize, outputSize);
                        Buffer.BlockCopy(decoded, decodedIndex, output, outputIndex, count1);
                        outputSize -= count1;
                        outputIndex += count1;
                        decodedSize -= count1;
                        if (decodedSize != 0) {
                            decodedIndex += count1;
                            break;
                        }
                        decodedIndex = 0;
                        previousByte = 256;
                        repeatCount = 0;
                        accumCount = 0;
                        lineOffset = 1;
                        lines = 0;
                        checksum = 0;
                        if (this.header != null) {
                            var bytes = header.GetBytes();
                            Buffer.BlockCopy(bytes, 0, decoded, 0, bytes.Length);
                            decodedSize = bytes.Length;
                            decodedIndex = 0;
                            macbinHeaderSize = 0;
                        } else {
                            macbinHeader = new byte[128];
                            macbinHeaderOffset = 0;
                            macbinHeaderSize = macbinHeader.Length;
                        }
                        state = BinHexUtils.State.Header;
                        goto case 4;
                    case BinHexUtils.State.Header:
                        if (macbinHeaderSize != 0) {
                            var count2 = Math.Min(macbinHeaderSize, inputSize);
                            Buffer.BlockCopy(input, inputIndex, macbinHeader, macbinHeaderOffset, count2);
                            macbinHeaderOffset += count2;
                            macbinHeaderSize -= count2;
                            inputIndex += count2;
                            inputSize -= count2;
                            if (macbinHeaderSize == 0) {
                                var header = new MacBinaryHeader(macbinHeader);
                                macbinHeader = null;
                                this.header = new BinHexHeader(header);
                                var bytes = this.header.GetBytes();
                                Buffer.BlockCopy(bytes, 0, decoded, 0, bytes.Length);
                                decodedSize = bytes.Length;
                                decodedIndex = 0;
                            } else
                                break;
                        }
                        var inputMax1 = decodedSize;
                        if (inputMax1 != 0) {
                            flag = true;
                            this.CompressAndEncode(decoded, ref decodedIndex, ref decodedSize, ref inputTotal, ref inputMax1, false, output, ref outputIndex, ref outputSize);
                        }
                        break;
                    case BinHexUtils.State.Data:
                        var inputMax2 = Math.Min(decodedSize, inputSize);
                        if (inputMax2 != 0) {
                            flag = true;
                            this.CompressAndEncode(input, ref inputIndex, ref inputSize, ref decodedSize, ref inputMax2, true, output, ref outputIndex, ref outputSize);
                        }
                        break;
                    case BinHexUtils.State.DataCRC:
                        if (macbinHeaderSize != 0) {
                            BinHexEncoder.IgnoreChunkFromInput(ref macbinHeaderSize, ref inputIndex, ref inputSize);
                            if (macbinHeaderSize != 0)
                                break;
                        }
                        var inputMax3 = decodedSize;
                        if (inputMax3 != 0) {
                            flag = true;
                            this.CompressAndEncode(decoded, ref decodedIndex, ref decodedSize, ref inputTotal, ref inputMax3, false, output, ref outputIndex, ref outputSize);
                        }
                        break;
                    case BinHexUtils.State.Resource:
                        var inputMax4 = Math.Min(decodedSize, inputSize);
                        if (inputMax4 != 0) {
                            flag = true;
                            this.CompressAndEncode(input, ref inputIndex, ref inputSize, ref decodedSize, ref inputMax4, true, output, ref outputIndex, ref outputSize);
                        }
                        break;
                    case BinHexUtils.State.ResourceCRC:
                        if (macbinHeaderSize != 0) {
                            BinHexEncoder.IgnoreChunkFromInput(ref macbinHeaderSize, ref inputIndex, ref inputSize);
                            if (macbinHeaderSize != 0)
                                break;
                        }
                        var inputMax5 = decodedSize;
                        if (inputMax5 != 0) {
                            flag = true;
                            this.CompressAndEncode(decoded, ref decodedIndex, ref decodedSize, ref inputTotal, ref inputMax5, false, output, ref outputIndex, ref outputSize);
                        }
                        break;
                    case BinHexUtils.State.Ending:
                        var count3 = Math.Min(decodedSize, outputSize);
                        Buffer.BlockCopy(decoded, decodedIndex, output, outputIndex, count3);
                        outputSize -= count3;
                        outputIndex += count3;
                        decodedSize -= count3;
                        if (decodedSize != 0) {
                            decodedIndex += count3;
                            break;
                        }
                        decodedIndex = 0;
                        state = BinHexUtils.State.Ended;
                        goto case 11;
                    case BinHexUtils.State.Ended:
                        macbinHeaderSize = inputSize;
                        BinHexEncoder.IgnoreChunkFromInput(ref macbinHeaderSize, ref inputIndex, ref inputSize);
                        break;
                }
                if (encodedSize != 0)
                    break;
            }
            if (flush && inputSize == 0 && (state != BinHexUtils.State.Ended && outputSize != 0))
                throw new ByteEncoderException(Resources.EncodersStrings.BinHexEncoderDataCorruptCannotFinishEncoding);
            outputUsed = outputIndex - outputUsed;
            inputUsed = inputIndex - inputUsed;
            completed = inputSize == 0 && (!flush || state == BinHexUtils.State.Ended);
            if (!flush || !completed)
                return;
            state = BinHexUtils.State.Starting;
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            var num = (dataCount + 85 + 6)*8/3 + 1;
            return num + (num/64 + 1)*2 + BinHexPrefix.Length + BinHexSuffix.Length;
        }

        public override sealed void Reset() {
            state = BinHexUtils.State.Starting;
            encodedSize = 0;
        }

        public override sealed ByteEncoder Clone() {
            var binHexEncoder = this.MemberwiseClone() as BinHexEncoder;
            binHexEncoder.encoded = encoded.Clone() as byte[];
            binHexEncoder.decoded = decoded.Clone() as byte[];
            if (macbinHeader != null)
                binHexEncoder.macbinHeader = macbinHeader.Clone() as byte[];
            return binHexEncoder;
        }

        private static void IgnoreChunkFromInput(ref int ignoreTotal, ref int index, ref int size) {
            var num = Math.Min(ignoreTotal, size);
            ignoreTotal -= num;
            size -= num;
            index += num;
        }

        private void CompressAndEncode(byte[] input, ref int inputOffset, ref int inputSize, ref int inputTotal, ref int inputMax, bool inputIsCRC, byte[] output, ref int outputOffset, ref int outputSize) {
            byte[] bytes = null;
            var index1 = 0;
            var num1 = 0;
            if (inputIsCRC) {
                bytes = input;
                index1 = inputOffset;
                num1 = inputMax;
            }
            do {
                int num2 = input[inputOffset++];
                --inputSize;
                --inputTotal;
                --inputMax;
                if (num2 == previousByte && repeatCount < 254) {
                    ++repeatCount;
                    if (state >= BinHexUtils.State.ResourceCRC && inputMax == 0)
                        num2 = 257;
                    else
                        goto label_35;
                }
                var numArray1 = new byte[5];
                var num3 = 0;
                if (repeatCount >= 2) {
                    ++repeatCount;
                    var numArray2 = numArray1;
                    var index2 = num3;
                    var num4 = 1;
                    var num5 = index2 + num4;
                    var num6 = 144;
                    numArray2[index2] = (byte) num6;
                    var numArray3 = numArray1;
                    var index3 = num5;
                    var num7 = 1;
                    num3 = index3 + num7;
                    int num8 = (byte) repeatCount;
                    numArray3[index3] = (byte) num8;
                    repeatCount = 0;
                } else if (repeatCount != 0) {
                    numArray1[num3++] = (byte) previousByte;
                    if (previousByte == 144)
                        numArray1[num3++] = 0;
                    repeatCount = 0;
                }
                if (num2 <= byte.MaxValue) {
                    numArray1[num3++] = (byte) num2;
                    if (num2 == 144)
                        numArray1[num3++] = 0;
                }
                var flag = false;
                if (state == BinHexUtils.State.ResourceCRC && inputMax == 0 && (accumCount + num3)%3 != 0) {
                    numArray1[num3++] = 0;
                    flag = true;
                }
                previousByte = num2;
                var index4 = 0;
                while (num3 != 0) {
                    accumValue <<= 8;
                    accumValue |= numArray1[index4];
                    ++accumCount;
                    switch (accumCount) {
                        case 1:
                            encoded[encodedSize++] = Table[accumValue >> 2 & 63];
                            ++lineOffset;
                            break;
                        case 2:
                            encoded[encodedSize++] = Table[accumValue >> 4 & 63];
                            ++lineOffset;
                            break;
                        case 3:
                            encoded[encodedSize++] = Table[accumValue >> 6 & 63];
                            ++lineOffset;
                            if (!flag || 1 != num3) {
                                if (lineOffset == 64) {
                                    encoded[encodedSize++] = 13;
                                    encoded[encodedSize++] = 10;
                                    lineOffset = 0;
                                    ++lines;
                                }
                                encoded[encodedSize++] = Table[accumValue & 63];
                                ++lineOffset;
                            }
                            accumCount = 0;
                            break;
                    }
                    if (lineOffset == 64) {
                        encoded[encodedSize++] = 13;
                        encoded[encodedSize++] = 10;
                        lineOffset = 0;
                        ++lines;
                    }
                    --num3;
                    ++index4;
                }
                var count = Math.Min(encodedSize, outputSize);
                if (output != null) {
                    Buffer.BlockCopy(encoded, encodedIndex, output, outputOffset, count);
                    outputSize -= count;
                }
                outputOffset += count;
                encodedSize -= count;
                if (encodedSize != 0) {
                    encodedIndex += count;
                    break;
                }
                encodedIndex = 0;
                label_35:
                ;
            } while (inputMax != 0);
            switch (state) {
                case BinHexUtils.State.Header:
                    if (decodedSize != 0)
                        break;
                    checksum = 0;
                    if (0L != header.DataForkLength) {
                        decodedSize = (int) header.DataForkLength;
                        state = BinHexUtils.State.Data;
                        break;
                    }
                    macbinHeaderSize = 0;
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    decodedSize = BinHexUtils.MarshalUInt16(decoded, 0, checksum);
                    decodedIndex = 0;
                    state = BinHexUtils.State.DataCRC;
                    break;
                case BinHexUtils.State.Data:
                    var size1 = num1 - inputMax;
                    checksum = BinHexUtils.CalculateCrc(bytes, index1, size1, checksum);
                    if (decodedSize != 0)
                        break;
                    macbinHeaderSize = 0L >= header.ResourceForkLength ? 0 : (header.DataForkLength%128L != 0L ? (int) (128L - header.DataForkLength%128L) : 0);
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    decodedSize = BinHexUtils.MarshalUInt16(decoded, 0, checksum);
                    decodedIndex = 0;
                    state = BinHexUtils.State.DataCRC;
                    break;
                case BinHexUtils.State.DataCRC:
                    if (decodedSize != 0)
                        break;
                    checksum = 0;
                    if (0L != header.ResourceForkLength) {
                        decodedSize = (int) header.ResourceForkLength;
                        state = BinHexUtils.State.Resource;
                        break;
                    }
                    macbinHeaderSize = 0;
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    decodedSize = BinHexUtils.MarshalUInt16(decoded, 0, checksum);
                    decodedIndex = 0;
                    state = BinHexUtils.State.ResourceCRC;
                    break;
                case BinHexUtils.State.Resource:
                    var size2 = num1 - inputMax;
                    checksum = BinHexUtils.CalculateCrc(bytes, index1, size2, checksum);
                    if (decodedSize != 0)
                        break;
                    macbinHeaderSize = header.ResourceForkLength%128L != 0L ? (int) (128L - header.ResourceForkLength%128L) : 0;
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    decodedSize = BinHexUtils.MarshalUInt16(decoded, 0, checksum);
                    decodedIndex = 0;
                    state = BinHexUtils.State.ResourceCRC;
                    break;
                case BinHexUtils.State.ResourceCRC:
                    if (decodedSize != 0)
                        break;
                    Buffer.BlockCopy(BinHexSuffix, 0, decoded, 0, BinHexSuffix.Length);
                    decodedIndex = 0;
                    decodedSize = BinHexSuffix.Length;
                    state = BinHexUtils.State.Ending;
                    break;
                default:
                    throw new Exception(Resources.EncodersStrings.BinHexEncoderInternalError);
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private static readonly byte[] Table = System.Text.Encoding.ASCII.GetBytes("!\"#$%&'()*+,-012345689@ABCDEFGHIJKLMNPQRSTUVXYZ[`abcdefhijklmpqr");
        private static readonly byte[] BinHexPrefix = System.Text.Encoding.ASCII.GetBytes("(This file must be converted with BinHex 4.0)\r\n\r\n:");
        private static readonly byte[] BinHexSuffix = System.Text.Encoding.ASCII.GetBytes(":\r\n");
        private int accumCount;
        private int accumValue;
        private ushort checksum;
        private byte[] decoded = new byte[128];
        private int decodedIndex;
        private int decodedSize;
        private byte[] encoded = new byte[9];
        private int encodedIndex;
        private int encodedSize;
        private BinHexHeader header;
        private int lineOffset;
        private int lines;
        private byte[] macbinHeader;
        private int macbinHeaderOffset;
        private int macbinHeaderSize;
        private int previousByte;
        private int repeatCount;
        private BinHexUtils.State state;

    }

}