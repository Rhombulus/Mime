// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeParameter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Mime
{
  public class MimeParameter : MimeNode
  {
    private static readonly EncodingOptions EncodingOptionsAscii = new EncodingOptions(Globalization.Charset.ASCII);
    private static readonly byte[] OctetEncoderMap = new byte[16]
    {
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
      (byte) 65,
      (byte) 66,
      (byte) 67,
      (byte) 68,
      (byte) 69,
      (byte) 70
    };
    private static readonly char[] ControlCharacters = new char[5]
    {
      char.MinValue,
      '\r',
      '\n',
      '\f',
      '\v'
    };
    private int segmentNumber = -1;
    private MimeStringList valueFragments = new MimeStringList();
    internal const bool AllowUTF8Name = false;
    private const string DefaultLanguage = "en-us";
    private bool valueEncoded;
    private string paramName;
    private string decodedValue;
    private bool allowAppend;

    public string Name => this.paramName;

      public string Value
    {
      get
      {
        DecodingResults decodingResults;
        if (this.decodedValue == null && !this.TryGetValue(this.GetHeaderDecodingOptions(), out decodingResults, out this.decodedValue))
          MimeCommon.ThrowDecodingFailedException(ref decodingResults);
        return this.decodedValue;
      }
      set
      {
        if (this.segmentNumber == 0)
        {
          this.RemoveAll();
          this.segmentNumber = -1;
        }
        else if (0 < this.segmentNumber)
          throw new NotSupportedException(CtsResources.Strings.CantSetValueOfRfc2231ContinuationSegment);
        this.RawValue = (byte[]) null;
        this.decodedValue = value;
      }
    }

    internal byte[] RawValue
    {
      get
      {
        if (this.valueFragments.Length == 0 && this.decodedValue != null && 0 < this.decodedValue.Length)
          this.valueFragments = this.EncodeValue(this.decodedValue, this.paramName == "charset" ? MimeParameter.EncodingOptionsAscii : this.GetDocumentEncodingOptions());
        return this.valueFragments.GetSz(4026531839U);
      }
      set
      {
        if (this.segmentNumber == 0)
        {
          this.RemoveAll();
          this.segmentNumber = -1;
        }
        else if (0 < this.segmentNumber)
          throw new NotSupportedException(CtsResources.Strings.CantSetRawValueOfRfc2231ContinuationSegment);
        this.decodedValue = (string) null;
        this.valueFragments.Reset();
        this.valueEncoded = false;
        if (value != null && 0 < value.Length)
          this.valueFragments.AppendFragment(new MimeString(value));
        this.SetDirty();
      }
    }

    internal int RawLength => this.valueFragments.GetLength(4026531839U);

      internal bool ValueEncoded
    {
      set
      {
        this.valueEncoded = value;
      }
    }

    internal int SegmentNumber
    {
      get
      {
        return this.segmentNumber;
      }
      set
      {
        this.segmentNumber = value;
      }
    }

    internal bool AllowAppend
    {
      set
      {
        this.allowAppend = value;
      }
    }

    public MimeParameter(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      this.paramName = Header.NormalizeString(name);
    }

    public MimeParameter(string name, string value)
      : this(name)
    {
      this.decodedValue = value;
    }

    public override sealed MimeNode Clone()
    {
      MimeParameter mimeParameter = new MimeParameter(this.paramName);
      this.CopyTo((object) mimeParameter);
      return (MimeNode) mimeParameter;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      MimeParameter mimeParameter = destination as MimeParameter;
      if (mimeParameter == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      mimeParameter.AllowAppend = true;
      base.CopyTo(destination);
      mimeParameter.AllowAppend = false;
      mimeParameter.valueEncoded = this.valueEncoded;
      mimeParameter.segmentNumber = this.segmentNumber;
      mimeParameter.valueFragments = this.valueFragments.Clone();
      mimeParameter.decodedValue = this.decodedValue;
      mimeParameter.paramName = this.paramName;
    }

    public bool TryGetValue(out string value)
    {
      DecodingResults decodingResults;
      return this.TryGetValue(this.GetHeaderDecodingOptions(), out decodingResults, out value);
    }

    public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
    {
      if (decodingOptions.Charset == null)
        decodingOptions.Charset = this.GetDefaultHeaderDecodingCharset((MimeDocument) null, (MimeNode) null);
      if ((DecodingFlags.Rfc2231 & decodingOptions.DecodingFlags) != DecodingFlags.None)
      {
        if (this.segmentNumber == 0)
          return this.TryDecodeRfc2231(ref decodingOptions, out decodingResults, out value);
        if (0 < this.segmentNumber)
          throw new NotSupportedException(CtsResources.Strings.CantGetValueOfRfc2231ContinuationSegment);
      }
      if (this.valueFragments.Length != 0)
        return MimeCommon.TryDecodeValue(this.valueFragments, 4026531839U, decodingOptions, out decodingResults, out value);
      decodingResults = new DecodingResults();
      value = this.decodedValue != null ? this.decodedValue : string.Empty;
      return true;
    }

    internal static string CorrectValue(string value)
    {
      if (-1 == value.IndexOfAny(MimeParameter.ControlCharacters))
        return value;
      return MimeParameter.CorrectValue(value.ToCharArray());
    }

    internal static string CorrectValue(char[] value)
    {
      int length = MimeParameter.ControlCharacters.Length;
      for (int index1 = 0; index1 < value.Length; ++index1)
      {
        for (int index2 = 0; index2 < length; ++index2)
        {
          if ((int) value[index1] == (int) MimeParameter.ControlCharacters[index2])
          {
            value[index1] = ' ';
            break;
          }
        }
      }
      return new string(value);
    }

    internal bool FallBackIfRequired(byte[] bytes, DecodingOptions decodingOptions, out string value)
    {
      if ((DecodingFlags.FallbackToRaw & decodingOptions.DecodingFlags) != DecodingFlags.None)
      {
        if (bytes == null)
        {
          value = string.Empty;
        }
        else
        {
          value = Internal.ByteString.BytesToString(bytes, decodingOptions.AllowUTF8);
          if ((DecodingFlags.AllowControlCharacters & decodingOptions.DecodingFlags) == DecodingFlags.None)
            value = MimeParameter.CorrectValue(value);
        }
        return true;
      }
      value = (string) null;
      return false;
    }

    internal bool IsName(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      return this.paramName.Equals(name, StringComparison.OrdinalIgnoreCase);
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      MimeStringList fragments = this.valueFragments;
      long num1 = 0L;
      if (this.valueFragments.Length == 0 && this.decodedValue != null && 0 < this.decodedValue.Length)
      {
        fragments = this.EncodeValue(this.decodedValue, encodingOptions);
        this.valueFragments = fragments;
      }
      else if ((EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != EncodingFlags.None && 0 >= this.segmentNumber)
        fragments = this.EncodeValue(this.Value, encodingOptions);
      bool quoteOutput = false;
      if (this.IsQuotingReqired() || fragments.Length == 0)
      {
        quoteOutput = true;
      }
      else
      {
        for (int index = 0; index < fragments.Count; ++index)
        {
          MimeString str = fragments[index];
          int characterCount = 0;
          int num2 = ValueParser.ParseToken(str, out characterCount, encodingOptions.AllowUTF8);
          if (268435456 != (int) str.Mask && str.Length != num2)
          {
            quoteOutput = true;
            break;
          }
        }
      }
      MimeNode mimeNode = (MimeNode) null;
      if (this.segmentNumber == 0)
      {
        mimeNode = this.FirstChild;
        while (mimeNode != null && !(mimeNode is MimeParameter))
          mimeNode = mimeNode.NextSibling;
      }
      MimeString mimeString = this.segmentNumber == 0 && mimeNode != null || 0 < this.segmentNumber ? new MimeString(this.segmentNumber.ToString()) : new MimeString();
      if (1 < currentLineLength.InChars)
      {
        int num2 = 1 + this.paramName.Length + 1;
        int num3 = Internal.ByteString.BytesToCharCount(fragments.GetSz(), encodingOptions.AllowUTF8);
        if (mimeString.Length != 0)
          num2 += 1 + mimeString.Length;
        if (this.valueEncoded)
          ++num2;
        int num4 = num3;
        if (quoteOutput)
          num4 += 2;
        if (this.NextSibling != null)
        {
          if (num3 == 0)
            ++num2;
          else
            ++num4;
        }
        int num5 = num4 + num2;
        if (currentLineLength.InChars + num5 > 78)
        {
          long num6 = num1 + Header.WriteLineEnd(stream, ref currentLineLength);
          stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
          num1 = num6 + (long) Header.LineStartWhitespace.Length;
          currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
        }
        else
        {
          stream.Write(MimeString.Space, 0, MimeString.Space.Length);
          num1 += (long) MimeString.Space.Length;
          currentLineLength.IncrementBy(MimeString.Space.Length);
        }
      }
      int val2 = Internal.ByteString.StringToBytesCount(this.paramName, false);
      if (scratchBuffer == null || scratchBuffer.Length < val2)
        scratchBuffer = new byte[Math.Max(998, val2)];
      int num7 = Internal.ByteString.StringToBytes(this.paramName, scratchBuffer, 0, false);
      stream.Write(scratchBuffer, 0, num7);
      long num8 = num1 + (long) num7;
      currentLineLength.IncrementBy(this.paramName.Length, num7);
      if (mimeString.Length != 0)
      {
        stream.Write(MimeString.Asterisk, 0, MimeString.Asterisk.Length);
        long num2 = num8 + (long) MimeString.Asterisk.Length;
        currentLineLength.IncrementBy(MimeString.Asterisk.Length);
        mimeString.WriteTo(stream);
        num8 = num2 + (long) mimeString.Length;
        currentLineLength.IncrementBy(mimeString.Length);
      }
      if (this.valueEncoded)
      {
        stream.Write(MimeString.Asterisk, 0, MimeString.Asterisk.Length);
        num8 += (long) MimeString.Asterisk.Length;
        currentLineLength.IncrementBy(MimeString.Asterisk.Length);
      }
      stream.Write(MimeString.EqualTo, 0, MimeString.EqualTo.Length);
      long num9 = num8 + (long) MimeString.EqualTo.Length;
      currentLineLength.IncrementBy(MimeString.EqualTo.Length);
      int lastLineReserve = 0;
      if (this.NextSibling != null)
        ++lastLineReserve;
      long num10 = num9 + Header.QuoteAndFold(stream, fragments, 4026531839U, quoteOutput, false, encodingOptions.AllowUTF8, lastLineReserve, ref currentLineLength, ref scratchBuffer);
      int num11 = 0;
      for (; mimeNode != null; mimeNode = mimeNode.NextSibling)
      {
        MimeParameter mimeParameter = mimeNode as MimeParameter;
        if (mimeParameter != null)
        {
          ++num11;
          mimeParameter.segmentNumber = num11;
          stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
          long num2 = num10 + (long) MimeString.Semicolon.Length;
          currentLineLength.IncrementBy(MimeString.Semicolon.Length);
          long num3 = num2 + Header.WriteLineEnd(stream, ref currentLineLength);
          stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
          long num4 = num3 + (long) Header.LineStartWhitespace.Length;
          currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
          num10 = num4 + mimeNode.WriteTo(stream, encodingOptions, (MimeOutputFilter) null, ref currentLineLength, ref scratchBuffer);
        }
      }
      return num10;
    }

    internal override MimeNode ParseNextChild()
    {
      if (this.valueFragments.Length != 0 || this.decodedValue == null || this.decodedValue.Length == 0)
        return (MimeNode) null;
      if (this.InternalLastChild != null)
        return (MimeNode) null;
      this.valueFragments = this.EncodeValue(this.decodedValue, this.GetDocumentEncodingOptions());
      return this.FirstChild;
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      if (this.allowAppend)
        return refChild;
      throw new NotSupportedException(CtsResources.Strings.ParametersCannotHaveChildNodes);
    }

    internal void AppendValue(ref MimeStringList list)
    {
      this.valueFragments.TakeOverAppend(ref list);
    }

    private static byte DecodeHexadecimal(byte ch)
    {
      ch -= (byte) 48;
      if ((int) ch <= 9)
        return ch;
      ch -= (byte) 17;
      if ((int) ch <= 5)
        return (byte) ((uint) ch + 10U);
      ch -= (byte) 32;
      if ((int) ch <= 5)
        return (byte) ((uint) ch + 10U);
      return byte.MaxValue;
    }

    private bool IsQuotingReqired()
    {
      if (this.paramName != "boundary" && this.paramName != "filename" && (this.paramName != "name" && this.paramName != "id"))
        return this.paramName == "charset";
      return true;
    }

    private MimeStringList EncodeValue(string value, EncodingOptions encodingOptions)
    {
      this.valueFragments.Reset();
      int count = this.valueFragments.Count;
      if ((EncodingFlags.EnableRfc2231 & encodingOptions.EncodingFlags) == EncodingFlags.None)
        return MimeCommon.EncodeValue(value, encodingOptions, ValueEncodingStyle.Normal);
      this.EncodeRfc2231(encodingOptions);
      return this.valueFragments;
    }

    private void EncodeRfc2231(EncodingOptions encodingOptions)
    {
      if (!MimeCommon.IsEncodingRequired(this.decodedValue, encodingOptions.AllowUTF8))
      {
        this.valueEncoded = false;
        this.segmentNumber = -1;
        this.valueFragments.AppendFragment(new MimeString(this.decodedValue));
      }
      else
      {
        Globalization.Charset encodingCharset = encodingOptions.GetEncodingCharset();
        Encoding encoding = encodingCharset.GetEncoding();
        int bytesOffset1 = 0;
        int num1 = 0;
        string str = encodingOptions.CultureName == null ? string.Empty : encodingOptions.CultureName;
        if (encodingOptions.AllowUTF8 && encodingCharset.CodePage != 20127 && encodingCharset.CodePage != 65001 || (!encodingOptions.AllowUTF8 && encodingCharset.CodePage != 20127 || "en-us" != str))
        {
          string name = encodingCharset.Name;
          byte[] numArray1 = new byte[name.Length + str.Length + 2];
          int num2 = bytesOffset1 + Internal.ByteString.StringToBytes(name, numArray1, bytesOffset1, false);
          int num3 = num1 + name.Length;
          byte[] numArray2 = numArray1;
          int index1 = num2;
          int num4 = 1;
          int bytesOffset2 = index1 + num4;
          int num5 = 39;
          numArray2[index1] = (byte) num5;
          int num6 = num3 + 1;
          int num7 = bytesOffset2 + Internal.ByteString.StringToBytes(str, numArray1, bytesOffset2, false);
          int num8 = num6 + str.Length;
          byte[] numArray3 = numArray1;
          int index2 = num7;
          int num9 = 1;
          int count = index2 + num9;
          int num10 = 39;
          numArray3[index2] = (byte) num10;
          num1 = num8 + 1;
          this.valueFragments.AppendFragment(new MimeString(numArray1, 0, count));
          this.valueEncoded = true;
        }
        int num11 = 78 - this.paramName.Length - 6;
        int num12 = 2;
        byte[] bytes = encoding.GetBytes(this.decodedValue);
        int sourceIndex = this.EncodeRfc2231Segment(bytes, 0, num11 - num12 - num1, encodingOptions);
        this.segmentNumber = sourceIndex < bytes.Length ? 0 : -1;
        int num13 = 1;
        int num14 = 10;
        this.AllowAppend = true;
        while (sourceIndex < bytes.Length)
        {
          MimeParameter mimeParameter = new MimeParameter(this.paramName);
          if (num14 == num13)
          {
            ++num12;
            num14 *= 10;
          }
          sourceIndex = mimeParameter.EncodeRfc2231Segment(bytes, sourceIndex, num11 - num12, encodingOptions);
          mimeParameter.segmentNumber = num13++;
          this.InternalAppendChild((MimeNode) mimeParameter);
          if (10000 == num13)
            break;
        }
        this.AllowAppend = false;
      }
    }

    private int EncodeRfc2231Segment(byte[] source, int sourceIndex, int maxValueLength, EncodingOptions encodingOptions)
    {
      byte[] data = new byte[maxValueLength * 4];
      int count = 0;
      int num1 = 0;
      while (sourceIndex < source.Length)
      {
        int bytesUsed = 1;
        byte ch = source[sourceIndex];
        bool flag = true;
        if ((int) ch >= 128)
        {
          if (encodingOptions.AllowUTF8 && MimeScan.IsUTF8NonASCII(source, sourceIndex, source.Length, out bytesUsed))
            flag = false;
        }
        else if (!MimeScan.IsSegmentEncodingRequired(ch))
          flag = false;
        if (flag)
        {
          if (num1 + 3 <= maxValueLength)
          {
            byte[] numArray1 = data;
            int index1 = count;
            int num2 = 1;
            int num3 = index1 + num2;
            int num4 = 37;
            numArray1[index1] = (byte) num4;
            byte[] numArray2 = data;
            int index2 = num3;
            int num5 = 1;
            int num6 = index2 + num5;
            int num7 = (int) MimeParameter.OctetEncoderMap[(int) ch >> 4];
            numArray2[index2] = (byte) num7;
            byte[] numArray3 = data;
            int index3 = num6;
            int num8 = 1;
            count = index3 + num8;
            int num9 = (int) MimeParameter.OctetEncoderMap[(int) ch & 15];
            numArray3[index3] = (byte) num9;
            num1 += 3;
            this.valueEncoded = true;
            ++sourceIndex;
          }
          else
            break;
        }
        else if (num1 + 1 <= maxValueLength)
        {
          for (; bytesUsed > 0; --bytesUsed)
            data[count++] = source[sourceIndex++];
          ++num1;
        }
        else
          break;
      }
      this.valueFragments.AppendFragment(new MimeString(data, 0, count));
      return sourceIndex;
    }

    private bool TryDecodeRfc2231(ref DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
    {
      decodingResults = new DecodingResults();
      decodingResults.EncodingScheme = EncodingScheme.Rfc2231;
      Globalization.Charset charset = (Globalization.Charset) null;
      byte[] sz = this.valueFragments.GetSz(4026531839U);
      int sourceIndex = 0;
      if (this.valueEncoded)
      {
        int num1 = sz == null ? -1 : Internal.ByteString.IndexOf(sz, (byte) 39, 0, sz.Length);
        if (-1 < num1 && num1 < sz.Length - 1)
        {
          int offset;
          int num2 = Internal.ByteString.IndexOf(sz, (byte) 39, offset = num1 + 1, sz.Length - offset);
          if (-1 < num2)
          {
            decodingResults.CharsetName = Internal.ByteString.BytesToString(sz, 0, offset - 1, false);
            decodingResults.CultureName = Internal.ByteString.BytesToString(sz, offset, num2 - offset, false);
            if (!Globalization.Charset.TryGetCharset(decodingResults.CharsetName, out charset))
            {
              decodingResults.DecodingFailed = true;
              return this.FallBackIfRequired(sz, decodingOptions, out value);
            }
            sourceIndex = num2 + 1;
          }
        }
      }
      if (charset == null)
        charset = decodingOptions.Charset ?? DecodingOptions.DefaultCharset;
      decodingResults.CharsetName = charset.Name;
      Encoding encoding;
      if (!charset.TryGetEncoding(out encoding))
      {
        decodingResults.DecodingFailed = true;
        return this.FallBackIfRequired(sz, decodingOptions, out value);
      }
      int length = this.valueFragments.Length - sourceIndex;
      for (MimeNode mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
      {
        MimeParameter mimeParameter = mimeNode as MimeParameter;
        if (mimeParameter != null)
          length += mimeParameter.RawLength;
      }
      byte[] numArray = new byte[length];
      int num = 0;
      if (sz != null && sourceIndex < sz.Length)
        num += this.DecodeRfc2231Octets(this.valueEncoded, sz, sourceIndex, numArray, 0);
      for (MimeNode mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
      {
        MimeParameter mimeParameter = mimeNode as MimeParameter;
        if (mimeParameter != null && mimeParameter.RawValue != null)
          num += this.DecodeRfc2231Octets(mimeParameter.valueEncoded, mimeParameter.RawValue, 0, numArray, num);
      }
      value = num != 0 ? encoding.GetString(numArray, 0, num) : string.Empty;
      if ((DecodingFlags.AllowControlCharacters & decodingOptions.DecodingFlags) == DecodingFlags.None)
        value = MimeParameter.CorrectValue(value);
      return true;
    }

    private int DecodeRfc2231Octets(bool decode, byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
    {
      if (decode)
      {
        int num1 = destinationIndex;
        while (sourceIndex < source.Length)
        {
          if (37 == (int) source[sourceIndex] && sourceIndex + 2 < source.Length)
          {
            byte num2 = MimeParameter.DecodeHexadecimal(source[sourceIndex + 1]);
            if ((int) byte.MaxValue != (int) num2)
            {
              byte num3 = MimeParameter.DecodeHexadecimal(source[sourceIndex + 2]);
              if ((int) byte.MaxValue != (int) num3)
              {
                sourceIndex += 3;
                destination[destinationIndex++] = (byte) (((uint) num2 << 4) + (uint) num3);
                continue;
              }
            }
          }
          destination[destinationIndex++] = source[sourceIndex++];
        }
        return destinationIndex - num1;
      }
      int count = source.Length - sourceIndex;
      Buffer.BlockCopy((Array) source, sourceIndex, (Array) destination, destinationIndex, count);
      return count;
    }
  }
}
