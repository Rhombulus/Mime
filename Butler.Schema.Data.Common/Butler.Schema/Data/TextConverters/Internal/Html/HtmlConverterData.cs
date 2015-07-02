// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlConverterData
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal static class HtmlConverterData
  {
    public static HtmlTagInstruction[] tagInstructions = new HtmlTagInstruction[122]
    {
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.HyperLink, 0, 2, new HtmlAttributeInstruction[6]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Name, Format.PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Href, Format.PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Target, Format.PropertyId.HyperlinkTarget, HtmlConverterData.PropertyValueParsingMethods.ParseTarget),
        new HtmlAttributeInstruction(HtmlNameIndex.Id, Format.PropertyId.Id, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 4, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.Area, 0, 2, new HtmlAttributeInstruction[5]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Href, Format.PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Target, Format.PropertyId.HyperlinkTarget, HtmlConverterData.PropertyValueParsingMethods.ParseTarget),
        new HtmlAttributeInstruction(HtmlNameIndex.Alt, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 1, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.BaseFont, 0, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Face, Format.PropertyId.FontFace, HtmlConverterData.PropertyValueParsingMethods.ParseFontFace),
        new HtmlAttributeInstruction(HtmlNameIndex.Size, Format.PropertyId.FontSize, HtmlConverterData.PropertyValueParsingMethods.ParseFontSize),
        new HtmlAttributeInstruction(HtmlNameIndex.Color, Format.PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Inline, 11, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 2, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.BlockQuote, 16, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.BGColor, Format.PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.Button, 0, 2, new HtmlAttributeInstruction[5]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Name, Format.PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Value, Format.PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Disabled, Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.TableCaption, 13, 2, new HtmlAttributeInstruction[1]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 13, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 9, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.TableColumn, 0, 2, new HtmlAttributeInstruction[6]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Span, Format.PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Valign, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.TableColumnGroup, 0, 2, new HtmlAttributeInstruction[6]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Span, Format.PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Valign, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 25, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 3, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.List, 24, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[4]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage),
        new HtmlAttributeInstruction(HtmlNameIndex.Id, Format.PropertyId.Id, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 25, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.FieldSet, 0, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[5]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Face, Format.PropertyId.FontFace, HtmlConverterData.PropertyValueParsingMethods.ParseFontFace),
        new HtmlAttributeInstruction(HtmlNameIndex.Size, Format.PropertyId.FontSize, HtmlConverterData.PropertyValueParsingMethods.ParseFontSize),
        new HtmlAttributeInstruction(HtmlNameIndex.Color, Format.PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Form, 0, 2, new HtmlAttributeInstruction[6]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Action, Format.PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.EncType, Format.PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Accept, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.AcceptCharset, Format.PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 17, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 18, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 19, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 20, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 21, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 22, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.HorizontalLine, 0, 2, new HtmlAttributeInstruction[4]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Size, Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Color, Format.PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseHorizontalAlignment)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[4]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Height, Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Src, Format.PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Image, 0, 2, new HtmlAttributeInstruction[8]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Src, Format.PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Height, Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Border, Format.PropertyId.TableBorder, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Alt, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Image, 0, 2, new HtmlAttributeInstruction[8]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Src, Format.PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Height, Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Border, Format.PropertyId.TableBorder, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Alt, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Input, 0, 2, new HtmlAttributeInstruction[11]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Name, Format.PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.ReadOnly, Format.PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Disabled, Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Checked, Format.PropertyId.Overloaded3, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Value, Format.PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Size, Format.PropertyId.TableFrame, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.MaxLength, Format.PropertyId.TableRules, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Src, Format.PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Alt, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 5, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[5]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Name, Format.PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Prompt, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Disabled, Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 9, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Label, 0, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.For, Format.PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Legend, 0, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.ListItem, 0, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Start, Format.PropertyId.ListStart, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 15, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Map, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.List, 24, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 12, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.List, 23, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Start, Format.PropertyId.ListStart, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.OptionGroup, 0, 2, new HtmlAttributeInstruction[4]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Label, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Disabled, Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Option, 0, 2, new HtmlAttributeInstruction[6]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Selected, Format.PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Label, Format.PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Value, Format.PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
        new HtmlAttributeInstruction(HtmlNameIndex.Disabled, Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 14, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 14, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage),
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 3, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 9, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.Select, 0, 2, new HtmlAttributeInstruction[3]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Name, Format.PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.Multiple, Format.PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Disabled, Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 6, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 3, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 1, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 7, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 8, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.Table, 0, 3, new HtmlAttributeInstruction[10]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseHorizontalAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Border, Format.PropertyId.TableBorder, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Frame, Format.PropertyId.TableFrame, HtmlConverterData.PropertyValueParsingMethods.ParseTableFrame),
        new HtmlAttributeInstruction(HtmlNameIndex.Rules, Format.PropertyId.TableRules, HtmlConverterData.PropertyValueParsingMethods.ParseTableRules),
        new HtmlAttributeInstruction(HtmlNameIndex.BGColor, Format.PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
        new HtmlAttributeInstruction(HtmlNameIndex.CellSpacing, Format.PropertyId.TableCellSpacing, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.CellPadding, Format.PropertyId.TableCellPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.TableExtraContent, 0, 2, (HtmlAttributeInstruction[]) null),
      new HtmlTagInstruction(Format.FormatContainerType.TableCell, 0, 2, new HtmlAttributeInstruction[10]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Height, Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.ColSpan, Format.PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.RowSpan, Format.PropertyId.NumRows, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Valign, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.NoWrap, Format.PropertyId.TableCellNoWrap, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.BGColor, Format.PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.TextArea, 0, 2, new HtmlAttributeInstruction[7]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Name, Format.PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
        new HtmlAttributeInstruction(HtmlNameIndex.ReadOnly, Format.PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Disabled, Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.Cols, Format.PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Rows, Format.PropertyId.NumRows, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.TableCell, 0, 2, new HtmlAttributeInstruction[10]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Height, Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Width, Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.ColSpan, Format.PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.RowSpan, Format.PropertyId.NumRows, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Valign, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.NoWrap, Format.PropertyId.TableCellNoWrap, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
        new HtmlAttributeInstruction(HtmlNameIndex.BGColor, Format.PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.TableRow, 0, 4, new HtmlAttributeInstruction[6]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Height, Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
        new HtmlAttributeInstruction(HtmlNameIndex.Align, Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.Valign, Format.PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
        new HtmlAttributeInstruction(HtmlNameIndex.BGColor, Format.PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 10, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 5, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.List, 24, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(Format.FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(Format.FormatContainerType.Block, 14, 2, new HtmlAttributeInstruction[2]
      {
        new HtmlAttributeInstruction(HtmlNameIndex.Dir, Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
        new HtmlAttributeInstruction(HtmlNameIndex.Lang, Format.PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
      }),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction(),
      new HtmlTagInstruction()
    };
    public static CssPropertyInstruction[] cssPropertyInstructions = new CssPropertyInstruction[137]
    {
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Null, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssWhiteSpace),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BlockAlignment, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssVerticalAlignment),
      new CssPropertyInstruction(Format.PropertyId.RightBorderWidth, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.FontFace, HtmlConverterData.PropertyValueParsingMethods.ParseFontFace, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BottomBorderWidth, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.BorderStyles, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.TableShowEmptyCells, HtmlConverterData.PropertyValueParsingMethods.ParseEmptyCells, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.FirstFlag, HtmlConverterData.PropertyValueParsingMethods.ParseFontWeight, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.TableCaptionSideTop, HtmlConverterData.PropertyValueParsingMethods.ParseCaptionSide, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.LeftMargin, HtmlConverterData.PropertyValueParsingMethods.ParseLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.BorderWidths, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.UnicodeBiDi, HtmlConverterData.PropertyValueParsingMethods.ParseUnicodeBiDi, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Paddings, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeNonNegativeLength),
      new CssPropertyInstruction(Format.PropertyId.BottomBorderWidth, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.Visible, HtmlConverterData.PropertyValueParsingMethods.ParseVisibility, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseTableLayout, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.LeftBorderColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Margins, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeLength),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BottomBorderStyle, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.BorderColors, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeColor),
      new CssPropertyInstruction(Format.PropertyId.Null, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssTextDecoration),
      new CssPropertyInstruction(Format.PropertyId.Display, HtmlConverterData.PropertyValueParsingMethods.ParseDisplay, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BottomMargin, HtmlConverterData.PropertyValueParsingMethods.ParseLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.BorderStyles, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorderStyle),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Null, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeAllBorders),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBorderCollapse, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.TableBorderSpacingVertical, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompoundBorderSpacing),
      new CssPropertyInstruction(Format.PropertyId.Null, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssTextTransform),
      new CssPropertyInstruction(Format.PropertyId.RightBorderWidth, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.FirstLineIndent, HtmlConverterData.PropertyValueParsingMethods.ParseLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BottomBorderColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.RightMargin, HtmlConverterData.PropertyValueParsingMethods.ParseLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.RightPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.RightBorderStyle, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BackColor, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBackground),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BorderWidths, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorderWidth),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BorderColors, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.SmallCaps, HtmlConverterData.PropertyValueParsingMethods.ParseFontVariant, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.FontSize, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeFont),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.RightBorderColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.Italic, HtmlConverterData.PropertyValueParsingMethods.ParseFontStyle, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.Margins, HtmlConverterData.PropertyValueParsingMethods.ParseLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.LeftBorderWidth, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.LeftBorderWidth, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.BottomPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.FontSize, HtmlConverterData.PropertyValueParsingMethods.ParseCssFontSize, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.LeftBorderStyle, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.Paddings, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(Format.PropertyId.LeftPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, (MultiPropertyParsingMethod) null),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(),
      new CssPropertyInstruction(Format.PropertyId.BorderWidths, (PropertyValueParsingMethod) null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
      new CssPropertyInstruction()
    };

    public static class DefaultStyle
    {
      public const int None = 0;
      public const int B = 1;
      public const int Big = 2;
      public const int Del = 3;
      public const int EM = 4;
      public const int I = 4;
      public const int Ins = 5;
      public const int S = 3;
      public const int Small = 6;
      public const int Strike = 3;
      public const int Strong = 1;
      public const int Sub = 7;
      public const int Sup = 8;
      public const int U = 5;
      public const int Var = 4;
      public const int Code = 9;
      public const int Cite = 4;
      public const int Dfn = 4;
      public const int Kbd = 9;
      public const int Samp = 9;
      public const int TT = 10;
      public const int Bdo = 11;
      public const int NoBR = 12;
      public const int Center = 13;
      public const int Xmp = 14;
      public const int Pre = 14;
      public const int Listing = 15;
      public const int PlainText = 14;
      public const int BlockQuote = 16;
      public const int Address = 4;
      public const int H1 = 17;
      public const int H2 = 18;
      public const int H3 = 19;
      public const int H4 = 20;
      public const int H5 = 21;
      public const int H6 = 22;
      public const int OL = 23;
      public const int UL = 24;
      public const int Dir = 24;
      public const int Menu = 24;
      public const int DT = 25;
      public const int DD = 25;
      public const int Caption = 13;
    }

    public static class PropertyValueParsingMethods
    {
      public static readonly PropertyValueParsingMethod ParseBlockAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseBlockAlignment);
      public static readonly PropertyValueParsingMethod ParseBooleanAttribute = new PropertyValueParsingMethod(HtmlSupport.ParseBooleanAttribute);
      public static readonly PropertyValueParsingMethod ParseBorderCollapse = new PropertyValueParsingMethod(HtmlSupport.ParseBorderCollapse);
      public static readonly PropertyValueParsingMethod ParseBorderStyle = new PropertyValueParsingMethod(HtmlSupport.ParseBorderStyle);
      public static readonly PropertyValueParsingMethod ParseBorderWidth = new PropertyValueParsingMethod(HtmlSupport.ParseBorderWidth);
      public static readonly PropertyValueParsingMethod ParseCaptionSide = new PropertyValueParsingMethod(HtmlSupport.ParseCaptionSide);
      public static readonly PropertyValueParsingMethod ParseColor = new PropertyValueParsingMethod(HtmlSupport.ParseColor);
      public static readonly PropertyValueParsingMethod ParseColorCss = new PropertyValueParsingMethod(HtmlSupport.ParseColorCss);
      public static readonly PropertyValueParsingMethod ParseCssFontSize = new PropertyValueParsingMethod(HtmlSupport.ParseCssFontSize);
      public static readonly PropertyValueParsingMethod ParseDirection = new PropertyValueParsingMethod(HtmlSupport.ParseDirection);
      public static readonly PropertyValueParsingMethod ParseDisplay = new PropertyValueParsingMethod(HtmlSupport.ParseDisplay);
      public static readonly PropertyValueParsingMethod ParseEmptyCells = new PropertyValueParsingMethod(HtmlSupport.ParseEmptyCells);
      public static readonly PropertyValueParsingMethod ParseFontFace = new PropertyValueParsingMethod(HtmlSupport.ParseFontFace);
      public static readonly PropertyValueParsingMethod ParseFontSize = new PropertyValueParsingMethod(HtmlSupport.ParseFontSize);
      public static readonly PropertyValueParsingMethod ParseFontStyle = new PropertyValueParsingMethod(HtmlSupport.ParseFontStyle);
      public static readonly PropertyValueParsingMethod ParseFontVariant = new PropertyValueParsingMethod(HtmlSupport.ParseFontVariant);
      public static readonly PropertyValueParsingMethod ParseFontWeight = new PropertyValueParsingMethod(HtmlSupport.ParseFontWeight);
      public static readonly PropertyValueParsingMethod ParseHorizontalAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseHorizontalAlignment);
      public static readonly PropertyValueParsingMethod ParseLanguage = new PropertyValueParsingMethod(HtmlSupport.ParseLanguage);
      public static readonly PropertyValueParsingMethod ParseLength = new PropertyValueParsingMethod(HtmlSupport.ParseLength);
      public static readonly PropertyValueParsingMethod ParseNonNegativeInteger = new PropertyValueParsingMethod(HtmlSupport.ParseNonNegativeInteger);
      public static readonly PropertyValueParsingMethod ParseNonNegativeLength = new PropertyValueParsingMethod(HtmlSupport.ParseNonNegativeLength);
      public static readonly PropertyValueParsingMethod ParseStringProperty = new PropertyValueParsingMethod(HtmlSupport.ParseStringProperty);
      public static readonly PropertyValueParsingMethod ParseTableFrame = new PropertyValueParsingMethod(HtmlSupport.ParseTableFrame);
      public static readonly PropertyValueParsingMethod ParseTableLayout = new PropertyValueParsingMethod(HtmlSupport.ParseTableLayout);
      public static readonly PropertyValueParsingMethod ParseTableRules = new PropertyValueParsingMethod(HtmlSupport.ParseTableRules);
      public static readonly PropertyValueParsingMethod ParseTarget = new PropertyValueParsingMethod(HtmlSupport.ParseTarget);
      public static readonly PropertyValueParsingMethod ParseTextAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseTextAlignment);
      public static readonly PropertyValueParsingMethod ParseUnicodeBiDi = new PropertyValueParsingMethod(HtmlSupport.ParseUnicodeBiDi);
      public static readonly PropertyValueParsingMethod ParseUrl = new PropertyValueParsingMethod(HtmlSupport.ParseUrl);
      public static readonly PropertyValueParsingMethod ParseVerticalAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseVerticalAlignment);
      public static readonly PropertyValueParsingMethod ParseVisibility = new PropertyValueParsingMethod(HtmlSupport.ParseVisibility);
    }

    public static class MultiPropertyParsingMethods
    {
      public static readonly MultiPropertyParsingMethod ParseCompositeAllBorders = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeAllBorders);
      public static readonly MultiPropertyParsingMethod ParseCompositeBackground = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBackground);
      public static readonly MultiPropertyParsingMethod ParseCompositeBorder = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBorder);
      public static readonly MultiPropertyParsingMethod ParseCompositeBorderStyle = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBorderStyle);
      public static readonly MultiPropertyParsingMethod ParseCompositeBorderWidth = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBorderWidth);
      public static readonly MultiPropertyParsingMethod ParseCompositeColor = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeColor);
      public static readonly MultiPropertyParsingMethod ParseCompositeFont = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeFont);
      public static readonly MultiPropertyParsingMethod ParseCompositeLength = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeLength);
      public static readonly MultiPropertyParsingMethod ParseCompositeNonNegativeLength = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeNonNegativeLength);
      public static readonly MultiPropertyParsingMethod ParseCompoundBorderSpacing = new MultiPropertyParsingMethod(HtmlSupport.ParseCompoundBorderSpacing);
      public static readonly MultiPropertyParsingMethod ParseCssTextDecoration = new MultiPropertyParsingMethod(HtmlSupport.ParseCssTextDecoration);
      public static readonly MultiPropertyParsingMethod ParseCssTextTransform = new MultiPropertyParsingMethod(HtmlSupport.ParseCssTextTransform);
      public static readonly MultiPropertyParsingMethod ParseCssVerticalAlignment = new MultiPropertyParsingMethod(HtmlSupport.ParseCssVerticalAlignment);
      public static readonly MultiPropertyParsingMethod ParseCssWhiteSpace = new MultiPropertyParsingMethod(HtmlSupport.ParseCssWhiteSpace);
    }
  }
}
