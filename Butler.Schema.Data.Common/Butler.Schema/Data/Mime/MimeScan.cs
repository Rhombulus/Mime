// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeScan
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal static class MimeScan
  {
    private static readonly MimeScan.Token[] Dictionary = new MimeScan.Token[256]
    {
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl | MimeScan.Token.Lwsp | MimeScan.Token.Fwsp,
      MimeScan.Token.Ctl | MimeScan.Token.Lwsp,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl | MimeScan.Token.Lwsp,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Ctl,
      MimeScan.Token.Lwsp | MimeScan.Token.BChar | MimeScan.Token.Fwsp,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.Token | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.TSpec | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Digit | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.BChar,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.TSpec | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.TSpec | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.Spec | MimeScan.Token.TSpec | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Hex | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.BChar | MimeScan.Token.Field | MimeScan.Token.Alpha,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Token | MimeScan.Token.Atom | MimeScan.Token.Field,
      MimeScan.Token.Ctl,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0,
      (MimeScan.Token) 0
    };

    public static bool IsLWSP(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Lwsp);
    }

    public static bool IsFWSP(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Fwsp);
    }

    public static bool IsCTRL(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Ctl);
    }

    public static bool IsAtom(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Atom);
    }

    public static bool IsToken(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Token);
    }

    public static bool IsAlpha(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Alpha);
    }

    public static bool IsDigit(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Digit);
    }

    public static bool IsAlphaOrDigit(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & (MimeScan.Token.Digit | MimeScan.Token.Alpha));
    }

    public static bool IsHex(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Hex);
    }

    public static bool IsBChar(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.BChar);
    }

    public static bool IsField(byte ch)
    {
      return (MimeScan.Token) 0 != (MimeScan.Dictionary[(int) ch] & MimeScan.Token.Field);
    }

    public static bool IsUTF8NonASCII(byte[] bytes, int startOffset, int endOffset, out int bytesUsed)
    {
      if (endOffset == -1)
        endOffset = bytes.Length;
      return Internal.ByteString.IsUTF8NonASCII(startOffset < endOffset ? bytes[startOffset] : (byte) 0, startOffset + 1 < endOffset ? bytes[startOffset + 1] : (byte) 0, startOffset + 2 < endOffset ? bytes[startOffset + 2] : (byte) 0, startOffset + 3 < endOffset ? bytes[startOffset + 3] : (byte) 0, out bytesUsed);
    }

    public static bool IsEncodingRequired(byte ch)
    {
      return (MimeScan.Token) 0 == (MimeScan.Dictionary[(int) ch] & (MimeScan.Token.Spec | MimeScan.Token.Atom | MimeScan.Token.Fwsp));
    }

    public static bool IsEscapingRequired(byte ch)
    {
      if ((int) ch != 34 && (int) ch != 92)
        return (int) ch == 13;
      return true;
    }

    public static bool IsSegmentEncodingRequired(byte ch)
    {
      if ((MimeScan.Dictionary[(int) ch] & (MimeScan.Token.Ctl | MimeScan.Token.TSpec | MimeScan.Token.Lwsp)) == (MimeScan.Token) 0 && (int) ch != 39 && (int) ch != 42)
        return (int) ch == 37;
      return true;
    }

    public static int FindEndOf(MimeScan.Token token, byte[] bytes, int start, int length, out int characterCount, bool allowUTF8)
    {
      int startOffset = start - 1;
      int endOffset = start + length;
      characterCount = 0;
      while (++startOffset < endOffset)
      {
        if ((MimeScan.Dictionary[(int) bytes[startOffset]] & token) != (MimeScan.Token) 0)
          ++characterCount;
        else if (allowUTF8 && (int) bytes[startOffset] >= 128)
        {
          int bytesUsed = 0;
          if (MimeScan.IsUTF8NonASCII(bytes, startOffset, endOffset, out bytesUsed) && ((MimeScan.Token.Token | MimeScan.Token.Atom) & token) != (MimeScan.Token) 0)
          {
            startOffset += bytesUsed - 1;
            ++characterCount;
          }
          else
            break;
        }
        else
          break;
      }
      return startOffset - start;
    }

    public static int FindEndOf(MimeScan.Token token, string value, int currentOffset, bool allowUTF8)
    {
      int index = currentOffset - 1;
      do
        ;
      while (++index < value.Length && ((int) value[index] < 128 && (MimeScan.Dictionary[(int) value[index]] & token) != (MimeScan.Token) 0 || allowUTF8 && (int) value[index] >= 128 && ((MimeScan.Token.Token | MimeScan.Token.Atom) & token) != (MimeScan.Token) 0));
      return index - currentOffset;
    }

    public static int FindNextOf(MimeScan.Token token, byte[] bytes, int start, int length, out int characterCount, bool allowUTF8)
    {
      int startOffset = start - 1;
      int endOffset = start + length;
      characterCount = 0;
      while (++startOffset < endOffset && (MimeScan.Dictionary[(int) bytes[startOffset]] & token) == (MimeScan.Token) 0)
      {
        if (allowUTF8 && (int) bytes[startOffset] >= 128)
        {
          int bytesUsed = 0;
          if (MimeScan.IsUTF8NonASCII(bytes, startOffset, endOffset, out bytesUsed))
          {
            if (((MimeScan.Token.Token | MimeScan.Token.Atom) & token) == (MimeScan.Token) 0)
            {
              startOffset += bytesUsed - 1;
              ++characterCount;
              continue;
            }
            break;
          }
        }
        ++characterCount;
      }
      return startOffset - start;
    }

    public static int SkipLwsp(byte[] bytes, int offset, int length)
    {
      int characterCount = 0;
      return MimeScan.FindEndOf(MimeScan.Token.Lwsp, bytes, offset, length, out characterCount, false);
    }

    public static int SkipToLwspOrEquals(byte[] bytes, int start, int length)
    {
      int index = start - 1;
      int num1 = start + length;
      while (++index < num1)
      {
        byte num2 = bytes[index];
        MimeScan.Token token = MimeScan.Dictionary[(int) num2];
        if ((token & (MimeScan.Token.TSpec | MimeScan.Token.Lwsp)) != (MimeScan.Token) 0 && ((token & MimeScan.Token.Lwsp) != (MimeScan.Token) 0 || (int) num2 == 61))
          break;
      }
      return index - start;
    }

    public static int ScanComment(byte[] bytes, int start, int length, bool handleISO2022, ref int level, ref bool quotedPair)
    {
      int index = start - 1;
      int num1 = start + length;
      while (++index < num1)
      {
        byte num2 = bytes[index];
        if (quotedPair)
          quotedPair = false;
        else if (92 == (int) num2)
          quotedPair = true;
        else if (40 == (int) num2)
          ++level;
        else if (41 == (int) num2)
        {
          --level;
          if (level == 0)
          {
            ++index;
            break;
          }
        }
        else if (handleISO2022 && ((int) num2 == 14 || (int) num2 == 27))
          break;
      }
      return index - start;
    }

    public static int ScanQuotedString(byte[] bytes, int start, int length, bool handleISO2022, ref bool quotedPair)
    {
      int index = start - 1;
      int num1 = start + length;
      while (++index < num1)
      {
        byte num2 = bytes[index];
        if (quotedPair)
          quotedPair = false;
        else if (92 == (int) num2 || 34 == (int) num2 || handleISO2022 && ((int) num2 == 14 || (int) num2 == 27))
          break;
      }
      return index - start;
    }

    public static int ScanJISString(byte[] bytes, int start, int length, ref bool done)
    {
      int index = start - 1;
      int num = start + length;
      while (++index < num)
      {
        if ((int) bytes[index] < 33)
        {
          done = true;
          break;
        }
      }
      return index - start;
    }

    [Flags]
    internal enum Token : short
    {
      Ctl = (short) 1,
      Spec = (short) 2,
      TSpec = (short) 4,
      Token = (short) 8,
      Atom = (short) 16,
      Digit = (short) 32,
      Hex = (short) 64,
      Lwsp = (short) 128,
      BChar = (short) 256,
      Field = (short) 512,
      Alpha = (short) 1024,
      Fwsp = (short) 2048,
    }
  }
}
