namespace Butler.Schema.Data.Globalization {

    [System.Serializable]
    public class Culture {

        internal Culture(int lcid, string name) {
            this.LCID = lcid;
            this.Name = name;
        }

        public static Culture Default => CultureCharsetDatabase.Data.DefaultCulture;
        public static bool FallbackToDefaultCharset => CultureCharsetDatabase.Data.FallbackToDefaultCharset;
        public static Culture Invariant => CultureCharsetDatabase.Data.InvariantCulture;
        public int LCID { get; }
        public string Name { get; }
        public Charset WindowsCharset { get; private set; }
        public Charset MimeCharset { get; private set; }
        public Charset WebCharset { get; private set; }
        public string Description { get; private set; }
        public string NativeDescription { get; private set; }
        public Culture ParentCulture { get; private set; }
        internal int[] CodepageDetectionPriorityOrder => this.GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.Data);

        public static Culture GetCulture(string name) {
            Culture culture;
            if (!Culture.TryGetCulture(name, out culture))
                throw new UnknownCultureException(name);
            return culture;
        }

        public static bool TryGetCulture(string name, out Culture culture) {
            if (name != null)
                return CultureCharsetDatabase.Data.NameToCulture.TryGetValue(name, out culture);
            culture = null;
            return false;
        }

        public static Culture GetCulture(int lcid) {
            Culture culture;
            if (!Culture.TryGetCulture(lcid, out culture))
                throw new UnknownCultureException(lcid);
            return culture;
        }

        public static bool TryGetCulture(int lcid, out Culture culture) {
            return CultureCharsetDatabase.Data.LcidToCulture.TryGetValue(lcid, out culture);
        }

        public System.Globalization.CultureInfo GetCultureInfo() {
            if (_cultureInfo == null)
                return System.Globalization.CultureInfo.InvariantCulture;
            return _cultureInfo;
        }

        internal void SetWindowsCharset(Charset windowsCharset) {
            this.WindowsCharset = windowsCharset;
        }

        internal void SetMimeCharset(Charset mimeCharset) {
            this.MimeCharset = mimeCharset;
        }

        internal void SetWebCharset(Charset webCharset) {
            this.WebCharset = webCharset;
        }

        internal void SetDescription(string description) {
            this.Description = description;
        }

        internal void SetNativeDescription(string description) {
            this.NativeDescription = description;
        }

        internal void SetParentCulture(Culture parentCulture) {
            this.ParentCulture = parentCulture;
        }

        internal void SetCultureInfo(System.Globalization.CultureInfo cultureInfo) {
            _cultureInfo = cultureInfo;
        }

        internal int[] GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.GlobalizationData data) {
            return _codepageDetectionPriorityOrder ?? (_codepageDetectionPriorityOrder = CultureCharsetDatabase.GetCultureSpecificCodepageDetectionPriorityOrder(
                this,
                this.ParentCulture == null || this.ParentCulture == this ? data.DefaultDetectionPriorityOrder : this.ParentCulture.GetCodepageDetectionPriorityOrder(data)));
        }

        internal void SetCodepageDetectionPriorityOrder(int[] order) {
            _codepageDetectionPriorityOrder = order;
        }

        internal System.Globalization.CultureInfo GetSpecificCultureInfo() {
            if (_cultureInfo == null)
                return System.Globalization.CultureInfo.InvariantCulture;
            try {
                return System.Globalization.CultureInfo.CreateSpecificCulture(_cultureInfo.Name);
            } catch (System.ArgumentException ex) {
                return System.Globalization.CultureInfo.InvariantCulture;
            }
        }

        private int[] _codepageDetectionPriorityOrder;
        private System.Globalization.CultureInfo _cultureInfo;

    }

}