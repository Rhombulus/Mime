// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlLexicalUnit
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal enum HtmlLexicalUnit : uint
  {
    Invalid = 0U,
    Text = 67108864U,
    TagPrefix = 134217728U,
    TagSuffix = 201326592U,
    Name = 268435456U,
    TagWhitespace = 335544320U,
    AttrEqual = 402653184U,
    AttrQuote = 469762048U,
    AttrValue = 536870912U,
    TagText = 603979776U,
  }
}
