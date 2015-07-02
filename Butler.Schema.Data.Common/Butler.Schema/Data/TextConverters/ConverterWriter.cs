// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterWriter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class ConverterWriter : TextWriter, IProgressMonitor
  {
    private ConverterUnicodeInput sinkInputObject;
    private IProducerConsumer consumer;
    private bool madeProgress;
    private int maxLoopsWithoutProgress;
    private char[] chunkToReadBuffer;
    private int chunkToReadIndex;
    private int chunkToReadCount;
    private object destination;
    private bool endOfFile;
    private bool inconsistentState;
    private bool boundaryTesting;

    public override Encoding Encoding => (Encoding) null;

      public ConverterWriter(Stream destinationStream, TextConverter converter)
    {
      if (destinationStream == null)
        throw new ArgumentNullException("destinationStream");
      if (converter == null)
        throw new ArgumentNullException("converter");
      if (!destinationStream.CanWrite)
        throw new ArgumentException(CtsResources.TextConvertersStrings.CannotWriteToDestination, "destinationStream");
      this.consumer = converter.CreatePushChain(this, destinationStream);
      this.destination = (object) destinationStream;
      this.boundaryTesting = converter.TestBoundaryConditions;
      this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
    }

    public ConverterWriter(TextWriter destinationWriter, TextConverter converter)
    {
      if (destinationWriter == null)
        throw new ArgumentNullException("destinationWriter");
      if (converter == null)
        throw new ArgumentNullException("converter");
      this.consumer = converter.CreatePushChain(this, destinationWriter);
      this.destination = (object) destinationWriter;
      this.boundaryTesting = converter.TestBoundaryConditions;
      this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
    }

    public override void Flush()
    {
      if (this.destination == null)
        throw new ObjectDisposedException("ConverterWriter");
      this.endOfFile = true;
      if (!this.inconsistentState)
      {
        long num = 0L;
        this.inconsistentState = true;
        while (!this.consumer.Flush())
        {
          if (this.madeProgress)
          {
            num = 0L;
            this.madeProgress = false;
          }
          else if ((long) this.maxLoopsWithoutProgress == num++)
            throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToFlushConverter);
        }
        this.inconsistentState = false;
      }
      if (this.destination is Stream)
        ((Stream) this.destination).Flush();
      else
        ((TextWriter) this.destination).Flush();
    }

    public override void Write(char value)
    {
      if (this.destination == null)
        throw new ObjectDisposedException("ConverterWriter");
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterWriterInInconsistentStare);
      int parseCount = 10000;
      if (!this.boundaryTesting)
      {
        char[] inputBuffer;
        int inputOffset;
        int inputCount;
        this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputOffset, out inputCount, out parseCount);
        if (inputCount >= 1)
        {
          inputBuffer[inputOffset] = value;
          this.sinkInputObject.Commit(1);
          return;
        }
      }
      this.WriteBig(new char[1]
      {
        value
      }, 0, 1, parseCount);
    }

    public override void Write(char[] buffer)
    {
      if (this.destination == null)
        throw new ObjectDisposedException("ConverterWriter");
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterWriterInInconsistentStare);
      if (buffer == null)
        return;
      int parseCount = 10000;
      if (!this.boundaryTesting)
      {
        char[] inputBuffer;
        int inputOffset;
        int inputCount;
        this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputOffset, out inputCount, out parseCount);
        if (inputCount >= buffer.Length)
        {
          Buffer.BlockCopy((Array) buffer, 0, (Array) inputBuffer, inputOffset * 2, buffer.Length * 2);
          this.sinkInputObject.Commit(buffer.Length);
          return;
        }
      }
      this.WriteBig(buffer, 0, buffer.Length, parseCount);
    }

    public override void Write(char[] buffer, int index, int count)
    {
      if (this.destination == null)
        throw new ObjectDisposedException("ConverterWriter");
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException("index", CtsResources.TextConvertersStrings.IndexOutOfRange);
      if (count < 0 || count > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (buffer.Length - index < count)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterWriterInInconsistentStare);
      int parseCount = 10000;
      if (!this.boundaryTesting)
      {
        char[] inputBuffer;
        int inputOffset;
        int inputCount;
        this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputOffset, out inputCount, out parseCount);
        if (inputCount >= count)
        {
          Buffer.BlockCopy((Array) buffer, index * 2, (Array) inputBuffer, inputOffset * 2, count * 2);
          this.sinkInputObject.Commit(count);
          return;
        }
      }
      this.WriteBig(buffer, index, count, parseCount);
    }

    public override void Write(string value)
    {
      if (this.destination == null)
        throw new ObjectDisposedException("ConverterWriter");
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterWriterInInconsistentStare);
      if (value == null)
        return;
      int parseCount = 10000;
      if (!this.boundaryTesting)
      {
        char[] inputBuffer;
        int inputOffset;
        int inputCount;
        this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputOffset, out inputCount, out parseCount);
        if (inputCount >= value.Length)
        {
          value.CopyTo(0, inputBuffer, inputOffset, value.Length);
          this.sinkInputObject.Commit(value.Length);
          return;
        }
      }
      this.WriteBig(value.ToCharArray(), 0, value.Length, parseCount);
    }

    public override void WriteLine(string value)
    {
      this.Write(value);
      this.WriteLine();
    }

    internal void SetSink(ConverterUnicodeInput sinkInputObject)
    {
      this.sinkInputObject = sinkInputObject;
    }

    internal bool GetInputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount, out bool eof)
    {
      chunkBuffer = this.chunkToReadBuffer;
      chunkIndex = this.chunkToReadIndex;
      chunkCount = this.chunkToReadCount;
      eof = this.endOfFile && 0 == this.chunkToReadCount;
      if (this.chunkToReadCount == 0)
        return this.endOfFile;
      return true;
    }

    internal void ReportRead(int readCount)
    {
      if (readCount == 0)
        return;
      this.chunkToReadCount -= readCount;
      this.chunkToReadIndex += readCount;
      if (this.chunkToReadCount == 0)
      {
        this.chunkToReadBuffer = (char[]) null;
        this.chunkToReadIndex = 0;
      }
      this.madeProgress = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.destination != null)
        {
          if (!this.inconsistentState)
            this.Flush();
          if (this.destination is Stream)
            ((Stream) this.destination).Dispose();
          else
            ((TextWriter) this.destination).Dispose();
        }
        if (this.consumer != null && this.consumer is IDisposable)
          ((IDisposable) this.consumer).Dispose();
      }
      this.destination = (object) null;
      this.consumer = (IProducerConsumer) null;
      this.sinkInputObject = (ConverterUnicodeInput) null;
      this.chunkToReadBuffer = (char[]) null;
      base.Dispose(disposing);
    }

    private void WriteBig(char[] buffer, int index, int count, int parseCount)
    {
      this.chunkToReadBuffer = buffer;
      this.chunkToReadIndex = index;
      this.chunkToReadCount = count;
      long num = 0L;
      this.inconsistentState = true;
      while (this.chunkToReadCount != 0)
      {
        this.consumer.Run();
        if (this.madeProgress)
        {
          this.madeProgress = false;
          num = 0L;
        }
        else if ((long) this.maxLoopsWithoutProgress == num++)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToProcessInput);
      }
      this.inconsistentState = false;
    }

    void IProgressMonitor.ReportProgress()
    {
      this.madeProgress = true;
    }

    internal void Reuse(object newSink)
    {
      if (!(this.consumer is IReusable))
        throw new NotSupportedException("this converter is not reusable");
      ((IReusable) this.consumer).Initialize(newSink);
      this.destination = newSink;
      this.chunkToReadBuffer = (char[]) null;
      this.chunkToReadIndex = 0;
      this.chunkToReadCount = 0;
      this.endOfFile = false;
      this.inconsistentState = false;
    }
  }
}
