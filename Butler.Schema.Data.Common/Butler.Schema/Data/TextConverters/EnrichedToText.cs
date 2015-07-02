// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.EnrichedToText
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class EnrichedToText : TextConverter
  {
    private bool detectEncodingFromByteOrderMark = true;
    private bool outputEncodingSameAsInput = true;
    private bool fallbacks = true;
    private int testMaxTokenRuns = 512;
    private bool testTraceShowTokenNum = true;
    private Encoding inputEncoding;
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
        this.AssertNotLocked();
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
        this.AssertNotLocked();
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
        this.AssertNotLocked();
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

    internal EnrichedToText SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal EnrichedToText SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal EnrichedToText SetOutputEncoding(Encoding value)
    {
      this.OutputEncoding = value;
      return this;
    }

    internal EnrichedToText SetWrap(bool value)
    {
      this.Wrap = value;
      return this;
    }

    internal EnrichedToText SetWrapDeleteSpace(bool value)
    {
      this.WrapDeleteSpace = value;
      return this;
    }

    internal EnrichedToText SetUseFallbacks(bool value)
    {
      this.fallbacks = value;
      return this;
    }

    internal EnrichedToText SetHtmlEscapeOutput(bool value)
    {
      this.HtmlEscapeOutput = value;
      return this;
    }

    internal EnrichedToText SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal EnrichedToText SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal EnrichedToText SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal EnrichedToText SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal EnrichedToText SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      int num = value ? 1 : 0;
      return this;
    }

    internal EnrichedToText SetTestPreserveTrailingSpaces(bool value)
    {
      this.testPreserveTrailingSpaces = value;
      return this;
    }

    internal EnrichedToText SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal EnrichedToText SetTestTreatNbspAsBreakable(bool value)
    {
      this.testTreatNbspAsBreakable = value;
      return this;
    }

    internal EnrichedToText SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal EnrichedToText SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal EnrichedToText SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal EnrichedToText SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterEncodingOutput(output, true, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterUnicodeOutput((object) output, true, true), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, 4096, this.testBoundaryConditions, (IProgressMonitor) null), (ConverterOutput) new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
    {
      this.inputEncoding = Encoding.Unicode;
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, 4096, this.testBoundaryConditions, (IProgressMonitor) null), (ConverterOutput) new ConverterUnicodeOutput((object) output, true, false), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterStream), (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, 4096, this.testBoundaryConditions, (IProgressMonitor) converterStream), (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterReader), (ConverterOutput) new ConverterUnicodeOutput((object) converterReader, false, true), (IProgressMonitor) converterReader);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterReader converterReader)
    {
      this.inputEncoding = Encoding.Unicode;
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, 4096, this.testBoundaryConditions, (IProgressMonitor) converterReader), (ConverterOutput) new ConverterUnicodeOutput((object) converterReader, false, false), (IProgressMonitor) converterReader);
    }

    private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
    {
      this.locked = true;
      Injection injection = (Injection) null;
      Internal.Enriched.EnrichedParser parser = new Internal.Enriched.EnrichedParser(input, this.testMaxTokenRuns, this.testBoundaryConditions);
      if (this.injectHead != null || this.injectTail != null)
        injection = this.injectionFormat == HeaderFooterFormat.Html ? (Injection) new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, (Stream) null, progressMonitor) : (Injection) new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, (Stream) null, progressMonitor);
      Internal.Text.TextOutput output1 = new Internal.Text.TextOutput(output, this.wrapFlowed, this.wrapFlowed, 72, 78, (ImageRenderingCallbackInternal) null, this.fallbacks, this.htmlEscape, this.testPreserveTrailingSpaces, this.testFormatTraceStream);
      return (IProducerConsumer) new Internal.Enriched.EnrichedToTextConverter(parser, output1, injection, this.testTreatNbspAsBreakable, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
    }

    internal override void SetResult(ConfigParameter parameterId, object val)
    {
      switch (parameterId)
      {
        case ConfigParameter.InputEncoding:
          this.inputEncoding = (Encoding) val;
          break;
        case ConfigParameter.OutputEncoding:
          this.outputEncoding = (Encoding) val;
          break;
      }
      base.SetResult(parameterId, val);
    }
  }
}
