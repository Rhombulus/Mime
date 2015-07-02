// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.ContentDispositionHeader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class ContentDispositionHeader : ComplexHeader
  {
    internal const bool AllowUTF8Value = false;
    private const string DefaultDisposition = "attachment";
    private string disp;

    public override sealed string Value
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.disp;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof(value));
        if (ValueParser.ParseToken(value, 0, false) != value.Length)
          throw new ArgumentException("Value should be a valid token", nameof(value));
        if (!this.parsed)
          this.Parse();
        if (value == this.disp)
          return;
        this.SetRawValue((byte[]) null, true);
        this.parsed = true;
        this.disp = Header.NormalizeString(value);
      }
    }

    internal override byte[] RawValue
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return Internal.ByteString.StringToBytes(this.disp, false) ?? MimeString.EmptyByteArray;
      }
      set
      {
        base.RawValue = value;
      }
    }

    public ContentDispositionHeader()
      : base("Content-Disposition", HeaderId.ContentDisposition)
    {
      this.disp = "attachment";
      this.parsed = true;
    }

    public ContentDispositionHeader(string value)
      : base("Content-Disposition", HeaderId.ContentDisposition)
    {
      this.Value = value;
    }

    internal override void RawValueAboutToChange()
    {
      this.Reset();
    }

    public override sealed MimeNode Clone()
    {
      ContentDispositionHeader dispositionHeader = new ContentDispositionHeader();
      this.CopyTo((object) dispositionHeader);
      return (MimeNode) dispositionHeader;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      ContentDispositionHeader dispositionHeader = destination as ContentDispositionHeader;
      if (dispositionHeader == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType, nameof(destination));
      base.CopyTo(destination);
      dispositionHeader.parsed = this.parsed;
      dispositionHeader.disp = this.disp;
    }

    public override sealed bool IsValueValid(string value)
    {
      return value != null && ValueParser.ParseToken(value, 0, false) == value.Length;
    }

    internal override long WriteValue(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      if (this.disp.Length == 0)
        this.disp = "attachment";
      return base.WriteValue(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
    }

    internal override void ParseValue(ValueParser parser, bool storeValue)
    {
      MimeStringList phrase = new MimeStringList();
      parser.ParseCFWS(false, ref phrase, true);
      MimeString mimeString = parser.ParseToken();
      if (!storeValue)
        return;
      if (mimeString.Length == 0)
        this.disp = string.Empty;
      else
        this.disp = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
    }

    internal override void AppendLine(MimeString line, bool markDirty)
    {
      if (this.parsed)
        this.Reset();
      base.AppendLine(line, markDirty);
    }

    private void Reset()
    {
      this.InternalRemoveAll();
      this.parsed = false;
      this.disp = (string) null;
    }
  }
}
