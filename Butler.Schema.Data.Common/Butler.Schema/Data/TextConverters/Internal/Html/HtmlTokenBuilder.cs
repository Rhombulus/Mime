// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlTokenBuilder
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlTokenBuilder : TokenBuilder
  {
    protected const byte BuildStateEndedHtml = (byte) 6;
    protected const byte BuildStateTagStarted = (byte) 20;
    protected const byte BuildStateTagText = (byte) 21;
    protected const byte BuildStateTagName = (byte) 22;
    protected const byte BuildStateTagBeforeAttr = (byte) 23;
    protected const byte BuildStateTagAttrName = (byte) 24;
    protected const byte BuildStateTagEndAttrName = (byte) 25;
    protected const byte BuildStateTagAttrValue = (byte) 26;
    protected const byte BuildStateTagEndAttrValue = (byte) 27;
    protected HtmlToken htmlToken;
    protected int maxAttrs;
    protected int numCarryOverRuns;
    protected int carryOverRunsHeadOffset;
    protected int carryOverRunsLength;

    public HtmlToken Token
    {
      get
      {
        return this.htmlToken;
      }
    }

    public bool IncompleteTag
    {
      get
      {
        if ((int) this.state >= 10)
          return (int) this.state != 10;
        return false;
      }
    }

    public HtmlTokenBuilder(char[] buffer, int maxRuns, int maxAttrs, bool testBoundaryConditions)
      : base((Token) new HtmlToken(), buffer, maxRuns, testBoundaryConditions)
    {
      this.htmlToken = (HtmlToken) base.Token;
      int length = 8;
      if (maxAttrs != 0)
      {
        if (!testBoundaryConditions)
        {
          this.maxAttrs = maxAttrs;
        }
        else
        {
          length = 1;
          this.maxAttrs = 5;
        }
        this.htmlToken.AttributeList = new HtmlToken.AttributeEntry[length];
      }
      this.htmlToken.NameIndex = HtmlNameIndex._NOTANAME;
    }

    public static HtmlNameIndex LookupName(char[] nameBuffer, int nameOffset, int nameLength)
    {
      if (nameLength != 0 && nameLength <= 14)
      {
        short num = (short) ((long) (uint) (HashCode.CalculateLowerCase(nameBuffer, nameOffset, nameLength) ^ 221) % 601L);
        int index1 = (int) HtmlNameData.nameHashTable[(int) num];
        if (index1 > 0)
        {
          do
          {
            string str = HtmlNameData.Names[index1].Name;
            if (str.Length == nameLength && (int) str[0] == (int) ParseSupport.ToLowerCase(nameBuffer[nameOffset]))
            {
              int index2 = 0;
              do
                ;
              while (++index2 < str.Length && (int) ParseSupport.ToLowerCase(nameBuffer[nameOffset + index2]) == (int) str[index2]);
              if (index2 == str.Length)
                return (HtmlNameIndex) index1;
            }
            ++index1;
          }
          while ((int) HtmlNameData.Names[index1].Hash == (int) num);
        }
      }
      return HtmlNameIndex.Unknown;
    }

    public override void Reset()
    {
      if ((int) this.state >= 6)
      {
        this.htmlToken.Reset();
        this.numCarryOverRuns = 0;
      }
      base.Reset();
    }

    public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId)
    {
      return (HtmlTokenId) this.MakeEmptyToken((TokenId) tokenId);
    }

    public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId, int argument)
    {
      return (HtmlTokenId) this.MakeEmptyToken((TokenId) tokenId, argument);
    }

    public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId, Encoding tokenEncoding)
    {
      return (HtmlTokenId) this.MakeEmptyToken((TokenId) tokenId, tokenEncoding);
    }

    public void StartTag(HtmlNameIndex nameIndex, int baseOffset)
    {
      this.state = (byte) 20;
      this.htmlToken.HtmlTokenId = HtmlTokenId.Tag;
      this.htmlToken.PartMajor = HtmlToken.TagPartMajor.Begin;
      this.htmlToken.PartMinor = HtmlToken.TagPartMinor.Empty;
      this.htmlToken.NameIndex = nameIndex;
      this.htmlToken.TagIndex = HtmlNameData.Names[(int) nameIndex].TagIndex;
      this.htmlToken.Whole.HeadOffset = baseOffset;
      this.tailOffset = baseOffset;
    }

    public void AbortConditional(bool comment)
    {
      this.htmlToken.NameIndex = comment ? HtmlNameIndex._COMMENT : HtmlNameIndex._BANG;
    }

    public void SetEndTag()
    {
      this.htmlToken.Flags |= HtmlToken.TagFlags.EndTag;
    }

    public void SetEmptyScope()
    {
      this.htmlToken.Flags |= HtmlToken.TagFlags.EmptyScope;
    }

    public void StartTagText()
    {
      this.state = (byte) 21;
      this.htmlToken.Unstructured.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
      this.htmlToken.UnstructuredPosition.Rewind(this.htmlToken.Unstructured);
    }

    public void EndTagText()
    {
      if (this.htmlToken.Unstructured.Head == this.htmlToken.Whole.Tail)
        this.AddNullRun(HtmlRunKind.TagText);
      this.state = (byte) 20;
    }

    public void StartTagName()
    {
      this.state = (byte) 22;
      this.htmlToken.PartMinor |= HtmlToken.TagPartMinor.BeginName;
      this.htmlToken.NameInternal.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
      this.htmlToken.LocalName.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
      this.htmlToken.NamePosition.Rewind(this.htmlToken.NameInternal);
    }

    public void EndTagNamePrefix()
    {
      this.htmlToken.LocalName.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
    }

    public void EndTagName(int nameLength)
    {
      if (this.htmlToken.LocalName.Head == this.htmlToken.Whole.Tail)
      {
        this.AddNullRun(HtmlRunKind.Name);
        if (this.htmlToken.LocalName.Head == this.htmlToken.NameInternal.Head)
          this.htmlToken.Flags |= HtmlToken.TagFlags.EmptyTagName;
      }
      this.htmlToken.PartMinor |= HtmlToken.TagPartMinor.EndName;
      if (this.htmlToken.IsTagBegin)
      {
        this.AddSentinelRun();
        this.htmlToken.NameIndex = this.LookupName(nameLength, this.htmlToken.NameInternal);
        this.htmlToken.TagIndex = this.htmlToken.OriginalTagIndex = HtmlNameData.Names[(int) this.htmlToken.NameIndex].TagIndex;
      }
      this.state = (byte) 23;
    }

    public void EndTagName(HtmlNameIndex resolvedNameIndex)
    {
      if (this.htmlToken.LocalName.Head == this.htmlToken.Whole.Tail)
      {
        this.AddNullRun(HtmlRunKind.Name);
        if (this.htmlToken.LocalName.Head == this.htmlToken.NameInternal.Head)
          this.htmlToken.Flags |= HtmlToken.TagFlags.EmptyTagName;
      }
      this.htmlToken.PartMinor |= HtmlToken.TagPartMinor.EndName;
      if (this.htmlToken.IsTagBegin)
      {
        this.htmlToken.NameIndex = resolvedNameIndex;
        this.htmlToken.TagIndex = this.htmlToken.OriginalTagIndex = HtmlNameData.Names[(int) resolvedNameIndex].TagIndex;
      }
      this.state = (byte) 23;
    }

    public bool CanAddAttribute()
    {
      return this.htmlToken.AttributeTail < this.maxAttrs;
    }

    public void StartAttribute()
    {
      if (this.htmlToken.AttributeTail == this.htmlToken.AttributeList.Length)
      {
        HtmlToken.AttributeEntry[] attributeEntryArray = new HtmlToken.AttributeEntry[this.maxAttrs / 2 <= this.htmlToken.AttributeList.Length ? this.maxAttrs : this.htmlToken.AttributeList.Length * 2];
        Array.Copy((Array) this.htmlToken.AttributeList, 0, (Array) attributeEntryArray, 0, this.htmlToken.AttributeTail);
        this.htmlToken.AttributeList = attributeEntryArray;
      }
      if (this.htmlToken.PartMinor == HtmlToken.TagPartMinor.Empty)
        this.htmlToken.PartMinor = HtmlToken.TagPartMinor.BeginAttribute;
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].NameIndex = HtmlNameIndex.Unknown;
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMajor = HtmlToken.AttrPartMajor.Begin;
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMinor = HtmlToken.AttrPartMinor.BeginName;
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].QuoteChar = (byte) 0;
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].LocalName.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Value.Reset();
      this.state = (byte) 24;
    }

    public void EndAttributeNamePrefix()
    {
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].LocalName.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
    }

    public void EndAttributeName(int nameLength)
    {
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMinor |= HtmlToken.AttrPartMinor.EndName;
      if (this.htmlToken.AttributeList[this.htmlToken.AttributeTail].LocalName.Head == this.htmlToken.Whole.Tail)
      {
        this.AddNullRun(HtmlRunKind.Name);
        if (this.htmlToken.AttributeList[this.htmlToken.AttributeTail].LocalName.Head == this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name.Head)
          this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMajor |= HtmlToken.AttrPartMajor.EmptyName;
      }
      if (this.htmlToken.AttributeList[this.htmlToken.AttributeTail].IsAttrBegin)
      {
        this.AddSentinelRun();
        this.htmlToken.AttributeList[this.htmlToken.AttributeTail].NameIndex = this.LookupName(nameLength, this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name);
      }
      this.state = (byte) 25;
    }

    public void StartValue()
    {
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Value.Initialize(this.htmlToken.Whole.Tail, this.tailOffset);
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMinor |= HtmlToken.AttrPartMinor.BeginValue;
      this.state = (byte) 26;
    }

    public void SetValueQuote(char ch)
    {
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].IsAttrValueQuoted = true;
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].QuoteChar = (byte) ch;
    }

    public void SetBackquote()
    {
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].DangerousCharacters |= (byte) 1;
    }

    public void SetBackslash()
    {
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].DangerousCharacters |= (byte) 2;
    }

    public void EndValue()
    {
      if (this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Value.Head == this.htmlToken.Whole.Tail)
        this.AddNullRun(HtmlRunKind.AttrValue);
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMinor |= HtmlToken.AttrPartMinor.EndValue;
      this.state = (byte) 27;
    }

    public void EndAttribute()
    {
      this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMajor |= HtmlToken.AttrPartMajor.End;
      ++this.htmlToken.AttributeTail;
      if (this.htmlToken.AttributeTail < this.htmlToken.AttributeList.Length)
      {
        this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMajor = HtmlToken.AttrPartMajor.None;
        this.htmlToken.AttributeList[this.htmlToken.AttributeTail].PartMinor = HtmlToken.AttrPartMinor.Empty;
      }
      if (this.htmlToken.PartMinor == HtmlToken.TagPartMinor.BeginAttribute)
        this.htmlToken.PartMinor = HtmlToken.TagPartMinor.Attributes;
      else if (this.htmlToken.PartMinor == HtmlToken.TagPartMinor.ContinueAttribute)
        this.htmlToken.PartMinor = HtmlToken.TagPartMinor.EndAttribute;
      else
        this.htmlToken.PartMinor |= HtmlToken.TagPartMinor.Attributes;
      this.state = (byte) 23;
    }

    public void EndTag(bool complete)
    {
      if (complete)
      {
        if ((int) this.state != 23)
        {
          if ((int) this.state == 21)
            this.EndTagText();
          else if ((int) this.state == 22)
          {
            this.EndTagName(0);
          }
          else
          {
            if ((int) this.state == 24)
              this.EndAttributeName(0);
            else if ((int) this.state == 26)
              this.EndValue();
            if ((int) this.state == 25 || (int) this.state == 27)
              this.EndAttribute();
          }
        }
        this.AddSentinelRun();
        this.state = (byte) 6;
        this.htmlToken.PartMajor |= HtmlToken.TagPartMajor.End;
      }
      else if ((int) this.state >= 24)
      {
        if (this.htmlToken.AttributeTail != 0 || this.htmlToken.NameInternal.Head != -1 || this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name.Head > 0)
        {
          this.AddSentinelRun();
          this.numCarryOverRuns = this.htmlToken.Whole.Tail - this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name.Head;
          this.carryOverRunsHeadOffset = this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name.HeadOffset;
          this.carryOverRunsLength = this.tailOffset - this.carryOverRunsHeadOffset;
          this.htmlToken.Whole.Tail -= this.numCarryOverRuns;
        }
        else
        {
          if ((int) this.state == 24)
          {
            if (this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name.Head == this.htmlToken.Whole.Tail)
              this.AddNullRun(HtmlRunKind.Name);
          }
          else if ((int) this.state == 26 && this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Value.Head == this.htmlToken.Whole.Tail)
            this.AddNullRun(HtmlRunKind.AttrValue);
          this.AddSentinelRun();
          ++this.htmlToken.AttributeTail;
        }
      }
      else
      {
        if ((int) this.state == 22)
        {
          if (this.htmlToken.NameInternal.Head == this.htmlToken.Whole.Tail)
            this.AddNullRun(HtmlRunKind.Name);
        }
        else if ((int) this.state == 21 && this.htmlToken.Unstructured.Head == this.htmlToken.Whole.Tail)
          this.AddNullRun(HtmlRunKind.TagText);
        this.AddSentinelRun();
      }
      this.tokenValid = true;
    }

    public int RewindTag()
    {
      if ((int) this.state >= 24)
      {
        if (this.htmlToken.AttributeTail == 0 || this.htmlToken.AttributeList[this.htmlToken.AttributeTail - 1].IsAttrEnd)
        {
          int sourceIndex = this.htmlToken.Whole.Tail;
          Array.Copy((Array) this.htmlToken.RunList, sourceIndex, (Array) this.htmlToken.RunList, 0, this.numCarryOverRuns);
          this.htmlToken.Whole.Head = 0;
          this.htmlToken.Whole.HeadOffset = this.carryOverRunsHeadOffset;
          this.htmlToken.Whole.Tail = this.numCarryOverRuns;
          this.numCarryOverRuns = 0;
          this.htmlToken.AttributeList[0] = this.htmlToken.AttributeList[this.htmlToken.AttributeTail];
          this.htmlToken.PartMinor = (HtmlToken.TagPartMinor) this.htmlToken.AttributeList[0].MajorPart;
          if (this.htmlToken.AttributeList[0].Name.Head != -1)
            this.htmlToken.AttributeList[0].Name.Head -= sourceIndex;
          if (this.htmlToken.AttributeList[0].LocalName.Head != -1)
            this.htmlToken.AttributeList[0].LocalName.Head -= sourceIndex;
          if (this.htmlToken.AttributeList[0].Value.Head != -1)
            this.htmlToken.AttributeList[0].Value.Head -= sourceIndex;
        }
        else
        {
          this.htmlToken.Whole.Initialize(0, this.tailOffset);
          this.htmlToken.AttributeList[0].NameIndex = this.htmlToken.AttributeList[this.htmlToken.AttributeTail - 1].NameIndex;
          this.htmlToken.AttributeList[0].PartMajor = HtmlToken.AttrPartMajor.Continue;
          switch (this.htmlToken.AttributeList[this.htmlToken.AttributeTail - 1].PartMinor)
          {
            case HtmlToken.AttrPartMinor.BeginName:
            case HtmlToken.AttrPartMinor.ContinueName:
              this.htmlToken.AttributeList[0].PartMinor = HtmlToken.AttrPartMinor.ContinueName;
              break;
            case HtmlToken.AttrPartMinor.EndNameWithBeginValue:
            case HtmlToken.AttrPartMinor.CompleteNameWithBeginValue:
            case HtmlToken.AttrPartMinor.BeginValue:
            case HtmlToken.AttrPartMinor.ContinueValue:
              this.htmlToken.AttributeList[0].PartMinor = HtmlToken.AttrPartMinor.ContinueValue;
              break;
            default:
              this.htmlToken.AttributeList[0].PartMinor = HtmlToken.AttrPartMinor.Empty;
              break;
          }
          this.htmlToken.AttributeList[0].IsAttrDeleted = false;
          this.htmlToken.AttributeList[0].IsAttrValueQuoted = this.htmlToken.AttributeList[this.htmlToken.AttributeTail - 1].IsAttrValueQuoted;
          this.htmlToken.AttributeList[0].QuoteChar = this.htmlToken.AttributeList[this.htmlToken.AttributeTail - 1].QuoteChar;
          if ((int) this.state == 24)
          {
            this.htmlToken.AttributeList[0].Name.Initialize(0, this.tailOffset);
            this.htmlToken.AttributeList[0].LocalName.Initialize(0, this.tailOffset);
          }
          else
          {
            this.htmlToken.AttributeList[0].Name.Reset();
            this.htmlToken.AttributeList[0].LocalName.Reset();
          }
          if ((int) this.state == 26)
            this.htmlToken.AttributeList[0].Value.Initialize(0, this.tailOffset);
          else
            this.htmlToken.AttributeList[0].Value.Reset();
          this.htmlToken.PartMinor = (HtmlToken.TagPartMinor) this.htmlToken.AttributeList[0].MajorPart;
        }
      }
      else
      {
        this.htmlToken.Whole.Initialize(0, this.tailOffset);
        this.htmlToken.PartMinor = this.htmlToken.PartMinor == HtmlToken.TagPartMinor.BeginName || this.htmlToken.PartMinor == HtmlToken.TagPartMinor.ContinueName ? HtmlToken.TagPartMinor.ContinueName : HtmlToken.TagPartMinor.Empty;
        if (this.htmlToken.AttributeList != null)
        {
          this.htmlToken.AttributeList[0].PartMajor = HtmlToken.AttrPartMajor.None;
          this.htmlToken.AttributeList[0].PartMinor = HtmlToken.AttrPartMinor.Empty;
        }
      }
      if ((int) this.state == 21)
        this.htmlToken.Unstructured.Initialize(0, this.tailOffset);
      else
        this.htmlToken.Unstructured.Reset();
      if ((int) this.state == 22)
      {
        this.htmlToken.NameInternal.Initialize(0, this.tailOffset);
        this.htmlToken.LocalName.Initialize(0, this.tailOffset);
      }
      else
      {
        this.htmlToken.NameInternal.Reset();
        this.htmlToken.LocalName.Reset();
      }
      this.htmlToken.AttributeTail = 0;
      this.htmlToken.CurrentAttribute = -1;
      this.htmlToken.PartMajor = HtmlToken.TagPartMajor.Continue;
      this.tokenValid = false;
      return this.htmlToken.Whole.HeadOffset;
    }

    public HtmlNameIndex LookupName(int nameLength, Token.LexicalUnit unit)
    {
      if (nameLength != 0 && nameLength <= 14)
      {
        short num = (short) ((long) (uint) (this.token.CalculateHashLowerCase(unit) ^ 221) % 601L);
        int index = (int) HtmlNameData.nameHashTable[(int) num];
        if (index > 0)
        {
          do
          {
            string str = HtmlNameData.Names[index].Name;
            if (str.Length == nameLength)
            {
              if (this.token.IsContiguous(unit))
              {
                if ((int) str[0] == (int) ParseSupport.ToLowerCase(this.token.Buffer[unit.HeadOffset]) && (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(unit.HeadOffset + 1, str, 1)))
                  return (HtmlNameIndex) index;
              }
              else if (this.token.CaseInsensitiveCompareEqual(unit, str))
                return (HtmlNameIndex) index;
            }
            ++index;
          }
          while ((int) HtmlNameData.Names[index].Hash == (int) num);
        }
      }
      return HtmlNameIndex.Unknown;
    }

    public bool PrepareToAddMoreRuns(int numRuns, int start, HtmlRunKind skippedRunKind)
    {
      return this.PrepareToAddMoreRuns(numRuns, start, (uint) skippedRunKind);
    }

    public void AddInvalidRun(int end, HtmlRunKind kind)
    {
      this.AddInvalidRun(end, (uint) kind);
    }

    public void AddNullRun(HtmlRunKind kind)
    {
      this.AddNullRun((uint) kind);
    }

    public void AddRun(RunTextType textType, HtmlRunKind kind, int start, int end)
    {
      this.AddRun(RunType.Normal, textType, (uint) kind, start, end, 0);
    }

    public void AddLiteralRun(RunTextType textType, HtmlRunKind kind, int start, int end, int literal)
    {
      this.AddRun(RunType.Literal, textType, (uint) kind, start, end, literal);
    }

    protected override void Rebase(int deltaOffset)
    {
      this.htmlToken.Unstructured.HeadOffset += deltaOffset;
      this.htmlToken.UnstructuredPosition.RunOffset += deltaOffset;
      this.htmlToken.NameInternal.HeadOffset += deltaOffset;
      this.htmlToken.LocalName.HeadOffset += deltaOffset;
      this.htmlToken.NamePosition.RunOffset += deltaOffset;
      for (int index = 0; index < this.htmlToken.AttributeTail; ++index)
      {
        this.htmlToken.AttributeList[index].Name.HeadOffset += deltaOffset;
        this.htmlToken.AttributeList[index].LocalName.HeadOffset += deltaOffset;
        this.htmlToken.AttributeList[index].Value.HeadOffset += deltaOffset;
      }
      if ((int) this.state >= 24)
      {
        this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Name.HeadOffset += deltaOffset;
        this.htmlToken.AttributeList[this.htmlToken.AttributeTail].LocalName.HeadOffset += deltaOffset;
        this.htmlToken.AttributeList[this.htmlToken.AttributeTail].Value.HeadOffset += deltaOffset;
      }
      this.htmlToken.AttrNamePosition.RunOffset += deltaOffset;
      this.htmlToken.AttrValuePosition.RunOffset += deltaOffset;
      this.carryOverRunsHeadOffset += deltaOffset;
      base.Rebase(deltaOffset);
    }
  }
}
