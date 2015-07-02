// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TextConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  public abstract class TextConverter : IResultsFeedback
  {
    private int inputBufferSize = 4096;
    private int outputBufferSize = 4096;
    protected bool testBoundaryConditions;
    protected bool locked;

    internal bool TestBoundaryConditions
    {
      get
      {
        return this.testBoundaryConditions;
      }
      set
      {
        this.AssertNotLocked();
        this.testBoundaryConditions = value;
      }
    }

    public int InputStreamBufferSize
    {
      get
      {
        return this.inputBufferSize;
      }
      set
      {
        this.AssertNotLocked();
        if (value < 1024 || value > 81920)
          throw new ArgumentOutOfRangeException("value", CtsResources.TextConvertersStrings.BufferSizeValueRange);
        this.inputBufferSize = value;
      }
    }

    public int OutputStreamBufferSize
    {
      get
      {
        return this.outputBufferSize;
      }
      set
      {
        this.AssertNotLocked();
        if (value < 1024 || value > 81920)
          throw new ArgumentOutOfRangeException("value", CtsResources.TextConvertersStrings.BufferSizeValueRange);
        this.outputBufferSize = value;
      }
    }

    internal TextConverter()
    {
    }

    public void Convert(Stream sourceStream, Stream destinationStream)
    {
      if (destinationStream == null)
        throw new ArgumentNullException("destinationStream");
      Stream stream = (Stream) new ConverterStream(sourceStream, this, ConverterStreamAccess.Read);
      byte[] buffer = new byte[this.outputBufferSize];
      while (true)
      {
        int count = stream.Read(buffer, 0, buffer.Length);
        if (count != 0)
          destinationStream.Write(buffer, 0, count);
        else
          break;
      }
      destinationStream.Flush();
    }

    public void Convert(Stream sourceStream, TextWriter destinationWriter)
    {
      if (destinationWriter == null)
        throw new ArgumentNullException("destinationWriter");
      System.IO.TextReader textReader = (System.IO.TextReader) new ConverterReader(sourceStream, this);
      char[] buffer = new char[4096];
      while (true)
      {
        int count = textReader.Read(buffer, 0, buffer.Length);
        if (count != 0)
          destinationWriter.Write(buffer, 0, count);
        else
          break;
      }
      destinationWriter.Flush();
    }

    public void Convert(System.IO.TextReader sourceReader, Stream destinationStream)
    {
      if (destinationStream == null)
        throw new ArgumentNullException("destinationStream");
      Stream stream = (Stream) new ConverterStream(sourceReader, this);
      byte[] buffer = new byte[this.outputBufferSize];
      while (true)
      {
        int count = stream.Read(buffer, 0, buffer.Length);
        if (count != 0)
          destinationStream.Write(buffer, 0, count);
        else
          break;
      }
      destinationStream.Flush();
    }

    public void Convert(System.IO.TextReader sourceReader, TextWriter destinationWriter)
    {
      if (destinationWriter == null)
        throw new ArgumentNullException("destinationWriter");
      System.IO.TextReader textReader = (System.IO.TextReader) new ConverterReader(sourceReader, this);
      char[] buffer = new char[4096];
      while (true)
      {
        int count = textReader.Read(buffer, 0, buffer.Length);
        if (count != 0)
          destinationWriter.Write(buffer, 0, count);
        else
          break;
      }
      destinationWriter.Flush();
    }

    internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output);

    internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output);

    internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output);

    internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output);

    internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream);

    internal abstract IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream);

    internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader);

    internal abstract IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterReader converterReader);

    internal virtual void SetResult(ConfigParameter parameterId, object val)
    {
    }

    void IResultsFeedback.Set(ConfigParameter parameterId, object val)
    {
      this.SetResult(parameterId, val);
    }

    internal void AssertNotLocked()
    {
      if (this.locked)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.ParametersCannotBeChangedAfterConverterObjectIsUsed);
    }
  }
}
