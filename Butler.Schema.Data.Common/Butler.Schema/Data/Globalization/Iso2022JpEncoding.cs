// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022JpEncoding
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Globalization
{
  internal class Iso2022JpEncoding : Encoding
  {
    private Encoding defaultEncoding;

    internal Iso2022DecodingMode KillSwitch
    {
      get
      {
        return Iso2022JpEncoding.InternalReadKillSwitch();
      }
    }

    public override int CodePage
    {
      get
      {
        return this.defaultEncoding.CodePage;
      }
    }

    public override string BodyName
    {
      get
      {
        return this.defaultEncoding.BodyName;
      }
    }

    public override string EncodingName
    {
      get
      {
        return this.defaultEncoding.EncodingName;
      }
    }

    public override string HeaderName
    {
      get
      {
        return this.defaultEncoding.HeaderName;
      }
    }

    public override string WebName
    {
      get
      {
        return this.defaultEncoding.WebName;
      }
    }

    public override int WindowsCodePage
    {
      get
      {
        return this.defaultEncoding.WindowsCodePage;
      }
    }

    public override bool IsBrowserDisplay
    {
      get
      {
        return this.defaultEncoding.IsBrowserDisplay;
      }
    }

    public override bool IsBrowserSave
    {
      get
      {
        return this.defaultEncoding.IsBrowserSave;
      }
    }

    public override bool IsMailNewsDisplay
    {
      get
      {
        return this.defaultEncoding.IsMailNewsDisplay;
      }
    }

    public override bool IsMailNewsSave
    {
      get
      {
        return this.defaultEncoding.IsMailNewsSave;
      }
    }

    public override bool IsSingleByte
    {
      get
      {
        return this.defaultEncoding.IsSingleByte;
      }
    }

    internal Encoding DefaultEncoding
    {
      get
      {
        return this.defaultEncoding;
      }
    }

    public Iso2022JpEncoding(int codePage)
      : base(codePage)
    {
      if (codePage == 50220)
        this.defaultEncoding = Encoding.GetEncoding(50220);
      else if (codePage == 50221)
      {
        this.defaultEncoding = Encoding.GetEncoding(50221);
      }
      else
      {
        if (codePage != 50222)
          throw new ArgumentException("codePage", string.Format("Iso2022JpEncoding does not support codepage {0}", (object) codePage));
        this.defaultEncoding = Encoding.GetEncoding(50222);
      }
    }

    internal static Iso2022DecodingMode InternalReadKillSwitch()
    {
      switch (Common.RegistryConfigManager.Iso2022JpEncodingOverride)
      {
        case 1:
          return Iso2022DecodingMode.Override;
        case 2:
          return Iso2022DecodingMode.Throw;
        default:
          return Iso2022DecodingMode.Default;
      }
    }

    public override byte[] GetPreamble()
    {
      return this.defaultEncoding.GetPreamble();
    }

    public override int GetMaxByteCount(int charCount)
    {
      return this.defaultEncoding.GetMaxByteCount(charCount);
    }

    public override int GetMaxCharCount(int byteCount)
    {
      switch (this.KillSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultEncoding.GetMaxCharCount(byteCount);
        case Iso2022DecodingMode.Override:
          return this.defaultEncoding.GetMaxCharCount(byteCount);
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
      return this.defaultEncoding.GetByteCount(chars, index, count);
    }

    public override int GetByteCount(string s)
    {
      return this.defaultEncoding.GetByteCount(s);
    }

    public override unsafe int GetByteCount(char* chars, int count)
    {
      return this.defaultEncoding.GetByteCount(chars, count);
    }

    public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
      return this.defaultEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
      return this.defaultEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
    }

    public override unsafe int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
    {
      return this.defaultEncoding.GetBytes(chars, charCount, bytes, byteCount);
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      switch (this.KillSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultEncoding.GetCharCount(bytes, index, count);
        case Iso2022DecodingMode.Override:
          return this.GetDecoder().GetCharCount(bytes, index, count);
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override unsafe int GetCharCount(byte* bytes, int count)
    {
      switch (this.KillSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultEncoding.GetCharCount(bytes, count);
        case Iso2022DecodingMode.Override:
          throw new NotImplementedException();
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
      switch (this.KillSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        case Iso2022DecodingMode.Override:
          return this.GetDecoder().GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
    {
      switch (this.KillSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultEncoding.GetChars(bytes, byteCount, chars, charCount);
        case Iso2022DecodingMode.Override:
          throw new NotImplementedException();
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override string GetString(byte[] bytes, int index, int count)
    {
      switch (this.KillSwitch)
      {
        case Iso2022DecodingMode.Default:
          return this.defaultEncoding.GetString(bytes, index, count);
        case Iso2022DecodingMode.Override:
          Decoder decoder = this.GetDecoder();
          char[] chars1 = new char[this.GetMaxCharCount(count)];
          int chars2 = decoder.GetChars(bytes, index, count, chars1, 0);
          return new string(chars1, 0, chars2);
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override Decoder GetDecoder()
    {
      switch (this.KillSwitch)
      {
        case Iso2022DecodingMode.Default:
        case Iso2022DecodingMode.Override:
          return (Decoder) new Iso2022Jp.Iso2022JpDecoder(this);
        case Iso2022DecodingMode.Throw:
          throw new NotImplementedException();
        default:
          throw new InvalidOperationException();
      }
    }

    public override Encoder GetEncoder()
    {
      return this.defaultEncoding.GetEncoder();
    }

    public override object Clone()
    {
      return (object) (Iso2022JpEncoding) this.MemberwiseClone();
    }
  }
}
