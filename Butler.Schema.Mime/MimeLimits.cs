using System.Linq;

namespace Butler.Schema.Mime {

    public class MimeLimits {

        internal MimeLimits(int partDepth, int embeddedDepth, int size, int headerBytes, int parts, int headers, int addressItemsPerHeader, int textValueBytesPerValue, int parametersPerHeader, int encodedWordLength) {
            this.MaxPartDepth = partDepth;
            this.MaxEmbeddedDepth = embeddedDepth;
            this.MaxSize = size;
            this.MaxHeaderBytes = headerBytes;
            this.MaxParts = parts;
            this.MaxHeaders = headers;
            this.MaxAddressItemsPerHeader = addressItemsPerHeader;
            this.MaxTextValueBytesPerValue = textValueBytesPerValue;
            this.MaxParametersPerHeader = parametersPerHeader;
            this.MaxEncodedWordLength = encodedWordLength;
        }

        private MimeLimits() {}
        public static MimeLimits Default => MimeLimits.GetDefaultMimeDocumentLimits();
        public static MimeLimits Unlimited { get; } = new MimeLimits();
        public int MaxPartDepth { get; } = int.MaxValue;
        public int MaxEmbeddedDepth { get; } = int.MaxValue;
        public int MaxSize { get; } = int.MaxValue;
        public int MaxHeaderBytes { get; } = int.MaxValue;
        public int MaxParts { get; } = int.MaxValue;
        public int MaxHeaders { get; } = int.MaxValue;
        public int MaxAddressItemsPerHeader { get; } = int.MaxValue;
        public int MaxTextValueBytesPerValue { get; } = int.MaxValue;
        public int MaxParametersPerHeader { get; } = int.MaxValue;
        internal int MaxEncodedWordLength { get; }

        internal static void RefreshConfiguration() {
            defaultLimits = null;
        }

        private static MimeLimits GetDefaultMimeDocumentLimits() {
            if (defaultLimits == null) {
                lock (configurationLockObject) {
                    if (defaultLimits == null) {
                        var local_0 = Internal.ApplicationServices.Provider.GetConfiguration("MimeLimits");
                        var local_1 = 10;
                        var local_2 = 100;
                        var local_3 = int.MaxValue;
                        var local_4 = int.MaxValue;
                        var local_5 = 10000;
                        var local_6 = 100000;
                        var local_7 = int.MaxValue;
                        var local_8 = 32768;
                        var local_9 = int.MaxValue;
                        var local_10 = 256;
                        foreach (var item_0 in local_0) {
                            switch (item_0.Name.ToLower()) {
                                case "maximumpartdepth":
                                    local_1 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_1, 5, false);
                                    continue;
                                case "maximumembeddeddepth":
                                    local_2 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_2, 10, false);
                                    continue;
                                case "maximumsize":
                                    local_3 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_3, 100, true);
                                    continue;
                                case "maximumtotalheaderssize":
                                    local_4 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_4, 100, true);
                                    continue;
                                case "maximumparts":
                                    local_5 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_5, 100, false);
                                    continue;
                                case "maximumheaders":
                                    local_6 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_6, 100, false);
                                    continue;
                                case "maximumaddressitemsperheader":
                                    local_7 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_7, 100, false);
                                    continue;
                                case "maximumparametersperheader":
                                    local_9 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_9, 10, false);
                                    continue;
                                case "maximumtextvaluesize":
                                    local_8 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_8, 10, true);
                                    continue;
                                default:
                                    continue;
                            }
                        }
                        defaultLimits = new MimeLimits(local_1, local_2, local_3, local_4, local_5, local_6, local_7, local_8, local_9, local_10);
                    }
                }
            }
            return defaultLimits;
        }

        private static readonly object configurationLockObject = new object();
        private static volatile MimeLimits defaultLimits;

    }

}