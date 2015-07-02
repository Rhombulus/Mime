using System;
using System.Linq;

namespace Butler.Schema.Data.Common {

    [Serializable]
    public class LocalizedException : Exception, ILocalizedException, ILocalizedString {

        public LocalizedException(LocalizedString localizedString)
            : this(localizedString, null) {
            LocalizedException.TraceException("Created LocalizedException({0})", (object) localizedString);
        }

        public LocalizedException(LocalizedString localizedString, Exception innerException)
            : base(localizedString, innerException) {
            this.LocalizedString = localizedString;
            LocalizedException.TraceException("Created LocalizedException({0}, innerException)", (object) localizedString);
        }

        protected LocalizedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
            this.LocalizedString = (LocalizedString) info.GetValue("localizedString", typeof (LocalizedString));
            LocalizedException.TraceException("Created LocalizedException(info, context)");
        }

        public override string Message => this.LocalizedString.ToString(this.FormatProvider);

        public IFormatProvider FormatProvider {
            get {
                return formatProvider;
            }
            set {
                formatProvider = value;
            }
        }

        public int ErrorCode {
            get {
                return this.HResult;
            }
            set {
                this.HResult = value;
            }
        }

        public string StringId => this.LocalizedString.StringId;
        public System.Collections.ObjectModel.ReadOnlyCollection<object> StringFormatParameters => this.LocalizedString.FormatParameters;
        public LocalizedString LocalizedString { get; }

        internal static void TraceException(string formatString, params object[] formatObjects) {
            if (TraceExceptionCallback == null)
                return;
            TraceExceptionCallback(formatString, formatObjects);
        }

        public static int GenerateErrorCode(Exception e) {
            var num1 = LocalizedException.InternalGenerateErrorCode(e);
            var num2 = 0;
            if (e.InnerException != null) {
                var innerException = e.InnerException;
                while (innerException.InnerException != null)
                    innerException = innerException.InnerException;
                num2 = LocalizedException.InternalGenerateErrorCode(innerException);
            }
            return num1 ^ num2;
        }

        private static int InternalGenerateErrorCode(Exception e) {
            var hashCode1 = new System.Diagnostics.StackTrace(e).ToString()
                                                                .GetHashCode();
            var hashCode2 = e.GetType()
                             .GetHashCode();
            var num = 0;
            var localizedString = e as ILocalizedString;
            if (localizedString != null)
                num = localizedString.LocalizedString.GetHashCode();
            return num ^ hashCode1 ^ hashCode2;
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("localizedString", this.LocalizedString);
        }

        internal static TraceExceptionDelegate TraceExceptionCallback;
        private IFormatProvider formatProvider;


        internal delegate void TraceExceptionDelegate(string formatString, params object[] formatObjects);

    }

}