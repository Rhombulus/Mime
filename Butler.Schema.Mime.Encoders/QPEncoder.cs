using System.Linq;

namespace Butler.Schema.Mime.Encoders {

    public class QPEncoder : ByteEncoder {

        public QPEncoder() {}

        public QPEncoder(bool ebcdicDictionary) {
            this.EbcdicDictionary = ebcdicDictionary;
        }

        public bool EbcdicDictionary { get; set; }

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
            inputUsed = 0;
            outputUsed = outputIndex;
            if (encodedSize != 0) {
                var num = System.Math.Min(encodedSize, outputSize);
                if ((num & 4) != 0) {
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                }
                if ((num & 2) != 0) {
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                }
                if ((num & 1) != 0)
                    output[outputIndex++] = encoded[encodedIndex++];
                outputSize -= num;
                encodedSize -= num;
                if (encodedSize != 0) {
                    outputUsed = outputIndex - outputUsed;
                    completed = false;
                    return;
                }
                encodedIndex = 0;
            }
            inputUsed = inputIndex;
            var num1 = inputSize + inputIndex;
            while (num1 != inputIndex || flush && (state != State.Normal || lineOffset != 0)) {
                if (this.IsState(State.ForceSplit)) {
                    if (lineOffset + 1 > 76)
                        throw new System.Exception(Resources.EncodersStrings.QPEncoderNoSpaceForLineBreak);
                    encoded[encodedSize++] = 61;
                    encoded[encodedSize++] = 13;
                    encoded[encodedSize++] = 10;
                    ++lineOffset;
                    lineOffset = 0;
                    state &= ~State.ForceSplit;
                } else if (num1 != inputIndex && input[inputIndex] == 10) {
                    if (this.IsState(State.LastCR)) {
                        if (this.IsState(State.LastWSpCR)) {
                            if (lineOffset + 3 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = 61;
                            encoded[encodedSize++] = NibbleToHex[lastWSp >> 4];
                            encoded[encodedSize++] = NibbleToHex[lastWSp & 15];
                            lineOffset += 3;
                            state &= ~State.LastWSpCR;
                        }
                    } else {
                        if (this.IsState(State.LastWSp)) {
                            if (lineOffset + 1 + 1 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = lastWSp;
                            ++lineOffset;
                            state &= ~State.LastWSp;
                        }
                        if (lineOffset + 4 > 76) {
                            state |= State.ForceSplit;
                            continue;
                        }
                        encoded[encodedSize++] = 61;
                        encoded[encodedSize++] = NibbleToHex[0];
                        encoded[encodedSize++] = NibbleToHex[10];
                        encoded[encodedSize++] = 61;
                        lineOffset += 4;
                    }
                    encoded[encodedSize++] = 13;
                    encoded[encodedSize++] = 10;
                    lineOffset = 0;
                    state &= ~State.LastCR;
                    ++inputIndex;
                } else {
                    if (this.IsState(State.LastWSp)) {
                        if (num1 == inputIndex || input[inputIndex] != 13) {
                            if (lineOffset + 1 + 1 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = lastWSp;
                            ++lineOffset;
                            state &= ~State.LastWSp;
                        }
                    } else if (this.IsState(State.LastCR)) {
                        if (this.IsState(State.LastWSpCR)) {
                            if (lineOffset + 1 + 1 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = lastWSp;
                            ++lineOffset;
                            state &= ~State.LastWSpCR;
                        }
                        if (lineOffset + 3 + 1 > 76) {
                            state |= State.ForceSplit;
                            continue;
                        }
                        encoded[encodedSize++] = 61;
                        encoded[encodedSize++] = NibbleToHex[0];
                        encoded[encodedSize++] = NibbleToHex[13];
                        lineOffset += 3;
                        state &= ~State.LastCR;
                    }
                    if (num1 == inputIndex) {
                        if (lineOffset != 0) {
                            state |= State.ForceSplit;
                            continue;
                        }
                    } else {
                        var bT = input[inputIndex];
                        if (QPEncoder.IsSafe(bT)) {
                            if (lineOffset + 1 + 1 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = bT;
                            ++lineOffset;
                        } else if (bT == 13) {
                            state |= State.LastCR;
                            if (State.LastWSp == (state & State.LastWSp)) {
                                state |= State.LastWSpCR;
                                state &= ~State.LastWSp;
                            }
                        } else if (bT == 32 || bT == 9) {
                            state |= State.LastWSp;
                            lastWSp = bT;
                        } else if (QPEncoder.IsQPEncode(bT)) {
                            if (lineOffset + 3 + 1 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = 61;
                            encoded[encodedSize++] = NibbleToHex[bT >> 4];
                            encoded[encodedSize++] = NibbleToHex[bT & 15];
                            lineOffset += 3;
                        } else if (this.EbcdicDictionary) {
                            if (lineOffset + 3 + 1 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = 61;
                            encoded[encodedSize++] = NibbleToHex[bT >> 4];
                            encoded[encodedSize++] = NibbleToHex[bT & 15];
                            lineOffset += 3;
                        } else {
                            if (lineOffset + 1 + 1 > 76) {
                                state |= State.ForceSplit;
                                continue;
                            }
                            encoded[encodedSize++] = bT;
                            ++lineOffset;
                        }
                        ++inputIndex;
                    }
                }
                encodedIndex = 0;
                var num2 = System.Math.Min(encodedSize, outputSize);
                if ((num2 & 4) != 0) {
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                }
                if ((num2 & 2) != 0) {
                    output[outputIndex++] = encoded[encodedIndex++];
                    output[outputIndex++] = encoded[encodedIndex++];
                }
                if ((num2 & 1) != 0)
                    output[outputIndex++] = encoded[encodedIndex++];
                outputSize -= num2;
                encodedSize -= num2;
                if (encodedSize == 0)
                    encodedIndex = 0;
                else
                    break;
            }
            outputUsed = outputIndex - outputUsed;
            inputUsed = inputIndex - inputUsed;
            completed = num1 == inputIndex && encodedSize == 0 && (!flush || State.Normal == state);
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            var num = dataCount*3;
            return num + (num + 76)/76*2;
        }

        public override sealed void Reset() {
            encodedSize = 0;
            lineOffset = 0;
            state = State.Normal;
        }

        public override sealed ByteEncoder Clone() {
            var qpEncoder = this.MemberwiseClone() as QPEncoder;
            qpEncoder.encoded = encoded.Clone() as byte[];
            return qpEncoder;
        }

        private static bool IsQPEncode(byte bT) {
            if (bT < 128) {
                return
                    ~(Tables.CharClasses.WSp | Tables.CharClasses.QPEncode | Tables.CharClasses.QPUnsafe | Tables.CharClasses.QPWSp | Tables.CharClasses.QEncode | Tables.CharClasses.QPhraseUnsafe | Tables.CharClasses.QCommentUnsafe |
                      Tables.CharClasses.Token2047) != (Tables.CharacterTraits[bT] & Tables.CharClasses.QPEncode);
            }
            return true;
        }

        private static bool IsSafe(byte bT) {
            if (bT < 128) {
                return
                    ~(Tables.CharClasses.WSp | Tables.CharClasses.QPEncode | Tables.CharClasses.QPUnsafe | Tables.CharClasses.QPWSp | Tables.CharClasses.QEncode | Tables.CharClasses.QPhraseUnsafe | Tables.CharClasses.QCommentUnsafe |
                      Tables.CharClasses.Token2047) == (Tables.CharacterTraits[bT] & (Tables.CharClasses.QPEncode | Tables.CharClasses.QPUnsafe | Tables.CharClasses.QPWSp));
            }
            return false;
        }

        private bool IsState(State state) {
            return state == (this.state & state);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private const int LineMaximum = 76;
        private byte[] encoded = new byte[7];
        private int encodedIndex;
        private int encodedSize;
        private byte lastWSp;
        private int lineOffset;
        private State state;


        [System.Flags]
        private enum State {

            Normal = 0,
            ForceSplit = 1,
            LastWSp = 2,
            LastCR = 4,
            LastWSpCR = 8

        }

    }

}