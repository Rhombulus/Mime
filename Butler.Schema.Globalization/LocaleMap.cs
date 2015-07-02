namespace Butler.Schema.Globalization {

    internal static class LocaleMap {

        public static System.Globalization.CultureInfo GetCultureFromLcid(int lcid) {
            return System.Globalization.CultureInfo.GetCultureInfo(lcid);
        }

        public static int GetLcidFromCulture(System.Globalization.CultureInfo culture) {
            return culture.LCID;
        }

        public static int GetCompareLcidFromCulture(System.Globalization.CultureInfo culture) {
            return culture.CompareInfo.LCID;
        }

        public static int GetANSICodePage(System.Globalization.CultureInfo culture) {
            return culture.TextInfo.ANSICodePage;
        }

        public static System.Globalization.CultureInfo GetSpecificCulture(string cultureName) {
            return System.Globalization.CultureInfo.CreateSpecificCulture(cultureName);
        }

    }

}