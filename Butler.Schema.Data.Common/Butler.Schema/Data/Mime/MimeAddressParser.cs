// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeAddressParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal class MimeAddressParser
  {
    public const int MaxEmailName = 315;
    public const int MaxX400EmailName = 1604;
    public const int MaxDomainName = 255;
    public const int MaxSubDomainName = 63;
    public const int MaxInternetName = 571;
    public const int MaxX400InternetName = 1860;
    private bool parserInit;
    private bool ignoreComments;
    private bool useSquareBrackets;
    private ValueParser valueParser;
    private bool groupInProgress;

    public bool Initialized
    {
      get
      {
        return this.parserInit;
      }
    }

    public MimeAddressParser()
    {
    }

    public MimeAddressParser(MimeStringList lines, MimeAddressParser source)
    {
      this.parserInit = source.parserInit;
      this.ignoreComments = source.ignoreComments;
      this.useSquareBrackets = source.useSquareBrackets;
      this.valueParser = new ValueParser(lines, source.valueParser);
      this.groupInProgress = source.groupInProgress;
    }

    public static bool IsWellFormedAddress(string address, bool allowUTF8 = false)
    {
      int domainStart;
      return MimeAddressParser.IsValidSmtpAddress(address, false, out domainStart, allowUTF8);
    }

    public static bool IsValidSmtpAddress(string address, bool checkLength, out int domainStart, bool allowUTF8 = false)
    {
      if (string.IsNullOrEmpty(address))
      {
        domainStart = -1;
        return false;
      }
      if (checkLength && address.Length > 571 && (address.Length > 1860 || !MimeAddressParser.IsEncapsulatedX400Address(address)))
      {
        domainStart = -1;
        return false;
      }
      int offset = 0;
      MimeAddressParser.ValidationStage validationStage1 = MimeAddressParser.ValidationStage.BEGIN;
      while (offset < address.Length && validationStage1 != MimeAddressParser.ValidationStage.ERROR)
      {
        char ch = address[offset];
        ++offset;
        MimeAddressParser.ValidationStage validationStage2;
        switch (validationStage1)
        {
          case MimeAddressParser.ValidationStage.BEGIN:
            if ((int) ch < 128 && MimeScan.IsAtom((byte) ch) || (int) ch >= 128 && allowUTF8)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL;
              continue;
            }
            if ((int) ch == 92)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL_ESC;
              continue;
            }
            if ((int) ch == 34)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL_DQS;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.LOCAL:
            if ((int) ch == 64)
            {
              validationStage2 = MimeAddressParser.ValidationStage.DOMAIN;
              goto case 7;
            }
            else
            {
              if ((int) ch == 46)
              {
                validationStage1 = MimeAddressParser.ValidationStage.LOCAL_DOT;
                continue;
              }
              goto case 2;
            }
          case MimeAddressParser.ValidationStage.LOCAL_DOT:
            if ((int) ch < 128 && MimeScan.IsAtom((byte) ch) || (int) ch >= 128 && allowUTF8)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL;
              continue;
            }
            if ((int) ch == 92)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL_ESC;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.LOCAL_DQS:
            if ((int) ch == 34)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL_DQS_END;
              continue;
            }
            if ((int) ch == 92)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL_DQS_ESC;
              continue;
            }
            if ((int) ch < 128 && 13 != (int) ch && (10 != (int) ch && 92 != (int) ch) && 34 != (int) ch || (int) ch >= 128 && allowUTF8)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL_DQS;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.LOCAL_ESC:
            if ((int) ch < 128 || (int) ch >= 128 && allowUTF8)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.LOCAL_DQS_ESC:
            if ((int) ch < 128 || (int) ch >= 128 && allowUTF8)
            {
              validationStage1 = MimeAddressParser.ValidationStage.LOCAL_DQS;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.LOCAL_DQS_END:
            if ((int) ch == 64)
            {
              validationStage2 = MimeAddressParser.ValidationStage.DOMAIN;
              goto case 7;
            }
            else
              break;
          case MimeAddressParser.ValidationStage.DOMAIN:
            if (checkLength && offset - 1 > 315 && (offset - 1 > 1604 || !MimeAddressParser.IsEncapsulatedX400Address(address)))
            {
              domainStart = -1;
              return false;
            }
            if (MimeAddressParser.IsValidDomain(address, offset, checkLength, allowUTF8))
            {
              domainStart = offset;
              return true;
            }
            break;
        }
        validationStage1 = MimeAddressParser.ValidationStage.ERROR;
      }
      domainStart = -1;
      return false;
    }

    public static bool IsValidDomain(string address, int offset, bool checkLength, bool allowUTF8 = false)
    {
      if (string.IsNullOrEmpty(address) || checkLength && address.Length - offset > (int) byte.MaxValue)
        return false;
      MimeAddressParser.ValidationStage validationStage = MimeAddressParser.ValidationStage.DOMAIN;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      bool flag = false;
      int num4 = 0;
      while (offset < address.Length && validationStage != MimeAddressParser.ValidationStage.ERROR)
      {
        char ch = address[offset];
        ++offset;
        switch (validationStage)
        {
          case MimeAddressParser.ValidationStage.DOMAIN:
            if ((int) ch == 91)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL;
              continue;
            }
            if ((int) ch < 128 && MimeScan.IsAlphaOrDigit((byte) ch) || (int) ch >= 128 && allowUTF8 || ((int) ch == 45 || (int) ch == 95))
            {
              num4 = offset;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_SUB;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_SUB:
            if ((int) ch == 46)
            {
              if (checkLength && offset - num4 > 63)
                return false;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_DOT;
              continue;
            }
            if ((int) ch < 128 && MimeScan.IsAlphaOrDigit((byte) ch) || (int) ch >= 128 && allowUTF8 || ((int) ch == 45 || (int) ch == 95))
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_SUB;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_DOT:
            if ((int) ch < 128 && MimeScan.IsAlphaOrDigit((byte) ch) || (int) ch >= 128 && allowUTF8 || ((int) ch == 45 || (int) ch == 95))
            {
              num4 = offset;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_SUB;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL:
            if ((int) ch < 128 && MimeScan.IsDigit((byte) ch))
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV4;
              num1 = 1;
              num2 = 1;
              num3 = (int) ch - 48;
              continue;
            }
            if ((int) ch == 73 || (int) ch == 105)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_I;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV4:
            if ((int) ch < 128 && MimeScan.IsDigit((byte) ch) && num2 < 3)
            {
              ++num2;
              num3 = num3 * 10 + ((int) ch - 48);
              if (num3 <= (int) byte.MaxValue)
              {
                validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV4;
                continue;
              }
              break;
            }
            if ((int) ch == 46)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV4_DOT;
              continue;
            }
            if ((int) ch == 93 && num1 == 4)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_END;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV4_DOT:
            if ((int) ch < 128 && MimeScan.IsDigit((byte) ch) && num1 < 4)
            {
              ++num1;
              num2 = 1;
              num3 = (int) ch - 48;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV4;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_I:
            if ((int) ch == 80 || (int) ch == 112)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IP;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IP:
            if ((int) ch == 118 || (int) ch == 86)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV:
            if ((int) ch == 54)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6:
            if ((int) ch == 58)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_:
            if ((int) ch < 128 && MimeScan.IsHex((byte) ch))
            {
              flag = false;
              num1 = 1;
              num2 = 1;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
              continue;
            }
            if ((int) ch == 58)
            {
              num1 = 0;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_STARTCOLON;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_STARTCOLON:
            if ((int) ch == 58)
            {
              flag = true;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_COMP;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_GRP:
            if ((int) ch < 128 && MimeScan.IsHex((byte) ch) && num2 < 4)
            {
              ++num2;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
              continue;
            }
            if ((int) ch == 58)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_COLON;
              continue;
            }
            if ((int) ch == 93 && (!flag && num1 == 8 || flag && num1 <= 6))
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_END;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_V4GRP:
            if ((int) ch < 128 && MimeScan.IsDigit((byte) ch) && num2 < 3)
            {
              ++num2;
              num3 = num3 * 10 + ((int) ch - 48);
              validationStage = num3 <= (int) byte.MaxValue ? MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_V4GRP : MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
              continue;
            }
            if ((int) ch < 128 && MimeScan.IsHex((byte) ch) && num2 < 4)
            {
              ++num2;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
              continue;
            }
            if ((int) ch == 58)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_COLON;
              continue;
            }
            if ((int) ch == 46)
            {
              num1 = 1;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV4_DOT;
              continue;
            }
            if ((int) ch == 93 && flag && num1 <= 6)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_END;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_COLON:
            if ((int) ch < 128 && MimeScan.IsDigit((byte) ch) && (!flag && num1 == 6) || flag && num1 <= 4)
            {
              ++num1;
              num2 = 1;
              num3 = (int) ch - 48;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_V4GRP;
              continue;
            }
            if ((int) ch < 128 && MimeScan.IsHex((byte) ch) && (!flag && num1 < 8) || flag && num1 < 6)
            {
              ++num1;
              num2 = 1;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
              continue;
            }
            if ((int) ch == 58 && !flag && num1 <= 6)
            {
              flag = true;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_COMP;
              continue;
            }
            break;
          case MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_COMP:
            if ((int) ch < 128 && MimeScan.IsHex((byte) ch) && num1 < 6)
            {
              ++num1;
              num2 = 1;
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
              continue;
            }
            if ((int) ch == 93)
            {
              validationStage = MimeAddressParser.ValidationStage.DOMAIN_LITERAL_END;
              continue;
            }
            break;
        }
        validationStage = MimeAddressParser.ValidationStage.ERROR;
      }
      if (validationStage == MimeAddressParser.ValidationStage.DOMAIN_LITERAL_END)
        return true;
      if (validationStage != MimeAddressParser.ValidationStage.DOMAIN_SUB)
        return false;
      if (checkLength)
        return offset - num4 < 63;
      return true;
    }

    public void Initialize(MimeStringList lines, bool ignoreComments, bool useSquareBrackets, bool allowUTF8)
    {
      this.Reset();
      this.ignoreComments = ignoreComments;
      this.useSquareBrackets = useSquareBrackets;
      this.valueParser = new ValueParser(lines, allowUTF8);
      this.parserInit = true;
    }

    public void Reset()
    {
      this.groupInProgress = false;
      this.parserInit = false;
    }

    public AddressParserResult ParseNextMailbox(ref MimeStringList displayName, ref MimeStringList address)
    {
      AddressParserResult addressParserResult = this.groupInProgress ? AddressParserResult.GroupInProgress : AddressParserResult.Mailbox;
      MimeAddressParser.ParserStage parserStage = MimeAddressParser.ParserStage.BEGIN;
      MimeStringList mimeStringList1 = new MimeStringList();
      MimeStringList mimeStringList2 = new MimeStringList();
      MimeStringList mimeStringList3 = new MimeStringList();
      bool flag1 = true;
      bool flag2 = false;
      bool ignoreNextByte = false;
      if (!this.parserInit)
        throw new InvalidOperationException(CtsResources.Strings.AddressParserNotInitialized);
      while (true)
      {
        byte num1;
        do
        {
          do
          {
            do
            {
              do
              {
                if (this.valueParser.ParseToDelimiter(ignoreNextByte, !flag1 && flag2, ref mimeStringList1))
                {
                  flag2 = false;
                  ignoreNextByte = false;
                  flag1 = false;
                }
                num1 = this.valueParser.ParseGet();
                byte num2 = num1;
                if ((uint) num2 <= 34U)
                {
                  switch (num2)
                  {
                    case (byte) 0:
                      goto label_24;
                    case (byte) 9:
                    case (byte) 10:
                    case (byte) 13:
                    case (byte) 32:
                      this.valueParser.ParseWhitespace(false, ref mimeStringList3);
                      flag2 = true;
                      continue;
                    case (byte) 34:
                      this.valueParser.ParseUnget();
                      if (mimeStringList1.Length != 0 && !flag1)
                        this.valueParser.ParseAppendSpace(ref mimeStringList1);
                      else
                        flag1 = false;
                      this.valueParser.ParseQString(true, ref mimeStringList1, true);
                      flag2 = true;
                      continue;
                  }
                }
                else if ((uint) num2 <= 46U)
                {
                  switch (num2)
                  {
                    case (byte) 40:
                      if (mimeStringList2.Length != 0)
                        mimeStringList2.Reset();
                      this.valueParser.ParseUnget();
                      if (this.ignoreComments)
                      {
                        this.valueParser.ParseComment(true, false, ref mimeStringList2, true);
                        if (flag2)
                          mimeStringList1.AppendFragment(new MimeString(" "));
                        mimeStringList1.TakeOverAppend(ref mimeStringList2);
                      }
                      else
                        this.valueParser.ParseComment(true, true, ref mimeStringList2, true);
                      flag2 = true;
                      continue;
                    case (byte) 44:
                      goto label_24;
                    case (byte) 46:
                      this.valueParser.ParseAppendLastByte(ref mimeStringList1);
                      flag1 = true;
                      continue;
                  }
                }
                else
                {
                  switch (num2)
                  {
                    case (byte) 58:
                    case (byte) 59:
                    case (byte) 60:
                    case (byte) 62:
                    case (byte) 64:
                    case (byte) 91:
                    case (byte) 93:
                      goto label_24;
                  }
                }
                this.valueParser.ParseUnget();
                ignoreNextByte = true;
                continue;
label_24:
                switch (parserStage)
                {
                  case MimeAddressParser.ParserStage.BEGIN:
                    byte num3 = num1;
                    if ((uint) num3 <= 44U)
                    {
                      if ((int) num3 != 0)
                      {
                        if ((int) num3 != 44)
                          goto label_41;
                      }
                      else
                        goto label_40;
                    }
                    else
                    {
                      switch (num3)
                      {
                        case (byte) 58:
                          continue;
                        case (byte) 59:
                          this.groupInProgress = false;
                          addressParserResult = AddressParserResult.Mailbox;
                          break;
                        case (byte) 60:
                          goto label_38;
                        case (byte) 62:
                          goto label_39;
                        case (byte) 64:
                          goto label_35;
                        case (byte) 91:
                          goto label_36;
                        default:
                          goto label_41;
                      }
                    }
                    if (mimeStringList1.GetLength(4026531839U) != 0)
                    {
                      displayName.TakeOver(ref mimeStringList1);
                      goto label_89;
                    }
                    else
                    {
                      mimeStringList1.Reset();
                      continue;
                    }
                  case MimeAddressParser.ParserStage.ANGLEADDR:
                    goto label_42;
                  case MimeAddressParser.ParserStage.ADDRSPEC:
                    goto label_60;
                  case MimeAddressParser.ParserStage.ROUTEDOMAIN:
                    goto label_75;
                  case MimeAddressParser.ParserStage.END:
                    goto label_85;
                  default:
                    goto label_89;
                }
              }
              while (mimeStringList1.GetLength(4026531839U) == 0);
              displayName.TakeOver(ref mimeStringList1);
              this.groupInProgress = true;
              return AddressParserResult.GroupStart;
label_35:
              int length = mimeStringList1.Length;
              this.valueParser.ParseAppendLastByte(ref mimeStringList1);
              address.TakeOver(ref mimeStringList1);
              parserStage = MimeAddressParser.ParserStage.ADDRSPEC;
              continue;
label_36:
              if (!this.useSquareBrackets)
              {
                this.valueParser.ParseUnget();
                ignoreNextByte = true;
                continue;
              }
label_38:
              displayName.TakeOver(ref mimeStringList1);
              parserStage = MimeAddressParser.ParserStage.ANGLEADDR;
              continue;
label_39:
              mimeStringList1.Reset();
              continue;
label_40:
              displayName.TakeOver(ref mimeStringList1);
              goto label_89;
label_41:
              this.valueParser.ParseUnget();
              ignoreNextByte = true;
              continue;
label_42:
              byte num4 = num1;
              if ((uint) num4 <= 44U)
              {
                if ((int) num4 != 0)
                {
                  if ((int) num4 == 44)
                    goto label_57;
                  else
                    goto label_59;
                }
                else
                  goto label_58;
              }
              else
              {
                switch (num4)
                {
                  case (byte) 58:
                    goto label_56;
                  case (byte) 60:
                    continue;
                  case (byte) 62:
                    goto label_55;
                  case (byte) 64:
                    if (mimeStringList1.Length == 0)
                    {
                      parserStage = MimeAddressParser.ParserStage.ROUTEDOMAIN;
                      continue;
                    }
                    this.valueParser.ParseAppendLastByte(ref mimeStringList1);
                    address.TakeOver(ref mimeStringList1);
                    parserStage = MimeAddressParser.ParserStage.ADDRSPEC;
                    continue;
                  case (byte) 91:
                    if (!this.useSquareBrackets)
                    {
                      this.valueParser.ParseUnget();
                      ignoreNextByte = true;
                      continue;
                    }
                    goto case (byte) 60;
                  case (byte) 93:
                    goto label_53;
                  default:
                    goto label_59;
                }
              }
            }
            while (mimeStringList1.Length == 0);
            this.valueParser.ParseUnget();
            ignoreNextByte = true;
            continue;
label_53:
            if (!this.useSquareBrackets)
            {
              this.valueParser.ParseUnget();
              ignoreNextByte = true;
              continue;
            }
label_55:
            address.TakeOver(ref mimeStringList1);
            parserStage = address.Length != 0 || displayName.Length != 0 ? MimeAddressParser.ParserStage.END : MimeAddressParser.ParserStage.BEGIN;
            continue;
label_56:
            mimeStringList1.Reset();
            continue;
label_57:;
          }
          while (displayName.Length == 0 && mimeStringList1.Length == 0);
label_58:
          address.TakeOver(ref mimeStringList1);
          break;
label_59:
          this.valueParser.ParseUnget();
          ignoreNextByte = true;
          continue;
label_60:
          byte num5 = num1;
          if ((uint) num5 <= 44U)
          {
            if ((int) num5 != 0 && (int) num5 != 44)
              goto label_74;
          }
          else
          {
            switch (num5)
            {
              case (byte) 59:
                this.groupInProgress = false;
                break;
              case (byte) 60:
                if (displayName.Length == 0)
                {
                  displayName.TakeOverAppend(ref address);
                  parserStage = MimeAddressParser.ParserStage.ANGLEADDR;
                  continue;
                }
                this.valueParser.ParseUnget();
                ignoreNextByte = true;
                continue;
              case (byte) 62:
                address.TakeOverAppend(ref mimeStringList1);
                parserStage = MimeAddressParser.ParserStage.END;
                continue;
              case (byte) 91:
                if (mimeStringList1.Length == 0)
                {
                  this.valueParser.ParseUnget();
                  this.valueParser.ParseDomainLiteral(true, ref mimeStringList1);
                  address.TakeOverAppend(ref mimeStringList1);
                  parserStage = MimeAddressParser.ParserStage.END;
                  continue;
                }
                this.valueParser.ParseUnget();
                ignoreNextByte = true;
                continue;
              case (byte) 93:
                if (!this.useSquareBrackets)
                {
                  this.valueParser.ParseUnget();
                  ignoreNextByte = true;
                  continue;
                }
                goto case (byte) 62;
              default:
                goto label_74;
            }
          }
          address.TakeOverAppend(ref mimeStringList1);
        }
        while (address.Length == 0 && displayName.Length == 0 && (int) num1 != 0);
        break;
label_74:
        this.valueParser.ParseUnget();
        ignoreNextByte = true;
        continue;
label_75:
        byte num6 = num1;
        if ((uint) num6 <= 44U)
        {
          if ((int) num6 != 0)
          {
            if ((int) num6 != 44)
              goto label_84;
          }
          else
            break;
        }
        else
        {
          switch (num6)
          {
            case (byte) 58:
              break;
            case (byte) 62:
              mimeStringList1.Reset();
              parserStage = MimeAddressParser.ParserStage.END;
              continue;
            case (byte) 91:
              mimeStringList1.Reset();
              this.valueParser.ParseUnget();
              this.valueParser.ParseDomainLiteral(false, ref mimeStringList3);
              continue;
            case (byte) 93:
              if (!this.useSquareBrackets)
              {
                this.valueParser.ParseUnget();
                ignoreNextByte = true;
                continue;
              }
              goto case (byte) 62;
            default:
              goto label_84;
          }
        }
        mimeStringList1.Reset();
        parserStage = MimeAddressParser.ParserStage.ANGLEADDR;
        continue;
label_84:
        this.valueParser.ParseUnget();
        ignoreNextByte = true;
        continue;
label_85:
        mimeStringList1.Reset();
        switch (num1)
        {
          case (byte) 0:
          case (byte) 44:
            if (address.Length == 0 && displayName.Length == 0 && (int) num1 != 0)
            {
              parserStage = MimeAddressParser.ParserStage.BEGIN;
              continue;
            }
            goto label_89;
          case (byte) 59:
            this.groupInProgress = false;
            goto case (byte) 0;
          default:
            continue;
        }
      }
label_89:
      if (displayName.Length == 0 && mimeStringList2.Length != 0 && address.Length != 0)
        displayName.TakeOver(ref mimeStringList2);
      if (address.Length == 0 && displayName.Length == 0)
        addressParserResult = AddressParserResult.End;
      return addressParserResult;
    }

    private static bool IsEncapsulatedX400Address(string address)
    {
      return address.StartsWith("IMCEAX400-", StringComparison.OrdinalIgnoreCase);
    }

    private enum ParserStage
    {
      BEGIN,
      ANGLEADDR,
      ADDRSPEC,
      ROUTEDOMAIN,
      END,
    }

    private enum ValidationStage
    {
      BEGIN,
      LOCAL,
      LOCAL_DOT,
      LOCAL_DQS,
      LOCAL_ESC,
      LOCAL_DQS_ESC,
      LOCAL_DQS_END,
      DOMAIN,
      DOMAIN_SUB,
      DOMAIN_DOT,
      DOMAIN_LITERAL,
      DOMAIN_LITERAL_IPV4,
      DOMAIN_LITERAL_IPV4_DOT,
      DOMAIN_LITERAL_I,
      DOMAIN_LITERAL_IP,
      DOMAIN_LITERAL_IPV,
      DOMAIN_LITERAL_IPV6,
      DOMAIN_LITERAL_IPV6_,
      DOMAIN_LITERAL_IPV6_STARTCOLON,
      DOMAIN_LITERAL_IPV6_GRP,
      DOMAIN_LITERAL_IPV6_V4GRP,
      DOMAIN_LITERAL_IPV6_COLON,
      DOMAIN_LITERAL_IPV6_COMP,
      DOMAIN_LITERAL_END,
      ERROR,
    }
  }
}
