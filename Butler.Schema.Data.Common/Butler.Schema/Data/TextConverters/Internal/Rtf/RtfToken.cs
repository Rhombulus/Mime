// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfToken
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfToken
  {

      private int runQueueTail;
      private int currentRunDelta;
    private byte[] byteBuffer;
    private int byteBufferOffet;
    private int byteBufferCount;
      private int charBufferOffet;
    private int charBufferCount;
      private RtfToken.DecoderMruEntry[] mruDecoders;
    private int mruDecodersLastIndex;
      private Dictionary<int, Decoder> decoderCache;

    public RtfTokenId Id { get; private set; }

      public byte[] Buffer { get; }

      public int Offset { get; private set; }

      public int Length { get; private set; }

      public bool IsEmpty => this.runQueueTail == 0;

      public RtfToken.RunEnumerator Runs => new RtfToken.RunEnumerator(this);

      public RtfToken.KeywordEnumerator Keywords => new RtfToken.KeywordEnumerator(this);

      public TextMapping TextMapping { get; private set; }

      public int TextCodePage { get; private set; }

      public RtfToken.TextReader Text => new RtfToken.TextReader(this);

      public RtfToken.TextEnumerator TextElements => new RtfToken.TextEnumerator(this);

      public bool StripZeroBytes { get; set; }

      internal RtfRunEntry[] RunQueue { get; }

      internal int CurrentRun { get; private set; }

      internal int CurrentRunOffset { get; private set; }

      internal char[] CharBuffer { get; private set; }

      internal bool IsTextEof
    {
      get
      {
        if (this.charBufferCount == 0 && this.byteBufferCount == 0)
          return this.CurrentRun == this.runQueueTail;
        return false;
      }
    }

    internal RunTextType ElementTextType { get; private set; }

      internal int ElementOffset { get; private set; }

      internal int ElementLength { get; private set; }

      public RtfToken(byte[] buffer, RtfRunEntry[] runQueue)
    {
      this.Buffer = buffer;
      this.RunQueue = runQueue;
    }

    public static RtfTokenId TokenIdFromRunKind(RtfRunKind runKind)
    {
      if (runKind >= RtfRunKind.Text)
        return RtfTokenId.Text;
      return (RtfTokenId) ((int) runKind >> 12);
    }

    public void Reset()
    {
      this.Id = RtfTokenId.None;
      this.Offset = 0;
      this.Length = 0;
      this.runQueueTail = 0;
      this.TextCodePage = 0;
      this.Rewind();
    }

    public void Initialize(RtfTokenId tokenId, int queueTail, int offset, int length)
    {
      this.Id = tokenId;
      this.Offset = offset;
      this.Length = length;
      this.runQueueTail = queueTail;
      this.Rewind();
    }

    public void SetCodePage(int codePage, TextMapping textMapping)
    {
      this.TextCodePage = codePage;
      this.TextMapping = textMapping;
    }

    internal void Rewind()
    {
      this.charBufferOffet = this.charBufferCount = 0;
      this.byteBufferOffet = this.byteBufferCount = 0;
      this.CurrentRun = -1;
      this.CurrentRunOffset = this.Offset;
      this.currentRunDelta = 0;
      this.ElementOffset = this.ElementLength = 0;
    }

    private bool DecodeMore()
    {
      if (this.CharBuffer == null)
        this.CharBuffer = new char[1025];
      int charCount = this.CharBuffer.Length - 1;
      int charIndex = 0;
      this.charBufferOffet = 0;
      while (charCount >= 32)
      {
        int bytesUsed;
        int charsUsed;
        bool completed;
        if (this.byteBufferCount != 0 || this.byteBufferOffet != 0)
        {
          Decoder decoder = this.GetDecoder();
          bool flush = this.NeedToFlushDecoderBeforeRun(this.CurrentRun);
          decoder.Convert(this.byteBuffer, this.byteBufferOffet, this.byteBufferCount, this.CharBuffer, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
          charIndex += charsUsed;
          charCount -= charsUsed;
          this.byteBufferOffet += bytesUsed;
          this.byteBufferCount -= bytesUsed;
          if (completed)
          {
            this.byteBufferOffet = 0;
            if (charCount < 32)
              break;
          }
          else
            break;
        }
        if (this.CurrentRun != this.runQueueTail)
        {
          if (this.CurrentRun == -1 || this.CurrentRunIsSkiped())
          {
            do
            {
              this.MoveToNextRun();
            }
            while (this.CurrentRun != this.runQueueTail && this.CurrentRunIsSkiped());
            if (this.CurrentRun == this.runQueueTail)
              break;
          }
          if (this.CurrentRunIsSmall())
          {
            while (this.CurrentRunIsSkiped() || this.CurrentRunIsSmall() && this.CopyCurrentRunToBuffer())
            {
              this.MoveToNextRun();
              if (this.CurrentRun == this.runQueueTail)
                break;
            }
          }
          else if (!this.CurrentRunIsUnicode())
          {
            int byteIndex = this.CurrentRunOffset + this.currentRunDelta;
            int byteCount = (int) this.RunQueue[this.CurrentRun].Length - this.currentRunDelta;
            Decoder decoder = this.GetDecoder();
            bool flush = this.NeedToFlushDecoderBeforeRun((this.CurrentRun + 1) % this.RunQueue.Length);
            decoder.Convert(this.Buffer, byteIndex, byteCount, this.CharBuffer, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
            charIndex += charsUsed;
            charCount -= charsUsed;
            int num1 = byteIndex + bytesUsed;
            int num2 = byteCount - bytesUsed;
            this.currentRunDelta += bytesUsed;
            if (completed)
              this.MoveToNextRun();
            else
              break;
          }
          else
          {
            do
            {
              if (!this.CurrentRunIsSkiped())
              {
                if (this.CurrentRunIsUnicode())
                {
                  int ich = this.RunQueue[this.CurrentRun].Value;
                  if (charCount >= 2)
                  {
                    if (ich > (int) ushort.MaxValue)
                    {
                      char[] chArray1 = this.CharBuffer;
                      int index1 = charIndex;
                      int num1 = 1;
                      int num2 = index1 + num1;
                      int num3 = (int) ParseSupport.HighSurrogateCharFromUcs4(ich);
                      chArray1[index1] = (char) num3;
                      char[] chArray2 = this.CharBuffer;
                      int index2 = num2;
                      int num4 = 1;
                      charIndex = index2 + num4;
                      int num5 = (int) ParseSupport.LowSurrogateCharFromUcs4(ich);
                      chArray2[index2] = (char) num5;
                      charCount -= 2;
                    }
                    else
                    {
                      this.CharBuffer[charIndex++] = (char) ich;
                      --charCount;
                    }
                  }
                  else
                    break;
                }
                else
                  break;
              }
              this.MoveToNextRun();
            }
            while (this.CurrentRun != this.runQueueTail);
          }
        }
        else
          break;
      }
      this.charBufferCount = charIndex;
      this.CharBuffer[this.charBufferCount] = char.MinValue;
      return charIndex != 0;
    }

    private bool MoveToNextTextElement()
    {
      if (this.charBufferCount == 0 && !this.DecodeMore())
        return false;
      int index = this.charBufferOffet;
      this.ElementOffset = index;
      char ch1 = this.CharBuffer[index];
      if ((int) ch1 > 32 && (int) ch1 != 160)
      {
        this.ElementTextType = RunTextType.NonSpace;
        char ch2;
        do
        {
          ch2 = this.CharBuffer[++index];
        }
        while ((int) ch2 > 32 && (int) ch2 != 160);
      }
      else if ((int) ch1 == 32)
      {
        this.ElementTextType = RunTextType.Space;
        while ((int) this.CharBuffer[++index] == 32)
          ;
      }
      else
      {
        switch (ch1)
        {
          case '\t':
          case '\n':
          case '\v':
          case '\f':
          case '\r':
            this.ElementTextType = RunTextType.UnusualWhitespace;
            char ch3;
            while (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(ch3 = this.CharBuffer[++index])) && (int) ch3 != 32)
              ;
            break;
          case ' ':
            if (this.TextMapping == TextMapping.Unicode)
            {
              this.ElementTextType = RunTextType.Nbsp;
              while ((int) this.CharBuffer[++index] == 160)
                ;
              break;
            }
            this.ElementTextType = RunTextType.NonSpace;
            char ch4;
            do
            {
              ch4 = this.CharBuffer[++index];
            }
            while ((int) ch4 > 32 && (int) ch4 != 160);
            break;
          default:
            this.ElementTextType = RunTextType.NonSpace;
            while (ParseSupport.ControlCharacter(ParseSupport.GetCharClass(this.CharBuffer[++index])))
              ;
            break;
        }
      }
      this.ElementLength = index - this.ElementOffset;
      this.charBufferOffet = index;
      this.charBufferCount -= this.ElementLength;
      return true;
    }

    private int WriteTo(ITextSink sink)
    {
      int num = 0;
      while (this.charBufferCount != 0 || this.DecodeMore())
      {
        sink.Write(this.CharBuffer, this.charBufferOffet, this.charBufferCount);
        this.charBufferOffet = 0;
        this.charBufferCount = 0;
        num += this.charBufferCount;
      }
      return num;
    }

    private void OutputTextElements(Text.TextOutput output, bool treatNbspAsBreakable)
    {
      int index = 0;
      char ch = char.MinValue;
      while (true)
      {
        if (this.charBufferCount == 0)
        {
          if (this.DecodeMore())
          {
            index = this.charBufferOffet;
            ch = this.CharBuffer[index];
          }
          else
            break;
        }
        int offset = index;
        if ((int) ch > 32 && (int) ch != 160)
        {
          do
          {
            ch = this.CharBuffer[++index];
          }
          while ((int) ch > 32 && (int) ch != 160);
          output.OutputNonspace(this.CharBuffer, offset, index - offset, this.TextMapping);
        }
        else if ((int) ch == 32)
        {
          do
          {
            ch = this.CharBuffer[++index];
          }
          while ((int) ch == 32);
          output.OutputSpace(index - offset);
        }
        else
        {
          switch (ch)
          {
            case char.MinValue:
              break;
            case '\t':
            case '\n':
            case '\v':
            case '\f':
            case '\r':
              do
              {
                ch = this.CharBuffer[++index];
              }
              while (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(ch)) && (int) ch != 32);
              output.OutputSpace(1);
              break;
            case ' ':
              if (this.TextMapping == TextMapping.Unicode)
              {
                do
                {
                  ch = this.CharBuffer[++index];
                }
                while ((int) ch == 160);
                if (treatNbspAsBreakable)
                {
                  output.OutputSpace(index - offset);
                  break;
                }
                output.OutputNbsp(index - offset);
                break;
              }
              do
              {
                ch = this.CharBuffer[++index];
              }
              while ((int) ch > 32);
              output.OutputNonspace(this.CharBuffer, offset, index - offset, this.TextMapping);
              break;
            default:
              do
              {
                ch = this.CharBuffer[++index];
              }
              while (ParseSupport.ControlCharacter(ParseSupport.GetCharClass(ch)));
              output.OutputNonspace(this.CharBuffer, offset, index - offset, this.TextMapping);
              break;
          }
        }
        this.charBufferOffet = index;
        this.charBufferCount -= index - offset;
      }
    }

    private bool NeedToFlushDecoderBeforeRun(int run)
    {
      if (run != this.runQueueTail && this.RunQueue[run].Kind != RtfRunKind.Unicode)
        return this.RunQueue[run].Kind == RtfRunKind.Ignore;
      return true;
    }

    private Decoder GetDecoder()
    {
      if (this.mruDecoders == null)
      {
        this.mruDecoders = new RtfToken.DecoderMruEntry[4];
        this.mruDecodersLastIndex = this.mruDecoders.Length - 1;
      }
      if (this.TextCodePage == 0)
        this.TextCodePage = 1252;
      if (this.mruDecoders[this.mruDecodersLastIndex].CodePage == this.TextCodePage)
        return this.mruDecoders[this.mruDecodersLastIndex].Decoder;
      for (int index = 0; index < this.mruDecoders.Length; ++index)
      {
        if (this.mruDecoders[index].CodePage == this.TextCodePage)
        {
          this.mruDecodersLastIndex = index;
          return this.mruDecoders[index].Decoder;
        }
      }
      Decoder decoder = (Decoder) null;
      if (this.decoderCache != null && this.decoderCache.ContainsKey(this.TextCodePage))
        decoder = this.decoderCache[this.TextCodePage];
      if (decoder == null)
      {
        int codePage = this.TextCodePage;
        if (codePage == 42)
          codePage = 28591;
        decoder = Globalization.Charset.GetEncoding(codePage).GetDecoder();
      }
      this.mruDecodersLastIndex = (this.mruDecodersLastIndex + 1) % this.mruDecoders.Length;
      if (this.mruDecoders[this.mruDecodersLastIndex].Decoder != null)
      {
        if (this.decoderCache == null)
          this.decoderCache = new Dictionary<int, Decoder>();
        if (!this.decoderCache.ContainsKey(this.mruDecoders[this.mruDecodersLastIndex].CodePage))
          this.decoderCache[this.mruDecoders[this.mruDecodersLastIndex].CodePage] = this.mruDecoders[this.mruDecodersLastIndex].Decoder;
      }
      this.mruDecoders[this.mruDecodersLastIndex].Decoder = decoder;
      this.mruDecoders[this.mruDecodersLastIndex].CodePage = this.TextCodePage;
      return decoder;
    }

    private void MoveToNextRun()
    {
      if (this.CurrentRun >= 0)
        this.CurrentRunOffset += (int) this.RunQueue[this.CurrentRun].Length;
      this.currentRunDelta = 0;
      ++this.CurrentRun;
    }

    private bool CurrentRunIsSkiped()
    {
      return this.RunQueue[this.CurrentRun].IsSkiped;
    }

    private bool CurrentRunIsSmall()
    {
      return this.RunQueue[this.CurrentRun].IsSmall;
    }

    private bool CurrentRunIsUnicode()
    {
      return this.RunQueue[this.CurrentRun].IsUnicode;
    }

    private bool CopyCurrentRunToBuffer()
    {
      if (this.byteBuffer == null)
        this.byteBuffer = new byte[256];
      if (this.byteBuffer.Length == this.byteBufferCount)
        return false;
      switch (this.RunQueue[this.CurrentRun].Kind)
      {
        case RtfRunKind.Text:
          int count = Math.Min((int) this.RunQueue[this.CurrentRun].Length - this.currentRunDelta, this.byteBuffer.Length - this.byteBufferCount);
          System.Buffer.BlockCopy((Array) this.Buffer, this.CurrentRunOffset + this.currentRunDelta, (Array) this.byteBuffer, this.byteBufferOffet + this.byteBufferCount, count);
          this.byteBufferCount += count;
          this.currentRunDelta += count;
          break;
        case RtfRunKind.Escape:
          this.byteBuffer[this.byteBufferOffet + this.byteBufferCount] = (byte) this.RunQueue[this.CurrentRun].Value;
          ++this.byteBufferCount;
          return true;
        case RtfRunKind.Zero:
          int num = Math.Min((int) this.RunQueue[this.CurrentRun].Length - this.currentRunDelta, this.byteBuffer.Length - this.byteBufferCount);
          if (!this.StripZeroBytes)
          {
            for (int index = 0; index < num; ++index)
            {
              this.byteBuffer[this.byteBufferOffet + this.byteBufferCount] = (byte) 32;
              ++this.byteBufferCount;
            }
          }
          this.currentRunDelta += num;
          break;
      }
      return this.currentRunDelta == (int) this.RunQueue[this.CurrentRun].Length;
    }

    internal struct RunEnumerator
    {
      private RtfToken token;

      public int Count => this.token.runQueueTail;

        public RtfRun Current => new RtfRun(this.token);

        internal RunEnumerator(RtfToken token)
      {
        this.token = token;
      }

      public bool MoveNext()
      {
        if (this.token.CurrentRun != this.token.runQueueTail)
        {
          this.token.MoveToNextRun();
          if (this.token.CurrentRun != this.token.runQueueTail)
            return true;
        }
        return false;
      }

      public void Rewind()
      {
        this.token.Rewind();
      }

      public RtfToken.RunEnumerator GetEnumerator()
      {
        return this;
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    internal struct KeywordEnumerator
    {
      private RtfToken token;

      public int Count => this.token.runQueueTail;

        public RtfKeyword Current => new RtfKeyword(this.token);

        internal KeywordEnumerator(RtfToken token)
      {
        this.token = token;
      }

      public bool MoveNext()
      {
        if (this.token.CurrentRun != this.token.runQueueTail)
        {
          this.token.MoveToNextRun();
          if (this.token.CurrentRun != this.token.runQueueTail)
            return true;
        }
        return false;
      }

      public void Rewind()
      {
        this.token.Rewind();
      }

      public RtfToken.KeywordEnumerator GetEnumerator()
      {
        return this;
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    internal struct TextReader
    {
      private RtfToken token;

      internal TextReader(RtfToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        int num1 = offset;
        if (!this.token.IsTextEof)
        {
          do
          {
            if (this.token.charBufferCount != 0)
            {
              int num2 = Math.Min(count, this.token.charBufferCount);
              System.Buffer.BlockCopy((Array) this.token.CharBuffer, this.token.charBufferOffet * 2, (Array) buffer, offset * 2, num2 * 2);
              offset += num2;
              count -= num2;
              this.token.charBufferOffet += num2;
              this.token.charBufferCount -= num2;
              if (count == 0)
                break;
            }
          }
          while (this.token.DecodeMore());
        }
        return offset - num1;
      }

      public void Rewind()
      {
        this.token.Rewind();
      }

      public int WriteTo(ITextSink sink)
      {
        return this.token.WriteTo(sink);
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    internal struct TextEnumerator
    {
      private RtfToken token;

      public RtfTextElement Current => new RtfTextElement(this.token);

        internal TextEnumerator(RtfToken token)
      {
        this.token = token;
      }

      public bool MoveNext()
      {
        return this.token.MoveToNextTextElement();
      }

      public bool MoveNext(bool skipAllWhitespace)
      {
        return this.token.MoveToNextTextElement();
      }

      public void Rewind()
      {
        this.token.Rewind();
      }

      public RtfToken.TextEnumerator GetEnumerator()
      {
        return this;
      }

      public void OutputTextElements(Text.TextOutput output, bool treatNbspAsBreakable)
      {
        this.token.OutputTextElements(output, treatNbspAsBreakable);
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    private struct DecoderMruEntry
    {
      public int CodePage;
      public Decoder Decoder;
    }
  }
}
