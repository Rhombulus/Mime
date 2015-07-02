// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.ComplexHeader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public abstract class ComplexHeader : Header, IEnumerable<MimeParameter>, IEnumerable
  {
    protected bool handleISO2022 = true;
    protected bool parsed;

    public MimeParameter this[string name]
    {
      get
      {
        if (name == null)
          throw new ArgumentNullException(nameof(name));
        for (MimeNode mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
        {
          MimeParameter mimeParameter = mimeNode as MimeParameter;
          if (mimeParameter.IsName(name))
            return mimeParameter;
        }
        return (MimeParameter) null;
      }
    }

    internal ComplexHeader(string name, HeaderId headerId)
      : base(name, headerId)
    {
    }

    public MimeNode.Enumerator<MimeParameter> GetEnumerator()
    {
      return new MimeNode.Enumerator<MimeParameter>((MimeNode) this);
    }

    IEnumerator<MimeParameter> IEnumerable<MimeParameter>.GetEnumerator()
    {
      return (IEnumerator<MimeParameter>) new MimeNode.Enumerator<MimeParameter>((MimeNode) this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new MimeNode.Enumerator<MimeParameter>((MimeNode) this);
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      long num1 = this.WriteName(stream, ref scratchBuffer);
      currentLineLength.IncrementBy((int) num1);
      if (!this.IsDirty && this.RawLength != 0 && this.IsProtected)
      {
        long num2 = Header.WriteLines(this.Lines, stream);
        long num3 = num1 + num2;
        currentLineLength.SetAs(0);
        return num3;
      }
      if (!this.parsed)
        this.Parse();
      long num4 = num1 + this.WriteValue(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
      for (MimeNode mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
      {
        stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
        long num2 = num4 + (long) MimeString.Semicolon.Length;
        currentLineLength.IncrementBy(MimeString.Semicolon.Length);
        num4 = num2 + mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
      }
      return num4 + Header.WriteLineEnd(stream, ref currentLineLength);
    }

    internal virtual long WriteValue(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      long num1 = 0L;
      string str = this.Value;
      if (str != null)
      {
        int val2 = Internal.ByteString.StringToBytesCount(str, encodingOptions.AllowUTF8) + 1;
        if (scratchBuffer == null || scratchBuffer.Length < val2)
          scratchBuffer = new byte[Math.Max(998, val2)];
        scratchBuffer[0] = (byte) 32;
        int num2 = Internal.ByteString.StringToBytes(str, scratchBuffer, 1, encodingOptions.AllowUTF8);
        stream.Write(scratchBuffer, 0, num2 + 1);
        num1 += (long) (num2 + 1);
        currentLineLength.IncrementBy(str.Length + 1, num2 + 1);
      }
      return num1;
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      MimeParameter mimeParameter1 = newChild as MimeParameter;
      if (mimeParameter1 == null)
        throw new ArgumentException(CtsResources.Strings.NewChildNotMimeParameter, nameof(newChild));
      MimeParameter mimeParameter2 = this[mimeParameter1.Name];
      if (mimeParameter2 != null)
      {
        if (mimeParameter2 == refChild)
          refChild = mimeParameter2.PreviousSibling;
        this.InternalRemoveChild((MimeNode) mimeParameter2);
      }
      return refChild;
    }

    protected void Parse()
    {
      this.parsed = true;
      DecodingOptions headerDecodingOptions = this.GetHeaderDecodingOptions();
      ValueParser parser = new ValueParser(this.Lines, headerDecodingOptions.AllowUTF8);
      this.ParseValue(parser, true);
      this.ParseParameters(ref parser, headerDecodingOptions, int.MaxValue, int.MaxValue);
    }

    internal override void ForceParse()
    {
      if (this.parsed)
        return;
      this.Parse();
    }

    internal override void CheckChildrenLimit(int countLimit, int bytesLimit)
    {
      DecodingOptions headerDecodingOptions = this.GetHeaderDecodingOptions();
      ValueParser parser = new ValueParser(this.Lines, headerDecodingOptions.AllowUTF8);
      this.ParseValue(parser, false);
      this.ParseParameters(ref parser, headerDecodingOptions, countLimit, bytesLimit);
    }

    internal override void RemoveAllUnparsed()
    {
      this.parsed = true;
    }

    internal override MimeNode ParseNextChild()
    {
      if (this.parsed)
        return (MimeNode) null;
      MimeNode internalLastChild = this.InternalLastChild;
      this.Parse();
      if (internalLastChild != null)
        return internalLastChild.NextSibling;
      return this.FirstChild;
    }

    internal abstract void ParseValue(ValueParser parser, bool storeValue);

    internal void ParseParameters(ref ValueParser parser, DecodingOptions decodingOptions, int countLimit, int bytesLimit)
    {
      MimeStringList phrase = new MimeStringList();
      MimeStringList list = new MimeStringList();
      bool goodValue = false;
      int actual = 0;
      bool flag1 = DecodingFlags.None != (DecodingFlags.Rfc2231 & decodingOptions.DecodingFlags);
      byte num1;
      do
      {
        parser.ParseCFWS(false, ref phrase, this.handleISO2022);
        num1 = parser.ParseGet();
        if ((int) num1 != 59)
        {
          if ((int) num1 == 0)
            break;
          parser.ParseUnget();
          parser.ParseSkipToNextDelimiterByte((byte) 59);
        }
        else
        {
          parser.ParseCFWS(false, ref phrase, this.handleISO2022);
          MimeString mimeString = parser.ParseToken();
          if (mimeString.Length != 0 && mimeString.Length < 128)
          {
            parser.ParseCFWS(false, ref phrase, this.handleISO2022);
            num1 = parser.ParseGet();
            switch (num1)
            {
              case (byte) 61:
                parser.ParseCFWS(false, ref phrase, this.handleISO2022);
                parser.ParseParameterValue(ref list, ref goodValue, this.handleISO2022);
                if (int.MaxValue != countLimit || int.MaxValue != bytesLimit)
                {
                  if (++actual > countLimit)
                    throw new MimeException(CtsResources.Strings.TooManyParameters(actual, countLimit));
                  if (list.Length > bytesLimit)
                    throw new MimeException(CtsResources.Strings.TooManyTextValueBytes(list.Length, bytesLimit));
                  break;
                }
                bool flag2 = false;
                int num2 = -1;
                if (flag1)
                {
                  int num3 = Internal.ByteString.IndexOf(mimeString.Data, (byte) 42, mimeString.Offset, mimeString.Length);
                  if (num3 > 0)
                  {
                    int num4 = mimeString.Offset + mimeString.Length;
                    int index = num3 + 1;
                    num2 = 0;
                    for (; index != num4 && (int) mimeString.Data[index] >= 48 && (int) mimeString.Data[index] <= 57; ++index)
                    {
                      num2 = num2 * 10 + ((int) mimeString.Data[index] - 48);
                      if (10000 < num2)
                      {
                        num2 = -1;
                        break;
                      }
                    }
                    if (-1 != num2)
                    {
                      bool flag3 = 42 == (int) mimeString.Data[num4 - 1];
                      if (index < num4 - 1 || index < num4 && !flag3)
                      {
                        num2 = -1;
                      }
                      else
                      {
                        flag2 = flag3;
                        mimeString.TrimRight(num4 - num3);
                      }
                    }
                  }
                }
                string name = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
                MimeParameter mimeParameter1 = new MimeParameter(name);
                mimeParameter1.AppendValue(ref list);
                mimeParameter1.SegmentNumber = num2;
                mimeParameter1.ValueEncoded = flag2;
                MimeNode oldChild;
                MimeNode nextSibling1;
                for (oldChild = this.FirstChild; oldChild != null; oldChild = nextSibling1)
                {
                  nextSibling1 = oldChild.NextSibling;
                  MimeParameter mimeParameter2 = oldChild as MimeParameter;
                  if (mimeParameter2 != null && mimeParameter2.Name == name)
                    break;
                }
                if (0 >= num2)
                {
                  if (oldChild != null)
                  {
                    mimeParameter1.AllowAppend = true;
                    MimeNode nextSibling2;
                    for (MimeNode mimeNode = oldChild.FirstChild; mimeNode != null; mimeNode = nextSibling2)
                    {
                      nextSibling2 = mimeNode.NextSibling;
                      if (mimeNode is MimeParameter)
                      {
                        oldChild.RemoveChild(mimeNode);
                        mimeParameter1.InternalAppendChild(mimeNode);
                      }
                    }
                    mimeParameter1.AllowAppend = false;
                    this.InternalRemoveChild(oldChild);
                  }
                  this.InternalAppendChild((MimeNode) mimeParameter1);
                  break;
                }
                if (oldChild == null)
                {
                  MimeParameter mimeParameter2 = new MimeParameter(name);
                  mimeParameter2.SegmentNumber = 0;
                  this.InternalAppendChild((MimeNode) mimeParameter2);
                  oldChild = (MimeNode) mimeParameter2;
                }
                MimeNode refChild;
                MimeNode previousSibling;
                for (refChild = oldChild.LastChild; refChild != null; refChild = previousSibling)
                {
                  previousSibling = refChild.PreviousSibling;
                  MimeParameter mimeParameter2 = refChild as MimeParameter;
                  if (mimeParameter2 != null && mimeParameter2.SegmentNumber <= num2)
                    break;
                }
                MimeParameter mimeParameter3 = (MimeParameter) oldChild;
                mimeParameter3.AllowAppend = true;
                mimeParameter3.InternalInsertAfter((MimeNode) mimeParameter1, refChild);
                mimeParameter3.AllowAppend = false;
                break;
              case (byte) 0:
                return;
              default:
                parser.ParseUnget();
                break;
            }
          }
        }
      }
      while ((int) num1 != 0);
    }
  }
}
