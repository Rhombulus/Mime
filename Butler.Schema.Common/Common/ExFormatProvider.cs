using System;
using System.Linq;

namespace Butler.Schema.Data.Common {

    public class ExFormatProvider : IFormatProvider, ICustomFormatter {

        public ExFormatProvider() {
            defaultFormatProvider = System.Globalization.CultureInfo.InvariantCulture;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider) {
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
                str = !(arg is IFormattable) ? (arg == null ? string.Empty : arg.ToString()) : ((IFormattable) arg).ToString(format, defaultFormatProvider);
            return str;
        }

        public object GetFormat(Type formatType) {
            if (formatType == typeof (ICustomFormatter))
                return this;
            return defaultFormatProvider.GetFormat(formatType);
        }

        public static string FormatX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate, string format, IFormatProvider formatProvider) {
            var stringBuilder = new System.Text.StringBuilder(500);
            stringBuilder.Append("[Subject]" + Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.Subject);
            stringBuilder.Append(Environment.NewLine + Environment.NewLine + "[Issuer]" + Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.Issuer);
            stringBuilder.Append(Environment.NewLine + Environment.NewLine + "[Serial Number]" + Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.SerialNumber);
            stringBuilder.Append(Environment.NewLine + Environment.NewLine + "[Not Before]" + Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.NotBefore.ToString(formatProvider));
            stringBuilder.Append(Environment.NewLine + Environment.NewLine + "[Not After]" + Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.NotAfter.ToString(formatProvider));
            stringBuilder.Append(Environment.NewLine + Environment.NewLine + "[Thumbprint]" + Environment.NewLine + "  ");
            stringBuilder.Append(x509Certificate.GetCertHashString());
            stringBuilder.Append(Environment.NewLine);
            return stringBuilder.ToString();
        }

        private readonly IFormatProvider defaultFormatProvider;

    }

}