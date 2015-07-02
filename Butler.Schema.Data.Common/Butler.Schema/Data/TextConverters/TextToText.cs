// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TextToText
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class TextToText : TextConverter
  {
    private bool detectEncodingFromByteOrderMark = true;
    private bool outputEncodingSameAsInput = true;
    private bool fallbacks = true;
    private int testMaxTokenRuns = 512;
    private bool testTraceShowTokenNum = true;
    private TextToTextConversionMode mode;
    private Encoding inputEncoding;
    private bool unwrapFlowed;
    private bool unwrapDelSp;
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

    public bool Unwrap
    {
      get
      {
        return this.unwrapFlowed;
      }
      set
      {
        this.AssertNotLockedAndNotSimpleCodepageConversion();
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
        this.AssertNotLockedAndNotSimpleCodepageConversion();
        this.unwrapDelSp = value;
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
        this.AssertNotLockedAndNotSimpleCodepageConversion();
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
        this.AssertNotLockedAndNotSimpleCodepageConversion();
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
        this.AssertNotLockedAndNotSimpleCodepageConversion();
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
        this.AssertNotLockedAndNotSimpleCodepageConversion();
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
        this.AssertNotLockedAndNotSimpleCodepageConversion();
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
        this.AssertNotLockedAndNotSimpleCodepageConversion();
        this.injectTail = value;
      }
    }

    public TextToText()
    {
      this.mode = TextToTextConversionMode.Normal;
    }

    public TextToText(TextToTextConversionMode mode)
    {
      this.mode = mode;
    }

    internal TextToText SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal TextToText SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal TextToText SetOutputEncoding(Encoding value)
    {
      this.OutputEncoding = value;
      return this;
    }

    internal TextToText SetUnwrap(bool value)
    {
      this.Unwrap = value;
      return this;
    }

    internal TextToText SetUnwrapDeleteSpace(bool value)
    {
      this.UnwrapDeleteSpace = value;
      return this;
    }

    internal TextToText SetWrap(bool value)
    {
      this.Wrap = value;
      return this;
    }

    internal TextToText SetWrapDeleteSpace(bool value)
    {
      this.WrapDeleteSpace = value;
      return this;
    }

    internal TextToText SetUseFallbacks(bool value)
    {
      this.fallbacks = value;
      return this;
    }

    internal TextToText SetHtmlEscapeOutput(bool value)
    {
      this.HtmlEscapeOutput = value;
      return this;
    }

    internal TextToText SetHeaderFooterFormat(HeaderFooterFormat value)
    {
      this.HeaderFooterFormat = value;
      return this;
    }

    internal TextToText SetHeader(string value)
    {
      this.Header = value;
      return this;
    }

    internal TextToText SetFooter(string value)
    {
      this.Footer = value;
      return this;
    }

    internal TextToText SetInputStreamBufferSize(int value)
    {
      this.InputStreamBufferSize = value;
      return this;
    }

    internal TextToText SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      int num = value ? 1 : 0;
      return this;
    }

    internal TextToText SetTestPreserveTrailingSpaces(bool value)
    {
      this.testPreserveTrailingSpaces = value;
      return this;
    }

    internal TextToText SetTestTreatNbspAsBreakable(bool value)
    {
      this.testTreatNbspAsBreakable = value;
      return this;
    }

    internal TextToText SetTestMaxTokenRuns(int value)
    {
      this.testMaxTokenRuns = value;
      return this;
    }

    internal TextToText SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal TextToText SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal TextToText SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal TextToText SetTestFormatTraceStream(Stream value)
    {
      this.testFormatTraceStream = value;
      return this;
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
    }

    internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
    {
      if (this.inputEncoding == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
      this.outputEncoding = Encoding.Unicode;
      return this.CreateChain((ConverterInput) new ConverterDecodingInput((Stream) converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null), (ConverterOutput) new ConverterUnicodeOutput((object) output, true, false), (IProgressMonitor) converterStream);
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
      return this.CreateChain((ConverterInput) new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, this.InputStreamBufferSize, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) converterStream), (ConverterOutput) new ConverterEncodingOutput((Stream) converterStream, false, false, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, (IResultsFeedback) this), (IProgressMonitor) converterStream);
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
      if (this.mode == TextToTextConversionMode.ConvertCodePageOnly)
        return (IProducerConsumer) new Internal.Text.TextCodePageConverter(input, output);
      Injection injection = (Injection) null;
      if (this.injectHead != null || this.injectTail != null)
        injection = this.injectionFormat == HeaderFooterFormat.Html ? (Injection) new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, (HtmlTagCallback) null, this.testBoundaryConditions, (Stream) null, progressMonitor) : (Injection) new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, (Stream) null, progressMonitor);
      return (IProducerConsumer) new Internal.Text.TextToTextConverter(new Internal.Text.TextParser(input, this.unwrapFlowed, this.unwrapDelSp, this.testMaxTokenRuns, this.testBoundaryConditions), new Internal.Text.TextOutput(output, this.wrapFlowed, this.wrapFlowed, 72, 78, (ImageRenderingCallbackInternal) null, this.fallbacks, this.htmlEscape, this.testPreserveTrailingSpaces, this.testFormatTraceStream), injection, false, this.testTreatNbspAsBreakable, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
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

    private void AssertNotLockedAndNotSimpleCodepageConversion()
    {
      this.AssertNotLocked();
      if (this.mode == TextToTextConversionMode.ConvertCodePageOnly)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.PropertyNotValidForCodepageConversionMode);
    }
  }
}
