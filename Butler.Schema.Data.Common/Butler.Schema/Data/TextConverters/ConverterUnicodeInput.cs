// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterUnicodeInput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class ConverterUnicodeInput : ConverterInput, IReusable
  {
    private System.IO.TextReader pullSource;
    private ConverterWriter pushSource;
    private char[] parseBuffer;
    private int parseStart;
    private int parseEnd;
    private char[] pushChunkBuffer;
    private int pushChunkStart;
    private int pushChunkCount;
    private int pushChunkUsed;

    public ConverterUnicodeInput(object source, bool push, int maxParseToken, bool testBoundaryConditions, IProgressMonitor progressMonitor)
      : base(progressMonitor)
    {
      if (push)
        this.pushSource = source as ConverterWriter;
      else
        this.pullSource = source as System.IO.TextReader;
      this.maxTokenSize = maxParseToken;
      this.parseBuffer = new char[testBoundaryConditions ? 123 : 4096];
      if (this.pushSource == null)
        return;
      this.pushSource.SetSink(this);
    }

    private void Reinitialize()
    {
      this.parseStart = this.parseEnd = 0;
      this.pushChunkStart = 0;
      this.pushChunkCount = 0;
      this.pushChunkUsed = 0;
      this.pushChunkBuffer = (char[]) null;
      this.endOfFile = false;
    }

    void IReusable.Initialize(object newSourceOrDestination)
    {
      if (this.pullSource != null && newSourceOrDestination != null)
      {
        System.IO.TextReader textReader = newSourceOrDestination as System.IO.TextReader;
        if (textReader == null)
          throw new InvalidOperationException("cannot reinitialize this converter - new input should be a TextReader object");
        this.pullSource = textReader;
      }
      this.Reinitialize();
    }

    public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
    {
      int num1 = this.parseEnd - end;
      if (this.parseBuffer.Length - this.parseEnd <= 1 && !this.EnsureFreeSpace() && num1 == 0)
        return true;
      while (!this.endOfFile && this.parseBuffer.Length - this.parseEnd > 1)
      {
        if (this.pushSource != null)
        {
          if (this.pushChunkCount != 0 || this.pushSource.GetInputChunk(out this.pushChunkBuffer, out this.pushChunkStart, out this.pushChunkCount, out this.endOfFile))
          {
            if (this.pushChunkCount - this.pushChunkUsed != 0)
            {
              int num2 = Math.Min(this.pushChunkCount - this.pushChunkUsed, this.parseBuffer.Length - this.parseEnd - 1);
              Buffer.BlockCopy((Array) this.pushChunkBuffer, (this.pushChunkStart + this.pushChunkUsed) * 2, (Array) this.parseBuffer, this.parseEnd * 2, num2 * 2);
              this.pushChunkUsed += num2;
              this.parseEnd += num2;
              this.parseBuffer[this.parseEnd] = char.MinValue;
              num1 += num2;
              if (this.pushChunkCount - this.pushChunkUsed == 0)
              {
                this.pushSource.ReportRead(this.pushChunkCount);
                this.pushChunkStart = 0;
                this.pushChunkCount = 0;
                this.pushChunkUsed = 0;
                this.pushChunkBuffer = (char[]) null;
              }
            }
          }
          else
            break;
        }
        else
        {
          int num2 = this.pullSource.Read(this.parseBuffer, this.parseEnd, this.parseBuffer.Length - this.parseEnd - 1);
          if (num2 == 0)
          {
            this.endOfFile = true;
          }
          else
          {
            this.parseEnd += num2;
            this.parseBuffer[this.parseEnd] = char.MinValue;
            num1 += num2;
          }
          if (this.progressMonitor != null)
            this.progressMonitor.ReportProgress();
        }
      }
      buffer = this.parseBuffer;
      if (start != this.parseStart)
      {
        current = this.parseStart + (current - start);
        start = this.parseStart;
      }
      end = this.parseEnd;
      if (num1 == 0)
        return this.endOfFile;
      return true;
    }

    public override void ReportProcessed(int processedSize)
    {
      this.parseStart += processedSize;
    }

    public override int RemoveGap(int gapBegin, int gapEnd)
    {
      if (gapEnd == this.parseEnd)
      {
        this.parseEnd = gapBegin;
        this.parseBuffer[gapBegin] = char.MinValue;
        return gapBegin;
      }
      Buffer.BlockCopy((Array) this.parseBuffer, gapEnd, (Array) this.parseBuffer, gapBegin, this.parseEnd - gapEnd);
      this.parseEnd = gapBegin + (this.parseEnd - gapEnd);
      this.parseBuffer[this.parseEnd] = char.MinValue;
      return this.parseEnd;
    }

    public void GetInputBuffer(out char[] inputBuffer, out int inputOffset, out int inputCount, out int parseCount)
    {
      inputBuffer = this.parseBuffer;
      inputOffset = this.parseEnd;
      inputCount = this.parseBuffer.Length - this.parseEnd - 1;
      parseCount = this.parseEnd - this.parseStart;
    }

    public void Commit(int inputCount)
    {
      this.parseEnd += inputCount;
      this.parseBuffer[this.parseEnd] = char.MinValue;
    }

    protected override void Dispose(bool disposing)
    {
      this.pullSource = (System.IO.TextReader) null;
      this.pushSource = (ConverterWriter) null;
      this.parseBuffer = (char[]) null;
      this.pushChunkBuffer = (char[]) null;
      base.Dispose(disposing);
    }

    private bool EnsureFreeSpace()
    {
      if (this.parseBuffer.Length - (this.parseEnd - this.parseStart) <= 1 || this.parseStart < 1 && (long) this.parseBuffer.Length < (long) this.maxTokenSize + 1L)
      {
        if ((long) this.parseBuffer.Length >= (long) this.maxTokenSize + 1L)
          return false;
        long num = (long) (this.parseBuffer.Length * 2);
        if (num > (long) this.maxTokenSize + 1L)
          num = (long) this.maxTokenSize + 1L;
        if (num > (long) int.MaxValue)
          num = (long) int.MaxValue;
        char[] chArray = new char[(int) num];
        Buffer.BlockCopy((Array) this.parseBuffer, this.parseStart * 2, (Array) chArray, 0, (this.parseEnd - this.parseStart + 1) * 2);
        this.parseBuffer = chArray;
        this.parseEnd = this.parseEnd - this.parseStart;
        this.parseStart = 0;
      }
      else
      {
        Buffer.BlockCopy((Array) this.parseBuffer, this.parseStart * 2, (Array) this.parseBuffer, 0, (this.parseEnd - this.parseStart + 1) * 2);
        this.parseEnd = this.parseEnd - this.parseStart;
        this.parseStart = 0;
      }
      return true;
    }
  }
}
