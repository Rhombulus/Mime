// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlToEnriched
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class HtmlToEnriched : TextConverter
  {
    private bool detectEncodingFromByteOrderMark = true;
    private bool detectEncodingFromMetaTag = true;
    private bool outputEncodingSameAsInput = true;
    private bool fallbacks = true;
    private int testMaxTokenRuns = 512;
    private bool testTraceShowTokenNum = true;
    private bool testNormalizerTraceShowTokenNum = true;
    private int testMaxHtmlTagSize = 4096;
    private int testMaxHtmlTagAttributes = 64;
    private int testMaxHtmlRestartOffset = 4096;
    private int testMaxHtmlNormalizerNesting = 4096;
    private Encoding inputEncoding;
    private Encoding outputEncoding;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
    private Stream testNormalizerTraceStream;
    private int testNormalizerTraceStopOnTokenNum;
    private Stream testFormatTraceStream;
    private Stream testFormatOutputTraceStream;
    private Stream testFormatConverterTraceStream;

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

    public bool DetectEncodingFromMetaTag
    {
      get
      {
        return this.detectEncodingFromMetaTag;
      }
      set
      {
        this.AssertNotLocked();
        this.detectEncodingFromMetaTag = value;
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

    internal HtmlToEnriched SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal HtmlToEnriched SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal HtmlToEnriched SetDetectEncodingFromMetaTag(bool value)
    {
      this.DetectEncodingFromMetaTag = value;
      return this;
    }

    internal HtmlToEnriched SetOutputEncoding(Encoding value)
    {
      this.OutputEncoding = value;
      return this;
    }

    internal HtmlToEnriched SetUseFallbacks(bool value)
    {
      this.fallbacks = value;
      return this;
    }

    internal HtmlToEnriched SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal HtmlToEnriched SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal HtmlToEnriched SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal HtmlToEnriched SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal HtmlToEnriched SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      if (value)
      {
        this.testMaxHtmlTagSize = 123;
        this.testMaxHtmlTagAttributes = 5;
        this.testMaxHtmlNormalizerNesting = 10;
      }
      return this;
    }

    internal HtmlToEnriched SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal HtmlToEnriched SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal HtmlToEnriched SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToEnriched SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToEnriched SetTestNormalizerTraceStream(Stream value)
    {
      this.testNormalizerTraceStream = value;
      return this;
    }

    internal HtmlToEnriched SetTestNormalizerTraceShowTokenNum(bool value)
    {
      this.testNormalizerTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToEnriched SetTestNormalizerTraceStopOnTokenNum(int value)
    {
      this.testNormalizerTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToEnriched SetTestMaxHtmlTagSize(int value)
    {
      this.testMaxHtmlTagSize = value;
      return this;
    }

    internal HtmlToEnriched SetTestMaxHtmlTagAttributes(int value)
    {
      this.testMaxHtmlTagAttributes = value;
      return this;
    }

    internal HtmlToEnriched SetTestMaxHtmlRestartOffset(int value)
    {
      this.testMaxHtmlRestartOffset = value;
      return this;
    }

    internal HtmlToEnriched SetTestMaxHtmlNormalizerNesting(int value)
    {
      this.testMaxHtmlNormalizerNesting = value;
      return this;
    }

    internal HtmlToEnriched SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
      return this;
    }

    internal HtmlToEnriched SetTestFormatOutputTraceStream(Stream value)
    {
      this.testFormatOutputTraceStream = value;
      return this;
    }

    internal HtmlToEnriched SetTestFormatConverterTraceStream(Stream value)
    {
      this.testFormatConverterTraceStream = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterEncodingOutput(output, true, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterUnicodeOutput((object) output, true, true), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, this.testMaxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) null), (ConverterOutput) new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
    {
      this.inputEncoding = Encoding.Unicode;
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, this.testMaxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) null), (ConverterOutput) new ConverterUnicodeOutput((object) output, true, false), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterStream), (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, this.testMaxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) converterStream), (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterReader), (ConverterOutput) new ConverterUnicodeOutput((object) converterReader, false, true), (IProgressMonitor) converterReader);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterReader converterReader)
    {
      this.inputEncoding = Encoding.Unicode;
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, this.testMaxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) converterReader), (ConverterOutput) new ConverterUnicodeOutput((object) converterReader, false, false), (IProgressMonitor) converterReader);
    }

    private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
    {
      this.locked = true;
      HtmlInjection injection = (HtmlInjection) null;
      Internal.Html.HtmlParser parser = new Internal.Html.HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
      if (this.injectHead != null || this.injectTail != null)
        injection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, (Stream) null, progressMonitor);
      return (IProducerConsumer) new Internal.Html.HtmlFormatConverter(new Internal.Html.HtmlNormalizingParser(parser, injection, false, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum), (Internal.Format.FormatOutput) new Internal.Enriched.EnrichedFormatOutput(output, (Injection) null, this.fallbacks, this.testFormatTraceStream, this.testFormatOutputTraceStream), false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream, progressMonitor);
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
