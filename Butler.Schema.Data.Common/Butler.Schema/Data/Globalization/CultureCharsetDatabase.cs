// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.CultureCharsetDatabase
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Globalization
{
  [Serializable]
  internal static class CultureCharsetDatabase
  {
    internal static CultureCharsetDatabase.GlobalizationData Data = CultureCharsetDatabase.LoadGlobalizationData((string) null);
    private static readonly CultureCharsetDatabase.IntComparer IntComparerInstance = new CultureCharsetDatabase.IntComparer();

    internal static void RefreshConfiguration(string defaultCultureName)
    {
      CultureCharsetDatabase.Data = CultureCharsetDatabase.LoadGlobalizationData(defaultCultureName);
    }

    internal static int[] GetCultureSpecificCodepageDetectionPriorityOrder(Culture culture, int[] originalPriorityList)
    {
      int[] list = new int[CodePageDetectData.codePages.Length];
      int num1 = 0;
      int[] numArray = list;
      int index1 = num1;
      int num2 = 1;
      int listCount = index1 + num2;
      int num3 = 20127;
      numArray[index1] = num3;
      if (culture.MimeCharset.IsDetectable && !CultureCharsetDatabase.IsDbcs(culture.MimeCharset.CodePage) && !CultureCharsetDatabase.InList(culture.MimeCharset.CodePage, list, listCount))
        list[listCount++] = culture.MimeCharset.CodePage;
      if (culture.WebCharset.IsDetectable && !CultureCharsetDatabase.IsDbcs(culture.WebCharset.CodePage) && !CultureCharsetDatabase.InList(culture.WebCharset.CodePage, list, listCount))
        list[listCount++] = culture.WebCharset.CodePage;
      if (culture.WindowsCharset.IsDetectable && !CultureCharsetDatabase.IsDbcs(culture.WindowsCharset.CodePage) && !CultureCharsetDatabase.InList(culture.WindowsCharset.CodePage, list, listCount))
        list[listCount++] = culture.WindowsCharset.CodePage;
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs(originalPriorityList[index2]) && !CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount) && CultureCharsetDatabase.IsSameLanguage(originalPriorityList[index2], culture.WindowsCharset.CodePage))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs((int) CodePageDetectData.codePages[index2].cpid) && !CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount) && CultureCharsetDatabase.IsSameLanguage((int) CodePageDetectData.codePages[index2].cpid, culture.WindowsCharset.CodePage))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs(originalPriorityList[index2]) && !CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs((int) CodePageDetectData.codePages[index2].cpid) && !CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (culture.MimeCharset.IsDetectable && CultureCharsetDatabase.IsDbcs(culture.MimeCharset.CodePage) && !CultureCharsetDatabase.InList(culture.MimeCharset.CodePage, list, listCount))
        list[listCount++] = culture.MimeCharset.CodePage;
      if (culture.WebCharset.IsDetectable && CultureCharsetDatabase.IsDbcs(culture.WebCharset.CodePage) && !CultureCharsetDatabase.InList(culture.WebCharset.CodePage, list, listCount))
        list[listCount++] = culture.WebCharset.CodePage;
      if (culture.WindowsCharset.IsDetectable && CultureCharsetDatabase.IsDbcs(culture.WindowsCharset.CodePage) && !CultureCharsetDatabase.InList(culture.WindowsCharset.CodePage, list, listCount))
        list[listCount++] = culture.WindowsCharset.CodePage;
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (CultureCharsetDatabase.IsDbcs(originalPriorityList[index2]) && !CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount) && CultureCharsetDatabase.IsSameLanguage(originalPriorityList[index2], culture.WindowsCharset.CodePage))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (CultureCharsetDatabase.IsDbcs((int) CodePageDetectData.codePages[index2].cpid) && !CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount) && CultureCharsetDatabase.IsSameLanguage((int) CodePageDetectData.codePages[index2].cpid, culture.WindowsCharset.CodePage))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (!CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (!CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (originalPriorityList == null)
        return list;
      for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
      {
        if (list[index2] != originalPriorityList[index2])
          return list;
      }
      return originalPriorityList;
    }

    internal static int[] GetAdjustedCodepageDetectionPriorityOrder(Charset charset, int[] originalPriorityList)
    {
      if (!charset.IsDetectable && originalPriorityList != null)
        return originalPriorityList;
      int[] list = new int[CodePageDetectData.codePages.Length];
      int num1 = 0;
      int[] numArray = list;
      int index1 = num1;
      int num2 = 1;
      int listCount = index1 + num2;
      int num3 = 20127;
      numArray[index1] = num3;
      if (charset.IsDetectable && !CultureCharsetDatabase.IsDbcs(charset.CodePage) && !CultureCharsetDatabase.InList(charset.CodePage, list, listCount))
        list[listCount++] = charset.CodePage;
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs(originalPriorityList[index2]) && !CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount) && CultureCharsetDatabase.IsSameLanguage(originalPriorityList[index2], charset.Culture.WindowsCharset.CodePage))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs((int) CodePageDetectData.codePages[index2].cpid) && !CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount) && CultureCharsetDatabase.IsSameLanguage((int) CodePageDetectData.codePages[index2].cpid, charset.Culture.WindowsCharset.CodePage))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs(originalPriorityList[index2]) && !CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (!CultureCharsetDatabase.IsDbcs((int) CodePageDetectData.codePages[index2].cpid) && !CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (charset.IsDetectable && CultureCharsetDatabase.IsDbcs(charset.CodePage) && !CultureCharsetDatabase.InList(charset.CodePage, list, listCount))
        list[listCount++] = charset.CodePage;
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (CultureCharsetDatabase.IsDbcs(originalPriorityList[index2]) && !CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount) && CultureCharsetDatabase.IsSameLanguage(originalPriorityList[index2], charset.Culture.WindowsCharset.CodePage))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (CultureCharsetDatabase.IsDbcs((int) CodePageDetectData.codePages[index2].cpid) && !CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount) && CultureCharsetDatabase.IsSameLanguage((int) CodePageDetectData.codePages[index2].cpid, charset.Culture.WindowsCharset.CodePage))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (originalPriorityList != null)
      {
        for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
        {
          if (!CultureCharsetDatabase.InList(originalPriorityList[index2], list, listCount))
            list[listCount++] = originalPriorityList[index2];
        }
      }
      else
      {
        for (int index2 = 0; index2 < CodePageDetectData.codePages.Length; ++index2)
        {
          if (!CultureCharsetDatabase.InList((int) CodePageDetectData.codePages[index2].cpid, list, listCount))
            list[listCount++] = (int) CodePageDetectData.codePages[index2].cpid;
        }
      }
      if (originalPriorityList == null)
        return list;
      for (int index2 = 0; index2 < originalPriorityList.Length; ++index2)
      {
        if (list[index2] != originalPriorityList[index2])
          return list;
      }
      return originalPriorityList;
    }

    private static bool IsDbcs(int codePage)
    {
      switch (codePage)
      {
        case 51949:
        case 52936:
        case 50225:
        case 51932:
        case 949:
        case 950:
        case 50220:
        case 932:
        case 936:
          return true;
        default:
          return false;
      }
    }

    private static bool InList(int codePage, int[] list, int listCount)
    {
      for (int index = 0; index < listCount; ++index)
      {
        if (list[index] == codePage)
          return true;
      }
      return false;
    }

    private static bool IsSameLanguage(int codePage, int windowsCodePage)
    {
      if (windowsCodePage != codePage && (windowsCodePage != 1250 || codePage != 28592) && (windowsCodePage != 1251 || codePage != 28595 && codePage != 20866 && codePage != 21866) && ((windowsCodePage != 1252 || codePage != 28591 && codePage != 28605) && ((windowsCodePage != 1253 || codePage != 28597) && (windowsCodePage != 1254 || codePage != 28599)) && ((windowsCodePage != 1255 || codePage != 38598) && (windowsCodePage != 1256 || codePage != 28596) && (windowsCodePage != 1257 || codePage != 28594))) && ((windowsCodePage != 932 || codePage != 50220 && codePage != 51932) && (windowsCodePage != 949 || codePage != 50225 && codePage != 51949)))
        return windowsCodePage == 936 & codePage == 52936;
      return true;
    }

    private static CultureCharsetDatabase.GlobalizationData LoadGlobalizationData(string defaultCultureName)
    {
      CultureCharsetDatabase.WindowsCodePage[] windowsCodePageArray = new CultureCharsetDatabase.WindowsCodePage[15]
      {
        new CultureCharsetDatabase.WindowsCodePage(1200, "unicode", 0, (string) null, 65001, 65001, "Unicode generic culture"),
        new CultureCharsetDatabase.WindowsCodePage(1250, "windows-1250", 0, (string) null, 28592, 1250, "Central European generic culture"),
        new CultureCharsetDatabase.WindowsCodePage(1251, "windows-1251", 0, (string) null, 20866, 1251, "Cyrillic generic culture"),
        new CultureCharsetDatabase.WindowsCodePage(1252, "windows-1252", 0, (string) null, 28591, 1252, "Western European generic culture"),
        new CultureCharsetDatabase.WindowsCodePage(1253, "windows-1253", 8, "el", 28597, 1253, (string) null),
        new CultureCharsetDatabase.WindowsCodePage(1254, "windows-1254", 0, (string) null, 28599, 1254, "Turkish / Azeri generic culture"),
        new CultureCharsetDatabase.WindowsCodePage(1255, "windows-1255", 13, "he", 1255, 1255, (string) null),
        new CultureCharsetDatabase.WindowsCodePage(1256, "windows-1256", 0, (string) null, 1256, 1256, "Arabic generic culture"),
        new CultureCharsetDatabase.WindowsCodePage(1257, "windows-1257", 0, (string) null, 1257, 1257, "Baltic generic culture"),
        new CultureCharsetDatabase.WindowsCodePage(1258, "windows-1258", 42, "vi", 1258, 1258, (string) null),
        new CultureCharsetDatabase.WindowsCodePage(874, "windows-874", 30, "th", 874, 874, (string) null),
        new CultureCharsetDatabase.WindowsCodePage(932, "windows-932", 17, "ja", 50220, 932, (string) null),
        new CultureCharsetDatabase.WindowsCodePage(936, "windows-936", 4, "zh-CHS", 936, 936, (string) null),
        new CultureCharsetDatabase.WindowsCodePage(949, "windows-949", 18, "ko", 949, 949, (string) null),
        new CultureCharsetDatabase.WindowsCodePage(950, "windows-950", 31748, "zh-CHT", 950, 950, (string) null)
      };
      CultureCharsetDatabase.CodePageCultureOverride[] pageCultureOverrideArray = new CultureCharsetDatabase.CodePageCultureOverride[35]
      {
        new CultureCharsetDatabase.CodePageCultureOverride(37, "en"),
        new CultureCharsetDatabase.CodePageCultureOverride(437, "en"),
        new CultureCharsetDatabase.CodePageCultureOverride(860, "pt"),
        new CultureCharsetDatabase.CodePageCultureOverride(861, "is"),
        new CultureCharsetDatabase.CodePageCultureOverride(863, "fr-CA"),
        new CultureCharsetDatabase.CodePageCultureOverride(1141, "de"),
        new CultureCharsetDatabase.CodePageCultureOverride(1144, "it"),
        new CultureCharsetDatabase.CodePageCultureOverride(1145, "es"),
        new CultureCharsetDatabase.CodePageCultureOverride(1146, "en-GB"),
        new CultureCharsetDatabase.CodePageCultureOverride(1147, "fr"),
        new CultureCharsetDatabase.CodePageCultureOverride(1149, "is"),
        new CultureCharsetDatabase.CodePageCultureOverride(10010, "ro"),
        new CultureCharsetDatabase.CodePageCultureOverride(10017, "uk"),
        new CultureCharsetDatabase.CodePageCultureOverride(10079, "is"),
        new CultureCharsetDatabase.CodePageCultureOverride(10082, "hr"),
        new CultureCharsetDatabase.CodePageCultureOverride(20106, "de"),
        new CultureCharsetDatabase.CodePageCultureOverride(20107, "sv"),
        new CultureCharsetDatabase.CodePageCultureOverride(20108, "no"),
        new CultureCharsetDatabase.CodePageCultureOverride(20127, "en"),
        new CultureCharsetDatabase.CodePageCultureOverride(20273, "de"),
        new CultureCharsetDatabase.CodePageCultureOverride(20280, "it"),
        new CultureCharsetDatabase.CodePageCultureOverride(20284, "es"),
        new CultureCharsetDatabase.CodePageCultureOverride(20285, "en-GB"),
        new CultureCharsetDatabase.CodePageCultureOverride(20297, "fr"),
        new CultureCharsetDatabase.CodePageCultureOverride(20866, "ru"),
        new CultureCharsetDatabase.CodePageCultureOverride(20871, "is"),
        new CultureCharsetDatabase.CodePageCultureOverride(20880, "ru"),
        new CultureCharsetDatabase.CodePageCultureOverride(21866, "uk"),
        new CultureCharsetDatabase.CodePageCultureOverride(57003, "bn-IN"),
        new CultureCharsetDatabase.CodePageCultureOverride(57004, "ta"),
        new CultureCharsetDatabase.CodePageCultureOverride(57005, "te"),
        new CultureCharsetDatabase.CodePageCultureOverride(57008, "kn"),
        new CultureCharsetDatabase.CodePageCultureOverride(57009, "ml-IN"),
        new CultureCharsetDatabase.CodePageCultureOverride(57010, "gu"),
        new CultureCharsetDatabase.CodePageCultureOverride(57011, "pa")
      };
      CultureCharsetDatabase.CultureCodePageOverride[] codePageOverrideArray = new CultureCharsetDatabase.CultureCodePageOverride[16]
      {
        new CultureCharsetDatabase.CultureCodePageOverride("et", 28605, 28605),
        new CultureCharsetDatabase.CultureCodePageOverride("lt", 28603, 28603),
        new CultureCharsetDatabase.CultureCodePageOverride("lv", 28603, 28603),
        new CultureCharsetDatabase.CultureCodePageOverride("uk", 21866, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("az-AZ-Cyrl", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("be", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("bg", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("mk", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("sr", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("sr-BA-Cyrl", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("sr-Cyrl-CS", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("ky", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("kk", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("tt", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("uz-UZ-Cyrl", 1251, 1251),
        new CultureCharsetDatabase.CultureCodePageOverride("mn", 65001, 65001)
      };
      CultureCharsetDatabase.CharsetName[] charsetNameArray = new CultureCharsetDatabase.CharsetName[431]
      {
        new CultureCharsetDatabase.CharsetName("_autodetect", 50932),
        new CultureCharsetDatabase.CharsetName("_autodetect_all", 50001),
        new CultureCharsetDatabase.CharsetName("_autodetect_kr", 50949),
        new CultureCharsetDatabase.CharsetName("_iso-2022-jp$ESC", 50221),
        new CultureCharsetDatabase.CharsetName("_iso-2022-jp$SIO", 50222),
        new CultureCharsetDatabase.CharsetName("437", 437),
        new CultureCharsetDatabase.CharsetName("ANSI_X3.4-1968", 20127),
        new CultureCharsetDatabase.CharsetName("ANSI_X3.4-1986", 20127),
        new CultureCharsetDatabase.CharsetName("arabic", 28596),
        new CultureCharsetDatabase.CharsetName("ascii", 20127),
        new CultureCharsetDatabase.CharsetName("ASMO-708", 708),
        new CultureCharsetDatabase.CharsetName("Big5-HKSCS", 950),
        new CultureCharsetDatabase.CharsetName("Big5", 950),
        new CultureCharsetDatabase.CharsetName("CCSID00858", 858),
        new CultureCharsetDatabase.CharsetName("CCSID00924", 20924),
        new CultureCharsetDatabase.CharsetName("CCSID01140", 1140),
        new CultureCharsetDatabase.CharsetName("CCSID01141", 1141),
        new CultureCharsetDatabase.CharsetName("CCSID01142", 1142),
        new CultureCharsetDatabase.CharsetName("CCSID01143", 1143),
        new CultureCharsetDatabase.CharsetName("CCSID01144", 1144),
        new CultureCharsetDatabase.CharsetName("CCSID01145", 1145),
        new CultureCharsetDatabase.CharsetName("CCSID01146", 1146),
        new CultureCharsetDatabase.CharsetName("CCSID01147", 1147),
        new CultureCharsetDatabase.CharsetName("CCSID01148", 1148),
        new CultureCharsetDatabase.CharsetName("CCSID01149", 1149),
        new CultureCharsetDatabase.CharsetName("chinese", 936),
        new CultureCharsetDatabase.CharsetName("cn-big5", 950),
        new CultureCharsetDatabase.CharsetName("CN-GB", 936),
        new CultureCharsetDatabase.CharsetName("CP00858", 858),
        new CultureCharsetDatabase.CharsetName("CP00924", 20924),
        new CultureCharsetDatabase.CharsetName("CP01140", 1140),
        new CultureCharsetDatabase.CharsetName("CP01141", 1141),
        new CultureCharsetDatabase.CharsetName("CP01142", 1142),
        new CultureCharsetDatabase.CharsetName("CP01143", 1143),
        new CultureCharsetDatabase.CharsetName("CP01144", 1144),
        new CultureCharsetDatabase.CharsetName("CP01145", 1145),
        new CultureCharsetDatabase.CharsetName("CP01146", 1146),
        new CultureCharsetDatabase.CharsetName("CP01147", 1147),
        new CultureCharsetDatabase.CharsetName("CP01148", 1148),
        new CultureCharsetDatabase.CharsetName("CP01149", 1149),
        new CultureCharsetDatabase.CharsetName("cp037", 37),
        new CultureCharsetDatabase.CharsetName("cp1025", 21025),
        new CultureCharsetDatabase.CharsetName("CP1026", 1026),
        new CultureCharsetDatabase.CharsetName("cp1256", 1256),
        new CultureCharsetDatabase.CharsetName("CP273", 20273),
        new CultureCharsetDatabase.CharsetName("CP278", 20278),
        new CultureCharsetDatabase.CharsetName("CP280", 20280),
        new CultureCharsetDatabase.CharsetName("CP284", 20284),
        new CultureCharsetDatabase.CharsetName("CP285", 20285),
        new CultureCharsetDatabase.CharsetName("cp290", 20290),
        new CultureCharsetDatabase.CharsetName("cp297", 20297),
        new CultureCharsetDatabase.CharsetName("cp367", 20127),
        new CultureCharsetDatabase.CharsetName("cp420", 20420),
        new CultureCharsetDatabase.CharsetName("cp423", 20423),
        new CultureCharsetDatabase.CharsetName("cp424", 20424),
        new CultureCharsetDatabase.CharsetName("cp437", 437),
        new CultureCharsetDatabase.CharsetName("CP500", 500),
        new CultureCharsetDatabase.CharsetName("cp50227", 50227),
        new CultureCharsetDatabase.CharsetName("cp50229", 50229),
        new CultureCharsetDatabase.CharsetName("cp819", 28591),
        new CultureCharsetDatabase.CharsetName("cp850", 850),
        new CultureCharsetDatabase.CharsetName("cp852", 852),
        new CultureCharsetDatabase.CharsetName("cp855", 855),
        new CultureCharsetDatabase.CharsetName("cp857", 857),
        new CultureCharsetDatabase.CharsetName("cp858", 858),
        new CultureCharsetDatabase.CharsetName("cp860", 860),
        new CultureCharsetDatabase.CharsetName("cp861", 861),
        new CultureCharsetDatabase.CharsetName("cp862", 862),
        new CultureCharsetDatabase.CharsetName("cp863", 863),
        new CultureCharsetDatabase.CharsetName("cp864", 864),
        new CultureCharsetDatabase.CharsetName("cp865", 865),
        new CultureCharsetDatabase.CharsetName("cp866", 866),
        new CultureCharsetDatabase.CharsetName("cp869", 869),
        new CultureCharsetDatabase.CharsetName("CP870", 870),
        new CultureCharsetDatabase.CharsetName("CP871", 20871),
        new CultureCharsetDatabase.CharsetName("cp875", 875),
        new CultureCharsetDatabase.CharsetName("cp880", 20880),
        new CultureCharsetDatabase.CharsetName("CP905", 20905),
        new CultureCharsetDatabase.CharsetName("cp930", 50930),
        new CultureCharsetDatabase.CharsetName("cp933", 50933),
        new CultureCharsetDatabase.CharsetName("cp935", 50935),
        new CultureCharsetDatabase.CharsetName("cp937", 50937),
        new CultureCharsetDatabase.CharsetName("cp939", 50939),
        new CultureCharsetDatabase.CharsetName("csASCII", 20127),
        new CultureCharsetDatabase.CharsetName("csbig5", 950),
        new CultureCharsetDatabase.CharsetName("csEUCKR", 51949),
        new CultureCharsetDatabase.CharsetName("csEUCPkdFmtJapanese", 51932),
        new CultureCharsetDatabase.CharsetName("csGB2312", 936),
        new CultureCharsetDatabase.CharsetName("csGB231280", 936),
        new CultureCharsetDatabase.CharsetName("csIBM037", 37),
        new CultureCharsetDatabase.CharsetName("csIBM1026", 1026),
        new CultureCharsetDatabase.CharsetName("csIBM273", 20273),
        new CultureCharsetDatabase.CharsetName("csIBM277", 20277),
        new CultureCharsetDatabase.CharsetName("csIBM278", 20278),
        new CultureCharsetDatabase.CharsetName("csIBM280", 20280),
        new CultureCharsetDatabase.CharsetName("csIBM284", 20284),
        new CultureCharsetDatabase.CharsetName("csIBM285", 20285),
        new CultureCharsetDatabase.CharsetName("csIBM290", 20290),
        new CultureCharsetDatabase.CharsetName("csIBM297", 20297),
        new CultureCharsetDatabase.CharsetName("csIBM420", 20420),
        new CultureCharsetDatabase.CharsetName("csIBM423", 20423),
        new CultureCharsetDatabase.CharsetName("csIBM424", 20424),
        new CultureCharsetDatabase.CharsetName("csIBM500", 500),
        new CultureCharsetDatabase.CharsetName("csIBM870", 870),
        new CultureCharsetDatabase.CharsetName("csIBM871", 20871),
        new CultureCharsetDatabase.CharsetName("csIBM880", 20880),
        new CultureCharsetDatabase.CharsetName("csIBM905", 20905),
        new CultureCharsetDatabase.CharsetName("csIBMThai", 20838),
        new CultureCharsetDatabase.CharsetName("csISO2022JP", 50221),
        new CultureCharsetDatabase.CharsetName("csISO2022KR", 50225),
        new CultureCharsetDatabase.CharsetName("csISO58GB231280", 936),
        new CultureCharsetDatabase.CharsetName("csISOLatin1", 28591),
        new CultureCharsetDatabase.CharsetName("csISOLatin2", 28592),
        new CultureCharsetDatabase.CharsetName("csISOLatin3", 28593),
        new CultureCharsetDatabase.CharsetName("csISOLatin4", 28594),
        new CultureCharsetDatabase.CharsetName("csISOLatin5", 28599),
        new CultureCharsetDatabase.CharsetName("csISOLatin9", 28605),
        new CultureCharsetDatabase.CharsetName("csISOLatinArabic", 28596),
        new CultureCharsetDatabase.CharsetName("csISOLatinCyrillic", 28595),
        new CultureCharsetDatabase.CharsetName("csISOLatinGreek", 28597),
        new CultureCharsetDatabase.CharsetName("csISOLatinHebrew", 38598),
        new CultureCharsetDatabase.CharsetName("csKOI8R", 20866),
        new CultureCharsetDatabase.CharsetName("csKSC56011987", 949),
        new CultureCharsetDatabase.CharsetName("csPC8CodePage437", 437),
        new CultureCharsetDatabase.CharsetName("csShiftJIS", 932),
        new CultureCharsetDatabase.CharsetName("csUnicode11UTF7", 65000),
        new CultureCharsetDatabase.CharsetName("csWindows31J", 932),
        new CultureCharsetDatabase.CharsetName("Windows-31J", 932),
        new CultureCharsetDatabase.CharsetName("cyrillic", 28595),
        new CultureCharsetDatabase.CharsetName("DIN_66003", 20106),
        new CultureCharsetDatabase.CharsetName("DOS-720", 720),
        new CultureCharsetDatabase.CharsetName("DOS-862", 862),
        new CultureCharsetDatabase.CharsetName("DOS-874", 874),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-ar1", 20420),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-be", 500),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-ca", 37),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-ch", 500),
        new CultureCharsetDatabase.CharsetName("EBCDIC-CP-DK", 20277),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-es", 20284),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-fi", 20278),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-fr", 20297),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-gb", 20285),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-gr", 20423),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-he", 20424),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-is", 20871),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-it", 20280),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-nl", 37),
        new CultureCharsetDatabase.CharsetName("EBCDIC-CP-NO", 20277),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-roece", 870),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-se", 20278),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-tr", 20905),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-us", 37),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-wt", 37),
        new CultureCharsetDatabase.CharsetName("ebcdic-cp-yu", 870),
        new CultureCharsetDatabase.CharsetName("EBCDIC-Cyrillic", 20880),
        new CultureCharsetDatabase.CharsetName("ebcdic-de-273+euro", 1141),
        new CultureCharsetDatabase.CharsetName("ebcdic-dk-277+euro", 1142),
        new CultureCharsetDatabase.CharsetName("ebcdic-es-284+euro", 1145),
        new CultureCharsetDatabase.CharsetName("ebcdic-fi-278+euro", 1143),
        new CultureCharsetDatabase.CharsetName("ebcdic-fr-297+euro", 1147),
        new CultureCharsetDatabase.CharsetName("ebcdic-gb-285+euro", 1146),
        new CultureCharsetDatabase.CharsetName("ebcdic-international-500+euro", 1148),
        new CultureCharsetDatabase.CharsetName("ebcdic-is-871+euro", 1149),
        new CultureCharsetDatabase.CharsetName("ebcdic-it-280+euro", 1144),
        new CultureCharsetDatabase.CharsetName("EBCDIC-JP-kana", 20290),
        new CultureCharsetDatabase.CharsetName("ebcdic-Latin9--euro", 20924),
        new CultureCharsetDatabase.CharsetName("ebcdic-no-277+euro", 1142),
        new CultureCharsetDatabase.CharsetName("ebcdic-se-278+euro", 1143),
        new CultureCharsetDatabase.CharsetName("ebcdic-us-37+euro", 1140),
        new CultureCharsetDatabase.CharsetName("ECMA-114", 28596),
        new CultureCharsetDatabase.CharsetName("ECMA-118", 28597),
        new CultureCharsetDatabase.CharsetName("ELOT_928", 28597),
        new CultureCharsetDatabase.CharsetName("euc-cn", 51936),
        new CultureCharsetDatabase.CharsetName("euc-jp", 51932),
        new CultureCharsetDatabase.CharsetName("euc-kr", 51949),
        new CultureCharsetDatabase.CharsetName("euc_kr", 51949),
        new CultureCharsetDatabase.CharsetName("Extended_UNIX_Code_Packed_Format_for_Japanese", 51932),
        new CultureCharsetDatabase.CharsetName("GB_2312-80", 936),
        new CultureCharsetDatabase.CharsetName("GB18030", 54936),
        new CultureCharsetDatabase.CharsetName("GB2312-80", 936),
        new CultureCharsetDatabase.CharsetName("GB2312", 936),
        new CultureCharsetDatabase.CharsetName("GB231280", 936),
        new CultureCharsetDatabase.CharsetName("GBK", 936),
        new CultureCharsetDatabase.CharsetName("German", 20106),
        new CultureCharsetDatabase.CharsetName("greek", 28597),
        new CultureCharsetDatabase.CharsetName("greek8", 28597),
        new CultureCharsetDatabase.CharsetName("hebrew", 38598),
        new CultureCharsetDatabase.CharsetName("hz-gb-2312", 52936),
        new CultureCharsetDatabase.CharsetName("IBM-Thai", 20838),
        new CultureCharsetDatabase.CharsetName("IBM00858", 858),
        new CultureCharsetDatabase.CharsetName("IBM00924", 20924),
        new CultureCharsetDatabase.CharsetName("IBM01047", 1047),
        new CultureCharsetDatabase.CharsetName("IBM01140", 1140),
        new CultureCharsetDatabase.CharsetName("IBM01141", 1141),
        new CultureCharsetDatabase.CharsetName("IBM01142", 1142),
        new CultureCharsetDatabase.CharsetName("IBM01143", 1143),
        new CultureCharsetDatabase.CharsetName("IBM01144", 1144),
        new CultureCharsetDatabase.CharsetName("IBM01145", 1145),
        new CultureCharsetDatabase.CharsetName("IBM01146", 1146),
        new CultureCharsetDatabase.CharsetName("IBM01147", 1147),
        new CultureCharsetDatabase.CharsetName("IBM01148", 1148),
        new CultureCharsetDatabase.CharsetName("IBM01149", 1149),
        new CultureCharsetDatabase.CharsetName("IBM037", 37),
        new CultureCharsetDatabase.CharsetName("IBM1026", 1026),
        new CultureCharsetDatabase.CharsetName("IBM273", 20273),
        new CultureCharsetDatabase.CharsetName("IBM277", 20277),
        new CultureCharsetDatabase.CharsetName("IBM278", 20278),
        new CultureCharsetDatabase.CharsetName("IBM280", 20280),
        new CultureCharsetDatabase.CharsetName("IBM284", 20284),
        new CultureCharsetDatabase.CharsetName("IBM285", 20285),
        new CultureCharsetDatabase.CharsetName("IBM290", 20290),
        new CultureCharsetDatabase.CharsetName("IBM297", 20297),
        new CultureCharsetDatabase.CharsetName("IBM367", 20127),
        new CultureCharsetDatabase.CharsetName("IBM420", 20420),
        new CultureCharsetDatabase.CharsetName("IBM423", 20423),
        new CultureCharsetDatabase.CharsetName("IBM424", 20424),
        new CultureCharsetDatabase.CharsetName("IBM437", 437),
        new CultureCharsetDatabase.CharsetName("IBM500", 500),
        new CultureCharsetDatabase.CharsetName("ibm737", 737),
        new CultureCharsetDatabase.CharsetName("ibm775", 775),
        new CultureCharsetDatabase.CharsetName("ibm819", 28591),
        new CultureCharsetDatabase.CharsetName("IBM850", 850),
        new CultureCharsetDatabase.CharsetName("IBM852", 852),
        new CultureCharsetDatabase.CharsetName("IBM855", 855),
        new CultureCharsetDatabase.CharsetName("IBM857", 857),
        new CultureCharsetDatabase.CharsetName("IBM860", 860),
        new CultureCharsetDatabase.CharsetName("IBM861", 861),
        new CultureCharsetDatabase.CharsetName("IBM862", 862),
        new CultureCharsetDatabase.CharsetName("IBM863", 863),
        new CultureCharsetDatabase.CharsetName("IBM864", 864),
        new CultureCharsetDatabase.CharsetName("IBM865", 865),
        new CultureCharsetDatabase.CharsetName("IBM866", 866),
        new CultureCharsetDatabase.CharsetName("IBM869", 869),
        new CultureCharsetDatabase.CharsetName("IBM870", 870),
        new CultureCharsetDatabase.CharsetName("IBM871", 20871),
        new CultureCharsetDatabase.CharsetName("IBM880", 20880),
        new CultureCharsetDatabase.CharsetName("IBM905", 20905),
        new CultureCharsetDatabase.CharsetName("irv", 20105),
        new CultureCharsetDatabase.CharsetName("ISO-10646-UCS-2", 1200),
        new CultureCharsetDatabase.CharsetName("iso-2022-jp", 50220),
        new CultureCharsetDatabase.CharsetName("iso-2022-jpdbcs", 50220),
        new CultureCharsetDatabase.CharsetName("iso-2022-jpesc", 50221),
        new CultureCharsetDatabase.CharsetName("iso-2022-jpsio", 50222),
        new CultureCharsetDatabase.CharsetName("iso-2022-jpeuc", 51932),
        new CultureCharsetDatabase.CharsetName("iso-2022-kr-7", 50225),
        new CultureCharsetDatabase.CharsetName("iso-2022-kr-7bit", 50225),
        new CultureCharsetDatabase.CharsetName("iso-2022-kr-8", 51949),
        new CultureCharsetDatabase.CharsetName("iso-2022-kr-8bit", 51949),
        new CultureCharsetDatabase.CharsetName("iso-2022-kr", 50225),
        new CultureCharsetDatabase.CharsetName("iso-8859-1", 28591),
        new CultureCharsetDatabase.CharsetName("iso-8859-11", 874),
        new CultureCharsetDatabase.CharsetName("iso-8859-13", 28603),
        new CultureCharsetDatabase.CharsetName("iso-8859-15", 28605),
        new CultureCharsetDatabase.CharsetName("iso-8859-2", 28592),
        new CultureCharsetDatabase.CharsetName("iso-8859-3", 28593),
        new CultureCharsetDatabase.CharsetName("iso-8859-4", 28594),
        new CultureCharsetDatabase.CharsetName("iso-8859-5", 28595),
        new CultureCharsetDatabase.CharsetName("iso-8859-6", 28596),
        new CultureCharsetDatabase.CharsetName("iso-8859-7", 28597),
        new CultureCharsetDatabase.CharsetName("iso-8859-8-i", 38598),
        new CultureCharsetDatabase.CharsetName("ISO-8859-8 Visual", 28598),
        new CultureCharsetDatabase.CharsetName("iso-8859-8", 28598),
        new CultureCharsetDatabase.CharsetName("iso-8859-9", 28599),
        new CultureCharsetDatabase.CharsetName("iso-ir-100", 28591),
        new CultureCharsetDatabase.CharsetName("iso-ir-101", 28592),
        new CultureCharsetDatabase.CharsetName("iso-ir-109", 28593),
        new CultureCharsetDatabase.CharsetName("iso-ir-110", 28594),
        new CultureCharsetDatabase.CharsetName("iso-ir-126", 28597),
        new CultureCharsetDatabase.CharsetName("iso-ir-127", 28596),
        new CultureCharsetDatabase.CharsetName("iso-ir-138", 38598),
        new CultureCharsetDatabase.CharsetName("iso-ir-144", 28595),
        new CultureCharsetDatabase.CharsetName("iso-ir-148", 28599),
        new CultureCharsetDatabase.CharsetName("iso-ir-149", 949),
        new CultureCharsetDatabase.CharsetName("iso-ir-58", 936),
        new CultureCharsetDatabase.CharsetName("iso-ir-6", 20127),
        new CultureCharsetDatabase.CharsetName("ISO_646.irv:1991", 20127),
        new CultureCharsetDatabase.CharsetName("iso_8859-1", 28591),
        new CultureCharsetDatabase.CharsetName("iso_8859-1:1987", 28591),
        new CultureCharsetDatabase.CharsetName("ISO_8859-15", 28605),
        new CultureCharsetDatabase.CharsetName("iso_8859-2", 28592),
        new CultureCharsetDatabase.CharsetName("iso_8859-2:1987", 28592),
        new CultureCharsetDatabase.CharsetName("ISO_8859-3", 28593),
        new CultureCharsetDatabase.CharsetName("ISO_8859-3:1988", 28593),
        new CultureCharsetDatabase.CharsetName("ISO_8859-4", 28594),
        new CultureCharsetDatabase.CharsetName("ISO_8859-4:1988", 28594),
        new CultureCharsetDatabase.CharsetName("ISO_8859-5", 28595),
        new CultureCharsetDatabase.CharsetName("ISO_8859-5:1988", 28595),
        new CultureCharsetDatabase.CharsetName("ISO_8859-6", 28596),
        new CultureCharsetDatabase.CharsetName("ISO_8859-6:1987", 28596),
        new CultureCharsetDatabase.CharsetName("ISO_8859-7", 28597),
        new CultureCharsetDatabase.CharsetName("ISO_8859-7:1987", 28597),
        new CultureCharsetDatabase.CharsetName("ISO_8859-8", 28598),
        new CultureCharsetDatabase.CharsetName("ISO_8859-8:1988", 28598),
        new CultureCharsetDatabase.CharsetName("ISO_8859-9", 28599),
        new CultureCharsetDatabase.CharsetName("ISO_8859-9:1989", 28599),
        new CultureCharsetDatabase.CharsetName("ISO646-US", 20127),
        new CultureCharsetDatabase.CharsetName("646", 20127),
        new CultureCharsetDatabase.CharsetName("iso8859-1", 28591),
        new CultureCharsetDatabase.CharsetName("iso8859-15", 28605),
        new CultureCharsetDatabase.CharsetName("iso8859-2", 28592),
        new CultureCharsetDatabase.CharsetName("iso8859_1", 28591),
        new CultureCharsetDatabase.CharsetName("Johab", 1361),
        new CultureCharsetDatabase.CharsetName("koi", 20866),
        new CultureCharsetDatabase.CharsetName("koi8-r", 20866),
        new CultureCharsetDatabase.CharsetName("koi8-ru", 21866),
        new CultureCharsetDatabase.CharsetName("koi8-u", 21866),
        new CultureCharsetDatabase.CharsetName("koi8", 20866),
        new CultureCharsetDatabase.CharsetName("koi8r", 20866),
        new CultureCharsetDatabase.CharsetName("korean", 949),
        new CultureCharsetDatabase.CharsetName("ks-c-5601", 949),
        new CultureCharsetDatabase.CharsetName("ks-c5601", 949),
        new CultureCharsetDatabase.CharsetName("ks_c_5601-1987", 949),
        new CultureCharsetDatabase.CharsetName("ks_c_5601-1989", 949),
        new CultureCharsetDatabase.CharsetName("ks_c_5601", 949),
        new CultureCharsetDatabase.CharsetName("ks_c_5601_1987", 949),
        new CultureCharsetDatabase.CharsetName("KSC_5601", 949),
        new CultureCharsetDatabase.CharsetName("KSC5601", 949),
        new CultureCharsetDatabase.CharsetName("l1", 28591),
        new CultureCharsetDatabase.CharsetName("l2", 28592),
        new CultureCharsetDatabase.CharsetName("l3", 28593),
        new CultureCharsetDatabase.CharsetName("l4", 28594),
        new CultureCharsetDatabase.CharsetName("l5", 28599),
        new CultureCharsetDatabase.CharsetName("l9", 28605),
        new CultureCharsetDatabase.CharsetName("latin1", 28591),
        new CultureCharsetDatabase.CharsetName("latin2", 28592),
        new CultureCharsetDatabase.CharsetName("latin3", 28593),
        new CultureCharsetDatabase.CharsetName("latin4", 28594),
        new CultureCharsetDatabase.CharsetName("latin5", 28599),
        new CultureCharsetDatabase.CharsetName("latin9", 28605),
        new CultureCharsetDatabase.CharsetName("logical", 38598),
        new CultureCharsetDatabase.CharsetName("macintosh", 10000),
        new CultureCharsetDatabase.CharsetName("MacRoman", 10000),
        new CultureCharsetDatabase.CharsetName("ms_Kanji", 932),
        new CultureCharsetDatabase.CharsetName("Norwegian", 20108),
        new CultureCharsetDatabase.CharsetName("NS_4551-1", 20108),
        new CultureCharsetDatabase.CharsetName("PC-Multilingual-850+euro", 858),
        new CultureCharsetDatabase.CharsetName("SEN_850200_B", 20107),
        new CultureCharsetDatabase.CharsetName("shift-jis", 932),
        new CultureCharsetDatabase.CharsetName("shift_jis", 932),
        new CultureCharsetDatabase.CharsetName("sjis", 932),
        new CultureCharsetDatabase.CharsetName("Swedish", 20107),
        new CultureCharsetDatabase.CharsetName("TIS-620", 874),
        new CultureCharsetDatabase.CharsetName("ucs-2", 1200),
        new CultureCharsetDatabase.CharsetName("unicode-1-1-utf-7", 65000),
        new CultureCharsetDatabase.CharsetName("unicode-1-1-utf-8", 65001),
        new CultureCharsetDatabase.CharsetName("unicode-2-0-utf-7", 65000),
        new CultureCharsetDatabase.CharsetName("unicode-2-0-utf-8", 65001),
        new CultureCharsetDatabase.CharsetName("unicode", 1200),
        new CultureCharsetDatabase.CharsetName("unicodeFFFE", 1201),
        new CultureCharsetDatabase.CharsetName("us-ascii", 20127),
        new CultureCharsetDatabase.CharsetName("us", 20127),
        new CultureCharsetDatabase.CharsetName("utf-16", 1200),
        new CultureCharsetDatabase.CharsetName("UTF-16BE", 1201),
        new CultureCharsetDatabase.CharsetName("UTF-16LE", 1200),
        new CultureCharsetDatabase.CharsetName("utf-32", 12000),
        new CultureCharsetDatabase.CharsetName("UTF-32BE", 12001),
        new CultureCharsetDatabase.CharsetName("UTF-32LE", 12000),
        new CultureCharsetDatabase.CharsetName("utf-7", 65000),
        new CultureCharsetDatabase.CharsetName("utf-8", 65001),
        new CultureCharsetDatabase.CharsetName("utf7", 65000),
        new CultureCharsetDatabase.CharsetName("utf8", 65001),
        new CultureCharsetDatabase.CharsetName("visual", 28598),
        new CultureCharsetDatabase.CharsetName("windows-1250", 1250),
        new CultureCharsetDatabase.CharsetName("windows-1251", 1251),
        new CultureCharsetDatabase.CharsetName("windows-1252", 1252),
        new CultureCharsetDatabase.CharsetName("windows-1253", 1253),
        new CultureCharsetDatabase.CharsetName("Windows-1254", 1254),
        new CultureCharsetDatabase.CharsetName("windows-1255", 1255),
        new CultureCharsetDatabase.CharsetName("windows-1256", 1256),
        new CultureCharsetDatabase.CharsetName("windows-1257", 1257),
        new CultureCharsetDatabase.CharsetName("windows-1258", 1258),
        new CultureCharsetDatabase.CharsetName("windows-874", 874),
        new CultureCharsetDatabase.CharsetName("x-ansi", 1252),
        new CultureCharsetDatabase.CharsetName("x-Chinese-CNS", 20000),
        new CultureCharsetDatabase.CharsetName("x-Chinese-Eten", 20002),
        new CultureCharsetDatabase.CharsetName("x-cp1250", 1250),
        new CultureCharsetDatabase.CharsetName("x-cp1251", 1251),
        new CultureCharsetDatabase.CharsetName("x-cp20001", 20001),
        new CultureCharsetDatabase.CharsetName("x-cp20003", 20003),
        new CultureCharsetDatabase.CharsetName("x-cp20004", 20004),
        new CultureCharsetDatabase.CharsetName("x-cp20005", 20005),
        new CultureCharsetDatabase.CharsetName("x-cp20261", 20261),
        new CultureCharsetDatabase.CharsetName("x-cp20269", 20269),
        new CultureCharsetDatabase.CharsetName("x-cp20936", 20936),
        new CultureCharsetDatabase.CharsetName("x-cp20949", 20949),
        new CultureCharsetDatabase.CharsetName("x-cp21027", 21027),
        new CultureCharsetDatabase.CharsetName("x-cp50227", 50227),
        new CultureCharsetDatabase.CharsetName("x-cp50229", 50229),
        new CultureCharsetDatabase.CharsetName("X-EBCDIC-JapaneseAndUSCanada", 50931),
        new CultureCharsetDatabase.CharsetName("X-EBCDIC-KoreanExtended", 20833),
        new CultureCharsetDatabase.CharsetName("x-euc-cn", 51936),
        new CultureCharsetDatabase.CharsetName("x-euc-jp", 51932),
        new CultureCharsetDatabase.CharsetName("x-euc", 51932),
        new CultureCharsetDatabase.CharsetName("x-Europa", 29001),
        new CultureCharsetDatabase.CharsetName("x-IA5-German", 20106),
        new CultureCharsetDatabase.CharsetName("x-IA5-Norwegian", 20108),
        new CultureCharsetDatabase.CharsetName("x-IA5-Swedish", 20107),
        new CultureCharsetDatabase.CharsetName("x-IA5", 20105),
        new CultureCharsetDatabase.CharsetName("x-iscii-as", 57006),
        new CultureCharsetDatabase.CharsetName("x-iscii-be", 57003),
        new CultureCharsetDatabase.CharsetName("x-iscii-de", 57002),
        new CultureCharsetDatabase.CharsetName("x-iscii-gu", 57010),
        new CultureCharsetDatabase.CharsetName("x-iscii-ka", 57008),
        new CultureCharsetDatabase.CharsetName("x-iscii-ma", 57009),
        new CultureCharsetDatabase.CharsetName("x-iscii-or", 57007),
        new CultureCharsetDatabase.CharsetName("x-iscii-pa", 57011),
        new CultureCharsetDatabase.CharsetName("x-iscii-ta", 57004),
        new CultureCharsetDatabase.CharsetName("x-iscii-te", 57005),
        new CultureCharsetDatabase.CharsetName("x-mac-arabic", 10004),
        new CultureCharsetDatabase.CharsetName("x-mac-ce", 10029),
        new CultureCharsetDatabase.CharsetName("x-mac-chinesesimp", 10008),
        new CultureCharsetDatabase.CharsetName("x-mac-chinesetrad", 10002),
        new CultureCharsetDatabase.CharsetName("x-mac-croatian", 10082),
        new CultureCharsetDatabase.CharsetName("x-mac-cyrillic", 10007),
        new CultureCharsetDatabase.CharsetName("x-mac-greek", 10006),
        new CultureCharsetDatabase.CharsetName("x-mac-hebrew", 10005),
        new CultureCharsetDatabase.CharsetName("x-mac-icelandic", 10079),
        new CultureCharsetDatabase.CharsetName("x-mac-japanese", 10001),
        new CultureCharsetDatabase.CharsetName("x-mac-korean", 10003),
        new CultureCharsetDatabase.CharsetName("x-mac-romanian", 10010),
        new CultureCharsetDatabase.CharsetName("x-mac-thai", 10021),
        new CultureCharsetDatabase.CharsetName("x-mac-turkish", 10081),
        new CultureCharsetDatabase.CharsetName("x-mac-ukrainian", 10017),
        new CultureCharsetDatabase.CharsetName("x-ms-cp932", 932),
        new CultureCharsetDatabase.CharsetName("x-sjis", 932),
        new CultureCharsetDatabase.CharsetName("x-unicode-1-1-utf-7", 65000),
        new CultureCharsetDatabase.CharsetName("x-unicode-1-1-utf-8", 65001),
        new CultureCharsetDatabase.CharsetName("x-unicode-2-0-utf-7", 65000),
        new CultureCharsetDatabase.CharsetName("x-unicode-2-0-utf-8", 65001),
        new CultureCharsetDatabase.CharsetName("x-user-defined", 1252),
        new CultureCharsetDatabase.CharsetName("x-x-big5", 950)
      };
      CultureCharsetDatabase.CultureData[] cultureDataArray = new CultureCharsetDatabase.CultureData[96]
      {
        new CultureCharsetDatabase.CultureData(1025, "ar-SA", 1256, 1256, 1256, "ar", "Arabic (Saudi Arabia)"),
        new CultureCharsetDatabase.CultureData(1026, "bg-BG", 1251, 1251, 1251, "bg", "Bulgarian (Bulgaria)"),
        new CultureCharsetDatabase.CultureData(1027, "ca-ES", 1252, 28591, 1252, "ca", "Catalan (Catalan)"),
        new CultureCharsetDatabase.CultureData(1028, "zh-TW", 950, 950, 950, "zh-CHT", "Chinese (Taiwan)"),
        new CultureCharsetDatabase.CultureData(1029, "cs-CZ", 1250, 28592, 1250, "cs", "Czech (Czech Republic)"),
        new CultureCharsetDatabase.CultureData(1030, "da-DK", 1252, 28591, 1252, "da", "Danish (Denmark)"),
        new CultureCharsetDatabase.CultureData(1031, "de-DE", 1252, 28591, 1252, "de", "German (Germany)"),
        new CultureCharsetDatabase.CultureData(1032, "el-GR", 1253, 28597, 1253, "el", "Greek (Greece)"),
        new CultureCharsetDatabase.CultureData(1033, "en-US", 1252, 28591, 1252, "en", "English (United States)"),
        new CultureCharsetDatabase.CultureData(1035, "fi-FI", 1252, 28591, 1252, "fi", "Finnish (Finland)"),
        new CultureCharsetDatabase.CultureData(1036, "fr-FR", 1252, 28591, 1252, "fr", "French (France)"),
        new CultureCharsetDatabase.CultureData(1037, "he-IL", 1255, 1255, 1255, "he", "Hebrew (Israel)"),
        new CultureCharsetDatabase.CultureData(1038, "hu-HU", 1250, 28592, 1250, "hu", "Hungarian (Hungary)"),
        new CultureCharsetDatabase.CultureData(1039, "is-IS", 1252, 28591, 1252, "is", "Icelandic (Iceland)"),
        new CultureCharsetDatabase.CultureData(1040, "it-IT", 1252, 28591, 1252, "it", "Italian (Italy)"),
        new CultureCharsetDatabase.CultureData(1041, "ja-JP", 932, 50220, 932, "ja", "Japanese (Japan)"),
        new CultureCharsetDatabase.CultureData(1042, "ko-KR", 949, 949, 949, "ko", "Korean (Korea)"),
        new CultureCharsetDatabase.CultureData(1043, "nl-NL", 1252, 28591, 1252, "nl", "Dutch (Netherlands)"),
        new CultureCharsetDatabase.CultureData(1044, "nb-NO", 1252, 28591, 1252, "no", "Norwegian - Bokm†l (Norway)"),
        new CultureCharsetDatabase.CultureData(1045, "pl-PL", 1250, 28592, 1250, "pl", "Polish (Poland)"),
        new CultureCharsetDatabase.CultureData(1046, "pt-BR", 1252, 28591, 1252, "pt", "Portuguese (Brazil)"),
        new CultureCharsetDatabase.CultureData(1048, "ro-RO", 1250, 28592, 1250, "ro", "Romanian (Romania)"),
        new CultureCharsetDatabase.CultureData(1049, "ru-RU", 1251, 20866, 1251, "ru", "Russian (Russia)"),
        new CultureCharsetDatabase.CultureData(1050, "hr-HR", 1250, 28592, 1250, "hr", "Croatian (Croatia)"),
        new CultureCharsetDatabase.CultureData(1051, "sk-SK", 1250, 28592, 1250, "sk", "Slovak (Slovakia)"),
        new CultureCharsetDatabase.CultureData(1053, "sv-SE", 1252, 28591, 1252, "sv", "Swedish (Sweden)"),
        new CultureCharsetDatabase.CultureData(1054, "th-TH", 874, 874, 874, "th", "Thai (Thailand)"),
        new CultureCharsetDatabase.CultureData(1055, "tr-TR", 1254, 28599, 1254, "tr", "Turkish (Turkey)"),
        new CultureCharsetDatabase.CultureData(1056, "ur-PK", 1256, 1256, 1256, "ur", "Urdu (Islamic Republic of Pakistan)"),
        new CultureCharsetDatabase.CultureData(1057, "id-ID", 1252, 28591, 1252, "id", "Indonesian (Indonesia)"),
        new CultureCharsetDatabase.CultureData(1058, "uk-UA", 1251, 21866, 1251, "uk", "Ukrainian (Ukraine)"),
        new CultureCharsetDatabase.CultureData(1060, "sl-SI", 1250, 28592, 1250, "sl", "Slovenian (Slovenia)"),
        new CultureCharsetDatabase.CultureData(1061, "et-EE", 1257, 28605, 28605, "et", "Estonian (Estonia)"),
        new CultureCharsetDatabase.CultureData(1062, "lv-LV", 1257, 28603, 28603, "lv", "Latvian (Latvia)"),
        new CultureCharsetDatabase.CultureData(1063, "lt-LT", 1257, 28603, 28603, "lt", "Lithuanian (Lithuania)"),
        new CultureCharsetDatabase.CultureData(1065, "fa-IR", 1256, 1256, 1256, "fa", "Persian (Iran)"),
        new CultureCharsetDatabase.CultureData(1066, "vi-VN", 1258, 1258, 1258, "vi", "Vietnamese (Vietnam)"),
        new CultureCharsetDatabase.CultureData(1069, "eu-ES", 1252, 28591, 1252, "eu", "Basque (Basque)"),
        new CultureCharsetDatabase.CultureData(1081, "hi-IN", 1200, 65001, 65001, "hi", "Hindi (India)"),
        new CultureCharsetDatabase.CultureData(1086, "ms-MY", 1252, 28591, 1252, "ms", "Malay (Malaysia)"),
        new CultureCharsetDatabase.CultureData(1087, "kk-KZ", 1251, 1251, 1251, "kk", "Kazakh (Kazakhstan)"),
        new CultureCharsetDatabase.CultureData(1110, "gl-ES", 1252, 28591, 1252, "gl", "Galician (Galician)"),
        new CultureCharsetDatabase.CultureData(1124, "fil-PH", 1252, 28591, 1252, "fil", "Filipino (Philippines)"),
        new CultureCharsetDatabase.CultureData(2052, "zh-CN", 936, 936, 936, "zh-CHS", "Chinese (People's Republic of China)"),
        new CultureCharsetDatabase.CultureData(2070, "pt-PT", 1252, 28591, 1252, "pt", "Portuguese (Portugal)"),
        new CultureCharsetDatabase.CultureData(2074, "sr-Latn-CS", 1250, 28592, 1250, "sr", "Serbian (Latin - Serbia and Montenegro)"),
        new CultureCharsetDatabase.CultureData(3076, "zh-HK", 950, 950, 950, "zh-CHT", "Chinese (Hong Kong S.A.R.)"),
        new CultureCharsetDatabase.CultureData(3082, "es-ES", 1252, 28591, 1252, "es", "Spanish (Spain)"),
        new CultureCharsetDatabase.CultureData(3098, "sr-Cyrl-CS", 1251, 1251, 1251, "sr", "Serbian (Cyrillic)"),
        new CultureCharsetDatabase.CultureData(33809, "ja-JP-Yomi", 932, 50220, 932, "ja", "Japanese (Phonetic)"),
        new CultureCharsetDatabase.CultureData(1, "ar", 1256, 1256, 1256, (string) null, "Arabic"),
        new CultureCharsetDatabase.CultureData(2, "bg", 1251, 1251, 1251, (string) null, "Bulgarian"),
        new CultureCharsetDatabase.CultureData(3, "ca", 1252, 28591, 1252, (string) null, "Catalan"),
        new CultureCharsetDatabase.CultureData(4, "zh-CHS", 936, 936, 936, (string) null, "Chinese (Simplified)"),
        new CultureCharsetDatabase.CultureData(5, "cs", 1250, 28592, 1250, (string) null, "Czech"),
        new CultureCharsetDatabase.CultureData(6, "da", 1252, 28591, 1252, (string) null, "Danish"),
        new CultureCharsetDatabase.CultureData(7, "de", 1252, 28591, 1252, (string) null, "German"),
        new CultureCharsetDatabase.CultureData(8, "el", 1253, 28597, 1253, (string) null, "Greek"),
        new CultureCharsetDatabase.CultureData(9, "en", 1252, 28591, 1252, (string) null, "English"),
        new CultureCharsetDatabase.CultureData(10, "es", 1252, 28591, 1252, (string) null, "Spanish"),
        new CultureCharsetDatabase.CultureData(11, "fi", 1252, 28591, 1252, (string) null, "Finnish"),
        new CultureCharsetDatabase.CultureData(12, "fr", 1252, 28591, 1252, (string) null, "French"),
        new CultureCharsetDatabase.CultureData(13, "he", 1255, 1255, 1255, (string) null, "Hebrew"),
        new CultureCharsetDatabase.CultureData(14, "hu", 1250, 28592, 1250, (string) null, "Hungarian"),
        new CultureCharsetDatabase.CultureData(15, "is", 1252, 28591, 1252, (string) null, "Icelandic"),
        new CultureCharsetDatabase.CultureData(16, "it", 1252, 28591, 1252, (string) null, "Italian"),
        new CultureCharsetDatabase.CultureData(17, "ja", 932, 50220, 932, (string) null, "Japanese"),
        new CultureCharsetDatabase.CultureData(18, "ko", 949, 949, 949, (string) null, "Korean"),
        new CultureCharsetDatabase.CultureData(19, "nl", 1252, 28591, 1252, (string) null, "Dutch"),
        new CultureCharsetDatabase.CultureData(20, "no", 1252, 28591, 1252, (string) null, "Norwegian"),
        new CultureCharsetDatabase.CultureData(21, "pl", 1250, 28592, 1250, (string) null, "Polish"),
        new CultureCharsetDatabase.CultureData(22, "pt", 1252, 28591, 1252, (string) null, "Portuguese"),
        new CultureCharsetDatabase.CultureData(24, "ro", 1250, 28592, 1250, (string) null, "Romanian"),
        new CultureCharsetDatabase.CultureData(25, "ru", 1251, 20866, 1251, (string) null, "Russian"),
        new CultureCharsetDatabase.CultureData(26, "hr", 1250, 28592, 1250, (string) null, "Croatian"),
        new CultureCharsetDatabase.CultureData(27, "sk", 1250, 28592, 1250, (string) null, "Slovak"),
        new CultureCharsetDatabase.CultureData(29, "sv", 1252, 28591, 1252, (string) null, "Swedish"),
        new CultureCharsetDatabase.CultureData(30, "th", 874, 874, 874, (string) null, "Thai"),
        new CultureCharsetDatabase.CultureData(31, "tr", 1254, 28599, 1254, (string) null, "Turkish"),
        new CultureCharsetDatabase.CultureData(32, "ur", 1256, 1256, 1256, (string) null, "Urdu"),
        new CultureCharsetDatabase.CultureData(33, "id", 1252, 28591, 1252, (string) null, "Indonesian"),
        new CultureCharsetDatabase.CultureData(34, "uk", 1251, 21866, 1251, (string) null, "Ukrainian"),
        new CultureCharsetDatabase.CultureData(36, "sl", 1250, 28592, 1250, (string) null, "Slovenian"),
        new CultureCharsetDatabase.CultureData(37, "et", 1257, 28605, 28605, (string) null, "Estonian"),
        new CultureCharsetDatabase.CultureData(38, "lv", 1257, 28603, 28603, (string) null, "Latvian"),
        new CultureCharsetDatabase.CultureData(39, "lt", 1257, 28603, 28603, (string) null, "Lithuanian"),
        new CultureCharsetDatabase.CultureData(41, "fa", 1256, 1256, 1256, (string) null, "Persian"),
        new CultureCharsetDatabase.CultureData(42, "vi", 1258, 1258, 1258, (string) null, "Vietnamese"),
        new CultureCharsetDatabase.CultureData(45, "eu", 1252, 28591, 1252, (string) null, "Basque"),
        new CultureCharsetDatabase.CultureData(57, "hi", 1200, 65001, 65001, (string) null, "Hindi"),
        new CultureCharsetDatabase.CultureData(62, "ms", 1252, 28591, 1252, (string) null, "Malay"),
        new CultureCharsetDatabase.CultureData(63, "kk", 1251, 1251, 1251, (string) null, "Kazakh"),
        new CultureCharsetDatabase.CultureData(86, "gl", 1252, 28591, 1252, (string) null, "Galician"),
        new CultureCharsetDatabase.CultureData(100, "fil", 1252, 28591, 1252, (string) null, "Filipino"),
        new CultureCharsetDatabase.CultureData(31748, "zh-CHT", 950, 950, 950, (string) null, "Chinese (Traditional)"),
        new CultureCharsetDatabase.CultureData(31770, "sr", 1251, 1251, 1251, (string) null, "Serbian")
      };
      CultureCharsetDatabase.GlobalizationData data = new CultureCharsetDatabase.GlobalizationData();
      Culture parentCulture1 = (Culture) null;
      CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
      Culture culture1;
      foreach (CultureInfo cultureInfo in cultures)
      {
        if (!data.LcidToCulture.TryGetValue(cultureInfo.LCID, out culture1))
        {
          culture1 = new Culture(cultureInfo.LCID, cultureInfo.Name);
          data.LcidToCulture.Add(cultureInfo.LCID, culture1);
          culture1.SetDescription(cultureInfo.EnglishName);
          culture1.SetNativeDescription(cultureInfo.NativeName);
          culture1.SetCultureInfo(cultureInfo);
          if (cultureInfo.LCID == (int) sbyte.MaxValue)
            parentCulture1 = culture1;
        }
        if (!data.NameToCulture.ContainsKey(cultureInfo.Name))
          data.NameToCulture.Add(cultureInfo.Name, culture1);
      }
      foreach (CultureCharsetDatabase.CultureData cultureData in cultureDataArray)
      {
        if (!data.LcidToCulture.TryGetValue(cultureData.LCID, out culture1))
        {
          culture1 = new Culture(cultureData.LCID, cultureData.Name);
          data.LcidToCulture.Add(cultureData.LCID, culture1);
          culture1.SetDescription(cultureData.Description);
          culture1.SetNativeDescription(cultureData.Description);
        }
        if (!data.NameToCulture.ContainsKey(cultureData.Name))
          data.NameToCulture.Add(cultureData.Name, culture1);
      }
      data.InvariantCulture = parentCulture1;
      foreach (CultureInfo cultureInfo in cultures)
      {
        culture1 = data.LcidToCulture[cultureInfo.LCID];
        if (cultureInfo.Parent != null)
        {
          Culture parentCulture2;
          if (data.LcidToCulture.TryGetValue(cultureInfo.Parent.LCID, out parentCulture2))
            culture1.SetParentCulture(parentCulture2);
          else
            culture1.SetParentCulture(culture1);
        }
        else
          culture1.SetParentCulture(culture1);
      }
      foreach (CultureCharsetDatabase.CultureData cultureData in cultureDataArray)
      {
        culture1 = data.LcidToCulture[cultureData.LCID];
        if (culture1.ParentCulture == null)
        {
          if (cultureData.ParentCultureName != null)
          {
            Culture parentCulture2;
            if (data.NameToCulture.TryGetValue(cultureData.ParentCultureName, out parentCulture2))
              culture1.SetParentCulture(parentCulture2);
            else
              culture1.SetParentCulture(culture1);
          }
          else
            culture1.SetParentCulture(culture1);
        }
      }
      EncodingInfo[] encodings = Encoding.GetEncodings();
      Charset charset1;
      foreach (EncodingInfo encodingInfo in encodings)
      {
        if (!data.CodePageToCharset.TryGetValue(encodingInfo.CodePage, out charset1))
        {
          charset1 = new Charset(encodingInfo.CodePage, encodingInfo.Name);
          charset1.SetDescription(encodingInfo.DisplayName);
          data.CodePageToCharset.Add(encodingInfo.CodePage, charset1);
        }
        if (!data.NameToCharset.ContainsKey(encodingInfo.Name))
        {
          data.NameToCharset.Add(encodingInfo.Name, charset1);
          if (encodingInfo.Name.Length > data.MaxCharsetNameLength)
            data.MaxCharsetNameLength = encodingInfo.Name.Length;
        }
      }
      foreach (CultureCharsetDatabase.WindowsCodePage windowsCodePage in windowsCodePageArray)
      {
        if (windowsCodePage.LCID == 0)
        {
          if (windowsCodePage.CodePage == 1200 && parentCulture1 != null)
          {
            culture1 = parentCulture1;
          }
          else
          {
            culture1 = new Culture(0, (string) null);
            culture1.SetParentCulture(parentCulture1);
          }
          culture1.SetDescription(windowsCodePage.GenericCultureDescription);
        }
        else if (!data.LcidToCulture.TryGetValue(windowsCodePage.LCID, out culture1) && !data.NameToCulture.TryGetValue(windowsCodePage.CultureName, out culture1))
        {
          culture1 = new Culture(windowsCodePage.LCID, windowsCodePage.CultureName);
          data.LcidToCulture.Add(windowsCodePage.LCID, culture1);
          data.NameToCulture.Add(windowsCodePage.CultureName, culture1);
        }
        if (!data.CodePageToCharset.TryGetValue(windowsCodePage.CodePage, out charset1))
        {
          charset1 = new Charset(windowsCodePage.CodePage, windowsCodePage.Name);
          data.NameToCharset.Add(charset1.Name, charset1);
          data.CodePageToCharset.Add(charset1.CodePage, charset1);
          if (charset1.Name.Length > data.MaxCharsetNameLength)
            data.MaxCharsetNameLength = charset1.Name.Length;
        }
        charset1.SetWindows();
        culture1.SetWindowsCharset(charset1);
        charset1.SetCulture(culture1);
        if (!data.CodePageToCharset.TryGetValue(windowsCodePage.MimeCodePage, out charset1))
          charset1 = data.CodePageToCharset[windowsCodePage.CodePage];
        culture1.SetMimeCharset(charset1);
        if (!data.CodePageToCharset.TryGetValue(windowsCodePage.WebCodePage, out charset1))
          charset1 = data.CodePageToCharset[windowsCodePage.CodePage];
        culture1.SetWebCharset(charset1);
      }
      foreach (CultureCharsetDatabase.CharsetName charsetName in charsetNameArray)
      {
        if (!data.NameToCharset.TryGetValue(charsetName.Name, out charset1))
        {
          if (data.CodePageToCharset.TryGetValue(charsetName.CodePage, out charset1))
          {
            data.NameToCharset.Add(charsetName.Name, charset1);
            if (charsetName.Name.Length > data.MaxCharsetNameLength)
              data.MaxCharsetNameLength = charsetName.Name.Length;
          }
        }
        else if (charset1.CodePage != charsetName.CodePage && data.CodePageToCharset.TryGetValue(charsetName.CodePage, out charset1))
          data.NameToCharset[charsetName.Name] = charset1;
      }
      for (int index = 0; index < CodePageMapData.codePages.Length; ++index)
      {
        if (data.CodePageToCharset.TryGetValue((int) CodePageMapData.codePages[index].cpid, out charset1))
          charset1.SetMapIndex(index);
        if (charset1.Culture == null)
        {
          Charset charset2 = data.CodePageToCharset[(int) CodePageMapData.codePages[index].windowsCpid];
          charset1.SetCulture(charset2.Culture);
        }
      }
      foreach (CultureCharsetDatabase.CultureCodePageOverride codePageOverride in codePageOverrideArray)
      {
        if (data.NameToCulture.TryGetValue(codePageOverride.CultureName, out culture1))
        {
          if (data.CodePageToCharset.TryGetValue(codePageOverride.MimeCodePage, out charset1))
            culture1.SetMimeCharset(charset1);
          if (data.CodePageToCharset.TryGetValue(codePageOverride.WebCodePage, out charset1))
            culture1.SetWebCharset(charset1);
        }
      }
      foreach (CultureInfo cultureInfo in cultures)
      {
        culture1 = data.LcidToCulture[cultureInfo.LCID];
        if (culture1.WindowsCharset == null)
        {
          int index = cultureInfo.TextInfo.ANSICodePage;
          if (index <= 500)
          {
            index = 1200;
          }
          else
          {
            bool flag = false;
            foreach (CultureCharsetDatabase.WindowsCodePage windowsCodePage in windowsCodePageArray)
            {
              if (index == windowsCodePage.CodePage)
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              index = 1200;
          }
          charset1 = data.CodePageToCharset[index];
          culture1.SetWindowsCharset(charset1);
          if (culture1 != culture1.ParentCulture && culture1.WindowsCharset == culture1.ParentCulture.WindowsCharset)
          {
            culture1.SetMimeCharset(culture1.ParentCulture.MimeCharset);
            culture1.SetWebCharset(culture1.ParentCulture.WebCharset);
          }
          if (culture1.MimeCharset == null)
            culture1.SetMimeCharset(charset1.Culture.MimeCharset);
          if (culture1.WebCharset == null)
            culture1.SetWebCharset(charset1.Culture.WebCharset);
        }
      }
      foreach (CultureCharsetDatabase.CultureData cultureData in cultureDataArray)
      {
        culture1 = data.LcidToCulture[cultureData.LCID];
        if (culture1.WindowsCharset == null)
        {
          int windowsCodePage = cultureData.WindowsCodePage;
          charset1 = data.CodePageToCharset[windowsCodePage];
          culture1.SetWindowsCharset(charset1);
          Charset charset2;
          if (data.CodePageToCharset.TryGetValue(cultureData.MimeCodePage, out charset2))
            culture1.SetMimeCharset(charset2);
          if (data.CodePageToCharset.TryGetValue(cultureData.WebCodePage, out charset2))
            culture1.SetWebCharset(charset2);
          if (culture1 != culture1.ParentCulture && culture1.WindowsCharset == culture1.ParentCulture.WindowsCharset)
          {
            if (culture1.MimeCharset == null)
              culture1.SetMimeCharset(culture1.ParentCulture.MimeCharset);
            if (culture1.WebCharset == null)
              culture1.SetWebCharset(culture1.ParentCulture.WebCharset);
          }
          if (culture1.MimeCharset == null)
            culture1.SetMimeCharset(charset1.Culture.MimeCharset);
          if (culture1.WebCharset == null)
            culture1.SetWebCharset(charset1.Culture.WebCharset);
        }
      }
      foreach (EncodingInfo encodingInfo in encodings)
      {
        charset1 = data.CodePageToCharset[encodingInfo.CodePage];
        if (charset1.Culture == null)
        {
          int index = 1200;
          Encoding encoding;
          if (charset1.TryGetEncoding(out encoding))
          {
            index = encoding.WindowsCodePage;
            bool flag = false;
            foreach (CultureCharsetDatabase.WindowsCodePage windowsCodePage in windowsCodePageArray)
            {
              if (index == windowsCodePage.CodePage)
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              index = 1200;
          }
          Charset charset2 = data.CodePageToCharset[index];
          charset1.SetCulture(charset2.Culture);
        }
      }
      foreach (CultureCharsetDatabase.CodePageCultureOverride pageCultureOverride in pageCultureOverrideArray)
      {
        if (data.CodePageToCharset.TryGetValue(pageCultureOverride.CodePage, out charset1) && data.NameToCulture.TryGetValue(pageCultureOverride.CultureName, out culture1))
          charset1.SetCulture(culture1);
      }
      if (!data.LcidToCulture.TryGetValue(CultureInfo.CurrentUICulture.LCID, out data.DefaultCulture) && !data.LcidToCulture.TryGetValue(CultureInfo.CurrentCulture.LCID, out data.DefaultCulture))
        data.DefaultCulture = data.LcidToCulture[1033];
      IList<Internal.CtsConfigurationSetting> configuration = Internal.ApplicationServices.Provider.GetConfiguration("Globalization");
      bool result1 = false;
      foreach (Internal.CtsConfigurationSetting configurationSetting in (IEnumerable<Internal.CtsConfigurationSetting>) configuration)
      {
        string name = configurationSetting.Name;
        IList<Internal.CtsConfigurationArgument> arguments = configurationSetting.Arguments;
        int result2;
        Charset charset2;
        int result3;
        switch (name.ToLower())
        {
          case "overridecharsetdefaultname":
            if (arguments.Count != 2)
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            string str1;
            string str2;
            if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("DefaultName", StringComparison.OrdinalIgnoreCase))
            {
              str1 = arguments[0].Value.Trim();
              str2 = arguments[1].Value.Trim();
            }
            else if (arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[0].Name.Equals("DefaultName", StringComparison.OrdinalIgnoreCase))
            {
              str1 = arguments[1].Value.Trim();
              str2 = arguments[0].Value.Trim();
            }
            else
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (int.TryParse(str1, out result2))
            {
              if (!data.CodePageToCharset.TryGetValue(result2, out charset1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
            }
            else if (!data.NameToCharset.TryGetValue(str1, out charset1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (data.NameToCharset.TryGetValue(str2, out charset2))
            {
              if (charset1 != charset2)
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
              charset1.SetDefaultName(str2);
              continue;
            }
            Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
            continue;
          case "addcharsetaliasname":
            if (arguments.Count != 2)
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            string str3;
            string key1;
            if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
            {
              str3 = arguments[0].Value.Trim();
              key1 = arguments[1].Value.Trim();
            }
            else if (arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[0].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
            {
              str3 = arguments[1].Value.Trim();
              key1 = arguments[0].Value.Trim();
            }
            else
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (int.TryParse(str3, out result2))
            {
              if (!data.CodePageToCharset.TryGetValue(result2, out charset1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
            }
            else if (!data.NameToCharset.TryGetValue(str3, out charset1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (data.NameToCharset.TryGetValue(key1, out charset2))
            {
              if (charset1 != charset2)
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
              continue;
            }
            data.NameToCharset.Add(key1, charset1);
            if (key1.Length > data.MaxCharsetNameLength)
            {
              data.MaxCharsetNameLength = key1.Length;
              continue;
            }
            continue;
          case "overridecharsetculture":
            if (arguments.Count != 2)
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            string str4;
            string str5;
            if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
            {
              str4 = arguments[0].Value.Trim();
              str5 = arguments[1].Value.Trim();
            }
            else if (arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
            {
              str4 = arguments[1].Value.Trim();
              str5 = arguments[0].Value.Trim();
            }
            else
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (int.TryParse(str4, out result2))
            {
              if (!data.CodePageToCharset.TryGetValue(result2, out charset1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
            }
            else if (!data.NameToCharset.TryGetValue(str4, out charset1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (int.TryParse(str5, out result3))
            {
              if (!data.LcidToCulture.TryGetValue(result3, out culture1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
            }
            else if (!data.NameToCulture.TryGetValue(str5, out culture1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            charset1.SetCulture(culture1);
            continue;
          case "addculturealiasname":
            if (arguments.Count != 2)
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            string str6;
            string key2;
            if (arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
            {
              str6 = arguments[0].Value.Trim();
              key2 = arguments[1].Value.Trim();
            }
            else if (arguments[1].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase) && arguments[0].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
            {
              str6 = arguments[1].Value.Trim();
              key2 = arguments[0].Value.Trim();
            }
            else
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (int.TryParse(str6, out result3))
            {
              if (!data.LcidToCulture.TryGetValue(result3, out culture1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
            }
            else if (!data.NameToCulture.TryGetValue(str6, out culture1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            Culture culture2;
            if (data.NameToCulture.TryGetValue(key2, out culture2))
            {
              if (culture1 != culture2)
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
              continue;
            }
            data.NameToCulture.Add(key2, culture1);
            continue;
          case "overrideculturecharset":
            string str7 = (string) null;
            string str8 = (string) null;
            string str9 = (string) null;
            string str10 = (string) null;
            foreach (Internal.CtsConfigurationArgument configurationArgument in (IEnumerable<Internal.CtsConfigurationArgument>) arguments)
            {
              if (configurationArgument.Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                str7 = configurationArgument.Value.Trim();
              else if (configurationArgument.Name.Equals("WindowsCharset", StringComparison.OrdinalIgnoreCase))
                str8 = configurationArgument.Value.Trim();
              else if (configurationArgument.Name.Equals("MimeCharset", StringComparison.OrdinalIgnoreCase))
                str9 = configurationArgument.Value.Trim();
              else if (configurationArgument.Name.Equals("WebCharset", StringComparison.OrdinalIgnoreCase))
              {
                str10 = configurationArgument.Value.Trim();
              }
              else
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                break;
              }
            }
            if (string.IsNullOrEmpty(str7) || string.IsNullOrEmpty(str8) && string.IsNullOrEmpty(str9) && string.IsNullOrEmpty(str10))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (int.TryParse(str7, out result3))
            {
              if (!data.LcidToCulture.TryGetValue(result3, out culture1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
            }
            else if (!data.NameToCulture.TryGetValue(str7, out culture1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (!string.IsNullOrEmpty(str8))
            {
              if (int.TryParse(str8, out result2))
              {
                if (!data.CodePageToCharset.TryGetValue(result2, out charset1))
                {
                  Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                  continue;
                }
              }
              else if (!data.NameToCharset.TryGetValue(str8, out charset1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
              if (!charset1.IsWindowsCharset)
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
              culture1.SetWindowsCharset(charset1);
            }
            if (!string.IsNullOrEmpty(str9))
            {
              if (int.TryParse(str9, out result2))
              {
                if (!data.CodePageToCharset.TryGetValue(result2, out charset1))
                {
                  Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                  continue;
                }
              }
              else if (!data.NameToCharset.TryGetValue(str9, out charset1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
              culture1.SetMimeCharset(charset1);
            }
            if (!string.IsNullOrEmpty(str10))
            {
              if (int.TryParse(str10, out result2))
              {
                if (!data.CodePageToCharset.TryGetValue(result2, out charset1))
                {
                  Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                  continue;
                }
              }
              else if (!data.NameToCharset.TryGetValue(str10, out charset1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
              culture1.SetWebCharset(charset1);
              continue;
            }
            continue;
          case "defaultculture":
            if (arguments.Count != 1 || !arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            string str11 = arguments[0].Value.Trim();
            if (int.TryParse(str11, out result3))
            {
              if (!data.LcidToCulture.TryGetValue(result3, out culture1))
              {
                Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
                continue;
              }
            }
            else if (!data.NameToCulture.TryGetValue(str11, out culture1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            data.DefaultCulture = culture1;
            continue;
          case "fallbacktodefaultcharset":
            if (arguments.Count != 1 || !arguments[0].Name.Equals("Fallback", StringComparison.OrdinalIgnoreCase))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            if (!bool.TryParse(arguments[0].Value.Trim(), out result1))
            {
              Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
              continue;
            }
            data.FallbackToDefaultCharset = result1;
            continue;
          default:
            Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
            continue;
        }
      }
      if (defaultCultureName != null)
        data.DefaultCulture = data.NameToCulture[defaultCultureName];
      data.DefaultDetectionPriorityOrder = CultureCharsetDatabase.GetCultureSpecificCodepageDetectionPriorityOrder(data.DefaultCulture, (int[]) null);
      parentCulture1.SetCodepageDetectionPriorityOrder(data.DefaultDetectionPriorityOrder);
      data.DefaultCulture.GetCodepageDetectionPriorityOrder(data);
      data.AsciiCharset = data.CodePageToCharset[20127];
      data.Utf8Charset = data.CodePageToCharset[65001];
      data.UnicodeCharset = data.CodePageToCharset[1200];
      return data;
    }

    private struct WindowsCodePage
    {

        public int CodePage { get; }

        public string Name { get; }

        public int LCID { get; }

        public string CultureName { get; }

        public int MimeCodePage { get; }

        public int WebCodePage { get; }

        public string GenericCultureDescription { get; }

        public WindowsCodePage(int codePage, string name, int lcid, string cultureName, int mimeCodePage, int webCodePage, string genericCultureDescription)
      {
        this.CodePage = codePage;
        this.Name = name;
        this.LCID = lcid;
        this.CultureName = cultureName;
        this.MimeCodePage = mimeCodePage;
        this.WebCodePage = webCodePage;
        this.GenericCultureDescription = genericCultureDescription;
      }
    }

    private struct CodePageCultureOverride
    {

        public int CodePage { get; }

        public string CultureName { get; }

        public CodePageCultureOverride(int codePage, string cultureName)
      {
        this.CodePage = codePage;
        this.CultureName = cultureName;
      }
    }

    private struct CultureCodePageOverride
    {

        public string CultureName { get; }

        public int MimeCodePage { get; }

        public int WebCodePage { get; }

        public CultureCodePageOverride(string cultureName, int mimeCodePage, int webCodePage)
      {
        this.CultureName = cultureName;
        this.MimeCodePage = mimeCodePage;
        this.WebCodePage = webCodePage;
      }
    }

    private struct CharsetName
    {

        public string Name { get; }

        public int CodePage { get; }

        public CharsetName(string name, int codePage)
      {
        this.Name = name;
        this.CodePage = codePage;
      }
    }

    private struct CultureData
    {

        public int LCID { get; }

        public string Name { get; }

        public int WindowsCodePage { get; }

        public int MimeCodePage { get; }

        public int WebCodePage { get; }

        public string ParentCultureName { get; }

        public string Description { get; }

        public CultureData(int lcid, string name, int windowsCodePage, int mimeCodePage, int webCodePage, string parentCultureName, string description)
      {
        this.LCID = lcid;
        this.Name = name;
        this.WindowsCodePage = windowsCodePage;
        this.MimeCodePage = mimeCodePage;
        this.WebCodePage = webCodePage;
        this.ParentCultureName = parentCultureName;
        this.Description = description;
      }
    }

    internal class GlobalizationData
    {
      internal Dictionary<string, Charset> NameToCharset = new Dictionary<string, Charset>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      internal Dictionary<int, Charset> CodePageToCharset = new Dictionary<int, Charset>((IEqualityComparer<int>) CultureCharsetDatabase.IntComparerInstance);
      internal Dictionary<string, Culture> NameToCulture = new Dictionary<string, Culture>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      internal Dictionary<int, Culture> LcidToCulture = new Dictionary<int, Culture>((IEqualityComparer<int>) CultureCharsetDatabase.IntComparerInstance);
      internal Culture DefaultCulture;
      internal bool FallbackToDefaultCharset;
      internal Culture InvariantCulture;
      internal int[] DefaultDetectionPriorityOrder;
      internal int MaxCharsetNameLength;
      internal Charset Utf8Charset;
      internal Charset AsciiCharset;
      internal Charset UnicodeCharset;
    }

    private class IntComparer : IEqualityComparer<int>
    {
      public bool Equals(int x, int y)
      {
        return x == y;
      }

      public int GetHashCode(int obj)
      {
        return obj;
      }
    }
  }
}
