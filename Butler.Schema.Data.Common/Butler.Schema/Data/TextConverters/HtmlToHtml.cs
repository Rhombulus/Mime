// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlToHtml
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class HtmlToHtml : TextConverter
  {
    private bool detectEncodingFromByteOrderMark = true;
    private bool detectEncodingFromMetaTag = true;
    private bool outputEncodingSameAsInput = true;
    private bool testTruncateForCallback = true;
    private int testMaxTokenRuns = 512;
    private bool testTraceShowTokenNum = true;
    private bool testNormalizerTraceShowTokenNum = true;
    private int maxHtmlTagSize = 32768;
    private int testMaxHtmlTagAttributes = 64;
    private int testMaxHtmlRestartOffset = 4096;
    private int testMaxHtmlNormalizerNesting = 4096;
    private int smallCssBlockThreshold = -1;
    private Encoding inputEncoding;
    private Encoding outputEncoding;
    private bool normalizeInputHtml;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private bool filterHtml;
    private HtmlTagCallback htmlCallback;
    private bool testConvertFragment;
    private bool outputFragment;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
    private Stream testNormalizerTraceStream;
    private int testNormalizerTraceStopOnTokenNum;
    private bool preserveDisplayNoneStyle;
    private bool testNoNewLines;

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
        this.AssertNotLocked();
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

    public bool OutputHtmlFragment
    {
      get
      {
        return this.outputFragment;
      }
      set
      {
        this.AssertNotLocked();
        this.outputFragment = value;
      }
    }

    public bool FilterHtml
    {
      get
      {
        return this.filterHtml;
      }
      set
      {
        this.AssertNotLocked();
        this.filterHtml = value;
      }
    }

    public HtmlTagCallback HtmlTagCallback
    {
      get
      {
        return this.htmlCallback;
      }
      set
      {
        this.AssertNotLocked();
        this.htmlCallback = value;
      }
    }

    public int MaxCallbackTagLength
    {
      get
      {
        return this.maxHtmlTagSize;
      }
      set
      {
        this.AssertNotLocked();
        this.maxHtmlTagSize = value;
      }
    }

    internal HtmlToHtml SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal HtmlToHtml SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal HtmlToHtml SetDetectEncodingFromMetaTag(bool value)
    {
      this.DetectEncodingFromMetaTag = value;
      return this;
    }

    internal HtmlToHtml SetOutputEncoding(Encoding value)
    {
      this.OutputEncoding = value;
      return this;
    }

    internal HtmlToHtml SetNormalizeHtml(bool value)
    {
      this.NormalizeHtml = value;
      return this;
    }

    internal HtmlToHtml SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal HtmlToHtml SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal HtmlToHtml SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal HtmlToHtml SetFilterHtml(bool value)
    {
      this.FilterHtml = value;
      return this;
    }

    internal HtmlToHtml SetHtmlTagCallback(HtmlTagCallback value)
    {
      this.HtmlTagCallback = value;
      return this;
    }

    internal HtmlToHtml SetTestTruncateForCallback(bool value)
    {
      this.testTruncateForCallback = value;
      return this;
    }

    internal HtmlToHtml SetMaxCallbackTagLength(int value)
    {
      this.maxHtmlTagSize = value;
      return this;
    }

    internal HtmlToHtml SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal HtmlToHtml SetOutputHtmlFragment(bool value)
    {
      this.OutputHtmlFragment = value;
      return this;
    }

    internal HtmlToHtml SetTestConvertHtmlFragment(bool value)
    {
      this.testConvertFragment = value;
      return this;
    }

    internal HtmlToHtml SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      if (value)
      {
        this.maxHtmlTagSize = 123;
        this.testMaxHtmlTagAttributes = 5;
        this.testMaxHtmlNormalizerNesting = 10;
      }
      return this;
    }

    internal HtmlToHtml SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal HtmlToHtml SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal HtmlToHtml SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToHtml SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToHtml SetTestNormalizerTraceStream(Stream value)
    {
      this.testNormalizerTraceStream = value;
      return this;
    }

    internal HtmlToHtml SetTestNormalizerTraceShowTokenNum(bool value)
    {
      this.testNormalizerTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToHtml SetTestNormalizerTraceStopOnTokenNum(int value)
    {
      this.testNormalizerTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToHtml SetTestMaxHtmlTagAttributes(int value)
    {
      this.testMaxHtmlTagAttributes = value;
      return this;
    }

    internal HtmlToHtml SetTestMaxHtmlRestartOffset(int value)
    {
      this.testMaxHtmlRestartOffset = value;
      return this;
    }

    internal HtmlToHtml SetTestMaxHtmlNormalizerNesting(int value)
    {
      this.testMaxHtmlNormalizerNesting = value;
      return this;
    }

    internal HtmlToHtml SetTestNoNewLines(bool value)
    {
      this.testNoNewLines = value;
      return this;
    }

    internal HtmlToHtml SetSmallCssBlockThreshold(int value)
    {
      this.smallCssBlockThreshold = value;
      return this;
    }

    internal HtmlToHtml SetPreserveDisplayNoneStyle(bool value)
    {
      this.preserveDisplayNoneStyle = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterEncodingOutput(output, true, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterUnicodeOutput((object) output, true, true), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, this.maxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) null), (ConverterOutput) new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
    {
      this.inputEncoding = Encoding.Unicode;
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, this.maxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) null), (ConverterOutput) new ConverterUnicodeOutput((object) output, true, false), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterStream), (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, this.maxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) converterStream), (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterReader), (ConverterOutput) new ConverterUnicodeOutput((object) converterReader, false, true), (IProgressMonitor) converterReader);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterReader converterReader)
    {
      this.inputEncoding = Encoding.Unicode;
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, this.maxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) converterReader), (ConverterOutput) new ConverterUnicodeOutput((object) converterReader, false, false), (IProgressMonitor) converterReader);
    }

    private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
    {
      this.locked = true;
      HtmlInjection injection = (HtmlInjection) null;
      HtmlInjection htmlInjection = (HtmlInjection) null;
      try
      {
        if (this.injectHead != null || this.injectTail != null)
        {
          htmlInjection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, this.filterHtml, this.htmlCallback, this.testBoundaryConditions, (Stream) null, progressMonitor);
          injection = htmlInjection;
          this.normalizeInputHtml = true;
        }
        if (this.filterHtml || this.outputFragment || this.htmlCallback != null)
          this.normalizeInputHtml = true;
        Internal.Html.IHtmlParser parser;
        if (this.normalizeInputHtml)
        {
          parser = (Internal.Html.IHtmlParser) new Internal.Html.HtmlNormalizingParser(new Internal.Html.HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions), injection, this.htmlCallback != null, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
          htmlInjection = (HtmlInjection) null;
        }
        else
          parser = (Internal.Html.IHtmlParser) new Internal.Html.HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
        HtmlWriter writer = new HtmlWriter(output, this.filterHtml, this.normalizeInputHtml && !this.testNoNewLines);
        return (IProducerConsumer) new Internal.Html.HtmlToHtmlConverter(parser, writer, this.testConvertFragment, this.outputFragment, this.filterHtml, this.htmlCallback, this.testTruncateForCallback, injection != null && injection.HaveTail, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.smallCssBlockThreshold, this.preserveDisplayNoneStyle, progressMonitor);
      }
      finally
      {
        IDisposable disposable = (IDisposable) htmlInjection;
        if (disposable != null)
          disposable.Dispose();
      }
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
