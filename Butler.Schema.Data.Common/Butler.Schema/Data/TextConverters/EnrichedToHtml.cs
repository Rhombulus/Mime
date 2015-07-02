// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.EnrichedToHtml
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class EnrichedToHtml : TextConverter
  {
    private bool detectEncodingFromByteOrderMark = true;
    private bool outputEncodingSameAsInput = true;
    private int testMaxTokenRuns = 512;
    private bool testTraceShowTokenNum = true;
    private Encoding inputEncoding;
    private Encoding outputEncoding;
    private bool filterHtml;
    private HtmlTagCallback htmlCallback;
    private HeaderFooterFormat injectionFormat;
    private string injectHead;
    private string injectTail;
    private bool outputFragment;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
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

    internal EnrichedToHtml SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal EnrichedToHtml SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal EnrichedToHtml SetOutputEncoding(Encoding value)
    {
      this.OutputEncoding = value;
      return this;
    }

    internal EnrichedToHtml SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal EnrichedToHtml SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal EnrichedToHtml SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal EnrichedToHtml SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal EnrichedToHtml SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      int num = value ? 1 : 0;
      return this;
    }

    internal EnrichedToHtml SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal EnrichedToHtml SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal EnrichedToHtml SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal EnrichedToHtml SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal EnrichedToHtml SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
      return this;
    }

    internal EnrichedToHtml SetTestFormatOutputTraceStream(Stream value)
    {
      this.testFormatOutputTraceStream = value;
      return this;
    }

    internal EnrichedToHtml SetTestFormatConverterTraceStream(Stream value)
    {
      this.testFormatConverterTraceStream = value;
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
      HtmlInjection injection = (HtmlInjection) null;
      if (this.injectHead != null || this.injectTail != null)
        injection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, this.filterHtml, this.htmlCallback, this.testBoundaryConditions, (Stream) null, progressMonitor);
      return (IProducerConsumer) new Internal.Enriched.EnrichedFormatConverter(new Internal.Enriched.EnrichedParser(input, this.testMaxTokenRuns, this.testBoundaryConditions), (Internal.Format.FormatOutput) new Internal.Html.HtmlFormatOutput(new HtmlWriter(output, this.filterHtml, false), injection, this.outputFragment, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.filterHtml, this.htmlCallback, true), (Injection) null, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream);
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
