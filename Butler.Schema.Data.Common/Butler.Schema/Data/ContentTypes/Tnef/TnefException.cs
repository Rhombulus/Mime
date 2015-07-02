// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  [Serializable]
  public class TnefException : ExchangeDataException
  {
    public TnefException(string message)
      : base(message)
    {
    }

    public TnefException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected TnefException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
