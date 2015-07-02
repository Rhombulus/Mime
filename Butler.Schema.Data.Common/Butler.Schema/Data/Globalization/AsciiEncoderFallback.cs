// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.AsciiEncoderFallback
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Globalization
{
  internal class AsciiEncoderFallback : EncoderFallback
  {
    public override int MaxCharCount
    {
      get
      {
        return AsciiEncoderFallback.AsciiFallbackBuffer.MaxCharCount;
      }
    }

    public static string GetCharacterFallback(char charUnknown)
    {
      if ((int) charUnknown <= 339 && (int) charUnknown >= 130)
      {
        switch (charUnknown)
        {
          case 'Ĳ':
            return "IJ";
          case 'ĳ':
            return "ij";
          case 'Œ':
            return "OE";
          case 'œ':
            return "oe";
          case '\x0082':
          case '\x0091':
          case '\x0092':
            return "'";
          case '\x0083':
            return "f";
          case '\x0084':
          case '\x0093':
          case '\x0094':
            return "\"";
          case '\x0085':
            return "...";
          case '\x008B':
            return "<";
          case '\x008C':
            return "OE";
          case '\x0095':
            return "*";
          case '\x0096':
            return "-";
          case '\x0097':
            return "-";
          case '\x0098':
            return "~";
          case '\x0099':
            return "(tm)";
          case '\x009B':
            return ">";
          case '\x009C':
            return "oe";
          case ' ':
            return " ";
          case '¢':
            return "c";
          case '¤':
            return "$";
          case '¥':
            return "Y";
          case '¦':
            return "|";
          case '©':
            return "(c)";
          case '«':
            return "<";
          case '\x00AD':
            return string.Empty;
          case '®':
            return "(r)";
          case '\x00B2':
            return "^2";
          case '\x00B3':
            return "^3";
          case '·':
            return "*";
          case '¸':
            return ",";
          case '\x00B9':
            return "^1";
          case '»':
            return ">";
          case '\x00BC':
            return "(1/4)";
          case '\x00BD':
            return "(1/2)";
          case '\x00BE':
            return "(3/4)";
          case 'Æ':
            return "AE";
          case 'æ':
            return "ae";
        }
      }
      else if ((int) charUnknown >= 8194 && (int) charUnknown <= 8482)
      {
        switch (charUnknown)
        {
          case '‹':
            return "<";
          case '›':
            return ">";
          case '€':
            return "EUR";
          case '™':
            return "(tm)";
          case ' ':
          case ' ':
            return " ";
          case '‑':
            return "-";
          case '–':
          case '—':
            return "-";
          case '‘':
          case '’':
          case '‚':
            return "'";
          case '“':
          case '”':
          case '„':
            return "\"";
          case '•':
            return "*";
          case '…':
            return "...";
        }
      }
      else if ((int) charUnknown >= 9785 && (int) charUnknown <= 9786)
      {
        switch (charUnknown)
        {
          case '☹':
            return ":(";
          case '☺':
            return ":)";
        }
      }
      return (string) null;
    }

    public override EncoderFallbackBuffer CreateFallbackBuffer()
    {
      return (EncoderFallbackBuffer) new AsciiEncoderFallback.AsciiFallbackBuffer();
    }

    private class AsciiFallbackBuffer : EncoderFallbackBuffer
    {
      private int fallbackIndex;
      private string fallbackString;

      public static int MaxCharCount
      {
        get
        {
          return 5;
        }
      }

      public override int Remaining
      {
        get
        {
          if (this.fallbackString != null)
            return this.fallbackString.Length - this.fallbackIndex;
          return 0;
        }
      }

      public override bool Fallback(char charUnknown, int index)
      {
        this.fallbackIndex = 0;
        this.fallbackString = AsciiEncoderFallback.GetCharacterFallback(charUnknown);
        if (this.fallbackString == null)
          this.fallbackString = "?";
        return true;
      }

      public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
      {
        this.fallbackIndex = 0;
        this.fallbackString = "?";
        return true;
      }

      public override char GetNextChar()
      {
        if (this.fallbackString == null || this.fallbackIndex == this.fallbackString.Length)
          return char.MinValue;
        return this.fallbackString[this.fallbackIndex++];
      }

      public override bool MovePrevious()
      {
        if (this.fallbackIndex <= 0)
          return false;
        --this.fallbackIndex;
        return true;
      }

      public override void Reset()
      {
        this.fallbackString = "?";
        this.fallbackIndex = 0;
      }
    }
  }
}
