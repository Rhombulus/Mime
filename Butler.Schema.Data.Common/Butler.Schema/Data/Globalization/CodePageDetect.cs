// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.CodePageDetect
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Globalization
{
  internal struct CodePageDetect
  {
    internal static uint ValidCodePagesMask = CodePageDetect.InitializeValidCodePagesMask();
    private int[] maskMap;
    private int[] codePageList;

    public static bool IsCodePageDetectable(int cpid, bool validOnly)
    {
      byte num = CodePageDetectData.codePageIndex[cpid % CodePageDetectData.codePageIndex.Length];
      if ((int) num == (int) byte.MaxValue || (int) CodePageDetectData.codePages[(int) num].cpid != cpid && ((int) CodePageDetectData.codePages[(int) num].cpid != 38598 || cpid != 28598) && ((int) CodePageDetectData.codePages[(int) num].cpid != 936 || cpid != 54936))
        return false;
      if (!validOnly)
        return true;
      return 0 != ((int) CodePageDetect.ValidCodePagesMask & (int) CodePageDetectData.codePages[(int) num].mask);
    }

    public static char[] GetCommonExceptionCharacters()
    {
      return CodePageDetectData.commonExceptions.Clone() as char[];
    }

    public static int[] GetDefaultPriorityList()
    {
      int[] numArray = new int[CodePageDetectData.codePages.Length];
      for (int index = 0; index < CodePageDetectData.codePages.Length; ++index)
        numArray[index] = (int) CodePageDetectData.codePages[index].cpid;
      return numArray;
    }

    public void Initialize()
    {
      this.maskMap = new int[CodePageDetectData.codePageMask.Length];
    }

    public void Reset()
    {
      for (int index = 0; index < this.maskMap.Length; ++index)
        this.maskMap[index] = 0;
    }

    public void AddData(char ch)
    {
      ++this.maskMap[(int) CodePageDetectData.index[(int) ch]];
    }

    public void AddData(char[] buffer, int offset, int count)
    {
      ++count;
      while (--count != 0)
      {
        ++this.maskMap[(int) CodePageDetectData.index[(int) buffer[offset]]];
        ++offset;
      }
    }

    public void AddData(string buffer, int offset, int count)
    {
      ++count;
      while (--count != 0)
      {
        ++this.maskMap[(int) CodePageDetectData.index[(int) buffer[offset]]];
        ++offset;
      }
    }

    public void AddData(TextReader reader, int maxCharacters)
    {
      char[] buffer = new char[1024];
      while (maxCharacters != 0)
      {
        int count = Math.Min(buffer.Length, maxCharacters);
        int num1 = reader.Read(buffer, 0, count);
        if (num1 == 0)
          break;
        int index = 0;
        maxCharacters -= num1;
        int num2 = num1 + 1;
        while (--num2 != 0)
        {
          ++this.maskMap[(int) CodePageDetectData.index[(int) buffer[index]]];
          ++index;
        }
      }
    }

    public int GetCodePage(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages)
    {
      uint cumulativeMask = uint.MaxValue;
      for (int index = 0; index < this.maskMap.Length; ++index)
      {
        if (this.maskMap[index] != 0)
        {
          uint num = CodePageDetectData.codePageMask[index];
          if (allowAnyFallbackExceptions || allowCommonFallbackExceptions && ((int) CodePageDetectData.fallbackMask[index] & 1) != 0)
            num |= CodePageDetectData.fallbackMask[index] & 4294967294U;
          cumulativeMask &= num;
          if ((int) cumulativeMask == 0)
            break;
        }
      }
      if (onlyValidCodePages)
        cumulativeMask &= CodePageDetect.ValidCodePagesMask;
      return CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
    }

    public int[] GetCodePages(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages)
    {
      uint cumulativeMask = uint.MaxValue;
      for (int index = 0; index < this.maskMap.Length; ++index)
      {
        if (this.maskMap[index] != 0)
        {
          uint num = CodePageDetectData.codePageMask[index];
          if (allowAnyFallbackExceptions || allowCommonFallbackExceptions && ((int) CodePageDetectData.fallbackMask[index] & 1) != 0)
            num |= CodePageDetectData.fallbackMask[index] & 4294967294U;
          cumulativeMask &= num;
          if ((int) cumulativeMask == 0)
            break;
        }
      }
      if (onlyValidCodePages)
        cumulativeMask &= CodePageDetect.ValidCodePagesMask;
      int[] numArray = new int[CodePageDetect.GetCodePageCount(cumulativeMask)];
      int index1 = 0;
      while ((int) cumulativeMask != 0)
        numArray[index1++] = CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
      numArray[index1] = 65001;
      return numArray;
    }

    public int GetCodePageCoverage(int codePage)
    {
      uint num1 = 0U;
      int num2 = 0;
      int num3 = 0;
      byte num4 = CodePageDetectData.codePageIndex[codePage % CodePageDetectData.codePageIndex.Length];
      if ((int) num4 != (int) byte.MaxValue && ((int) CodePageDetectData.codePages[(int) num4].cpid == codePage || (int) CodePageDetectData.codePages[(int) num4].cpid == 38598 && codePage == 28598 || (int) CodePageDetectData.codePages[(int) num4].cpid == 936 && codePage == 54936))
        num1 = CodePageDetectData.codePages[(int) num4].mask;
      if ((int) num1 != 0)
      {
        for (int index = 0; index < this.maskMap.Length; ++index)
        {
          if ((int) num1 == ((int) CodePageDetectData.codePageMask[index] & (int) num1))
            num2 += this.maskMap[index];
          num3 += this.maskMap[index];
        }
      }
      if (num3 != 0)
        return (int) ((long) num2 * 100L / (long) num3);
      return 0;
    }

    public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions)
    {
      return this.GetBestWindowsCodePage(allowCommonFallbackExceptions, allowAnyFallbackExceptions, 0);
    }

    public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, int preferredCodePage)
    {
      int num1 = 0;
      if (this.codePageList == null)
      {
        this.codePageList = new int[CodePageDetectData.codePages.Length + 1];
      }
      else
      {
        for (int index = 0; index < this.codePageList.Length; ++index)
          this.codePageList[index] = 0;
      }
      for (int index1 = 0; index1 < this.maskMap.Length; ++index1)
      {
        if (this.maskMap[index1] != 0)
        {
          uint num2 = CodePageDetectData.codePageMask[index1];
          if (allowAnyFallbackExceptions || allowCommonFallbackExceptions && ((int) CodePageDetectData.fallbackMask[index1] & 1) != 0)
            num2 |= CodePageDetectData.fallbackMask[index1] & 4294967294U;
          uint num3 = num2 & 260038656U;
          if ((int) num3 != 0)
          {
            int index2 = 0;
            while ((int) num3 != 0)
            {
              if (((int) num3 & 1) != 0)
                this.codePageList[index2] += this.maskMap[index1];
              ++index2;
              num3 >>= 1;
            }
          }
          num1 += this.maskMap[index1];
        }
      }
      int num4 = 0;
      int index3 = 0;
      uint num5 = 260038656U;
      int index4 = 0;
      while ((int) num5 != 0)
      {
        if (CodePageDetectData.codePages[index4].windowsCodePage && this.codePageList[index4] > num4)
        {
          num4 = this.codePageList[index4];
          index3 = index4;
        }
        else if (this.codePageList[index4] == num4 && ((int) CodePageDetectData.codePages[index4].cpid == 1252 || (int) CodePageDetectData.codePages[index4].cpid == preferredCodePage))
          index3 = index4;
        num5 &= ~CodePageDetectData.codePages[index4].mask;
        ++index4;
      }
      return (int) CodePageDetectData.codePages[index3].cpid;
    }

    internal static int GetCodePage(ref uint cumulativeMask, int[] codePagePriorityList)
    {
      if ((int) cumulativeMask != 0)
      {
        if (codePagePriorityList != null)
        {
          for (int index = 0; index < codePagePriorityList.Length; ++index)
          {
            byte num = CodePageDetectData.codePageIndex[codePagePriorityList[index] % CodePageDetectData.codePageIndex.Length];
            if ((int) num != (int) byte.MaxValue && ((int) cumulativeMask & (int) CodePageDetectData.codePages[(int) num].mask) != 0 && ((int) CodePageDetectData.codePages[(int) num].cpid == codePagePriorityList[index] || (int) CodePageDetectData.codePages[(int) num].cpid == 38598 && codePagePriorityList[index] == 28598 || (int) CodePageDetectData.codePages[(int) num].cpid == 936 && codePagePriorityList[index] == 54936))
            {
              cumulativeMask &= ~CodePageDetectData.codePages[(int) num].mask;
              return codePagePriorityList[index];
            }
          }
        }
        for (int index = 0; index < CodePageDetectData.codePages.Length; ++index)
        {
          if (((int) cumulativeMask & (int) CodePageDetectData.codePages[index].mask) != 0)
          {
            cumulativeMask &= ~CodePageDetectData.codePages[index].mask;
            return (int) CodePageDetectData.codePages[index].cpid;
          }
        }
      }
      return 65001;
    }

    internal static int GetCodePageCount(uint cumulativeMask)
    {
      int num = 1;
      while ((int) cumulativeMask != 0)
      {
        if (((int) cumulativeMask & 1) != 0)
          ++num;
        cumulativeMask >>= 1;
      }
      return num;
    }

    private static uint InitializeValidCodePagesMask()
    {
      uint num = 0U;
      for (int index = 0; index < CodePageDetectData.codePages.Length; ++index)
      {
        Charset charset;
        if (Charset.TryGetCharset((int) CodePageDetectData.codePages[index].cpid, out charset) && charset.IsAvailable)
          num |= CodePageDetectData.codePages[index].mask;
      }
      return num;
    }
  }
}
