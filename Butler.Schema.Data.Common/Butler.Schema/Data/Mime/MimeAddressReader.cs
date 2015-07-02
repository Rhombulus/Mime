// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeAddressReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public struct MimeAddressReader
  {
    private MimeReader reader;
    private bool topLevel;

    public bool IsGroup
    {
      get
      {
        this.AssertGood(true);
        if (this.topLevel)
          return this.reader.GroupInProgress;
        return false;
      }
    }

    public MimeAddressReader GroupRecipientReader
    {
      get
      {
        if (!this.IsGroup)
          throw new InvalidOperationException(CtsResources.Strings.AddressReaderIsNotPositionedOnAGroup);
        return new MimeAddressReader(this.reader, false);
      }
    }

    public string DisplayName
    {
      get
      {
        DecodingResults decodingResults;
        string displayName;
        if (!this.TryGetDisplayName(this.reader.HeaderDecodingOptions, out decodingResults, out displayName))
          MimeCommon.ThrowDecodingFailedException(ref decodingResults);
        return displayName;
      }
    }

    public string Email
    {
      get
      {
        this.AssertGood(true);
        return this.reader.ReadRecipientEmail(this.topLevel);
      }
    }

    internal MimeAddressReader(MimeReader reader, bool topLevel)
    {
      this.reader = reader;
      this.topLevel = topLevel;
    }

    public bool ReadNextAddress()
    {
      this.AssertGood(false);
      return this.reader.ReadNextDescendant(this.topLevel);
    }

    public bool TryGetDisplayName(out string displayName)
    {
      DecodingResults decodingResults;
      return this.TryGetDisplayName(this.reader.HeaderDecodingOptions, out decodingResults, out displayName);
    }

    public bool TryGetDisplayName(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName)
    {
      this.AssertGood(true);
      return this.reader.TryReadDisplayName(this.topLevel, decodingOptions, out decodingResults, out displayName);
    }

    private void AssertGood(bool checkPositionedOnAddress)
    {
      if (this.reader == null)
        throw new NotSupportedException(CtsResources.Strings.AddressReaderNotInitialized);
      this.reader.AssertGoodToUse(true, true);
      if (this.reader.ReaderState != MimeReaderState.HeaderComplete || this.reader.CurrentHeaderObject == null || !(this.reader.CurrentHeaderObject is AddressHeader))
        throw new NotSupportedException(CtsResources.Strings.ReaderIsNotPositionedOnAddressHeader);
      if (!this.topLevel && !this.reader.GroupInProgress)
        throw new InvalidOperationException(CtsResources.Strings.AddressReaderIsNotPositionedOnAddress);
      if (checkPositionedOnAddress && !this.reader.IsCurrentChildValid(this.topLevel))
        throw new InvalidOperationException(CtsResources.Strings.AddressReaderIsNotPositionedOnAddress);
    }
  }
}
