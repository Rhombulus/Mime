// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.SimpleCodepageDetector
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization
{
  internal struct SimpleCodepageDetector
  {
    private uint mask;

    public void AddData(char ch)
    {
      this.mask |= ~CodePageDetectData.codePageMask[(int) CodePageDetectData.index[(int) ch]];
    }

    public void AddData(char[] buffer, int offset, int count)
    {
      ++count;
      while (--count != 0)
      {
        this.mask |= ~CodePageDetectData.codePageMask[(int) CodePageDetectData.index[(int) buffer[offset]]];
        ++offset;
      }
    }

    public void AddData(string buffer, int offset, int count)
    {
      ++count;
      while (--count != 0)
      {
        this.mask |= ~CodePageDetectData.codePageMask[(int) CodePageDetectData.index[(int) buffer[offset]]];
        ++offset;
      }
    }

    public int GetCodePage(int[] codePagePriorityList, bool onlyValidCodePages)
    {
      uint cumulativeMask = ~this.mask;
      if (onlyValidCodePages)
        cumulativeMask &= CodePageDetect.ValidCodePagesMask;
      return CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
    }

    public int GetWindowsCodePage(int[] codePagePriorityList, bool onlyValidCodePages)
    {
      uint cumulativeMask = ~this.mask & 260038656U;
      if (onlyValidCodePages)
        cumulativeMask &= CodePageDetect.ValidCodePagesMask;
      if ((int) cumulativeMask == 0)
        return 1252;
      return CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
    }
  }
}
