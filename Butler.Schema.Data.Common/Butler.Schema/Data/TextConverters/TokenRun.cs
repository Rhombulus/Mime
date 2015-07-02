// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TokenRun
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal struct TokenRun
  {
    private Token token;

    public RunType Type
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Type;
      }
    }

    public bool IsTextRun
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Type >= RunType.Normal;
      }
    }

    public bool IsSpecial
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Type == RunType.Special;
      }
    }

    public bool IsNormal
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Type == RunType.Normal;
      }
    }

    public bool IsLiteral
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Type == RunType.Literal;
      }
    }

    public RunTextType TextType
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].TextType;
      }
    }

    public char[] RawBuffer
    {
      get
      {
        return this.token.Buffer;
      }
    }

    public int RawOffset
    {
      get
      {
        return this.token.WholePosition.RunOffset;
      }
    }

    public int RawLength
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Length;
      }
    }

    public uint Kind
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Kind;
      }
    }

    public int Literal
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Value;
      }
    }

    public int Length
    {
      get
      {
        if (this.IsNormal)
          return this.RawLength;
        if (!this.IsLiteral)
          return 0;
        return Token.LiteralLength(this.Literal);
      }
    }

    public int Value
    {
      get
      {
        return this.token.RunList[this.token.WholePosition.Run].Value;
      }
    }

    public char FirstChar
    {
      get
      {
        if (!this.IsLiteral)
          return this.RawBuffer[this.RawOffset];
        return Token.LiteralFirstChar(this.Literal);
      }
    }

    public char LastChar
    {
      get
      {
        if (!this.IsLiteral)
          return this.RawBuffer[this.RawOffset + this.RawLength - 1];
        return Token.LiteralLastChar(this.Literal);
      }
    }

    public bool IsAnyWhitespace
    {
      get
      {
        return this.TextType <= RunTextType.UnusualWhitespace;
      }
    }

    internal TokenRun(Token token)
    {
      this.token = token;
    }

    public int ReadLiteral(char[] buffer)
    {
      int literal = this.token.RunList[this.token.WholePosition.Run].Value;
      if (Token.LiteralLength(literal) == 1)
      {
        buffer[0] = (char) literal;
        return 1;
      }
      buffer[0] = Token.LiteralFirstChar(literal);
      buffer[1] = Token.LiteralLastChar(literal);
      return 2;
    }

    public string GetString(int maxSize)
    {
      int run = this.token.WholePosition.Run;
      Token.RunEntry[] runEntryArray = this.token.RunList;
      switch (runEntryArray[run].Type)
      {
        case RunType.Normal:
          return new string(this.token.Buffer, this.token.WholePosition.RunOffset, Math.Min(maxSize, runEntryArray[run].Length));
        case RunType.Literal:
          if (this.Length == 1)
            return this.FirstChar.ToString();
          Token.Fragment fragment = new Token.Fragment();
          fragment.Initialize(run, this.token.WholePosition.RunOffset);
          fragment.Tail = fragment.Head + 1;
          return this.token.GetString(ref fragment, maxSize);
        default:
          return string.Empty;
      }
    }

    [Conditional("DEBUG")]
    private void AssertCurrent()
    {
    }
  }
}
