// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.EncodingOptions
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class EncodingOptions
  {
    internal static readonly Globalization.Charset DefaultCharset = DecodingOptions.DefaultCharset;
      private Globalization.Charset charset;

      public string CharsetName
    {
      get
      {
        if (this.charset != null)
          return this.charset.Name;
        return (string) null;
      }
    }

    public string CultureName { get; }

      public EncodingFlags EncodingFlags { get; }

      public bool AllowUTF8 => (this.EncodingFlags & EncodingFlags.AllowUTF8) == EncodingFlags.AllowUTF8;

      public EncodingOptions(string charsetName, string cultureName, EncodingFlags encodingFlags)
    {
      this.CultureName = cultureName;
      this.EncodingFlags = encodingFlags;
      this.charset = charsetName == null ? (Globalization.Charset) null : Globalization.Charset.GetCharset(charsetName);
      if (this.charset == null)
        return;
      this.charset.GetEncoding();
    }

    internal EncodingOptions(Globalization.Charset charset)
    {
      this.charset = charset;
      this.CultureName = (string) null;
      this.EncodingFlags = EncodingFlags.None;
    }

    internal Globalization.Charset GetEncodingCharset()
    {
      return this.charset ?? EncodingOptions.DefaultCharset;
    }
  }
}
