// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class HtmlReader : IRestartable, IResultsFeedback, IDisposable
  {
    private bool testTraceShowTokenNum = true;
    private bool testNormalizerTraceShowTokenNum = true;
    private const int InputBufferSize = 16384;
    private Encoding inputEncoding;
    private bool detectEncodingFromByteOrderMark;
    private bool normalizeInputHtml;
    private bool testBoundaryConditions;
    private Stream testTraceStream;
    private int testTraceStopOnTokenNum;
    private Stream testNormalizerTraceStream;
    private int testNormalizerTraceStopOnTokenNum;
    private bool locked;
    private object input;
    private Internal.Html.IHtmlParser parser;
    private Internal.Html.HtmlTokenId parserTokenId;
    private Internal.Html.HtmlToken parserToken;
    private Internal.Html.HtmlToken nextParserToken;
    private int depth;
    private StringBuildSink stringBuildSink;
    private int currentAttribute;
    private bool literalTags;
    private HtmlReader.State state;
    private HtmlTokenKind tokenKind;

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

    public HtmlTokenKind TokenKind
    {
      get
      {
        this.AssertInToken();
        return this.tokenKind;
      }
    }

    public int Depth
    {
      get
      {
        this.AssertNotDisposed();
        return this.depth;
      }
    }

    public int CurrentOffset
    {
      get
      {
        this.AssertNotDisposed();
        return this.parser.CurrentOffset;
      }
    }

    public int OverlappedDepth
    {
      get
      {
        if (this.state != HtmlReader.State.OverlappedClose && this.state != HtmlReader.State.OverlappedReopen)
        {
          this.AssertInToken();
          throw new InvalidOperationException("Reader must be positioned on OverlappedClose or OverlappedReopen token");
        }
        return this.parserToken.Argument;
      }
    }

    public HtmlTagId TagId
    {
      get
      {
        this.AssertInTag();
        return Internal.Html.HtmlNameData.Names[(int) this.parserToken.NameIndex].PublicTagId;
      }
    }

    public bool TagInjectedByNormalizer
    {
      get
      {
        this.AssertInTag();
        if (this.state != HtmlReader.State.BeginTag)
          throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
        return this.parserToken.Argument == 1;
      }
    }

    public bool TagNameIsLong
    {
      get
      {
        this.AssertInTag();
        if (this.state != HtmlReader.State.BeginTag)
          throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
        if (this.parserToken.NameIndex == Internal.Html.HtmlNameIndex.Unknown && this.parserToken.IsTagNameBegin)
          return !this.parserToken.IsTagNameEnd;
        return false;
      }
    }

    public HtmlAttributeReader AttributeReader
    {
      get
      {
        this.AssertInTag();
        if (this.state == HtmlReader.State.ReadTag)
          throw new InvalidOperationException("Cannot read attributes after reading tag as a markup text");
        return new HtmlAttributeReader(this);
      }
    }

    public HtmlReader(Stream input, Encoding inputEncoding)
    {
      if (input == null)
        throw new ArgumentNullException("input");
      if (!input.CanRead)
        throw new ArgumentException("input stream must support reading");
      this.input = (object) input;
      this.inputEncoding = inputEncoding;
      this.state = HtmlReader.State.Begin;
    }

    public HtmlReader(System.IO.TextReader input)
    {
      if (input == null)
        throw new ArgumentNullException("input");
      this.input = (object) input;
      this.inputEncoding = Encoding.Unicode;
      this.state = HtmlReader.State.Begin;
    }

    public bool ReadNextToken()
    {
      this.AssertNotDisposed();
      if (this.state == HtmlReader.State.EndOfFile)
        return false;
      if (!this.locked)
        this.InitializeAndLock();
      if (this.state == HtmlReader.State.Text)
      {
        do
        {
          this.ParseToken();
        }
        while (this.parserTokenId == Internal.Html.HtmlTokenId.Text || this.literalTags && this.parserTokenId == Internal.Html.HtmlTokenId.Tag && this.parserToken.TagIndex < Internal.Html.HtmlTagIndex.Unknown);
      }
      else if (this.state >= HtmlReader.State.SpecialTag)
      {
        while (!this.parserToken.IsTagEnd)
          this.ParseToken();
        if (this.parserToken.TagIndex > Internal.Html.HtmlTagIndex.Unknown && !this.parserToken.IsEndTag && (Internal.Html.HtmlDtd.tags[(int) this.parserToken.TagIndex].Scope != Internal.Html.HtmlDtd.TagScope.EMPTY && !this.parserToken.IsEmptyScope))
          ++this.depth;
        if (!this.parserToken.IsEndTag && (Internal.Html.HtmlDtd.tags[(int) this.parserToken.TagIndex].Literal & Internal.Html.HtmlDtd.Literal.Tags) != Internal.Html.HtmlDtd.Literal.None)
          this.literalTags = true;
        this.ParseToken();
      }
      else
      {
        if (this.state == HtmlReader.State.OverlappedClose)
          this.depth -= this.parserToken.Argument;
        this.ParseToken();
      }
      while (true)
      {
        switch (this.parserTokenId - (byte) 2)
        {
          case Internal.Html.HtmlTokenId.None:
            goto label_18;
          case Internal.Html.HtmlTokenId.EndOfFile:
            this.ParseToken();
            continue;
          case Internal.Html.HtmlTokenId.Text:
            if (this.parserToken.TagIndex >= Internal.Html.HtmlTagIndex.Unknown)
            {
              if (this.parserToken.TagIndex == Internal.Html.HtmlTagIndex.TC)
              {
                this.ParseToken();
                continue;
              }
              goto label_26;
            }
            else
              goto label_20;
          case Internal.Html.HtmlTokenId.EncodingChange:
            continue;
          case Internal.Html.HtmlTokenId.Tag:
            goto label_34;
          case Internal.Html.HtmlTokenId.Restart:
            goto label_35;
          default:
            goto label_37;
        }
      }
label_18:
      this.state = HtmlReader.State.Text;
      this.tokenKind = HtmlTokenKind.Text;
      this.parserToken.Text.Rewind();
      goto label_38;
label_20:
      if (this.literalTags)
      {
        this.state = HtmlReader.State.Text;
        this.tokenKind = HtmlTokenKind.Text;
      }
      else
      {
        this.state = HtmlReader.State.SpecialTag;
        this.tokenKind = HtmlTokenKind.SpecialTag;
      }
      this.parserToken.Text.Rewind();
      goto label_38;
label_26:
      if (this.parserToken.IsTagNameEmpty && this.parserToken.TagIndex == Internal.Html.HtmlTagIndex.Unknown)
      {
        this.state = HtmlReader.State.SpecialTag;
        this.tokenKind = HtmlTokenKind.SpecialTag;
        this.parserToken.Text.Rewind();
        goto label_38;
      }
      else
      {
        this.state = HtmlReader.State.BeginTag;
        if (this.parserToken.IsEndTag)
        {
          this.tokenKind = HtmlTokenKind.EndTag;
          if ((Internal.Html.HtmlDtd.tags[(int) this.parserToken.TagIndex].Literal & Internal.Html.HtmlDtd.Literal.Tags) != Internal.Html.HtmlDtd.Literal.None)
            this.literalTags = false;
        }
        else
          this.tokenKind = this.parserToken.TagIndex <= Internal.Html.HtmlTagIndex.Unknown || Internal.Html.HtmlDtd.tags[(int) this.parserToken.TagIndex].Scope != Internal.Html.HtmlDtd.TagScope.EMPTY ? HtmlTokenKind.StartTag : HtmlTokenKind.EmptyElementTag;
        this.parserToken.Text.Rewind();
        if (this.parserToken.IsEndTag && this.parserToken.TagIndex != Internal.Html.HtmlTagIndex.Unknown)
        {
          --this.depth;
          goto label_38;
        }
        else
          goto label_38;
      }
label_34:
      this.state = HtmlReader.State.OverlappedClose;
      this.tokenKind = HtmlTokenKind.OverlappedClose;
      goto label_38;
label_35:
      this.depth += this.parserToken.Argument;
      this.state = HtmlReader.State.OverlappedReopen;
      this.tokenKind = HtmlTokenKind.OverlappedReopen;
      goto label_38;
label_37:
      this.state = HtmlReader.State.EndOfFile;
      return false;
label_38:
      return true;
    }

    public string ReadTagName()
    {
      if (this.state != HtmlReader.State.BeginTag)
      {
        this.AssertInTag();
        throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
      }
      string str;
      if (this.parserToken.NameIndex != Internal.Html.HtmlNameIndex.Unknown)
      {
        str = Internal.Html.HtmlNameData.Names[(int) this.parserToken.NameIndex].Name;
      }
      else
      {
        if (this.parserToken.IsTagNameEnd)
          return this.parserToken.Name.GetString(int.MaxValue);
        StringBuildSink stringBuildSink = this.GetStringBuildSink();
        this.parserToken.Name.WriteTo((ITextSink) stringBuildSink);
        do
        {
          this.ParseToken();
          this.parserToken.Name.WriteTo((ITextSink) stringBuildSink);
        }
        while (!this.parserToken.IsTagNameEnd);
        str = stringBuildSink.ToString();
      }
      this.state = HtmlReader.State.EndTagName;
      return str;
    }

    public int ReadTagName(char[] buffer, int offset, int count)
    {
      this.AssertInTag();
      if (this.state > HtmlReader.State.EndTagName)
        throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TextConvertersStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      int num1 = 0;
      if (this.state != HtmlReader.State.EndTagName)
      {
        if (this.state == HtmlReader.State.BeginTag)
        {
          this.state = HtmlReader.State.ReadTagName;
          this.parserToken.Name.Rewind();
        }
        while (count != 0)
        {
          int num2 = this.parserToken.Name.Read(buffer, offset, count);
          if (num2 == 0)
          {
            if (this.parserToken.IsTagNameEnd)
            {
              this.state = HtmlReader.State.EndTagName;
              break;
            }
            this.ParseToken();
            this.parserToken.Name.Rewind();
          }
          else
          {
            offset += num2;
            count -= num2;
            num1 += num2;
          }
        }
      }
      return num1;
    }

    internal void WriteTagNameTo(ITextSink sink)
    {
      if (this.state != HtmlReader.State.BeginTag)
      {
        this.AssertInTag();
        throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
      }
      while (true)
      {
        this.parserToken.Name.WriteTo(sink);
        if (!this.parserToken.IsTagNameEnd)
          this.ParseToken();
        else
          break;
      }
      this.state = HtmlReader.State.EndTagName;
    }

    public int ReadText(char[] buffer, int offset, int count)
    {
      if (this.state == HtmlReader.State.EndText)
        return 0;
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TextConvertersStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      if (this.state != HtmlReader.State.Text)
      {
        this.AssertInToken();
        throw new InvalidOperationException("Reader must be positioned on a Text token");
      }
      int num1 = 0;
      while (count != 0)
      {
        int num2 = this.parserToken.Text.Read(buffer, offset, count);
        if (num2 == 0)
        {
          Internal.Html.HtmlTokenId htmlTokenId = this.PreviewNextToken();
          if (htmlTokenId != Internal.Html.HtmlTokenId.Text && (!this.literalTags || htmlTokenId != Internal.Html.HtmlTokenId.Tag || this.nextParserToken.TagIndex >= Internal.Html.HtmlTagIndex.Unknown))
          {
            this.state = HtmlReader.State.EndText;
            break;
          }
          this.ParseToken();
          this.parserToken.Text.Rewind();
        }
        else
        {
          offset += num2;
          count -= num2;
          num1 += num2;
        }
      }
      return num1;
    }

    internal void WriteTextTo(ITextSink sink)
    {
      if (this.state != HtmlReader.State.Text)
      {
        this.AssertInToken();
        throw new InvalidOperationException("Reader must be positioned on a Text token");
      }
      while (true)
      {
        this.parserToken.Text.WriteTo(sink);
        Internal.Html.HtmlTokenId htmlTokenId = this.PreviewNextToken();
        if (htmlTokenId == Internal.Html.HtmlTokenId.Text || this.literalTags && htmlTokenId == Internal.Html.HtmlTokenId.Tag && this.nextParserToken.TagIndex < Internal.Html.HtmlTagIndex.Unknown)
          this.ParseToken();
        else
          break;
      }
      this.state = HtmlReader.State.EndText;
    }

    public int ReadMarkupText(char[] buffer, int offset, int count)
    {
      if (this.state == HtmlReader.State.EndTag || this.state == HtmlReader.State.EndSpecialTag || this.state == HtmlReader.State.EndText)
        return 0;
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TextConvertersStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      if (this.state == HtmlReader.State.BeginTag)
        this.state = HtmlReader.State.ReadTag;
      else if (this.state != HtmlReader.State.SpecialTag && this.state != HtmlReader.State.ReadTag && this.state != HtmlReader.State.Text)
      {
        this.AssertInToken();
        if (this.state > HtmlReader.State.BeginTag)
          throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
        throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
      }
      int num1 = 0;
      while (count != 0)
      {
        int num2 = this.parserToken.Text.ReadOriginal(buffer, offset, count);
        if (num2 == 0)
        {
          if (this.state == HtmlReader.State.SpecialTag)
          {
            if (this.parserToken.IsTagEnd)
            {
              this.state = HtmlReader.State.EndSpecialTag;
              break;
            }
          }
          else if (this.state == HtmlReader.State.ReadTag)
          {
            if (this.parserToken.IsTagEnd)
            {
              this.state = HtmlReader.State.EndTag;
              break;
            }
          }
          else
          {
            Internal.Html.HtmlTokenId htmlTokenId = this.PreviewNextToken();
            if (htmlTokenId != Internal.Html.HtmlTokenId.Text && (!this.literalTags || htmlTokenId != Internal.Html.HtmlTokenId.Tag || this.nextParserToken.TagIndex >= Internal.Html.HtmlTagIndex.Unknown))
            {
              this.state = HtmlReader.State.EndText;
              break;
            }
          }
          this.ParseToken();
          this.parserToken.Text.Rewind();
        }
        else
        {
          offset += num2;
          count -= num2;
          num1 += num2;
        }
      }
      return num1;
    }

    internal void WriteMarkupTextTo(ITextSink sink)
    {
      if (this.state == HtmlReader.State.BeginTag)
        this.state = HtmlReader.State.ReadTag;
      else if (this.state != HtmlReader.State.SpecialTag && this.state != HtmlReader.State.ReadTag && this.state != HtmlReader.State.Text)
      {
        this.AssertInToken();
        if (this.state > HtmlReader.State.BeginTag)
          throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
        throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
      }
      while (true)
      {
        this.parserToken.Text.WriteOriginalTo(sink);
        if (this.state == HtmlReader.State.SpecialTag)
        {
          if (this.parserToken.IsTagEnd)
            break;
        }
        else if (this.state == HtmlReader.State.ReadTag)
        {
          if (this.parserToken.IsTagEnd)
            goto label_11;
        }
        else
        {
          Internal.Html.HtmlTokenId htmlTokenId = this.PreviewNextToken();
          if (htmlTokenId != Internal.Html.HtmlTokenId.Text && (!this.literalTags || htmlTokenId != Internal.Html.HtmlTokenId.Tag || this.nextParserToken.TagIndex >= Internal.Html.HtmlTagIndex.Unknown))
            goto label_13;
        }
        this.ParseToken();
      }
      this.state = HtmlReader.State.EndSpecialTag;
      return;
label_11:
      this.state = HtmlReader.State.EndTag;
      return;
label_13:
      this.state = HtmlReader.State.EndText;
    }

    public void Close()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.parser != null && this.parser is IDisposable)
          ((IDisposable) this.parser).Dispose();
        if (this.input != null && this.input is IDisposable)
          ((IDisposable) this.input).Dispose();
      }
      this.parser = (Internal.Html.IHtmlParser) null;
      this.input = (object) null;
      this.stringBuildSink = (StringBuildSink) null;
      this.parserToken = (Internal.Html.HtmlToken) null;
      this.nextParserToken = (Internal.Html.HtmlToken) null;
      this.state = HtmlReader.State.Disposed;
    }

    internal HtmlReader SetInputEncoding(Encoding value)
    {
      this.InputEncoding = value;
      return this;
    }

    internal HtmlReader SetDetectEncodingFromByteOrderMark(bool value)
    {
      this.DetectEncodingFromByteOrderMark = value;
      return this;
    }

    internal HtmlReader SetNormalizeHtml(bool value)
    {
      this.NormalizeHtml = value;
      return this;
    }

    internal HtmlReader SetTestBoundaryConditions(bool value)
    {
      this.testBoundaryConditions = value;
      return this;
    }

    internal HtmlReader SetTestTraceStream(Stream value)
    {
      this.testTraceStream = value;
      return this;
    }

    internal HtmlReader SetTestTraceShowTokenNum(bool value)
    {
      this.testTraceShowTokenNum = value;
      return this;
    }

    internal HtmlReader SetTestTraceStopOnTokenNum(int value)
    {
      this.testTraceStopOnTokenNum = value;
      return this;
    }

    internal HtmlReader SetTestNormalizerTraceStream(Stream value)
    {
      this.testNormalizerTraceStream = value;
      return this;
    }

    internal HtmlReader SetTestNormalizerTraceShowTokenNum(bool value)
    {
      this.testNormalizerTraceShowTokenNum = value;
      return this;
    }

    internal HtmlReader SetTestNormalizerTraceStopOnTokenNum(int value)
    {
      this.testNormalizerTraceStopOnTokenNum = value;
      return this;
    }

    private void InitializeAndLock()
    {
      this.locked = true;
      ConverterInput input;
      if (this.input is Stream)
      {
        if (this.inputEncoding == null)
          throw new InvalidOperationException(CtsResources.TextConvertersStrings.InputEncodingRequired);
        input = (ConverterInput) new ConverterDecodingInput((Stream) this.input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, TextConvertersDefaults.MaxTokenSize(this.testBoundaryConditions), TextConvertersDefaults.MaxHtmlMetaRestartOffset(this.testBoundaryConditions), 16384, this.testBoundaryConditions, (IResultsFeedback) this, (IProgressMonitor) null);
      }
      else
        input = (ConverterInput) new ConverterUnicodeInput(this.input, false, TextConvertersDefaults.MaxTokenSize(this.testBoundaryConditions), this.testBoundaryConditions, (IProgressMonitor) null);
      Internal.Html.HtmlParser parser = new Internal.Html.HtmlParser(input, false, false, TextConvertersDefaults.MaxTokenRuns(this.testBoundaryConditions), TextConvertersDefaults.MaxHtmlAttributes(this.testBoundaryConditions), this.testBoundaryConditions);
      if (this.normalizeInputHtml)
        this.parser = (Internal.Html.IHtmlParser) new Internal.Html.HtmlNormalizingParser(parser, (HtmlInjection) null, false, TextConvertersDefaults.MaxHtmlNormalizerNesting(this.testBoundaryConditions), this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
      else
        this.parser = (Internal.Html.IHtmlParser) parser;
    }

    bool IRestartable.CanRestart()
    {
      return false;
    }

    void IRestartable.Restart()
    {
    }

    void IRestartable.DisableRestart()
    {
    }

    void IResultsFeedback.Set(ConfigParameter parameterId, object val)
    {
      if (parameterId != ConfigParameter.InputEncoding)
        return;
      this.inputEncoding = (Encoding) val;
    }

    private void ParseToken()
    {
      if (this.nextParserToken != null)
      {
        this.parserToken = this.nextParserToken;
        this.parserTokenId = this.parserToken.HtmlTokenId;
        this.nextParserToken = (Internal.Html.HtmlToken) null;
      }
      else
      {
        this.parserTokenId = this.parser.Parse();
        this.parserToken = this.parser.Token;
      }
    }

    private Internal.Html.HtmlTokenId PreviewNextToken()
    {
      if (this.nextParserToken == null)
      {
        int num = (int) this.parser.Parse();
        this.nextParserToken = this.parser.Token;
      }
      return this.nextParserToken.HtmlTokenId;
    }

    private StringBuildSink GetStringBuildSink()
    {
      if (this.stringBuildSink == null)
        this.stringBuildSink = new StringBuildSink();
      this.stringBuildSink.Reset(int.MaxValue);
      return this.stringBuildSink;
    }

    internal bool AttributeReader_ReadNextAttribute()
    {
      if (this.state == HtmlReader.State.EndTag)
        return false;
      this.AssertInTag();
      if (this.state == HtmlReader.State.ReadTag)
        throw new InvalidOperationException("Cannot read attributes after reading tag as markup text");
      do
      {
        if (this.state >= HtmlReader.State.BeginTag && this.state < HtmlReader.State.BeginAttribute)
        {
          while (this.parserToken.Attributes.Count == 0 && !this.parserToken.IsTagEnd)
            this.ParseToken();
          if (this.parserToken.Attributes.Count == 0)
          {
            this.state = HtmlReader.State.EndTag;
            return false;
          }
          this.currentAttribute = 0;
          this.state = HtmlReader.State.BeginAttribute;
        }
        else if (++this.currentAttribute == this.parserToken.Attributes.Count)
        {
          if (this.parserToken.IsTagEnd)
          {
            this.state = HtmlReader.State.EndTag;
            return false;
          }
          do
          {
            this.ParseToken();
            if (this.parserToken.Attributes.Count != 0 && (this.parserToken.Attributes[0].IsAttrBegin || this.parserToken.Attributes.Count > 1))
              goto label_16;
          }
          while (!this.parserToken.IsTagEnd);
          this.state = HtmlReader.State.EndTag;
          return false;
label_16:
          this.currentAttribute = 0;
          if (!this.parserToken.Attributes[0].IsAttrBegin)
            ++this.currentAttribute;
        }
      }
      while (this.parserToken.Attributes[this.currentAttribute].IsAttrEmptyName);
      this.state = HtmlReader.State.BeginAttribute;
      return true;
    }

    internal HtmlAttributeId AttributeReader_GetCurrentAttributeId()
    {
      this.AssertInAttribute();
      return Internal.Html.HtmlNameData.Names[(int) this.parserToken.Attributes[this.currentAttribute].NameIndex].PublicAttributeId;
    }

    internal bool AttributeReader_CurrentAttributeNameIsLong()
    {
      if (this.state != HtmlReader.State.BeginAttribute)
      {
        this.AssertInAttribute();
        throw new InvalidOperationException();
      }
      if (this.parserToken.Attributes[this.currentAttribute].NameIndex == Internal.Html.HtmlNameIndex.Unknown && this.parserToken.Attributes[this.currentAttribute].IsAttrBegin)
        return !this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd;
      return false;
    }

    internal string AttributeReader_ReadCurrentAttributeName()
    {
      if (this.state != HtmlReader.State.BeginAttribute)
      {
        this.AssertInAttribute();
        throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
      }
      string str;
      if (this.parserToken.Attributes[this.currentAttribute].NameIndex != Internal.Html.HtmlNameIndex.Unknown)
      {
        str = Internal.Html.HtmlNameData.Names[(int) this.parserToken.Attributes[this.currentAttribute].NameIndex].Name;
      }
      else
      {
        if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
          return this.parserToken.Attributes[this.currentAttribute].Name.GetString(int.MaxValue);
        StringBuildSink stringBuildSink = this.GetStringBuildSink();
        this.parserToken.Attributes[this.currentAttribute].Name.WriteTo((ITextSink) stringBuildSink);
        do
        {
          this.ParseToken();
          this.currentAttribute = 0;
          this.parserToken.Attributes[this.currentAttribute].Name.WriteTo((ITextSink) stringBuildSink);
        }
        while (!this.parserToken.Attributes[0].IsAttrNameEnd);
        str = stringBuildSink.ToString();
      }
      this.state = HtmlReader.State.EndAttributeName;
      return str;
    }

    internal int AttributeReader_ReadCurrentAttributeName(char[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TextConvertersStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      if (this.state < HtmlReader.State.BeginAttribute || this.state > HtmlReader.State.EndAttributeName)
      {
        this.AssertInAttribute();
        throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
      }
      int num1 = 0;
      if (this.state != HtmlReader.State.EndAttributeName)
      {
        if (this.state == HtmlReader.State.BeginAttribute)
        {
          this.state = HtmlReader.State.ReadAttributeName;
          this.parserToken.Attributes[this.currentAttribute].Name.Rewind();
        }
        while (count != 0)
        {
          int num2 = this.parserToken.Attributes[this.currentAttribute].Name.Read(buffer, offset, count);
          if (num2 == 0)
          {
            if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
            {
              this.state = HtmlReader.State.EndAttributeName;
              break;
            }
            this.ParseToken();
            this.currentAttribute = 0;
            this.parserToken.Attributes[this.currentAttribute].Name.Rewind();
          }
          else
          {
            offset += num2;
            count -= num2;
            num1 += num2;
          }
        }
      }
      return num1;
    }

    internal void AttributeReader_WriteCurrentAttributeNameTo(ITextSink sink)
    {
      if (this.state != HtmlReader.State.BeginAttribute)
      {
        this.AssertInAttribute();
        throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
      }
      while (true)
      {
        this.parserToken.Attributes[this.currentAttribute].Name.WriteTo(sink);
        if (!this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
        {
          this.ParseToken();
          this.currentAttribute = 0;
        }
        else
          break;
      }
      this.state = HtmlReader.State.EndAttributeName;
    }

    internal bool AttributeReader_CurrentAttributeHasValue()
    {
      if (this.state != HtmlReader.State.BeginAttributeValue)
      {
        this.AssertInAttribute();
        if (this.state > HtmlReader.State.BeginAttributeValue)
          throw new InvalidOperationException("Reader must be positioned before attribute value");
        if (!this.SkipToAttributeValue())
        {
          this.state = HtmlReader.State.EndAttributeName;
          return false;
        }
        this.state = HtmlReader.State.BeginAttributeValue;
      }
      return true;
    }

    internal bool AttributeReader_CurrentAttributeValueIsLong()
    {
      if (this.state != HtmlReader.State.BeginAttributeValue)
      {
        this.AssertInAttribute();
        if (this.state > HtmlReader.State.BeginAttributeValue)
          throw new InvalidOperationException("Reader must be positioned before attribute value");
        if (!this.SkipToAttributeValue())
        {
          this.state = HtmlReader.State.EndAttributeName;
          return false;
        }
        this.state = HtmlReader.State.BeginAttributeValue;
      }
      if (this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin)
        return !this.parserToken.Attributes[this.currentAttribute].IsAttrEnd;
      return false;
    }

    internal string AttributeReader_ReadCurrentAttributeValue()
    {
      if (this.state != HtmlReader.State.BeginAttributeValue)
      {
        this.AssertInAttribute();
        if (this.state > HtmlReader.State.BeginAttributeValue)
          throw new InvalidOperationException("Reader must be positioned before attribute value");
        if (!this.SkipToAttributeValue())
        {
          this.state = HtmlReader.State.EndAttribute;
          return (string) null;
        }
      }
      if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
        return this.parserToken.Attributes[this.currentAttribute].Value.GetString(int.MaxValue);
      StringBuildSink stringBuildSink = this.GetStringBuildSink();
      this.parserToken.Attributes[this.currentAttribute].Value.WriteTo((ITextSink) stringBuildSink);
      do
      {
        this.ParseToken();
        this.currentAttribute = 0;
        this.parserToken.Attributes[0].Value.WriteTo((ITextSink) stringBuildSink);
      }
      while (!this.parserToken.Attributes[0].IsAttrEnd);
      this.state = HtmlReader.State.EndAttribute;
      return stringBuildSink.ToString();
    }

    internal int AttributeReader_ReadCurrentAttributeValue(char[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TextConvertersStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TextConvertersStrings.CountTooLarge);
      this.AssertInAttribute();
      if (this.state < HtmlReader.State.BeginAttributeValue)
      {
        if (!this.SkipToAttributeValue())
        {
          this.state = HtmlReader.State.EndAttribute;
          return 0;
        }
        this.state = HtmlReader.State.BeginAttributeValue;
      }
      int num1 = 0;
      if (this.state != HtmlReader.State.EndAttribute)
      {
        if (this.state == HtmlReader.State.BeginAttributeValue)
        {
          this.state = HtmlReader.State.ReadAttributeValue;
          this.parserToken.Attributes[this.currentAttribute].Value.Rewind();
        }
        while (count != 0)
        {
          int num2 = this.parserToken.Attributes[this.currentAttribute].Value.Read(buffer, offset, count);
          if (num2 == 0)
          {
            if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
            {
              this.state = HtmlReader.State.EndAttribute;
              break;
            }
            this.ParseToken();
            this.currentAttribute = 0;
            this.parserToken.Attributes[this.currentAttribute].Value.Rewind();
          }
          else
          {
            offset += num2;
            count -= num2;
            num1 += num2;
          }
        }
      }
      return num1;
    }

    internal void AttributeReader_WriteCurrentAttributeValueTo(ITextSink sink)
    {
      if (this.state != HtmlReader.State.BeginAttributeValue)
      {
        this.AssertInAttribute();
        if (this.state > HtmlReader.State.BeginAttributeValue)
          throw new InvalidOperationException("Reader must be positioned before attribute value");
        if (!this.SkipToAttributeValue())
        {
          this.state = HtmlReader.State.EndAttribute;
          return;
        }
      }
      while (true)
      {
        this.parserToken.Attributes[this.currentAttribute].Value.WriteTo(sink);
        if (!this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
        {
          this.ParseToken();
          this.currentAttribute = 0;
        }
        else
          break;
      }
      this.state = HtmlReader.State.EndAttribute;
    }

    private bool SkipToAttributeValue()
    {
      if (!this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin)
      {
        if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
          return false;
        do
        {
          this.ParseToken();
        }
        while (!this.parserToken.Attributes[0].IsAttrValueBegin && !this.parserToken.Attributes[0].IsAttrEnd);
        if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
          return false;
      }
      return true;
    }

    private void AssertNotLocked()
    {
      this.AssertNotDisposed();
      if (this.locked)
        throw new InvalidOperationException("Cannot set reader properties after reading a first token");
    }

    private void AssertNotDisposed()
    {
      if (this.state == HtmlReader.State.Disposed)
        throw new ObjectDisposedException("HtmlReader");
    }

    private void AssertInToken()
    {
      if (this.state <= HtmlReader.State.Begin)
      {
        this.AssertNotDisposed();
        throw new InvalidOperationException("Reader must be positioned inside a valid token");
      }
    }

    private void AssertInTag()
    {
      if (this.state < HtmlReader.State.BeginTag)
      {
        this.AssertInToken();
        throw new InvalidOperationException("Reader must be positioned inside a StartTag, EndTag or EmptyElementTag token");
      }
    }

    private void AssertInAttribute()
    {
      if (this.state < HtmlReader.State.BeginAttribute || this.state > HtmlReader.State.EndAttribute)
      {
        this.AssertInTag();
        throw new InvalidOperationException("Reader must be positioned inside attribute");
      }
    }

    private enum State : byte
    {
      Disposed,
      EndOfFile,
      Begin,
      Text,
      EndText,
      OverlappedClose,
      OverlappedReopen,
      SpecialTag,
      EndSpecialTag,
      BeginTag,
      ReadTagName,
      EndTagName,
      BeginAttribute,
      ReadAttributeName,
      EndAttributeName,
      BeginAttributeValue,
      ReadAttributeValue,
      EndAttribute,
      ReadTag,
      EndTag,
    }
  }
}
