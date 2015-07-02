// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlFormatOutputCallbackContext
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlFormatOutputCallbackContext : HtmlTagContext
  {
    private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteNameWithCompleteValue);
    private static readonly HtmlTagParts CompleteTagWithAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteNameWithAttributes);
    private static readonly HtmlTagParts CompleteTagWithoutAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteName);
    private HtmlFormatOutputCallbackContext.AttributeDescriptor[] attributes = new HtmlFormatOutputCallbackContext.AttributeDescriptor[10];
    private const int MaxCallbackAttributes = 10;
    private HtmlFormatOutput formatOutput;
    private int countAttributes;

    public HtmlFormatOutputCallbackContext(HtmlFormatOutput formatOutput)
    {
      this.formatOutput = formatOutput;
    }

    public new void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool tagDropped)
    {
      base.InitializeTag(isEndTag, tagNameIndex, tagDropped);
      this.countAttributes = 0;
    }

    public void InitializeFragment(bool isEmptyElementTag)
    {
      this.InitializeFragment(isEmptyElementTag, this.countAttributes, this.countAttributes == 0 ? HtmlFormatOutputCallbackContext.CompleteTagWithoutAttributesParts : HtmlFormatOutputCallbackContext.CompleteTagWithAttributesParts);
    }

    internal void Reset()
    {
      this.countAttributes = 0;
    }

    internal void AddAttribute(HtmlNameIndex nameIndex, string value)
    {
      this.attributes[this.countAttributes].NameIndex = nameIndex;
      this.attributes[this.countAttributes].Value = value;
      this.attributes[this.countAttributes].ReadIndex = 0;
      ++this.countAttributes;
    }

    internal override string GetTagNameImpl()
    {
      return HtmlNameData.Names[(int) this.TagNameIndex].Name;
    }

    internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
    {
      return HtmlNameData.Names[(int) this.attributes[attributeIndex].NameIndex].PublicAttributeId;
    }

    internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
    {
      return HtmlFormatOutputCallbackContext.CompleteAttributeParts;
    }

    internal override string GetAttributeNameImpl(int attributeIndex)
    {
      return HtmlNameData.Names[(int) this.attributes[attributeIndex].NameIndex].Name;
    }

    internal override string GetAttributeValueImpl(int attributeIndex)
    {
      return this.attributes[attributeIndex].Value;
    }

    internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
    {
      int count1 = Math.Min(count, this.attributes[attributeIndex].Value.Length - this.attributes[attributeIndex].ReadIndex);
      if (count1 != 0)
      {
        this.attributes[attributeIndex].Value.CopyTo(this.attributes[attributeIndex].ReadIndex, buffer, offset, count1);
        this.attributes[attributeIndex].ReadIndex += count1;
      }
      return count1;
    }

    internal override void WriteTagImpl(bool copyTagAttributes)
    {
      this.formatOutput.Writer.WriteTagBegin(this.TagNameIndex, (string) null, this.IsEndTag, false, false);
      if (!copyTagAttributes)
        return;
      for (int attributeIndex = 0; attributeIndex < this.countAttributes; ++attributeIndex)
        this.WriteAttributeImpl(attributeIndex, true, true);
    }

    internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
    {
      if (writeName)
        this.formatOutput.Writer.WriteAttributeName(this.attributes[attributeIndex].NameIndex);
      if (!writeValue)
        return;
      this.formatOutput.Writer.WriteAttributeValue(this.attributes[attributeIndex].Value);
    }

    private struct AttributeDescriptor
    {
      public HtmlNameIndex NameIndex;
      public string Value;
      public int ReadIndex;
    }
  }
}
