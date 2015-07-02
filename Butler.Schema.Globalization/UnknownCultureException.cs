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
  public class UnknownCultureException : ButlerSchemaException
  {

      public int LocaleId { get; }

      public string CultureName { get; }

      public UnknownCultureException(int localeId)
      : base(Resources.GlobalizationStrings.InvalidLocaleId(localeId))
    {
      this.LocaleId = localeId;
    }

    public UnknownCultureException(string cultureName)
      : base(Resources.GlobalizationStrings.InvalidCultureName(cultureName == null ? "<null>" : cultureName))
    {
      this.CultureName = cultureName;
    }

    public UnknownCultureException(int localeId, string message)
      : base(message)
    {
      this.LocaleId = localeId;
    }

    public UnknownCultureException(string cultureName, string message)
      : base(message)
    {
      this.CultureName = cultureName;
    }

    public UnknownCultureException(int localeId, string message, Exception innerException)
      : base(message, innerException)
    {
      this.LocaleId = localeId;
    }

    public UnknownCultureException(string cultureName, string message, Exception innerException)
      : base(message, innerException)
    {
      this.CultureName = cultureName;
    }

    protected UnknownCultureException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.LocaleId = info.GetInt32("localeId");
      this.CultureName = info.GetString("cultureName");
    }
  }
}
