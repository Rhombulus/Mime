// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Css.CssSelector
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Css
{
  internal struct CssSelector
  {
    private CssToken token;

    public int Index => this.token.CurrentSelector;

      public bool IsDeleted => this.token.SelectorList[this.token.CurrentSelector].IsSelectorDeleted;

      public Html.HtmlNameIndex NameId => this.token.SelectorList[this.token.CurrentSelector].NameId;

      public bool HasNameFragment => !this.token.SelectorList[this.token.CurrentSelector].Name.IsEmpty;

      public CssToken.SelectorNameTextReader Name => new CssToken.SelectorNameTextReader(this.token);

      public bool HasClassFragment => !this.token.SelectorList[this.token.CurrentSelector].ClassName.IsEmpty;

      public CssToken.SelectorClassTextReader ClassName => new CssToken.SelectorClassTextReader(this.token);

      public CssSelectorClassType ClassType => this.token.SelectorList[this.token.CurrentSelector].ClassType;

      public bool IsSimple
    {
      get
      {
        if (this.token.SelectorList[this.token.CurrentSelector].Combinator != CssSelectorCombinator.None)
          return false;
        if (this.token.SelectorTail != this.token.CurrentSelector + 1)
          return this.token.SelectorList[this.token.CurrentSelector + 1].Combinator == CssSelectorCombinator.None;
        return true;
      }
    }

    public CssSelectorCombinator Combinator => this.token.SelectorList[this.token.CurrentSelector].Combinator;

      internal CssSelector(CssToken token)
    {
      this.token = token;
    }

    [Conditional("DEBUG")]
    private void AssertCurrent()
    {
    }
  }
}
