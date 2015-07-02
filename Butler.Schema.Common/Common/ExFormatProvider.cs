namespace Butler.Schema.Data.Common {

    public class ExFormatProvider : System.IFormatProvider, System.ICustomFormatter {

        public ExFormatProvider() {
            defaultFormatProvider = System.Globalization.CultureInfo.InvariantCulture;
        }

        public string Format(string format, object arg, System.IFormatProvider formatProvider) {
            var str = string.Empty;
            System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate = null;
            var flag = false;
            if (arg is System.Security.Cryptography.X509Certificates.X509Certificate2)
                x509Certificate = arg as System.Security.Cryptography.X509Certificates.X509Certificate2;
            else if (arg is System.Security.Cryptography.X509Certificates.X509Certificate) {
                try {
                    x509Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2((System.Security.Cryptography.X509Certificates.X509Certificate) arg);
                } catch (System.Security.Cryptography.CryptographicException ex) {
                    flag = true;
                    str = string.Format("Error formatting certificate: {0}", ex.Message);
                }
            }
            if (x509Certificate != null)
                str = ExFormatProvider.FormatX509Certificate(x509Certificate, format, defaultFormatProvider);
            else if (!flag)
                str = !(arg is System.IFormattable) ? (arg == null ? string.Empty : arg.ToString()) : ((System.IFormattable) arg).ToString(format, defaultFormatProvider);
            return str;
        }

        public object GetFormat(System.Type formatType) {
            if (formatType == typeof (System.ICustomFormatter))
                return this;
            return defaultFormatProvider.GetFormat(formatType);
        }

        public static string FormatX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate, string format, System.IFormatProvider formatProvider) {
            var stringBuilder = new System.Text.StringBuilder(500);
            stringBuilder.Append("[Subject]" + System.Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.Subject);
            stringBuilder.Append(System.Environment.NewLine + System.Environment.NewLine + "[Issuer]" + System.Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.Issuer);
            stringBuilder.Append(System.Environment.NewLine + System.Environment.NewLine + "[Serial Number]" + System.Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.SerialNumber);
            stringBuilder.Append(System.Environment.NewLine + System.Environment.NewLine + "[Not Before]" + System.Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.NotBefore.ToString(formatProvider));
            stringBuilder.Append(System.Environment.NewLine + System.Environment.NewLine + "[Not After]" + System.Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.NotAfter.ToString(formatProvider));
            stringBuilder.Append(System.Environment.NewLine + System.Environment.NewLine + "[Thumbprint]" + System.Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.GetCertHashString());
            stringBuilder.Append(System.Environment.NewLine);
            return stringBuilder.ToString();
        }

        private readonly System.IFormatProvider defaultFormatProvider;

    }

}