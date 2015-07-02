// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfOutput : IRestartable, IByteSource, IDisposable
  {
    private Globalization.CodePageMap codePageMap = new Globalization.CodePageMap();
    private ByteCache cache = new ByteCache();
    private Stream pushSink;
    private ConverterStream pullSink;
    private bool endOfFile;
    private bool restartable;
    private bool restartablePushSink;
    private long restartPosition;
    private byte[] outputBuffer;
    private int outputStart;
    private int outputCurrent;
    private int outputEnd;
    private bool outputToCache;
    private Encoder encoder;
    private bool encoderFlushed;
    private char[] textBuffer;
    private int textEnd;
    private RtfOutput.TextType textType;
    private bool lastKeyword;
    private byte[] encodeBuffer;

      public int RtfLineLength { get; set; }

      public bool CanAcceptMoreOutput
    {
      get
      {
        if (!this.restartable && this.pullSink != null)
          return this.cache.Length == 0;
        return true;
      }
    }

    public RtfOutput(Stream destination, bool push, bool restartable)
    {
      if (!push)
      {
        this.pullSink = destination as ConverterStream;
        this.pullSink.SetSource((IByteSource) this);
      }
      else
      {
        this.pushSink = destination;
        if (restartable && destination.CanSeek && destination.Position == destination.Length)
        {
          this.restartablePushSink = true;
          this.restartPosition = destination.Position;
        }
      }
      this.restartable = restartable;
      this.textBuffer = new char[512];
    }

    bool IRestartable.CanRestart()
    {
      return this.restartable;
    }

    void IRestartable.Restart()
    {
      this.endOfFile = false;
      this.cache.Reset();
      this.textEnd = 0;
      this.textType = RtfOutput.TextType.Control;
      this.lastKeyword = false;
      if (this.pullSink == null && this.restartablePushSink)
      {
        this.pushSink.Position = this.restartPosition;
        this.pushSink.SetLength(this.restartPosition);
      }
      this.restartable = false;
    }

    void IRestartable.DisableRestart()
    {
      this.restartable = false;
      this.FlushCached();
    }

    public void SetEncoding(Encoding encoding)
    {
      this.codePageMap.ChoseCodePage(encoding);
      this.encoder = encoding.GetEncoder();
    }

    public void Flush()
    {
      if (this.endOfFile)
        return;
      this.restartable = false;
      this.FlushTextBuffer(true);
      this.CommitOutput();
      this.FlushCached();
      if (this.pullSink == null)
        this.pushSink.Flush();
      else if (this.cache.Length == 0)
        this.pullSink.ReportEndOfFile();
      this.endOfFile = true;
    }

    private bool FlushCached()
    {
      if (this.restartable || this.cache.Length == 0)
        return false;
      if (this.pullSink == null)
      {
        while (this.cache.Length != 0)
        {
          byte[] outputBuffer;
          int outputOffset;
          int outputCount;
          this.cache.GetData(out outputBuffer, out outputOffset, out outputCount);
          this.pushSink.Write(outputBuffer, outputOffset, outputCount);
          this.cache.ReportRead(outputCount);
        }
      }
      else
      {
        byte[] outputBuffer;
        int outputOffset;
        int outputCount;
        this.pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputCount);
        this.pullSink.ReportOutput(this.cache.Read(outputBuffer, outputOffset, outputCount));
      }
      return true;
    }

    public void WriteKeyword(string keyword, int value)
    {
      if (this.textType != RtfOutput.TextType.Control)
      {
        this.FlushTextBuffer(true);
        this.textType = RtfOutput.TextType.Control;
      }
      int num1 = 0;
      int num2 = value;
      do
      {
        ++num1;
      }
      while ((num2 /= 10) != 0);
      if (this.outputEnd - this.outputCurrent < keyword.Length + num1 + 1 + 2)
      {
        this.CommitOutput();
        this.GetOutputBuffer(keyword.Length + num1 + 1 + 2);
      }
      for (int index = 0; index < keyword.Length; ++index)
        this.outputBuffer[this.outputCurrent++] = (byte) keyword[index];
      if (value < 0)
      {
        this.outputBuffer[this.outputCurrent++] = (byte) 45;
        value = -value;
      }
      int num3 = this.outputCurrent = this.outputCurrent + num1;
      do
      {
        this.outputBuffer[--num3] = (byte) (value % 10 + 48);
      }
      while ((value /= 10) != 0);
      this.RtfLineLength += keyword.Length + num1 + 1;
      if (this.RtfLineLength > 128)
      {
        this.outputBuffer[this.outputCurrent++] = (byte) 13;
        this.outputBuffer[this.outputCurrent++] = (byte) 10;
        this.RtfLineLength = 0;
        this.lastKeyword = false;
      }
      else
        this.lastKeyword = true;
    }

    public void WriteControlText(string controlText, bool lastKeyword)
    {
      if (this.textType != RtfOutput.TextType.Control)
      {
        this.FlushTextBuffer(true);
        this.textType = RtfOutput.TextType.Control;
      }
      int index = 0;
      if (this.outputEnd - this.outputCurrent < controlText.Length)
      {
        while (this.outputCurrent != this.outputEnd)
        {
          this.outputBuffer[this.outputCurrent++] = (byte) controlText[index];
          ++index;
        }
        this.CommitOutput();
        this.GetOutputBuffer(controlText.Length - index);
      }
      for (; index < controlText.Length; ++index)
        this.outputBuffer[this.outputCurrent++] = (byte) controlText[index];
      this.RtfLineLength += controlText.Length;
      if (this.RtfLineLength > 128 && (int) controlText[controlText.Length - 1] != 10 && (this.outputEnd - this.outputCurrent >= 2 && lastKeyword))
      {
        this.outputBuffer[this.outputCurrent++] = (byte) 13;
        this.outputBuffer[this.outputCurrent++] = (byte) 10;
        this.RtfLineLength = 0;
        lastKeyword = false;
      }
      this.lastKeyword = lastKeyword;
    }

    public void WriteBinary(byte[] buffer, int offset, int count)
    {
      if (count == 0)
        return;
      if (this.textType != RtfOutput.TextType.Control)
      {
        this.FlushTextBuffer(true);
        this.textType = RtfOutput.TextType.Control;
      }
      int num;
      switch (buffer[offset + count - 1])
      {
        case (byte) 10:
        case (byte) 13:
          num = 0;
          break;
        default:
          num = this.RtfLineLength + count;
          break;
      }
      this.RtfLineLength = num;
      if (this.outputEnd - this.outputCurrent < count)
      {
        int count1 = this.outputEnd - this.outputCurrent;
        if (count1 != 0)
        {
          Buffer.BlockCopy((Array) buffer, offset, (Array) this.outputBuffer, this.outputCurrent, count1);
          this.outputCurrent += count1;
          offset += count1;
          count -= count1;
        }
        this.CommitOutput();
        this.GetOutputBuffer(count);
      }
      Buffer.BlockCopy((Array) buffer, offset, (Array) this.outputBuffer, this.outputCurrent, count);
      this.outputCurrent += count;
      this.lastKeyword = false;
    }

    public void WriteText(string buffer)
    {
      this.WriteText(buffer, 0, buffer.Length);
    }

    public void WriteText(string buffer, int offset, int count)
    {
      if (this.textType != RtfOutput.TextType.Text || this.textEnd > this.textBuffer.Length - count)
      {
        if (this.textType == RtfOutput.TextType.Control)
        {
          if (this.lastKeyword)
            this.WriteControlText(" ", false);
        }
        else
          this.FlushTextBuffer(this.textType != RtfOutput.TextType.Text);
        this.textType = RtfOutput.TextType.Text;
      }
      this.RtfLineLength += count;
      while (count != 0)
      {
        int count1 = Math.Min(count, this.textBuffer.Length - this.textEnd);
        buffer.CopyTo(offset, this.textBuffer, this.textEnd, count1);
        offset += count1;
        count -= count1;
        this.textEnd += count1;
        if (count == 0)
          break;
        this.FlushTextBuffer(false);
      }
    }

    public void WriteText(char[] buffer, int offset, int count)
    {
      if (this.textType != RtfOutput.TextType.Text || this.textEnd > this.textBuffer.Length - count)
      {
        if (this.textType == RtfOutput.TextType.Control)
        {
          if (this.lastKeyword)
            this.WriteControlText(" ", false);
        }
        else
          this.FlushTextBuffer(this.textType != RtfOutput.TextType.Text);
        this.textType = RtfOutput.TextType.Text;
      }
      this.RtfLineLength += count;
      if (count <= 64)
      {
        Buffer.BlockCopy((Array) buffer, offset * 2, (Array) this.textBuffer, this.textEnd * 2, count * 2);
        this.textEnd += count;
      }
      else
      {
        if (this.textEnd != 0)
        {
          this.FlushTextBuffer(false);
          if (this.textEnd != 0)
          {
            this.textBuffer[this.textEnd++] = buffer[offset++];
            --count;
            this.FlushTextBuffer(false);
          }
        }
        int num = this.EncodeText(buffer, offset, count, false);
        if (num != count)
        {
          this.textBuffer[0] = this.textBuffer[offset + num];
          this.textEnd = 1;
        }
        else
          this.textEnd = 0;
      }
    }

    public void WriteEncapsulatedMarkupText(char[] buffer, int offset, int count)
    {
      if (this.textType != RtfOutput.TextType.MarkupText || this.textEnd > this.textBuffer.Length - count)
      {
        if (this.textType == RtfOutput.TextType.Control)
        {
          if (this.lastKeyword)
            this.WriteControlText(" ", false);
        }
        else
          this.FlushTextBuffer(this.textType != RtfOutput.TextType.MarkupText);
        this.textType = RtfOutput.TextType.MarkupText;
      }
      this.RtfLineLength += count;
      if (count <= 64)
      {
        Buffer.BlockCopy((Array) buffer, offset * 2, (Array) this.textBuffer, this.textEnd * 2, count * 2);
        this.textEnd += count;
      }
      else
      {
        if (this.textEnd != 0)
        {
          this.FlushTextBuffer(false);
          if (this.textEnd != 0)
          {
            this.textBuffer[this.textEnd++] = buffer[offset++];
            --count;
            this.FlushTextBuffer(false);
          }
        }
        int num = this.EncodeText(buffer, offset, count, false);
        if (num != count)
        {
          this.textBuffer[0] = this.textBuffer[offset + num];
          this.textEnd = 1;
        }
        else
          this.textEnd = 0;
      }
    }

    public void WriteDoubleEscapedText(string buffer)
    {
      this.WriteDoubleEscapedText(buffer, 0, buffer.Length);
    }

    public void WriteDoubleEscapedText(string buffer, int offset, int count)
    {
      if (this.textType != RtfOutput.TextType.DoubleEscapedText || this.textEnd > this.textBuffer.Length - count)
      {
        if (this.textType == RtfOutput.TextType.Control)
        {
          if (this.lastKeyword)
            this.WriteControlText(" ", false);
        }
        else
          this.FlushTextBuffer(this.textType != RtfOutput.TextType.Text);
        this.textType = RtfOutput.TextType.DoubleEscapedText;
      }
      this.RtfLineLength += count;
      while (true)
      {
        int count1 = Math.Min(count, this.textBuffer.Length - this.textEnd);
        buffer.CopyTo(offset, this.textBuffer, this.textEnd, count1);
        offset += count1;
        count -= count1;
        this.textEnd += count1;
        if (count != 0)
          this.FlushTextBuffer(false);
        else
          break;
      }
    }

    private void FlushTextBuffer(bool flushEncoder)
    {
      if (this.textEnd == 0 && (!flushEncoder || this.encoderFlushed))
        return;
      int index = this.EncodeText(this.textBuffer, 0, this.textEnd, flushEncoder);
      if (index != this.textEnd)
      {
        this.textBuffer[0] = this.textBuffer[index];
        this.textEnd = 1;
      }
      else
        this.textEnd = 0;
    }

    private int EncodeText(char[] buffer, int offset, int count, bool flushEncoder)
    {
      int charIndex = offset;
      int index1;
      for (int index2 = offset + count; charIndex != index2; charIndex = index1)
      {
        if (this.outputCurrent == this.outputEnd)
        {
          this.CommitOutput();
          this.GetOutputBuffer(16);
        }
        index1 = charIndex;
        char ch1 = buffer[index1];
        if ((int) ch1 < RtfSupport.UnsafeAsciiMap.Length)
        {
          if ((int) RtfSupport.UnsafeAsciiMap[(int) ch1] != 0)
          {
            if (this.outputEnd - this.outputCurrent < 6)
            {
              this.CommitOutput();
              this.GetOutputBuffer(16);
            }
            if ((int) ch1 == 92)
            {
              if (this.textType == RtfOutput.TextType.DoubleEscapedText)
              {
                this.outputBuffer[this.outputCurrent++] = (byte) 92;
                this.outputBuffer[this.outputCurrent++] = (byte) 92;
              }
              this.outputBuffer[this.outputCurrent++] = (byte) 92;
              this.outputBuffer[this.outputCurrent++] = (byte) 92;
            }
            else if ((int) ch1 == 123 || (int) ch1 == 125)
            {
              if (this.textType == RtfOutput.TextType.DoubleEscapedText)
                this.outputBuffer[this.outputCurrent++] = (byte) 92;
              this.outputBuffer[this.outputCurrent++] = (byte) 92;
              this.outputBuffer[this.outputCurrent++] = (byte) ch1;
            }
            else if ((int) ch1 == 34)
            {
              if (this.textType == RtfOutput.TextType.DoubleEscapedText)
              {
                this.outputBuffer[this.outputCurrent++] = (byte) 92;
                this.outputBuffer[this.outputCurrent++] = (byte) 92;
                this.outputBuffer[this.outputCurrent++] = (byte) 34;
              }
              else
                this.outputBuffer[this.outputCurrent++] = (byte) 34;
            }
            else if ((int) ch1 == 160)
              this.outputBuffer[this.outputCurrent++] = (byte) 32;
            else if (this.textType == RtfOutput.TextType.MarkupText && ((int) ch1 == 13 || (int) ch1 == 10))
            {
              this.outputBuffer[this.outputCurrent++] = (byte) 92;
              this.outputBuffer[this.outputCurrent++] = (byte) 112;
              this.outputBuffer[this.outputCurrent++] = (byte) 97;
              this.outputBuffer[this.outputCurrent++] = (byte) 114;
              this.outputBuffer[this.outputCurrent++] = (byte) 13;
              this.outputBuffer[this.outputCurrent++] = (byte) 10;
              if ((int) ch1 == 13 && index1 < index2 - 1 && (int) buffer[index1 + 1] == 10)
                ++index1;
            }
            else
            {
              this.outputBuffer[this.outputCurrent++] = (byte) 92;
              this.outputBuffer[this.outputCurrent++] = (byte) 39;
              RtfSupport.Escape(ch1, this.outputBuffer, this.outputCurrent);
              this.outputCurrent += 2;
            }
            ++index1;
          }
          else
          {
            this.outputBuffer[this.outputCurrent++] = (byte) ch1;
            char ch2;
            for (++index1; index1 != index2 && this.outputCurrent != this.outputEnd && ((int) (ch2 = buffer[index1]) < RtfSupport.UnsafeAsciiMap.Length && (int) RtfSupport.UnsafeAsciiMap[(int) ch2] == 0); ++index1)
              this.outputBuffer[this.outputCurrent++] = (byte) ch2;
          }
        }
        else if (!this.codePageMap.IsUnsafeExtendedCharacter(ch1))
        {
          if (this.outputEnd - this.outputCurrent < 16)
          {
            this.CommitOutput();
            this.GetOutputBuffer(16);
          }
          char ch2;
          do
          {
            ++index1;
          }
          while (index1 != index2 && (int) (ch2 = buffer[index1]) >= RtfSupport.UnsafeAsciiMap.Length && !this.codePageMap.IsUnsafeExtendedCharacter(ch2));
          if (this.encodeBuffer == null)
            this.encodeBuffer = new byte[512];
          bool flush = index1 != index2 || flushEncoder;
          bool completed;
          do
          {
            int charsUsed;
            int bytesUsed;
            this.encoder.Convert(buffer, charIndex, index1 - charIndex, this.encodeBuffer, 0, this.encodeBuffer.Length, flush, out charsUsed, out bytesUsed, out completed);
            this.encoderFlushed = flush && completed;
            for (int index3 = 0; index3 < bytesUsed; ++index3)
            {
              if (this.outputEnd - this.outputCurrent < 4)
              {
                this.CommitOutput();
                this.GetOutputBuffer(16);
              }
              this.outputBuffer[this.outputCurrent++] = (byte) 92;
              this.outputBuffer[this.outputCurrent++] = (byte) 39;
              RtfSupport.Escape((char) this.encodeBuffer[index3], this.outputBuffer, this.outputCurrent);
              this.outputCurrent += 2;
            }
            charIndex += charsUsed;
          }
          while (charIndex != index1 || flush && !completed);
        }
        else
        {
          if (this.outputEnd - this.outputCurrent < 8)
          {
            this.CommitOutput();
            this.GetOutputBuffer(16);
          }
          int num1 = (int) ch1;
          int num2 = num1 < 10 ? 1 : (num1 < 100 ? 2 : (num1 < 1000 ? 3 : (num1 < 10000 ? 4 : 5)));
          this.outputBuffer[this.outputCurrent++] = (byte) 92;
          this.outputBuffer[this.outputCurrent++] = (byte) 117;
          int num3 = this.outputCurrent + num2;
          while (num1 != 0)
          {
            int num4 = num1 % 10;
            this.outputBuffer[--num3] = (byte) (num4 + 48);
            num1 /= 10;
          }
          this.outputCurrent += num2;
          this.outputBuffer[this.outputCurrent++] = (byte) 63;
          ++index1;
        }
      }
      return count;
    }

    private void GetOutputBuffer(int minLength)
    {
      if (this.restartable || this.pullSink == null || this.cache.Length != 0)
      {
        this.cache.GetBuffer(minLength, out this.outputBuffer, out this.outputStart);
        this.outputEnd = this.outputBuffer.Length;
        this.outputToCache = true;
      }
      else
      {
        int outputCount;
        this.pullSink.GetOutputBuffer(out this.outputBuffer, out this.outputStart, out outputCount);
        this.outputEnd = this.outputStart + outputCount;
        if (outputCount < minLength)
        {
          this.cache.GetBuffer(minLength, out this.outputBuffer, out this.outputStart);
          this.outputEnd = this.outputBuffer.Length;
          this.outputToCache = true;
        }
        else
          this.outputToCache = false;
      }
      this.outputCurrent = this.outputStart;
    }

    private void CommitOutput()
    {
      if (this.outputCurrent == this.outputStart)
        return;
      if (this.outputToCache)
      {
        int length = this.cache.Length;
        this.cache.Commit(this.outputCurrent - this.outputStart);
        if (length == 0)
        {
          if (this.pullSink == null)
          {
            if (!this.restartable || this.restartablePushSink)
            {
              while (this.cache.Length != 0)
              {
                byte[] outputBuffer;
                int outputOffset;
                int outputCount;
                this.cache.GetData(out outputBuffer, out outputOffset, out outputCount);
                this.pushSink.Write(outputBuffer, outputOffset, outputCount);
                this.cache.ReportRead(outputCount);
              }
            }
          }
          else if (!this.restartable)
          {
            byte[] outputBuffer;
            int outputOffset;
            int outputCount;
            this.pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputCount);
            if (outputCount != 0)
            {
              outputCount = this.cache.Read(outputBuffer, outputOffset, outputCount);
              this.pullSink.ReportOutput(outputCount);
            }
          }
        }
      }
      else
        this.pullSink.ReportOutput(this.outputCurrent - this.outputStart);
      this.outputStart = this.outputCurrent;
    }

    bool IByteSource.GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount)
    {
      if (this.outputCurrent != this.outputStart)
        this.CommitOutput();
      if (this.cache.Length == 0 || this.restartable)
      {
        chunkBuffer = (byte[]) null;
        chunkOffset = 0;
        chunkCount = 0;
        return false;
      }
      this.cache.GetData(out chunkBuffer, out chunkOffset, out chunkCount);
      return true;
    }

    void IByteSource.ReportOutput(int readCount)
    {
      this.cache.ReportRead(readCount);
      if (this.cache.Length != 0)
        return;
      this.outputToCache = false;
      this.outputStart = this.outputCurrent = this.outputEnd;
      if (!this.endOfFile)
        return;
      this.pullSink.ReportEndOfFile();
    }

    void IDisposable.Dispose()
    {
      if (this.cache != null && this.cache is IDisposable)
        ((IDisposable) this.cache).Dispose();
      this.cache = (ByteCache) null;
      this.pushSink = (Stream) null;
      this.pullSink = (ConverterStream) null;
      this.textBuffer = (char[]) null;
      this.encodeBuffer = (byte[]) null;
      this.encoder = (Encoder) null;
      GC.SuppressFinalize((object) this);
    }

    private enum TextType
    {
      Control,
      Text,
      DoubleEscapedText,
      MarkupText,
    }
  }
}
