// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Css.CssRunKind
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Css
{
  internal enum CssRunKind : uint
  {
    Invalid = 0U,
    Text = 67108864U,
    Space = 184549376U,
    SimpleSelector = 201326592U,
    Identifier = 218103808U,
    Delimiter = 234881024U,
    AtRuleName = 251658240U,
    SelectorName = 268435456U,
    SelectorCombinatorOrComma = 285212672U,
    SelectorPseudoStart = 301989888U,
    SelectorPseudo = 318767104U,
    SelectorPseudoArg = 335544320U,
    SelectorClassStart = 352321536U,
    SelectorClass = 369098752U,
    SelectorHashStart = 385875968U,
    SelectorHash = 402653184U,
    SelectorAttribStart = 419430400U,
    SelectorAttribName = 436207616U,
    SelectorAttribEquals = 452984832U,
    SelectorAttribIncludes = 469762048U,
    SelectorAttribDashmatch = 486539264U,
    SelectorAttribIdentifier = 503316480U,
    SelectorAttribString = 520093696U,
    SelectorAttribEnd = 536870912U,
    PropertyName = 671088640U,
    PropertyColon = 687865856U,
    ImportantStart = 704643072U,
    Important = 721420288U,
    Operator = 738197504U,
    UnaryOperator = 754974720U,
    Dot = 771751936U,
    Percent = 788529152U,
    Metrics = 805306368U,
    TermIdentifier = 822083584U,
    UnicodeRange = 838860800U,
    FunctionStart = 855638016U,
    FunctionEnd = 872415232U,
    HexColorStart = 889192448U,
    HexColor = 905969664U,
    String = 922746880U,
    Numeric = 939524096U,
    PropertySemicolon = 956301312U,
    Url = 973078528U,
    PageIdent = 1174405120U,
    PagePseudoStart = 1191182336U,
    PagePseudo = 1207959552U,
  }
}
