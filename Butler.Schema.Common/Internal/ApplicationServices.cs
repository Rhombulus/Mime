using System.Linq;

namespace Butler.Schema.Internal {

    internal static class ApplicationServices {

        public static IApplicationServices Provider { get; } = ApplicationServices.LoadServices();

        public static CtsConfigurationSetting GetSimpleConfigurationSetting(string subSectionName, string settingName) {
            CtsConfigurationSetting configurationSetting1 = null;

            foreach (var configurationSetting2 in ApplicationServices.Provider.GetConfiguration(subSectionName)
                                                                     .Where(configurationSetting2 => string.Equals(configurationSetting2.Name, settingName, System.StringComparison.OrdinalIgnoreCase))) {
                if (configurationSetting1 != null) {
                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                    break;
                }
                configurationSetting1 = configurationSetting2;
            }
            return configurationSetting1;
        }

        internal static int ParseIntegerSetting(CtsConfigurationSetting setting, int defaultValue, int min, bool kilobytes) {
            if (setting.Arguments.Count != 1 || !setting.Arguments[0].Name.Equals("Value", System.StringComparison.OrdinalIgnoreCase)) {
                ApplicationServices.Provider.LogConfigurationErrorEvent();
                return defaultValue;
            }
            if (setting.Arguments[0].Value.Trim()
                                    .Equals("unlimited", System.StringComparison.OrdinalIgnoreCase))
                return int.MaxValue;
            int result;
            if (!int.TryParse(setting.Arguments[0].Value.Trim(), out result)) {
                ApplicationServices.Provider.LogConfigurationErrorEvent();
                return defaultValue;
            }
            if (result < min) {
                ApplicationServices.Provider.LogConfigurationErrorEvent();
                return defaultValue;
            }
            if (kilobytes) {
                if (result > 2097151)
                    result = int.MaxValue;
                else
                    result *= 1024;
            }
            return result;
        }

        private static IApplicationServices LoadServices() {
            return new DefaultApplicationServices();
        }

    }

}