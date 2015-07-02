// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.LocalizedException
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Butler.Schema.Data.Common
{
  [Serializable]
  public class LocalizedException : Exception, ILocalizedException, ILocalizedString
  {
    internal static LocalizedException.TraceExceptionDelegate TraceExceptionCallback;
    private IFormatProvider formatProvider;
    private LocalizedString localizedString;

    public override string Message => this.LocalizedString.ToString(this.FormatProvider);

      public IFormatProvider FormatProvider
    {
      get
      {
        return this.formatProvider;
      }
      set
      {
        this.formatProvider = value;
      }
    }

    public LocalizedString LocalizedString => this.localizedString;

      public int ErrorCode
    {
      get
      {
        return this.HResult;
      }
      set
      {
        this.HResult = value;
      }
    }

    public string StringId => this.localizedString.StringId;

      public ReadOnlyCollection<object> StringFormatParameters => this.localizedString.FormatParameters;

      public LocalizedException(LocalizedString localizedString)
      : this(localizedString, (Exception) null)
    {
      LocalizedException.TraceException("Created LocalizedException({0})", (object) localizedString);
    }

    public LocalizedException(LocalizedString localizedString, Exception innerException)
      : base((string) localizedString, innerException)
    {
      this.localizedString = localizedString;
      LocalizedException.TraceException("Created LocalizedException({0}, innerException)", (object) localizedString);
    }

    protected LocalizedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.localizedString = (LocalizedString) info.GetValue("localizedString", typeof (LocalizedString));
      LocalizedException.TraceException("Created LocalizedException(info, context)");
    }

    internal static void TraceException(string formatString, params object[] formatObjects)
    {
      if (LocalizedException.TraceExceptionCallback == null)
        return;
      LocalizedException.TraceExceptionCallback(formatString, formatObjects);
    }

    public static int GenerateErrorCode(Exception e)
    {
      int num1 = LocalizedException.InternalGenerateErrorCode(e);
      int num2 = 0;
      if (e.InnerException != null)
      {
        Exception innerException = e.InnerException;
        while (innerException.InnerException != null)
          innerException = innerException.InnerException;
        num2 = LocalizedException.InternalGenerateErrorCode(innerException);
      }
      return num1 ^ num2;
    }

    private static int InternalGenerateErrorCode(Exception e)
    {
      int hashCode1 = new StackTrace(e).ToString().GetHashCode();
      int hashCode2 = e.GetType().GetHashCode();
      int num = 0;
      ILocalizedString localizedString = e as ILocalizedString;
      if (localizedString != null)
        num = localizedString.LocalizedString.GetHashCode();
      return num ^ hashCode1 ^ hashCode2;
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("localizedString", (object) this.LocalizedString);
    }

    internal delegate void TraceExceptionDelegate(string formatString, params object[] formatObjects);
  }
}
