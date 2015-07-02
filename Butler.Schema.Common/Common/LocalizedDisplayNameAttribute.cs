namespace Butler.Schema.Common {

    [System.AttributeUsage(System.AttributeTargets.All)]
    [System.Serializable]
    public class LocalizedDisplayNameAttribute : System.ComponentModel.DisplayNameAttribute, ILocalizedString {

        public LocalizedDisplayNameAttribute() {}

        public LocalizedDisplayNameAttribute(LocalizedString displayname) {
            this.LocalizedString = displayname;
        }

        public override sealed string DisplayName {
            [System.Security.Permissions.HostProtection(System.Security.Permissions.SecurityAction.LinkDemand, SharedState = true)]
            get {
                return this.LocalizedString;
            }
        }

        public LocalizedString LocalizedString { get; }

    }

}