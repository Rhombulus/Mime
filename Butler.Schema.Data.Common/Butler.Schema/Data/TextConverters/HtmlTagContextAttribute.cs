// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlTagContextAttribute
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  public struct HtmlTagContextAttribute
  {
    public static readonly HtmlTagContextAttribute Null = new HtmlTagContextAttribute();
    private HtmlTagContext tagContext;
    private int attributeIndexAndCookie;

    public bool IsNull => this.tagContext == null;

      public HtmlAttributeId Id
    {
      get
      {
        this.AssertValid();
        return this.tagContext.GetAttributeNameIdImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
      }
    }

    public string Name
    {
      get
      {
        this.AssertValid();
        return this.tagContext.GetAttributeNameImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
      }
    }

    public string Value
    {
      get
      {
        this.AssertValid();
        return this.tagContext.GetAttributeValueImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
      }
    }

    internal HtmlAttributeParts Parts
    {
      get
      {
        this.AssertValid();
        return this.tagContext.GetAttributePartsImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
      }
    }

    internal HtmlTagContextAttribute(HtmlTagContext tagContext, int attributeIndexAndCookie)
    {
      this.tagContext = tagContext;
      this.attributeIndexAndCookie = attributeIndexAndCookie;
    }

    public int ReadValue(char[] buffer, int offset, int count)
    {
      this.AssertValid();
      return this.tagContext.ReadAttributeValueImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), buffer, offset, count);
    }

    public void Write()
    {
      this.AssertValid();
      this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), true, true);
    }

    public void WriteName()
    {
      this.AssertValid();
      this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), true, false);
    }

    public void WriteValue()
    {
      this.AssertValid();
      this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), false, true);
    }

    public override string ToString()
    {
      if (this.tagContext != null)
        return HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie).ToString();
      return "null";
    }

    private void AssertValid()
    {
      if (this.tagContext == null)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.AttributeNotInitialized);
      this.tagContext.AssertAttributeValid(this.attributeIndexAndCookie);
    }
  }
}
