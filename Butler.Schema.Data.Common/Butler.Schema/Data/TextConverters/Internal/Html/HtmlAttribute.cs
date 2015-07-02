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

    public bool IsNull
    {
      get
      {
        return this.token == null;
      }
    }

    public int Index
    {
      get
      {
        return this.token.CurrentAttribute;
      }
    }

    public HtmlToken.AttrPartMajor MajorPart
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].MajorPart;
      }
    }

    public HtmlToken.AttrPartMinor MinorPart
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].MinorPart;
      }
    }

    public bool IsCompleteAttr
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsCompleteAttr;
      }
    }

    public bool IsAttrBegin
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsAttrBegin;
      }
    }

    public bool IsAttrEmptyName
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsAttrEmptyName;
      }
    }

    public bool IsAttrEnd
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsAttrEnd;
      }
    }

    public bool IsAttrNameEnd
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsAttrNameEnd;
      }
    }

    public bool IsDeleted
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsAttrDeleted;
      }
    }

    public bool IsAttrValueBegin
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsAttrValueBegin;
      }
    }

    public bool IsAttrValueQuoted
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].IsAttrValueQuoted;
      }
    }

    public HtmlNameIndex NameIndex
    {
      get
      {
        return this.token.AttributeList[this.token.CurrentAttribute].NameIndex;
      }
    }

    public char QuoteChar
    {
      get
      {
        return (char) this.token.AttributeList[this.token.CurrentAttribute].QuoteChar;
      }
    }

    public bool AttributeValueContainsDangerousCharacter
    {
      get
      {
        return (int) this.token.AttributeList[this.token.CurrentAttribute].DangerousCharacters != 0;
      }
    }

    public bool AttributeValueContainsBackquote
    {
      get
      {
        return ((int) this.token.AttributeList[this.token.CurrentAttribute].DangerousCharacters & 1) != 0;
      }
    }

    public bool AttributeValueContainsBackslash
    {
      get
      {
        return ((int) this.token.AttributeList[this.token.CurrentAttribute].DangerousCharacters & 2) != 0;
      }
    }

    public bool HasNameFragment
    {
      get
      {
        return !this.token.IsFragmentEmpty(this.token.AttributeList[this.token.CurrentAttribute].Name);
      }
    }

    public HtmlToken.AttributeNameTextReader Name
    {
      get
      {
        return new HtmlToken.AttributeNameTextReader(this.token);
      }
    }

    public bool HasValueFragment
    {
      get
      {
        return !this.token.IsFragmentEmpty(this.token.AttributeList[this.token.CurrentAttribute].Value);
      }
    }

    public HtmlToken.AttributeValueTextReader Value
    {
      get
      {
        return new HtmlToken.AttributeValueTextReader(this.token);
      }
    }

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
