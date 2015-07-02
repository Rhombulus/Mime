// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlToRtf
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class HtmlToRtf : TextConverter
  {
    private bool detectEncodingFromByteOrderMark = true;
    private bool detectEncodingFromMetaTag = true;
    private int testMaxTokenRuns = 512;
    private bool testTraceShowTokenNum = true;
    private bool testNormalizerTraceShowTokenNum = true;
    private int testMaxHtmlTagSize = 4096;
    private int testMaxHtmlTagAttributes = 64;
    private int testMaxHtmlRestartOffset = 4096;
    private int testMaxHtmlNormalizerNesting = 4096;
    private Encoding inputEncoding;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private bool encapsulateMarkup;
    private ImageRenderingCallbackInternal imageRenderingCallback;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
    private Stream testFormatTraceStream;
    private Stream testFormatOutputTraceStream;
    private Stream testFormatConverterTraceStream;
    private Stream testNormalizerTraceStream;
    private int testNormalizerTraceStopOnTokenNum;

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

    public bool EncapsulateHtmlMarkup
    {
      get
      {
        return this.encapsulateMarkup;
      }
      set
      {
        this.AssertNotLocked();
        this.encapsulateMarkup = value;
      }
    }

    public HtmlToRtf()
    {
      this.encapsulateMarkup = Common.RegistryConfigManager.HtmlEncapsulationOverride;
    }

    internal HtmlToRtf SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal HtmlToRtf SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal HtmlToRtf SetDetectEncodingFromMetaTag(bool value)
    {
      this.DetectEncodingFromMetaTag = value;
      return this;
    }

    internal HtmlToRtf SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal HtmlToRtf SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal HtmlToRtf SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal HtmlToRtf SetImageRenderingCallback(ImageRenderingCallbackInternal value)
    {
      this.imageRenderingCallback = value;
      return this;
    }

    internal HtmlToRtf SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal HtmlToRtf SetTestBoundaryConditions(bool value)
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

    internal HtmlToRtf SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal HtmlToRtf SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal HtmlToRtf SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToRtf SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToRtf SetTestNormalizerTraceStream(Stream value)
    {
      this.testNormalizerTraceStream = value;
      return this;
    }

    internal HtmlToRtf SetTestNormalizerTraceShowTokenNum(bool value)
    {
      this.testNormalizerTraceShowTokenNum = value;
      return this;
    }

    internal HtmlToRtf SetTestNormalizerTraceStopOnTokenNum(int value)
    {
      this.testNormalizerTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlToRtf SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
      return this;
    }

    internal HtmlToRtf SetTestFormatOutputTraceStream(Stream value)
    {
      this.testFormatOutputTraceStream = value;
      return this;
    }

    internal HtmlToRtf SetTestFormatConverterTraceStream(Stream value)
    {
      this.testFormatConverterTraceStream = value;
      return this;
    }

    internal HtmlToRtf SetTestMaxHtmlTagSize(int value)
    {
      this.testMaxHtmlTagSize = value;
      return this;
    }

    internal HtmlToRtf SetTestMaxHtmlTagAttributes(int value)
    {
      this.testMaxHtmlTagAttributes = value;
      return this;
    }

    internal HtmlToRtf SetTestMaxHtmlRestartOffset(int value)
    {
      this.testMaxHtmlRestartOffset = value;
      return this;
    }

    internal HtmlToRtf SetTestMaxHtmlNormalizerNesting(int value)
    {
      this.testMaxHtmlNormalizerNesting = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput(output, true, true, this.testBoundaryConditions, (IResultsFeedback) this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.inputEncoding), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextWriterUnsupported);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) converterWriter, true, this.testMaxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) null), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput(output, true, false, this.testBoundaryConditions, (IResultsFeedback) this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, (Encoding) null), (IProgressMonitor) converterWriter);
    }

    internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.TextWriterUnsupported);
    }

    internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
    {
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterStream), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput((Stream) converterStream, false, true, this.testBoundaryConditions, (IResultsFeedback) this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.inputEncoding), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePullChain(System.IO.TextReader input, ConverterStream converterStream)
    {
      this.inputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterUnicodeInput((object) input, false, this.testMaxHtmlTagSize, this.testBoundaryConditions, (IProgressMonitor) converterStream), (Internal.Format.FormatOutput) new Internal.Rtf.RtfFormatOutput((Stream) converterStream, false, false, this.testBoundaryConditions, (IResultsFeedback) this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, (Encoding) null), (IProgressMonitor) converterStream);
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
      HtmlInjection injection = (HtmlInjection) null;
      if (this.injectHead != null || this.injectTail != null)
        injection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, (Stream) null, progressMonitor);
      Internal.Html.HtmlNormalizingParser parser = new Internal.Html.HtmlNormalizingParser(new Internal.Html.HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions), injection, false, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
      return !this.encapsulateMarkup ? (IProducerConsumer) new Internal.Html.HtmlFormatConverter(parser, output, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream, progressMonitor) : (IProducerConsumer) new Internal.Html.HtmlFormatConverterWithEncapsulation(parser, output, this.encapsulateMarkup, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream, progressMonitor);
    }

    internal override void SetResult(ConfigParameter parameterId, object val)
    {
      if (parameterId == ConfigParameter.InputEncoding)
        this.inputEncoding = (Encoding) val;
      base.SetResult(parameterId, val);
    }
  }
}
