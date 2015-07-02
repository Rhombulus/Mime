using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Text
{

    internal class TextParser : IDisposable
    {
        protected bool endOfFile;
        protected ConverterInput input;
        protected bool lastLineFlowed;
        protected int lastLineQuotingLevel;
        protected bool lastSpace;
        protected int lineCount;
        protected char[] parseBuffer;
        protected int parseCurrent;
        protected int parseEnd;
        protected int parseStart;
        protected int parseThreshold = 1;
        protected bool quotingExpected = true;
        protected int quotingLevel;
        protected bool signaturePossible = true;
        protected TextToken token;
        protected TextTokenBuilder tokenBuilder;
        protected bool unwrapDelSpace;
        protected bool unwrapFlowed;

        public TextParser(ConverterInput input, bool unwrapFlowed, bool unwrapDelSp, int maxRuns, bool testBoundaryConditions)
        {
            this.input = input;
            this.tokenBuilder = new TextTokenBuilder(null, maxRuns, testBoundaryConditions);
            this.token = this.tokenBuilder.Token;
            this.unwrapFlowed = unwrapFlowed;
            this.unwrapDelSpace = unwrapDelSp;
        }

        private void AddTextRun(RunTextType textType, int runStart, int runEnd)
        {
            if (!this.unwrapFlowed)
            {
                this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
            }
            else
            {
                this.AddTextRunUnwrap(textType, runStart, runEnd);
            }
        }

        private void AddTextRunUnwrap(RunTextType textType, int runStart, int runEnd)
        {
            RunTextType type = textType;
            if (type <= RunTextType.Tabulation)
            {
                if (type != RunTextType.Space)
                {
                    if (type != RunTextType.NewLine)
                    {
                        if (type == RunTextType.Tabulation)
                        {
                            if (this.quotingExpected)
                            {
                                if (this.lastLineQuotingLevel != this.quotingLevel)
                                {
                                    if (this.lastLineFlowed)
                                    {
                                        this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, 10);
                                    }
                                    this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
                                    this.lastLineQuotingLevel = this.quotingLevel;
                                }
                                this.quotingExpected = false;
                            }
                            this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
                            this.lineCount += runEnd - runStart;
                            this.lastSpace = false;
                            this.signaturePossible = false;
                        }
                        return;
                    }
                    if (!this.lastSpace || (this.signaturePossible && (this.lineCount == 3)))
                    {
                        this.lastLineFlowed = false;
                        this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
                    }
                    else
                    {
                        this.lastLineFlowed = true;
                    }
                    this.lineCount = 0;
                    this.lastSpace = false;
                    this.signaturePossible = true;
                    this.quotingExpected = true;
                    this.lastLineQuotingLevel = this.quotingLevel;
                    this.quotingLevel = 0;
                    return;
                }
            }
            else
            {
                switch (type)
                {
                    case RunTextType.UnusualWhitespace:
                        break;

                    case RunTextType.Nbsp:
                    case RunTextType.NonSpace:
                        if (this.quotingExpected)
                        {
                            while ((runStart != runEnd) && (this.parseBuffer[runStart] == '>'))
                            {
                                this.quotingLevel++;
                                runStart++;
                            }
                            this.tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);
                            if (runStart != runEnd)
                            {
                                if (this.lastLineQuotingLevel != this.quotingLevel)
                                {
                                    if (this.lastLineFlowed)
                                    {
                                        this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, 10);
                                    }
                                    this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
                                    this.lastLineQuotingLevel = this.quotingLevel;
                                }
                                this.quotingExpected = false;
                            }
                        }
                        if (runStart != runEnd)
                        {
                            this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
                            this.lineCount += runEnd - runStart;
                            this.lastSpace = false;
                            if (((this.lineCount > 2) || (this.parseBuffer[runStart] != '-')) || (((runEnd - runStart) == 2) && (this.parseBuffer[runStart + 1] != '-')))
                            {
                                this.signaturePossible = false;
                            }
                        }
                        return;

                    default:
                        return;
                }
            }
            if (this.quotingExpected)
            {
                runStart++;
                this.tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);
                if (this.lastLineQuotingLevel != this.quotingLevel)
                {
                    if (this.lastLineFlowed)
                    {
                        this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, 10);
                    }
                    this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
                    this.lastLineQuotingLevel = this.quotingLevel;
                }
                this.quotingExpected = false;
            }
            if (runStart != runEnd)
            {
                this.lineCount += runEnd - runStart;
                this.lastSpace = true;
                this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
                if ((this.lineCount != 3) || ((runEnd - runStart) != 1))
                {
                    this.signaturePossible = false;
                }
            }
        }

        public void Initialize(string fragment)
        {
            (this.input as ConverterBufferInput).Initialize(fragment);
            this.endOfFile = false;
            this.parseBuffer = null;
            this.parseStart = 0;
            this.parseCurrent = 0;
            this.parseEnd = 0;
            this.parseThreshold = 1;
            this.tokenBuilder.Reset();
            this.lastSpace = false;
            this.lineCount = 0;
            this.quotingExpected = true;
            this.quotingLevel = 0;
            this.lastLineQuotingLevel = 0;
            this.lastLineFlowed = false;
            this.signaturePossible = true;
        }

        public TextTokenId Parse()
        {
            char ch2;
            bool flag;
            if (this.tokenBuilder.Valid)
            {
                this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                this.parseStart = this.parseCurrent;
                this.tokenBuilder.Reset();
            }
            Label_003C:
            flag = false;
            if ((this.parseCurrent + this.parseThreshold) > this.parseEnd)
            {
                if (!this.endOfFile)
                {
                    if (!this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd))
                    {
                        return TextTokenId.None;
                    }
                    this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
                    ConverterDecodingInput input = this.input as ConverterDecodingInput;
                    if ((input != null) && input.EncodingChanged)
                    {
                        input.EncodingChanged = false;
                        return this.tokenBuilder.MakeEmptyToken(TextTokenId.EncodingChange, input.Encoding);
                    }
                    if (this.input.EndOfFile)
                    {
                        this.endOfFile = true;
                    }
                    if (!this.endOfFile && ((this.parseEnd - this.parseStart) < this.input.MaxTokenSize))
                    {
                        goto Label_003C;
                    }
                }
                flag = true;
            }
            char ch = this.parseBuffer[this.parseCurrent];
            CharClass charClass = ParseSupport.GetCharClass(ch);
            if (ParseSupport.InvalidUnicodeCharacter(charClass) || (this.parseThreshold > 1))
            {
                while (ParseSupport.InvalidUnicodeCharacter(charClass) && (this.parseCurrent < this.parseEnd))
                {
                    ch = this.parseBuffer[++this.parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                if ((this.parseThreshold > 1) && ((this.parseCurrent + 1) < this.parseEnd))
                {
                    int index = this.parseCurrent + 1;
                    int num3 = this.parseCurrent + 1;
                    while ((index < this.parseEnd) && (num3 < (this.parseCurrent + this.parseThreshold)))
                    {
                        ch2 = this.parseBuffer[index];
                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch2)))
                        {
                            if (index != num3)
                            {
                                this.parseBuffer[num3] = ch2;
                                this.parseBuffer[index] = '\0';
                            }
                            num3++;
                        }
                        index++;
                    }
                    if ((index == this.parseEnd) && ((this.parseCurrent + this.parseThreshold) > num3))
                    {
                        Array.Copy(this.parseBuffer, this.parseCurrent, this.parseBuffer, this.parseEnd - (num3 - this.parseCurrent), num3 - this.parseCurrent);
                        this.parseCurrent = this.parseEnd - (num3 - this.parseCurrent);
                        this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                        this.parseStart = this.parseCurrent;
                    }
                }
                if ((this.parseCurrent + this.parseThreshold) > this.parseEnd)
                {
                    if (!flag)
                    {
                        goto Label_003C;
                    }
                    if (((this.parseCurrent == this.parseEnd) && !this.tokenBuilder.IsStarted) && this.endOfFile)
                    {
                        return this.tokenBuilder.MakeEmptyToken(TextTokenId.EndOfFile);
                    }
                }
                this.parseThreshold = 1;
            }
            int parseCurrent = this.parseCurrent;
            this.tokenBuilder.StartText(parseCurrent);
            Label_0518:
            if (this.tokenBuilder.PrepareToAddMoreRuns(9, parseCurrent, RunKind.Text))
            {
                while (ParseSupport.TextUriCharacter(charClass))
                {
                    ch = this.parseBuffer[++this.parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                if (ParseSupport.TextNonUriCharacter(charClass))
                {
                    if (this.parseCurrent != parseCurrent)
                    {
                        this.AddTextRun(RunTextType.NonSpace, parseCurrent, this.parseCurrent);
                    }
                    parseCurrent = this.parseCurrent;
                    do
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.NbspCharacter(charClass));
                    this.AddTextRun(RunTextType.NonSpace, parseCurrent, this.parseCurrent);
                }
                else if (ParseSupport.WhitespaceCharacter(charClass))
                {
                    if (this.parseCurrent != parseCurrent)
                    {
                        this.AddTextRun(RunTextType.NonSpace, parseCurrent, this.parseCurrent);
                    }
                    parseCurrent = this.parseCurrent;
                    if (ch == ' ')
                    {
                        ch2 = this.parseBuffer[this.parseCurrent + 1];
                        CharClass class3 = ParseSupport.GetCharClass(ch2);
                        if (!ParseSupport.WhitespaceCharacter(class3))
                        {
                            ch = ch2;
                            charClass = class3;
                            this.parseCurrent++;
                            this.AddTextRun(RunTextType.Space, parseCurrent, this.parseCurrent);
                            parseCurrent = this.parseCurrent;
                            goto Label_0518;
                        }
                    }
                    this.ParseWhitespace(ch, charClass);
                    if (this.parseThreshold > 1)
                    {
                        goto Label_0531;
                    }
                    parseCurrent = this.parseCurrent;
                    ch = this.parseBuffer[this.parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                else if (ParseSupport.NbspCharacter(charClass))
                {
                    if (this.parseCurrent != parseCurrent)
                    {
                        this.AddTextRun(RunTextType.NonSpace, parseCurrent, this.parseCurrent);
                    }
                    parseCurrent = this.parseCurrent;
                    do
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.NbspCharacter(charClass));
                    this.AddTextRun(RunTextType.Nbsp, parseCurrent, this.parseCurrent);
                }
                else
                {
                    if (this.parseCurrent != parseCurrent)
                    {
                        this.AddTextRun(RunTextType.NonSpace, parseCurrent, this.parseCurrent);
                    }
                    if (this.parseCurrent >= this.parseEnd)
                    {
                        goto Label_0531;
                    }
                    do
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.InvalidUnicodeCharacter(charClass) && (this.parseCurrent < this.parseEnd));
                }
                parseCurrent = this.parseCurrent;
                goto Label_0518;
            }
            Label_0531:
            if (this.token.IsEmpty)
            {
                this.tokenBuilder.Reset();
                this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                this.parseStart = this.parseCurrent;
                goto Label_003C;
            }
            this.tokenBuilder.EndText();
            return (TextTokenId)this.token.TokenId;
        }

        private void ParseWhitespace(char ch, CharClass charClass)
        {
            int parseCurrent = this.parseCurrent;
            Label_0007:
            switch (ch)
            {
                case '\t':
                    do
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                    }
                    while (ch == '\t');
                    this.AddTextRun(RunTextType.Tabulation, parseCurrent, this.parseCurrent);
                    goto Label_0196;

                case '\n':
                    ch = this.parseBuffer[++this.parseCurrent];
                    this.AddTextRun(RunTextType.NewLine, parseCurrent, this.parseCurrent);
                    goto Label_0196;

                case '\r':
                    if (this.parseBuffer[this.parseCurrent + 1] == '\n')
                    {
                        this.parseCurrent++;
                        break;
                    }
                    if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(this.parseBuffer[this.parseCurrent + 1])) || (this.endOfFile && ((this.parseCurrent + 1) >= this.parseEnd)))
                    {
                        break;
                    }
                    this.parseThreshold = 2;
                    goto Label_0196;

                case ' ':
                    do
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                    }
                    while (ch == ' ');
                    this.AddTextRun(RunTextType.Space, parseCurrent, this.parseCurrent);
                    goto Label_0196;

                default:
                    do
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                    }
                    while ((ch == '\v') || (ch == '\f'));
                    this.AddTextRun(RunTextType.UnusualWhitespace, parseCurrent, this.parseCurrent);
                    goto Label_0196;
            }
            ch = this.parseBuffer[++this.parseCurrent];
            this.AddTextRun(RunTextType.NewLine, parseCurrent, this.parseCurrent);
            Label_0196:
            charClass = ParseSupport.GetCharClass(ch);
            parseCurrent = this.parseCurrent;
            if ((!ParseSupport.WhitespaceCharacter(charClass) || !this.tokenBuilder.PrepareToAddMoreRuns(4, parseCurrent, RunKind.Text)) || (this.parseThreshold != 1))
            {
                return;
            }
            goto Label_0007;
        }

        void IDisposable.Dispose()
        {
            if (this.input != null)
            {
                this.input.Dispose();
            }
            this.input = null;
            this.parseBuffer = null;
            this.token = null;
            this.tokenBuilder = null;
            GC.SuppressFinalize(this);
        }

        public TextToken Token => this.token;

    }
}

