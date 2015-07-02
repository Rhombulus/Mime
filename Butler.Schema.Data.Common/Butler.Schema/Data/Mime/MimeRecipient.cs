// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeRecipient
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class MimeRecipient : AddressItem
  {
    private MimeStringList emailAddressFragments;

    public string Email
    {
      get
      {
        byte[] sz = this.emailAddressFragments.GetSz();
        if (sz != null)
          return Internal.ByteString.BytesToString(sz, true);
        return string.Empty;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof(value));
        if (!MimeAddressParser.IsWellFormedAddress(value, true))
          throw new ArgumentException("Address string must be well-formed", nameof(value));
        this.emailAddressFragments.Reset();
        this.emailAddressFragments.Append(new MimeString(value));
        this.SetDirty();
      }
    }

    public override sealed bool RequiresSMTPUTF8
    {
      get
      {
        return !MimeString.IsPureASCII(this.emailAddressFragments);
      }
    }

    public MimeRecipient()
    {
    }

    public MimeRecipient(string displayName, string email)
      : base(displayName)
    {
      if (email == null)
        throw new ArgumentNullException(nameof(email));
      this.emailAddressFragments.Append(new MimeString(email));
    }

    internal MimeRecipient(ref MimeStringList address, ref MimeStringList displayName)
      : base(ref displayName)
    {
      this.emailAddressFragments.TakeOverAppend(ref address);
    }

    public static MimeRecipient Parse(string address, AddressParserFlags flags)
    {
      MimeRecipient mimeRecipient = new MimeRecipient();
      if (!string.IsNullOrEmpty(address))
      {
        byte[] data = Internal.ByteString.StringToBytes(address, true);
        MimeAddressParser mimeAddressParser = new MimeAddressParser();
        mimeAddressParser.Initialize(new MimeStringList(data, 0, data.Length), AddressParserFlags.None != (flags & AddressParserFlags.IgnoreComments), AddressParserFlags.None != (flags & AddressParserFlags.AllowSquareBrackets), true);
        MimeStringList displayName = new MimeStringList();
        int num = (int) mimeAddressParser.ParseNextMailbox(ref displayName, ref mimeRecipient.emailAddressFragments);
        MimeRecipient.ConvertDisplayNameBack((AddressItem) mimeRecipient, displayName, true);
      }
      return mimeRecipient;
    }

    public static bool IsEmailValid(string email, bool allowUTF8 = false)
    {
      return MimeAddressParser.IsWellFormedAddress(email, allowUTF8);
    }

    public override sealed MimeNode Clone()
    {
      MimeRecipient mimeRecipient = new MimeRecipient();
      this.CopyTo((object) mimeRecipient);
      return (MimeNode) mimeRecipient;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      MimeRecipient mimeRecipient = destination as MimeRecipient;
      if (mimeRecipient == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      base.CopyTo(destination);
      mimeRecipient.emailAddressFragments = this.emailAddressFragments.Clone();
    }

    internal static void ConvertDisplayNameBack(AddressItem addressItem, MimeStringList displayNameFragments, bool allowUTF8)
    {
      byte[] sz = displayNameFragments.GetSz(4026531839U);
      if (sz == null)
      {
        addressItem.DecodedDisplayName = (string) null;
      }
      else
      {
        string str = Internal.ByteString.BytesToString(sz, allowUTF8);
        addressItem.DecodedDisplayName = str;
      }
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      throw new NotSupportedException(CtsResources.Strings.RecipientsCannotHaveChildNodes);
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      MimeStringList displayNameToWrite = this.GetDisplayNameToWrite(encodingOptions);
      long num1 = 0L;
      int num2 = 0;
      if (this.NextSibling != null)
        ++num2;
      else if (this.Parent is MimeGroup)
      {
        ++num2;
        if (this.Parent.NextSibling != null)
          ++num2;
      }
      byte[] sz = this.emailAddressFragments.GetSz();
      int countInChars = Internal.ByteString.BytesToCharCount(sz, encodingOptions.AllowUTF8);
      if (displayNameToWrite.GetLength(4026531839U) != 0)
        num1 += Header.QuoteAndFold(stream, displayNameToWrite, 4026531839U, this.IsQuotingRequired(displayNameToWrite, encodingOptions.AllowUTF8), true, encodingOptions.AllowUTF8, countInChars == 0 ? num2 : 0, ref currentLineLength, ref scratchBuffer);
      if (countInChars != 0)
      {
        int num3 = 1 < currentLineLength.InChars ? 1 : 0;
        if (1 < currentLineLength.InChars)
        {
          if (currentLineLength.InChars + countInChars + 2 + num2 + num3 > 78)
          {
            long num4 = num1 + Header.WriteLineEnd(stream, ref currentLineLength);
            stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
            num1 = num4 + (long) Header.LineStartWhitespace.Length;
            currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
          }
          else
          {
            stream.Write(MimeString.Space, 0, MimeString.Space.Length);
            num1 += (long) MimeString.Space.Length;
            currentLineLength.IncrementBy(MimeString.Space.Length);
          }
        }
        stream.Write(MimeString.LessThan, 0, MimeString.LessThan.Length);
        long num5 = num1 + (long) MimeString.LessThan.Length;
        currentLineLength.IncrementBy(MimeString.LessThan.Length);
        stream.Write(sz, 0, sz.Length);
        long num6 = num5 + (long) sz.Length;
        currentLineLength.IncrementBy(countInChars, sz.Length);
        stream.Write(MimeString.GreaterThan, 0, MimeString.GreaterThan.Length);
        num1 = num6 + (long) MimeString.GreaterThan.Length;
        currentLineLength.IncrementBy(MimeString.GreaterThan.Length);
      }
      return num1;
    }
  }
}
