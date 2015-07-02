// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TextToRtf
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class TextToRtf : TextConverter
  {
    private bool detectEncodingFromByteOrderMark = true;
    private int testMaxTokenRuns = 512;
    private bool testTraceShowTokenNum = true;
    private Encoding inputEncoding;
    private bool unwrapFlowed;
    private bool unwrapDelSp;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private Stream testFormatTraceStream;
    private Stream testFormatOutputTraceStream;
    private Stream testFormatConverterTraceStream;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;

    public Encoding InputEncoding
    {
      get
      {
        return this.inputEncoding;
      }
      set
      {
        this.AssertNotLocked();
        this.inputEncoding = value;
      }
    }

    public bool DetectEncodingFromByteOrderMark
    {
      get
      {
        return this.detectEncodingFromByteOrderMark;
      }
      set
      {
        this.AssertNotLocked();
        this.detectEncodingFromByteOrderMark = value;
      }
    }

    public bool Unwrap
    {
      get
      {
        return this.unwrapFlowed;
      }
      set
      {
        this.AssertNotLocked();
        this.unwrapFlowed = value;
      }
    }

    internal bool UnwrapDeleteSpace
    {
      get
      {
        return this.unwrapDelSp;
      }
      set
      {
        this.AssertNotLocked();
        this.unwrapDelSp = value;
      }
    }

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

    internal TextToRtf SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal TextToRtf SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal TextToRtf SetUnwrap(bool value)
    {
      this.Unwrap = value;
      return this;
    }

    internal TextToRtf SetUnwrapDeleteSpace(bool value)
    {
      this.UnwrapDeleteSpace = value;
      return this;
    }

    internal TextToRtf SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal TextToRtf SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal TextToRtf SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal TextToRtf SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal TextToRtf SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      int num = value ? 1 : 0;
      return this;
    }

    internal TextToRtf SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal TextToRtf SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal TextToRtf SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal TextToRtf SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal TextToRtf SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
      return this;
    }

    internal TextToRtf SetTestFormatOutputTraceStream(Stream value)
    {
      this.testFormatOutputTraceStream = value;
      return this;
    }

    internal TextToRtf SetTestFormatConverterTraceStream(Stream value)
    {
      this.testFormatConverterTraceStream = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput(output, true, false, this.testBoundaryConditions, (IResultsFeedback) this, (ImageRenderingCallbackInternal) null, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.inputEncoding), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextWriterUnsupported);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, 4096, this.testBoundaryConditions, (IProgressMonitor) null), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput(output, true, false, this.testBoundaryConditions, (IResultsFeedback) this, (ImageRenderingCallbackInternal) null, this.testFormatTraceStream, this.testFormatOutputTraceStream, (Encoding) null), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextWriterUnsupported);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
    {
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterStream), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput((Stream) converterStream, false, false, this.testBoundaryConditions, (IResultsFeedback) this, (ImageRenderingCallbackInternal) null, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.inputEncoding), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, 4096, this.testBoundaryConditions, (IProgressMonitor) converterStream), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput((Stream) converterStream, false, false, this.testBoundaryConditions, (IResultsFeedback) this, (ImageRenderingCallbackInternal) null, this.testFormatTraceStream, this.testFormatOutputTraceStream, (Encoding) null), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.CannotUseConverterReader);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterReader converterReader)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.CannotUseConverterReader);
    }

    private IProducerConsumer CreateChain(ConverterInput input, Internal.Format.FormatOutput output, IProgressMonitor progressMonitor)
    {
      this.locked = true;
      Injection injection = (Injection) null;
      if (this.injectHead != null || this.injectTail != null)
        injection = this.injectionFormat == HeaderFooterFormat.Html ? (Injection) new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, (Stream) null, progressMonitor) : (Injection) new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, (Stream) null, progressMonitor);
      return (IProducerConsumer) new Internal.Text.TextFormatConverter(new Internal.Text.TextParser(input, this.unwrapFlowed, this.unwrapDelSp, this.testMaxTokenRuns, this.testBoundaryConditions), output, injection, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream);
    }

    internal override void SetResult(ConfigParameter parameterId, object val)
    {
      if (parameterId == ConfigParameter.InputEncoding)
        this.inputEncoding = (Encoding) val;
      base.SetResult(parameterId, val);
    }
  }
}
