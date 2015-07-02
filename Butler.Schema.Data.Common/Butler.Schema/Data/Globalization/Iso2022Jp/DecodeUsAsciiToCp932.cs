// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.DecodeUsAsciiToCp932
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal class DecodeUsAsciiToCp932 : DecodeToCp932
  {
    public override char Abbreviation => 'a';

      public override bool IsEscapeSequenceHandled(Escape escape)
    {
      return escape.Sequence == EscapeSequence.Iso646Irv;
    }

    public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
    {
      usedIn = 0;
      usedOut = 0;
      int index = offsetIn;
      int num1 = 0;
      bool flag = false;
      bool validEscapeSequence = escape.IsValidEscapeSequence;
      int count = 0;
      int limit = this.CalculateLoopCountLimit(lengthIn);
      if (validEscapeSequence)
      {
        if (!this.IsEscapeSequenceHandled(escape))
          throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", (object) escape.Sequence));
        index += escape.BytesInCurrentBuffer;
      }
      while (index < offsetIn + lengthIn)
      {
        this.CheckLoopCount(ref count, limit);
        byte num2 = dataIn[index];
        switch (num2)
        {
          case (byte) 27:
          case (byte) 15:
          case (byte) 14:
            goto label_10;
          default:
            if (((int) num2 <= (int) sbyte.MaxValue || validEscapeSequence) && (int) num2 != 0)
            {
              if (((int) num2 < 32 || (int) num2 > (int) sbyte.MaxValue) && ((int) num2 != 9 && (int) num2 != 10) && ((int) num2 != 11 && (int) num2 != 12 && (int) num2 != 13))
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

    public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete)
    {
      usedIn = 0;
      usedOut = 0;
      int index = offsetIn;
      int num = offsetOut;
      int count = 0;
      int limit = this.CalculateLoopCountLimit(lengthIn);
      if (escape.IsValidEscapeSequence)
      {
        if (!this.IsEscapeSequenceHandled(escape))
          throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", (object) escape.Sequence));
        index += escape.BytesInCurrentBuffer;
      }
      for (; index < offsetIn + lengthIn; dataOut[num++] = dataIn[index++])
      {
        this.CheckLoopCount(ref count, limit);
        switch (dataIn[index])
        {
          case (byte) 27:
          case (byte) 15:
          case (byte) 14:
          case (byte) 0:
            goto label_9;
          default:
            if (num + 1 > offsetOut + lengthOut)
              throw new InvalidOperationException(string.Format("DecodeUsAsciiToCp932.ConvertToCp932: output buffer overrun, offset {0}, length {1}", (object) offsetOut, (object) lengthOut));
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
