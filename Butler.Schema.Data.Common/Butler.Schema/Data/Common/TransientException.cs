// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.TransientException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Butler.Schema.Data.Common
{
  [Serializable]
  public class TransientException : LocalizedException
  {
    private LocalizedString localizedString;

    public new LocalizedString LocalizedString => this.localizedString;

      public TransientException(LocalizedString localizedString)
      : this(localizedString, (Exception) null)
    {
    }

    public TransientException(LocalizedString localizedString, Exception innerException)
      : base(localizedString, innerException)
    {
      this.localizedString = localizedString;
    }

    protected TransientException(SerializationInfo serializationInfo, StreamingContext context)
      : base(serializationInfo, context)
    {
    }
  }
}
