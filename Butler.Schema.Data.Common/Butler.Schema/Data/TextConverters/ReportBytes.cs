// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ReportBytes
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class ReportBytes : IReportBytes
  {
    private static bool isReadFromConfiguration = false;
    private static int configExpansionSizeLimit = -1;
    private static int configExpansionSizeMultiple = -1;
    private const int DefaultExpansionSizeLimit = 524288;
    private const int DefaultExpansionSizeMultiple = 10;
    private int expansionSizeLimit;
    private int expansionSizeMultiple;
    private long bytesRead;
    private long bytesWritten;

    internal ReportBytes()
      : this(0, 0)
    {
    }

    internal ReportBytes(int expansionSizeLimit, int expansionSizeMultiple)
    {
      if (expansionSizeLimit > 0 && expansionSizeMultiple > 0)
      {
        this.expansionSizeLimit = expansionSizeLimit;
        this.expansionSizeMultiple = expansionSizeMultiple;
      }
      else
      {
        if (!ReportBytes.isReadFromConfiguration)
        {
          Data.Internal.CtsConfigurationSetting configurationSetting1 = Data.Internal.ApplicationServices.GetSimpleConfigurationSetting("TextConverters", "ExpansionSizeLimit");
          ReportBytes.configExpansionSizeLimit = configurationSetting1 == null ? 524288 : Data.Internal.ApplicationServices.ParseIntegerSetting(configurationSetting1, 524288, 1, true);
          Data.Internal.CtsConfigurationSetting configurationSetting2 = Data.Internal.ApplicationServices.GetSimpleConfigurationSetting("TextConverters", "ExpansionSizeMultiple");
          ReportBytes.configExpansionSizeMultiple = configurationSetting2 == null ? 10 : Data.Internal.ApplicationServices.ParseIntegerSetting(configurationSetting2, 10, 5, false);
          ReportBytes.isReadFromConfiguration = true;
        }
        this.expansionSizeLimit = ReportBytes.configExpansionSizeLimit;
        this.expansionSizeMultiple = ReportBytes.configExpansionSizeMultiple;
      }
      this.bytesRead = 0L;
      this.bytesWritten = 0L;
    }

    public void ReportBytesRead(int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      this.bytesRead += (long) count;
      this.CheckBytes();
    }

    public void ReportBytesWritten(int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      this.bytesWritten += (long) count;
      this.CheckBytes();
    }

    private void CheckBytes()
    {
      if (this.bytesRead == 0L)
        return;
      if (this.bytesRead < 0L)
        throw new InvalidOperationException("ReportBytes.bytesRead < 0");
      if (this.bytesWritten < 0L)
        throw new InvalidOperationException("ReportBytes.bytesWritten < 0");
      if (this.bytesWritten > (long) this.expansionSizeLimit && this.bytesWritten / this.bytesRead > (long) this.expansionSizeMultiple)
        throw new TextConvertersException(CtsResources.TextConvertersStrings.DocumentGrowingExcessively(this.expansionSizeMultiple));
    }
  }
}
