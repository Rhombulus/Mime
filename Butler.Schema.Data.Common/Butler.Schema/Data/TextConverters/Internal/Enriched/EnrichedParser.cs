using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Enriched
{

    internal class EnrichedParser : IDisposable
    {
        private bool endOfFile;
        private ConverterInput input;
        private const int MaxValidTagLength = 0x11;
        private int nameLength;
        private NewLineState newLineState;
        private int nofill;
        private char[] parseBuffer;
        private int parseCurrent;
        private int parseEnd;
        private int parseStart;
        private ParseState parseState;
        private int parseThreshold = 1;
        private Html.HtmlToken token;
        private Html.HtmlTokenBuilder tokenBuilder;

        public EnrichedParser(ConverterInput input, int maxRuns, bool testBoundaryConditions)
        {
            this.input = input;
            this.tokenBuilder = new Html.HtmlTokenBuilder(null, maxRuns, 0, testBoundaryConditions);
            this.token = this.tokenBuilder.Token;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (this.input != null))
            {
                this.input.Dispose();
            }
            this.input = null;
            this.parseBuffer = null;
            this.token = null;
            this.tokenBuilder = null;
        }

        private void FlushNewLineState(int runStart)
        {
            if (this.newLineState == NewLineState.OneNewLine)
            {
                this.tokenBuilder.AddLiteralRun(RunTextType.Space, Html.HtmlRunKind.Text, runStart, runStart, 0x20);
            }
            this.newLineState = NewLineState.None;
        }

        public Html.HtmlTokenId Parse()
        {
            bool flag;
            bool flag2;
            int num6;
            Html.HtmlNameIndex index;
            Html.HtmlTokenBuilder tokenBuilder = this.tokenBuilder;
            if (tokenBuilder.Valid)
            {
                if (tokenBuilder.IncompleteTag)
                {
                    int num2 = tokenBuilder.RewindTag();
                    this.input.ReportProcessed(num2 - this.parseStart);
                    this.parseStart = num2;
                }
                else
                {
                    this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                    this.parseStart = this.parseCurrent;
                    tokenBuilder.Reset();
                }
            }
            char[] parseBuffer = this.parseBuffer;
            int parseCurrent = this.parseCurrent;
            int parseEnd = this.parseEnd;
            int parseThreshold = this.parseThreshold;
            Label_008C:
            flag = false;
            if ((parseCurrent + parseThreshold) > parseEnd)
            {
                if (!this.endOfFile)
                {
                    this.parseCurrent = parseCurrent;
                    if (!this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd))
                    {
                        return Html.HtmlTokenId.None;
                    }
                    tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
                    ConverterDecodingInput input = this.input as ConverterDecodingInput;
                    if ((input != null) && input.EncodingChanged)
                    {
                        input.EncodingChanged = false;
                        return tokenBuilder.MakeEmptyToken(Html.HtmlTokenId.EncodingChange, input.Encoding);
                    }
                    parseBuffer = this.parseBuffer;
                    parseCurrent = this.parseCurrent;
                    parseEnd = this.parseEnd;
                    if (this.input.EndOfFile)
                    {
                        this.endOfFile = true;
                    }
                    if (!this.endOfFile && ((parseEnd - this.parseStart) < this.input.MaxTokenSize))
                    {
                        goto Label_008C;
                    }
                }
                flag = true;
            }
            char ch = parseBuffer[parseCurrent];
            CharClass charClass = ParseSupport.GetCharClass(ch);
            if (ParseSupport.InvalidUnicodeCharacter(charClass) || (parseThreshold > 1))
            {
                if (!this.SkipInvalidCharacters(ref ch, ref charClass, ref parseCurrent))
                {
                    parseEnd = this.parseEnd;
                    if (!flag)
                    {
                        goto Label_008C;
                    }
                    if (((parseCurrent == parseEnd) && !tokenBuilder.IsStarted) && this.endOfFile)
                    {
                        this.parseCurrent = parseCurrent;
                        return tokenBuilder.MakeEmptyToken(Html.HtmlTokenId.EndOfFile);
                    }
                }
                parseEnd = this.parseEnd;
                parseThreshold = this.parseThreshold = 1;
            }
            int baseOffset = parseCurrent;
            switch (this.parseState)
            {
                case ParseState.Text:
                    tokenBuilder.StartText(baseOffset);
                    break;

                case ParseState.Tag:
                    if (((this.parseEnd - this.parseCurrent) >= 0x11) || flag)
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                        flag2 = false;
                        num6 = 1;
                        if (ch == '/')
                        {
                            flag2 = true;
                            num6++;
                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        ch = this.ScanTag(ch, ref charClass, ref parseCurrent);
                        this.nameLength = parseCurrent - (baseOffset + num6);
                        if (ch != '>')
                        {
                            if (this.newLineState == NewLineState.OneNewLine)
                            {
                                this.newLineState = NewLineState.None;
                                tokenBuilder.StartText(baseOffset);
                                tokenBuilder.AddLiteralRun(RunTextType.Space, Html.HtmlRunKind.Text, baseOffset, baseOffset, 0x20);
                                tokenBuilder.EndText();
                                this.parseCurrent = baseOffset;
                                return this.token.HtmlTokenId;
                            }
                            tokenBuilder.StartTag(Html.HtmlNameIndex.Unknown, baseOffset);
                            if (flag2)
                            {
                                tokenBuilder.SetEndTag();
                            }
                            tokenBuilder.AddRun(RunTextType.NonSpace, Html.HtmlRunKind.TagPrefix, baseOffset, baseOffset + num6);
                            tokenBuilder.StartTagName();
                            if (this.nameLength != 0)
                            {
                                tokenBuilder.AddRun(RunTextType.NonSpace, Html.HtmlRunKind.Name, baseOffset + num6, parseCurrent);
                            }
                            this.parseState = ParseState.LongTag;
                            goto Label_0502;
                        }
                        parseCurrent++;
                        index = Html.HtmlTokenBuilder.LookupName(parseBuffer, baseOffset + num6, this.nameLength);
                        switch (index)
                        {
                            case Html.HtmlNameIndex.FlushLeft:
                            case Html.HtmlNameIndex.FlushRight:
                            case Html.HtmlNameIndex.FlushBoth:
                            case Html.HtmlNameIndex.Center:
                            case Html.HtmlNameIndex.Nofill:
                            case Html.HtmlNameIndex.ParaIndent:
                            case Html.HtmlNameIndex.Excerpt:
                                this.newLineState = NewLineState.EatTwoNewLines;
                                if (index == Html.HtmlNameIndex.Nofill)
                                {
                                    if (!flag2)
                                    {
                                        this.nofill++;
                                        this.newLineState = NewLineState.None;
                                    }
                                    else if (this.nofill != 0)
                                    {
                                        this.nofill--;
                                    }
                                }
                                goto Label_043E;
                        }
                        if (this.newLineState == NewLineState.OneNewLine)
                        {
                            this.newLineState = NewLineState.None;
                            tokenBuilder.StartText(baseOffset);
                            tokenBuilder.AddLiteralRun(RunTextType.Space, Html.HtmlRunKind.Text, baseOffset, baseOffset, 0x20);
                            tokenBuilder.EndText();
                            this.parseCurrent = baseOffset;
                            return this.token.HtmlTokenId;
                        }
                        this.newLineState = NewLineState.None;
                        goto Label_043E;
                    }
                    parseThreshold = this.parseThreshold = 0x11;
                    goto Label_008C;

                case ParseState.LongTag:
                    if (!tokenBuilder.PrepareToAddMoreRuns(2, baseOffset, Html.HtmlRunKind.Name))
                    {
                        goto Label_0545;
                    }
                    ch = this.ScanTag(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent != baseOffset)
                    {
                        this.nameLength += parseCurrent - baseOffset;
                        tokenBuilder.AddRun(RunTextType.NonSpace, Html.HtmlRunKind.Name, baseOffset, parseCurrent);
                    }
                    if (ch != '>')
                    {
                        goto Label_0502;
                    }
                    tokenBuilder.EndTagName(this.nameLength);
                    parseCurrent++;
                    tokenBuilder.AddRun(RunTextType.NonSpace, Html.HtmlRunKind.TagSuffix, parseCurrent - 1, parseCurrent);
                    tokenBuilder.EndTag(true);
                    if (((parseBuffer[parseCurrent] == '<') && ((parseCurrent + 1) < parseEnd)) && ((parseBuffer[parseCurrent + 1] != '<') && !ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[parseCurrent + 1]))))
                    {
                        this.parseState = ParseState.Tag;
                    }
                    else
                    {
                        this.parseState = ParseState.Text;
                    }
                    this.parseCurrent = parseCurrent;
                    return this.token.HtmlTokenId;

                default:
                    this.parseCurrent = parseCurrent;
                    throw new TextConvertersException("internal error: invalid parse state");
            }
            Label_01F9:
            this.ParseText(ch, charClass, ref parseCurrent);
            parseThreshold = this.parseThreshold;
            if (this.token.IsEmpty && !flag)
            {
                tokenBuilder.Reset();
                goto Label_008C;
            }
            tokenBuilder.EndText();
            this.parseCurrent = parseCurrent;
            return this.token.HtmlTokenId;
            Label_043E:
            tokenBuilder.StartTag(Html.HtmlNameIndex.Unknown, baseOffset);
            if (flag2)
            {
                tokenBuilder.SetEndTag();
            }
            tokenBuilder.AddRun(RunTextType.NonSpace, Html.HtmlRunKind.TagPrefix, baseOffset, baseOffset + num6);
            tokenBuilder.StartTagName();
            if (this.nameLength != 0)
            {
                tokenBuilder.AddRun(RunTextType.NonSpace, Html.HtmlRunKind.Name, baseOffset + num6, parseCurrent - 1);
            }
            tokenBuilder.EndTagName(index);
            tokenBuilder.AddRun(RunTextType.NonSpace, Html.HtmlRunKind.TagSuffix, parseCurrent - 1, parseCurrent);
            tokenBuilder.EndTag(true);
            if (((parseBuffer[parseCurrent] != '<') || ((parseCurrent + 1) == parseEnd)) || ((parseBuffer[parseCurrent + 1] == '<') || ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[parseCurrent + 1]))))
            {
                this.parseState = ParseState.Text;
            }
            this.parseCurrent = parseCurrent;
            return this.token.HtmlTokenId;
            Label_0502:
            if (!flag || ((parseCurrent + parseThreshold) < parseEnd))
            {
                goto Label_008C;
            }
            if (this.endOfFile)
            {
                if (!this.token.IsTagBegin)
                {
                    tokenBuilder.EndTag(true);
                    this.parseCurrent = parseCurrent;
                    return this.token.HtmlTokenId;
                }
                parseCurrent = this.parseStart;
                tokenBuilder.Reset();
                baseOffset = parseCurrent;
                tokenBuilder.StartText(baseOffset);
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                this.PrepareToAddTextRun(baseOffset);
                tokenBuilder.AddTextRun(RunTextType.NonSpace, baseOffset, parseCurrent);
                this.parseState = ParseState.Text;
                goto Label_01F9;
            }
            Label_0545:
            tokenBuilder.EndTag(false);
            this.parseCurrent = parseCurrent;
            return this.token.HtmlTokenId;
        }

        private void ParseText(char ch, CharClass charClass, ref int parseCurrent)
        {
            char ch2;
            int runStart = parseCurrent;
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;
            Html.HtmlTokenBuilder tokenBuilder = this.tokenBuilder;
            Label_001A:
            ch = this.ScanText(ch, ref charClass, ref parseCurrent);
            if (ParseSupport.WhitespaceCharacter(charClass))
            {
                if (parseCurrent != runStart)
                {
                    this.PrepareToAddTextRun(runStart);
                    tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                }
                runStart = parseCurrent;
                if (ch == ' ')
                {
                    ch2 = parseBuffer[parseCurrent + 1];
                    CharClass class2 = ParseSupport.GetCharClass(ch2);
                    if (!ParseSupport.WhitespaceCharacter(class2))
                    {
                        ch = ch2;
                        charClass = class2;
                        parseCurrent++;
                        this.PrepareToAddTextRun(runStart);
                        tokenBuilder.AddTextRun(RunTextType.Space, runStart, parseCurrent);
                        runStart = parseCurrent;
                        goto Label_023F;
                    }
                }
                this.ParseWhitespace(ch, charClass, ref parseCurrent);
                if (this.parseThreshold > 1)
                {
                    return;
                }
                runStart = parseCurrent;
                ch = parseBuffer[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            else
            {
                if (ch == '<')
                {
                    if (parseCurrent != runStart)
                    {
                        this.PrepareToAddTextRun(runStart);
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        runStart = parseCurrent;
                    }
                    if (parseBuffer[parseCurrent + 1] != '<')
                    {
                        ch2 = parseBuffer[parseCurrent + 1];
                        if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch2)))
                        {
                            if (this.endOfFile && ((parseCurrent + 1) == parseEnd))
                            {
                                parseCurrent++;
                                tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                                return;
                            }
                            this.parseThreshold = 2;
                            return;
                        }
                        this.parseState = ParseState.Tag;
                        return;
                    }
                    this.PrepareToAddTextRun(runStart);
                    tokenBuilder.AddLiteralRun(RunTextType.NonSpace, Html.HtmlRunKind.Text, runStart, parseCurrent, 60);
                    parseCurrent += 2;
                    runStart = parseCurrent;
                    ch = parseBuffer[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    goto Label_023F;
                }
                if (ch == '&')
                {
                    int num3;
                    parseCurrent = num3 = parseCurrent + 1;
                    ch = parseBuffer[num3];
                    charClass = ParseSupport.GetCharClass(ch);
                    goto Label_023F;
                }
                if (ParseSupport.NbspCharacter(charClass))
                {
                    if (parseCurrent != runStart)
                    {
                        this.PrepareToAddTextRun(runStart);
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }
                    runStart = parseCurrent;
                    do
                    {
                        int num4;
                        parseCurrent = num4 = parseCurrent + 1;
                        ch = parseBuffer[num4];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.NbspCharacter(charClass));
                    this.PrepareToAddTextRun(runStart);
                    tokenBuilder.AddTextRun(RunTextType.Nbsp, runStart, parseCurrent);
                }
                else
                {
                    if (parseCurrent != runStart)
                    {
                        this.PrepareToAddTextRun(runStart);
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }
                    if (parseCurrent >= parseEnd)
                    {
                        return;
                    }
                    do
                    {
                        int num5;
                        parseCurrent = num5 = parseCurrent + 1;
                        ch = parseBuffer[num5];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.InvalidUnicodeCharacter(charClass) && (parseCurrent < parseEnd));
                }
            }
            runStart = parseCurrent;
            Label_023F:
            if (tokenBuilder.PrepareToAddMoreRuns(3, runStart, Html.HtmlRunKind.Text))
            {
                goto Label_001A;
            }
        }

        private void ParseWhitespace(char ch, CharClass charClass, ref int parseCurrent)
        {
            int num3;
            int runStart = parseCurrent;
            char[] parseBuffer = this.parseBuffer;
            Html.HtmlTokenBuilder tokenBuilder = this.tokenBuilder;
            Label_0011:
            switch (ch)
            {
                case '\t':
                    do
                    {
                        int num4;
                        parseCurrent = num4 = parseCurrent + 1;
                        ch = parseBuffer[num4];
                    }
                    while (ch == '\t');
                    this.PrepareToAddTextRun(runStart);
                    tokenBuilder.AddTextRun(RunTextType.Tabulation, runStart, parseCurrent);
                    goto Label_0196;

                case '\n':
                    break;

                case '\r':
                    if (parseBuffer[parseCurrent + 1] == '\n')
                    {
                        parseCurrent++;
                        break;
                    }
                    if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[parseCurrent + 1])) || (this.endOfFile && ((parseCurrent + 1) >= this.parseEnd)))
                    {
                        break;
                    }
                    this.parseThreshold = 2;
                    goto Label_0196;

                case ' ':
                    do
                    {
                        int num2;
                        parseCurrent = num2 = parseCurrent + 1;
                        ch = parseBuffer[num2];
                    }
                    while (ch == ' ');
                    this.PrepareToAddTextRun(runStart);
                    tokenBuilder.AddTextRun(RunTextType.Space, runStart, parseCurrent);
                    goto Label_0196;

                default:
                    do
                    {
                        int num5;
                        parseCurrent = num5 = parseCurrent + 1;
                        ch = parseBuffer[num5];
                    }
                    while ((ch == '\v') || (ch == '\f'));
                    this.PrepareToAddTextRun(runStart);
                    tokenBuilder.AddTextRun(RunTextType.UnusualWhitespace, runStart, parseCurrent);
                    goto Label_0196;
            }
            parseCurrent = num3 = parseCurrent + 1;
            ch = parseBuffer[num3];
            if ((this.newLineState == NewLineState.None) && (this.nofill == 0))
            {
                this.newLineState = NewLineState.OneNewLine;
                tokenBuilder.AddInvalidRun(parseCurrent, Html.HtmlRunKind.Text);
            }
            else if (this.newLineState == NewLineState.EatTwoNewLines)
            {
                tokenBuilder.AddInvalidRun(parseCurrent, Html.HtmlRunKind.Text);
                this.newLineState = NewLineState.EatOneNewLine;
            }
            else if (this.newLineState == NewLineState.EatOneNewLine)
            {
                tokenBuilder.AddInvalidRun(parseCurrent, Html.HtmlRunKind.Text);
                this.newLineState = NewLineState.ManyNewLines;
            }
            else
            {
                tokenBuilder.AddTextRun(RunTextType.NewLine, runStart, parseCurrent);
                this.newLineState = NewLineState.ManyNewLines;
            }
            Label_0196:
            charClass = ParseSupport.GetCharClass(ch);
            runStart = parseCurrent;
            if ((!ParseSupport.WhitespaceCharacter(charClass) || !tokenBuilder.PrepareToAddMoreRuns(2, parseCurrent, Html.HtmlRunKind.Text)) || (this.parseThreshold != 1))
            {
                return;
            }
            goto Label_0011;
        }

        private void PrepareToAddTextRun(int runStart)
        {
            if (this.newLineState != NewLineState.None)
            {
                this.FlushNewLineState(runStart);
            }
        }

        private char ScanTag(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            while (((parseCurrent < this.parseEnd) || !ParseSupport.InvalidUnicodeCharacter(charClass)) && (ch != '>'))
            {
                int num;
                parseCurrent = num = parseCurrent + 1;
                ch = this.parseBuffer[num];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private char ScanText(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] parseBuffer = this.parseBuffer;
            while (ParseSupport.HtmlTextCharacter(charClass))
            {
                int num;
                parseCurrent = num = parseCurrent + 1;
                ch = parseBuffer[num];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private bool SkipInvalidCharacters(ref char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseCurrent;
            int parseEnd = this.parseEnd;
            while (ParseSupport.InvalidUnicodeCharacter(charClass) && (num < parseEnd))
            {
                ch = this.parseBuffer[++num];
                charClass = ParseSupport.GetCharClass(ch);
            }
            if ((this.parseThreshold > 1) && ((num + 1) < parseEnd))
            {
                int index = num + 1;
                int num4 = index;
                while ((num4 < parseEnd) && (index < (num + this.parseThreshold)))
                {
                    char ch2 = this.parseBuffer[num4];
                    if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch2)))
                    {
                        if (num4 != index)
                        {
                            this.parseBuffer[index] = ch2;
                            this.parseBuffer[num4] = '\0';
                        }
                        index++;
                    }
                    num4++;
                }
                if (num4 == parseEnd)
                {
                    parseEnd = this.parseEnd = this.input.RemoveGap(index, parseEnd);
                }
            }
            parseCurrent = num;
            return ((num + this.parseThreshold) <= parseEnd);
        }

        public Html.HtmlToken Token
        {
            get
            {
                return this.token;
            }
        }

        private enum NewLineState
        {
            None,
            OneNewLine,
            ManyNewLines,
            EatOneNewLine,
            EatTwoNewLines
        }

        protected enum ParseState : byte
        {
            LongTag = 2,
            Tag = 1,
            Text = 0
        }
    }
}

