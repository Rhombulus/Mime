// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.AddressHeader
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
  public class AddressHeader : Header, IEnumerable<AddressItem>, IEnumerable
  {
    internal const bool AllowUTF8Value = true;
    private bool staticParsing;
    private bool parsed;
    private MimeAddressParser parser;

    public override sealed string Value
    {
      get
      {
        return (string) null;
      }
      set
      {
        throw new NotSupportedException(CtsResources.Strings.UnicodeMimeHeaderAddressNotSupported);
      }
    }

    public override sealed bool RequiresSMTPUTF8
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        for (MimeNode mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
        {
          AddressItem addressItem = mimeNode as AddressItem;
          if (addressItem != null && addressItem.RequiresSMTPUTF8)
            return true;
        }
        return false;
      }
    }

    internal override byte[] RawValue
    {
      get
      {
        return (byte[]) null;
      }
      set
      {
        base.RawValue = value;
      }
    }

    public AddressHeader(string name)
      : this(name, Header.GetHeaderId(name, true))
    {
      Type type = Header.TypeFromHeaderId(this.HeaderId);
      if (this.HeaderId != HeaderId.Unknown && type != typeof (AddressHeader))
        throw new ArgumentException(CtsResources.Strings.NameNotValidForThisHeaderType(name, "AddressHeader", type.Name));
    }

    internal AddressHeader(string name, HeaderId headerId)
      : base(name, headerId)
    {
    }

    internal override void RawValueAboutToChange()
    {
      this.parsed = true;
      this.InternalRemoveAll();
      if (this.parser != null)
        this.parser.Reset();
      this.parsed = false;
    }

    public override bool TryGetValue(out string value)
    {
      value = (string) null;
      return false;
    }

    public MimeNode.Enumerator<AddressItem> GetEnumerator()
    {
      return new MimeNode.Enumerator<AddressItem>((MimeNode) this);
    }

    IEnumerator<AddressItem> IEnumerable<AddressItem>.GetEnumerator()
    {
      return (IEnumerator<AddressItem>) new MimeNode.Enumerator<AddressItem>((MimeNode) this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new MimeNode.Enumerator<AddressItem>((MimeNode) this);
    }

    public override sealed MimeNode Clone()
    {
      AddressHeader addressHeader = new AddressHeader(this.Name, this.HeaderId);
      this.CopyTo((object) addressHeader);
      return (MimeNode) addressHeader;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      AddressHeader addressHeader = destination as AddressHeader;
      if (addressHeader == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      base.CopyTo(destination);
      addressHeader.parsed = this.parsed;
      addressHeader.parser = this.parser == null ? (MimeAddressParser) null : new MimeAddressParser(addressHeader.Lines, this.parser);
    }

    public static AddressHeader Parse(string name, string value, AddressParserFlags flags)
    {
      AddressHeader addressHeader = new AddressHeader(name);
      if (!string.IsNullOrEmpty(value))
      {
        byte[] data = Internal.ByteString.StringToBytes(value, true);
        addressHeader.parser = new MimeAddressParser();
        addressHeader.parser.Initialize(new MimeStringList(data, 0, data.Length), AddressParserFlags.None != (flags & AddressParserFlags.IgnoreComments), AddressParserFlags.None != (flags & AddressParserFlags.AllowSquareBrackets), true);
        addressHeader.staticParsing = true;
        addressHeader.Parse();
      }
      return addressHeader;
    }

    public override sealed bool IsValueValid(string value)
    {
      return false;
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      long nameLength = this.WriteName(stream, ref scratchBuffer);
      currentLineLength.IncrementBy((int) nameLength);
      if (!this.IsDirty && this.RawLength != 0)
      {
        if (this.IsProtected)
        {
          long num = nameLength + Header.WriteLines(this.Lines, stream);
          currentLineLength.SetAs(0);
          return num;
        }
        if (this.InternalLastChild == null)
        {
          bool merge = false;
          if (!this.IsHeaderLineTooLong(nameLength, out merge))
          {
            long num = nameLength + Header.WriteLines(this.Lines, stream);
            currentLineLength.SetAs(0);
            return num;
          }
        }
      }
      if (!this.parsed)
        this.Parse();
      MimeNode mimeNode = this.FirstChild;
      int num1 = 0;
      for (; mimeNode != null; mimeNode = mimeNode.NextSibling)
      {
        if (1 < ++num1)
        {
          stream.Write(MimeString.Comma, 0, MimeString.Comma.Length);
          nameLength += (long) MimeString.Comma.Length;
          currentLineLength.IncrementBy(MimeString.Comma.Length);
        }
        nameLength += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
      }
      return nameLength + Header.WriteLineEnd(stream, ref currentLineLength);
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
      MimeNode mimeNode;
      if (internalLastChild is MimeGroup)
      {
        do
          ;
        while (internalLastChild.ParseNextChild() != null);
        mimeNode = internalLastChild.InternalNextSibling;
      }
      else
        mimeNode = this.ParseNextMailBox(false);
      this.parsed = mimeNode == null;
      return mimeNode;
    }

    internal override void CheckChildrenLimit(int countLimit, int bytesLimit)
    {
      if (this.parser == null)
        this.parser = new MimeAddressParser();
      if (!this.parser.Initialized)
        this.parser.Initialize(this.Lines, false, false, this.GetHeaderDecodingOptions().AllowUTF8);
      int actual;
      for (actual = 0; actual <= countLimit; ++actual)
      {
        MimeStringList displayName = new MimeStringList();
        MimeStringList address = new MimeStringList();
        if (AddressParserResult.End == this.parser.ParseNextMailbox(ref displayName, ref address))
        {
          this.parser.Reset();
          return;
        }
        if (displayName.Length > bytesLimit)
          throw new MimeException(CtsResources.Strings.TooManyTextValueBytes(displayName.Length, bytesLimit));
      }
      throw new MimeException(CtsResources.Strings.TooManyAddressItems(actual, countLimit));
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      if (!(newChild is MimeRecipient) && !(newChild is MimeGroup))
        throw new ArgumentException(CtsResources.Strings.NewChildNotRecipientOrGroup, nameof(newChild));
      return refChild;
    }

    internal override void AppendLine(MimeString line, bool markDirty)
    {
      base.AppendLine(line, markDirty);
      this.parsed = false;
    }

    internal MimeNode ParseNextMailBox(bool fromGroup)
    {
      if (this.parsed)
        return (MimeNode) null;
      DecodingOptions headerDecodingOptions = this.GetHeaderDecodingOptions();
      if (this.parser == null)
        this.parser = new MimeAddressParser();
      if (!this.parser.Initialized)
        this.parser.Initialize(this.Lines, false, false, headerDecodingOptions.AllowUTF8);
      MimeStringList displayName = new MimeStringList();
      MimeStringList address = new MimeStringList();
      AddressParserResult addressParserResult = this.parser.ParseNextMailbox(ref displayName, ref address);
      switch (addressParserResult)
      {
        case AddressParserResult.Mailbox:
        case AddressParserResult.GroupInProgress:
          MimeRecipient mimeRecipient = new MimeRecipient(ref address, ref displayName);
          if (this.staticParsing)
            MimeRecipient.ConvertDisplayNameBack((AddressItem) mimeRecipient, displayName, headerDecodingOptions.AllowUTF8);
          if (addressParserResult == AddressParserResult.GroupInProgress)
          {
            MimeGroup mimeGroup = this.InternalLastChild as MimeGroup;
            mimeGroup.InternalInsertAfter((MimeNode) mimeRecipient, mimeGroup.InternalLastChild);
            return (MimeNode) mimeRecipient;
          }
          this.InternalInsertAfter((MimeNode) mimeRecipient, this.InternalLastChild);
          if (!fromGroup)
            return (MimeNode) mimeRecipient;
          return (MimeNode) null;
        case AddressParserResult.GroupStart:
          MimeGroup mimeGroup1 = new MimeGroup(ref displayName);
          if (this.staticParsing)
            MimeRecipient.ConvertDisplayNameBack((AddressItem) mimeGroup1, displayName, headerDecodingOptions.AllowUTF8);
          this.InternalInsertAfter((MimeNode) mimeGroup1, this.InternalLastChild);
          return (MimeNode) mimeGroup1;
        case AddressParserResult.End:
          return (MimeNode) null;
        default:
          return (MimeNode) null;
      }
    }

    private void Parse()
    {
      while (!this.parsed)
        this.ParseNextChild();
      if (!this.staticParsing)
        return;
      this.staticParsing = false;
    }

    internal override void ForceParse()
    {
      this.Parse();
    }
  }
}
