// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlToText
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class HtmlToText : TextConverter
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
    private bool outputAnchorLinks = true;
    private bool outputImageLinks = true;
    private TextExtractionMode mode;
    private Encoding inputEncoding;
    private bool normalizeInputHtml;
    private Encoding outputEncoding;
    private bool wrapFlowed;
    private bool wrapDelSp;
    private bool htmlEscape;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private ImageRenderingCallbackInternal imageRenderingCallback;
    private bool testPreserveTrailingSpaces;
    private bool testTreatNbspAsBreakable;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
    private Stream testNormalizerTraceStream;
    private int testNormalizerTraceStopOnTokenNum;
    private Stream testFormatTraceStream;

    public TextExtractionMode TextExtractionMode => this.mode;

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

    public bool NormalizeHtml
    {
      get
      {
        return this.normalizeInputHtml;
      }
      set
      {
        this.AssertNotLockedAndNotTextExtraction();
        this.normalizeInputHtml = value;
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

    public bool ShouldUseNarrowGapForPTagHtmlToTextConversion { get; set; }

    public bool OutputAnchorLinks
    {
      get
      {
        return this.outputAnchorLinks;
      }
      set
      {
        this.outputAnchorLinks = value;
      }
    }

    public bool OutputImageLinks
    {
      get
      {
        return this.outputImageLinks;
      }
      set
      {
        this.outputImageLinks = value;
      }
    }

    public HtmlToText()
    {
      this.mode = TextExtractionMode.NormalConversion;
    }

    public HtmlToText(TextExtractionMode mode)
    {
      this.mode = mode;
    }

    internal HtmlToText SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal HtmlToText SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal HtmlToText SetDetectEncodingFromMetaTag(bool value)
    {
      this.DetectEncodingFromMetaTag = value;
      return this;
    }

    internal HtmlToText SetOutputEncoding(Encoding value)
    {
      this.OutputEncoding = value;
      return this;
    }

    internal HtmlToText SetNormalizeHtml(bool value)
    {
      this.NormalizeHtml = value;
      return this;
    }

    internal HtmlToText SetWrap(bool value)
    {
      this.Wrap = value;
      return this;
    }

    internal HtmlToText SetWrapDeleteSpace(bool value)
    {
      this.WrapDeleteSpace = value;
      return this;
    }

    internal HtmlToText SetUseFallbacks(bool value)
    {
      this.fallbacks = value;
      return this;
    }

    internal HtmlToText SetHtmlEscapeOutput(bool value)
    {
      this.HtmlEscapeOutput = value;
      return this;
    }

    internal HtmlToText SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal HtmlToText SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal HtmlToText SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal HtmlToText SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal HtmlToText SetImageRenderingCallback(ImageRenderingCallbackInternal value)
    {
      this.imageRenderingCallback = value;
      return this;
    }

    internal HtmlToText SetTestBoundaryConditions(bool value)
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

    internal HtmlToText SetTestPreserveTrailingSpaces(bool value)
    {
      this.testPreserveTrailingSpaces = value;
      return this;
    }

    internal HtmlToText SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal HtmlToText SetTestTreatNbspAsBreakable(bool value)
    {
      this.testTreatNbspAsBreakable = value;
      return this;
    }

    internal HtmlToText SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal HtmlToText SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToText SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToText SetTestNormalizerTraceStream(Stream value)
    {
      this.testNormalizerTraceStream = value;
      return this;
    }

    internal HtmlToText SetTestNormalizerTraceShowTokenNum(bool value)
    {
      this.testNormalizerTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToText SetTestNormalizerTraceStopOnTokenNum(int value)
    {
      this.testNormalizerTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToText SetTestMaxHtmlTagSize(int value)
    {
      this.testMaxHtmlTagSize = value;
      return this;
    }

    internal HtmlToText SetTestMaxHtmlTagAttributes(int value)
    {
      this.testMaxHtmlTagAttributes = value;
      return this;
    }

    internal HtmlToText SetTestMaxHtmlRestartOffset(int value)
    {
      this.testMaxHtmlRestartOffset = value;
      return this;
    }

    internal HtmlToText SetTestMaxHtmlNormalizerNesting(int value)
    {
      this.testMaxHtmlNormalizerNesting = value;
      return this;
    }

    internal HtmlToText SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
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
      Internal.Html.HtmlParser parser1 = new Internal.Html.HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
      if (this.mode == TextExtractionMode.ExtractText)
        return (IProducerConsumer) new Internal.Html.HtmlTextExtractionConverter((Internal.Html.IHtmlParser) parser1, output, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
      Injection injection = (Injection) null;
      if (this.injectHead != null || this.injectTail != null)
        injection = this.injectionFormat == HeaderFooterFormat.Html ? (Injection) new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, (Stream) null, progressMonitor) : (Injection) new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, (Stream) null, progressMonitor);
      Internal.Html.IHtmlParser parser2 = (Internal.Html.IHtmlParser) parser1;
      if (this.normalizeInputHtml)
      {
        if (this.injectionFormat == HeaderFooterFormat.Html)
        {
          parser2 = (Internal.Html.IHtmlParser) new Internal.Html.HtmlNormalizingParser(parser1, (HtmlInjection) injection, false, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
          injection = (Injection) null;
        }
        else
          parser2 = (Internal.Html.IHtmlParser) new Internal.Html.HtmlNormalizingParser(parser1, (HtmlInjection) null, false, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
      }
      Internal.Text.TextOutput output1 = new Internal.Text.TextOutput(output, this.wrapFlowed, this.wrapFlowed, 72, 78, this.imageRenderingCallback, this.fallbacks, this.htmlEscape, this.testPreserveTrailingSpaces, this.testFormatTraceStream);
      return (IProducerConsumer) new Internal.Html.HtmlToTextConverter(parser2, output1, injection, false, false, this.testTreatNbspAsBreakable, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.ShouldUseNarrowGapForPTagHtmlToTextConversion, this.OutputAnchorLinks, this.OutputImageLinks);
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

    private void AssertNotLockedAndNotTextExtraction()
    {
      this.AssertNotLocked();
      if (this.mode == TextExtractionMode.ExtractText)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.PropertyNotValidForTextExtractionMode);
    }
  }
}
