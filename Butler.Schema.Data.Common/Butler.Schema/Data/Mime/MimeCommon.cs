// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeCommon
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Mime
{
  internal static class MimeCommon
  {
    internal static readonly EncodingOptions DefaultEncodingOptions = new EncodingOptions(Encoding.UTF8.WebName, (string) null, EncodingFlags.None);
    private static MimeCommon.CalculateMethodAndChunkSize calculateMethodAndChunkSizeSbcs = new MimeCommon.CalculateMethodAndChunkSize(MimeCommon.CalculateMethodAndChunkSize_Sbcs);
    private static MimeCommon.CalculateMethodAndChunkSize calculateMethodAndChunkSizeDbcs = new MimeCommon.CalculateMethodAndChunkSize(MimeCommon.CalculateMethodAndChunkSize_Dbcs);
    private static MimeCommon.CalculateMethodAndChunkSize calculateMethodAndChunkSizeUtf8 = new MimeCommon.CalculateMethodAndChunkSize(MimeCommon.CalculateMethodAndChunkSize_Utf8);
    private static MimeCommon.CalculateMethodAndChunkSize calculateMethodAndChunkSizeUnicode16 = new MimeCommon.CalculateMethodAndChunkSize(MimeCommon.CalculateMethodAndChunkSize_Unicode16);
    private static MimeCommon.CalculateMethodAndChunkSize calculateMethodAndChunkSizeUnicode32 = new MimeCommon.CalculateMethodAndChunkSize(MimeCommon.CalculateMethodAndChunkSize_Unicode32);
    private static MimeCommon.CalculateMethodAndChunkSize calculateMethodAndChunkSizeMbcs = new MimeCommon.CalculateMethodAndChunkSize(MimeCommon.CalculateMethodAndChunkSize_Mbcs);
    public const int FoldLineLength = 78;
    public const int MaxMimeLineLength = 998;
    public const int MaxBoundaryLength = 70;
    public const int BoundaryPrefixSuffixLength = 2;
    public const int MaxRfc2231SegmentsAllowed = 10000;

    internal static void CheckBufferArguments(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      if (count + offset > buffer.Length)
        throw new ArgumentException(CtsResources.Strings.LengthExceeded(offset + count, buffer.Length), nameof(buffer));
      if (0 > offset || 0 > count)
        throw new ArgumentOutOfRangeException(offset < 0 ? "offset" : "count");
    }

    internal static bool TryDecodeValue(MimeStringList lines, uint linesMask, DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
    {
      decodingResults = new DecodingResults();
      if (lines.GetLength(linesMask) == 0)
      {
        value = string.Empty;
        return true;
      }
      DecodingFlags decodingFlags = decodingOptions.DecodingFlags;
      bool enableFallback = DecodingFlags.None != (DecodingFlags.FallbackToRaw & decodingFlags);
      bool allowControlCharacters = DecodingFlags.None != (DecodingFlags.AllowControlCharacters & decodingFlags);
      bool enable2047 = false;
      bool enableJisDetection = false;
      bool enableUtf8Detection = false;
      bool enableDbcsDetection = false;
      Globalization.Charset defaultCharset = (Globalization.Charset) null;
      ValueDecoder valueDecoder = new ValueDecoder(lines, linesMask);
      if ((decodingFlags & DecodingFlags.AllEncodings) == DecodingFlags.None)
      {
        if (!enableFallback)
          defaultCharset = Globalization.Charset.ASCII;
      }
      else
      {
        enable2047 = DecodingFlags.None != (DecodingFlags.Rfc2047 & decodingFlags);
        enableJisDetection = DecodingFlags.None != (DecodingFlags.Jis & decodingFlags);
        enableUtf8Detection = DecodingFlags.None != (DecodingFlags.Utf8 & decodingFlags);
        enableDbcsDetection = DecodingFlags.None != (DecodingFlags.Dbcs & decodingFlags);
        defaultCharset = decodingOptions.Charset;
      }
      string charsetName;
      string cultureName;
      EncodingScheme encodingScheme;
      bool flag = valueDecoder.TryDecodeValue(defaultCharset, enableFallback, allowControlCharacters, enable2047, enableJisDetection, enableUtf8Detection, enableDbcsDetection, out charsetName, out cultureName, out encodingScheme, out value);
      decodingResults.EncodingScheme = encodingScheme;
      decodingResults.CharsetName = charsetName;
      decodingResults.CultureName = cultureName;
      decodingResults.DecodingFailed = !flag;
      return flag;
    }

    internal static void ThrowDecodingFailedException(ref DecodingResults decodingResults)
    {
      Globalization.Charset.GetEncoding(decodingResults.CharsetName);
      throw new ExchangeDataException("internal value decoding error");
    }

    public static bool IsAnySurrogate(char ch)
    {
      if (55296 <= (int) ch)
        return (int) ch < 57344;
      return false;
    }

    public static bool IsHighSurrogate(char ch)
    {
      if (55296 <= (int) ch)
        return (int) ch < 56320;
      return false;
    }

    public static bool IsLowSurrogate(char ch)
    {
      if (56320 <= (int) ch)
        return (int) ch < 57344;
      return false;
    }

    internal static bool IsEncodingRequired(string value, bool allowUTF8)
    {
      if (string.IsNullOrEmpty(value))
        return false;
      char[] chArray = new char[1];
      int num = 0;
      for (int index = 0; index < value.Length; ++index)
      {
        char ch = value[index];
        if ((int) ch < 128)
        {
          if (MimeScan.IsEncodingRequired((byte) ch))
            return true;
          if (MimeScan.IsLWSP((byte) ch))
            num = 0;
          else
            ++num;
        }
        else
        {
          if (!allowUTF8)
            return true;
          chArray[0] = ch;
          num += Internal.ByteString.StringToBytesCount(new string(chArray), allowUTF8);
        }
        if (998 < num + 1)
          return true;
      }
      return false;
    }

    internal static MimeStringList EncodeValue(string value, EncodingOptions encodingOptions, ValueEncodingStyle style)
    {
      if (string.IsNullOrEmpty(value))
        return MimeStringList.Empty;
      if (!MimeCommon.IsEncodingRequired(value, encodingOptions.AllowUTF8))
        return new MimeStringList(new MimeString(value));
      MimeStringList mimeStringList = new MimeStringList();
      Globalization.Charset charset;
      if (encodingOptions.CharsetName != null)
      {
        charset = encodingOptions.GetEncodingCharset();
      }
      else
      {
        Globalization.OutboundCodePageDetector codePageDetector = new Globalization.OutboundCodePageDetector();
        codePageDetector.AddText(value);
        if (!Globalization.Charset.TryGetCharset(codePageDetector.GetCodePage(), out charset))
          charset = MimeCommon.DefaultEncodingOptions.GetEncodingCharset();
      }
      Encoders.ByteEncoder.Tables.CharClasses charClasses = Encoders.ByteEncoder.Tables.CharClasses.QEncode;
      if (style == ValueEncodingStyle.Phrase)
        charClasses |= Encoders.ByteEncoder.Tables.CharClasses.QPhraseUnsafe;
      else if (style == ValueEncodingStyle.Comment)
        charClasses |= Encoders.ByteEncoder.Tables.CharClasses.QCommentUnsafe;
      bool allowQEncoding = false;
      MimeCommon.CalculateMethodAndChunkSize methodAndChunkSize;
      if (charset.Kind == Globalization.CodePageKind.Sbcs)
      {
        methodAndChunkSize = MimeCommon.calculateMethodAndChunkSizeSbcs;
        if (charset.AsciiSupport >= Globalization.CodePageAsciiSupport.Fine)
          allowQEncoding = true;
      }
      else if (charset.Kind == Globalization.CodePageKind.Dbcs)
      {
        methodAndChunkSize = MimeCommon.calculateMethodAndChunkSizeDbcs;
        if (charset.AsciiSupport >= Globalization.CodePageAsciiSupport.Fine)
          allowQEncoding = true;
      }
      else if (charset.CodePage == 65001)
      {
        methodAndChunkSize = MimeCommon.calculateMethodAndChunkSizeUtf8;
        allowQEncoding = true;
      }
      else
        methodAndChunkSize = charset.CodePage == 1200 || charset.CodePage == 1201 ? MimeCommon.calculateMethodAndChunkSizeUnicode16 : (charset.CodePage == 12000 || charset.CodePage == 12001 ? MimeCommon.calculateMethodAndChunkSizeUnicode32 : MimeCommon.calculateMethodAndChunkSizeMbcs);
      int num1 = 75;
      int num2 = 7 + charset.Name.Length;
      int num3 = num1 - num2;
      if (num3 < 32)
      {
        num1 = num2 + 32;
        num3 = 32;
      }
      byte[] byteBuffer = Internal.ScratchPad.GetByteBuffer(num3);
      Encoding encoding = charset.GetEncoding();
      byte[] numArray1 = new byte[5 + charset.Name.Length];
      int num4 = 0;
      byte[] numArray2 = numArray1;
      int index1 = num4;
      int num5 = 1;
      int num6 = index1 + num5;
      int num7 = 61;
      numArray2[index1] = (byte) num7;
      byte[] numArray3 = numArray1;
      int index2 = num6;
      int num8 = 1;
      int bytesOffset = index2 + num8;
      int num9 = 63;
      numArray3[index2] = (byte) num9;
      int num10 = bytesOffset + Internal.ByteString.StringToBytes(charset.Name, numArray1, bytesOffset, false);
      byte[] numArray4 = numArray1;
      int index3 = num10;
      int num11 = 1;
      int num12 = index3 + num11;
      int num13 = 63;
      numArray4[index3] = (byte) num13;
      byte[] numArray5 = numArray1;
      int index4 = num12;
      int num14 = 1;
      int num15 = index4 + num14;
      int num16 = 88;
      numArray5[index4] = (byte) num16;
      byte[] numArray6 = numArray1;
      int index5 = num15;
      int num17 = 1;
      int num18 = index5 + num17;
      int num19 = 63;
      numArray6[index5] = (byte) num19;
      MimeString mimeString = new MimeString(numArray1);
      int num20 = 0;
      byte[] numArray7 = (byte[]) null;
      int count = 0;
      int num21 = num3 / 4;
      while (num20 != value.Length)
      {
        byte method;
        int chunkSize;
        methodAndChunkSize(allowQEncoding, charClasses, encoding, value, num20, num3, out method, out chunkSize);
label_25:
        int bytes;
        int outputOffset;
        while (true)
        {
          do
          {
            do
            {
              try
              {
                bytes = encoding.GetBytes(value, num20, chunkSize, byteBuffer, 0);
              }
              catch (ArgumentException ex)
              {
                if (chunkSize < 2)
                {
                  throw;
                }
                else
                {
                  chunkSize -= chunkSize > 10 ? 3 : 1;
                  if (MimeCommon.IsLowSurrogate(value[num20 + chunkSize]))
                  {
                    if (MimeCommon.IsHighSurrogate(value[num20 + chunkSize - 1]))
                    {
                      --chunkSize;
                      goto label_25;
                    }
                    else
                      goto label_25;
                  }
                  else
                    goto label_25;
                }
              }
              if (bytes != 0)
              {
                if (numArray7 == null || numArray7.Length - count < num1 + 1)
                {
                  if (numArray7 != null)
                  {
                    mimeStringList.Append(new MimeString(numArray7, 0, count));
                    count = 0;
                  }
                  numArray7 = new byte[Math.Min(((value.Length - num20) / chunkSize + 1) * (num1 + 1), 4096 / (num1 + 1) * (num1 + 1))];
                }
                int destinationIndex = count;
                if (mimeStringList.Count > 0 || destinationIndex > 0)
                  numArray7[destinationIndex++] = (byte) 32;
                outputOffset = destinationIndex + mimeString.CopyTo(numArray7, destinationIndex);
                numArray7[outputOffset - 2] = method;
                if ((int) method == 81)
                {
                  int num22 = outputOffset;
                  int index6;
                  for (index6 = 0; index6 < bytes && outputOffset - num22 + 1 <= num3; ++index6)
                  {
                    byte num23 = byteBuffer[index6];
                    if (MimeCommon.QEncodingRequired((char) num23, charClasses))
                    {
                      if (outputOffset - num22 + 3 <= num3)
                      {
                        byte[] numArray8 = numArray7;
                        int index7 = outputOffset;
                        int num24 = 1;
                        int num25 = index7 + num24;
                        int num26 = 61;
                        numArray8[index7] = (byte) num26;
                        byte[] numArray9 = numArray7;
                        int index8 = num25;
                        int num27 = 1;
                        int num28 = index8 + num27;
                        int num29 = (int) Encoders.ByteEncoder.NibbleToHex[(int) num23 >> 4];
                        numArray9[index8] = (byte) num29;
                        byte[] numArray10 = numArray7;
                        int index9 = num28;
                        int num30 = 1;
                        outputOffset = index9 + num30;
                        int num31 = (int) Encoders.ByteEncoder.NibbleToHex[(int) num23 & 15];
                        numArray10[index9] = (byte) num31;
                      }
                      else
                        break;
                    }
                    else
                    {
                      if ((int) num23 == 32)
                        num23 = (byte) 95;
                      numArray7[outputOffset++] = num23;
                    }
                  }
                  if (index6 != bytes)
                  {
                    if (chunkSize < 2)
                      throw new InvalidOperationException("unexpected thing just happened");
                    chunkSize -= chunkSize > 10 ? 3 : 1;
                  }
                  else
                    goto label_56;
                }
                else
                  goto label_55;
              }
              else
                goto label_57;
            }
            while (!MimeCommon.IsLowSurrogate(value[num20 + chunkSize]));
          }
          while (!MimeCommon.IsHighSurrogate(value[num20 + chunkSize - 1]));
          --chunkSize;
        }
label_55:
        outputOffset += MimeCommon.Base64EncodeChunk(byteBuffer, 0, bytes, numArray7, outputOffset);
label_56:
        byte[] numArray11 = numArray7;
        int index10 = outputOffset;
        int num32 = 1;
        int num33 = index10 + num32;
        int num34 = 63;
        numArray11[index10] = (byte) num34;
        byte[] numArray12 = numArray7;
        int index11 = num33;
        int num35 = 1;
        int num36 = index11 + num35;
        int num37 = 61;
        numArray12[index11] = (byte) num37;
        count = num36;
label_57:
        num20 += chunkSize;
      }
      if (numArray7 != null)
        mimeStringList.Append(new MimeString(numArray7, 0, count));
      return mimeStringList;
    }

    internal static int Base64EncodeChunk(byte[] input, int offset, int length, byte[] encodedOutput, int outputOffset)
    {
      int num = 0;
      while (length >= 3)
      {
        encodedOutput[outputOffset++] = Encoders.ByteEncoder.Tables.ByteToBase64[(int) input[offset] >> 2 & 63];
        encodedOutput[outputOffset++] = Encoders.ByteEncoder.Tables.ByteToBase64[((int) input[offset] << 4 | (int) input[offset + 1] >> 4) & 63];
        encodedOutput[outputOffset++] = Encoders.ByteEncoder.Tables.ByteToBase64[((int) input[offset + 1] << 2 | (int) input[offset + 2] >> 6) & 63];
        encodedOutput[outputOffset++] = Encoders.ByteEncoder.Tables.ByteToBase64[(int) input[offset + 2] & 63];
        length -= 3;
        offset += 3;
        num += 4;
      }
      if (length > 0)
      {
        encodedOutput[outputOffset++] = Encoders.ByteEncoder.Tables.ByteToBase64[(int) input[offset] >> 2 & 63];
        encodedOutput[outputOffset++] = Encoders.ByteEncoder.Tables.ByteToBase64[((int) input[offset] << 4 | (length < 2 ? 0 : (int) input[offset + 1] >> 4)) & 63];
        encodedOutput[outputOffset++] = length < 2 ? (byte) 61 : Encoders.ByteEncoder.Tables.ByteToBase64[(int) input[offset + 1] << 2 & 63];
        encodedOutput[outputOffset++] = (byte) 61;
        num += 4;
      }
      return num;
    }

    private static void CalculateMethodAndChunkSize_Sbcs(bool allowQEncoding, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClassesForQEncoding, Encoding encoding, string value, int valueOffset, int encodedWordSpace, out byte method, out int chunkSize)
    {
      int num1 = encodedWordSpace / 4 * 3;
      int chunkSize1 = Math.Min(num1, value.Length - valueOffset);
      if (chunkSize1 != value.Length - valueOffset && MimeCommon.IsHighSurrogate(value[valueOffset + chunkSize1 - 1]) && MimeCommon.IsLowSurrogate(value[valueOffset + chunkSize1]))
        --chunkSize1;
      int num2 = (chunkSize1 + 2) / 3 * 4;
      if (allowQEncoding)
      {
        int num3 = 0;
        int num4 = 0;
        int index = valueOffset;
        while (index != value.Length && num4 < encodedWordSpace)
        {
          int num5 = 1;
          int num6 = 1;
          char ch = value[index++];
          if (MimeCommon.QEncodingRequired(ch, unsafeCharClassesForQEncoding))
          {
            num6 = 3;
            if (MimeCommon.IsHighSurrogate(ch) && index != value.Length && MimeCommon.IsLowSurrogate(value[index]))
            {
              ++index;
              ++num5;
              num6 = 6;
            }
          }
          if (num4 + num6 <= encodedWordSpace)
          {
            num4 += num6;
            num3 += num5;
            if (num4 > num2 && num3 <= chunkSize1)
              break;
          }
          else
            break;
        }
        if (num3 >= chunkSize1 && num4 < num2 + 3)
        {
          chunkSize = num3;
          method = (byte) 81;
        }
        else
        {
          chunkSize = MimeCommon.AdjustChunkSize(encoding, chunkSize1, value, valueOffset, num1);
          method = (byte) 66;
        }
      }
      else
      {
        chunkSize = MimeCommon.AdjustChunkSize(encoding, chunkSize1, value, valueOffset, num1);
        method = (byte) 66;
      }
    }

    private static void CalculateMethodAndChunkSize_Dbcs(bool allowQEncoding, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClassesForQEncoding, Encoding encoding, string value, int valueOffset, int encodedWordSpace, out byte method, out int chunkSize)
    {
      int targetBytes = encodedWordSpace / 4 * 3;
      int num1 = 0;
      int num2 = 0;
      int chunkSize1 = 0;
      int num3 = 0;
      int index = valueOffset;
      bool flag1 = false;
      bool flag2 = !allowQEncoding;
      while (index != value.Length && (!flag1 || !flag2))
      {
        int num4 = 1;
        int num5 = 1;
        int num6 = 1;
        char ch = value[index++];
        if (MimeCommon.QEncodingRequired(ch, unsafeCharClassesForQEncoding))
        {
          num5 = (int) ch > (int) sbyte.MaxValue ? 6 : 3;
          num6 = (int) ch > (int) sbyte.MaxValue ? 2 : 1;
          if (MimeCommon.IsAnySurrogate(ch))
          {
            if (MimeCommon.IsHighSurrogate(ch) && index != value.Length && MimeCommon.IsLowSurrogate(value[index]))
            {
              ++index;
              ++num4;
            }
            else
            {
              num5 = 3;
              num6 = 1;
            }
          }
        }
        flag1 = flag1 || num3 + num6 > targetBytes;
        flag2 = flag2 || num2 + num5 > encodedWordSpace;
        if (!flag1)
        {
          num3 += num6;
          chunkSize1 += num4;
        }
        if (!flag2)
        {
          num2 += num5;
          num1 += num4;
        }
      }
      if (allowQEncoding && num1 >= chunkSize1 && num2 < num3 + 3)
      {
        chunkSize = num1;
        method = (byte) 81;
      }
      else
      {
        chunkSize = MimeCommon.AdjustChunkSize(encoding, chunkSize1, value, valueOffset, targetBytes);
        method = (byte) 66;
      }
    }

    private static void CalculateMethodAndChunkSize_Utf8(bool allowQEncoding, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClassesForQEncoding, Encoding encoding, string value, int valueOffset, int encodedWordSpace, out byte method, out int chunkSize)
    {
      int targetBytes = encodedWordSpace / 4 * 3;
      int num1 = 0;
      int num2 = 0;
      int chunkSize1 = 0;
      int num3 = 0;
      int index = valueOffset;
      bool flag1 = false;
      bool flag2 = false;
      while (index != value.Length && (!flag1 || !flag2))
      {
        int num4 = 1;
        int num5 = 1;
        int num6 = 1;
        char ch = value[index++];
        if (MimeCommon.QEncodingRequired(ch, unsafeCharClassesForQEncoding))
        {
          if ((int) ch > (int) sbyte.MaxValue)
          {
            ++num6;
            if ((int) ch > 2047)
            {
              ++num6;
              if (MimeCommon.IsAnySurrogate(ch) && MimeCommon.IsHighSurrogate(ch) && (index != value.Length && MimeCommon.IsLowSurrogate(value[index])))
              {
                ++index;
                ++num4;
                ++num6;
              }
            }
          }
          num5 = num6 * 3;
        }
        flag1 = flag1 || num3 + num6 > targetBytes;
        flag2 = flag2 || num2 + num5 > encodedWordSpace;
        if (!flag1)
        {
          num3 += num6;
          chunkSize1 += num4;
        }
        if (!flag2)
        {
          num2 += num5;
          num1 += num4;
        }
      }
      if (num1 >= chunkSize1 && num2 < num3 + 3)
      {
        chunkSize = num1;
        method = (byte) 81;
      }
      else
      {
        chunkSize = MimeCommon.AdjustChunkSize(encoding, chunkSize1, value, valueOffset, targetBytes);
        method = (byte) 66;
      }
    }

    private static void CalculateMethodAndChunkSize_Unicode16(bool allowQEncoding, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClassesForQEncoding, Encoding encoding, string value, int valueOffset, int encodedWordSpace, out byte method, out int chunkSize)
    {
      int targetBytes = encodedWordSpace / 4 * 3;
      chunkSize = Math.Min(targetBytes / 2, value.Length - valueOffset);
      if (chunkSize < value.Length - valueOffset && MimeCommon.IsLowSurrogate(value[valueOffset + chunkSize]) && MimeCommon.IsHighSurrogate(value[valueOffset + chunkSize - 1]))
        --chunkSize;
      chunkSize = MimeCommon.AdjustChunkSize(encoding, chunkSize, value, valueOffset, targetBytes);
      method = (byte) 66;
    }

    private static void CalculateMethodAndChunkSize_Unicode32(bool allowQEncoding, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClassesForQEncoding, Encoding encoding, string value, int valueOffset, int encodedWordSpace, out byte method, out int chunkSize)
    {
      int targetBytes = encodedWordSpace / 4 * 3;
      chunkSize = Math.Min(targetBytes / 4, value.Length - valueOffset);
      if (chunkSize < value.Length - valueOffset && MimeCommon.IsLowSurrogate(value[valueOffset + chunkSize]) && MimeCommon.IsHighSurrogate(value[valueOffset + chunkSize - 1]))
        --chunkSize;
      chunkSize = MimeCommon.AdjustChunkSize(encoding, chunkSize, value, valueOffset, targetBytes);
      method = (byte) 66;
    }

    private static void CalculateMethodAndChunkSize_Mbcs(bool allowQEncoding, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClassesForQEncoding, Encoding encoding, string value, int valueOffset, int encodedWordSpace, out byte method, out int chunkSize)
    {
      int num = encodedWordSpace / 4 * 3;
      chunkSize = Math.Min(num, value.Length - valueOffset);
      if (chunkSize < value.Length - valueOffset && MimeCommon.IsLowSurrogate(value[valueOffset + chunkSize]) && MimeCommon.IsHighSurrogate(value[valueOffset + chunkSize - 1]))
        --chunkSize;
      chunkSize = MimeCommon.AdjustChunkSize(encoding, chunkSize, value, valueOffset, num);
      method = (byte) 66;
    }

    private static int AdjustChunkSize(Encoding encoding, int chunkSize, string value, int valueOffset, int targetBytes)
    {
      int byteCount = MimeCommon.GetByteCount(encoding, value, valueOffset, chunkSize);
      if (byteCount > targetBytes)
      {
        int num1 = chunkSize * targetBytes / byteCount;
        if (num1 < chunkSize)
        {
          if (MimeCommon.IsLowSurrogate(value[valueOffset + num1]) && MimeCommon.IsHighSurrogate(value[valueOffset + num1 - 1]))
            --num1;
          chunkSize = num1;
          byteCount = MimeCommon.GetByteCount(encoding, value, valueOffset, chunkSize);
        }
        for (; byteCount > targetBytes; byteCount = MimeCommon.GetByteCount(encoding, value, valueOffset, chunkSize))
        {
          int num2 = chunkSize * targetBytes / byteCount;
          if (num2 < chunkSize)
            chunkSize = num2;
          else
            --chunkSize;
          if (MimeCommon.IsLowSurrogate(value[valueOffset + chunkSize]) && MimeCommon.IsHighSurrogate(value[valueOffset + chunkSize - 1]))
            --chunkSize;
        }
      }
      if (byteCount < targetBytes - 2 && byteCount != 0)
      {
        int num1 = 0;
        while (valueOffset + chunkSize < value.Length && num1++ < 3)
        {
          int num2 = 1;
          if (MimeCommon.IsHighSurrogate(value[valueOffset + chunkSize]) && valueOffset + chunkSize + 1 < value.Length && MimeCommon.IsLowSurrogate(value[valueOffset + chunkSize + 1]))
            ++num2;
          if (MimeCommon.GetByteCount(encoding, value, valueOffset, chunkSize + num2) <= targetBytes)
            chunkSize += num2;
          else
            break;
        }
      }
      return chunkSize;
    }

    private static bool QEncodingRequired(char ch, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClasses)
    {
      if ((int) ch < 128)
        return ~(Encoders.ByteEncoder.Tables.CharClasses.WSp | Encoders.ByteEncoder.Tables.CharClasses.QPEncode | Encoders.ByteEncoder.Tables.CharClasses.QPUnsafe | Encoders.ByteEncoder.Tables.CharClasses.QPWSp | Encoders.ByteEncoder.Tables.CharClasses.QEncode | Encoders.ByteEncoder.Tables.CharClasses.QPhraseUnsafe | Encoders.ByteEncoder.Tables.CharClasses.QCommentUnsafe | Encoders.ByteEncoder.Tables.CharClasses.Token2047) != (Encoders.ByteEncoder.Tables.CharacterTraits[(int) ch] & unsafeCharClasses);
      return true;
    }

    internal static LineTerminationState AdvanceLineTerminationState(LineTerminationState previousLineTerminationState, byte[] data, int offset, int count)
    {
      if (count == 0)
        return previousLineTerminationState;
      switch (data[offset + count - 1])
      {
        case (byte) 13:
          return LineTerminationState.CR;
        case (byte) 10:
          if (count >= 2)
            return (int) data[offset + count - 2] == 13 ? LineTerminationState.CRLF : LineTerminationState.Other;
          return previousLineTerminationState == LineTerminationState.CR ? LineTerminationState.CRLF : LineTerminationState.Other;
        default:
          return LineTerminationState.Other;
      }
    }

    private static unsafe int GetByteCount(Encoding encoding, string value, int offset, int size)
    {
      int byteCount;
      fixed (char* chPtr = value)
        byteCount = encoding.GetByteCount(chPtr + offset, size);
      return byteCount;
    }

    private delegate void CalculateMethodAndChunkSize(bool allowQEncoding, Encoders.ByteEncoder.Tables.CharClasses unsafeCharClassesForQEncoding, Encoding encoding, string value, int valueOffset, int encodedWordSpace, out byte method, out int chunkSize);
  }
}
