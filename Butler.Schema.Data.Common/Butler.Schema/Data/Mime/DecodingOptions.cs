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
    private DecodingFlags decodingFlags;
    private Globalization.Charset charset;

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

    public DecodingFlags DecodingFlags
    {
      get
      {
        return this.decodingFlags;
      }
      internal set
      {
        this.decodingFlags = value;
      }
    }

    public string CharsetName
    {
      get
      {
        if (this.charset != null)
          return this.charset.Name;
        return (string) null;
      }
    }

    public Encoding CharsetEncoding
    {
      get
      {
        if (this.charset != null && this.charset.IsAvailable)
          return this.charset.GetEncoding();
        return (Encoding) null;
      }
    }

    public bool AllowUTF8
    {
      get
      {
        return (this.decodingFlags & DecodingFlags.Utf8) == DecodingFlags.Utf8;
      }
    }

    internal Globalization.Charset Charset
    {
      get
      {
        return this.charset;
      }
      set
      {
        this.charset = value;
      }
    }

    public DecodingOptions(DecodingFlags decodingFlags)
    {
      this = new DecodingOptions(decodingFlags, (string) null);
    }

    public DecodingOptions(DecodingFlags decodingFlags, Encoding encoding)
    {
      this.decodingFlags = decodingFlags;
      this.charset = encoding == null ? (Globalization.Charset) null : Globalization.Charset.GetCharset(encoding);
    }

    public DecodingOptions(DecodingFlags decodingFlags, string charsetName)
    {
      this.decodingFlags = decodingFlags;
      this.charset = charsetName == null ? (Globalization.Charset) null : Globalization.Charset.GetCharset(charsetName);
    }

    internal DecodingOptions(string charsetName)
    {
      this = new DecodingOptions(DecodingOptions.Default.DecodingFlags, charsetName);
    }
  }
}
