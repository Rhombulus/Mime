// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.AddressItem
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Mime
{
  public abstract class AddressItem : MimeNode
  {
    internal static readonly byte[] WordBreakBytes = Internal.ByteString.StringToBytes(" =?", true);
    private MimeStringList displayNameFragments;
    private string decodedDisplayName;

    public string DisplayName
    {
      get
      {
        DecodingResults decodingResults;
        if (this.decodedDisplayName == null && !this.TryGetDisplayName(this.GetHeaderDecodingOptions(), out decodingResults, out this.decodedDisplayName))
          MimeCommon.ThrowDecodingFailedException(ref decodingResults);
        return this.decodedDisplayName;
      }
      set
      {
        this.displayNameFragments.Reset();
        this.decodedDisplayName = value;
        this.SetDirty();
      }
    }

    public virtual bool RequiresSMTPUTF8 => false;

      internal string DecodedDisplayName
    {
      set
      {
        this.decodedDisplayName = value;
      }
    }

    internal AddressItem()
    {
    }

    internal AddressItem(string displayName)
    {
      this.decodedDisplayName = displayName;
    }

    internal AddressItem(ref MimeStringList displayName)
    {
      this.displayNameFragments.TakeOver(ref displayName);
    }

    public override void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      AddressItem addressItem = destination as AddressItem;
      if (addressItem == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      base.CopyTo(destination);
      addressItem.displayNameFragments = this.displayNameFragments.Clone();
      addressItem.decodedDisplayName = this.decodedDisplayName;
    }

    public bool TryGetDisplayName(out string displayName)
    {
      DecodingResults decodingResults;
      return this.TryGetDisplayName(this.GetHeaderDecodingOptions(), out decodingResults, out displayName);
    }

    public bool TryGetDisplayName(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName)
    {
      if (this.displayNameFragments.Count == 0)
      {
        displayName = this.decodedDisplayName;
        decodingResults = new DecodingResults();
        return true;
      }
      if (decodingOptions.Charset == null)
        decodingOptions.Charset = this.GetDefaultHeaderDecodingCharset((MimeDocument) null, (MimeNode) null);
      return MimeCommon.TryDecodeValue(this.displayNameFragments, 4026531839U, decodingOptions, out decodingResults, out displayName);
    }

    private bool IsQuotingRequired(string displayName, bool allowUTF8)
    {
      return this.IsQuotingRequired(new MimeString(displayName), allowUTF8);
    }

    private bool IsQuotingRequired(MimeString mimeStr, bool allowUTF8)
    {
      AddressItem.WriteState writeState = AddressItem.WriteState.Begin;
      MimeString mimeString = new MimeString(AddressItem.WordBreakBytes, 0, AddressItem.WordBreakBytes.Length);
      int offset;
      int count;
      byte[] data = mimeStr.GetData(out offset, out count);
      while (count != 0)
      {
        switch (writeState)
        {
          case AddressItem.WriteState.Begin:
            int characterCount = 0;
            int endOf = MimeScan.FindEndOf(MimeScan.Token.Atom, data, offset, count, out characterCount, allowUTF8);
            if (endOf == 0)
            {
              if (count <= 3 || offset != 0 || !mimeString.HasPrefixEq(data, 0, 3))
                return true;
              offset += 3;
              count -= 3;
              writeState = AddressItem.WriteState.Begin;
              continue;
            }
            offset += endOf;
            count -= endOf;
            writeState = AddressItem.WriteState.Atom;
            continue;
          case AddressItem.WriteState.Atom:
            if ((count < 2 || (int) data[offset] != 32) && (count < 1 || (int) data[offset] != 46))
              return true;
            ++offset;
            --count;
            writeState = AddressItem.WriteState.Begin;
            continue;
          default:
            continue;
        }
      }
      return false;
    }

    internal bool IsQuotingRequired(MimeStringList displayNameFragments, bool allowUTF8)
    {
      for (int index = 0; index != displayNameFragments.Count; ++index)
      {
        MimeString mimeStr = displayNameFragments[index];
        if (((int) mimeStr.Mask & -268435457) != 0 && this.IsQuotingRequired(mimeStr, allowUTF8))
          return true;
      }
      return false;
    }

    internal string QuoteString(string inputString)
    {
      StringBuilder stringBuilder = new StringBuilder(inputString.Length + 2);
      stringBuilder.Append("\"");
      for (int index = 0; index < inputString.Length; ++index)
      {
        char ch = inputString[index];
        if ((int) ch < 128 && MimeScan.IsEscapingRequired((byte) ch))
          stringBuilder.Append("\\");
        stringBuilder.Append(ch);
      }
      stringBuilder.Append("\"");
      return stringBuilder.ToString();
    }

    internal void ResetDisplayNameFragments()
    {
      this.displayNameFragments.Reset();
    }

    internal MimeStringList GetDisplayNameToWrite(EncodingOptions encodingOptions)
    {
      MimeStringList mimeStringList = this.displayNameFragments;
      if (mimeStringList.GetLength(4026531839U) == 0 && this.decodedDisplayName != null && this.decodedDisplayName.Length != 0)
      {
        mimeStringList = MimeCommon.EncodeValue((encodingOptions.EncodingFlags & EncodingFlags.QuoteDisplayNameBeforeRfc2047Encoding) == EncodingFlags.None || !this.IsQuotingRequired(this.decodedDisplayName, encodingOptions.AllowUTF8) || !MimeCommon.IsEncodingRequired(this.decodedDisplayName, encodingOptions.AllowUTF8) ? this.decodedDisplayName : this.QuoteString(this.decodedDisplayName), encodingOptions, ValueEncodingStyle.Phrase);
        this.displayNameFragments = mimeStringList;
      }
      else if ((EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != EncodingFlags.None)
        mimeStringList = MimeCommon.EncodeValue(this.DisplayName, encodingOptions, ValueEncodingStyle.Phrase);
      return mimeStringList;
    }

    private enum WriteState
    {
      Begin,
      Atom,
    }
  }
}
