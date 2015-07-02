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

      public int CodePage { get; }

      public string CharsetName { get; }

      public InvalidCharsetException(int codePage)
      : base(CtsResources.GlobalizationStrings.InvalidCodePage(codePage))
    {
      this.CodePage = codePage;
    }

    public InvalidCharsetException(string charsetName)
      : base(CtsResources.GlobalizationStrings.InvalidCharset(charsetName == null ? "<null>" : charsetName))
    {
      this.CharsetName = charsetName;
    }

    public InvalidCharsetException(int codePage, string message)
      : base(message)
    {
      this.CodePage = codePage;
    }

    public InvalidCharsetException(string charsetName, string message)
      : base(message)
    {
      this.CharsetName = charsetName;
    }

    internal InvalidCharsetException(string charsetName, int codePage, string message)
      : base(message)
    {
      this.CodePage = codePage;
      this.CharsetName = charsetName;
    }

    public InvalidCharsetException(int codePage, string message, Exception innerException)
      : base(message, innerException)
    {
      this.CodePage = codePage;
    }

    public InvalidCharsetException(string charsetName, string message, Exception innerException)
      : base(message, innerException)
    {
      this.CharsetName = charsetName;
    }

    protected InvalidCharsetException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.CodePage = info.GetInt32("codePage");
      this.CharsetName = info.GetString("charsetName");
    }
  }
}
