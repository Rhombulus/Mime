// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.InvalidCharsetException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Butler.Schema.Data.Globalization
{
  [Serializable]
  public class InvalidCharsetException : ExchangeDataException
  {
    private int codePage;
    private string charsetName;

    public int CodePage => this.codePage;

      public string CharsetName => this.charsetName;

      public InvalidCharsetException(int codePage)
      : base(CtsResources.GlobalizationStrings.InvalidCodePage(codePage))
    {
      this.codePage = codePage;
    }

    public InvalidCharsetException(string charsetName)
      : base(CtsResources.GlobalizationStrings.InvalidCharset(charsetName == null ? "<null>" : charsetName))
    {
      this.charsetName = charsetName;
    }

    public InvalidCharsetException(int codePage, string message)
      : base(message)
    {
      this.codePage = codePage;
    }

    public InvalidCharsetException(string charsetName, string message)
      : base(message)
    {
      this.charsetName = charsetName;
    }

    internal InvalidCharsetException(string charsetName, int codePage, string message)
      : base(message)
    {
      this.codePage = codePage;
      this.charsetName = charsetName;
    }

    public InvalidCharsetException(int codePage, string message, Exception innerException)
      : base(message, innerException)
    {
      this.codePage = codePage;
    }

    public InvalidCharsetException(string charsetName, string message, Exception innerException)
      : base(message, innerException)
    {
      this.charsetName = charsetName;
    }

    protected InvalidCharsetException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.codePage = info.GetInt32("codePage");
      this.charsetName = info.GetString("charsetName");
    }
  }
}
