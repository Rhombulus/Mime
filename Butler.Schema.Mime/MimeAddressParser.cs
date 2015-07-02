using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal class MimeAddressParser {

        public MimeAddressParser() {}

        public MimeAddressParser(MimeStringList lines, MimeAddressParser source) {
            this.Initialized = source.Initialized;
            ignoreComments = source.ignoreComments;
            useSquareBrackets = source.useSquareBrackets;
            valueParser = new ValueParser(lines, source.valueParser);
            groupInProgress = source.groupInProgress;
        }

        public bool Initialized { get; private set; }

        public static bool IsWellFormedAddress(string address, bool allowUTF8 = false) {
            int domainStart;
            return MimeAddressParser.IsValidSmtpAddress(address, false, out domainStart, allowUTF8);
        }

        public static bool IsValidSmtpAddress(string address, bool checkLength, out int domainStart, bool allowUTF8 = false) {
            if (string.IsNullOrEmpty(address)) {
                domainStart = -1;
                return false;
            }
            if (checkLength && address.Length > 571 && (address.Length > 1860 || !MimeAddressParser.IsEncapsulatedX400Address(address))) {
                domainStart = -1;
                return false;
            }
            var offset = 0;
            var validationStage1 = ValidationStage.BEGIN;
            while (offset < address.Length && validationStage1 != ValidationStage.ERROR) {
                var ch = address[offset];
                ++offset;
                ValidationStage validationStage2;
                switch (validationStage1) {
                    case ValidationStage.BEGIN:
                        if (ch < 128 && MimeScan.IsAtom((byte) ch) || ch >= 128 && allowUTF8) {
                            validationStage1 = ValidationStage.LOCAL;
                            continue;
                        }
                        if (ch == 92) {
                            validationStage1 = ValidationStage.LOCAL_ESC;
                            continue;
                        }
                        if (ch == 34) {
                            validationStage1 = ValidationStage.LOCAL_DQS;
                            continue;
                        }
                        break;
                    case ValidationStage.LOCAL:
                        if (ch == 64) {
                            validationStage2 = ValidationStage.DOMAIN;
                            goto case 7;
                        }
                        if (ch == 46) {
                            validationStage1 = ValidationStage.LOCAL_DOT;
                            continue;
                        }
                        goto case 2;
                    case ValidationStage.LOCAL_DOT:
                        if (ch < 128 && MimeScan.IsAtom((byte) ch) || ch >= 128 && allowUTF8) {
                            validationStage1 = ValidationStage.LOCAL;
                            continue;
                        }
                        if (ch == 92) {
                            validationStage1 = ValidationStage.LOCAL_ESC;
                            continue;
                        }
                        break;
                    case ValidationStage.LOCAL_DQS:
                        if (ch == 34) {
                            validationStage1 = ValidationStage.LOCAL_DQS_END;
                            continue;
                        }
                        if (ch == 92) {
                            validationStage1 = ValidationStage.LOCAL_DQS_ESC;
                            continue;
                        }
                        if (ch < 128 && 13 != ch && (10 != ch && 92 != ch) && 34 != ch || ch >= 128 && allowUTF8) {
                            validationStage1 = ValidationStage.LOCAL_DQS;
                            continue;
                        }
                        break;
                    case ValidationStage.LOCAL_ESC:
                        if (ch < 128 || ch >= 128 && allowUTF8) {
                            validationStage1 = ValidationStage.LOCAL;
                            continue;
                        }
                        break;
                    case ValidationStage.LOCAL_DQS_ESC:
                        if (ch < 128 || ch >= 128 && allowUTF8) {
                            validationStage1 = ValidationStage.LOCAL_DQS;
                            continue;
                        }
                        break;
                    case ValidationStage.LOCAL_DQS_END:
                        if (ch == 64) {
                            validationStage2 = ValidationStage.DOMAIN;
                            goto case 7;
                        }
                        break;
                    case ValidationStage.DOMAIN:
                        if (checkLength && offset - 1 > 315 && (offset - 1 > 1604 || !MimeAddressParser.IsEncapsulatedX400Address(address))) {
                            domainStart = -1;
                            return false;
                        }
                        if (MimeAddressParser.IsValidDomain(address, offset, checkLength, allowUTF8)) {
                            domainStart = offset;
                            return true;
                        }
                        break;
                }
                validationStage1 = ValidationStage.ERROR;
            }
            domainStart = -1;
            return false;
        }

        public static bool IsValidDomain(string address, int offset, bool checkLength, bool allowUTF8 = false) {
            if (string.IsNullOrEmpty(address) || checkLength && address.Length - offset > byte.MaxValue)
                return false;
            var validationStage = ValidationStage.DOMAIN;
            var num1 = 0;
            var num2 = 0;
            var num3 = 0;
            var flag = false;
            var num4 = 0;
            while (offset < address.Length && validationStage != ValidationStage.ERROR) {
                var ch = address[offset];
                ++offset;
                switch (validationStage) {
                    case ValidationStage.DOMAIN:
                        if (ch == 91) {
                            validationStage = ValidationStage.DOMAIN_LITERAL;
                            continue;
                        }
                        if (ch < 128 && MimeScan.IsAlphaOrDigit((byte) ch) || ch >= 128 && allowUTF8 || (ch == 45 || ch == 95)) {
                            num4 = offset;
                            validationStage = ValidationStage.DOMAIN_SUB;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_SUB:
                        if (ch == 46) {
                            if (checkLength && offset - num4 > 63)
                                return false;
                            validationStage = ValidationStage.DOMAIN_DOT;
                            continue;
                        }
                        if (ch < 128 && MimeScan.IsAlphaOrDigit((byte) ch) || ch >= 128 && allowUTF8 || (ch == 45 || ch == 95)) {
                            validationStage = ValidationStage.DOMAIN_SUB;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_DOT:
                        if (ch < 128 && MimeScan.IsAlphaOrDigit((byte) ch) || ch >= 128 && allowUTF8 || (ch == 45 || ch == 95)) {
                            num4 = offset;
                            validationStage = ValidationStage.DOMAIN_SUB;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL:
                        if (ch < 128 && MimeScan.IsDigit((byte) ch)) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV4;
                            num1 = 1;
                            num2 = 1;
                            num3 = ch - 48;
                            continue;
                        }
                        if (ch == 73 || ch == 105) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_I;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV4:
                        if (ch < 128 && MimeScan.IsDigit((byte) ch) && num2 < 3) {
                            ++num2;
                            num3 = num3*10 + (ch - 48);
                            if (num3 <= byte.MaxValue) {
                                validationStage = ValidationStage.DOMAIN_LITERAL_IPV4;
                                continue;
                            }
                            break;
                        }
                        if (ch == 46) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV4_DOT;
                            continue;
                        }
                        if (ch == 93 && num1 == 4) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_END;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV4_DOT:
                        if (ch < 128 && MimeScan.IsDigit((byte) ch) && num1 < 4) {
                            ++num1;
                            num2 = 1;
                            num3 = ch - 48;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV4;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_I:
                        if (ch == 80 || ch == 112) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IP;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IP:
                        if (ch == 118 || ch == 86) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV:
                        if (ch == 54) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV6:
                        if (ch == 58) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV6_:
                        if (ch < 128 && MimeScan.IsHex((byte) ch)) {
                            flag = false;
                            num1 = 1;
                            num2 = 1;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
                            continue;
                        }
                        if (ch == 58) {
                            num1 = 0;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_STARTCOLON;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV6_STARTCOLON:
                        if (ch == 58) {
                            flag = true;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_COMP;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV6_GRP:
                        if (ch < 128 && MimeScan.IsHex((byte) ch) && num2 < 4) {
                            ++num2;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
                            continue;
                        }
                        if (ch == 58) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_COLON;
                            continue;
                        }
                        if (ch == 93 && (!flag && num1 == 8 || flag && num1 <= 6)) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_END;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV6_V4GRP:
                        if (ch < 128 && MimeScan.IsDigit((byte) ch) && num2 < 3) {
                            ++num2;
                            num3 = num3*10 + (ch - 48);
                            validationStage = num3 <= (int) byte.MaxValue ? ValidationStage.DOMAIN_LITERAL_IPV6_V4GRP : ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
                            continue;
                        }
                        if (ch < 128 && MimeScan.IsHex((byte) ch) && num2 < 4) {
                            ++num2;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
                            continue;
                        }
                        if (ch == 58) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_COLON;
                            continue;
                        }
                        if (ch == 46) {
                            num1 = 1;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV4_DOT;
                            continue;
                        }
                        if (ch == 93 && flag && num1 <= 6) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_END;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV6_COLON:
                        if (ch < 128 && MimeScan.IsDigit((byte) ch) && (!flag && num1 == 6) || flag && num1 <= 4) {
                            ++num1;
                            num2 = 1;
                            num3 = ch - 48;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_V4GRP;
                            continue;
                        }
                        if (ch < 128 && MimeScan.IsHex((byte) ch) && (!flag && num1 < 8) || flag && num1 < 6) {
                            ++num1;
                            num2 = 1;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
                            continue;
                        }
                        if (ch == 58 && !flag && num1 <= 6) {
                            flag = true;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_COMP;
                            continue;
                        }
                        break;
                    case ValidationStage.DOMAIN_LITERAL_IPV6_COMP:
                        if (ch < 128 && MimeScan.IsHex((byte) ch) && num1 < 6) {
                            ++num1;
                            num2 = 1;
                            validationStage = ValidationStage.DOMAIN_LITERAL_IPV6_GRP;
                            continue;
                        }
                        if (ch == 93) {
                            validationStage = ValidationStage.DOMAIN_LITERAL_END;
                            continue;
                        }
                        break;
                }
                validationStage = ValidationStage.ERROR;
            }
            if (validationStage == ValidationStage.DOMAIN_LITERAL_END)
                return true;
            if (validationStage != ValidationStage.DOMAIN_SUB)
                return false;
            if (checkLength)
                return offset - num4 < 63;
            return true;
        }

        public void Initialize(MimeStringList lines, bool ignoreComments, bool useSquareBrackets, bool allowUTF8) {
            this.Reset();
            this.ignoreComments = ignoreComments;
            this.useSquareBrackets = useSquareBrackets;
            valueParser = new ValueParser(lines, allowUTF8);
            this.Initialized = true;
        }

        public void Reset() {
            groupInProgress = false;
            this.Initialized = false;
        }

        public AddressParserResult ParseNextMailbox(ref MimeStringList displayName, ref MimeStringList address) {
            var addressParserResult = groupInProgress ? AddressParserResult.GroupInProgress : AddressParserResult.Mailbox;
            var parserStage = ParserStage.BEGIN;
            var mimeStringList1 = new MimeStringList();
            var mimeStringList2 = new MimeStringList();
            var mimeStringList3 = new MimeStringList();
            var flag1 = true;
            var flag2 = false;
            var ignoreNextByte = false;
            if (!this.Initialized)
                throw new InvalidOperationException(Resources.Strings.AddressParserNotInitialized);
            while (true) {
                byte num1;
                do {
                    do {
                        do {
                            do {
                                if (valueParser.ParseToDelimiter(ignoreNextByte, !flag1 && flag2, ref mimeStringList1)) {
                                    flag2 = false;
                                    ignoreNextByte = false;
                                    flag1 = false;
                                }
                                num1 = valueParser.ParseGet();
                                var num2 = num1;
                                if (num2 <= 34U) {
                                    switch (num2) {
                                        case 0:
                                            goto label_24;
                                        case 9:
                                        case 10:
                                        case 13:
                                        case 32:
                                            valueParser.ParseWhitespace(false, ref mimeStringList3);
                                            flag2 = true;
                                            continue;
                                        case 34:
                                            valueParser.ParseUnget();
                                            if (mimeStringList1.Length != 0 && !flag1)
                                                valueParser.ParseAppendSpace(ref mimeStringList1);
                                            else
                                                flag1 = false;
                                            valueParser.ParseQString(true, ref mimeStringList1, true);
                                            flag2 = true;
                                            continue;
                                    }
                                } else if (num2 <= 46U) {
                                    switch (num2) {
                                        case 40:
                                            if (mimeStringList2.Length != 0)
                                                mimeStringList2.Reset();
                                            valueParser.ParseUnget();
                                            if (ignoreComments) {
                                                valueParser.ParseComment(true, false, ref mimeStringList2, true);
                                                if (flag2)
                                                    mimeStringList1.AppendFragment(new MimeString(" "));
                                                mimeStringList1.TakeOverAppend(ref mimeStringList2);
                                            } else
                                                valueParser.ParseComment(true, true, ref mimeStringList2, true);
                                            flag2 = true;
                                            continue;
                                        case 44:
                                            goto label_24;
                                        case 46:
                                            valueParser.ParseAppendLastByte(ref mimeStringList1);
                                            flag1 = true;
                                            continue;
                                    }
                                } else {
                                    switch (num2) {
                                        case 58:
                                        case 59:
                                        case 60:
                                        case 62:
                                        case 64:
                                        case 91:
                                        case 93:
                                            goto label_24;
                                    }
                                }
                                valueParser.ParseUnget();
                                ignoreNextByte = true;
                                continue;
                                label_24:
                                switch (parserStage) {
                                    case ParserStage.BEGIN:
                                        var num3 = num1;
                                        if (num3 <= 44U) {
                                            if (num3 != 0) {
                                                if (num3 != 44)
                                                    goto label_41;
                                            } else
                                                goto label_40;
                                        } else {
                                            switch (num3) {
                                                case 58:
                                                    continue;
                                                case 59:
                                                    groupInProgress = false;
                                                    addressParserResult = AddressParserResult.Mailbox;
                                                    break;
                                                case 60:
                                                    goto label_38;
                                                case 62:
                                                    goto label_39;
                                                case 64:
                                                    goto label_35;
                                                case 91:
                                                    goto label_36;
                                                default:
                                                    goto label_41;
                                            }
                                        }
                                        if (mimeStringList1.GetLength(4026531839U) != 0) {
                                            displayName.TakeOver(ref mimeStringList1);
                                            goto label_89;
                                        }
                                        mimeStringList1.Reset();
                                        continue;
                                    case ParserStage.ANGLEADDR:
                                        goto label_42;
                                    case ParserStage.ADDRSPEC:
                                        goto label_60;
                                    case ParserStage.ROUTEDOMAIN:
                                        goto label_75;
                                    case ParserStage.END:
                                        goto label_85;
                                    default:
                                        goto label_89;
                                }
                            } while (mimeStringList1.GetLength(4026531839U) == 0);
                            displayName.TakeOver(ref mimeStringList1);
                            groupInProgress = true;
                            return AddressParserResult.GroupStart;
                            label_35:
                            var length = mimeStringList1.Length;
                            valueParser.ParseAppendLastByte(ref mimeStringList1);
                            address.TakeOver(ref mimeStringList1);
                            parserStage = ParserStage.ADDRSPEC;
                            continue;
                            label_36:
                            if (!useSquareBrackets) {
                                valueParser.ParseUnget();
                                ignoreNextByte = true;
                                continue;
                            }
                            label_38:
                            displayName.TakeOver(ref mimeStringList1);
                            parserStage = ParserStage.ANGLEADDR;
                            continue;
                            label_39:
                            mimeStringList1.Reset();
                            continue;
                            label_40:
                            displayName.TakeOver(ref mimeStringList1);
                            goto label_89;
                            label_41:
                            valueParser.ParseUnget();
                            ignoreNextByte = true;
                            continue;
                            label_42:
                            var num4 = num1;
                            if (num4 <= 44U) {
                                if (num4 != 0) {
                                    if (num4 == 44)
                                        goto label_57;
                                    goto label_59;
                                }
                                goto label_58;
                            }
                            switch (num4) {
                                case 58:
                                    goto label_56;
                                case 60:
                                    continue;
                                case 62:
                                    goto label_55;
                                case 64:
                                    if (mimeStringList1.Length == 0) {
                                        parserStage = ParserStage.ROUTEDOMAIN;
                                        continue;
                                    }
                                    valueParser.ParseAppendLastByte(ref mimeStringList1);
                                    address.TakeOver(ref mimeStringList1);
                                    parserStage = ParserStage.ADDRSPEC;
                                    continue;
                                case 91:
                                    if (!useSquareBrackets) {
                                        valueParser.ParseUnget();
                                        ignoreNextByte = true;
                                        continue;
                                    }
                                    goto case (byte) 60;
                                case 93:
                                    goto label_53;
                                default:
                                    goto label_59;
                            }
                        } while (mimeStringList1.Length == 0);
                        valueParser.ParseUnget();
                        ignoreNextByte = true;
                        continue;
                        label_53:
                        if (!useSquareBrackets) {
                            valueParser.ParseUnget();
                            ignoreNextByte = true;
                            continue;
                        }
                        label_55:
                        address.TakeOver(ref mimeStringList1);
                        parserStage = address.Length != 0 || displayName.Length != 0 ? ParserStage.END : ParserStage.BEGIN;
                        continue;
                        label_56:
                        mimeStringList1.Reset();
                        continue;
                        label_57:
                        ;
                    } while (displayName.Length == 0 && mimeStringList1.Length == 0);
                    label_58:
                    address.TakeOver(ref mimeStringList1);
                    break;
                    label_59:
                    valueParser.ParseUnget();
                    ignoreNextByte = true;
                    continue;
                    label_60:
                    var num5 = num1;
                    if (num5 <= 44U) {
                        if (num5 != 0 && num5 != 44)
                            goto label_74;
                    } else {
                        switch (num5) {
                            case 59:
                                groupInProgress = false;
                                break;
                            case 60:
                                if (displayName.Length == 0) {
                                    displayName.TakeOverAppend(ref address);
                                    parserStage = ParserStage.ANGLEADDR;
                                    continue;
                                }
                                valueParser.ParseUnget();
                                ignoreNextByte = true;
                                continue;
                            case 62:
                                address.TakeOverAppend(ref mimeStringList1);
                                parserStage = ParserStage.END;
                                continue;
                            case 91:
                                if (mimeStringList1.Length == 0) {
                                    valueParser.ParseUnget();
                                    valueParser.ParseDomainLiteral(true, ref mimeStringList1);
                                    address.TakeOverAppend(ref mimeStringList1);
                                    parserStage = ParserStage.END;
                                    continue;
                                }
                                valueParser.ParseUnget();
                                ignoreNextByte = true;
                                continue;
                            case 93:
                                if (!useSquareBrackets) {
                                    valueParser.ParseUnget();
                                    ignoreNextByte = true;
                                    continue;
                                }
                                goto case (byte) 62;
                            default:
                                goto label_74;
                        }
                    }
                    address.TakeOverAppend(ref mimeStringList1);
                } while (address.Length == 0 && displayName.Length == 0 && num1 != 0);
                break;
                label_74:
                valueParser.ParseUnget();
                ignoreNextByte = true;
                continue;
                label_75:
                var num6 = num1;
                if (num6 <= 44U) {
                    if (num6 != 0) {
                        if (num6 != 44)
                            goto label_84;
                    } else
                        break;
                } else {
                    switch (num6) {
                        case 58:
                            break;
                        case 62:
                            mimeStringList1.Reset();
                            parserStage = ParserStage.END;
                            continue;
                        case 91:
                            mimeStringList1.Reset();
                            valueParser.ParseUnget();
                            valueParser.ParseDomainLiteral(false, ref mimeStringList3);
                            continue;
                        case 93:
                            if (!useSquareBrackets) {
                                valueParser.ParseUnget();
                                ignoreNextByte = true;
                                continue;
                            }
                            goto case (byte) 62;
                        default:
                            goto label_84;
                    }
                }
                mimeStringList1.Reset();
                parserStage = ParserStage.ANGLEADDR;
                continue;
                label_84:
                valueParser.ParseUnget();
                ignoreNextByte = true;
                continue;
                label_85:
                mimeStringList1.Reset();
                switch (num1) {
                    case 0:
                    case 44:
                        if (address.Length == 0 && displayName.Length == 0 && num1 != 0) {
                            parserStage = ParserStage.BEGIN;
                            continue;
                        }
                        goto label_89;
                    case 59:
                        groupInProgress = false;
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

        private static bool IsEncapsulatedX400Address(string address) {
            return address.StartsWith("IMCEAX400-", StringComparison.OrdinalIgnoreCase);
        }

        public const int MaxEmailName = 315;
        public const int MaxX400EmailName = 1604;
        public const int MaxDomainName = 255;
        public const int MaxSubDomainName = 63;
        public const int MaxInternetName = 571;
        public const int MaxX400InternetName = 1860;
        private bool groupInProgress;
        private bool ignoreComments;
        private bool useSquareBrackets;
        private ValueParser valueParser;


        private enum ParserStage {

            BEGIN,
            ANGLEADDR,
            ADDRSPEC,
            ROUTEDOMAIN,
            END

        }


        private enum ValidationStage {

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
            ERROR

        }

    }

}