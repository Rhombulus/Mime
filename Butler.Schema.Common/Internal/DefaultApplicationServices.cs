namespace Butler.Schema.Internal {

    internal class DefaultApplicationServices : IApplicationServices {

        public System.IO.Stream CreateTemporaryStorage() {
            return DefaultApplicationServices.CreateTemporaryStorage(null, null);
        }

        public System.Collections.Generic.IList<CtsConfigurationSetting> GetConfiguration(string subSectionName) {
            System.Collections.Generic.IList<CtsConfigurationSetting> list;
            if (!this.GetConfigurationSubSections()
                     .TryGetValue(subSectionName ?? string.Empty, out list))
                return emptySubSection;
            return list;
        }

        public void LogConfigurationErrorEvent() {}

        public void RefreshConfiguration() {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            System.Configuration.ConfigurationManager.RefreshSection("CTS");
            lock (lockObject)
                configurationSubSections = null;
        }

        public static System.IO.Stream CreateTemporaryStorage(System.Func<int, byte[]> acquireBuffer, System.Action<byte[]> releaseBuffer) {
            var temporaryDataStorage = new TemporaryDataStorage(acquireBuffer, releaseBuffer);
            System.IO.Stream stream = temporaryDataStorage.OpenWriteStream(false);
            temporaryDataStorage.Release();
            return stream;
        }

        private System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<CtsConfigurationSetting>> GetConfigurationSubSections() {
            var dictionary = configurationSubSections;
            if (dictionary == null) {
                CtsConfigurationSection configurationSection = null;
                try {
                    configurationSection = System.Configuration.ConfigurationManager.GetSection("CTS") as CtsConfigurationSection;
                } catch (System.Configuration.ConfigurationErrorsException ex) {
                    this.LogConfigurationErrorEvent();
                }
                CtsConfigurationSetting configurationSetting = null;
                try {
                    var str = System.Configuration.ConfigurationManager.AppSettings["TemporaryStoragePath"];
                    if (!string.IsNullOrEmpty(str)) {
                        configurationSetting = new CtsConfigurationSetting("TemporaryStorage");
                        configurationSetting.AddArgument("Path", str);
                    }
                } catch (System.Configuration.ConfigurationErrorsException ex) {
                    this.LogConfigurationErrorEvent();
                }
                lock (lockObject) {
                    dictionary = configurationSubSections;
                    if (dictionary == null) {
                        if (configurationSection != null)
                            dictionary = configurationSection.SubSectionsDictionary;
                        else {
                            dictionary = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<CtsConfigurationSetting>>();
                            dictionary.Add(string.Empty, new System.Collections.Generic.List<CtsConfigurationSetting>());
                        }
                        if (configurationSetting != null) {
                            var local_4 = dictionary[string.Empty];
                            var local_5 = false;
                            foreach (var item_0 in local_4) {
                                if (string.Equals(item_0.Name, configurationSetting.Name)) {
                                    local_5 = true;
                                    break;
                                }
                            }
                            if (!local_5)
                                local_4.Add(configurationSetting);
                        }
                        configurationSubSections = dictionary;
                    }
                }
            }
            return dictionary;
        }

        private readonly System.Collections.Generic.IList<CtsConfigurationSetting> emptySubSection = new System.Collections.Generic.List<CtsConfigurationSetting>();
        private readonly object lockObject = new object();
        private volatile System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<CtsConfigurationSetting>> configurationSubSections;

    }

}