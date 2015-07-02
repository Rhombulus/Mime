namespace Butler.Schema.Data.Globalization.Iso2022Jp {

    internal class DecodeLastChanceToCp932 : DecodeToCp932 {

        public override char Abbreviation => 'L';

        public override bool IsEscapeSequenceHandled(Escape escape) {
            return true;
        }

        public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut) {
            usedIn = 0;
            usedOut = 0;
            var index = offsetIn;
            var num1 = 0;
            var count = 0;
            var limit = this.CalculateLoopCountLimit(lengthIn);
            if (escape.IsValidEscapeSequence) {
                if (!this.IsEscapeSequenceHandled(escape))
                    throw new System.InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
                index += escape.BytesInCurrentBuffer;
            } else if (escape.Sequence == EscapeSequence.NotRecognized) {
                index += escape.BytesInCurrentBuffer;
                num1 += escape.BytesInCurrentBuffer;
            }
            while (index < offsetIn + lengthIn) {
                this.CheckLoopCount(ref count, limit);
                var num2 = dataIn[index];
                if (num2 != 27 && num2 != 15 && (num2 != 14 && num2 != 0)) {
                    if (num2 == 160) {
                        ++index;
                        ++num1;
                    } else {
                        if (num2 >= 129 && num2 <= 159 || num2 >= 224 && num2 <= 252) {
                            ++index;
                            ++num1;
                            if (index >= offsetIn + lengthIn || dataIn[index] == 0)
                                break;
                        }
                        ++index;
                        ++num1;
                    }
                } else
                    break;
            }
            usedIn = index - offsetIn;
            usedOut = num1;
            return ValidationResult.Valid;
        }

        public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete) {
            usedIn = 0;
            usedOut = 0;
            complete = false;
            var index1 = offsetIn;
            var num1 = offsetOut;
            var num2 = 0;
            var count = 0;
            var limit = this.CalculateLoopCountLimit(lengthIn);
            if (escape.IsValidEscapeSequence) {
                if (!this.IsEscapeSequenceHandled(escape))
                    throw new System.InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
                index1 += escape.BytesInCurrentBuffer;
            } else if (escape.Sequence == EscapeSequence.NotRecognized) {
                for (var index2 = 0; index2 < escape.BytesInCurrentBuffer && index2 < lengthIn; ++index2) {
                    if (num1 < offsetOut + lengthOut) {
                        dataOut[num1++] = dataIn[index1 + index2];
                        ++num2;
                    }
                }
                index1 += escape.BytesInCurrentBuffer;
            } else if (escape.Sequence == EscapeSequence.Incomplete)
                index1 += escape.BytesInCurrentBuffer;
            while (index1 < offsetIn + lengthIn) {
                this.CheckLoopCount(ref count, limit);
                var num3 = dataIn[index1];
                if (num3 != 27 && num3 != 15 && (num3 != 14 && num3 != 0)) {
                    if (num3 == 160) {
                        if (num1 + 1 <= offsetOut + lengthOut) {
                            dataOut[num1++] = 32;
                            ++index1;
                            ++num2;
                        } else
                            break;
                    } else if ((num3 >= 129 && num3 <= 159 || num3 >= 224 && num3 <= 252) && (index1 + 2 <= offsetIn + lengthIn && num1 + 2 <= offsetOut + lengthOut)) {
                        var numArray1 = dataOut;
                        var index2 = num1;
                        var num4 = 1;
                        var num5 = index2 + num4;
                        int num6 = dataIn[index1++];
                        numArray1[index2] = (byte) num6;
                        ++num2;
                        if (dataIn[index1] != 0) {
                            var numArray2 = dataOut;
                            var index3 = num5;
                            var num7 = 1;
                            num1 = index3 + num7;
                            int num8 = dataIn[index1++];
                            numArray2[index3] = (byte) num8;
                            ++num2;
                        } else
                            break;
                    } else if (num1 + 1 <= offsetOut + lengthOut) {
                        dataOut[num1++] = dataIn[index1++];
                        ++num2;
                    } else
                        break;
                } else
                    break;
            }
            usedIn = index1 - offsetIn;
            usedOut = num2;
            complete = index1 == offsetIn + lengthIn;
        }

    }

}