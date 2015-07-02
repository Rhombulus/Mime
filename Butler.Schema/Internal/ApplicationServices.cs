// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.ApplicationServices
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal static class ApplicationServices
  {
    private static IApplicationServices provider = ApplicationServices.LoadServices();

    public static IApplicationServices Provider => ApplicationServices.provider;

      public static CtsConfigurationSetting GetSimpleConfigurationSetting(string subSectionName, string settingName)
    {
      CtsConfigurationSetting configurationSetting1 = (CtsConfigurationSetting) null;
      foreach (CtsConfigurationSetting configurationSetting2 in (IEnumerable<CtsConfigurationSetting>) ApplicationServices.Provider.GetConfiguration(subSectionName))
      {
        if (string.Equals(configurationSetting2.Name, settingName, StringComparison.OrdinalIgnoreCase))
        {
          if (configurationSetting1 != null)
          {
            ApplicationServices.Provider.LogConfigurationErrorEvent();
            break;
          }
          configurationSetting1 = configurationSetting2;
        }
      }
      return configurationSetting1;
    }

    internal static int ParseIntegerSetting(CtsConfigurationSetting setting, int defaultValue, int min, bool kilobytes)
    {
      if (setting.Arguments.Count != 1 || !setting.Arguments[0].Name.Equals("Value", StringComparison.OrdinalIgnoreCase))
      {
        ApplicationServices.Provider.LogConfigurationErrorEvent();
        return defaultValue;
      }
      if (setting.Arguments[0].Value.Trim().Equals("unlimited", StringComparison.OrdinalIgnoreCase))
        return int.MaxValue;
      int result;
      if (!int.TryParse(setting.Arguments[0].Value.Trim(), out result))
      {
        ApplicationServices.Provider.LogConfigurationErrorEvent();
        return defaultValue;
      }
      if (result < min)
      {
        ApplicationServices.Provider.LogConfigurationErrorEvent();
        return defaultValue;
      }
      if (kilobytes)
      {
        if (result > 2097151)
          result = int.MaxValue;
        else
          result *= 1024;
      }
      return result;
    }

    private static IApplicationServices LoadServices()
    {
      return (IApplicationServices) new DefaultApplicationServices();
    }
  }
}
