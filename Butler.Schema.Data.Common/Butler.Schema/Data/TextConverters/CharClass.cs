// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.CharClass
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  [Flags]
  internal enum CharClass : uint
  {
    Invalid = 0U,
    NotInterestingText = 1U,
    Control = 2U,
    Whitespace = 4U,
    Alpha = 8U,
    Numeric = 16U,
    Backslash = 32U,
    LessThan = 64U,
    Equals = 128U,
    GreaterThan = 256U,
    Solidus = 512U,
    Ampersand = 1024U,
    Nbsp = 2048U,
    Comma = 4096U,
    SingleQuote = 8192U,
    DoubleQuote = 16384U,
    GraveAccent = 32768U,
    Circumflex = 65536U,
    VerticalLine = 131072U,
    Parentheses = 262144U,
    CurlyBrackets = 524288U,
    SquareBrackets = 1048576U,
    Tilde = 2097152U,
    Colon = 4194304U,
    UniqueMask = 16777215U,
    AlphaHex = 2147483648U,
    HtmlSuffix = 1073741824U,
    RtfInteresting = 536870912U,
    OverlappedMask = 4278190080U,
    Quote = GraveAccent | DoubleQuote | SingleQuote,
    Brackets = SquareBrackets | CurlyBrackets,
    NonWhitespaceText = 16775163U,
    NonWhitespaceNonControlText = 16775161U,
    HtmlNonWhitespaceText = 16774075U,
    NonWhitespaceNonUri = Brackets | Tilde | VerticalLine | Circumflex | GraveAccent | DoubleQuote | Ampersand | GreaterThan | LessThan,
    NonWhitespaceUri = 12858043U,
    HtmlTagName = 16776443U,
    HtmlTagNamePrefix = 12582139U,
    HtmlAttrName = 16776315U,
    HtmlAttrNamePrefix = 12582011U,
    HtmlAttrValue = 16718587U,
    HtmlScanQuoteSensitive = Equals | Whitespace,
    HtmlEntity = Numeric | Alpha,
    HtmlSimpleTagName = 12524731U,
    HtmlEndTagName = Solidus | GreaterThan | Whitespace,
    HtmlSimpleAttrName = 12524667U,
    HtmlEndAttrName = HtmlEndTagName | Equals,
    HtmlSimpleAttrQuotedValue = 16718847U,
    HtmlSimpleAttrUnquotedValue = 16718587U,
    HtmlEndAttrUnquotedValue = GreaterThan | Whitespace,
    Hex = AlphaHex | Numeric,
  }
}
