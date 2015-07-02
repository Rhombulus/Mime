// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Css.CssToken
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Css
{
  internal class CssToken : Token
  {
    protected internal CssToken.PropertyListPartMajor PartMajor;
    protected internal CssToken.PropertyListPartMinor PartMinor;
    protected internal CssToken.PropertyEntry[] PropertyList;
    protected internal int PropertyHead;
    protected internal int PropertyTail;
    protected internal int CurrentProperty;
    protected internal Token.FragmentPosition PropertyNamePosition;
    protected internal Token.FragmentPosition PropertyValuePosition;
    protected internal CssToken.SelectorEntry[] SelectorList;
    protected internal int SelectorHead;
    protected internal int SelectorTail;
    protected internal int CurrentSelector;
    protected internal Token.FragmentPosition SelectorNamePosition;
    protected internal Token.FragmentPosition SelectorClassPosition;

    public CssTokenId CssTokenId => (CssTokenId) this.TokenId;

      public CssToken.PropertyListPartMajor MajorPart => this.PartMajor;

      public CssToken.PropertyListPartMinor MinorPart => this.PartMinor;

      public bool IsPropertyListBegin => (this.PartMajor & CssToken.PropertyListPartMajor.Begin) == CssToken.PropertyListPartMajor.Begin;

      public bool IsPropertyListEnd => (this.PartMajor & CssToken.PropertyListPartMajor.End) == CssToken.PropertyListPartMajor.End;

      public CssToken.PropertyEnumerator Properties => new CssToken.PropertyEnumerator(this);

      public CssToken.SelectorEnumerator Selectors => new CssToken.SelectorEnumerator(this);

      public CssToken()
    {
      this.Reset();
    }

    internal static bool AttemptUnescape(char[] parseBuffer, int parseEnd, ref char ch, ref int parseCurrent)
    {
      if ((int) ch != 92 || parseCurrent == parseEnd)
        return false;
      ch = parseBuffer[++parseCurrent];
      CharClass charClass = ParseSupport.GetCharClass(ch);
      int num1 = parseCurrent + 6;
      int num2 = num1 < parseEnd ? num1 : parseEnd;
      if (ParseSupport.HexCharacter(charClass))
      {
        int num3 = 0;
        do
        {
          num3 = num3 << 4 | ParseSupport.CharToHex(ch);
          if (parseCurrent != num2)
            ch = parseBuffer[++parseCurrent];
          else
            goto label_12;
        }
        while (ParseSupport.HexCharacter(ParseSupport.GetCharClass(ch)));
        if ((int) ch == 13 && parseCurrent != parseEnd)
        {
          ch = parseBuffer[++parseCurrent];
          if ((int) ch == 10)
            ParseSupport.GetCharClass(ch);
          else
            --parseCurrent;
        }
        if ((int) ch != 32 && (int) ch != 9 && ((int) ch != 13 && (int) ch != 10) && (int) ch != 12)
          --parseCurrent;
label_12:
        ch = (char) num3;
        return true;
      }
      if ((int) ch >= 32 && (int) ch != (int) sbyte.MaxValue)
        return true;
      --parseCurrent;
      ch = '\\';
      return false;
    }

    internal new void Reset()
    {
      this.PartMajor = CssToken.PropertyListPartMajor.None;
      this.PartMinor = CssToken.PropertyListPartMinor.Empty;
      this.PropertyHead = this.PropertyTail = 0;
      this.CurrentProperty = -1;
      this.SelectorHead = this.SelectorTail = 0;
      this.CurrentSelector = -1;
    }

    protected internal void WriteEscapedOriginalTo(ref Token.Fragment fragment, ITextSink sink)
    {
      int index = fragment.Head;
      if (index == fragment.Tail)
        return;
      int offset = fragment.HeadOffset;
      do
      {
        Token.RunEntry runEntry = this.RunList[index];
        if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
        {
          if ((int) runEntry.Kind == 184549376 && (int) this.Buffer[offset] == 47)
          {
            foreach (int ucs32Char in "/**/")
              sink.Write(ucs32Char);
          }
          else
            this.EscapeAndWriteBuffer(this.Buffer, offset, runEntry.Length, sink);
        }
        offset += runEntry.Length;
      }
      while (++index != fragment.Tail && !sink.IsEnough);
    }

    private void EscapeAndWriteBuffer(char[] buffer, int offset, int length, ITextSink sink)
    {
      int offset1 = offset;
      int parseCurrent = offset;
      while (parseCurrent < offset + length)
      {
        char ch = buffer[parseCurrent];
        switch (ch)
        {
          case '>':
          case '<':
            if (parseCurrent - offset1 > 0)
              sink.Write(buffer, offset1, parseCurrent - offset1);
            uint num1 = (uint) ch;
            char[] buffer1 = new char[4]
            {
              '\\',
              char.MinValue,
              char.MinValue,
              ' '
            };
            for (int index = 2; index > 0; --index)
            {
              uint num2 = num1 & 15U;
              buffer1[index] = (char) ((long) num2 + (num2 < 10U ? 48L : 55L));
              num1 >>= 4;
            }
            sink.Write(buffer1, 0, 4);
            offset1 = ++parseCurrent;
            continue;
          default:
            CssToken.AttemptUnescape(buffer, offset + length, ref ch, ref parseCurrent);
            ++parseCurrent;
            continue;
        }
      }
      sink.Write(buffer, offset1, length - (offset1 - offset));
    }

    public enum PropertyListPartMajor : byte
    {
      None = (byte) 0,
      Continue = (byte) 2,
      Begin = (byte) 3,
      End = (byte) 6,
      Complete = (byte) 7,
    }

    public enum PropertyListPartMinor : byte
    {
      Empty = (byte) 0,
      ContinueProperty = (byte) 16,
      BeginProperty = (byte) 24,
      EndProperty = (byte) 48,
      PropertyPartMask = (byte) 56,
      Properties = (byte) 128,
      EndPropertyWithOtherProperties = (byte) 176,
    }

    public enum PropertyPartMajor : byte
    {
      None = (byte) 0,
      Continue = (byte) 2,
      Begin = (byte) 3,
      End = (byte) 6,
      Complete = (byte) 7,
      MaskOffFlags = (byte) 7,
      ValueQuoted = (byte) 64,
      Deleted = (byte) 128,
    }

    public enum PropertyPartMinor : byte
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

    public struct PropertyEnumerator
    {
      private CssToken token;

      public int Count => this.token.PropertyTail - this.token.PropertyHead;

        public int ValidCount
      {
        get
        {
          int num = 0;
          for (int index = this.token.PropertyHead; index < this.token.PropertyTail; ++index)
          {
            if (!this.token.PropertyList[index].IsPropertyDeleted)
              ++num;
          }
          return num;
        }
      }

      public CssProperty Current => new CssProperty(this.token);

        public int CurrentIndex => this.token.CurrentProperty;

        public CssProperty this[int i]
      {
        get
        {
          this.token.CurrentProperty = i;
          this.token.PropertyNamePosition.Rewind(this.token.PropertyList[i].Name);
          this.token.PropertyValuePosition.Rewind(this.token.PropertyList[i].Value);
          return new CssProperty(this.token);
        }
      }

      internal PropertyEnumerator(CssToken token)
      {
        this.token = token;
      }

      public bool MoveNext()
      {
        if (this.token.CurrentProperty != this.token.PropertyTail)
        {
          ++this.token.CurrentProperty;
          if (this.token.CurrentProperty != this.token.PropertyTail)
          {
            this.token.PropertyNamePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Name);
            this.token.PropertyValuePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Value);
          }
        }
        return this.token.CurrentProperty != this.token.PropertyTail;
      }

      public void Rewind()
      {
        this.token.CurrentProperty = this.token.PropertyHead - 1;
      }

      public CssToken.PropertyEnumerator GetEnumerator()
      {
        return this;
      }

      public bool Find(CssNameIndex nameId)
      {
        for (int index = this.token.PropertyHead; index < this.token.PropertyTail; ++index)
        {
          if (this.token.PropertyList[index].NameId == nameId)
          {
            this.token.CurrentProperty = index;
            this.token.PropertyNamePosition.Rewind(this.token.PropertyList[index].Name);
            this.token.PropertyValuePosition.Rewind(this.token.PropertyList[index].Value);
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

    public struct PropertyNameTextReader
    {
      private CssToken token;

      public int Length => this.token.GetLength(ref this.token.PropertyList[this.token.CurrentProperty].Name);

        internal PropertyNameTextReader(CssToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(ref this.token.PropertyList[this.token.CurrentProperty].Name, ref this.token.PropertyNamePosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.PropertyNamePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Name);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(ref this.token.PropertyList[this.token.CurrentProperty].Name, sink);
      }

      public void WriteOriginalTo(ITextSink sink)
      {
        this.token.WriteOriginalTo(ref this.token.PropertyList[this.token.CurrentProperty].Name, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(ref this.token.PropertyList[this.token.CurrentProperty].Name, maxSize);
      }

      public void MakeEmpty()
      {
        this.token.PropertyList[this.token.CurrentProperty].Name.Reset();
        this.Rewind();
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct PropertyValueTextReader
    {
      private CssToken token;

      public int Length => this.token.GetLength(ref this.token.PropertyList[this.token.CurrentProperty].Value);

        public bool IsEmpty => this.token.IsFragmentEmpty(ref this.token.PropertyList[this.token.CurrentProperty].Value);

        public bool IsContiguous => this.token.IsContiguous(ref this.token.PropertyList[this.token.CurrentProperty].Value);

        public BufferString ContiguousBufferString => new BufferString(this.token.Buffer, this.token.PropertyList[this.token.CurrentProperty].Value.HeadOffset, this.token.RunList[this.token.PropertyList[this.token.CurrentProperty].Value.Head].Length);

        internal PropertyValueTextReader(CssToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(ref this.token.PropertyList[this.token.CurrentProperty].Value, ref this.token.PropertyValuePosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.PropertyValuePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Value);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(ref this.token.PropertyList[this.token.CurrentProperty].Value, sink);
      }

      public void WriteOriginalTo(ITextSink sink)
      {
        this.token.WriteOriginalTo(ref this.token.PropertyList[this.token.CurrentProperty].Value, sink);
      }

      public void WriteEscapedOriginalTo(ITextSink sink)
      {
        this.token.WriteEscapedOriginalTo(ref this.token.PropertyList[this.token.CurrentProperty].Value, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(ref this.token.PropertyList[this.token.CurrentProperty].Value, maxSize);
      }

      public bool CaseInsensitiveCompareEqual(string str)
      {
        return this.token.CaseInsensitiveCompareEqual(ref this.token.PropertyList[this.token.CurrentProperty].Value, str);
      }

      public bool CaseInsensitiveContainsSubstring(string str)
      {
        return this.token.CaseInsensitiveContainsSubstring(ref this.token.PropertyList[this.token.CurrentProperty].Value, str);
      }

      public void MakeEmpty()
      {
        this.token.PropertyList[this.token.CurrentProperty].Value.Reset();
        this.Rewind();
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct SelectorEnumerator
    {
      private CssToken token;

      public int Count => this.token.SelectorTail - this.token.SelectorHead;

        public int ValidCount
      {
        get
        {
          int num = 0;
          for (int index = this.token.SelectorHead; index < this.token.SelectorTail; ++index)
          {
            if (!this.token.SelectorList[index].IsSelectorDeleted)
              ++num;
          }
          return num;
        }
      }

      public CssSelector Current => new CssSelector(this.token);

        public int CurrentIndex => this.token.CurrentSelector;

        public CssSelector this[int i]
      {
        get
        {
          this.token.CurrentSelector = i;
          this.token.SelectorNamePosition.Rewind(this.token.SelectorList[i].Name);
          this.token.SelectorClassPosition.Rewind(this.token.SelectorList[i].ClassName);
          return new CssSelector(this.token);
        }
      }

      internal SelectorEnumerator(CssToken token)
      {
        this.token = token;
      }

      public bool MoveNext()
      {
        if (this.token.CurrentSelector != this.token.SelectorTail)
        {
          ++this.token.CurrentSelector;
          if (this.token.CurrentSelector != this.token.SelectorTail)
          {
            this.token.SelectorNamePosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].Name);
            this.token.SelectorClassPosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].ClassName);
          }
        }
        return this.token.CurrentSelector != this.token.SelectorTail;
      }

      public void Rewind()
      {
        this.token.CurrentSelector = this.token.SelectorHead - 1;
      }

      public CssToken.SelectorEnumerator GetEnumerator()
      {
        return this;
      }

      public bool Find(Html.HtmlNameIndex nameId)
      {
        for (int index = this.token.SelectorHead; index < this.token.SelectorTail; ++index)
        {
          if (this.token.SelectorList[index].NameId == nameId)
          {
            this.token.CurrentSelector = index;
            this.token.SelectorNamePosition.Rewind(this.token.SelectorList[index].Name);
            this.token.SelectorClassPosition.Rewind(this.token.SelectorList[index].ClassName);
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

    public struct SelectorNameTextReader
    {
      private CssToken token;

      public int Length => this.token.GetLength(ref this.token.SelectorList[this.token.CurrentSelector].Name);

        internal SelectorNameTextReader(CssToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(ref this.token.SelectorList[this.token.CurrentSelector].Name, ref this.token.SelectorNamePosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.SelectorNamePosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].Name);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(ref this.token.SelectorList[this.token.CurrentSelector].Name, sink);
      }

      public void WriteOriginalTo(ITextSink sink)
      {
        this.token.WriteOriginalTo(ref this.token.SelectorList[this.token.CurrentSelector].Name, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(ref this.token.SelectorList[this.token.CurrentSelector].Name, maxSize);
      }

      public void MakeEmpty()
      {
        this.token.SelectorList[this.token.CurrentSelector].Name.Reset();
        this.Rewind();
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct SelectorClassTextReader
    {
      private CssToken token;

      public int Length => this.token.GetLength(ref this.token.SelectorList[this.token.CurrentSelector].ClassName);

        internal SelectorClassTextReader(CssToken token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, ref this.token.SelectorClassPosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.SelectorClassPosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].ClassName);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, sink);
      }

      public void WriteOriginalTo(ITextSink sink)
      {
        this.token.WriteEscapedOriginalTo(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, sink);
      }

      public string GetString(int maxSize)
      {
        return this.token.GetString(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, maxSize);
      }

      public bool CaseInsensitiveCompareEqual(string str)
      {
        return this.token.CaseInsensitiveCompareEqual(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, str);
      }

      public bool CaseInsensitiveContainsSubstring(string str)
      {
        return this.token.CaseInsensitiveContainsSubstring(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, str);
      }

      public void MakeEmpty()
      {
        this.token.SelectorList[this.token.CurrentSelector].ClassName.Reset();
        this.Rewind();
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    protected internal struct PropertyEntry
    {
      public CssNameIndex NameId;
      public byte QuoteChar;
      public CssToken.PropertyPartMajor PartMajor;
      public CssToken.PropertyPartMinor PartMinor;
      public Token.Fragment Name;
      public Token.Fragment Value;

      public bool IsCompleteProperty => this.MajorPart == CssToken.PropertyPartMajor.Complete;

        public bool IsPropertyBegin => (this.PartMajor & CssToken.PropertyPartMajor.Begin) == CssToken.PropertyPartMajor.Begin;

        public bool IsPropertyEnd => (this.PartMajor & CssToken.PropertyPartMajor.End) == CssToken.PropertyPartMajor.End;

        public bool IsPropertyNameEnd => (this.PartMinor & CssToken.PropertyPartMinor.EndName) == CssToken.PropertyPartMinor.EndName;

        public bool IsPropertyValueBegin => (this.PartMinor & CssToken.PropertyPartMinor.BeginValue) == CssToken.PropertyPartMinor.BeginValue;

        public CssToken.PropertyPartMajor MajorPart => this.PartMajor & CssToken.PropertyPartMajor.Complete;

        public CssToken.PropertyPartMinor MinorPart
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

      public bool IsPropertyValueQuoted
      {
        get
        {
          return (this.PartMajor & CssToken.PropertyPartMajor.ValueQuoted) == CssToken.PropertyPartMajor.ValueQuoted;
        }
        set
        {
          this.PartMajor = value ? this.PartMajor | CssToken.PropertyPartMajor.ValueQuoted : this.PartMajor & (CssToken.PropertyPartMajor) 191;
        }
      }

      public bool IsPropertyDeleted
      {
        get
        {
          return (this.PartMajor & CssToken.PropertyPartMajor.Deleted) == CssToken.PropertyPartMajor.Deleted;
        }
        set
        {
          this.PartMajor = value ? this.PartMajor | CssToken.PropertyPartMajor.Deleted : this.PartMajor & (CssToken.PropertyPartMajor) 127;
        }
      }
    }

    protected internal struct SelectorEntry
    {
      public Html.HtmlNameIndex NameId;
      private bool deleted;
      public Token.Fragment Name;
      public Token.Fragment ClassName;
      public CssSelectorClassType ClassType;
      public CssSelectorCombinator Combinator;

      public bool IsSelectorDeleted
      {
        get
        {
          return this.deleted;
        }
        set
        {
          this.deleted = value;
        }
      }
    }
  }
}
