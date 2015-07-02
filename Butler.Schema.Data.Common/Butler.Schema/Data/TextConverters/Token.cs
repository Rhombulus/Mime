// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Token
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  internal class Token
  {
    private static char[] staticCollapseWhitespace = new char[3]
    {
      ' ',
      '\r',
      '\n'
    };
    protected internal char[] Buffer;
    protected internal Token.RunEntry[] RunList;
    protected internal Token.Fragment Whole;
    protected internal Token.FragmentPosition WholePosition;
      private Token.LowerCaseCompareSink compareSink;
    private Token.LowerCaseSubstringSearchSink searchSink;
    private StringBuildSink stringBuildSink;

    public TokenId TokenId { get; set; }

      public int Argument { get; set; }

      public Encoding TokenEncoding { get; set; }

      public bool IsEmpty => this.Whole.Tail == this.Whole.Head;

      public Token.RunEnumerator Runs => new Token.RunEnumerator(this);

      public Token.TextReader Text => new Token.TextReader(this);

      public bool IsWhitespaceOnly => this.IsWhitespaceOnlyImp(ref this.Whole);

      public Token()
    {
      this.Reset();
    }

    internal static int LiteralLength(int literal)
    {
      return literal <= (int) ushort.MaxValue ? 1 : 2;
    }

    internal static char LiteralFirstChar(int literal)
    {
      if (literal <= (int) ushort.MaxValue)
        return (char) literal;
      return ParseSupport.HighSurrogateCharFromUcs4(literal);
    }

    internal static char LiteralLastChar(int literal)
    {
      if (literal <= (int) ushort.MaxValue)
        return (char) literal;
      return ParseSupport.LowSurrogateCharFromUcs4(literal);
    }

    protected internal bool IsWhitespaceOnlyImp(ref Token.Fragment fragment)
    {
      bool flag = true;
      for (int index = fragment.Head; index != fragment.Tail; ++index)
      {
        if (this.RunList[index].Type >= RunType.Normal && this.RunList[index].TextType > RunTextType.UnusualWhitespace)
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    protected internal int Read(ref Token.Fragment fragment, ref Token.FragmentPosition position, char[] buffer, int offset, int count)
    {
      int num1 = offset;
      int index = position.Run;
      if (index == fragment.Head - 1)
        index = position.Run = fragment.Head;
      if (index != fragment.Tail)
      {
        int num2 = position.RunOffset;
        int num3 = position.RunDeltaOffset;
        do
        {
          Token.RunEntry runEntry = this.RunList[index];
          if (runEntry.Type == RunType.Literal)
          {
            int num4 = Token.LiteralLength(runEntry.Value);
            if (num3 != num4)
            {
              if (num4 == 1)
              {
                buffer[offset++] = (char) runEntry.Value;
                --count;
              }
              else if (num3 != 0)
              {
                buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                --count;
              }
              else
              {
                buffer[offset++] = Token.LiteralFirstChar(runEntry.Value);
                --count;
                if (count == 0)
                {
                  num3 = 1;
                  break;
                }
                buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                --count;
              }
            }
          }
          else if (runEntry.Type == RunType.Normal)
          {
            int num4 = Math.Min(count, runEntry.Length - num3);
            System.Buffer.BlockCopy((Array) this.Buffer, (num2 + num3) * 2, (Array) buffer, offset * 2, num4 * 2);
            offset += num4;
            count -= num4;
            if (num3 + num4 != runEntry.Length)
            {
              num3 += num4;
              break;
            }
          }
          num2 += runEntry.Length;
          num3 = 0;
        }
        while (++index != fragment.Tail && count != 0);
        position.Run = index;
        position.RunOffset = num2;
        position.RunDeltaOffset = num3;
      }
      return offset - num1;
    }

    protected internal int ReadOriginal(ref Token.Fragment fragment, ref Token.FragmentPosition position, char[] buffer, int offset, int count)
    {
      int num1 = offset;
      int index = position.Run;
      if (index == fragment.Head - 1)
        index = position.Run = fragment.Head;
      if (index != fragment.Tail)
      {
        int num2 = position.RunOffset;
        int num3 = position.RunDeltaOffset;
        do
        {
          Token.RunEntry runEntry = this.RunList[index];
          if (runEntry.Type == RunType.Literal || runEntry.Type == RunType.Normal)
          {
            int num4 = Math.Min(count, runEntry.Length - num3);
            System.Buffer.BlockCopy((Array) this.Buffer, (num2 + num3) * 2, (Array) buffer, offset * 2, num4 * 2);
            offset += num4;
            count -= num4;
            if (num3 + num4 != runEntry.Length)
            {
              num3 += num4;
              break;
            }
          }
          num2 += runEntry.Length;
          num3 = 0;
        }
        while (++index != fragment.Tail && count != 0);
        position.Run = index;
        position.RunOffset = num2;
        position.RunDeltaOffset = num3;
      }
      return offset - num1;
    }

    protected internal int Read(Token.LexicalUnit unit, ref Token.FragmentPosition position, char[] buffer, int offset, int count)
    {
      int num1 = offset;
      if (unit.Head != -1)
      {
        uint majorKind = this.RunList[unit.Head].MajorKind;
        int index = position.Run;
        if (index == unit.Head - 1)
          index = position.Run = unit.Head;
        Token.RunEntry runEntry = this.RunList[index];
        if (index == unit.Head || (int) runEntry.MajorKindPlusStartFlag == (int) majorKind)
        {
          int num2 = position.RunOffset;
          int num3 = position.RunDeltaOffset;
          do
          {
            if (runEntry.Type == RunType.Literal)
            {
              int num4 = Token.LiteralLength(runEntry.Value);
              if (num3 != num4)
              {
                if (num4 == 1)
                {
                  buffer[offset++] = (char) runEntry.Value;
                  --count;
                }
                else if (num3 != 0)
                {
                  buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                  --count;
                }
                else
                {
                  buffer[offset++] = Token.LiteralFirstChar(runEntry.Value);
                  --count;
                  if (count == 0)
                  {
                    num3 = 1;
                    break;
                  }
                  buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                  --count;
                }
              }
            }
            else if (runEntry.Type == RunType.Normal)
            {
              int num4 = Math.Min(count, runEntry.Length - num3);
              System.Buffer.BlockCopy((Array) this.Buffer, (num2 + num3) * 2, (Array) buffer, offset * 2, num4 * 2);
              offset += num4;
              count -= num4;
              if (num3 + num4 != runEntry.Length)
              {
                num3 += num4;
                break;
              }
            }
            num2 += runEntry.Length;
            num3 = 0;
            runEntry = this.RunList[++index];
          }
          while ((int) runEntry.MajorKindPlusStartFlag == (int) majorKind && count != 0);
          position.Run = index;
          position.RunOffset = num2;
          position.RunDeltaOffset = num3;
        }
      }
      return offset - num1;
    }

    protected internal virtual void Rewind()
    {
      this.WholePosition.Rewind(this.Whole);
    }

    protected internal int GetLength(ref Token.Fragment fragment)
    {
      int index = fragment.Head;
      int num = 0;
      if (index != fragment.Tail)
      {
        do
        {
          Token.RunEntry runEntry = this.RunList[index];
          if (runEntry.Type == RunType.Normal)
            num += runEntry.Length;
          else if (runEntry.Type == RunType.Literal)
            num += Token.LiteralLength(runEntry.Value);
        }
        while (++index != fragment.Tail);
      }
      return num;
    }

    protected internal int GetOriginalLength(ref Token.Fragment fragment)
    {
      int index = fragment.Head;
      int num = 0;
      if (index != fragment.Tail)
      {
        do
        {
          Token.RunEntry runEntry = this.RunList[index];
          if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
            num += runEntry.Length;
        }
        while (++index != fragment.Tail);
      }
      return num;
    }

    protected internal int GetLength(Token.LexicalUnit unit)
    {
      int index = unit.Head;
      int num = 0;
      if (index != -1)
      {
        Token.RunEntry runEntry = this.RunList[index];
        uint majorKind = runEntry.MajorKind;
        do
        {
          if (runEntry.Type == RunType.Normal)
            num += runEntry.Length;
          else if (runEntry.Type == RunType.Literal)
            num += Token.LiteralLength(runEntry.Value);
          runEntry = this.RunList[++index];
        }
        while ((int) runEntry.MajorKindPlusStartFlag == (int) majorKind);
      }
      return num;
    }

    protected internal bool IsFragmentEmpty(ref Token.Fragment fragment)
    {
      int index = fragment.Head;
      if (index != fragment.Tail)
      {
        do
        {
          Token.RunEntry runEntry = this.RunList[index];
          if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
            return false;
        }
        while (++index != fragment.Tail);
      }
      return true;
    }

    protected internal bool IsFragmentEmpty(Token.LexicalUnit unit)
    {
      int index = unit.Head;
      if (index != -1)
      {
        Token.RunEntry runEntry = this.RunList[index];
        uint majorKind = runEntry.MajorKind;
        while (runEntry.Type != RunType.Normal && runEntry.Type != RunType.Literal)
        {
          runEntry = this.RunList[++index];
          if ((int) runEntry.MajorKindPlusStartFlag != (int) majorKind)
            goto label_5;
        }
        return false;
      }
label_5:
      return true;
    }

    protected internal bool IsContiguous(ref Token.Fragment fragment)
    {
      if (fragment.Head + 1 == fragment.Tail)
        return this.RunList[fragment.Head].Type == RunType.Normal;
      return false;
    }

    protected internal bool IsContiguous(Token.LexicalUnit unit)
    {
      if (this.RunList[unit.Head].Type == RunType.Normal)
        return (int) this.RunList[unit.Head].MajorKind != (int) this.RunList[unit.Head + 1].MajorKindPlusStartFlag;
      return false;
    }

    protected internal int CalculateHashLowerCase(Token.Fragment fragment)
    {
      int index = fragment.Head;
      if (index == fragment.Tail)
        return HashCode.CalculateEmptyHash();
      int offset = fragment.HeadOffset;
      if (index + 1 == fragment.Tail && this.RunList[index].Type == RunType.Normal)
        return HashCode.CalculateLowerCase(this.Buffer, offset, this.RunList[index].Length);
      HashCode hashCode = new HashCode(true);
      do
      {
        Token.RunEntry runEntry = this.RunList[index];
        if (runEntry.Type == RunType.Normal)
          hashCode.AdvanceLowerCase(this.Buffer, offset, runEntry.Length);
        else if (runEntry.Type == RunType.Literal)
          hashCode.AdvanceLowerCase(runEntry.Value);
        offset += runEntry.Length;
      }
      while (++index != fragment.Tail);
      return hashCode.FinalizeHash();
    }

    protected internal int CalculateHashLowerCase(Token.LexicalUnit unit)
    {
      int index = unit.Head;
      if (index == -1)
        return HashCode.CalculateEmptyHash();
      int offset = unit.HeadOffset;
      Token.RunEntry runEntry = this.RunList[index];
      uint majorKind = runEntry.MajorKind;
      if (runEntry.Type == RunType.Normal && (int) majorKind != (int) this.RunList[index + 1].MajorKindPlusStartFlag)
        return HashCode.CalculateLowerCase(this.Buffer, offset, runEntry.Length);
      HashCode hashCode = new HashCode(true);
      do
      {
        if (runEntry.Type == RunType.Normal)
          hashCode.AdvanceLowerCase(this.Buffer, offset, runEntry.Length);
        else if (runEntry.Type == RunType.Literal)
          hashCode.AdvanceLowerCase(runEntry.Value);
        offset += runEntry.Length;
        runEntry = this.RunList[++index];
      }
      while ((int) runEntry.MajorKindPlusStartFlag == (int) majorKind);
      return hashCode.FinalizeHash();
    }

    protected internal int CalculateHash(Token.Fragment fragment)
    {
      int index = fragment.Head;
      if (index == fragment.Tail)
        return HashCode.CalculateEmptyHash();
      int offset = fragment.HeadOffset;
      if (index + 1 == fragment.Tail && this.RunList[index].Type == RunType.Normal)
        return HashCode.Calculate(this.Buffer, offset, this.RunList[index].Length);
      HashCode hashCode = new HashCode(true);
      do
      {
        Token.RunEntry runEntry = this.RunList[index];
        if (runEntry.Type == RunType.Normal)
          hashCode.Advance(this.Buffer, offset, runEntry.Length);
        else if (runEntry.Type == RunType.Literal)
          hashCode.Advance(runEntry.Value);
        offset += runEntry.Length;
      }
      while (++index != fragment.Tail);
      return hashCode.FinalizeHash();
    }

    protected internal int CalculateHash(Token.LexicalUnit unit)
    {
      int index = unit.Head;
      if (index == -1)
        return HashCode.CalculateEmptyHash();
      int offset = unit.HeadOffset;
      Token.RunEntry runEntry = this.RunList[index];
      uint majorKind = runEntry.MajorKind;
      if (runEntry.Type == RunType.Normal && (int) majorKind != (int) this.RunList[index + 1].MajorKindPlusStartFlag)
        return HashCode.Calculate(this.Buffer, offset, runEntry.Length);
      HashCode hashCode = new HashCode(true);
      do
      {
        if (runEntry.Type == RunType.Normal)
          hashCode.Advance(this.Buffer, offset, runEntry.Length);
        else if (runEntry.Type == RunType.Literal)
          hashCode.Advance(runEntry.Value);
        offset += runEntry.Length;
        runEntry = this.RunList[++index];
      }
      while ((int) runEntry.MajorKindPlusStartFlag == (int) majorKind);
      return hashCode.FinalizeHash();
    }

    protected internal void WriteOriginalTo(ref Token.Fragment fragment, ITextSink sink)
    {
      int index = fragment.Head;
      if (index == fragment.Tail)
        return;
      int offset = fragment.HeadOffset;
      do
      {
        Token.RunEntry runEntry = this.RunList[index];
        if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
          sink.Write(this.Buffer, offset, runEntry.Length);
        offset += runEntry.Length;
      }
      while (++index != fragment.Tail && !sink.IsEnough);
    }

    protected internal void WriteTo(ref Token.Fragment fragment, ITextSink sink)
    {
      int index = fragment.Head;
      if (index == fragment.Tail)
        return;
      int offset = fragment.HeadOffset;
      do
      {
        Token.RunEntry runEntry = this.RunList[index];
        if (runEntry.Type == RunType.Normal)
          sink.Write(this.Buffer, offset, runEntry.Length);
        else if (runEntry.Type == RunType.Literal)
          sink.Write(runEntry.Value);
        offset += runEntry.Length;
      }
      while (++index != fragment.Tail && !sink.IsEnough);
    }

    protected internal void WriteTo(Token.LexicalUnit unit, ITextSink sink)
    {
      int index = unit.Head;
      if (index == -1)
        return;
      int offset = unit.HeadOffset;
      Token.RunEntry runEntry = this.RunList[index];
      uint majorKind = runEntry.MajorKind;
      do
      {
        if (runEntry.Type == RunType.Normal)
          sink.Write(this.Buffer, offset, runEntry.Length);
        else if (runEntry.Type == RunType.Literal)
          sink.Write(runEntry.Value);
        offset += runEntry.Length;
        runEntry = this.RunList[++index];
      }
      while ((int) runEntry.MajorKindPlusStartFlag == (int) majorKind && !sink.IsEnough);
    }

    protected internal void WriteToAndCollapseWhitespace(ref Token.Fragment fragment, ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
    {
      int run = fragment.Head;
      if (run == fragment.Tail)
        return;
      int runOffset = fragment.HeadOffset;
      if (this.RunList[run].Type < RunType.Normal)
        this.SkipNonTextRuns(ref run, ref runOffset, fragment.Tail);
      while (run != fragment.Tail && !sink.IsEnough)
      {
        if (this.RunList[run].TextType <= RunTextType.Nbsp)
        {
          if (this.RunList[run].TextType == RunTextType.NewLine)
            collapseWhitespaceState = CollapseWhitespaceState.NewLine;
          else if (collapseWhitespaceState == CollapseWhitespaceState.NonSpace)
            collapseWhitespaceState = CollapseWhitespaceState.Whitespace;
        }
        else
        {
          if (collapseWhitespaceState != CollapseWhitespaceState.NonSpace)
          {
            if (collapseWhitespaceState == CollapseWhitespaceState.NewLine)
              sink.Write(Token.staticCollapseWhitespace, 1, 2);
            else
              sink.Write(Token.staticCollapseWhitespace, 0, 1);
            collapseWhitespaceState = CollapseWhitespaceState.NonSpace;
          }
          if (this.RunList[run].Type == RunType.Literal)
            sink.Write(this.RunList[run].Value);
          else
            sink.Write(this.Buffer, runOffset, this.RunList[run].Length);
        }
        runOffset += this.RunList[run].Length;
        ++run;
        if (run != fragment.Tail && this.RunList[run].Type < RunType.Normal)
          this.SkipNonTextRuns(ref run, ref runOffset, fragment.Tail);
      }
    }

    protected internal string GetString(ref Token.Fragment fragment, int maxLength)
    {
      if (fragment.Head == fragment.Tail)
        return string.Empty;
      if (this.IsContiguous(ref fragment))
        return new string(this.Buffer, fragment.HeadOffset, this.GetLength(ref fragment));
      if (this.IsFragmentEmpty(ref fragment))
        return string.Empty;
      if (this.stringBuildSink == null)
        this.stringBuildSink = new StringBuildSink();
      this.stringBuildSink.Reset(maxLength);
      this.WriteTo(ref fragment, (ITextSink) this.stringBuildSink);
      return this.stringBuildSink.ToString();
    }

    protected internal string GetString(Token.LexicalUnit unit, int maxLength)
    {
      if (this.IsFragmentEmpty(unit))
        return string.Empty;
      if (this.IsContiguous(unit))
        return new string(this.Buffer, unit.HeadOffset, this.GetLength(unit));
      if (this.stringBuildSink == null)
        this.stringBuildSink = new StringBuildSink();
      this.stringBuildSink.Reset(maxLength);
      this.WriteTo(unit, (ITextSink) this.stringBuildSink);
      return this.stringBuildSink.ToString();
    }

    protected internal bool CaseInsensitiveCompareEqual(ref Token.Fragment fragment, string str)
    {
      if (this.compareSink == null)
        this.compareSink = new Token.LowerCaseCompareSink();
      this.compareSink.Reset(str);
      this.WriteTo(ref fragment, (ITextSink) this.compareSink);
      return this.compareSink.IsEqual;
    }

    protected internal bool CaseInsensitiveCompareEqual(Token.LexicalUnit unit, string str)
    {
      if (this.compareSink == null)
        this.compareSink = new Token.LowerCaseCompareSink();
      this.compareSink.Reset(str);
      this.WriteTo(unit, (ITextSink) this.compareSink);
      return this.compareSink.IsEqual;
    }

    protected internal virtual bool CaseInsensitiveCompareRunEqual(int runOffset, string str, int strOffset)
    {
      int num = strOffset;
      while (num < str.Length)
      {
        if ((int) ParseSupport.ToLowerCase(this.Buffer[runOffset++]) != (int) str[num++])
          return false;
      }
      return true;
    }

    protected internal bool CaseInsensitiveContainsSubstring(ref Token.Fragment fragment, string str)
    {
      if (this.searchSink == null)
        this.searchSink = new Token.LowerCaseSubstringSearchSink();
      this.searchSink.Reset(str);
      this.WriteTo(ref fragment, (ITextSink) this.searchSink);
      return this.searchSink.IsFound;
    }

    protected internal bool CaseInsensitiveContainsSubstring(Token.LexicalUnit unit, string str)
    {
      if (this.searchSink == null)
        this.searchSink = new Token.LowerCaseSubstringSearchSink();
      this.searchSink.Reset(str);
      this.WriteTo(unit, (ITextSink) this.searchSink);
      return this.searchSink.IsFound;
    }

    protected internal void StripLeadingWhitespace(ref Token.Fragment fragment)
    {
      int run = fragment.Head;
      if (run == fragment.Tail)
        return;
      int runOffset = fragment.HeadOffset;
      if (this.RunList[run].Type < RunType.Normal)
        this.SkipNonTextRuns(ref run, ref runOffset, fragment.Tail);
      if (run == fragment.Tail)
        return;
      do
      {
        if (this.RunList[run].Type == RunType.Literal)
        {
          if (this.RunList[run].Value > (int) ushort.MaxValue || !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char) this.RunList[run].Value)))
            break;
        }
        else
        {
          int index = runOffset;
          while (index < runOffset + this.RunList[run].Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(this.Buffer[index])))
            ++index;
          if (index < runOffset + this.RunList[run].Length)
          {
            this.RunList[run].Length -= index - runOffset;
            runOffset = index;
            break;
          }
        }
        runOffset += this.RunList[run].Length;
        ++run;
        if (run != fragment.Tail && this.RunList[run].Type < RunType.Normal)
          this.SkipNonTextRuns(ref run, ref runOffset, fragment.Tail);
      }
      while (run != fragment.Tail);
      fragment.Head = run;
      fragment.HeadOffset = runOffset;
    }

    protected internal bool SkipLeadingWhitespace(Token.LexicalUnit unit, ref Token.FragmentPosition position)
    {
      int index1 = unit.Head;
      if (index1 != -1)
      {
        int num1 = unit.HeadOffset;
        Token.RunEntry runEntry = this.RunList[index1];
        uint majorKind = runEntry.MajorKind;
        int num2 = 0;
        do
        {
          if (runEntry.Type == RunType.Literal)
          {
            if (runEntry.Value > (int) ushort.MaxValue || !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char) runEntry.Value)))
              break;
          }
          else if (runEntry.Type == RunType.Normal)
          {
            int index2 = num1;
            while (index2 < num1 + runEntry.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(this.Buffer[index2])))
              ++index2;
            if (index2 < num1 + runEntry.Length)
            {
              num2 = index2 - num1;
              break;
            }
          }
          num1 += runEntry.Length;
          runEntry = this.RunList[++index1];
        }
        while ((int) runEntry.MajorKindPlusStartFlag == (int) majorKind);
        position.Run = index1;
        position.RunOffset = num1;
        position.RunDeltaOffset = num2;
        if (index1 == unit.Head || (int) runEntry.MajorKindPlusStartFlag == (int) majorKind)
          return true;
      }
      return false;
    }

    protected internal bool MoveToNextRun(ref Token.Fragment fragment, ref Token.FragmentPosition position, bool skipInvalid)
    {
      int index1 = position.Run;
      if (index1 == fragment.Tail)
        return false;
      if (index1 >= fragment.Head)
      {
        position.RunOffset += this.RunList[index1].Length;
        position.RunDeltaOffset = 0;
      }
      int index2 = index1 + 1;
      if (skipInvalid)
      {
        for (; index2 != fragment.Tail && this.RunList[index2].Type == RunType.Invalid; ++index2)
          position.RunOffset += this.RunList[index2].Length;
      }
      position.Run = index2;
      return index2 != fragment.Tail;
    }

    protected internal bool IsCurrentEof(ref Token.FragmentPosition position)
    {
      int index = position.Run;
      if (this.RunList[index].Type == RunType.Literal)
        return position.RunDeltaOffset == Token.LiteralLength(this.RunList[index].Value);
      return position.RunDeltaOffset == this.RunList[index].Length;
    }

    protected internal int ReadCurrent(ref Token.FragmentPosition position, char[] buffer, int offset, int count)
    {
      int index = position.Run;
      if (this.RunList[index].Type == RunType.Literal)
      {
        int num = Token.LiteralLength(this.RunList[index].Value);
        if (position.RunDeltaOffset == num)
          return 0;
        if (num == 1)
        {
          buffer[offset] = (char) this.RunList[index].Value;
          ++position.RunDeltaOffset;
          return 1;
        }
        if (position.RunDeltaOffset != 0)
        {
          buffer[offset] = Token.LiteralLastChar(this.RunList[index].Value);
          ++position.RunDeltaOffset;
          return 1;
        }
        buffer[offset++] = Token.LiteralFirstChar(this.RunList[index].Value);
        --count;
        ++position.RunDeltaOffset;
        if (count == 0)
          return 1;
        buffer[offset] = Token.LiteralLastChar(this.RunList[index].Value);
        ++position.RunDeltaOffset;
        return 2;
      }
      int num1 = Math.Min(count, this.RunList[index].Length - position.RunDeltaOffset);
      if (num1 != 0)
      {
        System.Buffer.BlockCopy((Array) this.Buffer, (position.RunOffset + position.RunDeltaOffset) * 2, (Array) buffer, offset * 2, num1 * 2);
        position.RunDeltaOffset += num1;
      }
      return num1;
    }

    internal void SkipNonTextRuns(ref int run, ref int runOffset, int tail)
    {
      do
      {
        runOffset += this.RunList[run].Length;
        ++run;
      }
      while (run != tail && this.RunList[run].Type < RunType.Normal);
    }

    internal void Reset()
    {
      this.TokenId = TokenId.None;
      this.Argument = 0;
      this.TokenEncoding = (Encoding) null;
      this.Whole.Reset();
      this.WholePosition.Reset();
    }

    public struct RunEnumerator
    {
      private Token token;

      public TokenRun Current => new TokenRun(this.token);

        public bool IsValidPosition
      {
        get
        {
          if (this.token.WholePosition.Run >= this.token.Whole.Head)
            return this.token.WholePosition.Run < this.token.Whole.Tail;
          return false;
        }
      }

      public int CurrentIndex => this.token.WholePosition.Run;

        public int CurrentOffset => this.token.WholePosition.RunOffset;

        internal RunEnumerator(Token token)
      {
        this.token = token;
      }

      public bool MoveNext()
      {
        return this.token.MoveToNextRun(ref this.token.Whole, ref this.token.WholePosition, false);
      }

      public bool MoveNext(bool skipInvalid)
      {
        return this.token.MoveToNextRun(ref this.token.Whole, ref this.token.WholePosition, skipInvalid);
      }

      public void Rewind()
      {
        this.token.WholePosition.Rewind(this.token.Whole);
      }

      public Token.RunEnumerator GetEnumerator()
      {
        return this;
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    public struct TextReader
    {
      private Token token;

      public int Length => this.token.GetLength(ref this.token.Whole);

        public int OriginalLength => this.token.GetOriginalLength(ref this.token.Whole);

        internal TextReader(Token token)
      {
        this.token = token;
      }

      public int Read(char[] buffer, int offset, int count)
      {
        return this.token.Read(ref this.token.Whole, ref this.token.WholePosition, buffer, offset, count);
      }

      public void Rewind()
      {
        this.token.WholePosition.Rewind(this.token.Whole);
      }

      public void WriteTo(ITextSink sink)
      {
        this.token.WriteTo(ref this.token.Whole, sink);
      }

      public void WriteToAndCollapseWhitespace(ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
      {
        this.token.WriteToAndCollapseWhitespace(ref this.token.Whole, sink, ref collapseWhitespaceState);
      }

      public void StripLeadingWhitespace()
      {
        this.token.StripLeadingWhitespace(ref this.token.Whole);
        this.Rewind();
      }

      public int ReadOriginal(char[] buffer, int offset, int count)
      {
        return this.token.ReadOriginal(ref this.token.Whole, ref this.token.WholePosition, buffer, offset, count);
      }

      public void WriteOriginalTo(ITextSink sink)
      {
        this.token.WriteOriginalTo(ref this.token.Whole, sink);
      }

      [Conditional("DEBUG")]
      private void AssertCurrent()
      {
      }
    }

    internal struct RunEntry
    {
      internal const int MaxRunLength = 134217727;
      internal const int MaxRunValue = 16777215;
      private uint lengthAndType;
      private uint valueAndKind;

      public RunType Type => (RunType) ((int) this.lengthAndType & -1073741824);

        public RunTextType TextType => (RunTextType) ((int) this.lengthAndType & 939524096);

        public int Length
      {
        get
        {
          return (int) this.lengthAndType & 16777215;
        }
        set
        {
          this.lengthAndType = (uint) (value | (int) this.lengthAndType & -16777216);
        }
      }

      public uint Kind => this.valueAndKind & 4278190080U;

        public uint MajorKindPlusStartFlag => this.valueAndKind & 4227858432U;

        public uint MajorKind => this.valueAndKind & 2080374784U;

        public int Value => (int) this.valueAndKind & 16777215;

        public void Initialize(RunType type, RunTextType textType, uint kind, int length, int value)
      {
        this.lengthAndType = (uint) ((RunType) length | type | (RunType) textType);
        this.valueAndKind = (uint) value | kind;
      }

      public void InitializeSentinel()
      {
        this.valueAndKind = unchecked( (uint) int.MinValue);
      }

      public override string ToString()
      {
        return this.Type.ToString() + (object) " - " + this.TextType.ToString() + " - " + ((this.Kind & (uint) int.MaxValue) >> 26).ToString() + "/" + (this.Kind >> 24 & 3U).ToString() + " (" + (string) (object) this.Length + ") = " + this.Value.ToString("X6");
      }
    }

    internal struct LexicalUnit
    {
      public int Head;
      public int HeadOffset;

      public void Reset()
      {
        this.Head = -1;
        this.HeadOffset = 0;
      }

      public void Initialize(int run, int offset)
      {
        this.Head = run;
        this.HeadOffset = offset;
      }

      public override string ToString()
      {
        return this.Head.ToString("X") + " / " + this.HeadOffset.ToString("X");
      }
    }

    internal struct Fragment
    {
      public int Head;
      public int Tail;
      public int HeadOffset;

      public bool IsEmpty => this.Head == this.Tail;

        public void Reset()
      {
        this.Head = this.Tail = this.HeadOffset = 0;
      }

      public void Initialize(int run, int offset)
      {
        this.Head = this.Tail = run;
        this.HeadOffset = offset;
      }

      public override string ToString()
      {
        return this.Head.ToString("X") + " - " + this.Tail.ToString("X") + " / " + this.HeadOffset.ToString("X");
      }
    }

    internal struct FragmentPosition
    {
      public int Run;
      public int RunOffset;
      public int RunDeltaOffset;

      public void Reset()
      {
        this.Run = -2;
        this.RunOffset = 0;
        this.RunDeltaOffset = 0;
      }

      public void Rewind(Token.LexicalUnit unit)
      {
        this.Run = unit.Head - 1;
        this.RunOffset = unit.HeadOffset;
        this.RunDeltaOffset = 0;
      }

      public void Rewind(Token.Fragment fragment)
      {
        this.Run = fragment.Head - 1;
        this.RunOffset = fragment.HeadOffset;
        this.RunDeltaOffset = 0;
      }

      public bool SameAs(Token.FragmentPosition pos2)
      {
        if (this.Run == pos2.Run && this.RunOffset == pos2.RunOffset)
          return this.RunDeltaOffset == pos2.RunDeltaOffset;
        return false;
      }

      public override string ToString()
      {
        return this.Run.ToString("X") + " / " + this.RunOffset.ToString("X") + " + " + this.RunDeltaOffset.ToString("X");
      }
    }

    private class LowerCaseCompareSink : ITextSink
    {

        private int strIndex;
      private string str;

      public bool IsEqual
      {
        get
        {
          if (!this.IsEnough)
            return this.strIndex == this.str.Length;
          return false;
        }
      }

      public bool IsEnough { get; private set; }

        public void Reset(string str)
      {
        this.str = str;
        this.strIndex = 0;
        this.IsEnough = false;
      }

      public void Write(char[] buffer, int offset, int count)
      {
        int num = offset + count;
        while (offset < num)
        {
          if (this.strIndex == 0)
          {
            if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
            {
              ++offset;
              continue;
            }
          }
          else if (this.strIndex == this.str.Length)
          {
            if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
            {
              ++offset;
              continue;
            }
            this.IsEnough = true;
            break;
          }
          if ((int) ParseSupport.ToLowerCase(buffer[offset]) != (int) this.str[this.strIndex])
          {
            this.IsEnough = true;
            break;
          }
          ++offset;
          ++this.strIndex;
        }
      }

      public void Write(int ucs32Char)
      {
        if (Token.LiteralLength(ucs32Char) != 1)
        {
          this.IsEnough = true;
        }
        else
        {
          if (this.strIndex == 0)
          {
            if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char) ucs32Char)))
              return;
          }
          else if (this.strIndex == this.str.Length)
          {
            if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char) ucs32Char)))
              return;
            this.IsEnough = true;
            return;
          }
          if ((int) this.str[this.strIndex] != (int) ParseSupport.ToLowerCase((char) ucs32Char))
            this.IsEnough = true;
          else
            ++this.strIndex;
        }
      }
    }

    private class LowerCaseSubstringSearchSink : ITextSink
    {

        private int strIndex;
      private string str;

      public bool IsFound { get; private set; }

        public bool IsEnough => this.IsFound;

        public void Reset(string str)
      {
        this.str = str;
        this.strIndex = 0;
        this.IsFound = false;
      }

      public void Write(char[] buffer, int offset, int count)
      {
        for (int index = offset + count; offset < index && this.strIndex < this.str.Length; ++offset)
        {
          if ((int) ParseSupport.ToLowerCase(buffer[offset]) == (int) this.str[this.strIndex])
            ++this.strIndex;
          else
            this.strIndex = 0;
        }
        if (this.strIndex != this.str.Length)
          return;
        this.IsFound = true;
      }

      public void Write(int ucs32Char)
      {
        if (Token.LiteralLength(ucs32Char) != 1 || (int) this.str[this.strIndex] != (int) ParseSupport.ToLowerCase((char) ucs32Char))
        {
          this.strIndex = 0;
        }
        else
        {
          ++this.strIndex;
          if (this.strIndex != this.str.Length)
            return;
          this.IsFound = true;
        }
      }
    }
  }
}
