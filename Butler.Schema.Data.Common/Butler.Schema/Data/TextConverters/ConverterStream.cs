// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterStream
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  public class ConverterStream : Stream, IProgressMonitor
  {
    private IProducerConsumer consumer;
    private int maxLoopsWithoutProgress;
    private bool madeProgress;
    private byte[] chunkToReadBuffer;
    private int chunkToReadOffset;
    private int chunkToReadCount;
    private IByteSource byteSource;
    private IProducerConsumer producer;
    private byte[] writeBuffer;
    private int writeOffset;
    private int writeCount;
    private object sourceOrDestination;
    private bool endOfFile;
    private bool inconsistentState;

    public override bool CanRead => this.producer != null;

      public override bool CanWrite => this.consumer != null;

      public override bool CanSeek => false;

      public override long Length
    {
      get
      {
        throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
      }
    }

    public override long Position
    {
      get
      {
        throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
      }
      set
      {
        throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
      }
    }

    public ConverterStream(Stream stream, TextConverter converter, ConverterStreamAccess access)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (converter == null)
        throw new ArgumentNullException("converter");
      if (access < ConverterStreamAccess.Read || ConverterStreamAccess.Write < access)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AccessShouldBeReadOrWrite, "access");
      if (access == ConverterStreamAccess.Read)
      {
        if (!stream.CanRead)
          throw new ArgumentException(CtsResources.TextConvertersStrings.CannotReadFromSource, "stream");
        this.producer = converter.CreatePullChain(stream, this);
      }
      else
      {
        if (!stream.CanWrite)
          throw new ArgumentException(CtsResources.TextConvertersStrings.CannotWriteToDestination, "stream");
        this.consumer = converter.CreatePushChain(this, stream);
      }
      this.sourceOrDestination = (object) stream;
      this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
    }

    public ConverterStream(System.IO.TextReader sourceReader, TextConverter converter)
    {
      if (sourceReader == null)
        throw new ArgumentNullException("sourceReader");
      if (converter == null)
        throw new ArgumentNullException("converter");
      this.producer = converter.CreatePullChain(sourceReader, this);
      this.sourceOrDestination = (object) sourceReader;
      this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
    }

    public ConverterStream(TextWriter destinationWriter, TextConverter converter)
    {
      if (destinationWriter == null)
        throw new ArgumentNullException("destinationWriter");
      if (converter == null)
        throw new ArgumentNullException("converter");
      this.consumer = converter.CreatePushChain(this, destinationWriter);
      this.sourceOrDestination = (object) destinationWriter;
      this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.sourceOrDestination == null)
        throw new ObjectDisposedException("ConverterStream");
      if (this.consumer == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.WriteUnsupported);
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TextConvertersStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      if (this.endOfFile)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.WriteAfterFlush);
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterStreamInInconsistentStare);
      this.chunkToReadBuffer = buffer;
      this.chunkToReadOffset = offset;
      this.chunkToReadCount = count;
      long num = 0L;
      this.inconsistentState = true;
      while (this.chunkToReadCount != 0)
      {
        this.consumer.Run();
        if (this.madeProgress)
        {
          num = 0L;
          this.madeProgress = false;
        }
        else if ((long) this.maxLoopsWithoutProgress == num++)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToProcessInput);
      }
      this.inconsistentState = false;
      this.chunkToReadBuffer = (byte[]) null;
    }

    public override void Flush()
    {
      if (this.sourceOrDestination == null)
        throw new ObjectDisposedException("ConverterStream");
      if (this.consumer == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.WriteUnsupported);
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
      if (this.sourceOrDestination is Stream)
      {
        ((Stream) this.sourceOrDestination).Flush();
      }
      else
      {
        if (!(this.sourceOrDestination is TextWriter))
          return;
        ((TextWriter) this.sourceOrDestination).Flush();
      }
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing)
        {
          if (this.sourceOrDestination != null && this.consumer != null && !this.inconsistentState)
            this.Flush();
          if (this.producer != null && this.producer is IDisposable)
            ((IDisposable) this.producer).Dispose();
          if (this.consumer != null)
          {
            if (this.consumer is IDisposable)
              ((IDisposable) this.consumer).Dispose();
          }
        }
      }
      finally
      {
        if (disposing && this.sourceOrDestination != null)
        {
          if (this.sourceOrDestination is Stream)
            ((Stream) this.sourceOrDestination).Dispose();
          else if (this.sourceOrDestination is System.IO.TextReader)
            ((System.IO.TextReader) this.sourceOrDestination).Dispose();
          else
            ((TextWriter) this.sourceOrDestination).Dispose();
        }
        this.sourceOrDestination = (object) null;
        this.consumer = (IProducerConsumer) null;
        this.producer = (IProducerConsumer) null;
        this.chunkToReadBuffer = (byte[]) null;
        this.writeBuffer = (byte[]) null;
        this.byteSource = (IByteSource) null;
      }
      base.Dispose(disposing);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.sourceOrDestination == null)
        throw new ObjectDisposedException("ConverterStream");
      if (this.producer == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ReadUnsupported);
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TextConvertersStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      if (this.inconsistentState)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ConverterStreamInInconsistentStare);
      int num1 = count;
      if (this.byteSource != null)
      {
        int chunkLength;
        byte[] chunkBuffer;
        int chunkOffset;
        while (count != 0 && this.byteSource.GetOutputChunk(out chunkBuffer, out chunkOffset, out chunkLength))
        {
          int num2 = Math.Min(chunkLength, count);
          Buffer.BlockCopy((Array) chunkBuffer, chunkOffset, (Array) buffer, offset, num2);
          offset += num2;
          count -= num2;
          this.byteSource.ReportOutput(num2);
        }
      }
      if (count != 0)
      {
        long num2 = 0L;
        this.writeBuffer = buffer;
        this.writeOffset = offset;
        this.writeCount = count;
        this.inconsistentState = true;
        while (this.writeCount != 0 && !this.endOfFile)
        {
          this.producer.Run();
          if (this.madeProgress)
          {
            num2 = 0L;
            this.madeProgress = false;
          }
          else if ((long) this.maxLoopsWithoutProgress == num2++)
            throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToProduceOutput);
        }
        count = this.writeCount;
        this.writeBuffer = (byte[]) null;
        this.writeOffset = 0;
        this.writeCount = 0;
        this.inconsistentState = false;
      }
      return num1 - count;
    }

    internal void SetSource(IByteSource byteSource)
    {
      this.byteSource = byteSource;
    }

    internal void GetOutputBuffer(out byte[] outputBuffer, out int outputOffset, out int outputCount)
    {
      outputBuffer = this.writeBuffer;
      outputOffset = this.writeOffset;
      outputCount = this.writeCount;
    }

    internal void ReportOutput(int outputCount)
    {
      if (outputCount == 0)
        return;
      this.madeProgress = true;
      this.writeCount -= outputCount;
      this.writeOffset += outputCount;
    }

    internal void ReportEndOfFile()
    {
      this.endOfFile = true;
    }

    internal bool GetInputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount, out bool eof)
    {
      chunkBuffer = this.chunkToReadBuffer;
      chunkOffset = this.chunkToReadOffset;
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
      this.madeProgress = true;
      this.chunkToReadCount -= readCount;
      this.chunkToReadOffset += readCount;
      if (this.chunkToReadCount != 0)
        return;
      this.chunkToReadBuffer = (byte[]) null;
      this.chunkToReadOffset = 0;
    }

    void IProgressMonitor.ReportProgress()
    {
      this.madeProgress = true;
    }

    internal void Reuse(object newSourceOrSink)
    {
      if (this.producer != null)
      {
        if (!(this.producer is IReusable))
          throw new NotSupportedException("this converter is not reusable");
        ((IReusable) this.producer).Initialize(newSourceOrSink);
      }
      else
      {
        if (!(this.consumer is IReusable))
          throw new NotSupportedException("this converter is not reusable");
        ((IReusable) this.consumer).Initialize(newSourceOrSink);
      }
      this.sourceOrDestination = newSourceOrSink;
      this.chunkToReadBuffer = (byte[]) null;
      this.chunkToReadOffset = 0;
      this.chunkToReadCount = 0;
      this.writeBuffer = (byte[]) null;
      this.writeOffset = 0;
      this.writeCount = 0;
      this.endOfFile = false;
      this.inconsistentState = false;
    }
  }
}
