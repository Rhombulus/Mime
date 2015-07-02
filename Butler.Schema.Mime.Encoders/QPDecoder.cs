using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public class QPDecoder : ByteEncoder {

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
            outputUsed = outputIndex;
            inputUsed = inputIndex;
            var num1 = inputIndex + inputSize;
            while (inputIndex != num1 && outputSize != 0) {
                if (encodedIndex != 0) {
                    do {
                        var num2 = input[inputIndex++];
                        if (32 != num2 && 9 != num2) {
                            encoded[encodedIndex++] = num2;
                            if (encodedIndex == 3) {
                                encodedIndex = 0;
                                var num3 = encoded[1];
                                var num4 = encoded[2];
                                if (num3 != 13 || num4 != 10) {
                                    var num5 = Tables.NumFromHex[num3];
                                    var num6 = Tables.NumFromHex[num4];
                                    var num7 = (int) num5 == (int) byte.MaxValue || (int) num6 == (int) byte.MaxValue ? (byte) 61 : (byte) ((uint) num5 << 4 | num6);
                                    output[outputIndex++] = num7;
                                    if (--outputSize == 0) {
                                        inputUsed = inputIndex - inputUsed;
                                        outputUsed = outputIndex - outputUsed;
                                        completed = inputIndex == num1;
                                        return;
                                    }
                                }
                                break;
                            }
                        }
                    } while (inputIndex != num1);
                }
                if (inputIndex != num1) {
                    var count = System.Math.Min(num1 - inputIndex, outputSize);
                    var num2 = Internal.ByteString.IndexOf(input, 61, inputIndex, count);
                    int num3;
                    if (input.GetLowerBound(0) - 1 == num2)
                        num3 = inputIndex + count;
                    else {
                        count = num2 - inputIndex;
                        encoded[encodedIndex++] = 61;
                        num3 = num2 + 1;
                    }
                    if (0 < count) {
                        ByteEncoder.BlockCopy(input, inputIndex, output, outputIndex, count);
                        outputSize -= count;
                        outputIndex += count;
                    }
                    inputIndex = num3;
                }
            }
            if (flush && encodedIndex != 0 && inputIndex == num1) {
                if (encodedIndex == 1 && 61 == encoded[0] || encoded[1] == 13)
                    encodedIndex = 0;
                else if (0 < outputSize) {
                    output[outputIndex++] = encoded[0];
                    if (0 < --outputSize) {
                        if (1 < encodedIndex)
                            output[outputIndex++] = encoded[1];
                        encodedIndex = 0;
                    } else if (1 < encodedIndex) {
                        encoded[0] = encoded[1];
                        --encodedIndex;
                    }
                }
            }
            inputUsed = inputIndex - inputUsed;
            outputUsed = outputIndex - outputUsed;
            completed = inputIndex == num1 && (!flush || 0 == encodedIndex);
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            return dataCount;
        }

        public override sealed void Reset() {
            encodedIndex = 0;
        }

        public override sealed ByteEncoder Clone() {
            var qpDecoder = this.MemberwiseClone() as QPDecoder;
            qpDecoder.encoded = encoded.Clone() as byte[];
            return qpDecoder;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private byte[] encoded = new byte[3];
        private int encodedIndex;

    }

}