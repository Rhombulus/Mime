// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.CtsResources.GlobalizationStrings
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Butler.Schema.CtsResources
{
  internal static class GlobalizationStrings
  {
    private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);
    private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.GlobalizationStrings", ((Type) IntrospectionExtensions.GetTypeInfo(typeof (GlobalizationStrings))).Assembly);

    public static string MaxCharactersCannotBeNegative
    {
      get
      {
        return GlobalizationStrings.ResourceManager.GetString("MaxCharactersCannotBeNegative");
      }
    }

    public static string CountOutOfRange
    {
      get
      {
        return GlobalizationStrings.ResourceManager.GetString("CountOutOfRange");
      }
    }

    public static string PriorityListIncludesNonDetectableCodePage
    {
      get
      {
        return GlobalizationStrings.ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage");
      }
    }

    public static string OffsetOutOfRange
    {
      get
      {
        return GlobalizationStrings.ResourceManager.GetString("OffsetOutOfRange");
      }
    }

    public static string IndexOutOfRange
    {
      get
      {
        return GlobalizationStrings.ResourceManager.GetString("IndexOutOfRange");
      }
    }

    public static string CountTooLarge
    {
      get
      {
        return GlobalizationStrings.ResourceManager.GetString("CountTooLarge");
      }
    }

    static GlobalizationStrings()
    {
      GlobalizationStrings.stringIDs.Add(1308081499U, "MaxCharactersCannotBeNegative");
      GlobalizationStrings.stringIDs.Add(1590522975U, "CountOutOfRange");
      GlobalizationStrings.stringIDs.Add(1083457927U, "PriorityListIncludesNonDetectableCodePage");
      GlobalizationStrings.stringIDs.Add(3590683541U, "OffsetOutOfRange");
      GlobalizationStrings.stringIDs.Add(1226301788U, "IndexOutOfRange");
      GlobalizationStrings.stringIDs.Add(2746482960U, "CountTooLarge");
    }

    public static string InvalidCodePage(int codePage)
    {
      return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidCodePage"), (object) codePage);
    }

    public static string NotInstalledCodePage(int codePage)
    {
      return string.Format(GlobalizationStrings.ResourceManager.GetString("NotInstalledCodePage"), (object) codePage);
    }

    public static string InvalidCultureName(string cultureName)
    {
      return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidCultureName"), (object) cultureName);
    }

    public static string NotInstalledCharsetCodePage(int codePage, string charsetName)
    {
      return string.Format(GlobalizationStrings.ResourceManager.GetString("NotInstalledCharsetCodePage"), (object) codePage, (object) charsetName);
    }

    public static string NotInstalledCharset(string charsetName)
    {
      return string.Format(GlobalizationStrings.ResourceManager.GetString("NotInstalledCharset"), (object) charsetName);
    }

    public static string InvalidLocaleId(int localeId)
    {
      return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidLocaleId"), (object) localeId);
    }

    public static string InvalidCharset(string charsetName)
    {
      return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidCharset"), (object) charsetName);
    }

    public static string GetLocalizedString(GlobalizationStrings.IDs key)
    {
      return GlobalizationStrings.ResourceManager.GetString(GlobalizationStrings.stringIDs[(uint) key]);
    }

    public enum IDs : uint
    {
      PriorityListIncludesNonDetectableCodePage = 1083457927U,
      IndexOutOfRange = 1226301788U,
      MaxCharactersCannotBeNegative = 1308081499U,
      CountOutOfRange = 1590522975U,
      CountTooLarge = 2746482960U,
      OffsetOutOfRange = 3590683541U,
    }

    private enum ParamIDs
    {
      InvalidCodePage,
      NotInstalledCodePage,
      InvalidCultureName,
      NotInstalledCharsetCodePage,
      NotInstalledCharset,
      InvalidLocaleId,
      InvalidCharset,
    }
  }
}
