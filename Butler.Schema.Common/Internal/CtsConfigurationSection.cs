namespace Butler.Schema.Data.Internal {

    internal sealed class CtsConfigurationSection : System.Configuration.ConfigurationSection {

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<CtsConfigurationSetting>> SubSectionsDictionary { get; } =
            new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<CtsConfigurationSetting>>();

        protected override System.Configuration.ConfigurationPropertyCollection Properties {
            get {
                if (properties == null)
                    properties = new System.Configuration.ConfigurationPropertyCollection();
                return properties;
            }
        }

        protected override void DeserializeSection(System.Xml.XmlReader reader) {
            System.Collections.Generic.IList<CtsConfigurationSetting> list1 = new System.Collections.Generic.List<CtsConfigurationSetting>();
            this.SubSectionsDictionary.Add(string.Empty, list1);
            if (!reader.Read() || reader.NodeType != System.Xml.XmlNodeType.Element)
                throw new System.Configuration.ConfigurationErrorsException("error", reader);
            if (reader.IsEmptyElement)
                return;
            while (reader.Read()) {
                if (reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (reader.IsEmptyElement) {
                        var configurationSetting = this.DeserializeSetting(reader);
                        list1.Add(configurationSetting);
                    } else {
                        var name = reader.Name;
                        System.Collections.Generic.IList<CtsConfigurationSetting> list2;
                        if (!this.SubSectionsDictionary.TryGetValue(name, out list2)) {
                            list2 = new System.Collections.Generic.List<CtsConfigurationSetting>();
                            this.SubSectionsDictionary.Add(name, list2);
                        }
                        while (reader.Read()) {
                            if (reader.NodeType == System.Xml.XmlNodeType.Element) {
                                if (!reader.IsEmptyElement)
                                    throw new System.Configuration.ConfigurationErrorsException("error", reader);
                                var configurationSetting = this.DeserializeSetting(reader);
                                list2.Add(configurationSetting);
                            } else if (reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                if (reader.NodeType == System.Xml.XmlNodeType.CDATA || reader.NodeType == System.Xml.XmlNodeType.Text)
                                    throw new System.Configuration.ConfigurationErrorsException("error", reader);
                            } else
                                break;
                        }
                    }
                } else {
                    if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
                        break;
                    if (reader.NodeType == System.Xml.XmlNodeType.CDATA || reader.NodeType == System.Xml.XmlNodeType.Text)
                        throw new System.Configuration.ConfigurationErrorsException("error", reader);
                }
            }
        }

        private CtsConfigurationSetting DeserializeSetting(System.Xml.XmlReader reader) {
            var configurationSetting = new CtsConfigurationSetting(reader.Name);
            if (reader.AttributeCount > 0) {
                while (reader.MoveToNextAttribute()) {
                    var name = reader.Name;
                    var str = reader.Value;
                    configurationSetting.AddArgument(name, str);
                }
            }
            return configurationSetting;
        }

        private static System.Configuration.ConfigurationPropertyCollection properties;

    }

}