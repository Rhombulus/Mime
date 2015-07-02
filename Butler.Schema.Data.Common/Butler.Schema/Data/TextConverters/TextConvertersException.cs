// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TextConvertersException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Butler.Schema.Data.TextConverters
{
  [Serializable]
  public class TextConvertersException : ExchangeDataException
  {
    internal TextConvertersException()
      : base("internal text conversion error (document too complex)")
    {
    }

    public TextConvertersException(string message)
      : base(message)
    {
    }

    public TextConvertersException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected TextConvertersException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
