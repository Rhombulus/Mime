// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterUnicodeOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class ConverterUnicodeOutput : ConverterOutput, IRestartable, IReusable
  {
    private bool isFirstChar = true;
    private TextCache cache = new TextCache();
    private const int FallbackExpansionMax = 16;
    private TextWriter pushSink;
    private ConverterReader pullSink;
    private bool endOfFile;
    private bool restartable;
    private bool canRestart;

    public override bool CanAcceptMore
    {
      get
      {
        if (!this.canRestart && this.pullSink != null)
          return this.cache.Length == 0;
        return true;
      }
    }

    public ConverterUnicodeOutput(object destination, bool push, bool restartable)
    {
      if (push)
      {
        this.pushSink = destination as TextWriter;
      }
      else
      {
        this.pullSink = destination as ConverterReader;
        this.pullSink.SetSource(this);
      }
      this.restartable = this.canRestart = restartable;
    }

    bool IRestartable.CanRestart()
    {
      return this.canRestart;
    }

    void IRestartable.Restart()
    {
      this.Reinitialize();
      this.canRestart = false;
    }

    void IRestartable.DisableRestart()
    {
      this.canRestart = false;
      this.FlushCached();
    }

    void IReusable.Initialize(object newSourceOrDestination)
    {
      if (this.pushSink != null && newSourceOrDestination != null)
      {
        TextWriter textWriter = newSourceOrDestination as TextWriter;
        if (textWriter == null)
          throw new InvalidOperationException("cannot reinitialize this converter - new output should be a TextWriter object");
        this.pushSink = textWriter;
      }
      this.Reinitialize();
    }

    public override void Write(char[] buffer, int offset, int count, IFallback fallback)
    {
      byte unsafeAsciiMask = (byte) 0;
      byte[] unsafeAsciiMap = fallback == null ? (byte[]) null : fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
      bool hasUnsafeUnicode = fallback != null && fallback.HasUnsafeUnicode();
      char[] buffer1;
      int offset1;
      int realSize1;
      if (this.cache.Length != 0 || this.canRestart)
      {
        while (count != 0)
        {
          if (fallback != null)
          {
            this.cache.GetBuffer(16, out buffer1, out offset1, out realSize1);
            int num1 = offset1;
            while (count != 0 && realSize1 != 0)
            {
              char ch = buffer[offset];
              if (ConverterUnicodeOutput.IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
              {
                int num2 = offset1;
                if (fallback.FallBackChar(ch, buffer1, ref offset1, offset1 + realSize1))
                  realSize1 -= offset1 - num2;
                else
                  break;
              }
              else
              {
                buffer1[offset1++] = ch;
                --realSize1;
              }
              this.isFirstChar = false;
              --count;
              ++offset;
            }
            this.cache.Commit(offset1 - num1);
          }
          else
          {
            this.cache.GetBuffer(Math.Min(count, 256), out buffer1, out offset1, out realSize1);
            int count1 = Math.Min(realSize1, count);
            Buffer.BlockCopy((Array) buffer, offset * 2, (Array) buffer1, offset1 * 2, count1 * 2);
            this.isFirstChar = false;
            this.cache.Commit(count1);
            offset += count1;
            count -= count1;
          }
        }
      }
      else if (this.pullSink != null)
      {
        char[] outputBuffer1;
        int outputIndex;
        int outputCount1;
        this.pullSink.GetOutputBuffer(out outputBuffer1, out outputIndex, out outputCount1);
        if (outputCount1 != 0)
        {
          if (fallback != null)
          {
            int num1 = outputIndex;
            while (count != 0 && outputCount1 != 0)
            {
              char ch = buffer[offset];
              if (ConverterUnicodeOutput.IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
              {
                int num2 = outputIndex;
                if (fallback.FallBackChar(ch, outputBuffer1, ref outputIndex, outputIndex + outputCount1))
                  outputCount1 -= outputIndex - num2;
                else
                  break;
              }
              else
              {
                outputBuffer1[outputIndex++] = ch;
                --outputCount1;
              }
              this.isFirstChar = false;
              --count;
              ++offset;
            }
            this.pullSink.ReportOutput(outputIndex - num1);
          }
          else
          {
            int outputCount2 = Math.Min(outputCount1, count);
            Buffer.BlockCopy((Array) buffer, offset * 2, (Array) outputBuffer1, outputIndex * 2, outputCount2 * 2);
            this.isFirstChar = false;
            count -= outputCount2;
            offset += outputCount2;
            this.pullSink.ReportOutput(outputCount2);
            outputIndex += outputCount2;
            outputCount1 -= outputCount2;
          }
        }
        char[] buffer2;
        int offset2;
        int realSize2;
        while (count != 0)
        {
          if (fallback != null)
          {
            this.cache.GetBuffer(16, out buffer2, out offset2, out realSize2);
            int num1 = offset2;
            while (count != 0 && realSize2 != 0)
            {
              char ch = buffer[offset];
              if (ConverterUnicodeOutput.IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
              {
                int num2 = offset2;
                if (fallback.FallBackChar(ch, buffer2, ref offset2, offset2 + realSize2))
                  realSize2 -= offset2 - num2;
                else
                  break;
              }
              else
              {
                buffer2[offset2++] = ch;
                --realSize2;
              }
              this.isFirstChar = false;
              --count;
              ++offset;
            }
            this.cache.Commit(offset2 - num1);
          }
          else
          {
            this.cache.GetBuffer(Math.Min(count, 256), out buffer2, out offset2, out realSize2);
            int count1 = Math.Min(realSize2, count);
            Buffer.BlockCopy((Array) buffer, offset * 2, (Array) buffer2, offset2 * 2, count1 * 2);
            this.isFirstChar = false;
            this.cache.Commit(count1);
            offset += count1;
            count -= count1;
          }
        }
        while (outputCount1 != 0 && this.cache.Length != 0)
        {
          char[] outputBuffer2;
          int outputOffset;
          int outputCount2;
          this.cache.GetData(out outputBuffer2, out outputOffset, out outputCount2);
          int num = Math.Min(outputCount2, outputCount1);
          Buffer.BlockCopy((Array) outputBuffer2, outputOffset * 2, (Array) outputBuffer1, outputIndex * 2, num * 2);
          this.cache.ReportRead(num);
          this.pullSink.ReportOutput(num);
          outputIndex += num;
          outputCount1 -= num;
        }
      }
      else if (fallback != null)
      {
        char[] buffer2;
        int offset2;
        int realSize2;
        this.cache.GetBuffer(1024, out buffer2, out offset2, out realSize2);
        int index = offset2;
        int num1 = realSize2;
        while (count != 0)
        {
          while (count != 0 && realSize2 != 0)
          {
            char ch = buffer[offset];
            if (ConverterUnicodeOutput.IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
            {
              int num2 = offset2;
              if (fallback.FallBackChar(ch, buffer2, ref offset2, offset2 + realSize2))
                realSize2 -= offset2 - num2;
              else
                break;
            }
            else
            {
              buffer2[offset2++] = ch;
              --realSize2;
            }
            this.isFirstChar = false;
            --count;
            ++offset;
          }
          if (offset2 - index != 0)
          {
            this.pushSink.Write(buffer2, index, offset2 - index);
            offset2 = index;
            realSize2 = num1;
          }
        }
      }
      else
      {
        if (count == 0)
          return;
        this.pushSink.Write(buffer, offset, count);
        this.isFirstChar = false;
      }
    }

    public override void Flush()
    {
      if (this.endOfFile)
        return;
      this.canRestart = false;
      this.FlushCached();
      if (this.pullSink == null)
        this.pushSink.Flush();
      else if (this.cache.Length == 0)
        this.pullSink.ReportEndOfFile();
      this.endOfFile = true;
    }

    public bool GetOutputChunk(out char[] chunkBuffer, out int chunkOffset, out int chunkLength)
    {
      if (this.cache.Length == 0 || this.canRestart)
      {
        chunkBuffer = (char[]) null;
        chunkOffset = 0;
        chunkLength = 0;
        return false;
      }
      this.cache.GetData(out chunkBuffer, out chunkOffset, out chunkLength);
      return true;
    }

    public void ReportOutput(int readCount)
    {
      this.cache.ReportRead(readCount);
      if (this.cache.Length != 0 || !this.endOfFile)
        return;
      this.pullSink.ReportEndOfFile();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.cache != null && this.cache is IDisposable)
        ((IDisposable) this.cache).Dispose();
      this.cache = (TextCache) null;
      this.pushSink = (TextWriter) null;
      this.pullSink = (ConverterReader) null;
      base.Dispose(disposing);
    }

    private static bool IsUnsafeCharacter(char ch, byte[] unsafeAsciiMap, byte unsafeAsciiMask, bool hasUnsafeUnicode, bool isFirstChar, IFallback fallback)
    {
      if (unsafeAsciiMap == null)
        return false;
      return (int) ch < unsafeAsciiMap.Length && ((int) unsafeAsciiMap[(int) ch] & (int) unsafeAsciiMask) != 0 || hasUnsafeUnicode && (int) ch >= (int) sbyte.MaxValue && fallback.IsUnsafeUnicode(ch, isFirstChar);
    }

    private void Reinitialize()
    {
      this.endOfFile = false;
      this.cache.Reset();
      this.canRestart = this.restartable;
      this.isFirstChar = true;
    }

    private bool FlushCached()
    {
      if (this.canRestart || this.cache.Length == 0)
        return false;
      if (this.pullSink == null)
      {
        while (this.cache.Length != 0)
        {
          char[] outputBuffer;
          int outputOffset;
          int outputCount;
          this.cache.GetData(out outputBuffer, out outputOffset, out outputCount);
          this.pushSink.Write(outputBuffer, outputOffset, outputCount);
          this.cache.ReportRead(outputCount);
        }
      }
      else
      {
        char[] outputBuffer;
        int outputIndex;
        int outputCount;
        this.pullSink.GetOutputBuffer(out outputBuffer, out outputIndex, out outputCount);
        this.pullSink.ReportOutput(this.cache.Read(outputBuffer, outputIndex, outputCount));
        if (this.cache.Length == 0 && this.endOfFile)
          this.pullSink.ReportEndOfFile();
      }
      return true;
    }
  }
}
