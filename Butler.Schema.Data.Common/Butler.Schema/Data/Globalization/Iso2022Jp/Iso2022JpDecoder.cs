// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.Iso2022JpDecoder
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal class Iso2022JpDecoder : Decoder
  {
    private const int SummarySize = 32;
    private Iso2022DecodingMode killSwitch;
    private Decoder defaultDecoder;
    private Decoder cp932Decoder;
    private DecodeToCp932[] subDecoders;
    private int currentSubDecoder;
    private readonly int lastChanceDecoderIndex;
    private Iso2022JpEncoding parent;
    private byte[] leftovers;
    private Escape escape;
    private int runCount;
    private StringBuilder sbSummary;

    internal Iso2022JpDecoder(Iso2022JpEncoding parent)
    {
      this.killSwitch = Iso2022JpEncoding.InternalReadKillSwitch();
      this.parent = parent;
      switch (this.killSwitch)
      {
        case Iso2022DecodingMode.Default:
          this.defaultDecoder = parent.DefaultEncoding.GetDecoder();
          goto case 2;
        case Iso2022DecodingMode.Override:
          this.subDecoders = DecodeToCp932.GetStandardOrder();
          this.cp932Decoder = Encoding.GetEncoding(932).GetDecoder();
          this.currentSubDecoder = -1;
          this.lastChanceDecoderIndex = DecodeToCp932.LastChanceDecoderIndex;
          this.leftovers = (byte[]) null;
          this.escape = new Escape();
          goto case 2;
        case Iso2022DecodingMode.Throw:
          this.sbSummary = new StringBuilder(32);
          break;
        default:
          throw new InvalidOperationException("Invalid Iso2022DecodingMode!");
      }
    }

    public override unsafe void Convert(byte* bytes, int byteCount, char* chars, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
    {
      throw new NotImplementedException();
    }

    public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
    {
      switch (this.killSwitch)
      {
        case Iso2022DecodingMode.Default:
          this.defaultDecoder.Convert(bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
          break;
        case Iso2022DecodingMode.Override:
          bytesUsed = 0;
          charsUsed = 0;
          completed = false;
          int bytesUsed1 = 0;
          int charsUsed1 = 0;
          bool completed1 = false;
          if (this.leftovers != null)
          {
            this.ConvertBuffer(this.leftovers, 0, this.leftovers.Length, chars, charIndex, charCount, flush, out bytesUsed1, out charsUsed1, out completed1);
            if (!completed1)
              throw new ArgumentException("Caller did not provide enough space for previously unprocessed data!", nameof(chars));
            charIndex += charsUsed1;
            charCount -= charsUsed1;
            bytesUsed += bytesUsed1;
            this.leftovers = (byte[]) null;
          }
          if (byteCount == 0)
          {
            completed = true;
            break;
          }
          this.ConvertBuffer(bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
          int length = byteCount - (bytesUsed - bytesUsed1);
          int sourceIndex = byteIndex + (byteCount - length);
          if (length <= 0 || (int) bytes[sourceIndex] == 0)
            break;
          this.leftovers = new byte[length];
          Array.Copy((Array) bytes, sourceIndex, (Array) this.leftovers, 0, length);
          break;
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    private void ConvertBuffer(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
    {
      bytesUsed = 0;
      charsUsed = 0;
      completed = false;
      int usedOut = 0;
      while (byteCount > 0)
      {
        int bytesConsumed = 0;
        int usedIn = 0;
        int bytesUsed1 = 0;
        int charsUsed1 = 0;
        int cp932ByteCount;
        this.GetNextRun(bytes, byteIndex, byteCount, out bytesConsumed, out cp932ByteCount);
        if (bytesConsumed == 0 && (int) bytes[byteIndex] == 0)
        {
          if (charCount > 0)
          {
            chars[charIndex] = ' ';
            ++charsUsed;
            ++charIndex;
            --charCount;
          }
          ++bytesUsed;
          ++byteIndex;
          --byteCount;
          if (byteCount == 0)
          {
            completed = true;
            break;
          }
        }
        else
        {
          byte[] numArray = new byte[cp932ByteCount];
          bool complete;
          this.subDecoders[this.currentSubDecoder].ConvertToCp932(bytes, byteIndex, bytesConsumed, numArray, 0, cp932ByteCount, flush, this.escape, out usedIn, out usedOut, out complete);
          bytesUsed += usedIn;
          byteIndex += usedIn;
          byteCount -= usedIn;
          if (bytesConsumed != usedIn)
            throw new InvalidOperationException(string.Format("{2}.GetNextRun input length {0} does not match ConvertToCp932 length {1}", (object) bytesConsumed, (object) usedIn, (object) this.subDecoders[this.currentSubDecoder].GetType()));
          if (usedOut != cp932ByteCount)
            throw new InvalidOperationException(string.Format("{2}.GetNextRun output length {0} does not match ConvertToCp932 length {1}", (object) cp932ByteCount, (object) usedOut, (object) this.subDecoders[this.currentSubDecoder].GetType()));
          bool flag = byteCount > 0 && (int) bytes[byteIndex] == 0;
          if (cp932ByteCount == 0)
          {
            if (byteCount <= 0 || flag)
            {
              completed = true;
              break;
            }
          }
          else
          {
            int count = cp932ByteCount;
            int charCount1 = this.cp932Decoder.GetCharCount(numArray, 0, cp932ByteCount, flush);
            while (charCount1 > charCount)
            {
              --count;
              charCount1 = this.cp932Decoder.GetCharCount(numArray, 0, count, flush);
              complete = false;
            }
            this.cp932Decoder.Convert(numArray, 0, usedOut, chars, charIndex, charCount, flush, out bytesUsed1, out charsUsed1, out completed);
            charsUsed += charsUsed1;
            charIndex += charsUsed1;
            charCount -= charsUsed1;
            completed = flag || completed && complete;
          }
        }
      }
    }

    private void GetNextRun(byte[] bytes, int byteIndex, int byteCount, out int bytesConsumed, out int cp932ByteCount)
    {
      bytesConsumed = 0;
      cp932ByteCount = 0;
      DecodeToCp932.MapEscape(bytes, byteIndex, byteCount, this.escape);
      if (this.escape.Sequence == EscapeSequence.Incomplete)
      {
        bytesConsumed += this.escape.BytesInCurrentBuffer;
        this.currentSubDecoder = this.lastChanceDecoderIndex;
        this.sbSummary.Append("|");
      }
      else
      {
        int bytesConsumed1;
        int bytesProduced;
        if (this.currentSubDecoder != -1 && !this.escape.IsValidEscapeSequence && (this.subDecoders[this.currentSubDecoder].GetRunLength(bytes, byteIndex, byteCount, this.escape, out bytesConsumed1, out bytesProduced) == ValidationResult.Valid && bytesConsumed1 > 0))
        {
          cp932ByteCount = bytesProduced;
          bytesConsumed = bytesConsumed1;
          this.sbSummary.AppendFormat("|{0}", (object) bytesConsumed);
        }
        else
        {
          ++this.runCount;
          if (this.escape.IsValidEscapeSequence)
          {
            for (int index = 0; index < this.subDecoders.Length; ++index)
            {
              if (this.subDecoders[index].IsEscapeSequenceHandled(this.escape))
              {
                if (this.subDecoders[index].GetRunLength(bytes, byteIndex, byteCount, this.escape, out bytesConsumed1, out bytesProduced) != ValidationResult.Valid)
                  throw new InvalidOperationException(string.Format("{0}.IsEscapeSequenceCovered and GetRunLength must agree on validity", (object) this.subDecoders[index].GetType().Name));
                cp932ByteCount = bytesProduced;
                bytesConsumed = bytesConsumed1;
                this.currentSubDecoder = index;
                this.sbSummary.AppendFormat("E{0}{1}", (object) this.escape.Abbreviation, (object) bytesConsumed);
                return;
              }
            }
          }
          else
          {
            for (int index = 0; index < this.subDecoders.Length; ++index)
            {
              if (this.subDecoders[index].GetRunLength(bytes, byteIndex, byteCount, this.escape, out bytesConsumed1, out bytesProduced) == ValidationResult.Valid && (bytesConsumed1 > 0 || bytesConsumed1 < byteCount && (int) bytes[byteIndex + bytesConsumed1] == 0))
              {
                this.currentSubDecoder = index;
                cp932ByteCount = bytesProduced;
                bytesConsumed = bytesConsumed1;
                this.sbSummary.AppendFormat("{0}{1}", (object) this.subDecoders[index].Abbreviation, (object) bytesConsumed);
                return;
              }
            }
          }
          throw new InvalidOperationException("What happened to the last-chance decoder?");
        }
      }
    }

    public override unsafe int GetCharCount(byte* bytes, int count, bool flush)
    {
      throw new NotImplementedException();
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      return this.GetCharCount(bytes, index, count, false);
    }

    public override int GetCharCount(byte[] bytes, int index, int count, bool flush)
    {
      switch (this.killSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultDecoder.GetCharCount(bytes, index, count, flush);
        case Iso2022DecodingMode.Override:
          char[] chars = new char[this.parent.GetMaxCharCount(count)];
          int bytesUsed;
          int charsUsed;
          bool completed;
          this.Convert(bytes, index, count, chars, 0, chars.Length, flush, out bytesUsed, out charsUsed, out completed);
          this.Reset();
          return charsUsed;
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount, bool flush)
    {
      throw new NotImplementedException();
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
      return this.GetChars(bytes, byteIndex, byteCount, chars, charIndex, false);
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, bool flush)
    {
      switch (this.killSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultDecoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex, flush);
        case Iso2022DecodingMode.Override:
          int bytesUsed;
          int charsUsed;
          bool completed;
          this.Convert(bytes, byteIndex, byteCount, chars, charIndex, chars.Length - charIndex, flush, out bytesUsed, out charsUsed, out completed);
          if (!completed)
            throw new ArgumentException("insufficient capacity", nameof(chars));
          return charsUsed;
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override void Reset()
    {
      switch (this.killSwitch)
      {
        case Iso2022DecodingMode.Default:
          this.defaultDecoder.Reset();
          break;
        case Iso2022DecodingMode.Override:
          foreach (DecodeToCp932 decodeToCp932 in this.subDecoders)
            decodeToCp932.Reset();
          this.currentSubDecoder = -1;
          this.leftovers = (byte[]) null;
          this.escape.Reset();
          this.runCount = 0;
          this.sbSummary = new StringBuilder(32);
          break;
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override string ToString()
    {
      return string.Format("{0} runs; {1}", (object) this.runCount, (object) this.sbSummary.ToString());
    }
  }
}
