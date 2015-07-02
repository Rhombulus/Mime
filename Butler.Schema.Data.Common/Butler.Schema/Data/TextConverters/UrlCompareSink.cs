// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.UrlCompareSink
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class UrlCompareSink : ITextSink
  {
    private string url;
    private int urlPosition;

    public bool IsActive
    {
      get
      {
        return this.urlPosition >= 0;
      }
    }

    public bool IsMatch
    {
      get
      {
        return this.urlPosition == this.url.Length;
      }
    }

    public bool IsEnough
    {
      get
      {
        return this.urlPosition < 0;
      }
    }

    public void Initialize(string url)
    {
      this.url = url;
      this.urlPosition = 0;
    }

    public void Reset()
    {
      this.urlPosition = -1;
    }

    public void Write(char[] buffer, int offset, int count)
    {
      if (!this.IsActive)
        return;
      int num = offset + count;
      while (offset < num)
      {
        if (this.urlPosition == 0)
        {
          if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
          {
            ++offset;
            continue;
          }
        }
        else if (this.urlPosition == this.url.Length)
        {
          if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
          {
            ++offset;
            continue;
          }
          this.urlPosition = -1;
          break;
        }
        if ((int) buffer[offset] != (int) this.url[this.urlPosition])
        {
          this.urlPosition = -1;
          break;
        }
        ++offset;
        ++this.urlPosition;
      }
    }

    public void Write(int ucs32Char)
    {
      if (Token.LiteralLength(ucs32Char) != 1)
      {
        this.urlPosition = -1;
      }
      else
      {
        if (this.urlPosition == 0)
        {
          if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char) ucs32Char)))
            return;
        }
        else if (this.urlPosition == this.url.Length)
        {
          if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char) ucs32Char)))
            return;
          this.urlPosition = -1;
          return;
        }
        if ((int) (ushort) ucs32Char != (int) this.url[this.urlPosition])
          this.urlPosition = -1;
        else
          ++this.urlPosition;
      }
    }
  }
}
