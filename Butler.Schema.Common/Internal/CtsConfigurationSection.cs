// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.CtsConfigurationSection
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace Butler.Schema.Data.Internal
{
  internal sealed class CtsConfigurationSection : ConfigurationSection
  {

      private static ConfigurationPropertyCollection properties;

    public Dictionary<string, IList<CtsConfigurationSetting>> SubSectionsDictionary { get; } = new Dictionary<string, IList<CtsConfigurationSetting>>();

      protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (CtsConfigurationSection.properties == null)
          CtsConfigurationSection.properties = new ConfigurationPropertyCollection();
        return CtsConfigurationSection.properties;
      }
    }

    protected override void DeserializeSection(XmlReader reader)
    {
      IList<CtsConfigurationSetting> list1 = (IList<CtsConfigurationSetting>) new List<CtsConfigurationSetting>();
      this.SubSectionsDictionary.Add(string.Empty, list1);
      if (!reader.Read() || reader.NodeType != XmlNodeType.Element)
        throw new ConfigurationErrorsException("error", reader);
      if (reader.IsEmptyElement)
        return;
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.IsEmptyElement)
          {
            CtsConfigurationSetting configurationSetting = this.DeserializeSetting(reader);
            list1.Add(configurationSetting);
          }
          else
          {
            string name = reader.Name;
            IList<CtsConfigurationSetting> list2;
            if (!this.SubSectionsDictionary.TryGetValue(name, out list2))
            {
              list2 = (IList<CtsConfigurationSetting>) new List<CtsConfigurationSetting>();
              this.SubSectionsDictionary.Add(name, list2);
            }
            while (reader.Read())
            {
              if (reader.NodeType == XmlNodeType.Element)
              {
                if (!reader.IsEmptyElement)
                  throw new ConfigurationErrorsException("error", reader);
                CtsConfigurationSetting configurationSetting = this.DeserializeSetting(reader);
                list2.Add(configurationSetting);
              }
              else if (reader.NodeType != XmlNodeType.EndElement)
              {
                if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
                  throw new ConfigurationErrorsException("error", reader);
              }
              else
                break;
            }
          }
        }
        else
        {
          if (reader.NodeType == XmlNodeType.EndElement)
            break;
          if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
            throw new ConfigurationErrorsException("error", reader);
        }
      }
    }

    private CtsConfigurationSetting DeserializeSetting(XmlReader reader)
    {
      CtsConfigurationSetting configurationSetting = new CtsConfigurationSetting(reader.Name);
      if (reader.AttributeCount > 0)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          string str = reader.Value;
          configurationSetting.AddArgument(name, str);
        }
      }
      return configurationSetting;
    }
  }
}
