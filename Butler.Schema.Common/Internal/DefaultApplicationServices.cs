// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.DefaultApplicationServices
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal class DefaultApplicationServices : IApplicationServices
  {
    private IList<CtsConfigurationSetting> emptySubSection = (IList<CtsConfigurationSetting>) new List<CtsConfigurationSetting>();
    private object lockObject = new object();
    private volatile Dictionary<string, IList<CtsConfigurationSetting>> configurationSubSections;

    public static Stream CreateTemporaryStorage(Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
    {
      TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage(acquireBuffer, releaseBuffer);
      Stream stream = (Stream) temporaryDataStorage.OpenWriteStream(false);
      temporaryDataStorage.Release();
      return stream;
    }

    public Stream CreateTemporaryStorage()
    {
      return DefaultApplicationServices.CreateTemporaryStorage((Func<int, byte[]>) null, (Action<byte[]>) null);
    }

    public IList<CtsConfigurationSetting> GetConfiguration(string subSectionName)
    {
      IList<CtsConfigurationSetting> list;
      if (!this.GetConfigurationSubSections().TryGetValue(subSectionName ?? string.Empty, out list))
        return this.emptySubSection;
      return list;
    }

    public void LogConfigurationErrorEvent()
    {
    }

    public void RefreshConfiguration()
    {
      ConfigurationManager.RefreshSection("appSettings");
      ConfigurationManager.RefreshSection("CTS");
      lock (this.lockObject)
        this.configurationSubSections = (Dictionary<string, IList<CtsConfigurationSetting>>) null;
    }

    private Dictionary<string, IList<CtsConfigurationSetting>> GetConfigurationSubSections()
    {
      Dictionary<string, IList<CtsConfigurationSetting>> dictionary = this.configurationSubSections;
      if (dictionary == null)
      {
        CtsConfigurationSection configurationSection = (CtsConfigurationSection) null;
        try
        {
          configurationSection = ConfigurationManager.GetSection("CTS") as CtsConfigurationSection;
        }
        catch (ConfigurationErrorsException ex)
        {
          this.LogConfigurationErrorEvent();
        }
        CtsConfigurationSetting configurationSetting = (CtsConfigurationSetting) null;
        try
        {
          string str = ConfigurationManager.AppSettings["TemporaryStoragePath"];
          if (!string.IsNullOrEmpty(str))
          {
            configurationSetting = new CtsConfigurationSetting("TemporaryStorage");
            configurationSetting.AddArgument("Path", str);
          }
        }
        catch (ConfigurationErrorsException ex)
        {
          this.LogConfigurationErrorEvent();
        }
        lock (this.lockObject)
        {
          dictionary = this.configurationSubSections;
          if (dictionary == null)
          {
            if (configurationSection != null)
            {
              dictionary = configurationSection.SubSectionsDictionary;
            }
            else
            {
              dictionary = new Dictionary<string, IList<CtsConfigurationSetting>>();
              dictionary.Add(string.Empty, (IList<CtsConfigurationSetting>) new List<CtsConfigurationSetting>());
            }
            if (configurationSetting != null)
            {
              IList<CtsConfigurationSetting> local_4 = dictionary[string.Empty];
              bool local_5 = false;
              foreach (CtsConfigurationSetting item_0 in (IEnumerable<CtsConfigurationSetting>) local_4)
              {
                if (string.Equals(item_0.Name, configurationSetting.Name))
                {
                  local_5 = true;
                  break;
                }
              }
              if (!local_5)
                local_4.Add(configurationSetting);
            }
            this.configurationSubSections = dictionary;
          }
        }
      }
      return dictionary;
    }
  }
}
