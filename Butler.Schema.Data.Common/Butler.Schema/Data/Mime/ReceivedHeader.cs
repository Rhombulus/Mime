// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.ReceivedHeader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class ReceivedHeader : Header
  {
    private static readonly byte[] ParamFrom = Internal.ByteString.StringToBytes("from", true);
    private static readonly byte[] ParamBy = Internal.ByteString.StringToBytes("by", true);
    private static readonly byte[] ParamVia = Internal.ByteString.StringToBytes("via", true);
    private static readonly byte[] ParamWith = Internal.ByteString.StringToBytes("with", true);
    private static readonly byte[] ParamId = Internal.ByteString.StringToBytes("id", true);
    private static readonly byte[] ParamFor = Internal.ByteString.StringToBytes("for", true);
    private static readonly byte[] ParamDate = Internal.ByteString.StringToBytes("date", true);
    private static readonly byte[] ParamFromTcpInfo = Internal.ByteString.StringToBytes("x-from-tcp-info", true);
    private static readonly byte[] ParamByTcpInfo = Internal.ByteString.StringToBytes("x-by-tcp-info", true);
    internal const bool AllowUTF8Value = true;
    private const uint ParamFromCRC = 2556329580U;
    private const uint ParamByCRC = 2115158205U;
    private const uint ParamViaCRC = 3740702146U;
    private const uint ParamWithCRC = 3117694226U;
    private const uint ParamIdCRC = 4276123055U;
    private const uint ParamForCRC = 271896810U;
    private string fromValue;
    private string fromTcpInfoValue;
    private string byValue;
    private string byTcpInfoValue;
    private string viaValue;
    private string withValue;
    private string idValue;
    private string forValue;
    private string dateValue;
    private bool parsed;

    public string From
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.fromValue;
      }
    }

    public string FromTcpInfo
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.fromTcpInfoValue;
      }
    }

    public string By
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.byValue;
      }
    }

    public string ByTcpInfo
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.byTcpInfoValue;
      }
    }

    public string Via
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.viaValue;
      }
    }

    public string With
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.withValue;
      }
    }

    public string Id
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.idValue;
      }
    }

    public string For
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.forValue;
      }
    }

    public string Date
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.dateValue;
      }
    }

    public override sealed string Value
    {
      get
      {
        return this.GetRawValue(true);
      }
      set
      {
        throw new NotSupportedException(CtsResources.Strings.UnicodeMimeHeaderReceivedNotSupported);
      }
    }

    public override sealed bool RequiresSMTPUTF8 => !MimeString.IsPureASCII(this.Lines);

      internal ReceivedHeader()
      : base("Received", HeaderId.Received)
    {
    }

    public ReceivedHeader(string from, string fromTcpInfo, string by, string byTcpInfo, string forMailbox, string with, string id, string via, string date)
      : base("Received", HeaderId.Received)
    {
      int num1 = -1;
      int num2 = Internal.ByteString.StringToBytesCount(from, true);
      int num3 = Internal.ByteString.StringToBytesCount(fromTcpInfo, true);
      int num4 = Internal.ByteString.StringToBytesCount(by, true);
      int num5 = Internal.ByteString.StringToBytesCount(byTcpInfo, true);
      int num6 = Internal.ByteString.StringToBytesCount(forMailbox, true);
      int num7 = Internal.ByteString.StringToBytesCount(with, true);
      int num8 = Internal.ByteString.StringToBytesCount(id, true);
      int num9 = Internal.ByteString.StringToBytesCount(via, true);
      int num10 = Internal.ByteString.StringToBytesCount(date, false);
      int num11 = num1 + (from != null ? num2 + ReceivedHeader.ParamFrom.Length + 2 : 0) + (fromTcpInfo != null ? num3 + (from == null ? ReceivedHeader.ParamFrom.Length + 1 : 0) + 3 : 0) + (by != null ? num4 + ReceivedHeader.ParamBy.Length + 2 : 0) + (byTcpInfo != null ? num5 + (by == null ? ReceivedHeader.ParamBy.Length + 1 : 0) + 3 : 0) + (forMailbox != null ? num6 + ReceivedHeader.ParamFor.Length + 2 : 0) + (with != null ? num7 + ReceivedHeader.ParamWith.Length + 2 : 0) + (id != null ? num8 + ReceivedHeader.ParamId.Length + 2 : 0) + (via != null ? num9 + ReceivedHeader.ParamVia.Length + 2 : 0);
      int length = num11 + (date != null ? num10 + (-1 == num11 ? 3 : 2) : 0);
      if (-1 == length)
        return;
      byte[] numArray = new byte[length];
      int offset = 0;
      this.AppendNameValue(ReceivedHeader.ParamFrom, from, numArray, ref offset);
      if (fromTcpInfo != null)
      {
        if (from == null)
          this.AppendName(ReceivedHeader.ParamFrom, numArray, ref offset);
        numArray[offset++] = (byte) 32;
        numArray[offset++] = (byte) 40;
        Internal.ByteString.StringToBytes(fromTcpInfo, numArray, offset, true);
        offset += num3;
        numArray[offset++] = (byte) 41;
      }
      this.AppendNameValue(ReceivedHeader.ParamBy, by, numArray, ref offset);
      if (byTcpInfo != null)
      {
        if (by == null)
          this.AppendName(ReceivedHeader.ParamBy, numArray, ref offset);
        numArray[offset++] = (byte) 32;
        numArray[offset++] = (byte) 40;
        Internal.ByteString.StringToBytes(byTcpInfo, numArray, offset, true);
        offset += num5;
        numArray[offset++] = (byte) 41;
      }
      this.AppendNameValue(ReceivedHeader.ParamFor, forMailbox, numArray, ref offset);
      this.AppendNameValue(ReceivedHeader.ParamWith, with, numArray, ref offset);
      this.AppendNameValue(ReceivedHeader.ParamId, id, numArray, ref offset);
      this.AppendNameValue(ReceivedHeader.ParamVia, via, numArray, ref offset);
      if (date != null)
      {
        Internal.ByteString.ValidateStringArgument(date, false);
        numArray[offset++] = (byte) 59;
        numArray[offset++] = (byte) 32;
        Internal.ByteString.StringToBytes(date, numArray, offset, false);
        offset += num10;
      }
      this.RawValue = numArray;
    }

    internal override void RawValueAboutToChange()
    {
      this.parsed = false;
    }

    public override sealed MimeNode Clone()
    {
      ReceivedHeader receivedHeader = new ReceivedHeader();
      this.CopyTo((object) receivedHeader);
      return (MimeNode) receivedHeader;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      ReceivedHeader receivedHeader = destination as ReceivedHeader;
      if (receivedHeader == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      base.CopyTo(destination);
      receivedHeader.parsed = this.parsed;
      receivedHeader.fromValue = this.fromValue;
      receivedHeader.fromTcpInfoValue = this.fromTcpInfoValue;
      receivedHeader.byValue = this.byValue;
      receivedHeader.byTcpInfoValue = this.byTcpInfoValue;
      receivedHeader.viaValue = this.viaValue;
      receivedHeader.withValue = this.withValue;
      receivedHeader.idValue = this.idValue;
      receivedHeader.forValue = this.forValue;
      receivedHeader.dateValue = this.dateValue;
    }

    public override sealed bool IsValueValid(string value)
    {
      return false;
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      throw new NotSupportedException(CtsResources.Strings.ChildrenCannotBeAddedToReceivedHeader);
    }

    private void AppendNameValue(byte[] name, string value, byte[] array, ref int offset)
    {
      if (value == null)
        return;
      this.AppendName(name, array, ref offset);
      array[offset++] = (byte) 32;
      int num = Internal.ByteString.StringToBytes(value, array, offset, true);
      offset += num;
    }

    private void AppendName(byte[] name, byte[] array, ref int offset)
    {
      if (offset != 0)
        array[offset++] = (byte) 32;
      Buffer.BlockCopy((Array) name, 0, (Array) array, offset, name.Length);
      offset += name.Length;
    }

    private void Reset()
    {
      this.parsed = false;
      this.fromValue = (string) null;
      this.fromTcpInfoValue = (string) null;
      this.byValue = (string) null;
      this.byTcpInfoValue = (string) null;
      this.viaValue = (string) null;
      this.withValue = (string) null;
      this.idValue = (string) null;
      this.forValue = (string) null;
      this.dateValue = (string) null;
    }

    private void Parse()
    {
      this.Reset();
      this.parsed = true;
      DecodingOptions headerDecodingOptions = this.GetHeaderDecodingOptions();
      ValueParser valueParser = new ValueParser(this.Lines, headerDecodingOptions.AllowUTF8);
      MimeStringList mimeStringList1 = new MimeStringList();
      MimeStringList mimeStringList2 = new MimeStringList();
      MimeString mimeString1 = new MimeString();
      MimeString mimeString2 = MimeString.Empty;
      ReceivedHeader.ParseState parseState1 = ReceivedHeader.ParseState.None;
      while (true)
      {
        valueParser.ParseWhitespace(true, ref mimeStringList1);
        byte num1 = valueParser.ParseGet();
        if ((int) num1 != 0)
        {
          if (59 == (int) num1)
            parseState1 = ReceivedHeader.ParseState.Date;
          else if (40 == (int) num1 && parseState1 == ReceivedHeader.ParseState.DomainValue)
          {
            parseState1 = ReceivedHeader.ParseState.DomainAddress;
          }
          else
          {
            valueParser.ParseUnget();
            mimeString1 = valueParser.ParseToken();
            if (mimeString1.Length == 0)
            {
              int num2 = (int) valueParser.ParseGet();
              mimeStringList2.TakeOverAppend(ref mimeStringList1);
              valueParser.ParseAppendLastByte(ref mimeStringList2);
              continue;
            }
            ReceivedHeader.ParseState parseState2 = this.StateFromToken(mimeString1);
            if (ReceivedHeader.ParseState.None != parseState2)
              parseState1 = parseState2;
          }
          switch (parseState1)
          {
            case ReceivedHeader.ParseState.Domain:
            case ReceivedHeader.ParseState.OptInfo:
              if (mimeString2.Length != 0)
                this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
              else
                mimeStringList2.Reset();
              mimeString2 = mimeString1;
              valueParser.ParseWhitespace(false, ref mimeStringList1);
              mimeStringList1.Reset();
              ++parseState1;
              continue;
            case ReceivedHeader.ParseState.DomainValue:
              mimeStringList2.TakeOverAppend(ref mimeStringList1);
              mimeStringList2.AppendFragment(mimeString1);
              continue;
            case ReceivedHeader.ParseState.DomainAddress:
              bool flag = mimeString2.CompareEqI(ReceivedHeader.ParamFrom);
              this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
              mimeStringList1.Reset();
              valueParser.ParseUnget();
              valueParser.ParseComment(true, false, ref mimeStringList2, true);
              byte[] sz1 = mimeStringList2.GetSz();
              string str = sz1 == null ? (string) null : Internal.ByteString.BytesToString(sz1, headerDecodingOptions.AllowUTF8);
              if (flag)
                this.fromTcpInfoValue = str;
              else
                this.byTcpInfoValue = str;
              mimeStringList2.Reset();
              parseState1 = ReceivedHeader.ParseState.None;
              continue;
            case ReceivedHeader.ParseState.OptInfoValue:
              mimeStringList2.TakeOverAppend(ref mimeStringList1);
              mimeStringList2.AppendFragment(mimeString1);
              continue;
            case ReceivedHeader.ParseState.Date:
              this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
              mimeStringList1.Reset();
              valueParser.ParseWhitespace(false, ref mimeStringList1);
              valueParser.ParseToEnd(ref mimeStringList2);
              byte[] sz2 = mimeStringList2.GetSz();
              this.dateValue = sz2 == null ? (string) null : Internal.ByteString.BytesToString(sz2, false);
              continue;
            case ReceivedHeader.ParseState.None:
              mimeStringList2.Reset();
              continue;
            default:
              continue;
          }
        }
        else
          break;
      }
      this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
    }

    internal override void ForceParse()
    {
      if (this.parsed)
        return;
      this.Parse();
    }

    private void FinishClause(ref MimeString param, ref MimeStringList value, bool allowUTF8)
    {
      if (param.Length == 0)
        return;
      byte[] sz = value.GetSz();
      string str = sz == null ? (string) null : Internal.ByteString.BytesToString(sz, allowUTF8);
      uint crcI = param.ComputeCrcI();
      if (crcI <= 2556329580U)
      {
        if ((int) crcI != 271896810)
        {
          if ((int) crcI != 2115158205)
          {
            if ((int) crcI == -1738637716)
              this.fromValue = str;
          }
          else
            this.byValue = str;
        }
        else
          this.forValue = str;
      }
      else if ((int) crcI != -1177273070)
      {
        if ((int) crcI != -554265150)
        {
          if ((int) crcI == -18844241)
            this.idValue = str;
        }
        else
          this.viaValue = str;
      }
      else
        this.withValue = str;
      value.Reset();
      param = MimeString.Empty;
    }

    private ReceivedHeader.ParseState StateFromToken(MimeString token)
    {
      uint crcI = token.ComputeCrcI();
      if (crcI <= 2556329580U)
      {
        if ((int) crcI == 271896810)
          return ReceivedHeader.ParseState.OptInfo;
        if ((int) crcI == 2115158205 || (int) crcI == -1738637716)
          return ReceivedHeader.ParseState.Domain;
      }
      else if ((int) crcI == -1177273070 || (int) crcI == -554265150 || (int) crcI == -18844241)
        return ReceivedHeader.ParseState.OptInfo;
      return ReceivedHeader.ParseState.None;
    }

    private enum ParseState
    {
      Domain,
      DomainValue,
      DomainAddress,
      OptInfo,
      OptInfoValue,
      Date,
      None,
    }
  }
}
