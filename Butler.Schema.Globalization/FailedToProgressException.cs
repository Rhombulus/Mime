// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.FailedToProgressException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Butler.Schema.Data.Globalization
{
  [Serializable]
  public class FailedToProgressException : ButlerSchemaException
  {
    public FailedToProgressException(string message)
      : base(message)
    {
    }

    protected FailedToProgressException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
