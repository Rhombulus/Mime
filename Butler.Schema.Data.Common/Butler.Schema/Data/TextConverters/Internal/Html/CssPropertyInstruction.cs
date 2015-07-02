// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.CssPropertyInstruction
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal struct CssPropertyInstruction
  {
    private Format.PropertyId propertyId;
    private PropertyValueParsingMethod parsingMethod;
    private MultiPropertyParsingMethod multiPropertyParsingMethod;

    public Format.PropertyId PropertyId => this.propertyId;

      public PropertyValueParsingMethod ParsingMethod => this.parsingMethod;

      public MultiPropertyParsingMethod MultiPropertyParsingMethod => this.multiPropertyParsingMethod;

      public CssPropertyInstruction(Format.PropertyId propertyId, PropertyValueParsingMethod parsingMethod, MultiPropertyParsingMethod multiPropertyParsingMethod)
    {
      this.propertyId = propertyId;
      this.parsingMethod = parsingMethod;
      this.multiPropertyParsingMethod = multiPropertyParsingMethod;
    }
  }
}
