// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.DecodingOptions
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Mime
{
  public struct DecodingOptions
  {
    public static readonly DecodingOptions None = new DecodingOptions();
    public static readonly DecodingOptions Default = new DecodingOptions(DecodingFlags.AllEncodings);
    private static Globalization.Charset defaultCharset;

      internal static Globalization.Charset DefaultCharset
    {
      get
      {
        if (DecodingOptions.defaultCharset == null)
        {
          Globalization.Charset charset = Globalization.Charset.DefaultMimeCharset;
          if (!charset.IsAvailable || charset.AsciiSupport < Globalization.CodePageAsciiSupport.Fine)
            charset = Globalization.Charset.UTF8;
          DecodingOptions.defaultCharset = charset;
        }
        return DecodingOptions.defaultCharset;
      }
    }

    public DecodingFlags DecodingFlags { get; internal set; }

      public string CharsetName
    {
      get
      {
        if (this.Charset != null)
          return this.Charset.Name;
        return (string) null;
      }
    }

    public Encoding CharsetEncoding
    {
      get
      {
        if (this.Charset != null && this.Charset.IsAvailable)
          return this.Charset.GetEncoding();
        return (Encoding) null;
      }
    }

    public bool AllowUTF8 => (this.DecodingFlags & DecodingFlags.Utf8) == DecodingFlags.Utf8;

      internal Globalization.Charset Charset { get; set; }

      public DecodingOptions(DecodingFlags decodingFlags)
    {
      this = new DecodingOptions(decodingFlags, (string) null);
    }

    public DecodingOptions(DecodingFlags decodingFlags, Encoding encoding)
    {
      this.DecodingFlags = decodingFlags;
      this.Charset = encoding == null ? (Globalization.Charset) null : Globalization.Charset.GetCharset(encoding);
    }

    public DecodingOptions(DecodingFlags decodingFlags, string charsetName)
    {
      this.DecodingFlags = decodingFlags;
      this.Charset = charsetName == null ? (Globalization.Charset) null : Globalization.Charset.GetCharset(charsetName);
    }

    internal DecodingOptions(string charsetName)
    {
      this = new DecodingOptions(DecodingOptions.Default.DecodingFlags, charsetName);
    }
  }
}
