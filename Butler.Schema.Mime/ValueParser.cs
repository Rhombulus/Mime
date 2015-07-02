using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal struct ValueParser {

        public ValueParser(MimeStringList lines, bool allowUTF8) {
            this.lines = lines;
            this.allowUTF8 = allowUTF8;
            nextLine = 0;
            bytes = null;
            start = 0;
            end = 0;
            position = 0;
            this.ParseNextLine();
        }

        public ValueParser(MimeStringList lines, ValueParser valueParser) {
            this.lines = lines;
            allowUTF8 = valueParser.allowUTF8;
            nextLine = valueParser.nextLine;
            if (nextLine > 0 && nextLine <= this.lines.Count) {
                int count;
                bytes = this.lines[nextLine - 1].GetData(out start, out count);
                start = valueParser.start;
                position = valueParser.position;
                end = valueParser.end;
            } else {
                bytes = null;
                start = 0;
                end = 0;
                position = 0;
            }
        }

        private bool Eof => nextLine >= lines.Count;

        public static int ParseToken(string value, int currentOffset, bool allowUTF8) {
            return MimeScan.FindEndOf(MimeScan.Token.Token, value, currentOffset, allowUTF8);
        }

        public static int ParseToken(MimeString str, out int characterCount, bool allowUTF8) {
            return MimeScan.FindEndOf(MimeScan.Token.Token, str.Data, str.Offset, str.Length, out characterCount, allowUTF8);
        }

        public bool ParseToDelimiter(bool ignoreNextByte, bool separateWithWhitespace, ref MimeStringList phrase) {
            var flag = false;
            var num = ignoreNextByte ? 1 : 0;
            while (true) {
                var characterCount = 0;
                var count = num + MimeScan.FindEndOf(MimeScan.Token.Atom, bytes, position + num, end - position - num, out characterCount, allowUTF8);
                if (count != 0) {
                    flag = true;
                    if (phrase.Length != 0 && separateWithWhitespace) {
                        if (position == start || bytes[position - 1] != 32)
                            phrase.AppendFragment(SpaceLine);
                        else {
                            --position;
                            ++count;
                        }
                    }
                    separateWithWhitespace = false;
                    phrase.AppendFragment(new MimeString(bytes, position, count));
                    position += count;
                }
                if (position == end && this.ParseNextLine())
                    num = 0;
                else
                    break;
            }
            return flag;
        }

        public byte ParseGet() {
            if (position == end && !this.ParseNextLine())
                return 0;
            return bytes[position++];
        }

        public void ParseUnget() {
            if (position == start)
                this.ParseUngetLine();
            --position;
        }

        public void ParseQString(bool save, ref MimeStringList phrase, bool handleISO2022) {
            var quotedPair = false;
            if (save)
                phrase.AppendFragment(new MimeString(bytes, position, 1, 268435456U));
            ++position;
            var singleByte = true;
            do {
                var count = MimeScan.ScanQuotedString(bytes, position, end - position, handleISO2022, ref quotedPair);
                if (count != 0) {
                    if (save)
                        phrase.AppendFragment(new MimeString(bytes, position, count));
                    position += count;
                }
                if (position != end) {
                    if (bytes[position] == 14 || bytes[position] == 27)
                        this.ParseEscapedString(save, ref phrase, out singleByte);
                    else {
                        if (save)
                            phrase.AppendFragment(new MimeString(bytes, position, 1, 268435456U));
                        ++position;
                        if (bytes[position - 1] == 34)
                            return;
                        quotedPair = true;
                    }
                }
            } while (this.ParseNextLine());
            if (!save || !singleByte)
                return;
            phrase.AppendFragment(new MimeString(MimeString.DoubleQuote, 0, MimeString.DoubleQuote.Length, 268435456U));
        }

        public void ParseComment(bool save, bool saveInnerOnly, ref MimeStringList comment, bool handleISO2022) {
            var level = 1;
            var quotedPair = false;
            var num1 = 0;
            if (save && !saveInnerOnly)
                comment.AppendFragment(new MimeString(bytes, position, 1));
            ++position;
            do {
                var num2 = MimeScan.ScanComment(bytes, position, end - position, handleISO2022, ref level, ref quotedPair);
                if (num2 != 0) {
                    if (save) {
                        if (level == 0 && saveInnerOnly)
                            num1 = 1;
                        comment.AppendFragment(new MimeString(bytes, position, num2 - num1));
                    }
                    position += num2;
                    if (level == 0)
                        break;
                }
                if (position != end && (bytes[position] == 14 || bytes[position] == 27)) {
                    bool singleByte;
                    this.ParseEscapedString(save, ref comment, out singleByte);
                }
            } while (this.ParseNextLine());
        }

        public bool ParseNextLine() {
            if (nextLine >= lines.Count)
                return false;
            int count;
            bytes = lines[nextLine].GetData(out start, out count);
            position = start;
            end = start + count;
            ++nextLine;
            return true;
        }

        public void ParseUngetLine() {
            int count;
            bytes = lines[nextLine - 2].GetData(out start, out count);
            position = end = start + count;
            --nextLine;
        }

        public void ParseWhitespace(bool save, ref MimeStringList phrase) {
            do {
                var count = MimeScan.SkipLwsp(bytes, position, end - position);
                if (save && count != 0)
                    phrase.AppendFragment(new MimeString(bytes, position, count));
                position += count;
            } while (position == end && this.ParseNextLine());
        }

        public void ParseCFWS(bool save, ref MimeStringList phrase, bool handleISO2022) {
            do {
                var count = MimeScan.SkipLwsp(bytes, position, end - position);
                if (save && count != 0)
                    phrase.AppendFragment(new MimeString(bytes, position, count));
                position += count;
                if (position != end) {
                    if (bytes[position] == 40)
                        this.ParseComment(save, false, ref phrase, handleISO2022);
                    else
                        goto label_6;
                }
            } while (this.ParseNextLine());
            goto label_7;
            label_6:
            return;
            label_7:
            ;
        }

        public void ParseSkipToNextDelimiterByte(byte delimiter) {
            var mimeStringList = new MimeStringList();
            while (true) {
                while (position == end) {
                    if (!this.ParseNextLine())
                        return;
                }
                var num = bytes[position];
                if (num != delimiter) {
                    if (num == 34)
                        this.ParseQString(false, ref mimeStringList, true);
                    else if (num == 40)
                        this.ParseComment(false, false, ref mimeStringList, true);
                    else {
                        ++position;
                        this.ParseCFWS(false, ref mimeStringList, true);
                        var characterCount = 0;
                        position += MimeScan.FindEndOf(MimeScan.Token.Atom, bytes, position, end - position, out characterCount, allowUTF8);
                    }
                } else
                    break;
            }
        }

        public MimeString ParseToken() {
            return this.ParseToken(MimeScan.Token.Token);
        }

        public MimeString ParseToken(MimeScan.Token token) {
            var mimeStringList = new MimeStringList();
            while (position != end || this.ParseNextLine()) {
                var characterCount = 0;
                var endOf = MimeScan.FindEndOf(token, bytes, position, end - position, out characterCount, allowUTF8);
                if (endOf != 0) {
                    mimeStringList.AppendFragment(new MimeString(bytes, position, endOf));
                    position += endOf;
                } else
                    break;
            }
            if (mimeStringList.Count == 0)
                return new MimeString();
            if (mimeStringList.Count == 1)
                return mimeStringList[0];
            var sz = mimeStringList.GetSz();
            return new MimeString(sz, 0, sz.Length);
        }

        public void ParseParameterValue(ref MimeStringList value, ref bool goodValue, bool handleISO2022) {
            var mimeStringList = new MimeStringList();
            goodValue = true;
            while (position != end || this.ParseNextLine()) {
                var ch = bytes[position];
                switch (ch) {
                    case 34:
                        value.Reset();
                        mimeStringList.Reset();
                        this.ParseQString(true, ref value, handleISO2022);
                        return;
                    case 40:
                        this.ParseCFWS(true, ref mimeStringList, handleISO2022);
                        continue;
                    default:
                        if (!MimeScan.IsLWSP(ch)) {
                            if (ch == 59)
                                return;
                            var offset = position;
                            do {
                                var bytesUsed = 1;
                                if (!MimeScan.IsToken(ch)) {
                                    if (allowUTF8 && ch >= 128) {
                                        if (!MimeScan.IsUTF8NonASCII(bytes, position, end, out bytesUsed)) {
                                            bytesUsed = 1;
                                            goodValue = false;
                                        }
                                    } else
                                        goodValue = false;
                                }
                                position += bytesUsed;
                                if (position != end) {
                                    ch = bytes[position];
                                    switch (ch) {
                                        case 59:
                                        case 40:
                                            goto label_17;
                                        default:
                                            continue;
                                    }
                                }
                                break;
                            } while (!MimeScan.IsLWSP(ch));
                            label_17:
                            value.TakeOverAppend(ref mimeStringList);
                            value.AppendFragment(new MimeString(bytes, offset, position - offset));
                            continue;
                        }
                        goto case (byte) 40;
                }
            }
        }

        public void ParseDomainLiteral(bool save, ref MimeStringList domain) {
            var flag = false;
            var offset = position;
            ++position;
            byte num;
            do {
                if (position == end) {
                    if (offset != position && save)
                        domain.AppendFragment(new MimeString(bytes, offset, position - offset));
                    if (!this.ParseNextLine()) {
                        offset = position;
                        break;
                    }
                    offset = position;
                }
                num = bytes[position++];
                if (flag)
                    flag = false;
                else if (num == 92)
                    flag = true;
            } while (num != 93);
            if (offset == position || !save)
                return;
            domain.AppendFragment(new MimeString(bytes, offset, position - offset));
        }

        public void ParseToEnd(ref MimeStringList phrase) {
            if (position != end) {
                phrase.AppendFragment(new MimeString(bytes, position, end - position));
                position = end;
            }
            while (this.ParseNextLine()) {
                phrase.AppendFragment(new MimeString(bytes, start, end - start));
                position = end;
            }
        }

        public void ParseAppendLastByte(ref MimeStringList phrase) {
            phrase.AppendFragment(new MimeString(bytes, position - 1, 1));
        }

        public void ParseAppendSpace(ref MimeStringList phrase) {
            if (position == start || bytes[position - 1] != 32)
                phrase.AppendFragment(SpaceLine);
            else
                phrase.AppendFragment(new MimeString(bytes, position - 1, 1));
        }

        private void ParseEscapedString(bool save, ref MimeStringList outStr, out bool singleByte) {
            var flag = bytes[position] == 27;
            if (save)
                outStr.AppendFragment(new MimeString(bytes, position, 1, 536870912U));
            ++position;
            if (flag && !this.ParseEscapeSequence(save, ref outStr))
                singleByte = true;
            else {
                singleByte = false;
                do {
                    var count = MimeScan.ScanJISString(bytes, position, end - position, ref singleByte);
                    if (save && count != 0)
                        outStr.AppendFragment(new MimeString(bytes, position, count, 536870912U));
                    position += count;
                } while (!singleByte && this.ParseNextLine());
                if (flag || position == end || bytes[position] != 15)
                    return;
                if (save)
                    outStr.AppendFragment(new MimeString(bytes, position, 1, 536870912U));
                ++position;
            }
        }

        private bool ParseEscapeSequence(bool save, ref MimeStringList outStr) {
            var num1 = this.ParseGet();
            var num2 = this.ParseGet();
            var num3 = this.ParseGet();
            if (num3 != 0)
                this.ParseUnget();
            if (num2 != 0)
                this.ParseUnget();
            if (num1 != 0)
                this.ParseUnget();
            var num4 = 0;
            var flag = false;
            switch (num1) {
                case 36:
                    if (num2 == 66 || num2 == 65 || num2 == 64) {
                        num4 = 2;
                        flag = true;
                        break;
                    }
                    if (num2 == 40 && (num3 == 67 || num3 == 68)) {
                        num4 = 3;
                        flag = true;
                    }
                    break;
                case 40:
                    if (num2 == 73) {
                        flag = true;
                        num4 = 2;
                        break;
                    }
                    if (num2 == 66 || num2 == 74 || num2 == 72)
                        num4 = 2;
                    break;
                case 78:
                case 79:
                    if (num2 >= 33) {
                        num4 = 2;
                        if (num3 >= 33)
                            num4 = 3;
                    }
                    break;
            }
            while (num4-- != 0) {
                int num5 = this.ParseGet();
                if (save)
                    outStr.AppendFragment(new MimeString(bytes, position - 1, 1, 536870912U));
            }
            return flag;
        }

        private static readonly MimeString SpaceLine = new MimeString(" ");
        private readonly bool allowUTF8;
        private byte[] bytes;
        private int end;
        private MimeStringList lines;
        private int nextLine;
        private int position;
        private int start;

    }

}