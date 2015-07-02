// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeGroup
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
  public class MimeGroup : AddressItem, IEnumerable<MimeRecipient>, IEnumerable
  {
    private bool parsed;

    public MimeGroup()
    {
    }

    public MimeGroup(string displayName)
      : base(displayName)
    {
    }

    internal MimeGroup(ref MimeStringList displayName)
      : base(ref displayName)
    {
    }

    public MimeNode.Enumerator<MimeRecipient> GetEnumerator()
    {
      return new MimeNode.Enumerator<MimeRecipient>((MimeNode) this);
    }

    IEnumerator<MimeRecipient> IEnumerable<MimeRecipient>.GetEnumerator()
    {
      return (IEnumerator<MimeRecipient>) new MimeNode.Enumerator<MimeRecipient>((MimeNode) this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new MimeNode.Enumerator<MimeRecipient>((MimeNode) this);
    }

    public override sealed MimeNode Clone()
    {
      MimeGroup mimeGroup = new MimeGroup();
      this.CopyTo((object) mimeGroup);
      return (MimeNode) mimeGroup;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      MimeGroup mimeGroup = destination as MimeGroup;
      if (mimeGroup == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      do
        ;
      while (mimeGroup.ParseNextChild() != null);
      do
        ;
      while (this.ParseNextChild() != null);
      base.CopyTo(destination);
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      if (!(newChild is MimeRecipient))
        throw new ArgumentException(CtsResources.Strings.NewChildIsNotARecipient);
      return refChild;
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      MimeNode nextSibling = this.NextSibling;
      MimeStringList displayNameToWrite = this.GetDisplayNameToWrite(encodingOptions);
      long num1 = 0L;
      if (displayNameToWrite.GetLength(4026531839U) != 0)
      {
        int lastLineReserve = 1;
        if (this.FirstChild == null)
          ++lastLineReserve;
        if (this.NextSibling != null)
          ++lastLineReserve;
        long num2 = num1 + Header.QuoteAndFold(stream, displayNameToWrite, 4026531839U, this.IsQuotingRequired(displayNameToWrite, encodingOptions.AllowUTF8), true, encodingOptions.AllowUTF8, lastLineReserve, ref currentLineLength, ref scratchBuffer);
        stream.Write(MimeString.Colon, 0, MimeString.Colon.Length);
        num1 = num2 + (long) MimeString.Colon.Length;
        currentLineLength.IncrementBy(MimeString.Colon.Length);
      }
      MimeNode mimeNode = this.FirstChild;
      int num3 = 0;
      for (; mimeNode != null; mimeNode = mimeNode.NextSibling)
      {
        if (1 < ++num3)
        {
          stream.Write(MimeString.Comma, 0, MimeString.Comma.Length);
          num1 += (long) MimeString.Comma.Length;
          currentLineLength.IncrementBy(MimeString.Comma.Length);
        }
        num1 += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
      }
      stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
      long num4 = num1 + (long) MimeString.Semicolon.Length;
      currentLineLength.IncrementBy(MimeString.Semicolon.Length);
      return num4;
    }

    internal override MimeNode ParseNextChild()
    {
      MimeNode mimeNode = (MimeNode) null;
      if (!this.parsed && this.Parent != null)
      {
        AddressHeader addressHeader = this.Parent as AddressHeader;
        if (addressHeader != null)
          mimeNode = addressHeader.ParseNextMailBox(true);
      }
      this.parsed = mimeNode == null;
      return mimeNode;
    }
  }
}
