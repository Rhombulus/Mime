namespace Butler.Schema.Data.Common {

    internal class SipCultureInfoBase : System.Globalization.CultureInfo {

        internal SipCultureInfoBase(System.Globalization.CultureInfo parent, string segmentID)
            : base(parent.Name) {
            this.parent = parent;
            this.segmentID = segmentID;
            name = parent.Name;
            sipName = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}-x-{1}",
                parent.IsNeutralCulture ? parent.Name : parent.Parent.Name,
                segmentID);
            description = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "Role-Based Culture ({0})",
                name);
        }

        public override string Name {
            get {
                if (!useSipName)
                    return name;
                return sipName;
            }
        }

        public override System.Globalization.CultureInfo Parent => parent;
        public override string EnglishName => description;

        internal virtual bool UseSipName {
            get {
                return useSipName;
            }
            set {
                useSipName = value;
            }
        }

        internal virtual string SipName => sipName;
        internal virtual string SipSegmentID => segmentID;
        protected string description;
        protected string name;
        protected System.Globalization.CultureInfo parent;
        protected string segmentID;
        protected string sipName;
        protected bool useSipName;

    }

}