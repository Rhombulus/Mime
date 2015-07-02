// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlAttribute
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal struct HtmlAttribute
  {
    private HtmlToken token;

    public bool IsNull => this.token == null;

      public int Index => this.token.CurrentAttribute;

      public HtmlToken.AttrPartMajor MajorPart => this.token.AttributeList[this.token.CurrentAttribute].MajorPart;

      public HtmlToken.AttrPartMinor MinorPart => this.token.AttributeList[this.token.CurrentAttribute].MinorPart;

      public bool IsCompleteAttr => this.token.AttributeList[this.token.CurrentAttribute].IsCompleteAttr;

      public bool IsAttrBegin => this.token.AttributeList[this.token.CurrentAttribute].IsAttrBegin;

      public bool IsAttrEmptyName => this.token.AttributeList[this.token.CurrentAttribute].IsAttrEmptyName;

      public bool IsAttrEnd => this.token.AttributeList[this.token.CurrentAttribute].IsAttrEnd;

      public bool IsAttrNameEnd => this.token.AttributeList[this.token.CurrentAttribute].IsAttrNameEnd;

      public bool IsDeleted => this.token.AttributeList[this.token.CurrentAttribute].IsAttrDeleted;

      public bool IsAttrValueBegin => this.token.AttributeList[this.token.CurrentAttribute].IsAttrValueBegin;

      public bool IsAttrValueQuoted => this.token.AttributeList[this.token.CurrentAttribute].IsAttrValueQuoted;

      public HtmlNameIndex NameIndex => this.token.AttributeList[this.token.CurrentAttribute].NameIndex;

      public char QuoteChar => (char) this.token.AttributeList[this.token.CurrentAttribute].QuoteChar;

      public bool AttributeValueContainsDangerousCharacter => (int) this.token.AttributeList[this.token.CurrentAttribute].DangerousCharacters != 0;

      public bool AttributeValueContainsBackquote => ((int) this.token.AttributeList[this.token.CurrentAttribute].DangerousCharacters & 1) != 0;

      public bool AttributeValueContainsBackslash => ((int) this.token.AttributeList[this.token.CurrentAttribute].DangerousCharacters & 2) != 0;

      public bool HasNameFragment => !this.token.IsFragmentEmpty(this.token.AttributeList[this.token.CurrentAttribute].Name);

      public HtmlToken.AttributeNameTextReader Name => new HtmlToken.AttributeNameTextReader(this.token);

      public bool HasValueFragment => !this.token.IsFragmentEmpty(this.token.AttributeList[this.token.CurrentAttribute].Value);

      public HtmlToken.AttributeValueTextReader Value => new HtmlToken.AttributeValueTextReader(this.token);

      internal HtmlAttribute(HtmlToken token)
    {
      this.token = token;
    }

    public void SetMinorPart(HtmlToken.AttrPartMinor newMinorPart)
    {
      this.token.AttributeList[this.token.CurrentAttribute].MinorPart = newMinorPart;
    }

    [Conditional("DEBUG")]
    private void AssertCurrent()
    {
    }
  }
}
