// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.RtfToRtf
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  public class RtfToRtf : TextConverter
  {
    private bool testTraceShowTokenNum = true;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
    private Stream testInjectionTraceStream;

    public HeaderFooterFormat HeaderFooterFormat
    {
      get
      {
        return this.injectionFormat;
      }
      set
      {
        this.AssertNotLocked();
        this.injectionFormat = value;
      }
    }

    public string Header
    {
      get
      {
        return this.injectHead;
      }
      set
      {
        this.AssertNotLocked();
        this.injectHead = value;
      }
    }

    public string Footer
    {
      get
      {
        return this.injectTail;
      }
      set
      {
        this.AssertNotLocked();
        this.injectTail = value;
      }
    }

    internal RtfToRtf SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal RtfToRtf SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal RtfToRtf SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal RtfToRtf SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal RtfToRtf SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      int num = value ? 1 : 0;
      return this;
    }

    internal RtfToRtf SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal RtfToRtf SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal RtfToRtf SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal RtfToRtf SetTestInjectionTraceStream(Stream value)
    {
      this.testInjectionTraceStream = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      return (IProducerConsumer) this.CreateChain((Stream) converterStream, output, true, (IProgressMonitor) converterStream);
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
      return (IProducerConsumer) this.CreateChain(input, (Stream) converterStream, false, (IProgressMonitor) converterStream);
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

    private Internal.Rtf.RtfToRtfConverter CreateChain(Stream input, Stream output, bool push, IProgressMonitor progressMonitor)
    {
      this.locked = true;
      Internal.Rtf.RtfParser parser = new Internal.Rtf.RtfParser(input, push, this.InputStreamBufferSize, this.testBoundaryConditions, push ? (IProgressMonitor) null : progressMonitor, (IReportBytes) null);
      Injection injection = (Injection) null;
      if (this.injectHead != null || this.injectTail != null)
        injection = this.injectionFormat == HeaderFooterFormat.Html ? (Injection) new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, this.testInjectionTraceStream, progressMonitor) : (Injection) new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, this.testInjectionTraceStream, progressMonitor);
      return new Internal.Rtf.RtfToRtfConverter(parser, output, push, injection, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
    }
  }
}
