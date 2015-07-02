// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlFormatConverterWithEncapsulation
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlFormatConverterWithEncapsulation : HtmlFormatConverter
  {
    private readonly bool encapsulateMarkup;
    private HtmlFormatConverterWithEncapsulation.MarkupSink markupSink;

    public HtmlFormatConverterWithEncapsulation(HtmlNormalizingParser parser, Format.FormatOutput output, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream, IProgressMonitor progressMonitor)
      : this(parser, output, false, testTreatNbspAsBreakable, traceStream, traceShowTokenNum, traceStopOnTokenNum, formatConverterTraceStream, progressMonitor)
    {
    }

    public HtmlFormatConverterWithEncapsulation(HtmlNormalizingParser parser, Format.FormatOutput output, bool encapsulateMarkup, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream, IProgressMonitor progressMonitor)
      : base(parser, output, testTreatNbspAsBreakable, traceStream, traceShowTokenNum, traceStopOnTokenNum, formatConverterTraceStream, progressMonitor)
    {
      this.encapsulateMarkup = encapsulateMarkup;
      if (this.output == null || !this.encapsulateMarkup)
        return;
      this.output.Initialize(this.Store, Format.SourceFormat.HtmlEncapsulateMarkup, "converted from html");
    }

    protected override void Process(HtmlTokenId tokenId)
    {
      this.token = this.parser.Token;
      switch (tokenId)
      {
        case HtmlTokenId.EndOfFile:
          this.CloseAllContainersAndSetEOF();
          break;
        case HtmlTokenId.Text:
          if (this.insideStyle)
          {
            this.OutputEncapsulatedMarkup();
            this.token.Text.WriteTo((ITextSink) this.cssParserInput);
            break;
          }
          if (this.insideComment)
          {
            this.OutputEncapsulatedMarkup();
            break;
          }
          if (this.insidePre)
          {
            this.ProcessPreformatedText();
            break;
          }
          this.ProcessText();
          break;
        case HtmlTokenId.EncodingChange:
          if (this.output == null || !this.output.OutputCodePageSameAsInput)
            break;
          this.output.OutputEncoding = this.token.TokenEncoding;
          break;
        case HtmlTokenId.Tag:
          this.OutputEncapsulatedMarkup();
          if (this.token.TagIndex <= HtmlTagIndex.Unknown)
          {
            if (!this.insideStyle || this.token.TagIndex != HtmlTagIndex._COMMENT)
              break;
            this.token.Text.WriteTo((ITextSink) this.cssParserInput);
            break;
          }
          HtmlDtd.TagDefinition tagDefinition = HtmlFormatConverter.GetTagDefinition(this.token.TagIndex);
          if (!this.token.IsEndTag)
          {
            if (this.token.IsTagBegin)
              this.PushElement(tagDefinition, this.token.IsEmptyScope);
            this.ProcessStartTagAttributes(tagDefinition);
            break;
          }
          if (!this.token.IsTagEnd)
            break;
          this.PopElement(this.BuildStackTop - 1 - this.temporarilyClosedLevels, this.token.Argument != 1);
          break;
        case HtmlTokenId.OverlappedClose:
          this.temporarilyClosedLevels = this.token.Argument;
          break;
        case HtmlTokenId.OverlappedReopen:
          this.temporarilyClosedLevels = 0;
          break;
      }
    }

    private void OutputEncapsulatedMarkup()
    {
      if (!this.encapsulateMarkup)
        return;
      if (this.markupSink == null)
        this.markupSink = new HtmlFormatConverterWithEncapsulation.MarkupSink((HtmlFormatConverter) this);
      if (this.token.IsEndTag && this.token.TagIndex > HtmlTagIndex.Unknown)
      {
        char[] endTagText = this.GetEndTagText(this.token);
        this.markupSink.Write(endTagText, 0, endTagText.Length);
      }
      else
        this.token.Text.WriteTo((ITextSink) this.markupSink);
    }

    private char[] GetEndTagText(HtmlToken htmlToken)
    {
      char[] chArray1 = this.token.NameIndex.ToString().ToCharArray();
      char[] chArray2 = new char[chArray1.Length + 3];
      chArray2[0] = '<';
      chArray2[1] = '/';
      chArray1.CopyTo((Array) chArray2, 2);
      chArray2[chArray2.Length - 1] = '>';
      return chArray2;
    }

    private class MarkupSink : ITextSink
    {
      private HtmlFormatConverter builder;
      private char[] literalBuffer;

      public bool IsEnough
      {
        get
        {
          return false;
        }
      }

      public MarkupSink(HtmlFormatConverter builder)
      {
        this.builder = builder;
        this.literalBuffer = new char[2];
      }

      public void Write(char[] buffer, int offset, int count)
      {
        this.builder.AddMarkupText(buffer, offset, count);
      }

      public void Write(int ucs32Char)
      {
        int count = Token.LiteralLength(ucs32Char);
        this.literalBuffer[0] = Token.LiteralFirstChar(ucs32Char);
        if (count > 1)
          this.literalBuffer[1] = Token.LiteralLastChar(ucs32Char);
        this.builder.AddMarkupText(this.literalBuffer, 0, count);
      }
    }
  }
}
