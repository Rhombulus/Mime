// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.ExFormatProvider
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Butler.Schema.Data.Common
{
  public class ExFormatProvider : IFormatProvider, ICustomFormatter
  {
    private IFormatProvider defaultFormatProvider;

    public ExFormatProvider()
    {
      this.defaultFormatProvider = (IFormatProvider) CultureInfo.InvariantCulture;
    }

    public static string FormatX509Certificate(X509Certificate2 x509Certificate, string format, IFormatProvider formatProvider)
    {
      StringBuilder stringBuilder = new StringBuilder(500);
      stringBuilder.Append("[Subject]" + Environment.NewLine + "  ");
      stringBuilder.Append(x509Certificate.Subject.ToString());
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

    public object GetFormat(Type formatType)
    {
      if (formatType == typeof (ICustomFormatter))
        return (object) this;
      return this.defaultFormatProvider.GetFormat(formatType);
    }

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
      string str = string.Empty;
      X509Certificate2 x509Certificate = (X509Certificate2) null;
      bool flag = false;
      if (arg is X509Certificate2)
        x509Certificate = arg as X509Certificate2;
      else if (arg is X509Certificate)
      {
        try
        {
          x509Certificate = new X509Certificate2((X509Certificate) arg);
        }
        catch (CryptographicException ex)
        {
          flag = true;
          str = string.Format("Error formatting certificate: {0}", (object) ex.Message);
        }
      }
      if (x509Certificate != null)
        str = ExFormatProvider.FormatX509Certificate(x509Certificate, format, this.defaultFormatProvider);
      else if (!flag)
        str = !(arg is IFormattable) ? (arg == null ? string.Empty : arg.ToString()) : ((IFormattable) arg).ToString(format, this.defaultFormatProvider);
      return str;
    }
  }
}
