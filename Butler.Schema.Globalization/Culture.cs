// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Culture
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.Linq;

namespace Butler.Schema.Data.Globalization
{
  [Serializable]
  public class Culture
  {

      private int[] codepageDetectionPriorityOrder;
    private CultureInfo cultureInfo;

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

      internal Culture(int lcid, string name)
    {
      this.LCID = lcid;
      this.Name = name;
    }

    public static Culture GetCulture(string name)
    {
      Culture culture;
      if (!Culture.TryGetCulture(name, out culture))
        throw new UnknownCultureException(name);
      return culture;
    }

    public static bool TryGetCulture(string name, out Culture culture)
    {
      if (name != null)
        return CultureCharsetDatabase.Data.NameToCulture.TryGetValue(name, out culture);
      culture = (Culture) null;
      return false;
    }

    public static Culture GetCulture(int lcid)
    {
      Culture culture;
      if (!Culture.TryGetCulture(lcid, out culture))
        throw new UnknownCultureException(lcid);
      return culture;
    }

    public static bool TryGetCulture(int lcid, out Culture culture)
    {
      return CultureCharsetDatabase.Data.LcidToCulture.TryGetValue(lcid, out culture);
    }

    public CultureInfo GetCultureInfo()
    {
      if (this.cultureInfo == null)
        return CultureInfo.InvariantCulture;
      return this.cultureInfo;
    }

    internal void SetWindowsCharset(Charset windowsCharset)
    {
      this.WindowsCharset = windowsCharset;
    }

    internal void SetMimeCharset(Charset mimeCharset)
    {
      this.MimeCharset = mimeCharset;
    }

    internal void SetWebCharset(Charset webCharset)
    {
      this.WebCharset = webCharset;
    }

    internal void SetDescription(string description)
    {
      this.Description = description;
    }

    internal void SetNativeDescription(string description)
    {
      this.NativeDescription = description;
    }

    internal void SetParentCulture(Culture parentCulture)
    {
      this.ParentCulture = parentCulture;
    }

    internal void SetCultureInfo(CultureInfo cultureInfo)
    {
      this.cultureInfo = cultureInfo;
    }

    internal int[] GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.GlobalizationData data)
    {
      if (this.codepageDetectionPriorityOrder == null)
        this.codepageDetectionPriorityOrder = CultureCharsetDatabase.GetCultureSpecificCodepageDetectionPriorityOrder(this, this.ParentCulture == null || this.ParentCulture == this ? data.DefaultDetectionPriorityOrder : this.ParentCulture.GetCodepageDetectionPriorityOrder(data));
      return this.codepageDetectionPriorityOrder;
    }

    internal void SetCodepageDetectionPriorityOrder(int[] codepageDetectionPriorityOrder)
    {
      this.codepageDetectionPriorityOrder = codepageDetectionPriorityOrder;
    }

    internal CultureInfo GetSpecificCultureInfo()
    {
      if (this.cultureInfo == null)
        return CultureInfo.InvariantCulture;
      try
      {
        return CultureInfo.CreateSpecificCulture(this.cultureInfo.Name);
      }
      catch (ArgumentException ex)
      {
        return CultureInfo.InvariantCulture;
      }
    }
  }
}
