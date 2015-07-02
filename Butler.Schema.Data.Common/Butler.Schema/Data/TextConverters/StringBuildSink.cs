// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.StringBuildSink
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  internal class StringBuildSink : ITextSinkEx, ITextSink
  {
    private StringBuilder sb;
    private int maxLength;

    public bool IsEnough => this.sb.Length >= this.maxLength;

      public StringBuildSink()
    {
      this.sb = new StringBuilder();
    }

    public void Reset(int maxLength)
    {
      this.maxLength = maxLength;
      this.sb.Length = 0;
    }

    public void Write(char[] buffer, int offset, int count)
    {
      count = Math.Min(count, this.maxLength - this.sb.Length);
      this.sb.Append(buffer, offset, count);
    }

    public void Write(int ucs32Char)
    {
      if (Token.LiteralLength(ucs32Char) == 1)
      {
        this.sb.Append((char) ucs32Char);
      }
      else
      {
        this.sb.Append(Token.LiteralFirstChar(ucs32Char));
        if (this.IsEnough)
          return;
        this.sb.Append(Token.LiteralLastChar(ucs32Char));
      }
    }

    public void Write(string value)
    {
      this.sb.Append(value);
    }

    public void WriteNewLine()
    {
      this.sb.Append('\r');
      if (this.IsEnough)
        return;
      this.sb.Append('\n');
    }

    public override string ToString()
    {
      return this.sb.ToString();
    }
  }
}
