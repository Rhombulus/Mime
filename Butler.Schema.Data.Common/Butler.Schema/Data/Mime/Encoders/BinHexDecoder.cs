using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public class BinHexDecoder : ByteEncoder {

        public BinHexDecoder() {
            this.DataForkOnly = true;
        }

        public BinHexDecoder(bool dataForkOnly) {
            this.DataForkOnly = dataForkOnly;
        }

        public bool DataForkOnly { get; set; }

        public MacBinaryHeader MacBinaryHeader {
            get {
                if (header == null)
                    return new MacBinaryHeader();
                return header;
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
            do {
                if (extraSize != 0) {
                    var count = Math.Min(extraSize, outputSize);
                    Buffer.BlockCopy(extra, extraIndex, output, outputIndex, count);
                    outputSize -= count;
                    outputIndex += count;
                    extraSize -= count;
                    if (extraSize != 0) {
                        extraIndex += count;
                        goto label_45;
                    }
                    extraIndex = 0;
                }
                if (repeatCount != 0) {
                    switch (state) {
                        case BinHexUtils.State.Data:
                        case BinHexUtils.State.Resource:
                            this.OutputChunk(output, ref outputIndex, ref outputSize);
                            break;
                        default:
                            this.OutputChunk(scratch, ref scratchIndex, ref scratchSize);
                            break;
                    }
                    if (bytesNeeded == 0)
                        this.TransitionState();
                    if (outputSize != 0)
                        continue;
                } else if (lineReady) {
                    var bT = encoded[encodedIndex++];
                    --encodedSize;
                    if (encodedSize == 0)
                        lineReady = false;
                    if (!ByteEncoder.IsWhiteSpace(bT)) {
                        if (state == BinHexUtils.State.Started) {
                            if (58 != bT)
                                throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderFirstNonWhitespaceMustBeColon);
                            state = BinHexUtils.State.HdrFileSize;
                            bytesNeeded = 1;
                            scratchSize = scratch.Length;
                            scratchIndex = 0;
                            runningCRC = 0;
                            continue;
                        }
                        var num1 = (byte) (bT - 32U);
                        var num2 = (int) num1 >= Dictionary.Length ? (byte) 127 : Dictionary[num1];
                        if (num2 == sbyte.MaxValue)
                            throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderFoundInvalidCharacter);
                        if (accumCount == 0) {
                            accum = num2;
                            ++accumCount;
                            continue;
                        }
                        accum = accum << 6 | num2;
                        var num3 = (byte) (accum >> ShiftTabe[accumCount] & byte.MaxValue);
                        ++accumCount;
                        accumCount %= System.Runtime.InteropServices.Marshal.SizeOf(accum);
                        if (repeatCheck) {
                            repeatCheck = false;
                            if (num3 == 0) {
                                decodedByte = 144;
                                repeatCount = 1;
                                continue;
                            }
                            repeatCount = num3 - 1;
                            continue;
                        }
                        if (144 == num3) {
                            repeatCheck = true;
                            continue;
                        }
                        decodedByte = num3;
                        repeatCount = 1;
                    }
                    continue;
                }
                label_45:
                if (!lineReady)
                    lineReady = this.ReadLine(input, ref inputIndex, ref inputSize, flush);
                else
                    break;
            } while (lineReady);
            outputUsed = outputIndex - outputUsed;
            inputUsed = inputIndex - inputUsed;
            completed = inputSize == 0 && extraSize == 0 && repeatCount == 0 && !lineReady;
            if (!flush || !completed)
                return;
            if (state != BinHexUtils.State.Ending)
                throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderDataCorrupt);
            this.Reset();
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            throw new NotImplementedException();
        }

        public override sealed void Reset() {
            state = BinHexUtils.State.Starting;
            lineReady = false;
            encodedSize = 0;
            repeatCount = 0;
            extraIndex = 0;
            extraSize = 0;
        }

        public override sealed ByteEncoder Clone() {
            var binHexDecoder = this.MemberwiseClone() as BinHexDecoder;
            binHexDecoder.encoded = encoded.Clone() as byte[];
            binHexDecoder.scratch = scratch.Clone() as byte[];
            binHexDecoder.extra = extra.Clone() as byte[];
            binHexDecoder.header = header != null ? header.Clone() : null;
            return binHexDecoder;
        }

        private void OutputChunk(byte[] bytes, ref int index, ref int size) {
            var val2 = Math.Min(bytesNeeded, repeatCount);
            var count = Math.Min(size, val2);
            if (bytes != null) {
                for (var index1 = 0; index1 < count; ++index1) {
                    bytes[index++] = decodedByte;
                }
                size -= count;
            }
            runningCRC = BinHexUtils.CalculateCrc(decodedByte, count, runningCRC);
            bytesNeeded -= count;
            repeatCount -= count;
        }

        private bool ReadLine(byte[] input, ref int inputIndex, ref int inputSize, bool flush) {
            if (state == BinHexUtils.State.Ending) {
                inputIndex += inputSize;
                inputSize = 0;
                return false;
            }
            var flag = false;
            while (inputSize != 0 || flush && encodedSize != 0) {
                byte num = 10;
                if (inputSize != 0) {
                    --inputSize;
                    num = input[inputIndex++];
                }
                if (num != 10) {
                    if (encodedSize >= encoded.Length)
                        throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderLineTooLong);
                    encoded[encodedSize++] = num;
                } else if (state == BinHexUtils.State.Starting) {
                    for (var index = 0U; (long) index < (long) encodedSize; ++index) {
                        if (!ByteEncoder.IsWhiteSpace(encoded[index])) {
                            if ((uint) ((ulong) encodedSize - index) < BinHexPrologue.Length || !ByteEncoder.BeginsWithNI(encoded, (int) index, BinHexPrologue, BinHexPrologue.Length))
                                throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderLineCorrupt);
                            state = BinHexUtils.State.Started;
                            repeatCheck = false;
                            accumCount = 0;
                            accum = 0;
                            header = null;
                            break;
                        }
                    }
                    encodedSize = 0;
                } else {
                    while (encodedSize != 0 && (encoded[encodedSize - 1] == 13 || encoded[encodedSize - 1] == 32 || encoded[encodedSize - 1] == 9))
                        --encodedSize;
                    if (encodedSize == 0) {
                        if (state != BinHexUtils.State.Started)
                            ;
                    } else {
                        flag = true;
                        encodedIndex = 0;
                        break;
                    }
                }
            }
            return flag;
        }

        private void TransitionState() {
            switch (state) {
                case BinHexUtils.State.HdrFileSize:
                    if (scratch[0] > 63)
                        throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderFileNameTooLong);
                    state = BinHexUtils.State.Header;
                    bytesNeeded = scratch[0] + 21;
                    break;
                case BinHexUtils.State.Header:
                    header = new BinHexHeader(scratch);
                    if (!this.DataForkOnly) {
                        var bytes = header.GetBytes();
                        Buffer.BlockCopy(bytes, 0, extra, 0, bytes.Length);
                        extraIndex = 0;
                        extraSize = bytes.Length;
                    }
                    if (0L != header.DataForkLength) {
                        state = BinHexUtils.State.Data;
                        bytesNeeded = (int) header.DataForkLength;
                        runningCRC = 0;
                        break;
                    }
                    checksum = runningCRC;
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    state = BinHexUtils.State.DataCRC;
                    bytesNeeded = 2;
                    scratchSize = scratch.Length;
                    scratchIndex = 0;
                    break;
                case BinHexUtils.State.Data:
                    checksum = runningCRC;
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    state = BinHexUtils.State.DataCRC;
                    bytesNeeded = 2;
                    scratchSize = scratch.Length;
                    scratchIndex = 0;
                    break;
                case BinHexUtils.State.DataCRC:
                    if ((checksum & 65280) >> 8 != scratch[0] || (checksum & byte.MaxValue) != scratch[1])
                        throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderBadCrc);
                    if (this.DataForkOnly) {
                        state = BinHexUtils.State.Ending;
                        repeatCount = 0;
                        encodedSize = 0;
                        lineReady = false;
                        break;
                    }
                    var length1 = header.DataForkLength%128L != 0L ? (int) (128L - header.DataForkLength%128L) : 0;
                    if (length1 != 0) {
                        Array.Clear(extra, 0, length1);
                        extraSize = length1;
                    }
                    if (header.ResourceForkLength > 0L) {
                        state = BinHexUtils.State.Resource;
                        bytesNeeded = (int) header.ResourceForkLength;
                        runningCRC = 0;
                        break;
                    }
                    checksum = 0;
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    state = BinHexUtils.State.ResourceCRC;
                    bytesNeeded = 2;
                    scratchSize = scratch.Length;
                    scratchIndex = 0;
                    break;
                case BinHexUtils.State.Resource:
                    checksum = runningCRC;
                    checksum = BinHexUtils.CalculateCrc(checksum);
                    state = BinHexUtils.State.ResourceCRC;
                    bytesNeeded = 2;
                    scratchSize = scratch.Length;
                    scratchIndex = 0;
                    break;
                case BinHexUtils.State.ResourceCRC:
                    if ((checksum & 65280) >> 8 != scratch[0] || (checksum & byte.MaxValue) != scratch[1])
                        throw new ByteEncoderException(CtsResources.EncodersStrings.BinHexDecoderBadResourceForkCrc);
                    var length2 = header.ResourceForkLength%128L != 0L ? (int) (128L - header.ResourceForkLength%128L) : 0;
                    if (length2 != 0) {
                        Array.Clear(extra, 0, length2);
                        extraSize = length2;
                    }
                    state = BinHexUtils.State.Ending;
                    repeatCount = 0;
                    encodedSize = 0;
                    lineReady = false;
                    break;
                default:
                    throw new Exception(CtsResources.EncodersStrings.BinHexDecoderInternalError);
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private static readonly int[] ShiftTabe = new int[4] {
            0,
            4,
            2,
            0
        };

        private static readonly byte[] BinHexPrologue = CTSGlobals.AsciiEncoding.GetBytes("(This file must be converted with BinHex");

        private static readonly byte[] Dictionary = new byte[88] {
            127,
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
            127,
            127,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            127,
            20,
            21,
            22,
            127,
            127,
            127,
            127,
            127,
            22,
            23,
            24,
            25,
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
            127,
            37,
            38,
            39,
            40,
            41,
            42,
            43,
            127,
            44,
            45,
            46,
            47,
            127,
            127,
            127,
            127,
            48,
            49,
            50,
            51,
            52,
            53,
            54,
            127,
            55,
            56,
            57,
            58,
            59,
            60,
            127,
            127,
            61,
            62,
            63,
            127,
            127,
            127,
            127,
            127
        };

        private int accum;
        private int accumCount;
        private int bytesNeeded;
        private ushort checksum;
        private byte decodedByte;
        private byte[] encoded = new byte[128];
        private int encodedIndex;
        private int encodedSize;
        private byte[] extra = new byte[128];
        private int extraIndex;
        private int extraSize;
        private BinHexHeader header;
        private bool lineReady;
        private bool repeatCheck;
        private int repeatCount;
        private ushort runningCRC;
        private byte[] scratch = new byte[128];
        private int scratchIndex;
        private int scratchSize;
        private BinHexUtils.State state;

    }

}