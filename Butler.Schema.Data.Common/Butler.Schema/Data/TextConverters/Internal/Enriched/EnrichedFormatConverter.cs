// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Enriched.EnrichedFormatConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Enriched
{
  internal class EnrichedFormatConverter : Format.FormatConverter, IProducerConsumer, IDisposable
  {
    private char[] literalBuffer = new char[2];
    public const int PointsPerIndent = 30;
    public const int LeftIndentIncrementPoints = 12;
    public const int MaxIndent = 50;
    private EnrichedParser parser;
    private Format.FormatOutput output;
    private Html.HtmlToken token;
    private bool treatNbspAsBreakable;
    private bool insideParam;
    private Html.HtmlNameIndex tagPendingParameter;
    private int indentLevel;
    private ScratchBuffer scratch;
    private Injection injection;

    public EnrichedFormatConverter(EnrichedParser parser, Format.FormatOutput output, Injection injection, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream)
      : base(formatConverterTraceStream)
    {
      this.treatNbspAsBreakable = testTreatNbspAsBreakable;
      this.output = output;
      if (this.output != null)
        this.output.Initialize(this.Store, Format.SourceFormat.Rtf, "converted from text/enriched");
      this.parser = parser;
      this.injection = injection;
      this.InitializeDocument();
      if (this.injection == null)
        return;
      int num = this.injection.HaveHead ? 1 : 0;
    }

    public override void Run()
    {
      if (this.EndOfFile)
        return;
      Html.HtmlTokenId tokenId = this.parser.Parse();
      if (tokenId == Html.HtmlTokenId.None)
        return;
      this.Process(tokenId);
    }

    public bool Flush()
    {
      if (!this.EndOfFile)
        this.Run();
      return this.EndOfFile;
    }

    private void Process(Html.HtmlTokenId tokenId)
    {
      this.token = this.parser.Token;
      switch (tokenId)
      {
        case Html.HtmlTokenId.EndOfFile:
          if (this.injection != null)
          {
            int num1 = this.injection.HaveHead ? 1 : 0;
          }
          this.CloseAllContainersAndSetEOF();
          if (this.output == null)
            break;
          this.output.Flush();
          break;
        case Html.HtmlTokenId.Text:
          if (!this.insideParam)
          {
            this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
            this.OutputText(this.token);
            break;
          }
          if (this.tagPendingParameter == Html.HtmlNameIndex._NOTANAME)
            break;
          this.scratch.AppendTokenText((Token) this.token, 256);
          break;
        case Html.HtmlTokenId.EncodingChange:
          if (this.output == null || !this.output.OutputCodePageSameAsInput)
            break;
          this.output.OutputEncoding = this.token.TokenEncoding;
          break;
        case Html.HtmlTokenId.Tag:
          if (!this.token.IsTagBegin)
            break;
          if (this.token.IsEndTag)
          {
            if (this.token.NameIndex == Html.HtmlNameIndex.Param)
            {
              if (this.insideParam)
              {
                Html.HtmlNameIndex htmlNameIndex = this.tagPendingParameter;
                if ((uint) htmlNameIndex <= 105U)
                {
                  if (htmlNameIndex != Html.HtmlNameIndex.Color)
                  {
                    if (htmlNameIndex == Html.HtmlNameIndex.ParaIndent)
                    {
                      BufferString bufferString1 = this.scratch.BufferString;
                      bufferString1.TrimWhitespace();
                      int num2 = 0;
                      int num3 = 0;
                      int num4 = 0;
                      int num5 = 0;
                      int index1;
                      for (int offset = 0; offset != bufferString1.Length; offset = index1)
                      {
                        int index2 = offset;
                        index1 = offset;
                        while (index2 < bufferString1.Length && (int) bufferString1[index2] != 44)
                        {
                          ++index2;
                          ++index1;
                        }
                        while (index2 > offset && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(bufferString1[index2 - 1])))
                          --index2;
                        if (index1 < bufferString1.Length)
                        {
                          do
                          {
                            ++index1;
                          }
                          while (index1 < bufferString1.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(bufferString1[index1])));
                        }
                        BufferString bufferString2 = bufferString1.SubString(offset, index2 - offset);
                        if (bufferString2.EqualsToLowerCaseStringIgnoreCase("left"))
                          ++num2;
                        else if (bufferString2.EqualsToLowerCaseStringIgnoreCase("right"))
                          ++num3;
                        else if (bufferString2.EqualsToLowerCaseStringIgnoreCase("in"))
                          ++num4;
                        else if (bufferString2.EqualsToLowerCaseStringIgnoreCase("out"))
                          ++num5;
                      }
                      if (num2 + num5 != 0)
                        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.LeftPadding, new Format.PropertyValue(Format.LengthUnits.Points, 30 * (num2 + num5) - (this.indentLevel == 0 ? 12 : 0)));
                      if (num4 - num5 != 0)
                      {
                        if (num4 - num5 > 0)
                          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FirstLineIndent, new Format.PropertyValue(Format.LengthUnits.Points, 30 * (num4 - num5) - (this.indentLevel != 0 || num2 + num5 != 0 ? 0 : 12)));
                        else
                          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FirstLineIndent, new Format.PropertyValue(Format.LengthUnits.Points, 30 * (num4 - num5) + (this.indentLevel != 0 || num5 - num4 != num2 + num5 ? 0 : 12)));
                      }
                      if (num3 != 0)
                        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.RightPadding, new Format.PropertyValue(Format.LengthUnits.Points, 30 * num3));
                      if (num2 + num5 != 0 && this.indentLevel == 0)
                        ++this.indentLevel;
                    }
                  }
                  else
                  {
                    Format.PropertyValue propertyValue = Html.HtmlSupport.ParseColor(this.scratch.BufferString, true, false);
                    if (propertyValue.IsColor)
                    {
                      if (propertyValue.Color.Red > 250U && propertyValue.Color.Green > 250U && propertyValue.Color.Blue > 250U)
                        propertyValue = new Format.PropertyValue(Format.PropertyType.Color, 14737632U);
                      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontColor, propertyValue);
                    }
                  }
                }
                else if (htmlNameIndex != Html.HtmlNameIndex.Excerpt)
                {
                  if (htmlNameIndex != Html.HtmlNameIndex.Lang)
                  {
                    if (htmlNameIndex == Html.HtmlNameIndex.FontFamily)
                    {
                      Format.PropertyValue propertyValue = Html.HtmlSupport.ParseFontFace(this.scratch.BufferString, (Format.FormatConverter) this);
                      if (!propertyValue.IsNull)
                        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontFace, propertyValue);
                    }
                  }
                  else
                  {
                    Format.PropertyValue propertyValue = Html.HtmlSupport.ParseLanguage(this.scratch.BufferString, (Format.FormatConverter) null);
                    if (!propertyValue.IsNull)
                      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Language, propertyValue);
                  }
                }
                this.insideParam = false;
              }
            }
            else if (this.token.NameIndex != Html.HtmlNameIndex.Unknown)
            {
              if (this.token.NameIndex == Html.HtmlNameIndex.ParaIndent && this.indentLevel != 0)
                --this.indentLevel;
              this.CloseContainer(this.token.NameIndex);
            }
            this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
            break;
          }
          Html.HtmlNameIndex htmlNameIndex1 = this.token.NameIndex;
          if ((uint) htmlNameIndex1 <= 105U)
          {
            if ((uint) htmlNameIndex1 <= 24U)
            {
              if (htmlNameIndex1 != Html.HtmlNameIndex.Nofill)
              {
                if (htmlNameIndex1 != Html.HtmlNameIndex.FlushRight)
                {
                  if (htmlNameIndex1 == Html.HtmlNameIndex.Param)
                  {
                    this.insideParam = true;
                    this.scratch.Reset();
                    break;
                  }
                }
                else
                {
                  this.OpenContainer(Format.FormatContainerType.Block, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
                  this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.TextAlignment, new Format.PropertyValue((Enum) Format.TextAlign.Right));
                  this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                  break;
                }
              }
              else
              {
                this.OpenContainer(Format.FormatContainerType.Block, false, 2, this.GetStyle(14), this.token.NameIndex);
                this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                break;
              }
            }
            else if ((uint) htmlNameIndex1 <= 65U)
            {
              if (htmlNameIndex1 != Html.HtmlNameIndex.Color)
              {
                if (htmlNameIndex1 == Html.HtmlNameIndex.Italic)
                {
                  this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, this.GetStyle(4), this.token.NameIndex);
                  this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                  break;
                }
              }
              else
              {
                this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
                this.tagPendingParameter = Html.HtmlNameIndex.Color;
                break;
              }
            }
            else
            {
              switch (htmlNameIndex1)
              {
                case Html.HtmlNameIndex.Center:
                  this.OpenContainer(Format.FormatContainerType.Block, false, 2, this.GetStyle(13), this.token.NameIndex);
                  this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                  return;
                case Html.HtmlNameIndex.Underline:
                  this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, this.GetStyle(5), this.token.NameIndex);
                  this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                  return;
                case Html.HtmlNameIndex.FlushBoth:
                  this.OpenContainer(Format.FormatContainerType.Block, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
                  this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.TextAlignment, new Format.PropertyValue((Enum) Format.TextAlign.Justify));
                  this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                  return;
                case Html.HtmlNameIndex.ParaIndent:
                  this.OpenContainer(Format.FormatContainerType.Block, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
                  this.tagPendingParameter = Html.HtmlNameIndex.ParaIndent;
                  if (this.indentLevel == 0)
                    return;
                  ++this.indentLevel;
                  return;
              }
            }
          }
          else if ((uint) htmlNameIndex1 <= 148U)
          {
            switch (htmlNameIndex1)
            {
              case Html.HtmlNameIndex.Fixed:
                this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, this.GetStyle(10), this.token.NameIndex);
                this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                return;
              case Html.HtmlNameIndex.Smaller:
                this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
                this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.PropertyType.RelHtmlFontUnits, -1));
                this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                return;
              case Html.HtmlNameIndex.Bold:
                this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, this.GetStyle(1), this.token.NameIndex);
                this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                return;
              case Html.HtmlNameIndex.FlushLeft:
                this.OpenContainer(Format.FormatContainerType.Block, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
                this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.TextAlignment, new Format.PropertyValue((Enum) Format.TextAlign.Left));
                this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
                return;
            }
          }
          else if ((uint) htmlNameIndex1 <= 159U)
          {
            if (htmlNameIndex1 != Html.HtmlNameIndex.Excerpt)
            {
              if (htmlNameIndex1 == Html.HtmlNameIndex.Lang)
              {
                this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, this.GetStyle(6), this.token.NameIndex);
                this.tagPendingParameter = Html.HtmlNameIndex.Lang;
                break;
              }
            }
            else
            {
              this.OpenContainer(Format.FormatContainerType.Block, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.QuotingLevelDelta, new Format.PropertyValue(Format.PropertyType.Integer, 1));
              this.tagPendingParameter = Html.HtmlNameIndex.Excerpt;
              break;
            }
          }
          else if (htmlNameIndex1 != Html.HtmlNameIndex.Bigger)
          {
            if (htmlNameIndex1 == Html.HtmlNameIndex.FontFamily)
            {
              this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
              this.tagPendingParameter = Html.HtmlNameIndex.FontFamily;
              break;
            }
          }
          else
          {
            this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, 2, Format.FormatStyle.Null, this.token.NameIndex);
            this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.PropertyType.RelHtmlFontUnits, 1));
            this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
            break;
          }
          this.tagPendingParameter = Html.HtmlNameIndex._NOTANAME;
          break;
      }
    }

    private void OutputText(Html.HtmlToken token)
    {
      Token.RunEnumerator enumerator = token.Runs.GetEnumerator();
      while (enumerator.MoveNext())
      {
        TokenRun current = enumerator.Current;
        if (current.IsTextRun)
        {
          if (current.IsAnyWhitespace)
          {
            switch (current.TextType)
            {
              case RunTextType.NewLine:
                this.AddLineBreak(1);
                continue;
              case RunTextType.Tabulation:
                this.AddTabulation(current.Length);
                continue;
              default:
                this.AddSpace(current.Length);
                continue;
            }
          }
          else if (current.TextType == RunTextType.Nbsp)
          {
            if (this.treatNbspAsBreakable)
              this.AddSpace(current.Length);
            else
              this.AddNbsp(current.Length);
          }
          else if (current.IsLiteral)
            this.AddNonSpaceText(this.literalBuffer, 0, current.ReadLiteral(this.literalBuffer));
          else
            this.AddNonSpaceText(current.RawBuffer, current.RawOffset, current.RawLength);
        }
      }
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
        if (this.parser != null)
          this.parser.Dispose();
        if (this.output != null && this.output != null)
          ((IDisposable) this.output).Dispose();
        if (this.token != null && this.token is IDisposable)
          ((IDisposable) this.token).Dispose();
      }
      this.parser = (EnrichedParser) null;
      this.output = (Format.FormatOutput) null;
      this.token = (Html.HtmlToken) null;
      this.literalBuffer = (char[]) null;
    }
  }
}
