using System.Linq;

namespace Butler.Schema.Mime.Encoders {

    public class UUDecoder : ByteEncoder {

        public string FileName {
            get {
                if (fileName != null)
                    return System.Text.Encoding.ASCII.GetString(fileName, 0, fileName.Length);
                return string.Empty;
            }
        }

        public override sealed void Convert(byte[] input, int inputIndex, int inputSize, byte[] output, int outputIndex, int outputSize, bool flush, out int inputUsed, out int outputUsed, out bool completed) {
            if (inputSize != 0) {
                if (input == null)
                    throw new System.ArgumentNullException(nameof(input));
                if (inputIndex < 0 || inputIndex >= input.Length)
                    throw new System.ArgumentOutOfRangeException(nameof(inputIndex));
                if (inputSize < 0 || inputSize > input.Length - inputIndex)
                    throw new System.ArgumentOutOfRangeException(nameof(inputSize));
            }
            if (output == null)
                throw new System.ArgumentNullException(nameof(output));
            if (outputIndex < 0 || outputIndex >= output.Length)
                throw new System.ArgumentOutOfRangeException(nameof(outputIndex));
            if (outputSize < 1 || outputSize > output.Length - outputIndex)
                throw new System.ArgumentOutOfRangeException(nameof(outputSize));
            if (state == State.Starting) {
                lineReady = false;
                encodedSize = 0;
                encodedBytes = 0;
                decodedSize = 0;
                state = State.Prologue;
            }
            var num1 = inputIndex + inputSize;
            inputUsed = inputIndex;
            outputUsed = outputIndex;
            if (decodedSize != 0) {
                var num2 = System.Math.Min(outputSize, decodedSize);
                if ((num2 & 2) != 0) {
                    output[outputIndex++] = decoded[decodedIndex++];
                    output[outputIndex++] = decoded[decodedIndex++];
                }
                if ((num2 & 1) != 0)
                    output[outputIndex++] = decoded[decodedIndex++];
                outputSize -= num2;
                decodedSize -= num2;
                if (decodedSize != 0) {
                    inputUsed = 0;
                    outputUsed = num2;
                    completed = false;
                    return;
                }
                decodedIndex = 0;
            }
            var numArray = new byte[4];
            do {
                if (lineReady) {
                    var num2 = System.Math.Min(outputSize/3, encodedBytes/3);
                    for (var index = 0; index < num2; ++index) {
                        numArray[0] = UUDecoder.UUDecode(encoded[encodedIndex]);
                        numArray[1] = UUDecoder.UUDecode(encoded[encodedIndex + 1]);
                        numArray[2] = UUDecoder.UUDecode(encoded[encodedIndex + 2]);
                        numArray[3] = UUDecoder.UUDecode(encoded[encodedIndex + 3]);
                        output[outputIndex] = (byte) (numArray[0] << 2 | numArray[1] >> 4);
                        output[outputIndex + 1] = (byte) (numArray[1] << 4 | numArray[2] >> 2);
                        output[outputIndex + 2] = (byte) ((uint) numArray[2] << 6 | numArray[3]);
                        encodedIndex += 4;
                        outputIndex += 3;
                    }
                    encodedBytes -= 3*num2;
                    outputSize -= 3*num2;
                    decodedSize = 0;
                    if (0 < outputSize && 0 < encodedBytes) {
                        numArray[0] = UUDecoder.UUDecode(encoded[encodedIndex]);
                        numArray[1] = UUDecoder.UUDecode(encoded[encodedIndex + 1]);
                        numArray[2] = UUDecoder.UUDecode(encoded[encodedIndex + 2]);
                        numArray[3] = UUDecoder.UUDecode(encoded[encodedIndex + 3]);
                        decodedSize = System.Math.Min(encodedBytes, 3);
                        decodedIndex = 0;
                        decoded[0] = (byte) (numArray[0] << 2 | numArray[1] >> 4);
                        decoded[1] = (byte) (numArray[1] << 4 | numArray[2] >> 2);
                        decoded[2] = (byte) ((uint) numArray[2] << 6 | numArray[3]);
                        encodedBytes -= decodedSize;
                        encodedIndex += 4;
                        var num3 = System.Math.Min(outputSize, decodedSize);
                        if ((num3 & 2) != 0) {
                            output[outputIndex++] = decoded[decodedIndex++];
                            output[outputIndex++] = decoded[decodedIndex++];
                        }
                        if ((num3 & 1) != 0)
                            output[outputIndex++] = decoded[decodedIndex++];
                        outputSize -= num3;
                        decodedSize -= num3;
                    }
                    if (encodedBytes == 0) {
                        encodedIndex = 0;
                        encodedSize = 0;
                        lineReady = false;
                    }
                    if (outputSize == 0)
                        break;
                }
                while (num1 != inputIndex || flush && encodedSize != 0) {
                    var num2 = num1 != inputIndex ? input[inputIndex++] : (byte) 10;
                    if (10 != num2) {
                        if (encodedSize == encoded.Length)
                            throw new ByteEncoderException(Resources.EncodersStrings.UUDecoderInvalidData);
                        encoded[encodedSize++] = num2;
                    } else {
                        if (!this.UULineGood())
                            throw new ByteEncoderException(Resources.EncodersStrings.UUDecoderInvalidDataBadLine);
                        if (encodedBytes != 0) {
                            lineReady = true;
                            break;
                        }
                        encodedSize = 0;
                    }
                }
            } while (lineReady);
            inputUsed = inputIndex - inputUsed;
            outputUsed = outputIndex - outputUsed;
            completed = num1 == inputIndex && (!flush || decodedSize == 0 && !lineReady);
            if (!flush || !completed)
                return;
            state = State.Starting;
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            var num = dataCount/63;
            dataCount -= num*3;
            return (dataCount + 4)/4*3;
        }

        public override sealed void Reset() {
            state = State.Starting;
            fileName = null;
        }

        public override sealed ByteEncoder Clone() {
            var uuDecoder = this.MemberwiseClone() as UUDecoder;
            uuDecoder.decoded = decoded.Clone() as byte[];
            uuDecoder.encoded = encoded.Clone() as byte[];
            if (fileName != null)
                uuDecoder.fileName = fileName.Clone() as byte[];
            return uuDecoder;
        }

        private static byte UUDecode(byte c) {
            return (int) c == 96 ? (byte) 0 : (byte) (c - 32 & 63);
        }

        private bool UULineGood() {
            encodedBytes = 0;
            var num1 = encodedSize;
            while (encodedSize > 0 && ByteEncoder.IsWhiteSpace(encoded[encodedSize - 1]))
                --encodedSize;
            if (encodedSize == 0)
                return true;
            if (state == State.Prologue) {
                if (ByteEncoder.BeginsWithNI(encoded, 0, Prologue, encodedSize)) {
                    var count = encodedSize - 6;
                    int offset;
                    for (offset = 6; count != 0 && ByteEncoder.IsWhiteSpace(encoded[offset]); --count) {
                        ++offset;
                    }
                    if (count == 0 || encoded[offset] < 48 || encoded[offset] > 55)
                        return true;
                    do {
                        ++offset;
                        --count;
                    } while (count != 0 && encoded[offset] >= 48 && encoded[offset] <= 55);
                    if (count == 0 || !ByteEncoder.IsWhiteSpace(encoded[offset]))
                        return true;
                    do {
                        ++offset;
                        --count;
                    } while (count != 0 && ByteEncoder.IsWhiteSpace(encoded[offset]));
                    if (count <= 128) {
                        fileName = new byte[count];
                        ByteEncoder.BlockCopy(encoded, offset, fileName, 0, count);
                    }
                    return true;
                }
            } else if (state == State.Ending || ByteEncoder.BeginsWithNI(encoded, 0, Prologue, encodedSize))
                return true;
            if (true) {
                int num2 = UUDecoder.UUDecode(encoded[0]);
                var num3 = 0;
                var num4 = num2%3;
                if (num4 != 0) {
                    ++num4;
                    num3 = 4 - num4;
                }
                var index = 4*(num2/3) + num4 + 1;
                if (encodedSize < index) {
                    if (num1 >= index)
                        encodedSize = index;
                    else
                        goto label_36;
                }
                if (index != encodedSize && index + num3 != encodedSize) {
                    if (index + 1 == encodedSize || index + num3 + 1 == encodedSize)
                        --encodedSize;
                    else if (index - 1 == encodedSize || index + num3 - 1 != encodedSize)
                        goto label_36;
                    else
                        goto label_36;
                }
                encodedBytes = num2;
                if (num3 != 0 && index == encodedSize) {
                    encoded[index] = 32;
                    if (num3 > 1)
                        encoded[index + 1] = 32;
                    encodedSize += num3;
                }
                --encodedSize;
                encodedIndex = 1;
                state = encodedBytes != 0 ? State.Data : State.Ending;
                return true;
            }
            label_36:
            if (!ByteEncoder.BeginsWithNI(encoded, 0, Epilogue, encodedSize))
                return false;
            state = State.Ending;
            return true;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private const int EncodedBlockSize = 4;
        private const int DecodedBlockSize = 3;
        private const int MaximumBytes = 128;
        private static readonly byte[] Prologue = System.Text.Encoding.ASCII.GetBytes("begin ");
        private static readonly byte[] Epilogue = System.Text.Encoding.ASCII.GetBytes("end");
        private byte[] decoded = new byte[3];
        private int decodedIndex;
        private int decodedSize;
        private byte[] encoded = new byte[130];
        private int encodedBytes;
        private int encodedIndex;
        private int encodedSize;
        private byte[] fileName;
        private bool lineReady;
        private State state;


        private enum State {

            Starting,
            Prologue,
            Data,
            Ending

        }

    }

}