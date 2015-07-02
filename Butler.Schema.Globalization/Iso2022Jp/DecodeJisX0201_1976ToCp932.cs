namespace Butler.Schema.Globalization.Iso2022Jp {

    internal class DecodeJisX0201_1976ToCp932 : DecodeToCp932 {

        public override char Abbreviation => 'k';

        public override bool IsEscapeSequenceHandled(Escape escape) {
            if (escape.Sequence != EscapeSequence.JisX0201_1976 && escape.Sequence != EscapeSequence.JisX0201K_1976 && escape.Sequence != EscapeSequence.ShiftIn)
                return escape.Sequence == EscapeSequence.ShiftOut;
            return true;
        }

        //    public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
        //    {
        //      usedIn = 0;
        //      usedOut = 0;
        //      int index = offsetIn;
        //      int num = 0;
        //      int count = 0;
        //      int limit = this.CalculateLoopCountLimit(lengthIn);
        //      if (escape.IsValidEscapeSequence)
        //      {
        //        if (!this.IsEscapeSequenceHandled(escape))
        //          throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", (object) escape.Sequence));
        //        index += escape.BytesInCurrentBuffer;
        //        this.runBeganWithEscape = true;
        //        goto case (byte) 14;
        //      }
        //      else
        //      {
        //        if (!this.runBeganWithEscape)
        //          return ValidationResult.Invalid;
        //        goto case (byte) 14;
        //      }
        //      for (; index < offsetIn + lengthIn; ++index)
        //      {
        //        this.CheckLoopCount(ref count, limit);
        //        switch (dataIn[index])
        //        {
        //          case (byte) 27:
        //          case (byte) 0:
        //            goto label_10;
        //          case (byte) 14:
        //          case (byte) 15:
        //            goto case (byte) 14;
        //          default:
        //            ++num;
        //            goto case (byte) 14;
        //        }
        //      }
        //label_10:
        //      usedIn = index - offsetIn;
        //      usedOut = num;
        //      return ValidationResult.Valid;
        //    }
        public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut) {
            usedIn = 0;
            usedOut = 0;
            var index = offsetIn;
            var num2 = 0;
            var count = 0;
            var limit = this.CalculateLoopCountLimit(lengthIn);
            if (escape.IsValidEscapeSequence) {
                if (!this.IsEscapeSequenceHandled(escape))
                    throw new System.InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
                index += escape.BytesInCurrentBuffer;
                runBeganWithEscape = true;
            } else if (!runBeganWithEscape)
                return ValidationResult.Invalid;
            while (index < (offsetIn + lengthIn)) {
                this.CheckLoopCount(ref count, limit);
                var num5 = dataIn[index];
                switch (num5) {
                    case 0x1b:
                    case 0:
                        goto Label_0094;
                }
                if ((num5 != 14) && (num5 != 15))
                    num2++;
                index++;
            }
            Label_0094:
            usedIn = index - offsetIn;
            usedOut = num2;
            return ValidationResult.Valid;
        }

        public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete) {
            usedIn = 0;
            usedOut = 0;
            complete = false;
            var index1 = offsetIn;
            var index2 = offsetOut;
            var num1 = 0;
            var count = 0;
            var limit = this.CalculateLoopCountLimit(lengthIn);
            if (escape.IsValidEscapeSequence) {
                if (!this.IsEscapeSequenceHandled(escape))
                    throw new System.InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
                index1 += escape.BytesInCurrentBuffer;
                isKana = escape.Sequence == EscapeSequence.JisX0201K_1976 || escape.Sequence == EscapeSequence.ShiftOut;
                isEscapeKana = escape.Sequence == EscapeSequence.JisX0201K_1976;
            }
            for (; index1 < offsetIn + lengthIn; ++index1) {
                this.CheckLoopCount(ref count, limit);
                var num2 = dataIn[index1];
                switch (num2) {
                    case 27:
                    case 0:
                        goto label_16;
                    case 14:
                        isKana = true;
                        break;
                    case 15:
                        isKana = isEscapeKana;
                        break;
                    default:
                        if (num2 < 128) {
                            dataOut[index2] = !isKana || (int) num2 < 33 || (int) num2 > 95 ? num2 : (byte) (num2 + 128U);
                            ++index2;
                            ++num1;
                            break;
                        }
                        if (num2 >= 161 && num2 <= 223) {
                            dataOut[index2] = num2;
                            ++index2;
                            ++num1;
                            break;
                        }
                        if (num2 == 160) {
                            dataOut[index2] = 32;
                            ++index2;
                            ++num1;
                            break;
                        }
                        dataOut[index2] = 63;
                        ++index2;
                        ++num1;
                        break;
                }
            }
            label_16:
            complete = index1 == offsetIn + lengthIn;
            usedIn = index1 - offsetIn;
            usedOut = num1;
        }

        public override void Reset() {
            isKana = false;
            isEscapeKana = false;
            runBeganWithEscape = false;
        }

        private bool isEscapeKana;
        private bool isKana;
        private bool runBeganWithEscape;

    }

}