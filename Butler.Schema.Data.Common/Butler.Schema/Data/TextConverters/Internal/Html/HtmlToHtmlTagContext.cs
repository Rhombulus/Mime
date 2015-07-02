// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlToHtmlTagContext
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlToHtmlTagContext : HtmlTagContext
  {
    private HtmlToHtmlConverter converter;

    public HtmlToHtmlTagContext(HtmlToHtmlConverter converter)
    {
      this.converter = converter;
    }

    internal override string GetTagNameImpl()
    {
      if (this.TagNameIndex > HtmlNameIndex.Unknown)
      {
        if (!this.TagParts.Begin)
          return string.Empty;
        return HtmlNameData.Names[(int) this.TagNameIndex].Name;
      }
      if (this.TagParts.Name)
        return this.converter.InternalToken.Name.GetString(int.MaxValue);
      return string.Empty;
    }

    internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
    {
      return this.converter.GetAttributeNameId(attributeIndex);
    }

    internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
    {
      return this.converter.GetAttributeParts(attributeIndex);
    }

    internal override string GetAttributeNameImpl(int attributeIndex)
    {
      return this.converter.GetAttributeName(attributeIndex);
    }

    internal override string GetAttributeValueImpl(int attributeIndex)
    {
      return this.converter.GetAttributeValue(attributeIndex);
    }

    internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
    {
      return this.converter.ReadAttributeValue(attributeIndex, buffer, offset, count);
    }

    internal override void WriteTagImpl(bool copyTagAttributes)
    {
      this.converter.WriteTag(copyTagAttributes);
    }

    internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
    {
      this.converter.WriteAttribute(attributeIndex, writeName, writeValue);
    }
  }
}
