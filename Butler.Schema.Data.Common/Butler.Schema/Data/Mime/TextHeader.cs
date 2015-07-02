// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.TextHeader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class TextHeader : Header
  {
    private string decodedValue;

    public override sealed string Value
    {
      get
      {
        if (this.decodedValue == null)
        {
          DecodingResults decodingResults;
          string decodedValue = this.GetDecodedValue(this.GetHeaderDecodingOptions(), out decodingResults);
          if (decodingResults.DecodingFailed)
            MimeCommon.ThrowDecodingFailedException(ref decodingResults);
          this.decodedValue = decodedValue;
        }
        return this.decodedValue;
      }
      set
      {
        this.SetRawValue((byte[]) null, true);
        this.decodedValue = value;
      }
    }

    internal override byte[] RawValue
    {
      get
      {
        MimeStringList mimeStringList = this.RawLength != 0 || this.decodedValue == null || this.decodedValue.Length == 0 ? this.Lines : this.GetEncodedValue(this.GetDocumentEncodingOptions(), ValueEncodingStyle.Normal);
        if (mimeStringList.Length == 0)
          return MimeString.EmptyByteArray;
        return mimeStringList.GetSz() ?? MimeString.EmptyByteArray;
      }
      set
      {
        base.RawValue = value;
      }
    }

    public TextHeader(string name, string value)
      : this(name, Header.GetHeaderId(name, true))
    {
      Type type = Header.TypeFromHeaderId(this.HeaderId);
      if (this.HeaderId != HeaderId.Unknown && type != typeof (TextHeader) && type != typeof (AsciiTextHeader))
        throw new ArgumentException(CtsResources.Strings.NameNotValidForThisHeaderType(name, "TextHeader", type.Name));
      this.Value = value;
    }

    internal TextHeader(string name, HeaderId headerId)
      : base(name, headerId)
    {
    }

    internal override void ForceParse()
    {
      string str = this.Value;
    }

    internal override void RawValueAboutToChange()
    {
      this.decodedValue = (string) null;
    }

    public override sealed MimeNode Clone()
    {
      TextHeader textHeader = new TextHeader(this.Name, this.HeaderId);
      this.CopyTo((object) textHeader);
      return (MimeNode) textHeader;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      TextHeader textHeader = destination as TextHeader;
      if (textHeader == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      base.CopyTo(destination);
      textHeader.decodedValue = this.decodedValue;
    }

    public override bool TryGetValue(out string value)
    {
      DecodingResults decodingResults;
      return this.TryGetValue(this.GetHeaderDecodingOptions(), out decodingResults, out value);
    }

    public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
    {
      value = this.GetDecodedValue(decodingOptions, out decodingResults);
      if (!decodingResults.DecodingFailed)
        return true;
      value = (string) null;
      return false;
    }

    internal string GetDecodedValue(DecodingOptions decodingOptions, out DecodingResults decodingResults)
    {
      string str1 = (string) null;
      if (this.Lines.Length == 0)
      {
        string str2 = this.decodedValue != null ? this.decodedValue : string.Empty;
        decodingResults = new DecodingResults();
        return str2;
      }
      if (decodingOptions.Charset == null)
        decodingOptions.Charset = this.GetDefaultHeaderDecodingCharset((MimeDocument) null, (MimeNode) null);
      if (!MimeCommon.TryDecodeValue(this.Lines, 4026531840U, decodingOptions, out decodingResults, out str1))
        str1 = (string) null;
      return str1;
    }

    internal MimeStringList GetEncodedValue(EncodingOptions encodingOptions, ValueEncodingStyle encodingStyle)
    {
      if (string.IsNullOrEmpty(this.decodedValue))
        return this.Lines;
      return MimeCommon.EncodeValue(this.decodedValue, encodingOptions, encodingStyle);
    }

    public override sealed bool IsValueValid(string value)
    {
      return true;
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      long nameLength = this.WriteName(stream, ref scratchBuffer);
      currentLineLength.IncrementBy((int) nameLength);
      MimeStringList mimeStringList;
      if (this.RawLength == 0 && this.decodedValue != null && this.decodedValue.Length != 0)
        mimeStringList = this.GetEncodedValue(encodingOptions, ValueEncodingStyle.Normal);
      else if ((EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != EncodingFlags.None)
      {
        this.ForceParse();
        mimeStringList = this.GetEncodedValue(encodingOptions, ValueEncodingStyle.Normal);
      }
      else
      {
        bool merge = false;
        if (!this.IsDirty && this.RawLength != 0)
        {
          if (this.IsProtected)
          {
            long num = nameLength + Header.WriteLines(this.Lines, stream);
            currentLineLength.SetAs(0);
            return num;
          }
          if (!this.IsHeaderLineTooLong(nameLength, out merge))
          {
            long num = nameLength + Header.WriteLines(this.Lines, stream);
            currentLineLength.SetAs(0);
            return num;
          }
        }
        mimeStringList = this.Lines;
        if (merge)
          mimeStringList = Header.MergeLines(mimeStringList);
      }
      return nameLength + Header.QuoteAndFold(stream, mimeStringList, 4026531840U, false, mimeStringList.Length > 0, encodingOptions.AllowUTF8, 0, ref currentLineLength, ref scratchBuffer) + Header.WriteLineEnd(stream, ref currentLineLength);
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      throw new NotSupportedException(CtsResources.Strings.ChildrenCannotBeAddedToTextHeader);
    }
  }
}
