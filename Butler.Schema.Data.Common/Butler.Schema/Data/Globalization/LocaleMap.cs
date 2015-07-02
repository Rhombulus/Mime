// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.LocaleMap
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.Linq;

namespace Butler.Schema.Data.Globalization
{
  internal static class LocaleMap
  {
    public static CultureInfo GetCultureFromLcid(int lcid)
    {
      return CultureInfo.GetCultureInfo(lcid);
    }

    public static int GetLcidFromCulture(CultureInfo culture)
    {
      return culture.LCID;
    }

    public static int GetCompareLcidFromCulture(CultureInfo culture)
    {
      return culture.CompareInfo.LCID;
    }

    public static int GetANSICodePage(CultureInfo culture)
    {
      return culture.TextInfo.ANSICodePage;
    }

    public static CultureInfo GetSpecificCulture(string cultureName)
    {
      return CultureInfo.CreateSpecificCulture(cultureName);
    }
  }
}
