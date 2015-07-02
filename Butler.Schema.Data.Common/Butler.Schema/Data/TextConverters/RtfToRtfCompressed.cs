// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.RtfToRtfCompressed
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  public class RtfToRtfCompressed : TextConverter
  {
    private RtfCompressionMode compressionMode;

    public RtfCompressionMode CompressionMode
    {
      get
      {
        return this.compressionMode;
      }
      set
      {
        this.AssertNotLocked();
        this.compressionMode = value;
      }
    }

    internal RtfToRtfCompressed SetCompressionMode(RtfCompressionMode value)
    {
      this.CompressionMode = value;
      return this;
    }

    internal RtfToRtfCompressed SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal RtfToRtfCompressed SetOutputStreamBufferSize(int value)
    {
      this.OutputStreamBufferSize = value;
      return this;
    }

    internal RtfToRtfCompressed SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      return (IProducerConsumer) new Internal.RtfCompressed.RtfCompressConverter((Stream) converterStream, true, output, this.compressionMode, this.InputStreamBufferSize, this.OutputStreamBufferSize);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextWriterUnsupported);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.CannotUseConverterWriter);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.CannotUseConverterWriter);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
    {
      return (IProducerConsumer) new Internal.RtfCompressed.RtfCompressConverter(input, false, (Stream) converterStream, this.compressionMode, this.InputStreamBufferSize, this.OutputStreamBufferSize);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextReaderUnsupported);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.CannotUseConverterReader);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterReader converterReader)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextReaderUnsupported);
    }
  }
}
