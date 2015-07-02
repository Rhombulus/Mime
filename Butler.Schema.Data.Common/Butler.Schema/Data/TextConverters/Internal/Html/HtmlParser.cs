// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlParser : IHtmlParser, IRestartable, IReusable, IDisposable
  {
    private int parseThreshold = 1;
    private bool slowParse = true;
    private const int ParseThresholdMax = 16;
    private ConverterInput input;
    private bool endOfFile;
    private bool literalTags;
    private HtmlNameIndex literalTagNameId;
    private bool literalEntities;
    private bool plaintext;
    private HtmlParser.ParseState parseState;
    private char[] parseBuffer;
    private int parseStart;
    private int parseCurrent;
    private int parseEnd;
    private int parseDocumentOffset;
    private char scanQuote;
    private char valueQuote;
    private CharClass lastCharClass;
    private int nameLength;
    private HtmlTokenBuilder tokenBuilder;
    private HtmlToken token;
    private IRestartable restartConsumer;
    private bool detectEncodingFromMetaTag;
    private short[] hashValuesTable;
    private bool rightMeta;
    private Encoding newEncoding;
    private HtmlParser.SavedParserState savedState;

    public HtmlToken Token => this.token;

      public int CurrentOffset => this.parseDocumentOffset;

      public bool ParsingFragment
    {
      get
      {
        if (this.savedState != null)
          return this.savedState.StateSaved;
        return false;
      }
    }

    public HtmlParser(ConverterInput input, bool detectEncodingFromMetaTag, bool preformatedText, int maxRuns, int maxAttrs, bool testBoundaryConditions)
    {
      this.input = input;
      this.detectEncodingFromMetaTag = detectEncodingFromMetaTag;
      input.SetRestartConsumer((IRestartable) this);
      this.tokenBuilder = new HtmlTokenBuilder((char[]) null, maxRuns, maxAttrs, testBoundaryConditions);
      this.token = this.tokenBuilder.Token;
      this.plaintext = preformatedText;
      this.literalEntities = preformatedText;
      this.parseDocumentOffset = 0;
    }

    public void SetRestartConsumer(IRestartable restartConsumer)
    {
      this.restartConsumer = restartConsumer;
    }

    private void ReportProcessed(int processedSize)
    {
      this.input.ReportProcessed(processedSize);
      this.parseDocumentOffset += processedSize;
    }

    private static void ProcessNumericEntityValue(int entityValue, out int literal)
    {
      if (entityValue < 65536)
      {
        if (128 <= entityValue && entityValue <= 159)
          literal = ParseSupport.Latin1MappingInUnicodeControlArea(entityValue);
        else if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass((char) entityValue)))
          literal = 63;
        else
          literal = entityValue;
      }
      else if (entityValue < 1114112)
        literal = entityValue;
      else
        literal = 63;
    }

    private static bool FindEntityByHashName(short hash, char[] buffer, int nameOffset, int nameLength, out int entityValue)
    {
      entityValue = 0;
      bool flag = false;
      HtmlEntityIndex htmlEntityIndex = HtmlNameData.entityHashTable[(int) hash];
      if (htmlEntityIndex > (HtmlEntityIndex) 0)
      {
        do
        {
          if (HtmlNameData.entities[(int) htmlEntityIndex].Name.Length == nameLength)
          {
            int index = 0;
            while (index < nameLength && (int) HtmlNameData.entities[(int) htmlEntityIndex].Name[index] == (int) buffer[nameOffset + index])
              ++index;
            if (index == nameLength)
            {
              entityValue = (int) HtmlNameData.entities[(int) htmlEntityIndex].Value;
              flag = true;
              break;
            }
          }
          ++htmlEntityIndex;
        }
        while ((int) HtmlNameData.entities[(int) htmlEntityIndex].Hash == (int) hash);
      }
      return flag;
    }

    private static string CharsetFromString(string arg, bool lookForWordCharset)
    {
      int num1;
      for (int startIndex = 0; startIndex < arg.Length; startIndex = num1 + 1)
      {
        while (startIndex < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[startIndex])))
          ++startIndex;
        if (startIndex != arg.Length)
        {
          if (!lookForWordCharset || arg.Length - startIndex >= 7 && string.Equals(arg.Substring(startIndex, 7), "charset", StringComparison.OrdinalIgnoreCase))
          {
            if (lookForWordCharset)
            {
              int num2 = arg.IndexOf('=', startIndex + 7);
              if (num2 >= 0)
              {
                startIndex = num2 + 1;
                while (startIndex < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[startIndex])))
                  ++startIndex;
                if (startIndex == arg.Length)
                  break;
              }
              else
                break;
            }
            int index = startIndex;
            while (index < arg.Length && (int) arg[index] != 59 && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[index])))
              ++index;
            return arg.Substring(startIndex, index - startIndex);
          }
          num1 = arg.IndexOf(';', startIndex);
          if (num1 < 0)
            break;
        }
        else
          break;
      }
      return (string) null;
    }

    private void Reinitialize()
    {
      this.endOfFile = false;
      this.literalTags = false;
      this.literalTagNameId = HtmlNameIndex._NOTANAME;
      this.literalEntities = false;
      this.plaintext = false;
      this.parseState = HtmlParser.ParseState.Text;
      this.parseBuffer = (char[]) null;
      this.parseStart = 0;
      this.parseCurrent = 0;
      this.parseEnd = 0;
      this.parseThreshold = 1;
      this.parseDocumentOffset = 0;
      this.slowParse = true;
      this.scanQuote = char.MinValue;
      this.valueQuote = char.MinValue;
      this.lastCharClass = CharClass.Invalid;
      this.nameLength = 0;
      this.tokenBuilder.Reset();
      int num = (int) this.tokenBuilder.MakeEmptyToken(HtmlTokenId.Restart);
    }

    public void PushFragment(ConverterInput fragmentInput, bool literalTextInput)
    {
      if (this.savedState == null)
        this.savedState = new HtmlParser.SavedParserState();
      this.savedState.PushState(this, fragmentInput, literalTextInput);
    }

    public void PopFragment()
    {
      this.savedState.PopState(this);
    }

    bool IRestartable.CanRestart()
    {
      if (this.restartConsumer != null)
        return this.restartConsumer.CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      if (this.restartConsumer != null)
        this.restartConsumer.Restart();
      this.Reinitialize();
    }

    void IRestartable.DisableRestart()
    {
      if (this.restartConsumer == null)
        return;
      this.restartConsumer.DisableRestart();
      this.restartConsumer = (IRestartable) null;
    }

    void IReusable.Initialize(object newSourceOrDestination)
    {
      ((IReusable) this.input).Initialize(newSourceOrDestination);
      this.Reinitialize();
      this.input.SetRestartConsumer((IRestartable) this);
    }

    public void Initialize(string fragment, bool preformatedText)
    {
      (this.input as ConverterBufferInput).Initialize(fragment);
      this.Reinitialize();
      this.plaintext = preformatedText;
      this.literalEntities = preformatedText;
    }

    void IDisposable.Dispose()
    {
      if (this.input != null)
        this.input.Dispose();
      this.input = (ConverterInput) null;
      this.restartConsumer = (IRestartable) null;
      this.parseBuffer = (char[]) null;
      this.token = (HtmlToken) null;
      this.tokenBuilder = (HtmlTokenBuilder) null;
      this.hashValuesTable = (short[]) null;
      GC.SuppressFinalize((object) this);
    }

    public HtmlTokenId Parse()
    {
      if (this.slowParse)
        return this.ParseSlow();
      if (this.tokenBuilder.Valid)
      {
        this.ReportProcessed(this.parseCurrent - this.parseStart);
        this.parseStart = this.parseCurrent;
        this.tokenBuilder.Reset();
      }
      char[] chArray = this.parseBuffer;
      int num1 = this.parseCurrent;
      int num2 = num1;
      bool flag = false;
      int num3;
      char ch1 = chArray[num3 = num1 + 1];
      if ((int) ch1 == 47)
      {
        flag = true;
        ch1 = chArray[++num3];
      }
      if (ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(ch1)))
      {
        this.tokenBuilder.StartTag(HtmlNameIndex.Unknown, num2);
        if (flag)
          this.tokenBuilder.SetEndTag();
        this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num2, num3);
        this.tokenBuilder.StartTagName();
        int nameLength = 0;
        int start1 = num3;
        this.parseState = HtmlParser.ParseState.TagNamePrefix;
        char ch2;
        CharClass charClass1;
        do
        {
          ch2 = chArray[++num3];
          charClass1 = ParseSupport.GetCharClass(ch2);
        }
        while (ParseSupport.HtmlSimpleTagNameCharacter(charClass1));
        if ((int) ch2 == 58)
        {
          this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, start1, num3);
          this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num3, num3 + 1);
          this.tokenBuilder.EndTagNamePrefix();
          nameLength = num3 + 1 - start1;
          start1 = num3 + 1;
          do
          {
            ch2 = chArray[++num3];
            charClass1 = ParseSupport.GetCharClass(ch2);
          }
          while (ParseSupport.HtmlSimpleTagNameCharacter(charClass1));
          this.parseState = HtmlParser.ParseState.TagName;
        }
        if (num3 != start1)
        {
          this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, start1, num3);
          nameLength += num3 - start1;
        }
        if (ParseSupport.HtmlEndTagNameCharacter(charClass1))
        {
          this.tokenBuilder.EndTagName(nameLength);
label_19:
          if (ParseSupport.WhitespaceCharacter(charClass1))
          {
            int start2 = num3;
            do
            {
              ch2 = chArray[++num3];
              charClass1 = ParseSupport.GetCharClass(ch2);
            }
            while (ParseSupport.WhitespaceCharacter(charClass1));
            this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, start2, num3);
          }
          while ((int) ch2 != 62 && ((int) ch2 != 47 || (int) chArray[num3 + 1] != 62))
          {
            this.parseState = HtmlParser.ParseState.TagWsp;
            if (ParseSupport.HtmlSimpleAttrNameCharacter(charClass1) && this.tokenBuilder.CanAddAttribute() && this.tokenBuilder.PrepareToAddMoreRuns(11))
            {
              this.tokenBuilder.StartAttribute();
              nameLength = 0;
              int start2 = num3;
              this.parseState = HtmlParser.ParseState.AttrNamePrefix;
              do
              {
                ch2 = chArray[++num3];
                charClass1 = ParseSupport.GetCharClass(ch2);
              }
              while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass1));
              if ((int) ch2 == 58)
              {
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, start2, num3);
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num3, num3 + 1);
                this.tokenBuilder.EndAttributeNamePrefix();
                nameLength = num3 + 1 - start2;
                start2 = num3 + 1;
                do
                {
                  ch2 = chArray[++num3];
                  charClass1 = ParseSupport.GetCharClass(ch2);
                }
                while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass1));
                this.parseState = HtmlParser.ParseState.AttrName;
              }
              if (num3 != start2)
              {
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, start2, num3);
                nameLength += num3 - start2;
              }
              if (ParseSupport.HtmlEndAttrNameCharacter(charClass1))
              {
                this.tokenBuilder.EndAttributeName(nameLength);
                if (ParseSupport.WhitespaceCharacter(charClass1))
                {
                  int start3 = num3;
                  do
                  {
                    ch2 = chArray[++num3];
                    charClass1 = ParseSupport.GetCharClass(ch2);
                  }
                  while (ParseSupport.WhitespaceCharacter(charClass1));
                  this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, start3, num3);
                  this.parseState = HtmlParser.ParseState.AttrWsp;
                  if (ParseSupport.InvalidUnicodeCharacter(charClass1))
                    goto label_68;
                }
                if ((int) ch2 != 61)
                {
                  this.tokenBuilder.EndAttribute();
                }
                else
                {
                  this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, num3, num3 + 1);
                  char ch3 = chArray[++num3];
                  CharClass charClass2 = ParseSupport.GetCharClass(ch3);
                  if (ParseSupport.WhitespaceCharacter(charClass2))
                  {
                    int start3 = num3;
                    do
                    {
                      ch3 = chArray[++num3];
                      charClass2 = ParseSupport.GetCharClass(ch3);
                    }
                    while (ParseSupport.WhitespaceCharacter(charClass2));
                    this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, start3, num3);
                    this.parseState = HtmlParser.ParseState.AttrValueWsp;
                    if (ParseSupport.InvalidUnicodeCharacter(charClass2))
                      goto label_68;
                  }
                  if (ParseSupport.QuoteCharacter(charClass2))
                  {
                    this.valueQuote = ch3;
                    this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num3, num3 + 1);
                    this.tokenBuilder.StartValue();
                    this.tokenBuilder.SetValueQuote(this.valueQuote);
                    if ((charClass2 & CharClass.GraveAccent) == CharClass.GraveAccent)
                      this.tokenBuilder.SetBackquote();
                    if ((charClass2 & CharClass.Backslash) == CharClass.Backslash)
                      this.tokenBuilder.SetBackslash();
                    char ch4 = chArray[++num3];
                    if (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(ParseSupport.GetCharClass(ch4)))
                    {
                      int start3 = num3;
                      do
                      {
                        ch4 = chArray[++num3];
                      }
                      while (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(ParseSupport.GetCharClass(ch4)));
                      this.tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, start3, num3);
                    }
                    if ((int) ch4 != (int) this.valueQuote)
                    {
                      this.scanQuote = this.valueQuote;
                      this.parseState = HtmlParser.ParseState.AttrValue;
                      goto label_68;
                    }
                    else
                    {
                      this.valueQuote = char.MinValue;
                      this.tokenBuilder.EndValue();
                      this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num3, num3 + 1);
                      ch2 = chArray[++num3];
                      charClass1 = ParseSupport.GetCharClass(ch2);
                      this.tokenBuilder.EndAttribute();
                      goto label_19;
                    }
                  }
                  else if (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass2))
                  {
                    this.tokenBuilder.StartValue();
                    int start3 = num3;
                    do
                    {
                      ch2 = chArray[++num3];
                      charClass1 = ParseSupport.GetCharClass(ch2);
                    }
                    while (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass1));
                    this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrValue, start3, num3);
                    this.parseState = HtmlParser.ParseState.AttrValue;
                    if (ParseSupport.HtmlEndAttrUnquotedValueCharacter(charClass1))
                    {
                      this.tokenBuilder.EndValue();
                      this.tokenBuilder.EndAttribute();
                      goto label_19;
                    }
                    else
                      goto label_68;
                  }
                  else
                  {
                    this.parseState = HtmlParser.ParseState.AttrValueWsp;
                    goto label_68;
                  }
                }
              }
              else
                goto label_68;
            }
            else
              goto label_68;
          }
          int start4 = num3;
          if ((int) ch2 == 47)
          {
            ++num3;
            this.tokenBuilder.SetEmptyScope();
          }
          int end = num3 + 1;
          this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, start4, end);
          this.tokenBuilder.EndTag(true);
          if ((int) chArray[end] == 60)
          {
            this.parseState = HtmlParser.ParseState.TagStart;
          }
          else
          {
            this.parseState = HtmlParser.ParseState.Text;
            this.slowParse = true;
          }
          this.parseCurrent = end;
          this.HandleSpecialTag();
          return this.token.HtmlTokenId;
        }
label_68:
        this.parseCurrent = num3;
        this.lastCharClass = ParseSupport.GetCharClass(chArray[num3 - 1]);
        this.nameLength = nameLength;
      }
      this.slowParse = true;
      return this.ParseSlow();
    }

        public HtmlTokenId ParseSlow()
        {
            bool flag;
            if (this.tokenBuilder.Valid)
            {
                if (this.tokenBuilder.IncompleteTag)
                {
                    int num = this.tokenBuilder.RewindTag();
                    this.ReportProcessed(num - this.parseStart);
                    this.parseStart = num;
                }
                else
                {
                    this.ReportProcessed(this.parseCurrent - this.parseStart);
                    this.parseStart = this.parseCurrent;
                    this.tokenBuilder.Reset();
                }
            }
Label_0067:
            flag = false;
            if ((this.parseCurrent + this.parseThreshold) > this.parseEnd)
            {
                if (!this.endOfFile)
                {
                    if (!this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd))
                    {
                        return HtmlTokenId.None;
                    }
                    this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
                    ConverterDecodingInput input = this.input as ConverterDecodingInput;
                    if ((input != null) && input.EncodingChanged)
                    {
                        input.EncodingChanged = false;
                        return this.tokenBuilder.MakeEmptyToken(HtmlTokenId.EncodingChange, input.Encoding);
                    }
                    if (this.input.EndOfFile)
                    {
                        this.endOfFile = true;
                    }
                    if (!this.endOfFile && ((this.parseEnd - this.parseStart) < this.input.MaxTokenSize))
                    {
                        goto Label_0067;
                    }
                }
                flag = true;
            }
            char ch = this.parseBuffer[this.parseCurrent];
            CharClass charClass = ParseSupport.GetCharClass(ch);
            if (ParseSupport.InvalidUnicodeCharacter(charClass) || (this.parseThreshold > 1))
            {
                bool flag2 = this.SkipInvalidCharacters(ref ch, ref charClass, ref this.parseCurrent);
                if (this.token.IsEmpty)
                {
                    this.ReportProcessed(this.parseCurrent - this.parseStart);
                    this.parseStart = this.parseCurrent;
                    if (this.tokenBuilder.IncompleteTag)
                    {
                        this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
                    }
                }
                if (!flag2)
                {
                    if (!flag)
                    {
                        goto Label_0067;
                    }
                    if (((this.parseCurrent == this.parseEnd) && !this.tokenBuilder.IsStarted) && this.endOfFile)
                    {
                        return this.tokenBuilder.MakeEmptyToken(HtmlTokenId.EndOfFile);
                    }
                }
                this.parseThreshold = 1;
            }
            if (!this.ParseStateMachine(ch, charClass, flag))
            {
                goto Label_0067;
            }
            return this.token.HtmlTokenId;
        }

        //        public bool ParseStateMachine(char ch, CharClass charClass, bool forceFlushToken)
        //    {
        //      HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
        //      char[] chArray = this.parseBuffer;
        //      int parseCurrent = this.parseCurrent;
        //      int end1 = this.parseEnd;
        //      int num1 = parseCurrent;
        //      switch (this.parseState)
        //      {
        //        case HtmlParser.ParseState.Text:
        //          if ((int) ch == 60 && !this.plaintext)
        //          {
        //            this.parseState = HtmlParser.ParseState.TagStart;
        //            goto case 1;
        //          }
        //          else
        //            break;
        //        case HtmlParser.ParseState.TagStart:
        //          char ch1 = chArray[parseCurrent + 1];
        //          CharClass charClass1 = ParseSupport.GetCharClass(ch1);
        //          bool flag = false;
        //          if ((int) ch1 == 47)
        //          {
        //            ch1 = chArray[parseCurrent + 2];
        //            charClass1 = ParseSupport.GetCharClass(ch1);
        //            if (ParseSupport.InvalidUnicodeCharacter(charClass1) && (!this.endOfFile || parseCurrent + 2 < end1))
        //            {
        //              this.parseThreshold = 3;
        //              goto label_169;
        //            }
        //            else
        //            {
        //              ++parseCurrent;
        //              flag = true;
        //            }
        //          }
        //          else if (!ParseSupport.AlphaCharacter(charClass1) || this.literalTags)
        //          {
        //            if ((int) ch1 == 33)
        //            {
        //              this.parseState = HtmlParser.ParseState.CommentStart;
        //              goto case 14;
        //            }
        //            else if ((int) ch1 == 63 && !this.literalTags)
        //            {
        //              parseCurrent += 2;
        //              htmlTokenBuilder.StartTag(HtmlNameIndex._DTD, num1);
        //              htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num1, parseCurrent);
        //              htmlTokenBuilder.StartTagText();
        //              this.lastCharClass = charClass1;
        //              ch = chArray[parseCurrent];
        //              charClass = ParseSupport.GetCharClass(ch);
        //              num1 = parseCurrent;
        //              this.parseState = HtmlParser.ParseState.Dtd;
        //              goto case 15;
        //            }
        //            else if ((int) ch1 == 37)
        //            {
        //              parseCurrent += 2;
        //              htmlTokenBuilder.StartTag(HtmlNameIndex._ASP, num1);
        //              htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num1, parseCurrent);
        //              htmlTokenBuilder.StartTagText();
        //              ch = chArray[parseCurrent];
        //              charClass = ParseSupport.GetCharClass(ch);
        //              num1 = parseCurrent;
        //              this.parseState = HtmlParser.ParseState.Asp;
        //              goto case 15;
        //            }
        //            else if (ParseSupport.InvalidUnicodeCharacter(charClass1) && (!this.endOfFile || parseCurrent + 1 < end1))
        //            {
        //              this.parseThreshold = 2;
        //              goto label_169;
        //            }
        //            else
        //            {
        //              this.parseState = HtmlParser.ParseState.Text;
        //              break;
        //            }
        //          }
        //          ++parseCurrent;
        //          this.lastCharClass = charClass;
        //          ch = ch1;
        //          charClass = charClass1;
        //          htmlTokenBuilder.StartTag(HtmlNameIndex.Unknown, num1);
        //          if (flag)
        //            htmlTokenBuilder.SetEndTag();
        //          htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num1, parseCurrent);
        //          this.nameLength = 0;
        //          htmlTokenBuilder.StartTagName();
        //          num1 = parseCurrent;
        //          this.parseState = HtmlParser.ParseState.TagNamePrefix;
        //          goto case 2;
        //        case HtmlParser.ParseState.TagNamePrefix:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(2, num1, HtmlRunKind.Name))
        //          {
        //            ch = this.ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagNamePrefix);
        //            if (parseCurrent != num1)
        //            {
        //              this.nameLength += parseCurrent - num1;
        //              if (!this.literalTags || this.nameLength <= 14 && (int) ch != 60)
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num1, parseCurrent);
        //              else
        //                goto label_127;
        //            }
        //            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
        //            {
        //              if ((int) ch == 58)
        //              {
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
        //                ++this.nameLength;
        //                this.tokenBuilder.EndTagNamePrefix();
        //                ch = chArray[++parseCurrent];
        //                charClass = ParseSupport.GetCharClass(ch);
        //                num1 = parseCurrent;
        //                this.parseState = HtmlParser.ParseState.TagName;
        //                goto case 3;
        //              }
        //              else
        //                goto label_35;
        //            }
        //            else
        //              goto label_118;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.TagName:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(1, num1, HtmlRunKind.Name))
        //          {
        //            ch = this.ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagName);
        //            if (parseCurrent != num1)
        //            {
        //              this.nameLength += parseCurrent - num1;
        //              if (!this.literalTags || this.nameLength <= 14 && (int) ch != 60)
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num1, parseCurrent);
        //              else
        //                goto label_127;
        //            }
        //            if (ParseSupport.InvalidUnicodeCharacter(charClass))
        //              goto label_118;
        //            else
        //              goto label_35;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.TagWsp:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(2, num1, HtmlRunKind.TagWhitespace))
        //          {
        //            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);
        //            if (parseCurrent != num1)
        //              htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num1, parseCurrent);
        //            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
        //            {
        //              num1 = parseCurrent;
        //              if ((int) ch == 62)
        //              {
        //                this.parseState = HtmlParser.ParseState.TagEnd;
        //                goto case 12;
        //              }
        //              else if ((int) ch == 47)
        //              {
        //                this.parseState = HtmlParser.ParseState.EmptyTagEnd;
        //                goto case 11;
        //              }
        //              else
        //              {
        //                this.parseState = HtmlParser.ParseState.AttrNameStart;
        //                goto case 5;
        //              }
        //            }
        //            else
        //              goto label_118;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.AttrNameStart:
        //          if (htmlTokenBuilder.CanAddAttribute() && htmlTokenBuilder.PrepareToAddMoreRuns(3, num1, HtmlRunKind.Name))
        //          {
        //            this.nameLength = 0;
        //            htmlTokenBuilder.StartAttribute();
        //            this.parseState = HtmlParser.ParseState.AttrNamePrefix;
        //            goto case 6;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.AttrNamePrefix:
        //              if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num1, HtmlRunKind.Name))
        //                  goto label_125;
        //              ch = this.ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrNamePrefix);
        //              if (parseCurrent != num1) {
        //                  this.nameLength += parseCurrent - num1;
        //                  htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num1, parseCurrent);
        //              }
        //              if (ParseSupport.InvalidUnicodeCharacter(charClass))
        //                  goto label_118;
        //              if ( ch != ':')
        //                  goto label_62;
        //              htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
        //              ++this.nameLength;
        //              this.tokenBuilder.EndAttributeNamePrefix();
        //              ch = chArray[++parseCurrent];
        //              charClass = ParseSupport.GetCharClass(ch);
        //              num1 = parseCurrent;
        //              this.parseState = HtmlParser.ParseState.AttrName;
        //              goto case ParseState.AttrName;
        //          case HtmlParser.ParseState.AttrName:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(2, num1, HtmlRunKind.Name))
        //          {
        //            ch = this.ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrName);
        //            if (parseCurrent != num1)
        //            {
        //              this.nameLength += parseCurrent - num1;
        //              htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num1, parseCurrent);
        //            }
        //            if (ParseSupport.InvalidUnicodeCharacter(charClass))
        //              goto label_118;
        //            else
        //              goto label_62;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.AttrWsp:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(2, num1, HtmlRunKind.TagWhitespace))
        //          {
        //            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);
        //            if (parseCurrent != num1)
        //              htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num1, parseCurrent);
        //            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
        //            {
        //              num1 = parseCurrent;
        //              if ((int) ch != 61)
        //              {
        //                htmlTokenBuilder.EndAttribute();
        //                if ((int) ch == 62)
        //                {
        //                  this.parseState = HtmlParser.ParseState.TagEnd;
        //                  goto case 12;
        //                }
        //                else if ((int) ch == 47)
        //                {
        //                  this.parseState = HtmlParser.ParseState.EmptyTagEnd;
        //                  goto case 11;
        //                }
        //                else
        //                {
        //                  this.parseState = HtmlParser.ParseState.AttrNameStart;
        //                  goto case 5;
        //                }
        //              }
        //              else
        //                goto label_74;
        //            }
        //            else
        //              goto label_118;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.AttrValueWsp:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(3, num1, HtmlRunKind.TagWhitespace))
        //          {
        //            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);
        //            if (parseCurrent != num1)
        //              htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num1, parseCurrent);
        //            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
        //            {
        //              num1 = parseCurrent;
        //              if (ParseSupport.QuoteCharacter(charClass))
        //              {
        //                if ((int) ch == (int) this.scanQuote)
        //                  this.scanQuote = char.MinValue;
        //                else if ((int) this.scanQuote == 0)
        //                  this.scanQuote = ch;
        //                this.valueQuote = ch;
        //                this.lastCharClass = charClass;
        //                ch = chArray[++parseCurrent];
        //                charClass = ParseSupport.GetCharClass(ch);
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num1, parseCurrent);
        //                num1 = parseCurrent;
        //              }
        //              htmlTokenBuilder.StartValue();
        //              if ((int) this.valueQuote != 0)
        //                htmlTokenBuilder.SetValueQuote(this.valueQuote);
        //              this.parseState = HtmlParser.ParseState.AttrValue;
        //              goto case 10;
        //            }
        //            else
        //              goto label_118;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.AttrValue:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(2, num1, HtmlRunKind.AttrValue) && this.ParseAttributeText(ch, charClass, ref parseCurrent))
        //          {
        //            ch = chArray[parseCurrent];
        //            charClass = ParseSupport.GetCharClass(ch);
        //            if (!ParseSupport.InvalidUnicodeCharacter(charClass) && this.parseThreshold <= 1)
        //            {
        //              htmlTokenBuilder.EndValue();
        //              num1 = parseCurrent;
        //              if ((int) ch == (int) this.valueQuote)
        //              {
        //                this.lastCharClass = charClass;
        //                ch = chArray[++parseCurrent];
        //                charClass = ParseSupport.GetCharClass(ch);
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num1, parseCurrent);
        //                this.valueQuote = char.MinValue;
        //                num1 = parseCurrent;
        //              }
        //              htmlTokenBuilder.EndAttribute();
        //              if ((int) ch == 62)
        //              {
        //                this.parseState = HtmlParser.ParseState.TagEnd;
        //                goto case 12;
        //              }
        //              else if ((int) ch == 47)
        //              {
        //                this.parseState = HtmlParser.ParseState.EmptyTagEnd;
        //                goto case 11;
        //              }
        //              else
        //              {
        //                this.parseState = HtmlParser.ParseState.TagWsp;
        //                goto case 4;
        //              }
        //            }
        //            else
        //              goto label_118;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.EmptyTagEnd:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(1, num1, HtmlRunKind.TagWhitespace))
        //          {
        //            char ch2 = chArray[parseCurrent + 1];
        //            CharClass charClass2 = ParseSupport.GetCharClass(ch2);
        //            if ((int) ch2 == 62)
        //            {
        //              htmlTokenBuilder.SetEmptyScope();
        //              ++parseCurrent;
        //              this.lastCharClass = charClass;
        //              ch = ch2;
        //              charClass = charClass2;
        //              this.parseState = HtmlParser.ParseState.TagEnd;
        //              goto case 12;
        //            }
        //            else if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!this.endOfFile || parseCurrent + 1 < end1))
        //            {
        //              this.parseThreshold = 2;
        //              goto label_118;
        //            }
        //            else
        //            {
        //              this.lastCharClass = charClass;
        //              ++parseCurrent;
        //              ch = ch2;
        //              charClass = charClass2;
        //              num1 = parseCurrent;
        //              this.parseState = HtmlParser.ParseState.TagWsp;
        //              goto case 4;
        //            }
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.TagEnd:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(1, num1, HtmlRunKind.TagSuffix))
        //          {
        //            this.lastCharClass = charClass;
        //            ++parseCurrent;
        //            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num1, parseCurrent);
        //            if ((int) this.scanQuote != 0)
        //            {
        //              num1 = parseCurrent;
        //              ch = chArray[parseCurrent];
        //              charClass = ParseSupport.GetCharClass(ch);
        //              this.parseState = HtmlParser.ParseState.TagSkip;
        //              goto case 13;
        //            }
        //            else
        //            {
        //              htmlTokenBuilder.EndTag(true);
        //              if ((int) chArray[parseCurrent] == 60)
        //              {
        //                this.parseState = HtmlParser.ParseState.TagStart;
        //                this.slowParse = false;
        //              }
        //              else
        //                this.parseState = HtmlParser.ParseState.Text;
        //              this.parseCurrent = parseCurrent;
        //              this.HandleSpecialTag();
        //              return true;
        //            }
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.TagSkip:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(1, num1, HtmlRunKind.TagText))
        //          {
        //            ch = this.ScanSkipTag(ch, ref charClass, ref parseCurrent);
        //            if (parseCurrent != num1)
        //              htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num1, parseCurrent);
        //            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
        //            {
        //              int index = parseCurrent + 1;
        //              htmlTokenBuilder.EndTag(true);
        //              if ((int) chArray[index] == 60)
        //              {
        //                this.parseState = HtmlParser.ParseState.TagStart;
        //                this.slowParse = false;
        //              }
        //              else
        //                this.parseState = HtmlParser.ParseState.Text;
        //              this.parseCurrent = index;
        //              this.HandleSpecialTag();
        //              return true;
        //            }
        //            goto label_118;
        //          }
        //          else
        //            goto label_125;
        //        case HtmlParser.ParseState.CommentStart:
        //          int num2 = 2;
        //          char ch3 = chArray[parseCurrent + num2];
        //          if ((int) ch3 == 45)
        //          {
        //            ++num2;
        //            ch3 = chArray[parseCurrent + num2];
        //            if ((int) ch3 == 45)
        //            {
        //              ++num2;
        //              ch3 = chArray[parseCurrent + num2];
        //              if ((int) ch3 == 62)
        //              {
        //                int end2 = parseCurrent + 5;
        //                htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num1);
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num1, end2 - 1);
        //                htmlTokenBuilder.StartTagText();
        //                htmlTokenBuilder.EndTagText();
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, end2 - 1, end2);
        //                htmlTokenBuilder.EndTag(true);
        //                this.parseState = HtmlParser.ParseState.Text;
        //                this.parseCurrent = end2;
        //                return true;
        //              }
        //              if ((int) ch3 == 45)
        //              {
        //                ++num2;
        //                ch3 = chArray[parseCurrent + num2];
        //                if ((int) ch3 == 62)
        //                {
        //                  int end2 = parseCurrent + 6;
        //                  htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num1);
        //                  htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num1, end2 - 2);
        //                  htmlTokenBuilder.StartTagText();
        //                  htmlTokenBuilder.EndTagText();
        //                  htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, end2 - 2, end2);
        //                  htmlTokenBuilder.EndTag(true);
        //                  this.parseState = HtmlParser.ParseState.Text;
        //                  this.parseCurrent = end2;
        //                  return true;
        //                }
        //              }
        //              if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch3)))
        //              {
        //                parseCurrent += 4;
        //                htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num1);
        //                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num1, parseCurrent);
        //                htmlTokenBuilder.StartTagText();
        //                ch = chArray[parseCurrent];
        //                charClass = ParseSupport.GetCharClass(ch);
        //                num1 = parseCurrent;
        //                this.parseState = HtmlParser.ParseState.Comment;
        //                goto case 15;
        //              }
        //            }
        //          }
        //          if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch3)))
        //          {
        //            if (!this.endOfFile || parseCurrent + num2 < end1)
        //            {
        //              this.parseThreshold = num2 + 1;
        //              goto label_169;
        //            }
        //            else
        //            {
        //              this.parseState = HtmlParser.ParseState.Text;
        //              break;
        //            }
        //          }
        //          else
        //          {
        //            if (this.literalTags)
        //            {
        //              this.parseState = HtmlParser.ParseState.Text;
        //              break;
        //            }
        //            parseCurrent += 2;
        //            htmlTokenBuilder.StartTag(HtmlNameIndex._BANG, num1);
        //            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num1, parseCurrent);
        //            htmlTokenBuilder.StartTagText();
        //            this.lastCharClass = ParseSupport.GetCharClass('!');
        //            ch = chArray[parseCurrent];
        //            charClass = ParseSupport.GetCharClass(ch);
        //            num1 = parseCurrent;
        //            this.parseState = HtmlParser.ParseState.Bang;
        //            goto case 15;
        //          }
        //        case HtmlParser.ParseState.Comment:
        //        case HtmlParser.ParseState.Conditional:
        //        case HtmlParser.ParseState.CommentConditional:
        //        case HtmlParser.ParseState.Bang:
        //        case HtmlParser.ParseState.Dtd:
        //        case HtmlParser.ParseState.Asp:
        //          if (htmlTokenBuilder.PrepareToAddMoreRuns(2, num1, HtmlRunKind.TagText))
        //          {
        //            while (!ParseSupport.InvalidUnicodeCharacter(charClass))
        //            {
        //              if (ParseSupport.QuoteCharacter(charClass))
        //              {
        //                if ((int) ch == (int) this.scanQuote)
        //                  this.scanQuote = char.MinValue;
        //                else if ((int) this.scanQuote == 0 && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
        //                  this.scanQuote = ch;
        //              }
        //              else if (ParseSupport.HtmlSuffixCharacter(charClass))
        //              {
        //                int addToTextCnt;
        //                int tagSuffixCnt;
        //                bool endScan;
        //                if (this.CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan))
        //                {
        //                  if (!endScan)
        //                  {
        //                    parseCurrent += addToTextCnt;
        //                    this.lastCharClass = charClass;
        //                    ch = chArray[parseCurrent];
        //                    charClass = ParseSupport.GetCharClass(ch);
        //                    continue;
        //                  }
        //                  this.scanQuote = char.MinValue;
        //                  int end2 = parseCurrent + addToTextCnt;
        //                  if (end2 != num1)
        //                    htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num1, end2);
        //                  htmlTokenBuilder.EndTagText();
        //                  if (tagSuffixCnt != 0)
        //                  {
        //                    int start = end2;
        //                    end2 += tagSuffixCnt;
        //                    htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, start, end2);
        //                  }
        //                  htmlTokenBuilder.EndTag(true);
        //                  this.parseState = HtmlParser.ParseState.Text;
        //                  this.parseCurrent = end2;
        //                  return true;
        //                }
        //                parseCurrent += addToTextCnt;
        //                this.parseThreshold = tagSuffixCnt + 1;
        //                break;
        //              }
        //              this.lastCharClass = charClass;
        //              ch = chArray[++parseCurrent];
        //              charClass = ParseSupport.GetCharClass(ch);
        //            }
        //            if (parseCurrent != num1)
        //            {
        //              htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num1, parseCurrent);
        //              if (!htmlTokenBuilder.PrepareToAddMoreRuns(2))
        //                goto label_125;
        //            }
        //            if (forceFlushToken && parseCurrent + this.parseThreshold > end1)
        //            {
        //              if (this.endOfFile && parseCurrent < end1)
        //              {
        //                htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, parseCurrent, end1);
        //                parseCurrent = end1;
        //              }
        //              htmlTokenBuilder.EndTag(this.endOfFile);
        //              this.parseCurrent = parseCurrent;
        //              return true;
        //            }
        //            goto label_169;
        //          }
        //          else
        //            goto label_125;
        //        default:
        //          this.parseCurrent = parseCurrent;
        //          throw new TextConvertersException("internal error: invalid parse state");
        //      }
        //label_3:
        //      htmlTokenBuilder.StartText(num1);
        //      this.ParseText(ch, charClass, ref parseCurrent);
        //      if (this.token.IsEmpty && !forceFlushToken)
        //      {
        //        htmlTokenBuilder.Reset();
        //        this.slowParse = true;
        //        goto label_169;
        //      }
        //      else
        //      {
        //        htmlTokenBuilder.EndText();
        //        this.parseCurrent = parseCurrent;
        //        return true;
        //      }
        //label_35:
        //      htmlTokenBuilder.EndTagName(this.nameLength);
        //      if (!this.literalTags || this.token.NameIndex == this.literalTagNameId)
        //      {
        //        num1 = parseCurrent;
        //        if ((int) ch == 62)
        //        {
        //          this.parseState = HtmlParser.ParseState.TagEnd;
        //          goto case 12;
        //        }
        //        else if ((int) ch == 47)
        //        {
        //          this.parseState = HtmlParser.ParseState.EmptyTagEnd;
        //          goto case 11;
        //        }
        //        else
        //        {
        //          this.lastCharClass = charClass;
        //          this.parseState = HtmlParser.ParseState.TagWsp;
        //          goto case 4;
        //        }
        //      }
        //      else
        //        goto label_127;
        //label_62:
        //      htmlTokenBuilder.EndAttributeName(this.nameLength);
        //      num1 = parseCurrent;
        //      if ((int) ch != 61)
        //      {
        //        this.lastCharClass = charClass;
        //        this.parseState = HtmlParser.ParseState.AttrWsp;
        //        goto case 8;
        //      }
        //label_74:
        //      this.lastCharClass = charClass;
        //      ch = chArray[++parseCurrent];
        //      charClass = ParseSupport.GetCharClass(ch);
        //      htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, num1, parseCurrent);
        //      num1 = parseCurrent;
        //      this.parseState = HtmlParser.ParseState.AttrValueWsp;
        //      goto case 9;
        //label_118:
        //      if (forceFlushToken && parseCurrent + this.parseThreshold >= end1)
        //      {
        //        if (this.endOfFile)
        //        {
        //          if (parseCurrent < end1)
        //          {
        //            if (!this.ScanForInternalInvalidCharacters(parseCurrent))
        //              parseCurrent = end1;
        //            else
        //              goto label_169;
        //          }
        //          if (!this.token.IsTagBegin)
        //          {
        //            htmlTokenBuilder.EndTag(true);
        //            this.parseCurrent = parseCurrent;
        //            this.HandleSpecialTag();
        //            this.parseState = HtmlParser.ParseState.Text;
        //            return true;
        //          }
        //          goto label_127;
        //        }
        //      }
        //      else
        //        goto label_169;
        //label_125:
        //      if (!this.literalTags || this.token.NameIndex != HtmlNameIndex.Unknown)
        //      {
        //        htmlTokenBuilder.EndTag(false);
        //        this.parseCurrent = parseCurrent;
        //        this.HandleSpecialTag();
        //        return true;
        //      }
        //label_127:
        //      parseCurrent = this.parseStart;
        //      this.scanQuote = this.valueQuote = char.MinValue;
        //      htmlTokenBuilder.Reset();
        //      num1 = parseCurrent;
        //      ch = chArray[parseCurrent];
        //      charClass = ParseSupport.GetCharClass(ch);
        //      this.parseState = HtmlParser.ParseState.Text;
        //      goto label_3;
        //label_169:
        //      this.parseCurrent = parseCurrent;
        //      return false;
        //    }

        public bool ParseStateMachine(char ch, CharClass charClass, bool forceFlushToken)
        {
            char ch2;
            int num4;
            HtmlTokenBuilder tokenBuilder = this.tokenBuilder;
            char[] parseBuffer = this.parseBuffer;
            int parseCurrent = this.parseCurrent;
            int parseEnd = this.parseEnd;
            int baseOffset = parseCurrent;
            switch (this.parseState)
            {
                case ParseState.Text:
                    if ((ch != '<') || this.plaintext)
                    {
                        break;
                    }
                    this.parseState = ParseState.TagStart;
                    goto case ParseState.TagStart;

                case ParseState.TagStart:
                    goto Label_00E4;

                case ParseState.TagNamePrefix:
                    goto Label_028A;

                case ParseState.TagName:
                    goto Label_0358;

                case ParseState.TagWsp:
                    goto Label_0431;

                case ParseState.AttrNameStart:
                    goto Label_04A5;

                case ParseState.AttrNamePrefix:
                    goto Label_04D7;

                case ParseState.AttrName:
                    goto Label_0588;

                case ParseState.AttrWsp:
                    goto Label_060A;

                case ParseState.AttrValueWsp:
                    goto Label_06C8;

                case ParseState.AttrValue:
                    goto Label_0795;

                case ParseState.EmptyTagEnd:
                    goto Label_085B;

                case ParseState.TagEnd:
                    goto Label_08EB;

                case ParseState.TagSkip:
                    goto Label_0979;

                case ParseState.CommentStart:
                    goto Label_0ABE;

                case ParseState.Comment:
                case ParseState.Conditional:
                case ParseState.CommentConditional:
                case ParseState.Bang:
                case ParseState.Dtd:
                case ParseState.Asp:
                    goto Label_0CC6;

                default:
                    this.parseCurrent = parseCurrent;
                    throw new TextConvertersException("internal error: invalid parse state");
            }
Label_00A0:
            tokenBuilder.StartText(baseOffset);
            this.ParseText(ch, charClass, ref parseCurrent);
            if (this.token.IsEmpty && !forceFlushToken)
            {
                tokenBuilder.Reset();
                this.slowParse = true;
                goto Label_0E80;
            }
            tokenBuilder.EndText();
            this.parseCurrent = parseCurrent;
            return true;
Label_00E4:
            ch2 = parseBuffer[parseCurrent + 1];
            CharClass class2 = ParseSupport.GetCharClass(ch2);
            bool flag = false;
            if (ch2 == '/')
            {
                ch2 = parseBuffer[parseCurrent + 2];
                class2 = ParseSupport.GetCharClass(ch2);
                if (ParseSupport.InvalidUnicodeCharacter(class2) && (!this.endOfFile || ((parseCurrent + 2) < parseEnd)))
                {
                    this.parseThreshold = 3;
                    goto Label_0E80;
                }
                parseCurrent++;
                flag = true;
            }
            else if (!ParseSupport.AlphaCharacter(class2) || this.literalTags)
            {
                if (ch2 == '!')
                {
                    this.parseState = ParseState.CommentStart;
                    goto Label_0ABE;
                }
                if ((ch2 == '?') && !this.literalTags)
                {
                    parseCurrent += 2;
                    tokenBuilder.StartTag(HtmlNameIndex._DTD, baseOffset);
                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, baseOffset, parseCurrent);
                    tokenBuilder.StartTagText();
                    this.lastCharClass = class2;
                    ch = parseBuffer[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    baseOffset = parseCurrent;
                    this.parseState = ParseState.Dtd;
                    goto Label_0CC6;
                }
                if (ch2 == '%')
                {
                    parseCurrent += 2;
                    tokenBuilder.StartTag(HtmlNameIndex._ASP, baseOffset);
                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, baseOffset, parseCurrent);
                    tokenBuilder.StartTagText();
                    ch = parseBuffer[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    baseOffset = parseCurrent;
                    this.parseState = ParseState.Asp;
                    goto Label_0CC6;
                }
                if (ParseSupport.InvalidUnicodeCharacter(class2) && (!this.endOfFile || ((parseCurrent + 1) < parseEnd)))
                {
                    this.parseThreshold = 2;
                    goto Label_0E80;
                }
                this.parseState = ParseState.Text;
                goto Label_00A0;
            }
            parseCurrent++;
            this.lastCharClass = charClass;
            ch = ch2;
            charClass = class2;
            tokenBuilder.StartTag(HtmlNameIndex.Unknown, baseOffset);
            if (flag)
            {
                tokenBuilder.SetEndTag();
            }
            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, baseOffset, parseCurrent);
            this.nameLength = 0;
            tokenBuilder.StartTagName();
            baseOffset = parseCurrent;
            this.parseState = ParseState.TagNamePrefix;
            goto Label_028A;
Label_028A:
            if (!tokenBuilder.PrepareToAddMoreRuns(2, baseOffset, HtmlRunKind.Name))
            {
                goto Label_0A53;
            }
            ch = this.ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagNamePrefix);
            if (parseCurrent != baseOffset)
            {
                this.nameLength += parseCurrent - baseOffset;
                if (this.literalTags && ((this.nameLength > 14) || (ch == '<')))
                {
                    goto Label_0A80;
                }
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, baseOffset, parseCurrent);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto Label_09F3;
            }
            if (ch != ':')
            {
                goto Label_03D1;
            }
            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
            this.nameLength++;
            this.tokenBuilder.EndTagNamePrefix();
            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            baseOffset = parseCurrent;
            this.parseState = ParseState.TagName;
            goto Label_0358;
Label_0358:
            if (!tokenBuilder.PrepareToAddMoreRuns(1, baseOffset, HtmlRunKind.Name))
            {
                goto Label_0A53;
            }
            ch = this.ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagName);
            if (parseCurrent != baseOffset)
            {
                this.nameLength += parseCurrent - baseOffset;
                if (this.literalTags && ((this.nameLength > 14) || (ch == '<')))
                {
                    goto Label_0A80;
                }
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, baseOffset, parseCurrent);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto Label_09F3;
            }
            goto Label_03D1;
Label_03D1:
            tokenBuilder.EndTagName(this.nameLength);
            if (this.literalTags && (this.token.NameIndex != this.literalTagNameId))
            {
                goto Label_0A80;
            }
            baseOffset = parseCurrent;
            if (ch == '>')
            {
                this.parseState = ParseState.TagEnd;
                goto Label_08EB;
            }
            if (ch == '/')
            {
                this.parseState = ParseState.EmptyTagEnd;
                goto Label_085B;
            }
            this.lastCharClass = charClass;
            this.parseState = ParseState.TagWsp;
goto Label_0431;
Label_0431:
            if (!tokenBuilder.PrepareToAddMoreRuns(2, baseOffset, HtmlRunKind.TagWhitespace))
            {
                goto Label_0A53;
            }
            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);
            if (parseCurrent != baseOffset)
            {
                tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, baseOffset, parseCurrent);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto Label_09F3;
            }
            baseOffset = parseCurrent;
            if (ch == '>')
            {
                this.parseState = ParseState.TagEnd;
                goto Label_08EB;
            }
            if (ch == '/')
            {
                this.parseState = ParseState.EmptyTagEnd;
                goto Label_085B;
            }
            this.parseState = ParseState.AttrNameStart;
goto Label_04A5;
Label_04A5:
            if (!tokenBuilder.CanAddAttribute() || !tokenBuilder.PrepareToAddMoreRuns(3, baseOffset, HtmlRunKind.Name))
            {
                goto Label_0A53;
            }
            this.nameLength = 0;
            tokenBuilder.StartAttribute();
            this.parseState = ParseState.AttrNamePrefix;
goto Label_04D7;
Label_04D7:
            if (!tokenBuilder.PrepareToAddMoreRuns(3, baseOffset, HtmlRunKind.Name))
            {
                goto Label_0A53;
            }
            ch = this.ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrNamePrefix);
            if (parseCurrent != baseOffset)
            {
                this.nameLength += parseCurrent - baseOffset;
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, baseOffset, parseCurrent);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto Label_09F3;
            }
            if (ch != ':')
            {
                goto Label_05E4;
            }
            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
            this.nameLength++;
            this.tokenBuilder.EndAttributeNamePrefix();
            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            baseOffset = parseCurrent;
            this.parseState = ParseState.AttrName;
goto Label_0588;
Label_0588:
            if (!tokenBuilder.PrepareToAddMoreRuns(2, baseOffset, HtmlRunKind.Name))
            {
                goto Label_0A53;
            }
            ch = this.ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrName);
            if (parseCurrent != baseOffset)
            {
                this.nameLength += parseCurrent - baseOffset;
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, baseOffset, parseCurrent);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto Label_09F3;
            }
goto Label_05E4;
Label_05E4:
            tokenBuilder.EndAttributeName(this.nameLength);
            baseOffset = parseCurrent;
            if (ch == '=')
            {
                goto Label_068E;
            }
            this.lastCharClass = charClass;
            this.parseState = ParseState.AttrWsp;
Label_060A:
            if (!tokenBuilder.PrepareToAddMoreRuns(2, baseOffset, HtmlRunKind.TagWhitespace))
            {
                goto Label_0A53;
            }
            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);
            if (parseCurrent != baseOffset)
            {
                tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, baseOffset, parseCurrent);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto Label_09F3;
            }
            baseOffset = parseCurrent;
            if (ch != '=')
            {
                tokenBuilder.EndAttribute();
                if (ch == '>')
                {
                    this.parseState = ParseState.TagEnd;
                    goto Label_08EB;
                }
                if (ch == '/')
                {
                    this.parseState = ParseState.EmptyTagEnd;
                    goto Label_085B;
                }
                this.parseState = ParseState.AttrNameStart;
                goto Label_04A5;
            }
Label_068E:
            this.lastCharClass = charClass;
            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, baseOffset, parseCurrent);
            baseOffset = parseCurrent;
            this.parseState = ParseState.AttrValueWsp;
Label_06C8:
            if (!tokenBuilder.PrepareToAddMoreRuns(3, baseOffset, HtmlRunKind.TagWhitespace))
            {
                goto Label_0A53;
            }
            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);
            if (parseCurrent != baseOffset)
            {
                tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, baseOffset, parseCurrent);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto Label_09F3;
            }
            baseOffset = parseCurrent;
            if (ParseSupport.QuoteCharacter(charClass))
            {
                if (ch == this.scanQuote)
                {
                    this.scanQuote = '\0';
                }
                else if (this.scanQuote == '\0')
                {
                    this.scanQuote = ch;
                }
                this.valueQuote = ch;
                this.lastCharClass = charClass;
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, baseOffset, parseCurrent);
                baseOffset = parseCurrent;
            }
            tokenBuilder.StartValue();
            if (this.valueQuote != '\0')
            {
                tokenBuilder.SetValueQuote(this.valueQuote);
            }
            this.parseState = ParseState.AttrValue;
Label_0795:
            if (!tokenBuilder.PrepareToAddMoreRuns(2, baseOffset, HtmlRunKind.AttrValue) || !this.ParseAttributeText(ch, charClass, ref parseCurrent))
            {
                goto Label_0A53;
            }
            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            if (ParseSupport.InvalidUnicodeCharacter(charClass) || (this.parseThreshold > 1))
            {
                goto Label_09F3;
            }
            tokenBuilder.EndValue();
            baseOffset = parseCurrent;
            if (ch == this.valueQuote)
            {
                this.lastCharClass = charClass;
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, baseOffset, parseCurrent);
                this.valueQuote = '\0';
                baseOffset = parseCurrent;
            }
            tokenBuilder.EndAttribute();
            if (ch == '>')
            {
                this.parseState = ParseState.TagEnd;
                goto Label_08EB;
            }
            if (ch == '/')
            {
                this.parseState = ParseState.EmptyTagEnd;
            }
            else
            {
                this.parseState = ParseState.TagWsp;
                goto Label_0431;
            }
Label_085B:
            if (!tokenBuilder.PrepareToAddMoreRuns(1, baseOffset, HtmlRunKind.TagWhitespace))
            {
                goto Label_0A53;
            }
            ch2 = parseBuffer[parseCurrent + 1];
            class2 = ParseSupport.GetCharClass(ch2);
            if (ch2 == '>')
            {
                tokenBuilder.SetEmptyScope();
                parseCurrent++;
                this.lastCharClass = charClass;
                ch = ch2;
                charClass = class2;
                this.parseState = ParseState.TagEnd;
            }
            else
            {
                if (ParseSupport.InvalidUnicodeCharacter(class2) && (!this.endOfFile || ((parseCurrent + 1) < parseEnd)))
                {
                    this.parseThreshold = 2;
                    goto Label_09F3;
                }
                this.lastCharClass = charClass;
                parseCurrent++;
                ch = ch2;
                charClass = class2;
                baseOffset = parseCurrent;
                this.parseState = ParseState.TagWsp;
                goto Label_0431;
            }
Label_08EB:
            if (!tokenBuilder.PrepareToAddMoreRuns(1, baseOffset, HtmlRunKind.TagSuffix))
            {
                goto Label_0A53;
            }
            this.lastCharClass = charClass;
            parseCurrent++;
            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, baseOffset, parseCurrent);
            if (this.scanQuote != '\0')
            {
                baseOffset = parseCurrent;
                ch = parseBuffer[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                this.parseState = ParseState.TagSkip;
            }
            else
            {
                tokenBuilder.EndTag(true);
                if (parseBuffer[parseCurrent] == '<')
                {
                    this.parseState = ParseState.TagStart;
                    this.slowParse = false;
                }
                else
                {
                    this.parseState = ParseState.Text;
                }
                this.parseCurrent = parseCurrent;
                this.HandleSpecialTag();
                return true;
            }
Label_0979:
            if (!tokenBuilder.PrepareToAddMoreRuns(1, baseOffset, HtmlRunKind.TagText))
            {
                goto Label_0A53;
            }
            ch = this.ScanSkipTag(ch, ref charClass, ref parseCurrent);
            if (parseCurrent != baseOffset)
            {
                tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, baseOffset, parseCurrent);
            }
            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                parseCurrent++;
                tokenBuilder.EndTag(true);
                if (parseBuffer[parseCurrent] == '<')
                {
                    this.parseState = ParseState.TagStart;
                    this.slowParse = false;
                }
                else
                {
                    this.parseState = ParseState.Text;
                }
                this.parseCurrent = parseCurrent;
                this.HandleSpecialTag();
                return true;
            }
Label_09F3:
            if (!forceFlushToken || ((parseCurrent + this.parseThreshold) < parseEnd))
            {
                goto Label_0E80;
            }
            if (this.endOfFile)
            {
                if (parseCurrent < parseEnd)
                {
                    if (this.ScanForInternalInvalidCharacters(parseCurrent))
                    {
                        goto Label_0E80;
                    }
                    parseCurrent = parseEnd;
                }
                if (!this.token.IsTagBegin)
                {
                    tokenBuilder.EndTag(true);
                    this.parseCurrent = parseCurrent;
                    this.HandleSpecialTag();
                    this.parseState = ParseState.Text;
                    return true;
                }
                goto Label_0A80;
            }
Label_0A53:
            if (!this.literalTags || (this.token.NameIndex != HtmlNameIndex.Unknown))
            {
                tokenBuilder.EndTag(false);
                this.parseCurrent = parseCurrent;
                this.HandleSpecialTag();
                return true;
            }
Label_0A80:
            parseCurrent = this.parseStart;
            this.scanQuote = this.valueQuote = '\0';
            tokenBuilder.Reset();
            baseOffset = parseCurrent;
            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            this.parseState = ParseState.Text;
            goto Label_00A0;
Label_0ABE:
            num4 = 2;
            ch2 = parseBuffer[parseCurrent + num4];
            if (ch2 == '-')
            {
                num4++;
                ch2 = parseBuffer[parseCurrent + num4];
                if (ch2 == '-')
                {
                    num4++;
                    ch2 = parseBuffer[parseCurrent + num4];
                    switch (ch2)
                    {
                        case '>':
                            parseCurrent += 5;
                            tokenBuilder.StartTag(HtmlNameIndex._COMMENT, baseOffset);
                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, baseOffset, parseCurrent - 1);
                            tokenBuilder.StartTagText();
                            tokenBuilder.EndTagText();
                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, parseCurrent - 1, parseCurrent);
                            tokenBuilder.EndTag(true);
                            this.parseState = ParseState.Text;
                            this.parseCurrent = parseCurrent;
                            return true;

                        case '-':
                            num4++;
                            ch2 = parseBuffer[parseCurrent + num4];
                            if (ch2 == '>')
                            {
                                parseCurrent += 6;
                                tokenBuilder.StartTag(HtmlNameIndex._COMMENT, baseOffset);
                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, baseOffset, parseCurrent - 2);
                                tokenBuilder.StartTagText();
                                tokenBuilder.EndTagText();
                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, parseCurrent - 2, parseCurrent);
                                tokenBuilder.EndTag(true);
                                this.parseState = ParseState.Text;
                                this.parseCurrent = parseCurrent;
                                return true;
                            }
                            break;
                    }
                    if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch2)))
                    {
                        parseCurrent += 4;
                        tokenBuilder.StartTag(HtmlNameIndex._COMMENT, baseOffset);
                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, baseOffset, parseCurrent);
                        tokenBuilder.StartTagText();
                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                        baseOffset = parseCurrent;
                        this.parseState = ParseState.Comment;
                        goto Label_0CC6;
                    }
                }
            }
            if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch2)))
            {
                if (!this.endOfFile || ((parseCurrent + num4) < parseEnd))
                {
                    this.parseThreshold = num4 + 1;
                    goto Label_0E80;
                }
                this.parseState = ParseState.Text;
                goto Label_00A0;
            }
            if (this.literalTags)
            {
                this.parseState = ParseState.Text;
                goto Label_00A0;
            }
            parseCurrent += 2;
            tokenBuilder.StartTag(HtmlNameIndex._BANG, baseOffset);
            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, baseOffset, parseCurrent);
            tokenBuilder.StartTagText();
            this.lastCharClass = ParseSupport.GetCharClass('!');
            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            baseOffset = parseCurrent;
            this.parseState = ParseState.Bang;
Label_0CC6:
            if (tokenBuilder.PrepareToAddMoreRuns(2, baseOffset, HtmlRunKind.TagText))
            {
                Label_0DF8:
                if (!ParseSupport.InvalidUnicodeCharacter(charClass))
                {
                    if (ParseSupport.QuoteCharacter(charClass))
                    {
                        if (ch == this.scanQuote)
                        {
                            this.scanQuote = '\0';
                        }
                        else if ((this.scanQuote == '\0') && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
                        {
                            this.scanQuote = ch;
                        }
                    }
                    else if (ParseSupport.HtmlSuffixCharacter(charClass))
                    {
                        int num5;
                        int num6;
                        bool flag2;
                        if (this.CheckSuffix(parseCurrent, ch, out num5, out num6, out flag2))
                        {
                            if (flag2)
                            {
                                this.scanQuote = '\0';
                                parseCurrent += num5;
                                if (parseCurrent != baseOffset)
                                {
                                    tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, baseOffset, parseCurrent);
                                }
                                tokenBuilder.EndTagText();
                                if (num6 != 0)
                                {
                                    baseOffset = parseCurrent;
                                    parseCurrent += num6;
                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, baseOffset, parseCurrent);
                                }
                                tokenBuilder.EndTag(true);
                                this.parseState = ParseState.Text;
                                this.parseCurrent = parseCurrent;
                                return true;
                            }
                            parseCurrent += num5;
                            this.lastCharClass = charClass;
                            ch = parseBuffer[parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                            goto Label_0DF8;
                        }
                        parseCurrent += num5;
                        this.parseThreshold = num6 + 1;
                        goto Label_0E03;
                    }
                    this.lastCharClass = charClass;
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    goto Label_0DF8;
                }
            }
            else
            {
                goto Label_0A53;
            }
Label_0E03:
            if (parseCurrent != baseOffset)
            {
                tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, baseOffset, parseCurrent);
                if (!tokenBuilder.PrepareToAddMoreRuns(2))
                {
                    goto Label_0A53;
                }
            }
            if (forceFlushToken && ((parseCurrent + this.parseThreshold) > parseEnd))
            {
                if (this.endOfFile && (parseCurrent < parseEnd))
                {
                    tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, parseCurrent, parseEnd);
                    parseCurrent = parseEnd;
                }
                tokenBuilder.EndTag(this.endOfFile);
                this.parseCurrent = parseCurrent;
                return true;
            }
Label_0E80:
            this.parseCurrent = parseCurrent;
            return false;
        }

        private bool SkipInvalidCharacters(ref char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num = parseCurrent;
      int gapEnd = this.parseEnd;
      while (ParseSupport.InvalidUnicodeCharacter(charClass) && num < gapEnd)
      {
        ch = this.parseBuffer[++num];
        charClass = ParseSupport.GetCharClass(ch);
      }
      if (this.parseThreshold > 1 && num + 1 < gapEnd)
      {
        int gapBegin = num + 1;
        int index;
        for (index = gapBegin; index < gapEnd && gapBegin < num + this.parseThreshold; ++index)
        {
          char ch1 = this.parseBuffer[index];
          if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch1)))
          {
            if (index != gapBegin)
            {
              this.parseBuffer[gapBegin] = ch1;
              this.parseBuffer[index] = char.MinValue;
            }
            ++gapBegin;
          }
        }
        if (index == gapEnd)
          gapEnd = this.parseEnd = this.input.RemoveGap(gapBegin, gapEnd);
      }
      parseCurrent = num;
      return num + this.parseThreshold <= gapEnd;
    }

    private char ScanTagName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
    {
      char[] chArray = this.parseBuffer;
      while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
      {
        if (ParseSupport.QuoteCharacter(charClass))
        {
          if ((int) ch == (int) this.scanQuote)
            this.scanQuote = char.MinValue;
          else if ((int) this.scanQuote == 0 && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
            this.scanQuote = ch;
        }
        else if ((int) ch == 60 && this.literalTags)
          break;
        this.lastCharClass = charClass;
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      return ch;
    }

    private char ScanAttrName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
    {
      char[] chArray = this.parseBuffer;
      while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
      {
        if (ParseSupport.QuoteCharacter(charClass))
        {
          if ((int) ch == (int) this.scanQuote)
            this.scanQuote = char.MinValue;
          else if ((int) this.scanQuote == 0 && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
            this.scanQuote = ch;
          if ((int) ch != 96)
            chArray[parseCurrent] = '?';
        }
        this.lastCharClass = charClass;
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      return ch;
    }

    private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      char[] chArray = this.parseBuffer;
      while (ParseSupport.WhitespaceCharacter(charClass))
      {
        this.lastCharClass = charClass;
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      return ch;
    }

    private char ScanText(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      char[] chArray = this.parseBuffer;
      while (ParseSupport.HtmlTextCharacter(charClass))
      {
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      return ch;
    }

    private char ScanAttrValue(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      char[] chArray = this.parseBuffer;
      while (ParseSupport.HtmlAttrValueCharacter(charClass))
      {
        this.lastCharClass = charClass;
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      return ch;
    }

    private char ScanSkipTag(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      char[] chArray = this.parseBuffer;
      while (!ParseSupport.InvalidUnicodeCharacter(charClass) && ((int) ch != 62 || (int) this.scanQuote != 0))
      {
        if (ParseSupport.QuoteCharacter(charClass))
        {
          if ((int) ch == (int) this.scanQuote)
            this.scanQuote = char.MinValue;
          else if ((int) this.scanQuote == 0 && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
            this.scanQuote = ch;
        }
        this.lastCharClass = charClass;
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      return ch;
    }

    private bool ScanForInternalInvalidCharacters(int parseCurrent)
    {
      char[] chArray = this.parseBuffer;
      do
        ;
      while (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chArray[parseCurrent++])));
      --parseCurrent;
      return parseCurrent < this.parseEnd;
    }

    private void ParseText(char ch, CharClass charClass, ref int parseCurrent)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
      int num2 = parseCurrent;
      int start1 = num2;
      do
      {
        ch = this.ScanText(ch, ref charClass, ref parseCurrent);
        if (ParseSupport.WhitespaceCharacter(charClass))
        {
          if (parseCurrent != start1)
          {
            htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, start1, parseCurrent);
            start1 = parseCurrent;
          }
          if ((int) ch == 32)
          {
            char ch1 = chArray[parseCurrent + 1];
            CharClass charClass1 = ParseSupport.GetCharClass(ch1);
            if (!ParseSupport.WhitespaceCharacter(charClass1))
            {
              ch = ch1;
              charClass = charClass1;
              ++parseCurrent;
              htmlTokenBuilder.AddTextRun(RunTextType.Space, start1, parseCurrent);
              start1 = parseCurrent;
              goto label_50;
            }
          }
          this.ParseWhitespace(ch, charClass, ref parseCurrent);
          if (this.parseThreshold > 1)
            break;
          ch = chArray[parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
        }
        else if ((int) ch == 60)
        {
          if (this.plaintext || num2 == parseCurrent)
          {
            ch = chArray[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            goto label_50;
          }
          else
          {
            if (parseCurrent != start1)
              htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, start1, parseCurrent);
            this.parseState = HtmlParser.ParseState.TagStart;
            this.slowParse = this.literalTags;
            break;
          }
        }
        else if ((int) ch == 38)
        {
          if (this.literalEntities)
          {
            ch = chArray[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            goto label_50;
          }
          else
          {
            int literal;
            int consume;
            if (this.DecodeEntity(parseCurrent, false, out literal, out consume))
            {
              if (consume == 1)
              {
                ch = chArray[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                goto label_50;
              }
              else
              {
                if (parseCurrent != start1)
                  htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, start1, parseCurrent);
                if (literal <= (int) ushort.MaxValue && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char) literal)))
                {
                  switch ((char) literal)
                  {
                    case '\t':
                      htmlTokenBuilder.AddLiteralTextRun(RunTextType.Tabulation, parseCurrent, parseCurrent + consume, literal);
                      break;
                    case '\n':
                      htmlTokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + consume, literal);
                      break;
                    case '\r':
                      htmlTokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + consume, literal);
                      break;
                    case ' ':
                      htmlTokenBuilder.AddLiteralTextRun(RunTextType.Space, parseCurrent, parseCurrent + consume, literal);
                      break;
                    default:
                      htmlTokenBuilder.AddLiteralTextRun(RunTextType.UnusualWhitespace, parseCurrent, parseCurrent + consume, literal);
                      break;
                  }
                }
                else if (literal == 160)
                  htmlTokenBuilder.AddLiteralTextRun(RunTextType.Nbsp, parseCurrent, parseCurrent + consume, literal);
                else
                  htmlTokenBuilder.AddLiteralTextRun(RunTextType.NonSpace, parseCurrent, parseCurrent + consume, literal);
                parseCurrent += consume;
                ch = chArray[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
              }
            }
            else
            {
              if (parseCurrent != start1)
                htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, start1, parseCurrent);
              this.parseThreshold = 10;
              break;
            }
          }
        }
        else if (ParseSupport.NbspCharacter(charClass))
        {
          if (parseCurrent != start1)
            htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, start1, parseCurrent);
          int start2 = parseCurrent;
          do
          {
            ch = chArray[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
          }
          while (ParseSupport.NbspCharacter(charClass));
          htmlTokenBuilder.AddTextRun(RunTextType.Nbsp, start2, parseCurrent);
        }
        else
        {
          if (parseCurrent != start1)
            htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, start1, parseCurrent);
          if (parseCurrent >= num1)
            break;
          do
          {
            ch = chArray[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
          }
          while (ParseSupport.InvalidUnicodeCharacter(charClass) && parseCurrent < num1);
        }
        start1 = parseCurrent;
label_50:;
      }
      while (htmlTokenBuilder.PrepareToAddMoreRuns(3, start1, HtmlRunKind.Text));
    }

    private bool ParseAttributeText(char ch, CharClass charClass, ref int parseCurrent)
    {
      int start = parseCurrent;
      char[] chArray = this.parseBuffer;
      HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
      while (true)
      {
        ch = this.ScanAttrValue(ch, ref charClass, ref parseCurrent);
        if (ParseSupport.QuoteCharacter(charClass))
        {
          if (charClass == CharClass.GraveAccent)
            this.tokenBuilder.SetBackquote();
          if (charClass == CharClass.Backslash)
            this.tokenBuilder.SetBackslash();
          if ((int) ch == (int) this.scanQuote)
            this.scanQuote = char.MinValue;
          else if ((int) this.scanQuote == 0 && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
            this.scanQuote = ch;
          this.lastCharClass = charClass;
          if ((int) ch != (int) this.valueQuote)
            ++parseCurrent;
          else
            goto label_31;
        }
        else if ((int) ch == 38)
        {
          this.lastCharClass = charClass;
          int literal;
          int consume;
          if (this.DecodeEntity(parseCurrent, true, out literal, out consume))
          {
            if (consume == 1)
            {
              ch = chArray[++parseCurrent];
              charClass = ParseSupport.GetCharClass(ch);
              continue;
            }
            if (parseCurrent != start)
              htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, start, parseCurrent);
            htmlTokenBuilder.AddLiteralRun(RunTextType.Unknown, HtmlRunKind.AttrValue, parseCurrent, parseCurrent + consume, literal);
            parseCurrent += consume;
            if (htmlTokenBuilder.PrepareToAddMoreRuns(2))
              start = parseCurrent;
            else
              break;
          }
          else
            goto label_21;
        }
        else if ((int) ch == 62)
        {
          this.lastCharClass = charClass;
          if ((int) this.valueQuote != 0)
          {
            if ((int) this.scanQuote != 0)
              ++parseCurrent;
            else
              goto label_25;
          }
          else
            goto label_31;
        }
        else if (ParseSupport.WhitespaceCharacter(charClass))
        {
          this.lastCharClass = charClass;
          if ((int) this.valueQuote != 0)
            ++parseCurrent;
          else
            goto label_31;
        }
        else
          goto label_31;
        ch = chArray[parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      return false;
label_21:
      this.parseThreshold = 10;
      goto label_31;
label_25:
      this.valueQuote = char.MinValue;
label_31:
      if (parseCurrent != start)
        htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, start, parseCurrent);
      return true;
    }

    private void ParseWhitespace(char ch, CharClass charClass, ref int parseCurrent)
    {
      int start = parseCurrent;
      char[] chArray = this.parseBuffer;
      HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
      do
      {
        switch (ch)
        {
          case '\t':
            do
            {
              ch = chArray[++parseCurrent];
            }
            while ((int) ch == 9);
            htmlTokenBuilder.AddTextRun(RunTextType.Tabulation, start, parseCurrent);
            break;
          case '\n':
            ch = chArray[++parseCurrent];
            htmlTokenBuilder.AddTextRun(RunTextType.NewLine, start, parseCurrent);
            break;
          case '\r':
            if ((int) chArray[parseCurrent + 1] != 10)
            {
              if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chArray[parseCurrent + 1])) && (!this.endOfFile || parseCurrent + 1 < this.parseEnd))
              {
                this.parseThreshold = 2;
                break;
              }
            }
            else
              ++parseCurrent;
            ch = chArray[++parseCurrent];
            htmlTokenBuilder.AddTextRun(RunTextType.NewLine, start, parseCurrent);
            break;
          case ' ':
            do
            {
              ch = chArray[++parseCurrent];
            }
            while ((int) ch == 32);
            htmlTokenBuilder.AddTextRun(RunTextType.Space, start, parseCurrent);
            break;
          default:
            do
            {
              ch = chArray[++parseCurrent];
            }
            while ((int) ch == 12 || (int) ch == 11);
            htmlTokenBuilder.AddTextRun(RunTextType.UnusualWhitespace, start, parseCurrent);
            break;
        }
        charClass = ParseSupport.GetCharClass(ch);
        start = parseCurrent;
      }
      while (ParseSupport.WhitespaceCharacter(charClass) && htmlTokenBuilder.PrepareToAddMoreRuns(1) && this.parseThreshold == 1);
    }

    private bool CheckSuffix(int parseCurrent, char ch, out int addToTextCnt, out int tagSuffixCnt, out bool endScan)
    {
      addToTextCnt = 1;
      tagSuffixCnt = 0;
      endScan = false;
      switch (this.parseState)
      {
        case HtmlParser.ParseState.Comment:
          if ((int) ch != 45)
            return true;
          int num1 = parseCurrent;
          char ch1;
          do
          {
            ch1 = this.parseBuffer[++num1];
          }
          while ((int) ch1 == 45);
          if ((int) ch1 == 62 && num1 - parseCurrent >= 2)
          {
            if (this.parseState == HtmlParser.ParseState.CommentConditional)
            {
              this.parseState = HtmlParser.ParseState.Comment;
              this.tokenBuilder.AbortConditional(true);
            }
            addToTextCnt = num1 - parseCurrent - 2;
            tagSuffixCnt = 3;
            endScan = true;
            return true;
          }
          if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch1)))
          {
            addToTextCnt = num1 - parseCurrent;
            return true;
          }
          addToTextCnt = num1 - parseCurrent > 2 ? num1 - parseCurrent - 2 : 0;
          tagSuffixCnt = num1 - parseCurrent - addToTextCnt;
          return false;
        case HtmlParser.ParseState.Conditional:
        case HtmlParser.ParseState.CommentConditional:
          if ((int) ch == 62)
          {
            this.parseState = this.parseState == HtmlParser.ParseState.CommentConditional ? HtmlParser.ParseState.Comment : HtmlParser.ParseState.Bang;
            this.tokenBuilder.AbortConditional(this.parseState == HtmlParser.ParseState.Comment);
            return this.CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan);
          }
          if ((int) ch != 45 || this.parseState != HtmlParser.ParseState.CommentConditional)
          {
            if ((int) ch != 93)
              return true;
            char ch2 = this.parseBuffer[parseCurrent + 1];
            if ((int) ch2 == 62)
            {
              addToTextCnt = 0;
              tagSuffixCnt = 2;
              endScan = true;
              return true;
            }
            int num2 = 1;
            if ((int) ch2 == 45)
            {
              ++num2;
              ch2 = this.parseBuffer[parseCurrent + 2];
              if ((int) ch2 == 45)
              {
                ++num2;
                ch2 = this.parseBuffer[parseCurrent + 3];
                if ((int) ch2 == 62)
                {
                  addToTextCnt = 0;
                  tagSuffixCnt = 4;
                  endScan = true;
                  return true;
                }
              }
            }
            if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch2)))
            {
              addToTextCnt = num2;
              return true;
            }
            addToTextCnt = 0;
            tagSuffixCnt = num2;
            return false;
          }
          goto case HtmlParser.ParseState.Comment;
        case HtmlParser.ParseState.Bang:
        case HtmlParser.ParseState.Dtd:
          if ((int) ch == 62 && (int) this.scanQuote == 0)
          {
            addToTextCnt = 0;
            tagSuffixCnt = 1;
            endScan = true;
          }
          return true;
        case HtmlParser.ParseState.Asp:
          if ((int) ch != 37)
            return true;
          char ch3 = this.parseBuffer[parseCurrent + 1];
          if ((int) ch3 == 62)
          {
            addToTextCnt = 0;
            tagSuffixCnt = 2;
            endScan = true;
            return true;
          }
          if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch3)))
            return true;
          addToTextCnt = 0;
          tagSuffixCnt = 1;
          return false;
        default:
          return true;
      }
    }

    private bool DecodeEntity(int parseCurrent, bool inAttribute, out int literal, out int consume)
    {
      char[] buffer = this.parseBuffer;
      int nameOffset = parseCurrent + 1;
      int index1 = nameOffset;
      int nameLength = 0;
      int entityValue1 = 0;
      char ch1 = buffer[index1];
      CharClass charClass1 = ParseSupport.GetCharClass(ch1);
      if ((int) ch1 == 35)
      {
        int index2;
        char ch2 = buffer[index2 = index1 + 1];
        CharClass charClass2 = ParseSupport.GetCharClass(ch2);
        if ((int) ch2 == 120 || (int) ch2 == 88)
        {
          int num;
          char ch3 = buffer[num = index2 + 1];
          CharClass charClass3;
          for (charClass3 = ParseSupport.GetCharClass(ch3); ParseSupport.HexCharacter(charClass3); charClass3 = ParseSupport.GetCharClass(ch3))
          {
            ++nameLength;
            entityValue1 = (entityValue1 << 4) + ParseSupport.CharToHex(ch3);
            ch3 = buffer[++num];
          }
          if (!ParseSupport.InvalidUnicodeCharacter(charClass3) || this.endOfFile && num >= this.parseEnd || nameLength > 6)
          {
            if ((inAttribute || (int) ch3 == 59) && (entityValue1 != 0 && nameLength <= 6))
            {
              HtmlParser.ProcessNumericEntityValue(entityValue1, out literal);
              consume = num - parseCurrent;
              if ((int) ch3 == 59)
                ++consume;
              return true;
            }
            literal = 0;
            consume = 1;
            return true;
          }
        }
        else
        {
          for (; ParseSupport.NumericCharacter(charClass2); charClass2 = ParseSupport.GetCharClass(ch2))
          {
            ++nameLength;
            entityValue1 = entityValue1 * 10 + ParseSupport.CharToDecimal(ch2);
            ++index2;
            ch2 = buffer[index2];
          }
          if (!ParseSupport.InvalidUnicodeCharacter(charClass2) || this.endOfFile && index2 >= this.parseEnd || nameLength > 7)
          {
            if (entityValue1 != 0 && nameLength <= 7)
            {
              HtmlParser.ProcessNumericEntityValue(entityValue1, out literal);
              consume = index2 - parseCurrent;
              if ((int) ch2 == 59)
                ++consume;
              return true;
            }
            literal = 0;
            consume = 1;
            return true;
          }
        }
      }
      else
      {
        short[] numArray = this.hashValuesTable ?? (this.hashValuesTable = new short[8]);
        HashCode hashCode = new HashCode(true);
        for (; ParseSupport.HtmlEntityCharacter(charClass1) && nameLength < 8; charClass1 = ParseSupport.GetCharClass(ch1))
        {
          short num = (short) ((long) (uint) (hashCode.AdvanceAndFinalizeHash(ch1) ^ 230) % 705L);
          numArray[nameLength++] = num;
          ++index1;
          ch1 = buffer[index1];
        }
        if (!ParseSupport.InvalidUnicodeCharacter(charClass1) || this.endOfFile && index1 >= this.parseEnd)
        {
          if (nameLength > 1)
          {
            int entityValue2;
            if (HtmlParser.FindEntityByHashName(numArray[nameLength - 1], buffer, nameOffset, nameLength, out entityValue2) && ((int) ch1 == 59 || entityValue2 <= (int) byte.MaxValue))
              entityValue1 = entityValue2;
            else if (!inAttribute)
            {
              for (int index2 = nameLength - 2; index2 >= 0; --index2)
              {
                if (HtmlParser.FindEntityByHashName(numArray[index2], buffer, nameOffset, index2 + 1, out entityValue2) && entityValue2 <= (int) byte.MaxValue)
                {
                  entityValue1 = entityValue2;
                  nameLength = index2 + 1;
                  break;
                }
              }
            }
            if (entityValue1 != 0)
            {
              literal = entityValue1;
              consume = nameLength + 1;
              if ((int) buffer[nameOffset + nameLength] == 59)
                ++consume;
              return true;
            }
          }
          literal = 0;
          consume = 1;
          return true;
        }
      }
      literal = 0;
      consume = 0;
      return false;
    }

    private void HandleSpecialTag()
    {
      if (HtmlNameData.Names[(int) this.token.NameIndex].LiteralTag)
      {
        this.literalTags = !this.token.IsEndTag;
        this.literalTagNameId = this.literalTags ? this.token.NameIndex : HtmlNameIndex.Unknown;
        if (HtmlNameData.Names[(int) this.token.NameIndex].LiteralEnt)
          this.literalEntities = !this.token.IsEndTag && !this.token.IsEmptyScope;
        this.slowParse = this.slowParse || this.literalTags;
      }
      switch (this.token.NameIndex)
      {
        case HtmlNameIndex.Meta:
          if (!(this.input is ConverterDecodingInput) || !this.detectEncodingFromMetaTag || !((IRestartable) this).CanRestart())
            break;
          if (this.token.IsTagBegin)
          {
            this.rightMeta = false;
            this.newEncoding = (Encoding) null;
          }
          this.token.Attributes.Rewind();
          int index = -1;
          bool lookForWordCharset = false;
          foreach (HtmlAttribute htmlAttribute in this.token.Attributes)
          {
            if (htmlAttribute.NameIndex == HtmlNameIndex.HttpEquiv)
            {
              if (htmlAttribute.Value.CaseInsensitiveCompareEqual("content-type") || htmlAttribute.Value.CaseInsensitiveCompareEqual("charset"))
              {
                this.rightMeta = true;
                if (index != -1)
                  break;
              }
              else
                break;
            }
            else if (htmlAttribute.NameIndex == HtmlNameIndex.Content)
            {
              index = htmlAttribute.Index;
              lookForWordCharset = true;
              if (this.rightMeta)
                break;
            }
            else if (htmlAttribute.NameIndex == HtmlNameIndex.Charset)
            {
              index = htmlAttribute.Index;
              lookForWordCharset = false;
              this.rightMeta = true;
              break;
            }
          }
          if (index != -1)
          {
            string name = HtmlParser.CharsetFromString(this.token.Attributes[index].Value.GetString(100), lookForWordCharset);
            if (name != null)
              Globalization.Charset.TryGetEncoding(name, out this.newEncoding);
          }
          if (this.rightMeta && this.newEncoding != null)
            (this.input as ConverterDecodingInput).RestartWithNewEncoding(this.newEncoding);
          this.token.Attributes.Rewind();
          break;
        case HtmlNameIndex.PlainText:
          if (this.token.IsEndTag)
            break;
          this.plaintext = true;
          this.literalEntities = true;
          if (!this.token.IsTagEnd)
            break;
          this.parseState = HtmlParser.ParseState.Text;
          break;
      }
    }

    protected enum ParseState : byte
    {
      Text,
      TagStart,
      TagNamePrefix,
      TagName,
      TagWsp,
      AttrNameStart,
      AttrNamePrefix,
      AttrName,
      AttrWsp,
      AttrValueWsp,
      AttrValue,
      EmptyTagEnd,
      TagEnd,
      TagSkip,
      CommentStart,
      Comment,
      Conditional,
      CommentConditional,
      Bang,
      Dtd,
      Asp,
    }

    private class SavedParserState
    {
      private ConverterInput input;
      private bool endOfFile;
      private HtmlParser.ParseState parseState;
      private bool slowParse;
      private bool literalTags;
      private HtmlNameIndex literalTagNameId;
      private bool literalEntities;
      private bool plaintext;
      private char[] parseBuffer;
      private int parseStart;
      private int parseCurrent;
      private int parseEnd;
      private int parseThreshold;
      private int parseDocumentOffset;

      public bool StateSaved => this.input != null;

        public void PushState(HtmlParser parser, ConverterInput newInput, bool literalTextInput)
      {
        this.input = parser.input;
        this.endOfFile = parser.endOfFile;
        this.parseState = parser.parseState;
        this.slowParse = parser.slowParse;
        this.literalTags = parser.literalTags;
        this.literalTagNameId = parser.literalTagNameId;
        this.literalEntities = parser.literalEntities;
        this.plaintext = parser.plaintext;
        this.parseBuffer = parser.parseBuffer;
        this.parseStart = parser.parseStart;
        this.parseCurrent = parser.parseCurrent;
        this.parseEnd = parser.parseEnd;
        this.parseThreshold = parser.parseThreshold;
        this.parseDocumentOffset = parser.parseDocumentOffset;
        parser.input = newInput;
        parser.endOfFile = false;
        parser.parseState = HtmlParser.ParseState.Text;
        parser.slowParse = true;
        parser.literalTags = literalTextInput;
        parser.literalTagNameId = HtmlNameIndex.PlainText;
        parser.literalEntities = literalTextInput;
        parser.plaintext = literalTextInput;
        parser.parseBuffer = (char[]) null;
        parser.parseStart = 0;
        parser.parseCurrent = 0;
        parser.parseEnd = 0;
        parser.parseThreshold = 1;
      }

      public void PopState(HtmlParser parser)
      {
        parser.input = this.input;
        parser.endOfFile = this.endOfFile;
        parser.parseState = this.parseState;
        parser.slowParse = this.slowParse;
        parser.literalTags = this.literalTags;
        parser.literalTagNameId = this.literalTagNameId;
        parser.literalEntities = this.literalEntities;
        parser.plaintext = this.plaintext;
        parser.parseBuffer = this.parseBuffer;
        parser.parseStart = this.parseStart;
        parser.parseCurrent = this.parseCurrent;
        parser.parseEnd = this.parseEnd;
        parser.parseThreshold = this.parseThreshold;
        parser.parseDocumentOffset = this.parseDocumentOffset;
        this.input = (ConverterInput) null;
        this.parseBuffer = (char[]) null;
      }
    }
  }
}
