// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.DecodeToCp932
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal abstract class DecodeToCp932
  {
    private static int LastChanceIndex;

    public static int LastChanceDecoderIndex => DecodeToCp932.LastChanceIndex;

      public virtual char Abbreviation => 'X';

      public abstract bool IsEscapeSequenceHandled(Escape escape);

    public abstract ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int bytesConsumed, out int bytesProduced);

    public abstract void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete);

    public static void MapEscape(byte[] dataIn, int offsetIn, int lengthIn, Escape escape)
    {
      int num1 = 0;
      EscapeState state = escape.State;
      int num2 = offsetIn;
      while (lengthIn > 0)
      {
        byte num3 = dataIn[offsetIn];
        EscapeSequence escapeSequence;
        switch (escape.State)
        {
          case EscapeState.Begin:
            if ((int) num3 == 27)
            {
              escape.State = EscapeState.Esc_1;
              break;
            }
            if ((int) num3 == 14)
            {
              escapeSequence = EscapeSequence.ShiftOut;
              goto label_83;
            }
            else if ((int) num3 == 15)
            {
              escapeSequence = EscapeSequence.ShiftIn;
              goto label_83;
            }
            else
            {
              escape.BytesInCurrentBuffer = 0;
              escape.TotalBytes = 0;
              escape.Sequence = EscapeSequence.None;
              return;
            }
          case EscapeState.Esc_1:
            if ((int) num3 == 36)
            {
              escape.State = EscapeState.Esc_Dollar_2;
              break;
            }
            if ((int) num3 == 40)
            {
              escape.State = EscapeState.Esc_OpenParen_2;
              break;
            }
            if ((int) num3 == 72)
            {
              escapeSequence = EscapeSequence.NECKanjiIn;
              goto label_83;
            }
            else if ((int) num3 == 75)
            {
              escapeSequence = EscapeSequence.JisX0208_Nec;
              goto label_83;
            }
            else
            {
              if ((int) num3 == 38)
              {
                escape.State = EscapeState.Esc_Ampersand_2;
                break;
              }
              if ((int) num3 != 27)
              {
                if ((int) num3 != 14 && (int) num3 != 15)
                {
                  if ((int) num3 != 0)
                    goto label_84;
                  else
                    goto label_84;
                }
                else
                  goto case 12;
              }
              else
                goto case 11;
            }
          case EscapeState.Esc_Dollar_2:
            if ((int) num3 == 64)
            {
              escapeSequence = EscapeSequence.JisX0208_1983;
              goto label_83;
            }
            else if ((int) num3 == 65)
            {
              escapeSequence = EscapeSequence.Gb2312_1980;
              goto label_83;
            }
            else if ((int) num3 == 66)
            {
              escapeSequence = EscapeSequence.JisX0208_1978;
              goto label_83;
            }
            else
            {
              if ((int) num3 == 40)
              {
                escape.State = EscapeState.Esc_Dollar_OpenParen_3;
                break;
              }
              if ((int) num3 == 41)
              {
                escape.State = EscapeState.Esc_Dollar_CloseParen_3;
                break;
              }
              if ((int) num3 != 27)
              {
                if ((int) num3 == 14 || (int) num3 == 15)
                  goto case 12;
                else
                  goto label_84;
              }
              else
                goto case 11;
            }
          case EscapeState.Esc_OpenParen_2:
            if ((int) num3 == 73)
            {
              escapeSequence = EscapeSequence.JisX0201K_1976;
              goto label_83;
            }
            else if ((int) num3 == 74)
            {
              escapeSequence = EscapeSequence.JisX0201_1976;
              goto label_83;
            }
            else if ((int) num3 == 68)
            {
              escapeSequence = EscapeSequence.JisX0212_1990;
              goto label_83;
            }
            else if ((int) num3 == 66)
            {
              escapeSequence = EscapeSequence.Iso646Irv;
              goto label_83;
            }
            else if ((int) num3 != 27)
            {
              if ((int) num3 == 14 || (int) num3 == 15)
                goto case 12;
              else
                goto label_84;
            }
            else
              goto case 11;
          case EscapeState.Esc_Ampersand_2:
            if ((int) num3 == 64)
            {
              escape.State = EscapeState.Esc_Ampersand_At_3;
              break;
            }
            if ((int) num3 != 27)
            {
              if ((int) num3 == 14 || (int) num3 == 15)
                goto case 12;
              else
                goto label_84;
            }
            else
              goto case 11;
          case EscapeState.Esc_Dollar_OpenParen_3:
            if ((int) num3 == 71)
            {
              escapeSequence = EscapeSequence.Cns11643_1992_1;
              goto label_83;
            }
            else if ((int) num3 == 67)
            {
              escapeSequence = EscapeSequence.Kcs5601_1987;
              goto label_83;
            }
            else if ((int) num3 == 72)
            {
              escapeSequence = EscapeSequence.Cns11643_1992_1;
              goto label_83;
            }
            else if ((int) num3 == 81)
            {
              escapeSequence = EscapeSequence.Unknown_1;
              goto label_83;
            }
            else if ((int) num3 != 27)
            {
              if ((int) num3 == 14 || (int) num3 == 15)
                goto case 12;
              else
                goto label_84;
            }
            else
              goto case 11;
          case EscapeState.Esc_Dollar_CloseParen_3:
            if ((int) num3 == 67)
            {
              escapeSequence = EscapeSequence.EucKsc;
              goto label_83;
            }
            else if ((int) num3 != 27)
            {
              if ((int) num3 == 14 || (int) num3 == 15)
                goto case 12;
              else
                goto label_84;
            }
            else
              goto case 11;
          case EscapeState.Esc_Ampersand_At_3:
            if ((int) num3 == 27)
            {
              escape.State = EscapeState.Esc_Ampersand_At_Esc_4;
              break;
            }
            if ((int) num3 == 14 || (int) num3 == 15)
              goto case 12;
            else
              goto label_84;
          case EscapeState.Esc_Ampersand_At_Esc_4:
            if ((int) num3 == 36)
            {
              escape.State = EscapeState.Esc_Ampersand_At_Esc_Dollar_5;
              break;
            }
            if ((int) num3 != 27)
            {
              if ((int) num3 == 14 || (int) num3 == 15)
                goto case 12;
              else
                goto label_84;
            }
            else
              goto case 11;
          case EscapeState.Esc_Ampersand_At_Esc_Dollar_5:
            if ((int) num3 == 66)
            {
              escapeSequence = EscapeSequence.JisX0208_1990;
              goto label_83;
            }
            else if ((int) num3 != 27)
            {
              if ((int) num3 == 14 || (int) num3 == 15)
                goto case 12;
              else
                goto label_84;
            }
            else
              goto case 11;
          case EscapeState.Esc_Esc_Reset:
            escape.State = EscapeState.Esc_1;
            break;
          case EscapeState.Esc_SISO_Reset:
            if ((int) num3 == 14)
            {
              escapeSequence = EscapeSequence.ShiftOut;
              goto label_83;
            }
            else
            {
              if ((int) num3 != 15)
                throw new InvalidOperationException(string.Format("MapEscape: at Esc_SISO_Reset with {0}", (object) (int) num3));
              escapeSequence = EscapeSequence.ShiftIn;
              goto label_83;
            }
          default:
            throw new InvalidOperationException("MapEscape: unrecognized state!");
        }
        --lengthIn;
        ++offsetIn;
        ++num1;
        continue;
label_83:
        escape.BytesInCurrentBuffer = num1 + 1;
        escape.TotalBytes += escape.BytesInCurrentBuffer;
        escape.State = EscapeState.Begin;
        escape.Sequence = escapeSequence;
        return;
label_84:
        string str = string.Empty;
        while (num2 <= offsetIn && num2 < offsetIn + lengthIn)
          str += dataIn[num2++].ToString("X2");
        string.Format("Unrecognized escape sequence {0}, initial state {1}, current state {2}", (object) str, (object) state.ToString(), (object) escape.State.ToString());
        escape.State = EscapeState.Begin;
        escape.Sequence = EscapeSequence.NotRecognized;
        escape.BytesInCurrentBuffer = num1;
        escape.TotalBytes += num1;
        return;
      }
      escape.BytesInCurrentBuffer = num1;
      escape.TotalBytes += escape.BytesInCurrentBuffer;
      escape.Sequence = EscapeSequence.Incomplete;
    }

    public static DecodeToCp932[] GetStandardOrder()
    {
      DecodeToCp932[] decodeToCp932Array = new DecodeToCp932[4]
      {
        (DecodeToCp932) new DecodeUsAsciiToCp932(),
        (DecodeToCp932) new DecodeJisX0208_1983ToCp932(),
        (DecodeToCp932) new DecodeJisX0201_1976ToCp932(),
        (DecodeToCp932) new DecodeLastChanceToCp932()
      };
      DecodeToCp932.LastChanceIndex = 3;
      return decodeToCp932Array;
    }

    public virtual void CheckLoopCount(ref int count, int limit)
    {
      if (count > limit)
        throw new FailedToProgressException(string.Format("{0} decoder failed to progress", (object) this.GetType().Name));
      ++count;
    }

    public virtual int CalculateLoopCountLimit(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException();
      return length * 6 + 1000;
    }

    public virtual void Reset()
    {
    }
  }
}
