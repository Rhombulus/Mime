// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  public class ConverterReader : System.IO.TextReader, IProgressMonitor
  {
    private ConverterUnicodeOutput sourceOutputObject;
    private IProducerConsumer producer;
    private bool madeProgress;
    private int maxLoopsWithoutProgress;
    private char[] writeBuffer;
    private int writeIndex;
    private int writeCount;
    private object source;
    private bool endOfFile;
    private bool inconsistentState;

    public ConverterReader(Stream sourceStream, TextConverter converter)
    {
      if (sourceStream == null)
        throw new ArgumentNullException("sourceStream");
      if (converter == null)
        throw new ArgumentNullException("converter");
      if (!sourceStream.CanRead)
        throw new ArgumentException(CtsResources.TextConvertersStrings.CannotReadFromSource, "sourceStream");
      this.producer = converter.CreatePullChain(sourceStream, this);
      this.source = (object) sourceStream;
      this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
    }

    public ConverterReader(System.IO.TextReader sourceReader, TextConverter converter)
    {
      if (sourceReader == null)
        throw new ArgumentNullException("sourceReader");
      if (converter == null)
        throw new ArgumentNullException("converter");
      this.producer = converter.CreatePullChain(sourceReader, this);
      this.source = (object) sourceReader;
      this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
    }

    public override int Peek()
    {
      if (this.source == null)
        throw new ObjectDisposedException("ConverterReader");
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterReaderInInconsistentStare);
      long num = 0L;
      this.inconsistentState = true;
      while (!this.endOfFile)
      {
        char[] chunkBuffer;
        int chunkOffset;
        int chunkLength;
        if (this.sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkOffset, out chunkLength))
        {
          this.inconsistentState = false;
          return (int) chunkBuffer[chunkOffset];
        }
        this.producer.Run();
        if (this.madeProgress)
        {
          this.madeProgress = false;
          num = 0L;
        }
        else if ((long) this.maxLoopsWithoutProgress == num++)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToProduceOutput);
      }
      this.inconsistentState = false;
      return -1;
    }

    public override int Read()
    {
      if (this.source == null)
        throw new ObjectDisposedException("ConverterReader");
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterReaderInInconsistentStare);
      long num = 0L;
      this.inconsistentState = true;
      while (!this.endOfFile)
      {
        char[] chunkBuffer;
        int chunkOffset;
        int chunkLength;
        if (this.sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkOffset, out chunkLength))
        {
          this.sourceOutputObject.ReportOutput(1);
          this.inconsistentState = false;
          return (int) chunkBuffer[chunkOffset];
        }
        this.producer.Run();
        if (this.madeProgress)
        {
          this.madeProgress = false;
          num = 0L;
        }
        else if ((long) this.maxLoopsWithoutProgress == num++)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToProduceOutput);
      }
      this.inconsistentState = false;
      return -1;
    }

    public override int Read(char[] buffer, int index, int count)
    {
      if (this.source == null)
        throw new ObjectDisposedException("ConverterReader");
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException("index", CtsResources.TextConvertersStrings.IndexOutOfRange);
      if (count < 0 || count > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (buffer.Length - index < count)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterReaderInInconsistentStare);
      int num1 = count;
      int chunkLength;
      char[] chunkBuffer;
      int chunkOffset;
      while (count != 0 && this.sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkOffset, out chunkLength))
      {
        int readCount = Math.Min(chunkLength, count);
        Buffer.BlockCopy((Array) chunkBuffer, chunkOffset * 2, (Array) buffer, index * 2, readCount * 2);
        index += readCount;
        count -= readCount;
        this.sourceOutputObject.ReportOutput(readCount);
      }
      if (count != 0)
      {
        long num2 = 0L;
        this.writeBuffer = buffer;
        this.writeIndex = index;
        this.writeCount = count;
        this.inconsistentState = true;
        while (this.writeCount != 0 && !this.endOfFile)
        {
          this.producer.Run();
          if (this.madeProgress)
          {
            this.madeProgress = false;
            num2 = 0L;
          }
          else if ((long) this.maxLoopsWithoutProgress == num2++)
            throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToProduceOutput);
        }
        count = this.writeCount;
        this.writeBuffer = (char[]) null;
        this.writeIndex = 0;
        this.writeCount = 0;
        this.inconsistentState = false;
      }
      return num1 - count;
    }

    internal void SetSource(ConverterUnicodeOutput sourceOutputObject)
    {
      this.sourceOutputObject = sourceOutputObject;
    }

    internal void GetOutputBuffer(out char[] outputBuffer, out int outputIndex, out int outputCount)
    {
      outputBuffer = this.writeBuffer;
      outputIndex = this.writeIndex;
      outputCount = this.writeCount;
    }

    internal void ReportOutput(int outputCount)
    {
      if (outputCount == 0)
        return;
      this.writeCount -= outputCount;
      this.writeIndex += outputCount;
      this.madeProgress = true;
    }

    internal void ReportEndOfFile()
    {
      this.endOfFile = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.source != null)
        {
          if (this.source is Stream)
            ((Stream) this.source).Dispose();
          else
            ((System.IO.TextReader) this.source).Dispose();
        }
        if (this.producer != null && this.producer is IDisposable)
          ((IDisposable) this.producer).Dispose();
      }
      this.source = (object) null;
      this.producer = (IProducerConsumer) null;
      this.sourceOutputObject = (ConverterUnicodeOutput) null;
      this.writeBuffer = (char[]) null;
      base.Dispose(disposing);
    }

    void IProgressMonitor.ReportProgress()
    {
      this.madeProgress = true;
    }

    internal void Reuse(object newSource)
    {
      if (!(this.producer is IReusable))
        throw new NotSupportedException("this converter is not reusable");
      ((IReusable) this.producer).Initialize(newSource);
      this.source = newSource;
      this.writeBuffer = (char[]) null;
      this.writeIndex = 0;
      this.writeCount = 0;
      this.endOfFile = false;
      this.inconsistentState = false;
    }
  }
}
