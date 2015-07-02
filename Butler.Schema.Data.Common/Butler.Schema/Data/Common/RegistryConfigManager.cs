// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.RegistryConfigManager
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using Microsoft.Win32;

namespace Butler.Schema.Data.Common
{
  public class RegistryConfigManager
  {
    internal static readonly long RegistryReadIntervalTicks = 600000000L;
    private static int iso2022JpEncodingOverride = 1;
    private static int htmlEncapsulationOverride = 1;
    internal const string RegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics";
    internal const string Iso2022JpEncodingOverrideRegistryValueName = "Iso2022JpEncodingOverride";
    internal const string HtmlEncapsulationOverrideRegistryValueName = "HtmlEncapsulationOverride";
    private static long lastAccessTicks;

    internal static int Iso2022JpEncodingOverride
    {
      get
      {
        RegistryConfigManager.ReadAllConfigsIfRequired();
        return RegistryConfigManager.iso2022JpEncodingOverride;
      }
    }

    internal static bool HtmlEncapsulationOverride
    {
      get
      {
        RegistryConfigManager.ReadAllConfigsIfRequired();
        return RegistryConfigManager.htmlEncapsulationOverride != 0;
      }
    }

    private static void ReadAllConfigsIfRequired()
    {
      long ticks = DateTime.UtcNow.Ticks;
      if (RegistryConfigManager.lastAccessTicks != 0L)
      {
        if (RegistryConfigManager.lastAccessTicks <= ticks)
        {
          if (ticks - RegistryConfigManager.lastAccessTicks <= RegistryConfigManager.RegistryReadIntervalTicks)
            return;
        }
      }
      try
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics"))
        {
          if (registryKey == null)
            return;
          object obj1 = registryKey.GetValue("Iso2022JpEncodingOverride");
          if (obj1 != null && obj1 is int)
            RegistryConfigManager.iso2022JpEncodingOverride = (int) obj1;
          object obj2 = registryKey.GetValue("HtmlEncapsulationOverride");
          if (obj2 == null || !(obj2 is int))
            return;
          RegistryConfigManager.htmlEncapsulationOverride = (int) obj2;
        }
      }
      catch (Exception ex)
      {
      }
      finally
      {
        RegistryConfigManager.lastAccessTicks = ticks;
      }
    }
  }
}
