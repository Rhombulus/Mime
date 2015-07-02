// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfSupport
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal static class RtfSupport
  {
    private static readonly RtfSupport.CharRep[] CharRepFromLID = new RtfSupport.CharRep[102]
    {
      RtfSupport.CharRep.DEFAULT_INDEX,
      RtfSupport.CharRep.ARABIC_INDEX,
      RtfSupport.CharRep.RUSSIAN_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.GB2312_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.GREEK_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.HEBREW_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.SHIFTJIS_INDEX,
      RtfSupport.CharRep.HANGUL_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.DEFAULT_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.RUSSIAN_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.THAI_INDEX,
      RtfSupport.CharRep.TURKISH_INDEX,
      RtfSupport.CharRep.ARABIC_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.RUSSIAN_INDEX,
      RtfSupport.CharRep.RUSSIAN_INDEX,
      RtfSupport.CharRep.EASTEUROPE_INDEX,
      RtfSupport.CharRep.BALTIC_INDEX,
      RtfSupport.CharRep.BALTIC_INDEX,
      RtfSupport.CharRep.BALTIC_INDEX,
      RtfSupport.CharRep.DEFAULT_INDEX,
      RtfSupport.CharRep.ARABIC_INDEX,
      RtfSupport.CharRep.VIET_INDEX,
      RtfSupport.CharRep.NCHARSETS,
      RtfSupport.CharRep.TURKISH_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.DEFAULT_INDEX,
      RtfSupport.CharRep.RUSSIAN_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.GEORGIAN_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.DEVANAGARI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.HEBREW_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.RUSSIAN_INDEX,
      RtfSupport.CharRep.RUSSIAN_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.TURKISH_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.BENGALI_INDEX,
      RtfSupport.CharRep.GURMUKHI_INDEX,
      RtfSupport.CharRep.GUJARATI_INDEX,
      RtfSupport.CharRep.ORIYA_INDEX,
      RtfSupport.CharRep.TAMIL_INDEX,
      RtfSupport.CharRep.TELUGU_INDEX,
      RtfSupport.CharRep.KANNADA_INDEX,
      RtfSupport.CharRep.MALAYALAM_INDEX,
      RtfSupport.CharRep.BENGALI_INDEX,
      RtfSupport.CharRep.DEVANAGARI_INDEX,
      RtfSupport.CharRep.DEVANAGARI_INDEX,
      RtfSupport.CharRep.MONGOLIAN_INDEX,
      RtfSupport.CharRep.TIBETAN_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.KHMER_INDEX,
      RtfSupport.CharRep.LAO_INDEX,
      RtfSupport.CharRep.MYANMAR_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.DEVANAGARI_INDEX,
      RtfSupport.CharRep.BENGALI_INDEX,
      RtfSupport.CharRep.GURMUKHI_INDEX,
      RtfSupport.CharRep.SYRIAC_INDEX,
      RtfSupport.CharRep.SINHALA_INDEX,
      RtfSupport.CharRep.CHEROKEE_INDEX,
      RtfSupport.CharRep.ABORIGINAL_INDEX,
      RtfSupport.CharRep.ETHIOPIC_INDEX,
      RtfSupport.CharRep.DEFAULT_INDEX,
      RtfSupport.CharRep.DEFAULT_INDEX,
      RtfSupport.CharRep.DEVANAGARI_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.ARABIC_INDEX,
      RtfSupport.CharRep.ANSI_INDEX,
      RtfSupport.CharRep.THAANA_INDEX
    };
    private static readonly ushort[] CodePage = new ushort[20]
    {
      (ushort) 1252,
      (ushort) 1250,
      (ushort) 1251,
      (ushort) 1253,
      (ushort) 1254,
      (ushort) 1255,
      (ushort) 1256,
      (ushort) 1257,
      (ushort) 1258,
      (ushort) 0,
      (ushort) 42,
      (ushort) 874,
      (ushort) 932,
      (ushort) 936,
      (ushort) 949,
      (ushort) 950,
      (ushort) 437,
      (ushort) 850,
      (ushort) 10000,
      (ushort) 1256
    };
    private static readonly byte[] CharSet = unchecked(new byte[20]
    {
      (byte) 0,
      (byte) 238,
      (byte) 204,
      (byte) 161,
      (byte) 162,
      (byte) 177,
      (byte) 178,
      (byte) 186,
      (byte) 163,
      (byte) 1,
      (byte) 2,
      (byte) 222,
      (byte) sbyte.MinValue,
      (byte) 134,
      (byte) 129,
      (byte) 136,
      (byte) 254,
      byte.MaxValue,
      (byte) 77,
      (byte) 180
    });
        public static readonly byte[] UnsafeAsciiMap = new byte[161]
    {
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1
    };
    public const int RtfNestingLimit = 4096;
    public const int MaxBookmarkNameLength = 4096;
    public const int MaxFieldInstructionLength = 4096;
    public const int MaxFontNameLength = 256;
    public const int MaxUrlLength = 1024;
    public const int MaxShapePropertyName = 128;
    public const int MaxShapePropertyValue = 4096;
    public const byte LANG_NEUTRAL = (byte) 0;
    public const byte LANG_AFRIKAANS = (byte) 54;
    public const byte LANG_ALBANIAN = (byte) 28;
    public const byte LANG_ARABIC = (byte) 1;
    public const byte LANG_BASQUE = (byte) 45;
    public const byte LANG_BELARUSIAN = (byte) 35;
    public const byte LANG_BULGARIAN = (byte) 2;
    public const byte LANG_CATALAN = (byte) 3;
    public const byte LANG_CHINESE = (byte) 4;
    public const byte LANG_CROATIAN = (byte) 26;
    public const byte LANG_CZECH = (byte) 5;
    public const byte LANG_DANISH = (byte) 6;
    public const byte LANG_DUTCH = (byte) 19;
    public const byte LANG_ENGLISH = (byte) 9;
    public const byte LANG_ESTONIAN = (byte) 37;
    public const byte LANG_FAEROESE = (byte) 56;
    public const byte LANG_FARSI = (byte) 41;
    public const byte LANG_FINNISH = (byte) 11;
    public const byte LANG_FRENCH = (byte) 12;
    public const byte LANG_GERMAN = (byte) 7;
    public const byte LANG_GREEK = (byte) 8;
    public const byte LANG_HEBREW = (byte) 13;
    public const byte LANG_HUNGARIAN = (byte) 14;
    public const byte LANG_ICELANDIC = (byte) 15;
    public const byte LANG_INDONESIAN = (byte) 33;
    public const byte LANG_ITALIAN = (byte) 16;
    public const byte LANG_JAPANESE = (byte) 17;
    public const byte LANG_KOREAN = (byte) 18;
    public const byte LANG_LATVIAN = (byte) 38;
    public const byte LANG_LITHUANIAN = (byte) 39;
    public const byte LANG_NORWEGIAN = (byte) 20;
    public const byte LANG_POLISH = (byte) 21;
    public const byte LANG_PORTUGUESE = (byte) 22;
    public const byte LANG_ROMANIAN = (byte) 24;
    public const byte LANG_RUSSIAN = (byte) 25;
    public const byte LANG_SERBIAN = (byte) 26;
    public const byte LANG_SLOVAK = (byte) 27;
    public const byte LANG_SLOVENIAN = (byte) 36;
    public const byte LANG_SPANISH = (byte) 10;
    public const byte LANG_SWEDISH = (byte) 29;
    public const byte LANG_THAI = (byte) 30;
    public const byte LANG_TURKISH = (byte) 31;
    public const byte LANG_UKRAINIAN = (byte) 34;
    public const byte LANG_VIETNAMESE = (byte) 42;
    public const byte LANG_URDU = (byte) 32;
    public const byte LANG_SYRIAC = (byte) 90;
    public const byte LANG_DIVEHI = (byte) 101;
    public const short LID_SERBIAN_CYRILLIC = (short) 3098;
    public const short LID_AZERI_CYRILLIC = (short) 2092;
    public const short LID_UZBEK_CYRILLIC = (short) 2115;
    public const short LID_MONGOLIAN_CYRILLIC = (short) 1104;
    public const short LID_PRC = (short) 2052;
    public const short LID_SINGAPORE = (short) 4100;
    public const byte ANSI_CHARSET = (byte) 0;
    public const byte DEFAULT_CHARSET = (byte) 1;
    public const byte SYMBOL_CHARSET = (byte) 2;
    public const byte SHIFTJIS_CHARSET = (byte) 128;
    public const byte HANGEUL_CHARSET = (byte) 129;
    public const byte HANGUL_CHARSET = (byte) 129;
    public const byte GB2312_CHARSET = (byte) 134;
    public const byte CHINESEBIG5_CHARSET = (byte) 136;
    public const byte OEM_CHARSET = (byte) 255;
    public const byte JOHAB_CHARSET = (byte) 130;
    public const byte HEBREW_CHARSET = (byte) 177;
    public const byte ARABIC_CHARSET = (byte) 178;
    public const byte ARABIC1_CHARSET = (byte) 180;
    public const byte GREEK_CHARSET = (byte) 161;
    public const byte TURKISH_CHARSET = (byte) 162;
    public const byte VIETNAMESE_CHARSET = (byte) 163;
    public const byte THAI_CHARSET = (byte) 222;
    public const byte EASTEUROPE_CHARSET = (byte) 238;
    public const byte RUSSIAN_CHARSET = (byte) 204;
    public const byte MAC_CHARSET = (byte) 77;
    public const byte BALTIC_CHARSET = (byte) 186;
    public const byte PC437_CHARSET = (byte) 254;
    public const ushort CP_SYMBOL = (ushort) 42;
    private const string HexCharacters = "0123456789ABCDEF";

    public static ushort CodePageFromCharRep(RtfSupport.CharRep charRep)
    {
      if (charRep >= (RtfSupport.CharRep) RtfSupport.CodePage.Length)
        return (ushort) 0;
      return RtfSupport.CodePage[(int) charRep];
    }

    public static RtfSupport.CharRep CharRepFromLanguage(int langid)
    {
      short num = (short) (langid & 1023);
      if ((int) num >= 26)
      {
        if (langid == 3098 || langid == 2092 || (langid == 2115 || langid == 1104))
          return RtfSupport.CharRep.RUSSIAN_INDEX;
        if ((int) num >= RtfSupport.CharRepFromLID.Length)
          return RtfSupport.CharRep.ANSI_INDEX;
      }
      RtfSupport.CharRep charRep = RtfSupport.CharRepFromLID[(int) num];
      if (!RtfSupport.IsFECharRep(charRep) || charRep != RtfSupport.CharRep.GB2312_INDEX || (langid == 2052 || langid == 4100))
        return charRep;
      charRep = RtfSupport.CharRep.BIG5_INDEX;
      return charRep;
    }

    public static RtfSupport.CharRep CharRepFromCharSet(int charset)
    {
      for (byte index = (byte) 0; (int) index < RtfSupport.CharSet.Length; ++index)
      {
        if ((int) RtfSupport.CharSet[(int) index] == charset)
          return (RtfSupport.CharRep) index;
      }
      return RtfSupport.CharRep.UNDEFINED;
    }

    public static RtfSupport.CharRep CharRepFromCodePage(ushort codePage)
    {
      for (byte index = (byte) 0; (int) index < RtfSupport.CodePage.Length; ++index)
      {
        if ((int) RtfSupport.CodePage[(int) index] == (int) codePage)
          return (RtfSupport.CharRep) index;
      }
      return (int) codePage == 54936 ? RtfSupport.CharRep.GB18030_INDEX : RtfSupport.CharRep.UNDEFINED;
    }

    public static int CharSetFromCodePage(ushort codePage)
    {
      for (byte index = (byte) 0; (int) index < RtfSupport.CodePage.Length; ++index)
      {
        if ((int) RtfSupport.CodePage[(int) index] == (int) codePage)
          return (int) RtfSupport.CharSet[(int) index];
      }
      return 1;
    }

    public static bool IsBiDiCharRep(RtfSupport.CharRep charRep)
    {
      if (RtfSupport.CharRep.HEBREW_INDEX <= charRep && charRep <= RtfSupport.CharRep.ARABIC_INDEX)
        return true;
      if (RtfSupport.CharRep.SYRIAC_INDEX <= charRep)
        return charRep <= RtfSupport.CharRep.THAANA_INDEX;
      return false;
    }

    public static bool IsRtlCharSet(int charset)
    {
      if (177 <= charset)
        return charset <= 178;
      return false;
    }

    public static bool IsBiDiLanguage(int langid)
    {
      short num = (short) (langid & 1023);
      switch (num)
      {
        case (short) 1:
        case (short) 13:
        case (short) 32:
        case (short) 41:
        case (short) 90:
          return true;
        default:
          return (int) num == 101;
      }
    }

    public static bool IsHebrewLanguage(int langid)
    {
      return (int) (short) (langid & 1023) == 13;
    }

    public static bool IsArabicLanguage(int langid)
    {
      short num = (short) (langid & 1023);
      switch (num)
      {
        case (short) 1:
        case (short) 32:
        case (short) 41:
        case (short) 90:
          return true;
        default:
          return (int) num == 101;
      }
    }

    public static bool IsThaiLanguage(int langid)
    {
      return (int) (short) (langid & 1023) == 30;
    }

    public static bool IsFECharRep(RtfSupport.CharRep charRep)
    {
      if (RtfSupport.CharRep.SHIFTJIS_INDEX <= charRep)
        return charRep <= RtfSupport.CharRep.BIG5_INDEX;
      return false;
    }

    public static int RGB(int red, int green, int blue)
    {
      return (int) (byte) blue << 16 | (int) (byte) green << 8 | (int) (byte) red;
    }

    public static int Unescape(byte b1, byte b2)
    {
      if (ParseSupport.HexCharacter(ParseSupport.GetCharClass((char) b1)) && ParseSupport.HexCharacter(ParseSupport.GetCharClass((char) b2)))
        return ParseSupport.CharToHex((char) b1) << 4 | ParseSupport.CharToHex((char) b2);
      return 256;
    }

    public static void Escape(char ch, byte[] buffer, int offset)
    {
      buffer[offset] = (byte) "0123456789ABCDEF"[(int) ch >> 4 & 15];
      buffer[offset + 1] = (byte) "0123456789ABCDEF"[(int) ch & 15];
    }

    public static bool IsHyperlinkField(ref ScratchBuffer scratch, out bool local, out BufferString linkUrl)
    {
      int index = 0;
      int length = scratch.Length;
      while (index != scratch.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[index])))
        ++index;
      if (scratch.Length - index > 10 && (int) scratch[index] == 72 && ((int) scratch[index + 1] == 89 && (int) scratch[index + 2] == 80) && ((int) scratch[index + 3] == 69 && (int) scratch[index + 4] == 82 && ((int) scratch[index + 5] == 76 && (int) scratch[index + 6] == 73)) && ((int) scratch[index + 7] == 78 && (int) scratch[index + 8] == 75 && (int) scratch[index + 9] == 32))
      {
        int offset1 = index + 10;
        int rawResultOffset;
        int rawResultLength;
        int unescapedResultOffset;
        int unescapedResultLength;
        int fieldArgument = RtfSupport.GetFieldArgument(ref scratch, offset1, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
        if (rawResultLength == 2 && (int) scratch[rawResultOffset] == 92 && (int) scratch[rawResultOffset + 1] == 108)
        {
          local = true;
          int offset2 = offset1 + fieldArgument;
          RtfSupport.GetFieldArgument(ref scratch, offset2, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
        }
        else
          local = false;
        if (unescapedResultLength != 0)
        {
          if (local)
          {
            --unescapedResultOffset;
            ++unescapedResultLength;
            scratch[unescapedResultOffset] = '#';
          }
          linkUrl = scratch.SubString(unescapedResultOffset, unescapedResultLength);
          return true;
        }
      }
      local = false;
      linkUrl = BufferString.Null;
      return false;
    }

    public static bool IsIncludePictureField(ref ScratchBuffer scratch, out BufferString linkUrl)
    {
      int index = 0;
      int length = scratch.Length;
      while (index != scratch.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[index])))
        ++index;
      if (scratch.Length - index > 15 && (int) scratch[index] == 73 && ((int) scratch[index + 1] == 78 && (int) scratch[index + 2] == 67) && ((int) scratch[index + 3] == 76 && (int) scratch[index + 4] == 85 && ((int) scratch[index + 5] == 68 && (int) scratch[index + 6] == 69)) && ((int) scratch[index + 7] == 80 && (int) scratch[index + 8] == 73 && ((int) scratch[index + 9] == 67 && (int) scratch[index + 10] == 84) && ((int) scratch[index + 11] == 85 && (int) scratch[index + 12] == 82 && ((int) scratch[index + 13] == 69 && (int) scratch[index + 14] == 32))))
      {
        int offset = index + 15;
        int rawResultOffset;
        int rawResultLength;
        int unescapedResultOffset;
        int unescapedResultLength;
        int fieldArgument = RtfSupport.GetFieldArgument(ref scratch, offset, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
        while (rawResultLength == 2 && (int) scratch[rawResultOffset] == 92)
        {
          offset += fieldArgument;
          fieldArgument = RtfSupport.GetFieldArgument(ref scratch, offset, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
        }
        if (unescapedResultLength > 2)
        {
          linkUrl = scratch.SubString(unescapedResultOffset, unescapedResultLength);
          return true;
        }
      }
      linkUrl = BufferString.Null;
      return false;
    }

    public static bool IsSymbolField(ref ScratchBuffer scratch, out TextMapping textMapping, out char symbol, out short points)
    {
      textMapping = TextMapping.Unicode;
      symbol = char.MinValue;
      points = (short) 0;
      int index1 = 0;
      int length = scratch.Length;
      while (index1 != scratch.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[index1])))
        ++index1;
      if (scratch.Length - index1 <= 7 || (int) scratch[index1] != 83 || ((int) scratch[index1 + 1] != 89 || (int) scratch[index1 + 2] != 77) || ((int) scratch[index1 + 3] != 66 || (int) scratch[index1 + 4] != 79 || ((int) scratch[index1 + 5] != 76 || (int) scratch[index1 + 6] != 32)))
        return false;
      int offset1 = index1 + 7;
      int rawResultOffset;
      int rawResultLength;
      int unescapedResultOffset;
      int unescapedResultLength;
      int fieldArgument1 = RtfSupport.GetFieldArgument(ref scratch, offset1, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
      if (rawResultLength > 2 && (int) scratch.Buffer[rawResultOffset] == 48 && (int) (ushort) ((uint) scratch.Buffer[rawResultOffset + 1] | 32U) == 120)
      {
        char ch;
        for (int index2 = 2; index2 < rawResultLength && (int) (ch = scratch.Buffer[rawResultOffset + index2]) <= 102 && (48 <= (int) ch && (int) ch <= 57 || 97 <= (int) ch && (int) ch <= 102 || 65 <= (int) ch && (int) ch <= 70); ++index2)
          symbol = (char) (((int) symbol << 4) + ((int) ch <= 57 ? (int) ch - 48 : ((int) ch & 79) - 65 + 10));
      }
      else
      {
        char ch;
        for (int index2 = 0; index2 < rawResultLength && (int) (ch = scratch.Buffer[rawResultOffset + index2]) <= 57 && 48 <= (int) ch; ++index2)
          symbol = (char) (10 * (int) symbol + ((int) ch - 48));
      }
      int offset2 = offset1 + fieldArgument1;
      int fieldArgument2 = RtfSupport.GetFieldArgument(ref scratch, offset2, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
      if (rawResultLength != 2 || (int) scratch[rawResultOffset] != 92 || (int) scratch[rawResultOffset + 1] != 102)
        return false;
      int offset3 = offset2 + fieldArgument2;
      int fieldArgument3 = RtfSupport.GetFieldArgument(ref scratch, offset3, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
      if (unescapedResultLength == 0)
        return false;
      RecognizeInterestingFontName interestingFontName = new RecognizeInterestingFontName();
      for (int index2 = 0; index2 < unescapedResultLength && !interestingFontName.IsRejected; ++index2)
        interestingFontName.AddCharacter(scratch.Buffer[unescapedResultOffset + index2]);
      textMapping = interestingFontName.TextMapping;
      if (textMapping == TextMapping.Unicode)
        textMapping = TextMapping.OtherSymbol;
      int offset4 = offset3 + fieldArgument3;
      int fieldArgument4 = RtfSupport.GetFieldArgument(ref scratch, offset4, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
      if (rawResultLength != 2 || (int) scratch[rawResultOffset] != 92 || (int) scratch[rawResultOffset + 1] != 115)
        return true;
      int offset5 = offset4 + fieldArgument4;
      RtfSupport.GetFieldArgument(ref scratch, offset5, out rawResultOffset, out rawResultLength, out unescapedResultOffset, out unescapedResultLength);
      char ch1;
      for (int index2 = 0; index2 < rawResultLength && (int) (ch1 = scratch.Buffer[rawResultOffset + index2]) <= 57 && 48 <= (int) ch1; ++index2)
        points = (short) (10 * (int) points + ((int) ch1 - 48));
      return true;
    }

    private static int GetFieldArgument(ref ScratchBuffer scratch, int offset, out int rawResultOffset, out int rawResultLength, out int unescapedResultOffset, out int unescapedResultLength)
    {
      int length = scratch.Length;
      bool flag = false;
      int num = 0;
      while (offset < length && (int) scratch[offset] == 32)
      {
        ++offset;
        ++num;
      }
      if (offset < length && (int) scratch[offset] == 34)
      {
        flag = true;
        ++offset;
        ++num;
      }
      rawResultOffset = offset;
      rawResultLength = 0;
      unescapedResultOffset = length;
      unescapedResultLength = 0;
      while (offset < length)
      {
        char ch = scratch[offset];
        if ((int) ch == 34 && flag || (int) ch == 32 && !flag)
        {
          ++num;
          break;
        }
        if ((int) ch == 92)
        {
          ++offset;
          ++num;
          ++rawResultLength;
          if (offset != length)
            ch = scratch[offset];
          else
            break;
        }
        if (scratch.Append(ch, 5120) != 0)
          ++unescapedResultLength;
        ++rawResultLength;
        ++offset;
        ++num;
      }
      scratch.Length = length;
      return num;
    }

    public static string StringFontNameFromScratch(ScratchBuffer scratch)
    {
      int offset = 0;
      int length = scratch.Length;
      while (length != 0 && ((int) scratch[length - 1] == 59 || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[length - 1]))))
        --length;
      for (; length != 0 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[offset])); --length)
        ++offset;
      if (length != 0 && (int) scratch[offset] != 63)
        return scratch.ToString(offset, length);
      return (string) null;
    }

    public enum CharRep : byte
    {
      ANSI_INDEX = (byte) 0,
      EASTEUROPE_INDEX = (byte) 1,
      RUSSIAN_INDEX = (byte) 2,
      GREEK_INDEX = (byte) 3,
      TURKISH_INDEX = (byte) 4,
      HEBREW_INDEX = (byte) 5,
      ARABIC_INDEX = (byte) 6,
      BALTIC_INDEX = (byte) 7,
      VIET_INDEX = (byte) 8,
      DEFAULT_INDEX = (byte) 9,
      SYMBOL_INDEX = (byte) 10,
      THAI_INDEX = (byte) 11,
      SHIFTJIS_INDEX = (byte) 12,
      GB2312_INDEX = (byte) 13,
      HANGUL_INDEX = (byte) 14,
      BIG5_INDEX = (byte) 15,
      PC437_INDEX = (byte) 16,
      OEM_INDEX = (byte) 17,
      MAC_INDEX = (byte) 18,
      ARABIC1_INDEX = (byte) 19,
      ARMENIAN_INDEX = (byte) 20,
      NCHARSETS = (byte) 20,
      SYRIAC_INDEX = (byte) 21,
      THAANA_INDEX = (byte) 22,
      DEVANAGARI_INDEX = (byte) 23,
      BENGALI_INDEX = (byte) 24,
      GURMUKHI_INDEX = (byte) 25,
      GUJARATI_INDEX = (byte) 26,
      ORIYA_INDEX = (byte) 27,
      TAMIL_INDEX = (byte) 28,
      TELUGU_INDEX = (byte) 29,
      KANNADA_INDEX = (byte) 30,
      MALAYALAM_INDEX = (byte) 31,
      SINHALA_INDEX = (byte) 32,
      LAO_INDEX = (byte) 33,
      TIBETAN_INDEX = (byte) 34,
      MYANMAR_INDEX = (byte) 35,
      GEORGIAN_INDEX = (byte) 36,
      JAMO_INDEX = (byte) 37,
      ETHIOPIC_INDEX = (byte) 38,
      CHEROKEE_INDEX = (byte) 39,
      ABORIGINAL_INDEX = (byte) 40,
      OGHAM_INDEX = (byte) 41,
      RUNIC_INDEX = (byte) 42,
      KHMER_INDEX = (byte) 43,
      MONGOLIAN_INDEX = (byte) 44,
      BRAILLE_INDEX = (byte) 45,
      YI_INDEX = (byte) 46,
      JPN2_INDEX = (byte) 47,
      CHS2_INDEX = (byte) 48,
      KOR2_INDEX = (byte) 49,
      CHT2_INDEX = (byte) 50,
      GB18030_INDEX = (byte) 51,
      NCHARREPERTOIRES = (byte) 52,
      UNDEFINED = (byte) 255,
    }
  }
}
