using System;
using System.Linq;

namespace Butler.Schema.Data.Common {

    [AttributeUsage(AttributeTargets.All)]
    [Serializable]
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