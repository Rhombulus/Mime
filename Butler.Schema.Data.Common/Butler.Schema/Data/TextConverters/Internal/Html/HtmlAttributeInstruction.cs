// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlAttributeInstruction
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal struct HtmlAttributeInstruction
  {

      public HtmlNameIndex AttributeNameId { get; }

      public Format.PropertyId PropertyId { get; }

      public PropertyValueParsingMethod ParsingMethod { get; }

      public HtmlAttributeInstruction(HtmlNameIndex attributeNameId, Format.PropertyId propertyId, PropertyValueParsingMethod parsingMethod)
    {
      this.AttributeNameId = attributeNameId;
      this.PropertyId = propertyId;
      this.ParsingMethod = parsingMethod;
    }
  }
}
