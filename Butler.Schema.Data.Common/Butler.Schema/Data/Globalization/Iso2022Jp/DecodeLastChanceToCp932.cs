// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.DecodeLastChanceToCp932
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal class DecodeLastChanceToCp932 : DecodeToCp932
  {
    public override char Abbreviation
    {
      get
      {
        return 'L';
      }
    }

    public override bool IsEscapeSequenceHandled(Escape escape)
    {
      return true;
    }

    public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
    {
      usedIn = 0;
      usedOut = 0;
      int index = offsetIn;
      int num1 = 0;
      int count = 0;
      int limit = this.CalculateLoopCountLimit(lengthIn);
      if (escape.IsValidEscapeSequence)
      {
        if (!this.IsEscapeSequenceHandled(escape))
          throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", (object) escape.Sequence));
        index += escape.BytesInCurrentBuffer;
      }
      else if (escape.Sequence == EscapeSequence.NotRecognized)
      {
        index += escape.BytesInCurrentBuffer;
        num1 += escape.BytesInCurrentBuffer;
      }
      while (index < offsetIn + lengthIn)
      {
        this.CheckLoopCount(ref count, limit);
        byte num2 = dataIn[index];
        if ((int) num2 != 27 && (int) num2 != 15 && ((int) num2 != 14 && (int) num2 != 0))
        {
          if ((int) num2 == 160)
          {
            ++index;
            ++num1;
          }
          else
          {
            if ((int) num2 >= 129 && (int) num2 <= 159 || (int) num2 >= 224 && (int) num2 <= 252)
            {
              ++index;
              ++num1;
              if (index >= offsetIn + lengthIn || (int) dataIn[index] == 0)
                break;
            }
            ++index;
            ++num1;
          }
        }
        else
          break;
      }
      usedIn = index - offsetIn;
      usedOut = num1;
      return ValidationResult.Valid;
    }

    public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete)
    {
      usedIn = 0;
      usedOut = 0;
      complete = false;
      int index1 = offsetIn;
      int num1 = offsetOut;
      int num2 = 0;
      int count = 0;
      int limit = this.CalculateLoopCountLimit(lengthIn);
      if (escape.IsValidEscapeSequence)
      {
        if (!this.IsEscapeSequenceHandled(escape))
          throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", (object) escape.Sequence));
        index1 += escape.BytesInCurrentBuffer;
      }
      else if (escape.Sequence == EscapeSequence.NotRecognized)
      {
        for (int index2 = 0; index2 < escape.BytesInCurrentBuffer && index2 < lengthIn; ++index2)
        {
          if (num1 < offsetOut + lengthOut)
          {
            dataOut[num1++] = dataIn[index1 + index2];
            ++num2;
          }
        }
        index1 += escape.BytesInCurrentBuffer;
      }
      else if (escape.Sequence == EscapeSequence.Incomplete)
        index1 += escape.BytesInCurrentBuffer;
      while (index1 < offsetIn + lengthIn)
      {
        this.CheckLoopCount(ref count, limit);
        byte num3 = dataIn[index1];
        if ((int) num3 != 27 && (int) num3 != 15 && ((int) num3 != 14 && (int) num3 != 0))
        {
          if ((int) num3 == 160)
          {
            if (num1 + 1 <= offsetOut + lengthOut)
            {
              dataOut[num1++] = (byte) 32;
              ++index1;
              ++num2;
            }
            else
              break;
          }
          else if (((int) num3 >= 129 && (int) num3 <= 159 || (int) num3 >= 224 && (int) num3 <= 252) && (index1 + 2 <= offsetIn + lengthIn && num1 + 2 <= offsetOut + lengthOut))
          {
            byte[] numArray1 = dataOut;
            int index2 = num1;
            int num4 = 1;
            int num5 = index2 + num4;
            int num6 = (int) dataIn[index1++];
            numArray1[index2] = (byte) num6;
            ++num2;
            if ((int) dataIn[index1] != 0)
            {
              byte[] numArray2 = dataOut;
              int index3 = num5;
              int num7 = 1;
              num1 = index3 + num7;
              int num8 = (int) dataIn[index1++];
              numArray2[index3] = (byte) num8;
              ++num2;
            }
            else
              break;
          }
          else if (num1 + 1 <= offsetOut + lengthOut)
          {
            dataOut[num1++] = dataIn[index1++];
            ++num2;
          }
          else
            break;
        }
        else
          break;
      }
      usedIn = index1 - offsetIn;
      usedOut = num2;
      complete = index1 == offsetIn + lengthIn;
    }
  }
}
