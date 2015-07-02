// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlToken
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlToken : Token
  {

      protected internal HtmlTagIndex TagIndex;
    protected internal HtmlTagIndex OriginalTagIndex;
    protected internal HtmlNameIndex NameIndex;
    protected internal HtmlToken.TagPartMajor PartMajor;
    protected internal HtmlToken.TagPartMinor PartMinor;
    protected internal Token.LexicalUnit Unstructured;
    protected internal Token.FragmentPosition UnstructuredPosition;
    protected internal Token.LexicalUnit NameInternal;
    protected internal Token.LexicalUnit LocalName;
    protected internal Token.FragmentPosition NamePosition;
    protected internal HtmlToken.AttributeEntry[] AttributeList;
    protected internal int AttributeTail;
    protected internal int CurrentAttribute;
    protected internal Token.FragmentPosition AttrNamePosition;
    protected internal Token.FragmentPosition AttrValuePosition;

    public HtmlTokenId HtmlTokenId
    {
      get
      {
        return (HtmlTokenId) this.TokenId;
      }
      set
      {
        this.TokenId = (TokenId) value;
      }
    }

    public HtmlToken.TagFlags Flags { get; set; }

      public bool IsEndTag => HtmlToken.TagFlags.None != (this.Flags & HtmlToken.TagFlags.EndTag);

      public bool IsEmptyScope => HtmlToken.TagFlags.None != (this.Flags & HtmlToken.TagFlags.EmptyScope);

      public HtmlToken.TagPartMajor MajorPart => this.PartMajor;

      public HtmlToken.TagPartMinor MinorPart => this.PartMinor;

      public bool IsTagComplete => this.PartMajor == HtmlToken.TagPartMajor.Complete;

      public bool IsTagBegin => (this.PartMajor & HtmlToken.TagPartMajor.Begin) == HtmlToken.TagPartMajor.Begin;

      public bool IsTagEnd => (this.PartMajor & HtmlToken.TagPartMajor.End) == HtmlToken.TagPartMajor.End;

      public bool IsTagNameEmpty => HtmlToken.TagFlags.None != (this.Flags & HtmlToken.TagFlags.EmptyTagName);

      public bool IsTagNameBegin => (this.PartMinor & HtmlToken.TagPartMinor.BeginName) == HtmlToken.TagPartMinor.BeginName;

      public bool IsTagNameEnd => (this.PartMinor & HtmlToken.TagPartMinor.EndName) == HtmlToken.TagPartMinor.EndName;

      public bool HasNameFragment => !this.IsFragmentEmpty(this.NameInternal);

      public HtmlToken.TagNameTextReader Name => new HtmlToken.TagNameTextReader(this);

      public HtmlToken.TagUnstructuredContentTextReader UnstructuredContent => new HtmlToken.TagUnstructuredContentTextReader(this);

      public HtmlTagIndex OriginalTagId => this.OriginalTagIndex;

      public bool IsAllowWspLeft => (this.Flags & HtmlToken.TagFlags.AllowWspLeft) == HtmlToken.TagFlags.AllowWspLeft;

      public bool IsAllowWspRight => (this.Flags & HtmlToken.TagFlags.AllowWspRight) == HtmlToken.TagFlags.AllowWspRight;

      public HtmlToken.AttributeEnumerator Attributes => new HtmlToken.AttributeEnumerator(this);

      public HtmlToken()
    {
      this.Reset();
    }

    internal new void Reset()
    {
      this.TagIndex = this.OriginalTagIndex = HtmlTagIndex._NULL;
      this.NameIndex = HtmlNameIndex._NOTANAME;
      this.Flags = HtmlToken.TagFlags.None;
      this.PartMajor = HtmlToken.TagPartMajor.None;
      this.PartMinor = HtmlToken.TagPartMinor.Empty;
      this.NameInternal.Reset();
      this.Unstructured.Reset();
      this.NamePosition.Reset();
      this.UnstructuredPosition.Reset();
      this.AttributeTail = 0;
      this.CurrentAttribute = -1;
      this.AttrNamePosition.Reset();
      this.AttrValuePosition.Reset();
    }

    [System.Flags]
    public enum TagFlags : byte
    {
      None = (byte) 0,
      EmptyTagName = (byte) 8,
      EndTag = (byte) 16,
      EmptyScope = (byte) 32,
      AllowWspLeft = (byte) 64,
      AllowWspRight = (byte) 128,
    }

    public enum TagPartMajor : byte
    {
      None = (byte) 0,
      Continue = (byte) 2,
      Begin = (byte) 3,
      End = (byte) 6,
      Complete = (byte) 7,
    }

    public enum TagPartMinor : byte
    {
      Empty = (byte) 0,
      ContinueName = (byte) 2,
      BeginName = (byte) 3,
      EndName = (byte) 6,
      CompleteName = (byte) 7,
      ContinueAttribute = (byte) 16,
      BeginAttribute = (byte) 24,
      EndAttribute = (byte) 48,
      AttributePartMask = (byte) 56,
      Attributes = (byte) 128,
      EndNameWithAttributes = (byte) 134,
      CompleteNameWithAttributes = (byte) 135,
      EndAttributeWithOtherAttributes = (byte) 176,
    }

    public enum AttrPartMajor : byte
    {
      None = (byte) 0,
      EmptyName = (byte) 1,
      Continue = (byte) 16,
      Begin = (byte) 24,
      End = (byte) 48,
      Complete = (byte) 56,
      MaskOffFlags = (byte) 56,
      ValueQuoted = (byte) 64,
      Deleted = (byte) 128,
    }

    public enum AttrPartMinor : byte
    {
      Empty = (byte) 0,
      ContinueName = (byte) 2,
      BeginName = (byte) 3,
      EndName = (byte) 6,
      CompleteName = (byte) 7,
      ContinueValue = (byte) 16,
      BeginValue = (byte) 24,
      EndNameWithBeginValue = (byte) 30,
      CompleteNameWithBeginValue = (byte) 31,
      EndValue = (byte) 48,
      CompleteValue = (byte) 56,
      EndNameWithCompleteValue = (byte) 62,
      CompleteNameWithCompleteValue = (byte) 63,
    }

    public struct AttributeEnumerator
    {
      private HtmlToken token;

      public int Count => this.token.AttributeTail;

        public HtmlAttribute Current => new HtmlAttribute(this.token);

        public int CurrentIndex => this.token.CurrentAttribute;

        public HtmlAttribute this[int i]
      {
        get
        {
          if (i != this.token.CurrentAttribute)
          {
            this.token.AttrNamePosition.Rewind(this.token.AttributeList[i].Name);
            this.token.AttrValuePosition.Rewind(this.token.AttributeList[i].Value);
          }
          this.token.CurrentAttribute = i;
          return new HtmlAttribute(this.token);
        }
      }

      internal AttributeEnumerator(HtmlToken token)
      {
        this.token = token;
      }

      public bool MoveNext()
      {
        if (this.token.CurrentAttribute != this.token.AttributeTail)
        {
          ++this.token.CurrentAttribute;
          if (this.token.CurrentAttribute != this.token.AttributeTail)
          {
            this.token.AttrNamePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Name);
            this.token.AttrValuePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Value);
          }
        }
        return this.token.CurrentAttribute != this.token.AttributeTail;
      }

      public void Rewind()
      {
        this.token.CurrentAttribute = -1;
      }

      public HtmlToken.AttributeEnumerator GetEnumerator()
      {
        return this;
      }

      public bool Find(HtmlNameIndex nameIndex)
      {
        for (int index = 0; index < this.token.AttributeTail; ++index)
        {
          if (this.token.AttributeList[index].NameIndex == nameIndex)
          {
            this.token.CurrentAttribute = index;
            this.token.AttrNamePosition.Rewind(this.token.AttributeList[index].Name);
            this.token.AttrValuePosition.Rewind(this.token.AttributeList[index].Value);
            return true;
          }
        }
        return false;
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct TagUnstructuredContentTextReader
    {
      private HtmlToken token;

      internal TagUnstructuredContentTextReader(HtmlToken token)
      {
        this.token = token;
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(this.token.Unstructured, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(this.token.Unstructured, maxSize);
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct TagNameTextReader
    {
      private HtmlToken token;

      public int Length => this.token.GetLength(this.token.NameInternal);

        internal TagNameTextReader(HtmlToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(this.token.NameInternal, ref this.token.NamePosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.NamePosition.Rewind(this.token.NameInternal);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(this.token.NameInternal, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(this.token.NameInternal, maxSize);
      }

      public void MakeEmpty()
      {
        this.token.NameInternal.Reset();
        this.Rewind();
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct AttributeNameTextReader
    {
      private HtmlToken token;

      public int Length => this.token.GetLength(this.token.AttributeList[this.token.CurrentAttribute].Name);

        internal AttributeNameTextReader(HtmlToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(this.token.AttributeList[this.token.CurrentAttribute].Name, ref this.token.AttrNamePosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.AttrNamePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Name);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(this.token.AttributeList[this.token.CurrentAttribute].Name, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(this.token.AttributeList[this.token.CurrentAttribute].Name, maxSize);
      }

      public void MakeEmpty()
      {
        this.token.AttributeList[this.token.CurrentAttribute].Name.Reset();
        this.token.AttrNamePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Name);
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct AttributeValueTextReader
    {
      private HtmlToken token;

      public int Length => this.token.GetLength(this.token.AttributeList[this.token.CurrentAttribute].Value);

        public bool IsEmpty => this.token.IsFragmentEmpty(this.token.AttributeList[this.token.CurrentAttribute].Value);

        public bool IsContiguous => this.token.IsContiguous(this.token.AttributeList[this.token.CurrentAttribute].Value);

        public BufferString ContiguousBufferString => new BufferString(this.token.Buffer, this.token.AttributeList[this.token.CurrentAttribute].Value.HeadOffset, this.token.RunList[this.token.AttributeList[this.token.CurrentAttribute].Value.Head].Length);

        internal AttributeValueTextReader(HtmlToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(this.token.AttributeList[this.token.CurrentAttribute].Value, ref this.token.AttrValuePosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.AttrValuePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Value);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(this.token.AttributeList[this.token.CurrentAttribute].Value, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(this.token.AttributeList[this.token.CurrentAttribute].Value, maxSize);
      }

      public bool CaseInsensitiveCompareEqual(string str)
      {
        return this.token.CaseInsensitiveCompareEqual(this.token.AttributeList[this.token.CurrentAttribute].Value, str);
      }

      public bool CaseInsensitiveContainsSubstring(string str)
      {
        return this.token.CaseInsensitiveContainsSubstring(this.token.AttributeList[this.token.CurrentAttribute].Value, str);
      }

      public bool SkipLeadingWhitespace()
      {
        return this.token.SkipLeadingWhitespace(this.token.AttributeList[this.token.CurrentAttribute].Value, ref this.token.AttrValuePosition);
      }

      public void MakeEmpty()
      {
        this.token.AttributeList[this.token.CurrentAttribute].Value.Reset();
        this.Rewind();
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    protected internal struct AttributeEntry
    {
      public HtmlNameIndex NameIndex;
      public byte QuoteChar;
      public byte DangerousCharacters;
      public HtmlToken.AttrPartMajor PartMajor;
      public HtmlToken.AttrPartMinor PartMinor;
      public Token.LexicalUnit Name;
      public Token.LexicalUnit LocalName;
      public Token.LexicalUnit Value;

      public bool IsCompleteAttr => this.MajorPart == HtmlToken.AttrPartMajor.Complete;

        public bool IsAttrBegin => (this.PartMajor & HtmlToken.AttrPartMajor.Begin) == HtmlToken.AttrPartMajor.Begin;

        public bool IsAttrEnd => (this.PartMajor & HtmlToken.AttrPartMajor.End) == HtmlToken.AttrPartMajor.End;

        public bool IsAttrEmptyName => (this.PartMajor & HtmlToken.AttrPartMajor.EmptyName) == HtmlToken.AttrPartMajor.EmptyName;

        public bool IsAttrNameEnd => (this.PartMinor & HtmlToken.AttrPartMinor.EndName) == HtmlToken.AttrPartMinor.EndName;

        public bool IsAttrValueBegin => (this.PartMinor & HtmlToken.AttrPartMinor.BeginValue) == HtmlToken.AttrPartMinor.BeginValue;

        public HtmlToken.AttrPartMajor MajorPart => this.PartMajor & HtmlToken.AttrPartMajor.Complete;

        public HtmlToken.AttrPartMinor MinorPart
      {
        get
        {
          return this.PartMinor;
        }
        set
        {
          this.PartMinor = value;
        }
      }

      public bool IsAttrValueQuoted
      {
        get
        {
          return (this.PartMajor & HtmlToken.AttrPartMajor.ValueQuoted) == HtmlToken.AttrPartMajor.ValueQuoted;
        }
        set
        {
          this.PartMajor = value ? this.PartMajor | HtmlToken.AttrPartMajor.ValueQuoted : this.PartMajor & (HtmlToken.AttrPartMajor) 191;
        }
      }

      public bool IsAttrDeleted
      {
        get
        {
          return (this.PartMajor & HtmlToken.AttrPartMajor.Deleted) == HtmlToken.AttrPartMajor.Deleted;
        }
        set
        {
          this.PartMajor = value ? this.PartMajor | HtmlToken.AttrPartMajor.Deleted : this.PartMajor & (HtmlToken.AttrPartMajor) 127;
        }
      }
    }
  }
}
