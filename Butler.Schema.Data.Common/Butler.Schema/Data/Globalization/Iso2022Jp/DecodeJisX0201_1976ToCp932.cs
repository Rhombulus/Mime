// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.DecodeJisX0201_1976ToCp932
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal class DecodeJisX0201_1976ToCp932 : DecodeToCp932
  {
    private bool isKana;
    private bool isEscapeKana;
    private bool runBeganWithEscape;

    public override char Abbreviation
    {
      get
      {
        return 'k';
      }
    }

    public override bool IsEscapeSequenceHandled(Escape escape)
    {
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
        public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
        {
            usedIn = 0;
            usedOut = 0;
            int index = offsetIn;
            int num2 = 0;
            int count = 0;
            int limit = this.CalculateLoopCountLimit(lengthIn);
            if (escape.IsValidEscapeSequence)
            {
                if (!this.IsEscapeSequenceHandled(escape))
                {
                    throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
                }
                index += escape.BytesInCurrentBuffer;
                this.runBeganWithEscape = true;
            }
            else if (!this.runBeganWithEscape)
            {
                return ValidationResult.Invalid;
            }
            while (index < (offsetIn + lengthIn))
            {
                this.CheckLoopCount(ref count, limit);
                byte num5 = dataIn[index];
                switch (num5)
                {
                    case 0x1b:
                    case 0:
                        goto Label_0094;
                }
                if ((num5 != 14) && (num5 != 15))
                {
                    num2++;
                }
                index++;
            }
            Label_0094:
            usedIn = index - offsetIn;
            usedOut = num2;
            return ValidationResult.Valid;
        }

        public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete)
    {
      usedIn = 0;
      usedOut = 0;
      complete = false;
      int index1 = offsetIn;
      int index2 = offsetOut;
      int num1 = 0;
      int count = 0;
      int limit = this.CalculateLoopCountLimit(lengthIn);
      if (escape.IsValidEscapeSequence)
      {
        if (!this.IsEscapeSequenceHandled(escape))
          throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", (object) escape.Sequence));
        index1 += escape.BytesInCurrentBuffer;
        this.isKana = escape.Sequence == EscapeSequence.JisX0201K_1976 || escape.Sequence == EscapeSequence.ShiftOut;
        this.isEscapeKana = escape.Sequence == EscapeSequence.JisX0201K_1976;
      }
      for (; index1 < offsetIn + lengthIn; ++index1)
      {
        this.CheckLoopCount(ref count, limit);
        byte num2 = dataIn[index1];
        switch (num2)
        {
          case (byte) 27:
          case (byte) 0:
            goto label_16;
          case (byte) 14:
            this.isKana = true;
            break;
          case (byte) 15:
            this.isKana = this.isEscapeKana;
            break;
          default:
            if ((int) num2 < 128)
            {
              dataOut[index2] = !this.isKana || (int) num2 < 33 || (int) num2 > 95 ? num2 : (byte) ((uint) num2 + 128U);
              ++index2;
              ++num1;
              break;
            }
            if ((int) num2 >= 161 && (int) num2 <= 223)
            {
              dataOut[index2] = num2;
              ++index2;
              ++num1;
              break;
            }
            if ((int) num2 == 160)
            {
              dataOut[index2] = (byte) 32;
              ++index2;
              ++num1;
              break;
            }
            dataOut[index2] = (byte) 63;
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

    public override void Reset()
    {
      this.isKana = false;
      this.isEscapeKana = false;
      this.runBeganWithEscape = false;
    }
  }
}
