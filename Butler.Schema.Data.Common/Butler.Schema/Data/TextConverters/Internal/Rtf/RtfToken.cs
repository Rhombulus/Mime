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
    private RtfTokenId id;
    private byte[] dataBuffer;
    private int offset;
    private int length;
    private RtfRunEntry[] runQueue;
    private int runQueueTail;
    private int textCodePage;
    private TextMapping textMapping;
    private int currentRun;
    private int currentRunOffset;
    private int currentRunDelta;
    private byte[] byteBuffer;
    private int byteBufferOffet;
    private int byteBufferCount;
    private char[] charBuffer;
    private int charBufferOffet;
    private int charBufferCount;
    private RunTextType elementTextType;
    private int elementOffset;
    private int elementLength;
    private RtfToken.DecoderMruEntry[] mruDecoders;
    private int mruDecodersLastIndex;
    private bool stripZeroBytes;
    private Dictionary<int, Decoder> decoderCache;

    public RtfTokenId Id => this.id;

      public byte[] Buffer => this.dataBuffer;

      public int Offset => this.offset;

      public int Length => this.length;

      public bool IsEmpty => this.runQueueTail == 0;

      public RtfToken.RunEnumerator Runs => new RtfToken.RunEnumerator(this);

      public RtfToken.KeywordEnumerator Keywords => new RtfToken.KeywordEnumerator(this);

      public TextMapping TextMapping => this.textMapping;

      public int TextCodePage => this.textCodePage;

      public RtfToken.TextReader Text => new RtfToken.TextReader(this);

      public RtfToken.TextEnumerator TextElements => new RtfToken.TextEnumerator(this);

      public bool StripZeroBytes
    {
      get
      {
        return this.stripZeroBytes;
      }
      set
      {
        this.stripZeroBytes = value;
      }
    }

    internal RtfRunEntry[] RunQueue => this.runQueue;

      internal int CurrentRun => this.currentRun;

      internal int CurrentRunOffset => this.currentRunOffset;

      internal char[] CharBuffer => this.charBuffer;

      internal bool IsTextEof
    {
      get
      {
        if (this.charBufferCount == 0 && this.byteBufferCount == 0)
          return this.currentRun == this.runQueueTail;
        return false;
      }
    }

    internal RunTextType ElementTextType => this.elementTextType;

      internal int ElementOffset => this.elementOffset;

      internal int ElementLength => this.elementLength;

      public RtfToken(byte[] buffer, RtfRunEntry[] runQueue)
    {
      this.dataBuffer = buffer;
      this.runQueue = runQueue;
    }

    public static RtfTokenId TokenIdFromRunKind(RtfRunKind runKind)
    {
      if (runKind >= RtfRunKind.Text)
        return RtfTokenId.Text;
      return (RtfTokenId) ((int) runKind >> 12);
    }

    public void Reset()
    {
      this.id = RtfTokenId.None;
      this.offset = 0;
      this.length = 0;
      this.runQueueTail = 0;
      this.textCodePage = 0;
      this.Rewind();
    }

    public void Initialize(RtfTokenId tokenId, int queueTail, int offset, int length)
    {
      this.id = tokenId;
      this.offset = offset;
      this.length = length;
      this.runQueueTail = queueTail;
      this.Rewind();
    }

    public void SetCodePage(int codePage, TextMapping textMapping)
    {
      this.textCodePage = codePage;
      this.textMapping = textMapping;
    }

    internal void Rewind()
    {
      this.charBufferOffet = this.charBufferCount = 0;
      this.byteBufferOffet = this.byteBufferCount = 0;
      this.currentRun = -1;
      this.currentRunOffset = this.offset;
      this.currentRunDelta = 0;
      this.elementOffset = this.elementLength = 0;
    }

    private bool DecodeMore()
    {
      if (this.charBuffer == null)
        this.charBuffer = new char[1025];
      int charCount = this.charBuffer.Length - 1;
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
          bool flush = this.NeedToFlushDecoderBeforeRun(this.currentRun);
          decoder.Convert(this.byteBuffer, this.byteBufferOffet, this.byteBufferCount, this.charBuffer, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
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
        if (this.currentRun != this.runQueueTail)
        {
          if (this.currentRun == -1 || this.CurrentRunIsSkiped())
          {
            do
            {
              this.MoveToNextRun();
            }
            while (this.currentRun != this.runQueueTail && this.CurrentRunIsSkiped());
            if (this.currentRun == this.runQueueTail)
              break;
          }
          if (this.CurrentRunIsSmall())
          {
            while (this.CurrentRunIsSkiped() || this.CurrentRunIsSmall() && this.CopyCurrentRunToBuffer())
            {
              this.MoveToNextRun();
              if (this.currentRun == this.runQueueTail)
                break;
            }
          }
          else if (!this.CurrentRunIsUnicode())
          {
            int byteIndex = this.currentRunOffset + this.currentRunDelta;
            int byteCount = (int) this.runQueue[this.currentRun].Length - this.currentRunDelta;
            Decoder decoder = this.GetDecoder();
            bool flush = this.NeedToFlushDecoderBeforeRun((this.currentRun + 1) % this.runQueue.Length);
            decoder.Convert(this.dataBuffer, byteIndex, byteCount, this.charBuffer, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
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
                  int ich = this.runQueue[this.currentRun].Value;
                  if (charCount >= 2)
                  {
                    if (ich > (int) ushort.MaxValue)
                    {
                      char[] chArray1 = this.charBuffer;
                      int index1 = charIndex;
                      int num1 = 1;
                      int num2 = index1 + num1;
                      int num3 = (int) ParseSupport.HighSurrogateCharFromUcs4(ich);
                      chArray1[index1] = (char) num3;
                      char[] chArray2 = this.charBuffer;
                      int index2 = num2;
                      int num4 = 1;
                      charIndex = index2 + num4;
                      int num5 = (int) ParseSupport.LowSurrogateCharFromUcs4(ich);
                      chArray2[index2] = (char) num5;
                      charCount -= 2;
                    }
                    else
                    {
                      this.charBuffer[charIndex++] = (char) ich;
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
            while (this.currentRun != this.runQueueTail);
          }
        }
        else
          break;
      }
      this.charBufferCount = charIndex;
      this.charBuffer[this.charBufferCount] = char.MinValue;
      return charIndex != 0;
    }

    private bool MoveToNextTextElement()
    {
      if (this.charBufferCount == 0 && !this.DecodeMore())
        return false;
      int index = this.charBufferOffet;
      this.elementOffset = index;
      char ch1 = this.charBuffer[index];
      if ((int) ch1 > 32 && (int) ch1 != 160)
      {
        this.elementTextType = RunTextType.NonSpace;
        char ch2;
        do
        {
          ch2 = this.charBuffer[++index];
        }
        while ((int) ch2 > 32 && (int) ch2 != 160);
      }
      else if ((int) ch1 == 32)
      {
        this.elementTextType = RunTextType.Space;
        while ((int) this.charBuffer[++index] == 32)
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
            this.elementTextType = RunTextType.UnusualWhitespace;
            char ch3;
            while (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(ch3 = this.charBuffer[++index])) && (int) ch3 != 32)
              ;
            break;
          case ' ':
            if (this.textMapping == TextMapping.Unicode)
            {
              this.elementTextType = RunTextType.Nbsp;
              while ((int) this.charBuffer[++index] == 160)
                ;
              break;
            }
            this.elementTextType = RunTextType.NonSpace;
            char ch4;
            do
            {
              ch4 = this.charBuffer[++index];
            }
            while ((int) ch4 > 32 && (int) ch4 != 160);
            break;
          default:
            this.elementTextType = RunTextType.NonSpace;
            while (ParseSupport.ControlCharacter(ParseSupport.GetCharClass(this.charBuffer[++index])))
              ;
            break;
        }
      }
      this.elementLength = index - this.elementOffset;
      this.charBufferOffet = index;
      this.charBufferCount -= this.elementLength;
      return true;
    }

    private int WriteTo(ITextSink sink)
    {
      int num = 0;
      while (this.charBufferCount != 0 || this.DecodeMore())
      {
        sink.Write(this.charBuffer, this.charBufferOffet, this.charBufferCount);
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
            ch = this.charBuffer[index];
          }
          else
            break;
        }
        int offset = index;
        if ((int) ch > 32 && (int) ch != 160)
        {
          do
          {
            ch = this.charBuffer[++index];
          }
          while ((int) ch > 32 && (int) ch != 160);
          output.OutputNonspace(this.charBuffer, offset, index - offset, this.textMapping);
        }
        else if ((int) ch == 32)
        {
          do
          {
            ch = this.charBuffer[++index];
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
                ch = this.charBuffer[++index];
              }
              while (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(ch)) && (int) ch != 32);
              output.OutputSpace(1);
              break;
            case ' ':
              if (this.textMapping == TextMapping.Unicode)
              {
                do
                {
                  ch = this.charBuffer[++index];
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
                ch = this.charBuffer[++index];
              }
              while ((int) ch > 32);
              output.OutputNonspace(this.charBuffer, offset, index - offset, this.textMapping);
              break;
            default:
              do
              {
                ch = this.charBuffer[++index];
              }
              while (ParseSupport.ControlCharacter(ParseSupport.GetCharClass(ch)));
              output.OutputNonspace(this.charBuffer, offset, index - offset, this.textMapping);
              break;
          }
        }
        this.charBufferOffet = index;
        this.charBufferCount -= index - offset;
      }
    }

    private bool NeedToFlushDecoderBeforeRun(int run)
    {
      if (run != this.runQueueTail && this.runQueue[run].Kind != RtfRunKind.Unicode)
        return this.runQueue[run].Kind == RtfRunKind.Ignore;
      return true;
    }

    private Decoder GetDecoder()
    {
      if (this.mruDecoders == null)
      {
        this.mruDecoders = new RtfToken.DecoderMruEntry[4];
        this.mruDecodersLastIndex = this.mruDecoders.Length - 1;
      }
      if (this.textCodePage == 0)
        this.textCodePage = 1252;
      if (this.mruDecoders[this.mruDecodersLastIndex].CodePage == this.textCodePage)
        return this.mruDecoders[this.mruDecodersLastIndex].Decoder;
      for (int index = 0; index < this.mruDecoders.Length; ++index)
      {
        if (this.mruDecoders[index].CodePage == this.textCodePage)
        {
          this.mruDecodersLastIndex = index;
          return this.mruDecoders[index].Decoder;
        }
      }
      Decoder decoder = (Decoder) null;
      if (this.decoderCache != null && this.decoderCache.ContainsKey(this.textCodePage))
        decoder = this.decoderCache[this.textCodePage];
      if (decoder == null)
      {
        int codePage = this.textCodePage;
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
      this.mruDecoders[this.mruDecodersLastIndex].CodePage = this.textCodePage;
      return decoder;
    }

    private void MoveToNextRun()
    {
      if (this.currentRun >= 0)
        this.currentRunOffset += (int) this.runQueue[this.currentRun].Length;
      this.currentRunDelta = 0;
      ++this.currentRun;
    }

    private bool CurrentRunIsSkiped()
    {
      return this.runQueue[this.currentRun].IsSkiped;
    }

    private bool CurrentRunIsSmall()
    {
      return this.runQueue[this.currentRun].IsSmall;
    }

    private bool CurrentRunIsUnicode()
    {
      return this.runQueue[this.currentRun].IsUnicode;
    }

    private bool CopyCurrentRunToBuffer()
    {
      if (this.byteBuffer == null)
        this.byteBuffer = new byte[256];
      if (this.byteBuffer.Length == this.byteBufferCount)
        return false;
      switch (this.runQueue[this.currentRun].Kind)
      {
        case RtfRunKind.Text:
          int count = Math.Min((int) this.runQueue[this.currentRun].Length - this.currentRunDelta, this.byteBuffer.Length - this.byteBufferCount);
          System.Buffer.BlockCopy((Array) this.dataBuffer, this.currentRunOffset + this.currentRunDelta, (Array) this.byteBuffer, this.byteBufferOffet + this.byteBufferCount, count);
          this.byteBufferCount += count;
          this.currentRunDelta += count;
          break;
        case RtfRunKind.Escape:
          this.byteBuffer[this.byteBufferOffet + this.byteBufferCount] = (byte) this.runQueue[this.currentRun].Value;
          ++this.byteBufferCount;
          return true;
        case RtfRunKind.Zero:
          int num = Math.Min((int) this.runQueue[this.currentRun].Length - this.currentRunDelta, this.byteBuffer.Length - this.byteBufferCount);
          if (!this.stripZeroBytes)
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
      return this.currentRunDelta == (int) this.runQueue[this.currentRun].Length;
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
        if (this.token.currentRun != this.token.runQueueTail)
        {
          this.token.MoveToNextRun();
          if (this.token.currentRun != this.token.runQueueTail)
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
        if (this.token.currentRun != this.token.runQueueTail)
        {
          this.token.MoveToNextRun();
          if (this.token.currentRun != this.token.runQueueTail)
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
              System.Buffer.BlockCopy((Array) this.token.charBuffer, this.token.charBufferOffet * 2, (Array) buffer, offset * 2, num2 * 2);
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
