using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public class UUEncoder : ByteEncoder {

        public UUEncoder() {}

        public UUEncoder(string fileName) {
            this.FileName = fileName;
        }

        public string FileName {
            get {
                if (fileName != null)
                    return System.Text.Encoding.ASCII.GetString(fileName, 0, fileName.Length);
                return string.Empty;
            }
            set {
                var num = 0;
                if (value != null)
                    num = value.Length;
                if (num > 48)
                    throw new ArgumentOutOfRangeException("FileName", Resources.EncodersStrings.UUEncoderFileNameTooLong(48));
                if (num == 0)
                    fileName = null;
                else
                    fileName = System.Text.Encoding.ASCII.GetBytes(value);
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
            if (numLines == 0) {
                outLineReady = false;
                bufferSize = 0;
                Array.Clear(chunk, 0, chunk.Length);
                chunkIndex = 0;
                if (fileName != null) {
                    ByteEncoder.BlockCopy(Prologue, 0, buffer, 0, Prologue.Length);
                    bufferSize = Prologue.Length;
                    ByteEncoder.BlockCopy(fileName, 0, buffer, bufferSize, fileName.Length);
                    bufferSize += fileName.Length;
                    ByteEncoder.BlockCopy(LineWrap, 0, buffer, bufferSize, LineWrap.Length);
                    bufferSize += LineWrap.Length;
                    bufferIndex = 0;
                    outLineReady = true;
                    ++numLines;
                }
            }
            inputUsed = inputIndex;
            outputUsed = outputIndex;
            do {
                if (outLineReady) {
                    var count = Math.Min(bufferSize, outputSize);
                    ByteEncoder.BlockCopy(buffer, bufferIndex, output, outputIndex, count);
                    outputSize -= count;
                    outputIndex += count;
                    bufferSize -= count;
                    bufferIndex += count;
                    if (bufferSize == 0) {
                        outLineReady = false;
                        if (3 == bufferIndex) {
                            if (fileName != null) {
                                ByteEncoder.BlockCopy(Epilogue, 0, buffer, 0, Epilogue.Length);
                                bufferSize = Epilogue.Length;
                                bufferIndex = 0;
                                outLineReady = true;
                                continue;
                            }
                            break;
                        }
                        if (5 != bufferIndex || fileName == null || buffer[0] != 101)
                            bufferIndex = 0;
                        else
                            break;
                    } else
                        break;
                }
                if (bufferSize == 0) {
                    buffer[bufferSize++] = 0;
                    bufferIndex = 0;
                    rawCount = 0;
                    ++numLines;
                }
                while (inputSize != 0 || flush) {
                    if (inputSize != 0) {
                        var num = Math.Min(3 - chunkIndex, inputSize);
                        if ((num & 2) != 0) {
                            chunk[chunkIndex++] = input[inputIndex++];
                            chunk[chunkIndex++] = input[inputIndex++];
                        }
                        if ((num & 1) != 0)
                            chunk[chunkIndex++] = input[inputIndex++];
                        inputSize -= num;
                        if (chunkIndex != 3)
                            continue;
                    }
                    if (chunkIndex != 0) {
                        if (3 != chunkIndex)
                            Array.Clear(chunk, chunkIndex, 3 - chunkIndex);
                        buffer[bufferSize++] = UUEncoder.UUEncode(chunk[0] >> 2);
                        buffer[bufferSize++] = UUEncoder.UUEncode(chunk[0] << 4 | chunk[1] >> 4);
                        buffer[bufferSize++] = UUEncoder.UUEncode(chunk[1] << 2 | chunk[2] >> 6);
                        buffer[bufferSize++] = UUEncoder.UUEncode(chunk[2]);
                        rawCount += chunkIndex;
                        chunkIndex = 0;
                    }
                    if (bufferSize == 61 || flush && inputSize == 0) {
                        buffer[0] = UUEncoder.UUEncode(rawCount);
                        buffer[bufferSize++] = 13;
                        buffer[bufferSize++] = 10;
                        outLineReady = true;
                        break;
                    }
                }
            } while (outLineReady);
            inputUsed = inputIndex - inputUsed;
            outputUsed = outputIndex - outputUsed;
            completed = inputSize == 0 && (!flush || 0 == bufferSize);
            if (!flush || !completed)
                return;
            numLines = 0;
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            var num = ((dataCount + 3)/3*4 + 60)/60*63;
            if (fileName != null)
                num = num + (Prologue.Length + LineWrap.Length) + fileName.Length + Epilogue.Length;
            return num;
        }

        public override sealed void Reset() {
            numLines = 0;
            bufferIndex = 0;
            bufferSize = 0;
            rawCount = 0;
            chunkIndex = 0;
        }

        public override sealed ByteEncoder Clone() {
            var uuEncoder = this.MemberwiseClone() as UUEncoder;
            uuEncoder.buffer = buffer.Clone() as byte[];
            uuEncoder.chunk = chunk.Clone() as byte[];
            if (fileName != null)
                uuEncoder.fileName = fileName.Clone() as byte[];
            return uuEncoder;
        }

        private static byte UUEncode(int c) {
            return (c & 63) != 0 ? (byte) ((c & 63) + 32) : (byte) 96;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private const int MaximumCharacters = 60;
        private const int DecodedBlockSize = 3;
        private const int EncodedBlockSize = 4;
        private const int MaxFileNameLength = 48;
        private static readonly byte[] Prologue = System.Text.Encoding.ASCII.GetBytes("begin 600 ");
        private static readonly byte[] Epilogue = System.Text.Encoding.ASCII.GetBytes("end\r\n");
        private byte[] buffer = new byte[63];
        private int bufferIndex;
        private int bufferSize;
        private byte[] chunk = new byte[3];
        private int chunkIndex;
        private byte[] fileName;
        private int numLines;
        private bool outLineReady;
        private int rawCount;

    }

}