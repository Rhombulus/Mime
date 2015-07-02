// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeParameterReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public struct MimeParameterReader
  {
    private MimeReader reader;

    public string Name
    {
      get
      {
        this.AssertGood(true);
        return this.reader.ReadParameterName();
      }
    }

    public string Value
    {
      get
      {
        DecodingResults decodingResults;
        string str;
        if (!this.TryGetValue(this.reader.HeaderDecodingOptions, out decodingResults, out str))
          MimeCommon.ThrowDecodingFailedException(ref decodingResults);
        return str;
      }
    }

    internal MimeParameterReader(MimeReader reader)
    {
      this.reader = reader;
    }

    public bool ReadNextParameter()
    {
      this.AssertGood(false);
      return this.reader.ReadNextDescendant(true);
    }

    public bool TryGetValue(out string value)
    {
      DecodingResults decodingResults;
      return this.TryGetValue(this.reader.HeaderDecodingOptions, out decodingResults, out value);
    }

    public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
    {
      this.AssertGood(true);
      return this.reader.TryReadParameterValue(decodingOptions, out decodingResults, out value);
    }

    private void AssertGood(bool checkPositionedOnParameter)
    {
      if (this.reader == null)
        throw new NotSupportedException(CtsResources.Strings.ParameterReaderNotInitialized);
      this.reader.AssertGoodToUse(true, true);
      if (this.reader.ReaderState != MimeReaderState.HeaderComplete || this.reader.CurrentHeaderObject == null || !(this.reader.CurrentHeaderObject is ComplexHeader))
        throw new NotSupportedException(CtsResources.Strings.ReaderIsNotPositionedOnHeaderWithParameters);
      if (checkPositionedOnParameter && !this.reader.IsCurrentChildValid(true))
        throw new InvalidOperationException(CtsResources.Strings.ParameterReaderIsNotPositionedOnParameter);
    }
  }
}
