// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Css.CssTokenBuilder
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Css
{
  internal class CssTokenBuilder : TokenBuilder
  {
    protected const byte BuildStateEndedCss = (byte) 6;
    protected const byte BuildStatePropertyListStarted = (byte) 20;
    protected const byte BuildStateBeforeSelector = (byte) 23;
    protected const byte BuildStateSelectorName = (byte) 24;
    protected const byte BuildStateEndSelectorName = (byte) 25;
    protected const byte BuildStateSelectorClass = (byte) 26;
    protected const byte BuildStateEndSelectorClass = (byte) 27;
    protected const byte BuildStateBeforeProperty = (byte) 43;
    protected const byte BuildStatePropertyName = (byte) 44;
    protected const byte BuildStateEndPropertyName = (byte) 45;
    protected const byte BuildStatePropertyValue = (byte) 46;
    protected const byte BuildStateEndPropertyValue = (byte) 47;
    protected CssToken cssToken;
    protected int maxProperties;
    protected int maxSelectors;

    public CssToken Token => this.cssToken;

      public bool Incomplete
    {
      get
      {
        if ((int) this.state >= 10)
          return (int) this.state != 10;
        return false;
      }
    }

    public CssTokenBuilder(char[] buffer, int maxProperties, int maxSelectors, int maxRuns, bool testBoundaryConditions)
      : base((Token) new CssToken(), buffer, maxRuns, testBoundaryConditions)
    {
      this.cssToken = (CssToken) base.Token;
      int length1 = 16;
      int length2 = 16;
      if (!testBoundaryConditions)
      {
        this.maxProperties = maxProperties;
        this.maxSelectors = maxSelectors;
      }
      else
      {
        length1 = 1;
        length2 = 1;
        this.maxProperties = 5;
        this.maxSelectors = 5;
      }
      this.cssToken.PropertyList = new CssToken.PropertyEntry[length1];
      this.cssToken.SelectorList = new CssToken.SelectorEntry[length2];
    }

    public override void Reset()
    {
      if ((int) this.state >= 6)
        this.cssToken.Reset();
      base.Reset();
    }

    public void StartRuleSet(int baseOffset, CssTokenId id)
    {
      this.state = (byte) 23;
      this.cssToken.TokenId = (TokenId) id;
      this.cssToken.Whole.HeadOffset = baseOffset;
      this.tailOffset = baseOffset;
    }

    public void EndRuleSet()
    {
      if ((int) this.state >= 43)
        this.EndDeclarations();
      this.tokenValid = true;
      this.state = (byte) 6;
      this.token.WholePosition.Rewind(this.token.Whole);
    }

    public void BuildUniversalSelector()
    {
      this.StartSelectorName();
      this.EndSelectorName(0);
    }

    public bool CanAddSelector()
    {
      return this.cssToken.SelectorTail - this.cssToken.SelectorHead < this.maxSelectors;
    }

    public void StartSelectorName()
    {
      if (this.cssToken.SelectorTail == this.cssToken.SelectorList.Length)
      {
        CssToken.SelectorEntry[] selectorEntryArray = new CssToken.SelectorEntry[this.maxSelectors / 2 <= this.cssToken.SelectorList.Length ? this.maxSelectors : this.cssToken.SelectorList.Length * 2];
        Array.Copy((Array) this.cssToken.SelectorList, 0, (Array) selectorEntryArray, 0, this.cssToken.SelectorTail);
        this.cssToken.SelectorList = selectorEntryArray;
      }
      this.cssToken.SelectorList[this.cssToken.SelectorTail].NameId = Html.HtmlNameIndex.Unknown;
      this.cssToken.SelectorList[this.cssToken.SelectorTail].Name.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
      this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassName.Reset();
      this.state = (byte) 24;
    }

    public void EndSelectorName(int nameLength)
    {
      this.cssToken.SelectorList[this.cssToken.SelectorTail].Name.Tail = this.cssToken.Whole.Tail;
      this.cssToken.SelectorList[this.cssToken.SelectorTail].NameId = this.LookupTagName(nameLength, this.cssToken.SelectorList[this.cssToken.SelectorTail].Name);
      this.state = (byte) 25;
    }

    public void StartSelectorClass(CssSelectorClassType classType)
    {
      this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassName.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
      this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassType = classType;
      this.state = (byte) 26;
    }

    public void EndSelectorClass()
    {
      this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassName.Tail = this.cssToken.Whole.Tail;
      this.state = (byte) 27;
    }

    public void SetSelectorCombinator(CssSelectorCombinator combinator, bool previous)
    {
      int index = this.cssToken.SelectorTail;
      if (previous)
        --index;
      this.cssToken.SelectorList[index].Combinator = combinator;
    }

    public void EndSimpleSelector()
    {
      ++this.cssToken.SelectorTail;
    }

    public void StartDeclarations(int baseOffset)
    {
      this.state = (byte) 43;
      if (this.cssToken.TokenId == TokenId.None)
        this.cssToken.TokenId = (TokenId) 5;
      this.cssToken.PartMajor = CssToken.PropertyListPartMajor.Begin;
      this.cssToken.PartMinor = CssToken.PropertyListPartMinor.Empty;
      this.cssToken.Whole.HeadOffset = baseOffset;
      this.tailOffset = baseOffset;
    }

    public bool CanAddProperty()
    {
      return this.cssToken.PropertyTail - this.cssToken.PropertyHead < this.maxProperties;
    }

    public void StartPropertyName()
    {
      if (this.cssToken.PropertyTail == this.cssToken.PropertyList.Length)
      {
        CssToken.PropertyEntry[] propertyEntryArray = new CssToken.PropertyEntry[this.maxProperties / 2 <= this.cssToken.PropertyList.Length ? this.maxProperties : this.cssToken.PropertyList.Length * 2];
        Array.Copy((Array) this.cssToken.PropertyList, 0, (Array) propertyEntryArray, 0, this.cssToken.PropertyTail);
        this.cssToken.PropertyList = propertyEntryArray;
      }
      if (this.cssToken.PartMinor == CssToken.PropertyListPartMinor.Empty)
        this.cssToken.PartMinor = CssToken.PropertyListPartMinor.BeginProperty;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].NameId = CssNameIndex.Unknown;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMajor = CssToken.PropertyPartMajor.Begin;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMinor = CssToken.PropertyPartMinor.BeginName;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].QuoteChar = (byte) 0;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].Name.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
      this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Reset();
      this.state = (byte) 44;
    }

    public void EndPropertyName(int nameLength)
    {
      this.cssToken.PropertyList[this.cssToken.PropertyTail].Name.Tail = this.cssToken.Whole.Tail;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMinor |= CssToken.PropertyPartMinor.EndName;
      if (this.cssToken.PropertyList[this.cssToken.PropertyTail].IsPropertyBegin)
        this.cssToken.PropertyList[this.cssToken.PropertyTail].NameId = this.LookupName(nameLength, this.cssToken.PropertyList[this.cssToken.PropertyTail].Name);
      this.state = (byte) 45;
    }

    public void StartPropertyValue()
    {
      this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
      this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMinor |= CssToken.PropertyPartMinor.BeginValue;
      this.state = (byte) 46;
    }

    public void SetPropertyValueQuote(char ch)
    {
      this.cssToken.PropertyList[this.cssToken.PropertyTail].IsPropertyValueQuoted = true;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].QuoteChar = (byte) ch;
    }

    public void EndPropertyValue()
    {
      this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Tail = this.cssToken.Whole.Tail;
      this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMinor |= CssToken.PropertyPartMinor.EndValue;
      this.state = (byte) 47;
    }

    public void EndProperty()
    {
      this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMajor |= CssToken.PropertyPartMajor.End;
      ++this.cssToken.PropertyTail;
      if (this.cssToken.PropertyTail < this.cssToken.PropertyList.Length)
      {
        this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMajor = CssToken.PropertyPartMajor.None;
        this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMinor = CssToken.PropertyPartMinor.Empty;
      }
      if (this.cssToken.PartMinor == CssToken.PropertyListPartMinor.BeginProperty)
        this.cssToken.PartMinor = CssToken.PropertyListPartMinor.Properties;
      else if (this.cssToken.PartMinor == CssToken.PropertyListPartMinor.ContinueProperty)
        this.cssToken.PartMinor = CssToken.PropertyListPartMinor.EndProperty;
      else
        this.cssToken.PartMinor |= CssToken.PropertyListPartMinor.Properties;
      this.state = (byte) 43;
    }

    public void EndDeclarations()
    {
      if ((int) this.state != 20)
      {
        if ((int) this.state == 44)
          this.cssToken.PropertyList[this.cssToken.PropertyTail].Name.Tail = this.cssToken.Whole.Tail;
        else if ((int) this.state == 46)
          this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Tail = this.cssToken.Whole.Tail;
      }
      if ((int) this.state == 44)
        this.EndPropertyName(0);
      else if ((int) this.state == 46)
        this.EndPropertyValue();
      if ((int) this.state == 45 || (int) this.state == 47)
        this.EndProperty();
      this.state = (byte) 43;
      this.cssToken.PartMajor |= CssToken.PropertyListPartMajor.End;
      this.tokenValid = true;
    }

    public bool PrepareAndAddRun(CssRunKind cssRunKind, int start, int end)
    {
      if (end != start)
      {
        if (!this.PrepareToAddMoreRuns(1))
          return false;
        this.AddRun(cssRunKind == CssRunKind.Invalid ? RunType.Invalid : RunType.Normal, cssRunKind == CssRunKind.Space ? RunTextType.Space : RunTextType.NonSpace, (uint) cssRunKind, start, end, 0);
      }
      return true;
    }

    public bool PrepareAndAddInvalidRun(CssRunKind cssRunKind, int end)
    {
      if (!this.PrepareToAddMoreRuns(1))
        return false;
      this.AddInvalidRun(end, (uint) cssRunKind);
      return true;
    }

    public bool PrepareAndAddLiteralRun(CssRunKind cssRunKind, int start, int end, int value)
    {
      if (end != start)
      {
        if (!this.PrepareToAddMoreRuns(1))
          return false;
        this.AddRun(RunType.Literal, RunTextType.NonSpace, (uint) cssRunKind, start, end, value);
      }
      return true;
    }

    public void InvalidateLastValidRun(CssRunKind kind)
    {
      int index = this.token.Whole.Tail;
      do
      {
        --index;
        Token.RunEntry runEntry = this.token.RunList[index];
        if (runEntry.Type != RunType.Invalid)
        {
          if (kind == (CssRunKind) runEntry.Kind)
          {
            this.token.RunList[index].Initialize(RunType.Invalid, runEntry.TextType, runEntry.Kind, runEntry.Length, runEntry.Value);
            return;
          }
          goto label_5;
        }
      }
      while (index > 0);
      goto label_6;
label_5:
      return;
label_6:;
    }

    public void MarkPropertyAsDeleted()
    {
      this.cssToken.PropertyList[this.cssToken.PropertyTail].IsPropertyDeleted = true;
    }

    public CssTokenId MakeEmptyToken(CssTokenId tokenId)
    {
      return (CssTokenId) this.MakeEmptyToken((TokenId) tokenId);
    }

    public CssNameIndex LookupName(int nameLength, Token.Fragment fragment)
    {
      if (nameLength > 26)
        return CssNameIndex.Unknown;
      short num = (short) ((long) (uint) (this.token.CalculateHashLowerCase(fragment) ^ 2) % 329L);
      int index = (int) CssData.nameHashTable[(int) num];
      if (index > 0)
      {
        do
        {
          string str = CssData.names[index].Name;
          if (str.Length == nameLength)
          {
            if (fragment.Tail == fragment.Head + 1)
            {
              if ((int) str[0] == (int) ParseSupport.ToLowerCase(this.token.Buffer[fragment.HeadOffset]) && (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(fragment.HeadOffset + 1, str, 1)))
                return (CssNameIndex) index;
            }
            else if (this.token.CaseInsensitiveCompareEqual(ref fragment, str))
              return (CssNameIndex) index;
          }
          ++index;
        }
        while ((int) CssData.names[index].Hash == (int) num);
      }
      return CssNameIndex.Unknown;
    }

    public Html.HtmlNameIndex LookupTagName(int nameLength, Token.Fragment fragment)
    {
      if (nameLength > 14)
        return Html.HtmlNameIndex.Unknown;
      short num = (short) ((long) (uint) (this.token.CalculateHashLowerCase(fragment) ^ 221) % 601L);
      int index = (int) Html.HtmlNameData.nameHashTable[(int) num];
      if (index > 0)
      {
        do
        {
          string str = Html.HtmlNameData.Names[index].Name;
          if (str.Length == nameLength)
          {
            if (fragment.Tail == fragment.Head + 1)
            {
              if ((int) str[0] == (int) ParseSupport.ToLowerCase(this.token.Buffer[fragment.HeadOffset]) && (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(fragment.HeadOffset + 1, str, 1)))
                return (Html.HtmlNameIndex) index;
            }
            else if (this.token.CaseInsensitiveCompareEqual(ref fragment, str))
              return (Html.HtmlNameIndex) index;
          }
          ++index;
        }
        while ((int) Html.HtmlNameData.Names[index].Hash == (int) num);
      }
      return Html.HtmlNameIndex.Unknown;
    }
  }
}
