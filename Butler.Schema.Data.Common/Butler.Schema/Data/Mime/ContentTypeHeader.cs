// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.ContentTypeHeader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class ContentTypeHeader : ComplexHeader
  {
    internal const bool AllowUTF8Value = false;
    internal const bool AllowUTF8Boundary = false;
    internal const bool AllowUTF8Charset = false;
    private string value;
    private string type;
    private string subType;

    public string MediaType
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.type;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof(value));
        if (ValueParser.ParseToken(value, 0, false) != value.Length)
          throw new ArgumentException("Value should be valid MIME token", nameof(value));
        if (!this.parsed)
          this.Parse();
        if (value == this.type)
          return;
        string str = this.subType;
        this.SetRawValue((byte[]) null, true);
        this.parsed = true;
        this.type = Header.NormalizeString(value);
        this.subType = str;
        this.value = ContentTypeHeader.ComposeContentTypeValue(this.type, this.subType);
        if (this.type != "multipart")
          return;
        this.EnsureBoundary();
      }
    }

    public string SubType
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.subType;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof(value));
        if (ValueParser.ParseToken(value, 0, false) != value.Length)
          throw new ArgumentException("Value should be valid MIME token", nameof(value));
        if (!this.parsed)
          this.Parse();
        if (value == this.subType)
          return;
        string str = this.type;
        this.SetRawValue((byte[]) null, true);
        this.parsed = true;
        this.type = str;
        this.subType = Header.NormalizeString(value);
        this.value = ContentTypeHeader.ComposeContentTypeValue(this.type, this.subType);
      }
    }

    public override sealed string Value
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.value;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof(value));
        int length = ValueParser.ParseToken(value, 0, false);
        if (length == 0 || length > value.Length - 2 || ((int) value[length] != 47 || ValueParser.ParseToken(value, length + 1, false) != value.Length - length - 1))
          throw new ArgumentException("Value should be a valid content type in the form 'token/token'", nameof(value));
        if (!this.parsed)
          this.Parse();
        if (value == this.value)
          return;
        this.SetRawValue((byte[]) null, true);
        this.parsed = true;
        this.value = Header.NormalizeString(value);
        this.type = Header.NormalizeString(this.value, 0, length);
        this.subType = Header.NormalizeString(this.value, length + 1, this.value.Length - length - 1);
        if (this.type != "multipart")
          return;
        this.EnsureBoundary();
      }
    }

    internal bool IsMultipart
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.type == "multipart";
      }
    }

    internal bool IsEmbeddedMessage
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.value == "message/rfc822";
      }
    }

    internal bool IsAnyMessage
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.type == "message";
      }
    }

    internal override byte[] RawValue
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return Internal.ByteString.StringToBytes(this.value, false) ?? MimeString.EmptyByteArray;
      }
      set
      {
        base.RawValue = value;
        this.Parse();
        if (this.type != "multipart")
          return;
        this.EnsureBoundary();
      }
    }

    public ContentTypeHeader()
      : base("Content-Type", HeaderId.ContentType)
    {
      this.value = "text/plain";
      this.type = "text";
      this.subType = "plain";
      this.parsed = true;
    }

    public ContentTypeHeader(string value)
      : base("Content-Type", HeaderId.ContentType)
    {
      this.Value = value;
    }

    public override sealed MimeNode Clone()
    {
      ContentTypeHeader contentTypeHeader = new ContentTypeHeader();
      this.CopyTo((object) contentTypeHeader);
      return (MimeNode) contentTypeHeader;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      ContentTypeHeader contentTypeHeader = destination as ContentTypeHeader;
      if (contentTypeHeader == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType, nameof(destination));
      base.CopyTo(destination);
      contentTypeHeader.type = this.type;
      contentTypeHeader.subType = this.subType;
      contentTypeHeader.value = this.value;
      contentTypeHeader.parsed = this.parsed;
    }

    public override sealed bool IsValueValid(string value)
    {
      if (value == null)
        return false;
      int index = ValueParser.ParseToken(value, 0, false);
      return index != 0 && index <= value.Length - 2 && ((int) value[index] == 47 && ValueParser.ParseToken(value, index + 1, false) == value.Length - index - 1);
    }

    internal static byte[] CreateBoundary()
    {
      string str = Guid.NewGuid().ToString();
      byte[] bytes = new byte[str.Length + 2];
      bytes[0] = (byte) 95;
      Internal.ByteString.StringToBytes(str, bytes, 1, false);
      bytes[1 + str.Length] = (byte) 95;
      return bytes;
    }

    internal override void RawValueAboutToChange()
    {
      this.Reset();
    }

    internal override void ParseValue(ValueParser parser, bool storeValue)
    {
      MimeStringList phrase = MimeStringList.Empty;
      parser.ParseCFWS(false, ref phrase, true);
      MimeString mimeString1 = parser.ParseToken();
      MimeString mimeString2 = MimeString.Empty;
      parser.ParseCFWS(false, ref phrase, true);
      switch (parser.ParseGet())
      {
        case (byte) 47:
          parser.ParseCFWS(false, ref phrase, true);
          mimeString2 = parser.ParseToken();
          goto case (byte) 0;
        case (byte) 0:
          if (storeValue)
          {
            this.type = mimeString1.Length != 0 ? Header.NormalizeString(mimeString1.Data, mimeString1.Offset, mimeString1.Length, false) : "text";
            if (mimeString2.Length == 0)
            {
              if (this.type == "multipart")
                this.subType = "mixed";
              else if (this.type == "text")
              {
                this.subType = "plain";
              }
              else
              {
                this.type = "application";
                this.subType = "octet-stream";
              }
            }
            else
              this.subType = Header.NormalizeString(mimeString2.Data, mimeString2.Offset, mimeString2.Length, false);
            this.value = ContentTypeHeader.ComposeContentTypeValue(this.type, this.subType);
          }
          if (this.type != "multipart")
            break;
          this.handleISO2022 = false;
          break;
        default:
          parser.ParseUnget();
          goto case (byte) 0;
      }
    }

    internal override void AppendLine(MimeString line, bool markDirty)
    {
      if (this.parsed)
        this.Reset();
      base.AppendLine(line, markDirty);
    }

    private static string ComposeContentTypeValue(string type, string subType)
    {
      int num = type.Length + 1 + subType.Length;
      if (num >= 2 && num <= 32)
      {
        int index1 = MimeData.HashValueFinish(MimeData.HashValueAdd(MimeData.HashValueAdd(MimeData.HashValueAdd(0, type), "/"), subType));
        int index2 = MimeData.valueHashTable[index1];
        if (index2 > 0)
        {
          do
          {
            string str = MimeData.values[index2].value;
            if (str.Length == num && str.StartsWith(type, StringComparison.OrdinalIgnoreCase) && ((int) str[type.Length] == 47 && str.EndsWith(subType, StringComparison.OrdinalIgnoreCase)))
              return str;
            ++index2;
          }
          while ((int) MimeData.values[index2].hash == index1);
        }
      }
      return type + "/" + subType;
    }

    private void EnsureBoundary()
    {
      if (this["boundary"] != null)
        return;
      MimeParameter mimeParameter = new MimeParameter("boundary");
      this.InternalAppendChild((MimeNode) mimeParameter);
      mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
    }

    private void Reset()
    {
      this.InternalRemoveAll();
      this.parsed = false;
      this.value = (string) null;
      this.type = (string) null;
      this.subType = (string) null;
    }
  }
}
