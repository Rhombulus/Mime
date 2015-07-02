// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.CharsetNotInstalledException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Butler.Schema.Data.Globalization
{
  [Serializable]
  public class CharsetNotInstalledException : InvalidCharsetException
  {
    public CharsetNotInstalledException(int codePage)
      : base(codePage, Resources.GlobalizationStrings.NotInstalledCodePage(codePage))
    {
    }

    public CharsetNotInstalledException(string charsetName)
      : base(charsetName, Resources.GlobalizationStrings.NotInstalledCharset(charsetName == null ? "<null>" : charsetName))
    {
    }

    internal CharsetNotInstalledException(string charsetName, int codePage)
      : base(charsetName, codePage, Resources.GlobalizationStrings.NotInstalledCharsetCodePage(codePage, charsetName == null ? "<null>" : charsetName))
    {
    }

    public CharsetNotInstalledException(int codePage, string message)
      : base(codePage, message)
    {
    }

    public CharsetNotInstalledException(string charsetName, string message)
      : base(charsetName, message)
    {
    }

    public CharsetNotInstalledException(int codePage, string message, Exception innerException)
      : base(codePage, message, innerException)
    {
    }

    public CharsetNotInstalledException(string charsetName, string message, Exception innerException)
      : base(charsetName, message, innerException)
    {
    }

    protected CharsetNotInstalledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
