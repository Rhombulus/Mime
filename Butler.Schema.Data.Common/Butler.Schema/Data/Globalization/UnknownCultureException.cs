// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.UnknownCultureException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Butler.Schema.Data.Globalization
{
  [Serializable]
  public class UnknownCultureException : ExchangeDataException
  {
    private int localeId;
    private string cultureName;

    public int LocaleId
    {
      get
      {
        return this.localeId;
      }
    }

    public string CultureName
    {
      get
      {
        return this.cultureName;
      }
    }

    public UnknownCultureException(int localeId)
      : base(CtsResources.GlobalizationStrings.InvalidLocaleId(localeId))
    {
      this.localeId = localeId;
    }

    public UnknownCultureException(string cultureName)
      : base(CtsResources.GlobalizationStrings.InvalidCultureName(cultureName == null ? "<null>" : cultureName))
    {
      this.cultureName = cultureName;
    }

    public UnknownCultureException(int localeId, string message)
      : base(message)
    {
      this.localeId = localeId;
    }

    public UnknownCultureException(string cultureName, string message)
      : base(message)
    {
      this.cultureName = cultureName;
    }

    public UnknownCultureException(int localeId, string message, Exception innerException)
      : base(message, innerException)
    {
      this.localeId = localeId;
    }

    public UnknownCultureException(string cultureName, string message, Exception innerException)
      : base(message, innerException)
    {
      this.cultureName = cultureName;
    }

    protected UnknownCultureException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.localeId = info.GetInt32("localeId");
      this.cultureName = info.GetString("cultureName");
    }
  }
}
