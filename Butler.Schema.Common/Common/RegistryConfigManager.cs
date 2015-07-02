namespace Butler.Schema.Common {

    public class RegistryConfigManager {

        internal static int Iso2022JpEncodingOverride {
            get {
                RegistryConfigManager.ReadAllConfigsIfRequired();
                return iso2022JpEncodingOverride;
            }
        }

        internal static bool HtmlEncapsulationOverride {
            get {
                RegistryConfigManager.ReadAllConfigsIfRequired();
                return htmlEncapsulationOverride != 0;
            }
        }

        private static void ReadAllConfigsIfRequired() {
            var ticks = System.DateTime.UtcNow.Ticks;
            if (lastAccessTicks != 0L) {
                if (lastAccessTicks <= ticks) {
                    if (ticks - lastAccessTicks <= RegistryReadIntervalTicks)
                        return;
                }
            }
            try {
                using (var registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics")) {
                    if (registryKey == null)
                        return;
                    var obj1 = registryKey.GetValue("Iso2022JpEncodingOverride");
                    if (obj1 != null && obj1 is int)
                        iso2022JpEncodingOverride = (int) obj1;
                    var obj2 = registryKey.GetValue("HtmlEncapsulationOverride");
                    if (obj2 == null || !(obj2 is int))
                        return;
                    htmlEncapsulationOverride = (int) obj2;
                }
            } catch (System.Exception ex) {} finally {
                lastAccessTicks = ticks;
            }
        }

        internal const string RegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics";
        internal const string Iso2022JpEncodingOverrideRegistryValueName = "Iso2022JpEncodingOverride";
        internal const string HtmlEncapsulationOverrideRegistryValueName = "HtmlEncapsulationOverride";
        internal static readonly long RegistryReadIntervalTicks = 600000000L;
        private static int iso2022JpEncodingOverride = 1;
        private static int htmlEncapsulationOverride = 1;
        private static long lastAccessTicks;

    }

}