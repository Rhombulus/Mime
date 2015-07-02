// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.OutboundCodePageDetector
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Globalization
{
  public class OutboundCodePageDetector
  {
    private CodePageDetect detector;

    public OutboundCodePageDetector()
    {
      this.detector.Initialize();
    }

    internal static bool IsCodePageDetectable(int cpid, bool onlyValid)
    {
      return CodePageDetect.IsCodePageDetectable(cpid, onlyValid);
    }

    internal static int[] GetDefaultCodePagePriorityList()
    {
      return CodePageDetect.GetDefaultPriorityList();
    }

    internal static char[] GetCommonExceptionCharacters()
    {
      return CodePageDetect.GetCommonExceptionCharacters();
    }

    public void Reset()
    {
      this.detector.Reset();
    }

    public void AddText(char ch)
    {
      this.detector.AddData(ch);
    }

    public void AddText(char[] buffer, int index, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(index), CtsResources.GlobalizationStrings.IndexOutOfRange);
      if (count < 0 || count > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.GlobalizationStrings.CountOutOfRange);
      if (buffer.Length - index < count)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.GlobalizationStrings.CountTooLarge);
      this.detector.AddData(buffer, index, count);
    }

    public void AddText(char[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      this.detector.AddData(buffer, 0, buffer.Length);
    }

    public void AddText(string value, int index, int count)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value));
      if (index < 0 || index > value.Length)
        throw new ArgumentOutOfRangeException(nameof(index), CtsResources.GlobalizationStrings.IndexOutOfRange);
      if (count < 0 || count > value.Length)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.GlobalizationStrings.CountOutOfRange);
      if (value.Length - index < count)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.GlobalizationStrings.CountTooLarge);
      this.detector.AddData(value, index, count);
    }

    public void AddText(string value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value));
      this.detector.AddData(value, 0, value.Length);
    }

    public void AddText(TextReader reader, int maxCharacters)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof(reader));
      if (maxCharacters < 0)
        throw new ArgumentOutOfRangeException(nameof(maxCharacters), CtsResources.GlobalizationStrings.MaxCharactersCannotBeNegative);
      this.detector.AddData(reader, maxCharacters);
    }

    public void AddText(TextReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof(reader));
      this.detector.AddData(reader, int.MaxValue);
    }

    public int GetCodePage()
    {
      return this.detector.GetCodePage(Culture.Default.CodepageDetectionPriorityOrder, false, false, true);
    }

    public int GetCodePage(Culture culture, bool allowCommonFallbackExceptions)
    {
      if (culture == null)
        culture = Culture.Default;
      return this.detector.GetCodePage(culture.CodepageDetectionPriorityOrder, allowCommonFallbackExceptions, false, true);
    }

    public int GetCodePage(Charset preferredCharset, bool allowCommonFallbackExceptions)
    {
      int[] detectionPriorityOrder = Culture.Default.CodepageDetectionPriorityOrder;
      if (preferredCharset != null)
        detectionPriorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, detectionPriorityOrder);
      return this.detector.GetCodePage(detectionPriorityOrder, allowCommonFallbackExceptions, false, true);
    }

    internal int GetCodePage(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
    {
      if (codePagePriorityList != null)
      {
        for (int index = 0; index < codePagePriorityList.Length; ++index)
        {
          if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[index], false))
            throw new ArgumentException(CtsResources.GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, nameof(codePagePriorityList));
        }
      }
      return this.detector.GetCodePage(codePagePriorityList, fallbackExceptions > FallbackExceptions.None, fallbackExceptions > FallbackExceptions.Common, onlyValidCodePages);
    }

    public int[] GetCodePages()
    {
      return this.detector.GetCodePages(Culture.Default.CodepageDetectionPriorityOrder, false, false, true);
    }

    public int[] GetCodePages(Culture culture, bool allowCommonFallbackExceptions)
    {
      if (culture == null)
        culture = Culture.Default;
      return this.detector.GetCodePages(culture.CodepageDetectionPriorityOrder, allowCommonFallbackExceptions, false, true);
    }

    public int[] GetCodePages(Charset preferredCharset, bool allowCommonFallbackExceptions)
    {
      int[] detectionPriorityOrder = Culture.Default.CodepageDetectionPriorityOrder;
      if (preferredCharset != null)
        detectionPriorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, detectionPriorityOrder);
      return this.detector.GetCodePages(detectionPriorityOrder, allowCommonFallbackExceptions, false, true);
    }

    internal int[] GetCodePages(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
    {
      if (codePagePriorityList != null)
      {
        for (int index = 0; index < codePagePriorityList.Length; ++index)
        {
          if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[index], false))
            throw new ArgumentException(CtsResources.GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, nameof(codePagePriorityList));
        }
      }
      return this.detector.GetCodePages(codePagePriorityList, fallbackExceptions > FallbackExceptions.None, fallbackExceptions > FallbackExceptions.Common, onlyValidCodePages);
    }

    public int GetCodePageCoverage(int codePage)
    {
      Charset charset = Charset.GetCharset(codePage);
      if (charset.UnicodeCoverage == CodePageUnicodeCoverage.Complete)
        return 100;
      if (!charset.IsDetectable)
      {
        if (charset.DetectableCodePageWithEquivalentCoverage == 0)
          throw new ArgumentException("codePage is not detectable");
        codePage = charset.DetectableCodePageWithEquivalentCoverage;
      }
      return this.detector.GetCodePageCoverage(codePage);
    }

    public int GetBestWindowsCodePage()
    {
      return this.detector.GetBestWindowsCodePage(false, false);
    }

    internal int GetBestWindowsCodePage(int preferredCodePage)
    {
      return this.detector.GetBestWindowsCodePage(false, false, preferredCodePage);
    }
  }
}
