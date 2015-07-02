// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.RtfToText
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class RtfToText : TextConverter
  {
    private bool outputEncodingSameAsInput = true;
    private bool fallbacks = true;
    private bool testTraceShowTokenNum = true;
    private TextExtractionMode textExtractionMode;
    private Encoding outputEncoding;
    private bool wrapFlowed;
    private bool wrapDelSp;
    private bool htmlEscape;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private bool testPreserveTrailingSpaces;
    private bool testTreatNbspAsBreakable;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
    private Stream testFormatTraceStream;

    public TextExtractionMode TextExtractionMode
    {
      get
      {
        return this.textExtractionMode;
      }
    }

    public Encoding OutputEncoding
    {
      get
      {
        return this.outputEncoding;
      }
      set
      {
        this.AssertNotLocked();
        this.outputEncoding = value;
        this.outputEncodingSameAsInput = value == null;
      }
    }

    public bool Wrap
    {
      get
      {
        return this.wrapFlowed;
      }
      set
      {
        this.AssertNotLockedAndNotTextExtraction();
        this.wrapFlowed = value;
      }
    }

    internal bool WrapDeleteSpace
    {
      get
      {
        return this.wrapDelSp;
      }
      set
      {
        this.AssertNotLockedAndNotTextExtraction();
        this.wrapDelSp = value;
      }
    }

    public bool HtmlEscapeOutput
    {
      get
      {
        return this.htmlEscape;
      }
      set
      {
        this.AssertNotLockedAndNotTextExtraction();
        this.htmlEscape = value;
        if (!value)
          return;
        this.fallbacks = true;
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
        this.AssertNotLockedAndNotTextExtraction();
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
        this.AssertNotLockedAndNotTextExtraction();
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
        this.AssertNotLockedAndNotTextExtraction();
        this.injectTail = value;
      }
    }

    public RtfToText()
    {
      this.textExtractionMode = TextExtractionMode.NormalConversion;
    }

    public RtfToText(TextExtractionMode textExtractionMode)
    {
      this.textExtractionMode = textExtractionMode;
    }

    internal RtfToText SetOutputEncoding(Encoding value)
    {
      this.OutputEncoding = value;
      return this;
    }

    internal RtfToText SetWrap(bool value)
    {
      this.Wrap = value;
      return this;
    }

    internal RtfToText SetWrapDeleteSpace(bool value)
    {
      this.WrapDeleteSpace = value;
      return this;
    }

    internal RtfToText SetUseFallbacks(bool value)
    {
      this.fallbacks = value;
      return this;
    }

    internal RtfToText SetHtmlEscapeOutput(bool value)
    {
      this.HtmlEscapeOutput = value;
      return this;
    }

    internal RtfToText SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal RtfToText SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal RtfToText SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal RtfToText SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal RtfToText SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      int num = value ? 1 : 0;
      return this;
    }

    internal RtfToText SetTestPreserveTrailingSpaces(bool value)
    {
      this.testPreserveTrailingSpaces = value;
      return this;
    }

    internal RtfToText SetTestTreatNbspAsBreakable(bool value)
    {
      this.testTreatNbspAsBreakable = value;
      return this;
    }

    internal RtfToText SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal RtfToText SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal RtfToText SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal RtfToText SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      ConverterOutput output1 = (ConverterOutput) new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.GetEncoding("Windows-1252") : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this);
      return this.CreateChain((Stream) converterStream, true, output1, (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      ConverterOutput output1 = (ConverterOutput) new ConverterUnicodeOutput((object) output, true, false);
      return this.CreateChain((Stream) converterStream, true, output1, (IProgressMonitor) converterStream);
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
      ConverterOutput output = (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, false, this.outputEncodingSameAsInput ? Encoding.GetEncoding("Windows-1252") : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this);
      return this.CreateChain(input, false, output, (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextReaderUnsupported);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
    {
      ConverterOutput output = (ConverterOutput) new ConverterUnicodeOutput((object) converterReader, false, false);
      return this.CreateChain(input, false, output, (IProgressMonitor) converterReader);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterReader converterReader)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextReaderUnsupported);
    }

    private IProducerConsumer CreateChain(Stream input, bool push, ConverterOutput output, IProgressMonitor progressMonitor)
    {
      this.locked = true;
      Internal.Rtf.RtfParser parser = new Internal.Rtf.RtfParser(input, push, this.InputStreamBufferSize, this.testBoundaryConditions, push ? (IProgressMonitor) null : progressMonitor, (IReportBytes) null);
      if (this.textExtractionMode == TextExtractionMode.ExtractText)
        return (IProducerConsumer) new Internal.Rtf.RtfTextExtractionConverter(parser, output, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
      Injection injection = (Injection) null;
      if (this.injectHead != null || this.injectTail != null)
        injection = this.injectionFormat == HeaderFooterFormat.Html ? (Injection) new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, (Stream) null, progressMonitor) : (Injection) new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, (Stream) null, progressMonitor);
      Internal.Text.TextOutput output1 = new Internal.Text.TextOutput(output, this.wrapFlowed, this.wrapFlowed, 72, 78, (ImageRenderingCallbackInternal) null, this.fallbacks, this.htmlEscape, this.testPreserveTrailingSpaces, this.testFormatTraceStream);
      return (IProducerConsumer) new Internal.Rtf.RtfToTextConverter(parser, output1, injection, this.testTreatNbspAsBreakable, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
    }

    internal override void SetResult(ConfigParameter parameterId, object val)
    {
      if (parameterId == ConfigParameter.OutputEncoding)
        this.outputEncoding = (Encoding) val;
      base.SetResult(parameterId, val);
    }

    private void AssertNotLockedAndNotTextExtraction()
    {
      this.AssertNotLocked();
      if (this.textExtractionMode == TextExtractionMode.ExtractText)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.PropertyNotValidForTextExtractionMode);
    }
  }
}
