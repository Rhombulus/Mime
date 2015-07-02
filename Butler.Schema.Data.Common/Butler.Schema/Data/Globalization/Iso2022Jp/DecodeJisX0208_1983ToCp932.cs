// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.DecodeJisX0208_1983ToCp932
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal class DecodeJisX0208_1983ToCp932 : DecodeToCp932
  {
    private JisX0208PairClass isRunContainsIbmExtension = JisX0208PairClass.Unrecognized;
    private bool isKana;
    private byte leftoverByte;
    private bool runBeganWithEscape;

    public override char Abbreviation
    {
      get
      {
        return 'e';
      }
    }

    public override bool IsEscapeSequenceHandled(Escape escape)
    {
      if (escape.Sequence != EscapeSequence.JisX0208_1978 && escape.Sequence != EscapeSequence.JisX0208_1983)
        return escape.Sequence == EscapeSequence.JisX0208_1990;
      return true;
    }

    public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
    {
      usedIn = 0;
      usedOut = 0;
      bool flag1 = false;
      bool flag2 = false;
      int index = offsetIn;
      int num1 = 0;
      int count = 0;
      int limit = this.CalculateLoopCountLimit(lengthIn);
      byte num2 = (byte) 0;
      if (escape.IsValidEscapeSequence)
      {
        if (!this.IsEscapeSequenceHandled(escape))
          throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", (object) escape.Sequence));
        index += escape.BytesInCurrentBuffer;
        this.runBeganWithEscape = true;
        this.isKana = false;
        this.leftoverByte = (byte) 0;
      }
      while (index < offsetIn + lengthIn)
      {
        this.CheckLoopCount(ref count, limit);
        uint high;
        uint low;
        int num3;
        if ((int) this.leftoverByte != 0 && index == offsetIn)
        {
          high = (uint) this.leftoverByte;
          low = (uint) dataIn[index];
          num3 = 1;
          num2 = this.leftoverByte;
          this.leftoverByte = (byte) 0;
        }
        else if (index + 2 <= offsetIn + lengthIn)
        {
          high = (uint) dataIn[index];
          low = (uint) dataIn[index + 1];
          num3 = 2;
        }
        else
        {
          uint num4 = (uint) dataIn[index];
          if ((int) num4 == 27 || !this.runBeganWithEscape && ((int) num4 == 14 || (int) num4 == 15) || (int) num4 == 0)
          {
            flag2 = true;
            break;
          }
          if (this.isKana && (int) num4 != 14 && (int) num4 != 15)
          {
            ++index;
            ++num1;
            break;
          }
          break;
        }
        if ((int) high == 27)
        {
          flag2 = true;
          break;
        }
        if ((int) high == 0)
        {
          flag2 = true;
          break;
        }
        if ((int) low == 27)
        {
          if ((int) high == 14 || (int) high == 15)
          {
            if (this.runBeganWithEscape)
              ++index;
          }
          else
          {
            ++index;
            if (this.isKana)
              ++num1;
          }
          flag2 = true;
          break;
        }
        if ((int) low == 0)
        {
          if ((int) high == 14 || (int) high == 15)
          {
            if (this.runBeganWithEscape)
              ++index;
          }
          else
          {
            ++index;
            if (this.isKana)
              ++num1;
          }
          flag2 = true;
          break;
        }
        if (!this.runBeganWithEscape)
        {
          if ((int) high == 14 || (int) high == 15 || ((int) low == 14 || (int) low == 15))
          {
            flag2 = true;
            break;
          }
        }
        else
        {
          if ((int) high == 14)
          {
            this.isKana = true;
            index += num3;
            if ((int) low != 14 && (int) low != 15)
            {
              ++num1;
              continue;
            }
            continue;
          }
          if ((int) high == 15)
          {
            this.isKana = false;
            index += num3 - 1;
            continue;
          }
          if ((int) low == 14)
          {
            index += num3;
            ++num1;
            this.isKana = true;
            continue;
          }
          if ((int) low == 15)
          {
            this.isKana = false;
            index += num3;
            ++num1;
            continue;
          }
        }
        if (high >= 128U)
        {
          flag1 = true;
          if (this.runBeganWithEscape)
          {
            if (this.isRunContainsIbmExtension == JisX0208PairClass.Unrecognized && this.ClassifyPair(high, low) == JisX0208PairClass.IbmExtension)
              this.isRunContainsIbmExtension = JisX0208PairClass.IbmExtension;
          }
          else
            break;
        }
        index += num3;
        num1 += 2;
      }
      if (!flag2 && index + 1 == offsetIn + lengthIn)
        ++index;
      usedIn = index - offsetIn;
      usedOut = num1;
      if ((int) num2 != 0)
        this.leftoverByte = num2;
      return !flag1 || this.runBeganWithEscape ? ValidationResult.Valid : ValidationResult.Invalid;
    }

    public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete)
    {
      usedIn = 0;
      usedOut = 0;
      complete = false;
      bool flag = false;
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
        this.isKana = false;
        this.leftoverByte = (byte) 0;
      }
      while (index1 < offsetIn + lengthIn)
      {
        this.CheckLoopCount(ref count, limit);
        uint num2;
        uint num3;
        int num4;
        int num5;
        if ((int) this.leftoverByte != 0)
        {
          if (index1 != offsetIn)
            throw new InvalidOperationException(string.Format("DecodeJisX0208_1983ToCp932.ConvertToCp932: leftover byte processed at offset {0}, should have been {1}", (object) index1, (object) offsetIn));
          num2 = (uint) this.leftoverByte;
          num3 = (uint) dataIn[index1];
          num4 = 1;
          this.leftoverByte = (byte) 0;
        }
        else if (index1 + 2 <= offsetIn + lengthIn)
        {
          num2 = (uint) dataIn[index1];
          num3 = (uint) dataIn[index1 + 1];
          num4 = 2;
        }
        else
        {
          uint current = (uint) dataIn[index1];
          if ((int) current == 27 || !this.runBeganWithEscape && ((int) current == 14 || (int) current == 15) || (int) current == 0)
          {
            flag = true;
            break;
          }
          if (this.isKana && (int) current != 14 && (int) current != 15)
          {
            byte[] numArray = dataOut;
            int index3 = index2;
            int num6 = 1;
            num5 = index3 + num6;
            int num7 = (int) this.DecodeKana(current);
            numArray[index3] = (byte) num7;
            ++num1;
            ++index1;
            break;
          }
          break;
        }
        if ((int) num2 == 27)
        {
          this.runBeganWithEscape = false;
          flag = true;
          break;
        }
        if ((int) num2 == 0)
        {
          flag = true;
          break;
        }
        if ((int) num3 == 27)
        {
          this.runBeganWithEscape = false;
          flag = true;
          ++index1;
          if (this.isKana && (int) num2 != 14)
          {
            if ((int) num2 == 15)
            {
              this.isKana = false;
              break;
            }
            byte[] numArray = dataOut;
            int index3 = index2;
            int num6 = 1;
            num5 = index3 + num6;
            int num7 = (int) this.DecodeKana(num2);
            numArray[index3] = (byte) num7;
            ++num1;
            break;
          }
          break;
        }
        if ((int) num3 == 0)
        {
          flag = true;
          ++index1;
          if (this.isKana && (int) num2 != 14)
          {
            if ((int) num2 == 15)
            {
              this.isKana = false;
              break;
            }
            byte[] numArray = dataOut;
            int index3 = index2;
            int num6 = 1;
            num5 = index3 + num6;
            int num7 = (int) this.DecodeKana(num2);
            numArray[index3] = (byte) num7;
            ++num1;
            break;
          }
          break;
        }
        if (!this.runBeganWithEscape)
        {
          if ((int) num2 == 14 || (int) num2 == 15 || ((int) num3 == 14 || (int) num3 == 15))
          {
            flag = true;
            break;
          }
        }
        else if ((int) num2 == 14)
        {
          this.isKana = true;
          index1 += num4;
          switch (num3)
          {
            case 14U:
              continue;
            case 15U:
              this.isKana = false;
              continue;
            default:
              dataOut[index2++] = this.DecodeKana(num3);
              ++num1;
              continue;
          }
        }
        else
        {
          if ((int) num2 == 15)
          {
            this.isKana = false;
            index1 += num4 - 1;
            continue;
          }
          if ((int) num3 == 14)
          {
            index1 += num4;
            dataOut[index2] = this.isKana ? this.DecodeKana(num2) : (byte) 165;
            ++index2;
            ++num1;
            this.isKana = true;
            continue;
          }
          if ((int) num3 == 15)
          {
            dataOut[index2] = this.isKana ? this.DecodeKana(num2) : (byte) 165;
            index1 += num4;
            ++num1;
            ++index2;
            this.isKana = false;
            continue;
          }
        }
        if (index2 + 2 <= offsetOut + lengthOut)
        {
          if (this.isKana)
          {
            dataOut[index2] = this.DecodeKana(num2);
            dataOut[index2 + 1] = this.DecodeKana(num3);
          }
          else if (num2 >= 128U)
          {
            switch (this.ClassifyPair(num2, num3))
            {
              case JisX0208PairClass.Unrecognized:
              case JisX0208PairClass.Cp932:
                dataOut[index2] = (byte) num2;
                dataOut[index2 + 1] = (byte) num3;
                break;
              case JisX0208PairClass.IbmExtension:
                this.isRunContainsIbmExtension = JisX0208PairClass.IbmExtension;
                byte highOut;
                byte lowOut;
                this.MapIbmExtensionToCp932(num2, num3, out highOut, out lowOut);
                dataOut[index2] = highOut;
                dataOut[index2 + 1] = lowOut;
                break;
              case JisX0208PairClass.IbmExtension | JisX0208PairClass.Cp932:
                if (this.isRunContainsIbmExtension != JisX0208PairClass.IbmExtension)
                  goto case JisX0208PairClass.Unrecognized;
                else
                  goto case JisX0208PairClass.IbmExtension;
              default:
                throw new InvalidOperationException("unrecognized pair class, update DecodeJisX0208_1983.DecodeToCp932");
            }
          }
          else
          {
            uint num6 = ((int) num2 & 1) != 1 ? num3 + 125U : num3 + 31U;
            if (num6 >= (uint) sbyte.MaxValue)
              ++num6;
            uint num7 = (num2 - 33U >> 1) + 129U;
            if (num7 > 159U)
              num7 += 64U;
            dataOut[index2] = (byte) num7;
            dataOut[index2 + 1] = (byte) num6;
          }
          index1 += num4;
          index2 += 2;
          num1 += 2;
        }
        else
          break;
      }
      if (!flag && index1 + 1 == offsetIn + lengthIn)
        this.leftoverByte = dataIn[index1++];
      usedIn = index1 - offsetIn;
      usedOut = num1;
      complete = index1 == offsetIn + lengthIn;
    }

    private JisX0208PairClass ClassifyPair(uint high, uint low)
    {
      if (high >= 147U && high < 151U && (low > 32U && low < (uint) sbyte.MaxValue) || (int) high == 151 && low > 32U && low < 45U)
        return low >= 64U && (int) low != (int) sbyte.MaxValue && low < 253U ? JisX0208PairClass.IbmExtension | JisX0208PairClass.Cp932 : JisX0208PairClass.IbmExtension;
      return high < 240U && high > 128U && ((int) high != 160 && low >= 64U) && ((int) low != (int) sbyte.MaxValue && low < 253U) ? JisX0208PairClass.Cp932 : JisX0208PairClass.Unrecognized;
    }

    private void MapIbmExtensionToCp932(uint highIn, uint lowIn, out byte highOut, out byte lowOut)
    {
      highOut = (byte) 0;
      lowOut = (byte) 0;
      switch (highIn)
      {
        case 147U:
          highOut = (byte) 250;
          lowOut = (byte) (lowIn + 31U);
          break;
        case 148U:
          highOut = (byte) 250;
          lowOut = (byte) (lowIn + 126U);
          break;
        case 149U:
          highOut = (byte) 251;
          lowOut = (byte) (lowIn + 31U);
          break;
        case 150U:
          highOut = (byte) 251;
          lowOut = (byte) (lowIn + 126U);
          break;
        case 151U:
          highOut = (byte) 252;
          lowOut = (byte) (lowIn + 31U);
          break;
        default:
          throw new InvalidOperationException(string.Format("ClassifyPair and MapIbmExtensionToCp932 disagree on {0},{1}", (object) highIn.ToString("X2"), (object) lowIn.ToString("X2")));
      }
    }

    private byte DecodeKana(uint current)
    {
      if (!this.isKana || current < 33U || current > 95U)
        return (byte) current;
      return (byte) (current + 128U);
    }

    public override void Reset()
    {
      this.isKana = false;
      this.leftoverByte = (byte) 0;
      this.runBeganWithEscape = false;
      this.isRunContainsIbmExtension = JisX0208PairClass.Unrecognized;
    }
  }
}
