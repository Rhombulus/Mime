// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.ByteString
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal static class ByteString
  {
    internal static readonly byte[] Empty = new byte[0];
    internal static readonly byte[] LowerC = new byte[256]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 9,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 15,
      (byte) 16,
      (byte) 17,
      (byte) 18,
      (byte) 19,
      (byte) 20,
      (byte) 21,
      (byte) 22,
      (byte) 23,
      (byte) 24,
      (byte) 25,
      (byte) 26,
      (byte) 27,
      (byte) 28,
      (byte) 29,
      (byte) 30,
      (byte) 31,
      (byte) 32,
      (byte) 33,
      (byte) 34,
      (byte) 35,
      (byte) 36,
      (byte) 37,
      (byte) 38,
      (byte) 39,
      (byte) 40,
      (byte) 41,
      (byte) 42,
      (byte) 43,
      (byte) 44,
      (byte) 45,
      (byte) 46,
      (byte) 47,
      (byte) 48,
      (byte) 49,
      (byte) 50,
      (byte) 51,
      (byte) 52,
      (byte) 53,
      (byte) 54,
      (byte) 55,
      (byte) 56,
      (byte) 57,
      (byte) 58,
      (byte) 59,
      (byte) 60,
      (byte) 61,
      (byte) 62,
      (byte) 63,
      (byte) 64,
      (byte) 97,
      (byte) 98,
      (byte) 99,
      (byte) 100,
      (byte) 101,
      (byte) 102,
      (byte) 103,
      (byte) 104,
      (byte) 105,
      (byte) 106,
      (byte) 107,
      (byte) 108,
      (byte) 109,
      (byte) 110,
      (byte) 111,
      (byte) 112,
      (byte) 113,
      (byte) 114,
      (byte) 115,
      (byte) 116,
      (byte) 117,
      (byte) 118,
      (byte) 119,
      (byte) 120,
      (byte) 121,
      (byte) 122,
      (byte) 91,
      (byte) 92,
      (byte) 93,
      (byte) 94,
      (byte) 95,
      (byte) 96,
      (byte) 97,
      (byte) 98,
      (byte) 99,
      (byte) 100,
      (byte) 101,
      (byte) 102,
      (byte) 103,
      (byte) 104,
      (byte) 105,
      (byte) 106,
      (byte) 107,
      (byte) 108,
      (byte) 109,
      (byte) 110,
      (byte) 111,
      (byte) 112,
      (byte) 113,
      (byte) 114,
      (byte) 115,
      (byte) 116,
      (byte) 117,
      (byte) 118,
      (byte) 119,
      (byte) 120,
      (byte) 121,
      (byte) 122,
      (byte) 123,
      (byte) 124,
      (byte) 125,
      (byte) 126,
      (byte) 127,
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
      (byte) 0
    };
    private static readonly uint[] CrcTable = new uint[256]
    {
      0U,
      1996959894U,
      3993919788U,
      2567524794U,
      124634137U,
      1886057615U,
      3915621685U,
      2657392035U,
      249268274U,
      2044508324U,
      3772115230U,
      2547177864U,
      162941995U,
      2125561021U,
      3887607047U,
      2428444049U,
      498536548U,
      1789927666U,
      4089016648U,
      2227061214U,
      450548861U,
      1843258603U,
      4107580753U,
      2211677639U,
      325883990U,
      1684777152U,
      4251122042U,
      2321926636U,
      335633487U,
      1661365465U,
      4195302755U,
      2366115317U,
      997073096U,
      1281953886U,
      3579855332U,
      2724688242U,
      1006888145U,
      1258607687U,
      3524101629U,
      2768942443U,
      901097722U,
      1119000684U,
      3686517206U,
      2898065728U,
      853044451U,
      1172266101U,
      3705015759U,
      2882616665U,
      651767980U,
      1373503546U,
      3369554304U,
      3218104598U,
      565507253U,
      1454621731U,
      3485111705U,
      3099436303U,
      671266974U,
      1594198024U,
      3322730930U,
      2970347812U,
      795835527U,
      1483230225U,
      3244367275U,
      3060149565U,
      1994146192U,
      31158534U,
      2563907772U,
      4023717930U,
      1907459465U,
      112637215U,
      2680153253U,
      3904427059U,
      2013776290U,
      251722036U,
      2517215374U,
      3775830040U,
      2137656763U,
      141376813U,
      2439277719U,
      3865271297U,
      1802195444U,
      476864866U,
      2238001368U,
      4066508878U,
      1812370925U,
      453092731U,
      2181625025U,
      4111451223U,
      1706088902U,
      314042704U,
      2344532202U,
      4240017532U,
      1658658271U,
      366619977U,
      2362670323U,
      4224994405U,
      1303535960U,
      984961486U,
      2747007092U,
      3569037538U,
      1256170817U,
      1037604311U,
      2765210733U,
      3554079995U,
      1131014506U,
      879679996U,
      2909243462U,
      3663771856U,
      1141124467U,
      855842277U,
      2852801631U,
      3708648649U,
      1342533948U,
      654459306U,
      3188396048U,
      3373015174U,
      1466479909U,
      544179635U,
      3110523913U,
      3462522015U,
      1591671054U,
      702138776U,
      2966460450U,
      3352799412U,
      1504918807U,
      783551873U,
      3082640443U,
      3233442989U,
      3988292384U,
      2596254646U,
      62317068U,
      1957810842U,
      3939845945U,
      2647816111U,
      81470997U,
      1943803523U,
      3814918930U,
      2489596804U,
      225274430U,
      2053790376U,
      3826175755U,
      2466906013U,
      167816743U,
      2097651377U,
      4027552580U,
      2265490386U,
      503444072U,
      1762050814U,
      4150417245U,
      2154129355U,
      426522225U,
      1852507879U,
      4275313526U,
      2312317920U,
      282753626U,
      1742555852U,
      4189708143U,
      2394877945U,
      397917763U,
      1622183637U,
      3604390888U,
      2714866558U,
      953729732U,
      1340076626U,
      3518719985U,
      2797360999U,
      1068828381U,
      1219638859U,
      3624741850U,
      2936675148U,
      906185462U,
      1090812512U,
      3747672003U,
      2825379669U,
      829329135U,
      1181335161U,
      3412177804U,
      3160834842U,
      628085408U,
      1382605366U,
      3423369109U,
      3138078467U,
      570562233U,
      1426400815U,
      3317316542U,
      2998733608U,
      733239954U,
      1555261956U,
      3268935591U,
      3050360625U,
      752459403U,
      1541320221U,
      2607071920U,
      3965973030U,
      1969922972U,
      40735498U,
      2617837225U,
      3943577151U,
      1913087877U,
      83908371U,
      2512341634U,
      3803740692U,
      2075208622U,
      213261112U,
      2463272603U,
      3855990285U,
      2094854071U,
      198958881U,
      2262029012U,
      4057260610U,
      1759359992U,
      534414190U,
      2176718541U,
      4139329115U,
      1873836001U,
      414664567U,
      2282248934U,
      4279200368U,
      1711684554U,
      285281116U,
      2405801727U,
      4167216745U,
      1634467795U,
      376229701U,
      2685067896U,
      3608007406U,
      1308918612U,
      956543938U,
      2808555105U,
      3495958263U,
      1231636301U,
      1047427035U,
      2932959818U,
      3654703836U,
      1088359270U,
      936918000U,
      2847714899U,
      3736837829U,
      1202900863U,
      817233897U,
      3183342108U,
      3401237130U,
      1404277552U,
      615818150U,
      3134207493U,
      3453421203U,
      1423857449U,
      601450431U,
      3009837614U,
      3294710456U,
      1567103746U,
      711928724U,
      3020668471U,
      3272380065U,
      1510334235U,
      755167117U
    };

    public static unsafe int IndexOf(byte[] buffer, byte val, int offset, int count)
    {
      fixed (byte* numPtr1 = buffer)
      {
        byte* numPtr2;
        for (numPtr2 = numPtr1 + offset; ((int) numPtr2 & 3) != 0; ++numPtr2)
        {
          if (count == 0)
            return -1;
          if ((int) *numPtr2 == (int) val)
            return (int) (numPtr2 - numPtr1);
          --count;
        }
        uint num1 = (uint) val + ((uint) val << 8);
        uint num2 = num1 + (num1 << 16);
        while (count >= 32)
        {
          offset = 0;
          uint num3 = *(uint*) numPtr2 ^ num2;
          if ((((int) num3 ^ -1 ^ 2130640639 + (int) num3) & -2130640640) == 0)
          {
            offset += 4;
            uint num4 = *(uint*) (numPtr2 + 4) ^ num2;
            if ((((int) num4 ^ -1 ^ 2130640639 + (int) num4) & -2130640640) == 0)
            {
              offset += 4;
              uint num5 = *(uint*) (numPtr2 + 8) ^ num2;
              if ((((int) num5 ^ -1 ^ 2130640639 + (int) num5) & -2130640640) == 0)
              {
                offset += 4;
                uint num6 = *(uint*) (numPtr2 + 12) ^ num2;
                if ((((int) num6 ^ -1 ^ 2130640639 + (int) num6) & -2130640640) == 0)
                {
                  offset += 4;
                  uint num7 = *(uint*) (numPtr2 + 16) ^ num2;
                  if ((((int) num7 ^ -1 ^ 2130640639 + (int) num7) & -2130640640) == 0)
                  {
                    offset += 4;
                    uint num8 = *(uint*) (numPtr2 + 20) ^ num2;
                    if ((((int) num8 ^ -1 ^ 2130640639 + (int) num8) & -2130640640) == 0)
                    {
                      offset += 4;
                      uint num9 = *(uint*) (numPtr2 + 24) ^ num2;
                      if ((((int) num9 ^ -1 ^ 2130640639 + (int) num9) & -2130640640) == 0)
                      {
                        offset += 4;
                        uint num10 = *(uint*) (numPtr2 + 28) ^ num2;
                        if ((((int) num10 ^ -1 ^ 2130640639 + (int) num10) & -2130640640) == 0)
                        {
                          numPtr2 += 32;
                          count -= 32;
                          continue;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
          byte* numPtr3 = numPtr2 + offset;
          int num11 = (int) (numPtr3 - numPtr1);
          if ((int) *numPtr3 == (int) val)
            return num11;
          if ((int) numPtr3[1] == (int) val)
            return num11 + 1;
          if ((int) numPtr3[2] == (int) val)
            return num11 + 2;
          if ((int) numPtr3[3] == (int) val)
            return num11 + 3;
          numPtr2 = numPtr3 + 4;
          count -= offset + 4;
        }
        while (count != 0)
        {
          if ((int) *numPtr2 == (int) val)
            return (int) (numPtr2 - numPtr1);
          --count;
          ++numPtr2;
        }
        return -1;
      }
    }

    public static unsafe int IndexOf(byte[] buffer, byte val, int offset, int count, out bool containsBinary)
    {
      containsBinary = false;
      fixed (byte* numPtr1 = buffer)
      {
        byte* numPtr2;
        for (numPtr2 = numPtr1 + offset; ((int) numPtr2 & 3) != 0; ++numPtr2)
        {
          if (count == 0)
            return -1;
          if (((int) *numPtr2 & 128) != 0)
            containsBinary = true;
          if ((int) *numPtr2 == (int) val)
            return (int) (numPtr2 - numPtr1);
          --count;
        }
        uint num1 = (uint) val + ((uint) val << 8);
        uint num2 = num1 + (num1 << 16);
        bool flag = false;
        while (count >= 32)
        {
          containsBinary = containsBinary || flag;
          offset = 0;
          uint num3 = *(uint*) numPtr2 ^ num2;
          flag = ((int) *(uint*) numPtr2 & -2139062144) != 0;
          if ((((int) num3 ^ -1 ^ 2130640639 + (int) num3) & -2130640640) == 0)
          {
            containsBinary = containsBinary || flag;
            offset += 4;
            uint num4 = *(uint*) (numPtr2 + 4) ^ num2;
            flag = ((int) *(uint*) (numPtr2 + 4) & -2139062144) != 0;
            if ((((int) num4 ^ -1 ^ 2130640639 + (int) num4) & -2130640640) == 0)
            {
              containsBinary = containsBinary || flag;
              offset += 4;
              uint num5 = *(uint*) (numPtr2 + 8) ^ num2;
              flag = ((int) *(uint*) (numPtr2 + 8) & -2139062144) != 0;
              if ((((int) num5 ^ -1 ^ 2130640639 + (int) num5) & -2130640640) == 0)
              {
                containsBinary = containsBinary || flag;
                offset += 4;
                uint num6 = *(uint*) (numPtr2 + 12) ^ num2;
                flag = ((int) *(uint*) (numPtr2 + 12) & -2139062144) != 0;
                if ((((int) num6 ^ -1 ^ 2130640639 + (int) num6) & -2130640640) == 0)
                {
                  containsBinary = containsBinary || flag;
                  offset += 4;
                  uint num7 = *(uint*) (numPtr2 + 16) ^ num2;
                  flag = ((int) *(uint*) (numPtr2 + 16) & -2139062144) != 0;
                  if ((((int) num7 ^ -1 ^ 2130640639 + (int) num7) & -2130640640) == 0)
                  {
                    containsBinary = containsBinary || flag;
                    offset += 4;
                    uint num8 = *(uint*) (numPtr2 + 20) ^ num2;
                    flag = ((int) *(uint*) (numPtr2 + 20) & -2139062144) != 0;
                    if ((((int) num8 ^ -1 ^ 2130640639 + (int) num8) & -2130640640) == 0)
                    {
                      containsBinary = containsBinary || flag;
                      offset += 4;
                      uint num9 = *(uint*) (numPtr2 + 24) ^ num2;
                      flag = ((int) *(uint*) (numPtr2 + 24) & -2139062144) != 0;
                      if ((((int) num9 ^ -1 ^ 2130640639 + (int) num9) & -2130640640) == 0)
                      {
                        containsBinary = containsBinary || flag;
                        offset += 4;
                        uint num10 = *(uint*) (numPtr2 + 28) ^ num2;
                        uint num11 = *(uint*) (numPtr2 + 28);
                        flag = ((int) num11 & -2139062144) != 0;
                        if ((((int) num10 ^ -1 ^ 2130640639 + (int) num10) & -2130640640) == 0)
                        {
                          containsBinary = containsBinary || flag;
                          flag = ((int) num11 & -2139062144) != 0;
                          numPtr2 += 32;
                          count -= 32;
                          continue;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
          byte* numPtr3 = numPtr2 + offset;
          int num12 = (int) (numPtr3 - numPtr1);
          if ((int) *numPtr3 == (int) val)
          {
            containsBinary = containsBinary || ((int) *numPtr3 & 128) != 0;
            return num12;
          }
          if ((int) numPtr3[1] == (int) val)
          {
            containsBinary = containsBinary || ((int) *numPtr3 & 128) != 0 || ((int) numPtr3[1] & 128) != 0;
            return num12 + 1;
          }
          if ((int) numPtr3[2] == (int) val)
          {
            containsBinary = containsBinary || ((int) *numPtr3 & 128) != 0 || ((int) numPtr3[1] & 128) != 0 || ((int) numPtr3[2] & 128) != 0;
            return num12 + 2;
          }
          if ((int) numPtr3[3] == (int) val)
          {
            containsBinary = containsBinary || ((int) *numPtr3 & 128) != 0 || (((int) numPtr3[1] & 128) != 0 || ((int) numPtr3[2] & 128) != 0) || ((int) numPtr3[3] & 128) != 0;
            return num12 + 3;
          }
          numPtr2 = numPtr3 + 4;
          count -= offset + 4;
          containsBinary = containsBinary || flag;
        }
        while (count != 0)
        {
          if (((int) *numPtr2 & 128) != 0)
            containsBinary = true;
          if ((int) *numPtr2 == (int) val)
            return (int) (numPtr2 - numPtr1);
          --count;
          ++numPtr2;
        }
        return -1;
      }
    }

    public static void ValidateStringArgument(string value, bool allowUTF8)
    {
      if (ByteString.IsStringArgumentValid(value, allowUTF8))
        return;
      if (allowUTF8)
        throw new ArgumentException(Resources.SharedStrings.StringArgumentMustBeUTF8);
      throw new ArgumentException(Resources.SharedStrings.StringArgumentMustBeAscii);
    }

    public static bool IsStringArgumentValid(string value, bool allowUTF8)
    {
      if (allowUTF8)
        return true;
      for (int index = 0; index < value.Length; ++index)
      {
        if ((int) value[index] >= 128)
          return false;
      }
      return true;
    }

    public static int StringToBytesCount(string value, bool allowUTF8)
    {
      if (string.IsNullOrEmpty(value))
        return 0;
      if (!allowUTF8)
        return value.Length;
      try
      {
        return System.Text.Encoding.UTF8.GetByteCount(value);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(Resources.SharedStrings.StringArgumentMustBeUTF8, ex);
      }
    }

    public static byte[] StringToBytesAndAppendCRLF(string value, bool allowUTF8)
    {
      if (string.IsNullOrEmpty(value))
        return ByteString.Empty;
      return ByteString.StringToBytes(value + Environment.NewLine, allowUTF8);
    }

    public static byte[] StringToBytes(string value, bool allowUTF8 = true)
    {
      if (string.IsNullOrEmpty(value))
        return ByteString.Empty;
      if (allowUTF8)
      {
        try
        {
          return System.Text.Encoding.UTF8.GetBytes(value);
        }
        catch (Exception ex)
        {
          throw new ArgumentException(Resources.SharedStrings.StringArgumentMustBeUTF8, ex);
        }
      }
      else
      {
        byte[] numArray = new byte[value.Length];
        for (int index = 0; index < value.Length; ++index)
          numArray[index] = (int) value[index] < 128 ? (byte) value[index] : (byte) 63;
        return numArray;
      }
    }

    public static int StringToBytes(string value, byte[] bytes, int bytesOffset, bool allowUTF8)
    {
      if (string.IsNullOrEmpty(value))
        return 0;
      return ByteString.StringToBytes(value, 0, value.Length, bytes, bytesOffset, allowUTF8);
    }

    public static int StringToBytes(string value, int valueOffset, int valueCount, byte[] bytes, int bytesOffset, bool allowUTF8)
    {
      if (allowUTF8)
      {
        try
        {
          return System.Text.Encoding.UTF8.GetBytes(value, valueOffset, valueCount, bytes, bytesOffset);
        }
        catch (Exception ex)
        {
          throw new ArgumentException(Resources.SharedStrings.StringArgumentMustBeUTF8, ex);
        }
      }
      else
      {
        for (int index = 0; index < valueCount; ++index)
          bytes[bytesOffset + index] = (int) value[valueOffset + index] < 128 ? (byte) value[valueOffset + index] : (byte) 63;
        return valueCount;
      }
    }

    public static string BytesToString(byte[] bytes, bool allowUTF8)
    {
      if (bytes == null || bytes.Length == 0)
        return string.Empty;
      return ByteString.BytesToString(bytes, 0, bytes.Length, allowUTF8);
    }

    public static string BytesToString(byte[] bytes, int offset, int count, bool allowUTF8)
    {
      if (allowUTF8)
      {
        try
        {
          return System.Text.Encoding.UTF8.GetString(bytes, offset, count);
        }
        catch (Exception ex)
        {
          throw new ArgumentException(Resources.SharedStrings.StringArgumentMustBeUTF8, ex);
        }
      }
      else
      {
        char[] chArray = new char[count];
        for (int index = 0; index < count; ++index)
          chArray[index] = (int) bytes[offset + index] < 128 ? (char) bytes[offset + index] : '?';
        return new string(chArray);
      }
    }

    public static int BytesToCharCount(byte[] bytes, bool allowUTF8)
    {
      if (bytes == null || bytes.Length == 0)
        return 0;
      if (!allowUTF8)
        return bytes.Length;
      try
      {
        return System.Text.Encoding.UTF8.GetCharCount(bytes);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(Resources.SharedStrings.StringArgumentMustBeUTF8, ex);
      }
    }

    public static char BytesToChar(byte[] bytes, int index, out int bytesUsed, out bool replacementChar, bool allowUTF8)
    {
      if (allowUTF8)
      {
        if ((int) bytes[index] < 128)
        {
          bytesUsed = 1;
          replacementChar = false;
          return (char) bytes[index];
        }
        int bytesUsed1 = 0;
        if (ByteString.IsUTF8NonASCII(bytes[index], index + 1 < bytes.Length ? bytes[index + 1] : (byte) 0, index + 2 < bytes.Length ? bytes[index + 2] : (byte) 0, index + 3 < bytes.Length ? bytes[index + 3] : (byte) 0, out bytesUsed1))
        {
          try
          {
            char[] chars = System.Text.Encoding.UTF8.GetChars(bytes, index, bytesUsed1);
            bytesUsed = bytesUsed1;
            replacementChar = false;
            return chars[0];
          }
          catch (Exception ex)
          {
          }
        }
        bytesUsed = 1;
        replacementChar = true;
        return '�';
      }
      bytesUsed = 1;
      if ((int) bytes[index] < 128)
      {
        replacementChar = false;
        return (char) bytes[index];
      }
      replacementChar = true;
      return '?';
    }

    internal static int CompareI(byte[] str1, int str1Offset, int str1Length, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
    {
      if (str1 == null || str2 == null)
      {
        if (str1 != null)
          return 1;
        return str2 != null ? -1 : 0;
      }
      if (allowUTF8)
      {
        int num1 = 0;
        int num2 = 0;
        while (num1 < str1Length && num2 < str2Length)
        {
          int index1 = str1Offset + num1;
          int index2 = str2Offset + num2;
          if ((int) str1[index1] < 128 && (int) str2[index2] < 128)
          {
            byte num3 = ByteString.LowerC[(int) str1[index1]];
            byte num4 = ByteString.LowerC[(int) str2[index2]];
            if ((int) num3 != (int) num4)
              return (int) num3 >= (int) num4 ? 1 : -1;
            ++num1;
            ++num2;
          }
          else
          {
            int bytesUsed1 = 0;
            bool replacementChar1 = false;
            byte num3 = str1[index1];
            char c1 = ByteString.BytesToChar(str1, index1, out bytesUsed1, out replacementChar1, allowUTF8);
            int bytesUsed2 = 0;
            bool replacementChar2 = false;
            byte num4 = str2[index2];
            char c2 = ByteString.BytesToChar(str2, index2, out bytesUsed2, out replacementChar2, allowUTF8);
            if (num1 + bytesUsed1 > str1Length || num2 + bytesUsed2 > str2Length)
            {
              if ((int) num3 != (int) num4)
                return (int) num3 >= (int) num4 ? 1 : -1;
              ++num1;
              ++num2;
            }
            else
            {
              if (replacementChar1 || replacementChar2)
              {
                if (replacementChar1 != replacementChar2)
                  return (int) c1 >= (int) c2 ? 1 : -1;
                if ((int) num3 != (int) num4)
                  return (int) num3 >= (int) num4 ? 1 : -1;
              }
              else
              {
                char ch1 = char.ToLower(c1);
                char ch2 = char.ToLower(c2);
                if ((int) ch1 != (int) ch2)
                  return (int) ch1 >= (int) ch2 ? 1 : -1;
              }
              num1 += bytesUsed1;
              num2 += bytesUsed2;
            }
          }
        }
        if (num1 != str1Length)
          return 1;
        return num2 >= str2Length ? 0 : -1;
      }
      int num5;
      for (num5 = 0; num5 < str1Length && num5 < str2Length; ++num5)
      {
        int index1 = str1Offset + num5;
        int index2 = str2Offset + num5;
        byte num1 = (int) str1[index1] < 128 ? ByteString.LowerC[(int) str1[index1]] : str1[index1];
        byte num2 = (int) str2[index2] < 128 ? ByteString.LowerC[(int) str2[index2]] : str2[index2];
        if ((int) num1 != (int) num2)
          return (int) num1 >= (int) num2 ? 1 : -1;
      }
      if (num5 != str1Length)
        return 1;
      return num5 >= str2Length ? 0 : -1;
    }

    internal static int CompareI(string str1, int str1Offset, int str1Length, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
    {
      if (str1 == null || str2 == null)
      {
        if (str1 != null)
          return 1;
        return str2 != null ? -1 : 0;
      }
      if (allowUTF8)
      {
        int num1 = 0;
        int num2 = 0;
        while (num1 < str1Length && num2 < str2Length)
        {
          int index1 = str1Offset + num1;
          int index2 = str2Offset + num2;
          if ((int) str1[index1] < 128 && (int) str2[index2] < 128)
          {
            byte num3 = ByteString.LowerC[(int) str1[index1]];
            byte num4 = ByteString.LowerC[(int) str2[index2]];
            if ((int) num3 != (int) num4)
              return (int) num3 >= (int) num4 ? 1 : -1;
            ++num1;
            ++num2;
          }
          else
          {
            byte num3 = (byte) str1[index1];
            char c1 = str1[index1];
            int bytesUsed = 0;
            bool replacementChar = false;
            byte num4 = str2[index2];
            char c2 = ByteString.BytesToChar(str2, index2, out bytesUsed, out replacementChar, allowUTF8);
            if (num2 + bytesUsed > str2Length)
            {
              if ((int) num3 != (int) num4)
                return (int) num3 >= (int) num4 ? 1 : -1;
              ++num1;
              ++num2;
            }
            else
            {
              if (replacementChar)
              {
                if ((int) num3 != (int) num4)
                  return (int) num3 >= (int) num4 ? 1 : -1;
              }
              else
              {
                char ch1 = char.ToLower(c1);
                char ch2 = char.ToLower(c2);
                if ((int) ch1 != (int) ch2)
                  return (int) ch1 >= (int) ch2 ? 1 : -1;
              }
              ++num1;
              num2 += bytesUsed;
            }
          }
        }
        if (num1 != str1Length)
          return 1;
        return num2 >= str2Length ? 0 : -1;
      }
      int num5;
      for (num5 = 0; num5 < str1Length && num5 < str2Length; ++num5)
      {
        int index1 = str1Offset + num5;
        int index2 = str2Offset + num5;
        byte num1 = (int) str1[index1] < 128 ? ByteString.LowerC[(int) (byte) str1[index1]] : (byte) str1[index1];
        byte num2 = (int) str2[index2] < 128 ? ByteString.LowerC[(int) str2[index2]] : str2[index2];
        if ((int) num1 != (int) num2)
          return (int) num1 >= (int) num2 ? 1 : -1;
      }
      if (num5 != str1Length)
        return 1;
      return num5 >= str2Length ? 0 : -1;
    }

    internal static int CompareI(byte[] str1, byte[] str2, bool allowUTF8)
    {
      if (str1 != null && str2 != null)
        return ByteString.CompareI(str1, 0, str1.Length, str2, 0, str2.Length, allowUTF8);
      if (str1 != null)
        return 1;
      return str2 != null ? -1 : 0;
    }

    internal static bool EqualsI(byte[] str1, int str1Offset, int str1Length, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
    {
      if (str1 == null || str2 == null)
      {
        if (str1 == null)
          return str2 == null;
        return false;
      }
      if (!allowUTF8 && str1Length != str2Length)
        return false;
      return ByteString.CompareI(str1, str1Offset, str1Length, str2, str2Offset, str2Length, allowUTF8) == 0;
    }

    internal static bool EqualsI(string str1, byte[] str2, int str2Offset, int str2Length, bool allowUTF8)
    {
      if (str1 == null || str2 == null)
      {
        if (str1 == null)
          return str2 == null;
        return false;
      }
      if (!allowUTF8 && str1.Length != str2Length)
        return false;
      return ByteString.CompareI(str1, 0, str1.Length, str2, str2Offset, str2Length, allowUTF8) == 0;
    }

    internal static uint ComputeCrcI(byte[] bytes, int offset, int length)
    {
      uint seed = 0U;
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = offset + index1;
        byte ch = (int) bytes[index2] < 128 ? ByteString.LowerC[(int) bytes[index2]] : bytes[index2];
        seed = ByteString.ComputeCrc(seed, ch);
      }
      return seed;
    }

    internal static uint ComputeCrc(byte[] bytes, int offset, int length)
    {
      uint seed = 0U;
      for (int index = 0; index < length; ++index)
        seed = ByteString.ComputeCrc(seed, bytes[offset + index]);
      return seed;
    }

    private static uint ComputeCrc(uint seed, byte ch)
    {
      return ByteString.CrcTable[(uint) (((int) seed ^ (int) ch) & (int) byte.MaxValue)] ^ seed >> 8;
    }

    internal static bool IsUTF8_2(byte ch1, byte ch2)
    {
      if ((int) ch1 >= 194 && (int) ch1 <= 223 && (int) ch2 >= 128)
        return (int) ch2 <= 191;
      return false;
    }

    internal static bool IsUTF8_3(byte ch1, byte ch2, byte ch3)
    {
      if ((int) ch1 == 224 && (int) ch2 >= 160 && ((int) ch2 <= 191 && (int) ch3 >= 128) && (int) ch3 <= 191 || (int) ch1 >= 225 && (int) ch1 <= 236 && ((int) ch2 >= 128 && (int) ch2 <= 191) && ((int) ch3 >= 128 && (int) ch3 <= 191) || (int) ch1 == 237 && (int) ch2 >= 128 && ((int) ch2 <= 159 && (int) ch3 >= 128) && (int) ch3 <= 191)
        return true;
      if ((int) ch1 >= 238 && (int) ch1 <= 239 && ((int) ch2 >= 128 && (int) ch2 <= 191) && (int) ch3 >= 128)
        return (int) ch3 <= 191;
      return false;
    }

    internal static bool IsUTF8_4(byte ch1, byte ch2, byte ch3, byte ch4)
    {
      if ((int) ch1 == 240 && (int) ch2 >= 144 && ((int) ch2 <= 191 && (int) ch3 >= 128) && ((int) ch3 <= 191 && (int) ch4 >= 128 && (int) ch4 <= 191) || (int) ch1 >= 241 && (int) ch1 <= 243 && ((int) ch2 >= 128 && (int) ch2 <= 191) && ((int) ch3 >= 128 && (int) ch3 <= 191 && ((int) ch4 >= 128 && (int) ch4 <= 191)))
        return true;
      if ((int) ch1 == 244 && (int) ch2 >= 128 && ((int) ch2 <= 143 && (int) ch3 >= 128) && ((int) ch3 <= 191 && (int) ch4 >= 128))
        return (int) ch4 <= 191;
      return false;
    }

    internal static bool IsUTF8NonASCII(byte ch1, byte ch2, byte ch3, byte ch4, out int bytesUsed)
    {
      if (ByteString.IsUTF8_2(ch1, ch2))
      {
        bytesUsed = 2;
        return true;
      }
      if (ByteString.IsUTF8_3(ch1, ch2, ch3))
      {
        bytesUsed = 3;
        return true;
      }
      if (ByteString.IsUTF8_4(ch1, ch2, ch3, ch4))
      {
        bytesUsed = 4;
        return true;
      }
      bytesUsed = 0;
      return false;
    }
  }
}
