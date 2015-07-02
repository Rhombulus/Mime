namespace Butler.Schema.Data.Globalization.Iso2022Jp {

    internal class DecodeUsAsciiToCp932 : DecodeToCp932 {

        public override char Abbreviation => 'a';

        public override bool IsEscapeSequenceHandled(Escape escape) {
            return escape.Sequence == EscapeSequence.Iso646Irv;
        }

        public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut) {
            usedIn = 0;
            usedOut = 0;
            var index = offsetIn;
            var num1 = 0;
            var flag = false;
            var validEscapeSequence = escape.IsValidEscapeSequence;
            var count = 0;
            var limit = this.CalculateLoopCountLimit(lengthIn);
            if (validEscapeSequence) {
                if (!this.IsEscapeSequenceHandled(escape))
                    throw new System.InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
                index += escape.BytesInCurrentBuffer;
            }
            while (index < offsetIn + lengthIn) {
                this.CheckLoopCount(ref count, limit);
                var num2 = dataIn[index];
                switch (num2) {
                    case 27:
                    case 15:
                    case 14:
                        goto label_10;
                    default:
                        if ((num2 <= sbyte.MaxValue || validEscapeSequence) && num2 != 0) {
                            if ((num2 < 32 || num2 > sbyte.MaxValue) && (num2 != 9 && num2 != 10) && (num2 != 11 && num2 != 12 && num2 != 13))
                                flag = true;
                            ++index;
                            ++num1;
                            continue;
                        }
                        goto label_10;
                }
            }
            label_10:
            usedIn = index - offsetIn;
            usedOut = num1;
            return !flag || validEscapeSequence ? ValidationResult.Valid : ValidationResult.Invalid;
        }

        public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete) {
            usedIn = 0;
            usedOut = 0;
            var index = offsetIn;
            var num = offsetOut;
            var count = 0;
            var limit = this.CalculateLoopCountLimit(lengthIn);
            if (escape.IsValidEscapeSequence) {
                if (!this.IsEscapeSequenceHandled(escape))
                    throw new System.InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
                index += escape.BytesInCurrentBuffer;
            }
            for (; index < offsetIn + lengthIn; dataOut[num++] = dataIn[index++]) {
                this.CheckLoopCount(ref count, limit);
                switch (dataIn[index]) {
                    case 27:
                    case 15:
                    case 14:
                    case 0:
                        goto label_9;
                    default:
                        if (num + 1 > offsetOut + lengthOut)
                            throw new System.InvalidOperationException(string.Format("DecodeUsAsciiToCp932.ConvertToCp932: output buffer overrun, offset {0}, length {1}", offsetOut, lengthOut));
                        continue;
                }
            }
            label_9:
            complete = index == offsetIn + lengthIn;
            usedIn = index - offsetIn;
            usedOut = num - offsetOut;
        }

    }

}