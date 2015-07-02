// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.CodePageMap
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Globalization
{
  internal class CodePageMap : CodePageMapData
  {
    private int codePage;
    private CodePageMapData.CodePageRange[] ranges;
    private int lastRangeIndex;
    private CodePageMapData.CodePageRange lastRange;

    public bool ChoseCodePage(int codePage)
    {
      if (codePage == this.codePage)
        return true;
      this.codePage = codePage;
      this.ranges = (CodePageMapData.CodePageRange[]) null;
      if (codePage == 1200)
        return true;
      for (int index = CodePageMapData.codePages.Length - 1; index >= 0; --index)
      {
        if ((int) CodePageMapData.codePages[index].cpid == codePage)
        {
          this.ranges = CodePageMapData.codePages[index].ranges;
          this.lastRangeIndex = this.ranges.Length / 2;
          this.lastRange = this.ranges[this.lastRangeIndex];
          return true;
        }
      }
      return false;
    }

    public bool ChoseCodePage(Encoding encoding)
    {
      return this.ChoseCodePage(CodePageMap.GetCodePage(encoding));
    }

    public bool IsUnsafeExtendedCharacter(char ch)
    {
      if (this.ranges == null)
        return false;
      if ((int) ch <= (int) this.lastRange.last)
      {
        if ((int) ch >= (int) this.lastRange.first)
        {
          if ((int) this.lastRange.offset != (int) ushort.MaxValue)
            return ((int) CodePageMapData.bitmap[(int) this.lastRange.offset + ((int) ch - (int) this.lastRange.first)] & (int) this.lastRange.mask) == 0;
          return false;
        }
        int index = this.lastRangeIndex;
        while (--index >= 0)
        {
          if ((int) ch >= (int) this.ranges[index].first)
          {
            if ((int) ch <= (int) this.ranges[index].last)
            {
              if ((int) ch == (int) this.ranges[index].first)
                return false;
              this.lastRangeIndex = index;
              this.lastRange = this.ranges[index];
              if ((int) this.lastRange.offset != (int) ushort.MaxValue)
                return ((int) CodePageMapData.bitmap[(int) this.lastRange.offset + ((int) ch - (int) this.lastRange.first)] & (int) this.lastRange.mask) == 0;
              return false;
            }
            break;
          }
        }
      }
      else
      {
        int index = this.lastRangeIndex;
        while (++index < this.ranges.Length)
        {
          if ((int) ch <= (int) this.ranges[index].last)
          {
            if ((int) ch >= (int) this.ranges[index].first)
            {
              if ((int) ch == (int) this.ranges[index].first)
                return false;
              this.lastRangeIndex = index;
              this.lastRange = this.ranges[index];
              if ((int) this.lastRange.offset != (int) ushort.MaxValue)
                return ((int) CodePageMapData.bitmap[(int) this.lastRange.offset + ((int) ch - (int) this.lastRange.first)] & (int) this.lastRange.mask) == 0;
              return false;
            }
            break;
          }
        }
      }
      return true;
    }

    public static Encoding GetEncoding(int codePage)
    {
      return Encoding.GetEncoding(codePage);
    }

    public static int GetCodePage(Encoding encoding)
    {
      return encoding.CodePage;
    }

    public static int GetWindowsCodePage(Encoding encoding)
    {
      return encoding.WindowsCodePage;
    }
  }
}
