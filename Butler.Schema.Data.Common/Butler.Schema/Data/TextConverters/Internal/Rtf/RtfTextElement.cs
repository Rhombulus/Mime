// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfTextElement
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal struct RtfTextElement
  {
    private RtfToken token;

    public RunTextType TextType
    {
      get
      {
        return this.token.ElementTextType;
      }
    }

    public TextMapping TextMapping
    {
      get
      {
        return this.token.TextMapping;
      }
    }

    public int Length
    {
      get
      {
        return this.RawLength;
      }
    }

    public bool IsAnyWhitespace
    {
      get
      {
        return this.TextType <= RunTextType.UnusualWhitespace;
      }
    }

    public bool IsLiteral
    {
      get
      {
        return false;
      }
    }

    public char[] RawBuffer
    {
      get
      {
        return this.token.CharBuffer;
      }
    }

    public int RawOffset
    {
      get
      {
        return this.token.ElementOffset;
      }
    }

    public int RawLength
    {
      get
      {
        return this.token.ElementLength;
      }
    }

    public int Literal
    {
      get
      {
        return 0;
      }
    }

    public int Value
    {
      get
      {
        return 0;
      }
    }

    public bool Eof
    {
      get
      {
        return true;
      }
    }

    internal RtfTextElement(RtfToken token)
    {
      this.token = token;
    }

    public int Read(char[] buffer, int offset, int count)
    {
      return 0;
    }

    public void WriteTo(ITextSink sink)
    {
      sink.Write(this.token.CharBuffer, this.token.ElementOffset, this.token.ElementLength);
    }

    public string GetString(int maxLength)
    {
      return new string(this.token.CharBuffer, this.token.ElementOffset, this.token.ElementLength);
    }

    [Conditional("DEBUG")]
    private void AssertCurrent()
    {
    }
  }
}
