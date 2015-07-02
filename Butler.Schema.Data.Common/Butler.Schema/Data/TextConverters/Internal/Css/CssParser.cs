// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Css.CssParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Css
{
  internal class CssParser : IDisposable
  {
    private static readonly string[] SafeTermFunctions = new string[2]
    {
      "rgb",
      "counter"
    };
    private static readonly string[] SafePseudoFunctions = new string[1]
    {
      "lang"
    };
    internal const int MaxCssLength = 524288;
    protected CssTokenBuilder tokenBuilder;
    private ConverterInput input;
    private bool endOfFile;
    private CssParseMode parseMode;
    private bool isInvalid;
    private char[] parseBuffer;
    private int parseStart;
    private int parseCurrent;
    private int parseEnd;
    private int ruleDepth;
    private CssToken token;

    public CssToken Token => this.token;

      public CssParser(ConverterInput input, int maxRuns, bool testBoundaryConditions)
    {
      this.input = input;
      this.tokenBuilder = new CssTokenBuilder((char[]) null, 256, 256, maxRuns, testBoundaryConditions);
      this.token = this.tokenBuilder.Token;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && this.input != null)
        this.input.Dispose();
      this.input = (ConverterInput) null;
      this.parseBuffer = (char[]) null;
      this.token = (CssToken) null;
    }

    public void Reset()
    {
      this.endOfFile = false;
      this.parseBuffer = (char[]) null;
      this.parseStart = 0;
      this.parseCurrent = 0;
      this.parseEnd = 0;
      this.ruleDepth = 0;
    }

    public void SetParseMode(CssParseMode parseMode)
    {
      this.parseMode = parseMode;
    }

    public CssTokenId Parse()
    {
      if (this.endOfFile)
        return CssTokenId.EndOfFile;
      this.tokenBuilder.Reset();
      char[] chArray = this.parseBuffer;
      int parseCurrent = this.parseCurrent;
      int num1 = this.parseEnd;
      if (parseCurrent >= num1)
      {
        this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd);
        if (this.parseEnd == 0)
          return CssTokenId.EndOfFile;
        this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
        chArray = this.parseBuffer;
        parseCurrent = this.parseCurrent;
        num1 = this.parseEnd;
      }
      char ch = chArray[parseCurrent];
      CharClass charClass = ParseSupport.GetCharClass(ch);
      int num2 = parseCurrent;
      if (this.parseMode == CssParseMode.StyleTag)
      {
        int num3 = (int) this.ScanStyleSheet(ch, ref charClass, ref parseCurrent);
        if (num2 >= parseCurrent)
        {
          this.tokenBuilder.Reset();
          return CssTokenId.EndOfFile;
        }
        if (this.tokenBuilder.Incomplete)
          this.tokenBuilder.EndRuleSet();
      }
      else
      {
        int num3 = (int) this.ScanDeclarations(ch, ref charClass, ref parseCurrent);
        if (parseCurrent < num1)
        {
          this.endOfFile = true;
          this.tokenBuilder.Reset();
          return CssTokenId.EndOfFile;
        }
        if (this.tokenBuilder.Incomplete)
          this.tokenBuilder.EndDeclarations();
      }
      this.endOfFile = parseCurrent == num1;
      this.parseCurrent = parseCurrent;
      return this.token.CssTokenId;
    }

    private static bool IsNameCharacterNoEscape(char ch, CharClass charClass)
    {
      if (!CssParser.IsNameStartCharacterNoEscape(ch, charClass) && !ParseSupport.NumericCharacter(charClass))
        return (int) ch == 45;
      return true;
    }

    private static bool IsNameStartCharacterNoEscape(char ch, CharClass charClass)
    {
      if (!ParseSupport.AlphaCharacter(charClass) && (int) ch != 95)
        return (int) ch > (int) sbyte.MaxValue;
      return true;
    }

    private static bool IsUrlCharacterNoEscape(char ch, CharClass charClass)
    {
      if (((int) ch < 42 || (int) ch == (int) sbyte.MaxValue) && ((int) ch < 35 || (int) ch > 38))
        return (int) ch == 33;
      return true;
    }

    private char ScanStyleSheet(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      int num2;
      do
      {
        num2 = parseCurrent;
        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
        if (parseCurrent == num1)
          return ch;
        if (this.IsNameStartCharacter(ch, charClass, parseCurrent) || (int) ch == 42 || ((int) ch == 46 || (int) ch == 58) || ((int) ch == 35 || (int) ch == 91))
        {
          ch = this.ScanRuleSet(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1 || !this.isInvalid)
            return ch;
        }
        else if ((int) ch == 64)
        {
          ch = this.ScanAtRule(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1 || !this.isInvalid)
            return ch;
        }
        else if ((int) ch == 47 && parseCurrent < num1 && (int) chArray[parseCurrent + 1] == 42)
        {
          ch = this.ScanComment(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        else if ((int) ch == 60)
        {
          ch = this.ScanCdo(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        else if ((int) ch == 45)
        {
          ch = this.ScanCdc(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        else
          this.isInvalid = true;
        if (this.isInvalid)
        {
          this.isInvalid = false;
          this.tokenBuilder.Reset();
          ch = this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
      }
      while (num2 < parseCurrent);
      return ch;
    }

    private char ScanCdo(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      ++parseCurrent;
      if (parseCurrent + 3 >= this.parseEnd)
      {
        parseCurrent = this.parseEnd;
        return ch;
      }
      if ((int) this.parseBuffer[parseCurrent++] != 33 || (int) this.parseBuffer[parseCurrent++] != 45 || (int) this.parseBuffer[parseCurrent++] != 45)
        return this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
      ch = this.parseBuffer[parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
    }

    private char ScanCdc(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      ++parseCurrent;
      if (parseCurrent + 2 >= this.parseEnd)
      {
        parseCurrent = this.parseEnd;
        return ch;
      }
      if ((int) this.parseBuffer[parseCurrent++] != 45 || (int) this.parseBuffer[parseCurrent++] != 62)
        return this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
      ch = this.parseBuffer[parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
    }

    private char ScanAtRule(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      int num2 = parseCurrent;
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
      {
        this.isInvalid = true;
        return ch;
      }
      this.tokenBuilder.StartRuleSet(num2, CssTokenId.AtRule);
      if (!this.tokenBuilder.CanAddSelector())
      {
        parseCurrent = num1;
        return ch;
      }
      this.tokenBuilder.StartSelectorName();
      this.PrepareAndAddRun(CssRunKind.AtRuleName, num2, ref parseCurrent);
      if (parseCurrent == num1)
        return ch;
      int nameLength;
      ch = this.ScanIdent(CssRunKind.AtRuleName, ch, ref charClass, ref parseCurrent, out nameLength);
      this.tokenBuilder.EndSelectorName(nameLength);
      if (parseCurrent == num1)
        return ch;
      if (this.IsNameEqual("page", num2 + 1, parseCurrent - num2 - 1))
      {
        ch = this.ScanPageSelector(ch, ref charClass, ref parseCurrent);
        if (parseCurrent == num1)
          return ch;
      }
      else if (!this.IsNameEqual("font-face", num2 + 1, parseCurrent - num2 - 1))
      {
        this.isInvalid = true;
        return ch;
      }
      this.tokenBuilder.EndSimpleSelector();
      ch = this.ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
      if (parseCurrent == num1)
        return ch;
      return ch;
    }

    private char ScanPageSelector(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
      if (parseCurrent == this.parseEnd)
        return ch;
      if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
      {
        this.tokenBuilder.EndSimpleSelector();
        this.tokenBuilder.StartSelectorName();
        int nameLength;
        ch = this.ScanIdent(CssRunKind.PageIdent, ch, ref charClass, ref parseCurrent, out nameLength);
        this.tokenBuilder.EndSelectorName(nameLength);
        if (parseCurrent == this.parseEnd)
          return ch;
        this.tokenBuilder.SetSelectorCombinator(CssSelectorCombinator.Descendant, false);
      }
      if ((int) ch == 58)
      {
        ch = this.parseBuffer[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
        this.PrepareAndAddRun(CssRunKind.PagePseudoStart, parseCurrent - 1, ref parseCurrent);
        if (parseCurrent == this.parseEnd)
          return ch;
        if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
        {
          this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
          return ch;
        }
        this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
        int nameLength;
        ch = this.ScanIdent(CssRunKind.PagePseudo, ch, ref charClass, ref parseCurrent, out nameLength);
        this.tokenBuilder.EndSelectorClass();
        if (parseCurrent == this.parseEnd)
          return ch;
      }
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
      if (parseCurrent == this.parseEnd)
        return ch;
      return ch;
    }

    private char ScanRuleSet(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      this.tokenBuilder.StartRuleSet(parseCurrent, CssTokenId.RuleSet);
      ch = this.ScanSelectors(ch, ref charClass, ref parseCurrent);
      if (parseCurrent == this.parseEnd || this.isInvalid)
        return ch;
      ch = this.ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
      if (parseCurrent == this.parseEnd)
        return ch;
      return ch;
    }

    private char ScanDeclarationBlock(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
      if (parseCurrent == this.parseEnd)
        return ch;
      if ((int) ch != 123)
      {
        this.isInvalid = true;
        return ch;
      }
      ++this.ruleDepth;
      ch = this.parseBuffer[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
      if (parseCurrent == this.parseEnd)
        return ch;
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
      if (parseCurrent == this.parseEnd)
        return ch;
      ch = this.ScanDeclarations(ch, ref charClass, ref parseCurrent);
      if (parseCurrent == this.parseEnd)
        return ch;
      if ((int) ch != 125)
      {
        this.isInvalid = true;
        return ch;
      }
      --this.ruleDepth;
      ch = this.parseBuffer[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
      if (parseCurrent == this.parseEnd)
        return ch;
      return ch;
    }

    private char ScanSelectors(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      int num2 = parseCurrent;
      ch = this.ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
      if (parseCurrent == num1 || this.isInvalid)
        return ch;
      while (num2 < parseCurrent)
      {
        CssSelectorCombinator combinator = CssSelectorCombinator.None;
        bool flag1 = false;
        bool flag2 = false;
        int num3 = parseCurrent;
        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
        if (parseCurrent == num1)
          return ch;
        if (num3 < parseCurrent)
        {
          flag1 = true;
          combinator = CssSelectorCombinator.Descendant;
        }
        if ((int) ch == 43 || (int) ch == 62 || (int) ch == 44)
        {
          combinator = (int) ch == 43 ? CssSelectorCombinator.Adjacent : ((int) ch == 62 ? CssSelectorCombinator.Child : CssSelectorCombinator.None);
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          if (parseCurrent == num1)
            return ch;
          this.PrepareAndAddRun(CssRunKind.SelectorCombinatorOrComma, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
          flag2 = true;
          ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
          if (parseCurrent == num1)
            return ch;
        }
        else if (num3 == parseCurrent)
          break;
        num2 = parseCurrent;
        ch = this.ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
        if (num2 == parseCurrent)
        {
          if (flag2)
            this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorCombinatorOrComma);
          if (flag1)
          {
            this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
            break;
          }
          break;
        }
        if (this.isInvalid)
          return ch;
        this.tokenBuilder.SetSelectorCombinator(combinator, true);
        if (parseCurrent == num1)
          return ch;
      }
      return ch;
    }

    private char ScanSimpleSelector(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num = this.parseEnd;
      char[] chArray = this.parseBuffer;
      if ((int) ch == 46 || (int) ch == 58 || ((int) ch == 35 || (int) ch == 91))
      {
        if (!this.tokenBuilder.CanAddSelector())
        {
          parseCurrent = num;
          return ch;
        }
        this.tokenBuilder.BuildUniversalSelector();
      }
      else
      {
        if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent) && (int) ch != 42)
          return ch;
        if (!this.tokenBuilder.CanAddSelector())
        {
          parseCurrent = num;
          return ch;
        }
        this.tokenBuilder.StartSelectorName();
        int nameLength;
        if ((int) ch == 42)
        {
          nameLength = 1;
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          this.PrepareAndAddRun(CssRunKind.SelectorName, parseCurrent - 1, ref parseCurrent);
        }
        else
          ch = this.ScanIdent(CssRunKind.SelectorName, ch, ref charClass, ref parseCurrent, out nameLength);
        this.tokenBuilder.EndSelectorName(nameLength);
        if (parseCurrent == num)
          return ch;
      }
      ch = this.ScanSelectorSuffix(ch, ref charClass, ref parseCurrent);
      this.tokenBuilder.EndSimpleSelector();
      return ch;
    }

    private char ScanSelectorSuffix(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      if ((int) ch == 91)
      {
        this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Attrib);
        ch = this.ScanSelectorAttrib(ch, ref charClass, ref parseCurrent);
        this.tokenBuilder.EndSelectorClass();
        return ch;
      }
      int num = this.parseEnd;
      char[] chArray = this.parseBuffer;
      if ((int) ch == 58)
      {
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
        this.PrepareAndAddRun(CssRunKind.SelectorPseudoStart, parseCurrent - 1, ref parseCurrent);
        if (parseCurrent == num)
          return ch;
        this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
        ch = this.ScanSelectorPseudo(ch, ref charClass, ref parseCurrent);
        this.tokenBuilder.EndSelectorClass();
        return ch;
      }
      if ((int) ch == 46 || (int) ch == 35)
      {
        bool flag = (int) ch == 46;
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
        if (this.IsNameCharacter(ch, charClass, parseCurrent) && (!flag || this.IsIdentStartCharacter(ch, charClass, parseCurrent)))
        {
          this.PrepareAndAddRun(flag ? CssRunKind.SelectorClassStart : CssRunKind.SelectorHashStart, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num)
            return ch;
          this.tokenBuilder.StartSelectorClass(flag ? CssSelectorClassType.Regular : CssSelectorClassType.Hash);
          ch = this.ScanName(flag ? CssRunKind.SelectorClass : CssRunKind.SelectorHash, ch, ref charClass, ref parseCurrent);
          this.tokenBuilder.EndSelectorClass();
          if (parseCurrent == num)
            return ch;
        }
        else
          this.PrepareAndAddInvalidRun(CssRunKind.FunctionStart, ref parseCurrent);
      }
      return ch;
    }

    private char ScanSelectorPseudo(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num = this.parseEnd;
      char[] chArray = this.parseBuffer;
      if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
      {
        this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
        return ch;
      }
      int start = parseCurrent;
      int nameLength;
      ch = this.ScanIdent(CssRunKind.SelectorPseudo, ch, ref charClass, ref parseCurrent, out nameLength);
      if (parseCurrent == num || (int) ch != 40 || !this.IsSafeIdentifier(CssParser.SafePseudoFunctions, start, parseCurrent))
        return ch;
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      if (parseCurrent == num)
        return ch;
      this.PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
      if (parseCurrent == num)
        return ch;
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
      if (parseCurrent == num)
        return ch;
      if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
      {
        this.isInvalid = true;
        return ch;
      }
      ch = this.ScanIdent(CssRunKind.SelectorPseudoArg, ch, ref charClass, ref parseCurrent, out nameLength);
      if (parseCurrent == num)
        return ch;
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
      if (parseCurrent == num)
        return ch;
      if ((int) ch != 41)
      {
        this.isInvalid = true;
        return ch;
      }
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      this.PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
      if (parseCurrent == num)
        return ch;
      return ch;
    }

    private char ScanSelectorAttrib(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      this.PrepareAndAddRun(CssRunKind.SelectorAttribStart, parseCurrent - 1, ref parseCurrent);
      if (parseCurrent == num1)
        return ch;
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
      if (parseCurrent == num1)
        return ch;
      if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
      {
        this.isInvalid = true;
        return ch;
      }
      int nameLength;
      ch = this.ScanIdent(CssRunKind.SelectorAttribName, ch, ref charClass, ref parseCurrent, out nameLength);
      if (parseCurrent == num1)
        return ch;
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
      if (parseCurrent == num1)
        return ch;
      int num2 = parseCurrent;
      if ((int) ch == 61)
      {
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
        this.PrepareAndAddRun(CssRunKind.SelectorAttribEquals, parseCurrent - 1, ref parseCurrent);
        if (parseCurrent == num1)
          return ch;
      }
      else if (((int) ch == 126 || (int) ch == 124) && (int) chArray[parseCurrent + 1] == 61)
      {
        parseCurrent += 2;
        this.PrepareAndAddRun((int) ch == 126 ? CssRunKind.SelectorAttribIncludes : CssRunKind.SelectorAttribDashmatch, parseCurrent - 2, ref parseCurrent);
        if (parseCurrent == num1)
          return ch;
        ch = chArray[parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      if (num2 < parseCurrent)
      {
        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
        if (parseCurrent == num1)
          return ch;
        if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
        {
          int num3 = parseCurrent;
          ch = this.ScanIdent(CssRunKind.SelectorAttribIdentifier, ch, ref charClass, ref parseCurrent, out nameLength);
          if (parseCurrent == num1)
            return ch;
        }
        else if ((int) ch == 34 || (int) ch == 39)
        {
          int start = parseCurrent;
          ch = this.ScanString(ch, ref charClass, ref parseCurrent, false);
          if (this.isInvalid)
            return ch;
          this.PrepareAndAddRun(CssRunKind.SelectorAttribString, start, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
        if (parseCurrent == num1)
          return ch;
      }
      if ((int) ch != 93)
      {
        this.isInvalid = true;
        return ch;
      }
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      this.PrepareAndAddRun(CssRunKind.SelectorAttribEnd, parseCurrent - 1, ref parseCurrent);
      if (parseCurrent == num1)
        return ch;
      return ch;
    }

    private char ScanDeclarations(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      this.tokenBuilder.StartDeclarations(parseCurrent);
      int num2;
      do
      {
        num2 = parseCurrent;
        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
        if (parseCurrent == num1)
          return ch;
        if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
        {
          if (!this.tokenBuilder.CanAddProperty())
          {
            parseCurrent = num1;
            return ch;
          }
          this.tokenBuilder.StartPropertyName();
          int nameLength;
          ch = this.ScanIdent(CssRunKind.PropertyName, ch, ref charClass, ref parseCurrent, out nameLength);
          this.tokenBuilder.EndPropertyName(nameLength);
          if (parseCurrent == num1)
            return ch;
          ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
          if (parseCurrent == num1)
            return ch;
          if ((int) ch != 58)
          {
            this.tokenBuilder.MarkPropertyAsDeleted();
            return ch;
          }
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          this.PrepareAndAddRun(CssRunKind.PropertyColon, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
          ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
          if (parseCurrent == num1)
            return ch;
          this.tokenBuilder.StartPropertyValue();
          ch = this.ScanPropertyValue(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
          this.tokenBuilder.EndPropertyValue();
          this.tokenBuilder.EndProperty();
          ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
          if (parseCurrent == num1)
            return ch;
        }
        if ((int) ch != 59)
        {
          this.tokenBuilder.EndDeclarations();
          return ch;
        }
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
        this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
      }
      while (parseCurrent != num1 && num2 < parseCurrent);
      return ch;
    }

    private char ScanPropertyValue(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num = this.parseEnd;
      char[] chArray = this.parseBuffer;
      ch = this.ScanExpr(ch, ref charClass, ref parseCurrent, 0);
      if (parseCurrent == num || (int) ch != 33)
        return ch;
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      if (parseCurrent == num)
      {
        this.tokenBuilder.MarkPropertyAsDeleted();
        return ch;
      }
      this.PrepareAndAddRun(CssRunKind.ImportantStart, parseCurrent - 1, ref parseCurrent);
      if (parseCurrent == num)
        return ch;
      ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
      if (parseCurrent == num)
      {
        this.tokenBuilder.MarkPropertyAsDeleted();
        return ch;
      }
      if (this.IsNameStartCharacter(ch, charClass, parseCurrent))
      {
        int start = parseCurrent;
        ch = this.ScanName(CssRunKind.Important, ch, ref charClass, ref parseCurrent);
        if (parseCurrent == num || this.IsNameEqual("important", start, parseCurrent - start))
          return ch;
        this.tokenBuilder.MarkPropertyAsDeleted();
        return ch;
      }
      this.tokenBuilder.MarkPropertyAsDeleted();
      return ch;
    }

    private char ScanExpr(char ch, ref CharClass charClass, ref int parseCurrent, int level)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      int num2 = parseCurrent;
      ch = this.ScanTerm(ch, ref charClass, ref parseCurrent, level);
      if (parseCurrent == num1)
        return ch;
      while (num2 < parseCurrent)
      {
        bool flag1 = false;
        bool flag2 = false;
        int num3 = parseCurrent;
        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
        if (parseCurrent == num1)
          return ch;
        if (num3 < parseCurrent)
          flag1 = true;
        if ((int) ch == 47 || (int) ch == 44)
        {
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          if (parseCurrent == num1)
            return ch;
          this.PrepareAndAddRun(CssRunKind.Operator, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
          flag2 = true;
          ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
          if (parseCurrent == num1)
            return ch;
        }
        else if (num3 == parseCurrent)
          break;
        num2 = parseCurrent;
        ch = this.ScanTerm(ch, ref charClass, ref parseCurrent, level);
        if (parseCurrent == num1)
          return ch;
        if (num2 == parseCurrent)
        {
          if (flag2)
            this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Operator);
          if (flag1)
          {
            this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
            break;
          }
          break;
        }
      }
      return ch;
    }

    private char ScanTerm(char ch, ref CharClass charClass, ref int parseCurrent, int level)
    {
      int num1 = this.parseEnd;
      char[] chArray = this.parseBuffer;
      bool flag1 = false;
      if ((int) ch == 45 || (int) ch == 43)
      {
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
        if (parseCurrent == num1)
        {
          this.tokenBuilder.MarkPropertyAsDeleted();
          return ch;
        }
        this.PrepareAndAddRun(CssRunKind.UnaryOperator, parseCurrent - 1, ref parseCurrent);
        if (parseCurrent == num1)
          return ch;
        flag1 = true;
      }
      if (ParseSupport.NumericCharacter(charClass) || (int) ch == 46)
      {
        ch = this.ScanNumeric(ch, ref charClass, ref parseCurrent);
        if (parseCurrent == num1)
          return ch;
        if ((int) ch == 46)
        {
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          this.PrepareAndAddRun(CssRunKind.Dot, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
          int num2 = parseCurrent;
          ch = this.ScanNumeric(ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
          if (num2 == parseCurrent)
            this.tokenBuilder.MarkPropertyAsDeleted();
        }
        if ((int) ch == 37)
        {
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          this.PrepareAndAddRun(CssRunKind.Percent, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        else if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
        {
          int nameLength;
          ch = this.ScanIdent(CssRunKind.Metrics, ch, ref charClass, ref parseCurrent, out nameLength);
          if (parseCurrent == num1)
            return ch;
        }
      }
      else if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
      {
        int start1 = parseCurrent;
        int nameLength;
        ch = this.ScanIdent(CssRunKind.TermIdentifier, ch, ref charClass, ref parseCurrent, out nameLength);
        if (parseCurrent == num1)
          return ch;
        int start2 = parseCurrent;
        if ((int) ch == 43 && start1 + 1 == parseCurrent && ((int) chArray[start1] == 117 || (int) chArray[start1] == 85))
        {
          ch = this.ScanUnicodeRange(ch, ref charClass, ref parseCurrent);
          this.PrepareAndAddRun(CssRunKind.UnicodeRange, start2, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        else if ((int) ch == 40)
        {
          bool flag2 = false;
          if (!this.IsSafeIdentifier(CssParser.SafeTermFunctions, start1, parseCurrent))
          {
            this.tokenBuilder.MarkPropertyAsDeleted();
            if (this.IsNameEqual("url", start1, parseCurrent - start1))
              flag2 = true;
          }
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          if (parseCurrent == num1)
          {
            this.tokenBuilder.MarkPropertyAsDeleted();
            return ch;
          }
          this.PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num1)
          {
            this.tokenBuilder.MarkPropertyAsDeleted();
            return ch;
          }
          ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
          if (parseCurrent == num1)
          {
            this.tokenBuilder.MarkPropertyAsDeleted();
            return ch;
          }
          if (flag2)
          {
            if ((int) ch == 34 || (int) ch == 39)
            {
              int start3 = parseCurrent;
              ch = this.ScanString(ch, ref charClass, ref parseCurrent, true);
              if (this.isInvalid)
                return ch;
              this.PrepareAndAddRun(CssRunKind.String, start3, ref parseCurrent);
              if (parseCurrent == num1)
                return ch;
            }
            else
            {
              int num2 = parseCurrent;
              ch = this.ScanUrl(ch, ref charClass, ref parseCurrent);
              if (parseCurrent == num1)
                return ch;
            }
            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == num1)
              return ch;
          }
          else
          {
            if (++level > 16)
            {
              this.tokenBuilder.MarkPropertyAsDeleted();
              return ch;
            }
            ch = this.ScanExpr(ch, ref charClass, ref parseCurrent, level);
            if (parseCurrent == num1)
            {
              this.tokenBuilder.MarkPropertyAsDeleted();
              return ch;
            }
          }
          if ((int) ch != 41)
            this.tokenBuilder.MarkPropertyAsDeleted();
          ch = chArray[++parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          this.PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        else if (flag1)
          this.tokenBuilder.MarkPropertyAsDeleted();
      }
      else if (flag1)
        this.tokenBuilder.MarkPropertyAsDeleted();
      else if ((int) ch == 35)
      {
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
        this.PrepareAndAddRun(CssRunKind.HexColorStart, parseCurrent - 1, ref parseCurrent);
        if (parseCurrent == num1)
          return ch;
        if (this.IsNameCharacter(ch, charClass, parseCurrent))
        {
          ch = this.ScanName(CssRunKind.HexColor, ch, ref charClass, ref parseCurrent);
          if (parseCurrent == num1)
            return ch;
        }
        else
          this.tokenBuilder.MarkPropertyAsDeleted();
      }
      else if ((int) ch == 34 || (int) ch == 39)
      {
        int start = parseCurrent;
        ch = this.ScanString(ch, ref charClass, ref parseCurrent, true);
        if (this.isInvalid)
          return ch;
        this.PrepareAndAddRun(CssRunKind.String, start, ref parseCurrent);
        if (parseCurrent == num1)
          return ch;
      }
      return ch;
    }

    private char ScanNumeric(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int start = parseCurrent;
      char[] chArray = this.parseBuffer;
      while (ParseSupport.NumericCharacter(charClass))
      {
        ch = chArray[++parseCurrent];
        charClass = ParseSupport.GetCharClass(ch);
      }
      this.PrepareAndAddRun(CssRunKind.Numeric, start, ref parseCurrent);
      return ch;
    }

    private char ScanString(char ch, ref CharClass charClass, ref int parseCurrent, bool inProperty)
    {
      int parseEnd = this.parseEnd;
      char[] parseBuffer = this.parseBuffer;
      char ch1 = ch;
      char ch2 = char.MinValue;
      char ch3 = char.MinValue;
      bool flag = false;
      while (true)
      {
        ch = parseBuffer[++parseCurrent];
        if (parseCurrent != parseEnd)
        {
          if (CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
          {
            flag = true;
            if (parseCurrent != parseEnd)
            {
              if ((int) ch != (int) ch1)
              {
                ch2 = char.MinValue;
                ch3 = char.MinValue;
              }
              else
                goto label_14;
            }
            else
              goto label_7;
          }
          else if ((int) ch != (int) ch1 && ((int) ch != 10 || (int) ch2 != 13 || (int) ch3 == 92) && (((int) ch != 10 || (int) ch2 == 13) && ((int) ch != 13 && (int) ch != 12) || (int) ch2 == 92))
          {
            ch3 = ch2;
            ch2 = ch;
          }
          else
            goto label_14;
        }
        else
          break;
      }
      if (inProperty)
        this.tokenBuilder.MarkPropertyAsDeleted();
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
label_7:
      if (inProperty)
        this.tokenBuilder.MarkPropertyAsDeleted();
      charClass = ParseSupport.GetCharClass(parseBuffer[parseCurrent]);
      return parseBuffer[parseCurrent];
label_14:
      ch = parseBuffer[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      if (flag)
      {
        if (inProperty)
          this.tokenBuilder.MarkPropertyAsDeleted();
        else
          this.isInvalid = true;
      }
      return ch;
    }

    private char ScanName(CssRunKind runKind, char ch, ref CharClass charClass, ref int parseCurrent)
    {
      while (true)
      {
        int start1 = parseCurrent;
        while (CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)) && parseCurrent != this.parseEnd)
          ch = this.parseBuffer[++parseCurrent];
        if (parseCurrent != start1)
          this.PrepareAndAddRun(runKind, start1, ref parseCurrent);
        if (parseCurrent != this.parseEnd)
        {
          int start2 = parseCurrent;
          if ((int) ch == 92)
          {
            if (CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent) && CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
            {
              ++parseCurrent;
              this.PrepareAndAddLiteralRun(runKind, start2, ref parseCurrent, (int) ch);
              if (parseCurrent != this.parseEnd)
                ch = this.parseBuffer[parseCurrent];
              else
                goto label_11;
            }
            else
              break;
          }
          else
            goto label_11;
        }
        else
          goto label_11;
      }
      ch = this.parseBuffer[++parseCurrent];
      this.PrepareAndAddInvalidRun(runKind, ref parseCurrent);
label_11:
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
    }

    private char ScanIdent(CssRunKind runKind, char ch, ref CharClass charClass, ref int parseCurrent, out int nameLength)
    {
      bool flag = false;
      nameLength = 0;
      while (true)
      {
        int start1 = parseCurrent;
        for (; CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)); ch = this.parseBuffer[++parseCurrent])
        {
          if (nameLength == 0 && (int) ch == 45)
            flag = true;
          if (nameLength == 1 && flag && char.IsDigit(ch))
          {
            nameLength = 0;
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
          }
          ++nameLength;
          if (parseCurrent == this.parseEnd)
            break;
        }
        if (parseCurrent != start1)
          this.PrepareAndAddRun(runKind, start1, ref parseCurrent);
        if (parseCurrent != this.parseEnd)
        {
          int start2 = parseCurrent;
          if ((int) ch == 92)
          {
            if (CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent) && CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
            {
              ++parseCurrent;
              if (nameLength == 0 && (int) ch == 45)
                flag = true;
              if (nameLength != 1 || !flag || !char.IsDigit(ch))
              {
                ++nameLength;
                this.PrepareAndAddLiteralRun(runKind, start2, ref parseCurrent, (int) ch);
                if (parseCurrent != this.parseEnd)
                  ch = this.parseBuffer[parseCurrent];
                else
                  goto label_21;
              }
              else
                goto label_18;
            }
            else
              break;
          }
          else
            goto label_21;
        }
        else
          goto label_21;
      }
      ch = this.parseBuffer[++parseCurrent];
      this.PrepareAndAddInvalidRun(runKind, ref parseCurrent);
      nameLength = 0;
      goto label_21;
label_18:
      nameLength = 0;
label_21:
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
    }

    private char ScanUrl(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      while (true)
      {
        int start1 = parseCurrent;
        while (this.IsUrlCharacter(ch, ParseSupport.GetCharClass(ch), parseCurrent) && parseCurrent != this.parseEnd)
          ch = this.parseBuffer[++parseCurrent];
        if (parseCurrent != start1)
          this.PrepareAndAddRun(CssRunKind.Url, start1, ref parseCurrent);
        if (parseCurrent != this.parseEnd)
        {
          int start2 = parseCurrent;
          if ((int) ch == 92)
          {
            if (CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
            {
              ++parseCurrent;
              this.PrepareAndAddLiteralRun(CssRunKind.Url, start2, ref parseCurrent, (int) ch);
              if (parseCurrent != this.parseEnd)
                ch = this.parseBuffer[parseCurrent];
              else
                goto label_11;
            }
            else
              break;
          }
          else
            goto label_11;
        }
        else
          goto label_11;
      }
      ch = this.parseBuffer[++parseCurrent];
      this.PrepareAndAddInvalidRun(CssRunKind.Url, ref parseCurrent);
label_11:
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
    }

    private char ScanUnicodeRange(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      char[] chArray = this.parseBuffer;
      int num = parseCurrent + 1;
      int index1 = num;
      bool flag = true;
      for (; index1 < num + 6; ++index1)
      {
        char ch1 = chArray[index1];
        if (63 == (int) ch1)
        {
          flag = false;
          ++index1;
          while (index1 < num + 6 && 63 == (int) chArray[index1])
            ++index1;
          break;
        }
        if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(ch1)))
        {
          if (index1 == num)
            return ch;
          break;
        }
      }
      if (45 == (int) chArray[index1] && flag)
      {
        ++index1;
        for (int index2 = index1; index1 < index2 + 6; ++index1)
        {
          if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(chArray[index1])))
          {
            if (index1 == index2)
              return ch;
            break;
          }
        }
      }
      char ch2 = chArray[index1];
      charClass = ParseSupport.GetCharClass(ch2);
      parseCurrent = index1;
      return ch2;
    }

    private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent, bool ignorable)
    {
      char[] chArray = this.parseBuffer;
      int num = this.parseEnd;
label_11:
      while (ParseSupport.WhitespaceCharacter(charClass) || (int) ch == 47)
      {
        if ((int) ch == 47)
        {
          if (parseCurrent < num && (int) chArray[parseCurrent + 1] == 42)
          {
            ch = this.ScanComment(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == num)
              return ch;
          }
          else
            break;
        }
        else
        {
          int start = parseCurrent;
          while (++parseCurrent != num)
          {
            ch = chArray[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            if (!ParseSupport.WhitespaceCharacter(charClass))
            {
              if (this.tokenBuilder.IsStarted)
              {
                this.PrepareAndAddRun(ignorable ? CssRunKind.Invalid : CssRunKind.Space, start, ref parseCurrent);
                goto label_11;
              }
              else
                goto label_11;
            }
          }
          return ch;
        }
      }
      return ch;
    }

    private char ScanComment(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      char[] chArray = this.parseBuffer;
      int num = this.parseEnd;
      int start = parseCurrent;
      ch = chArray[++parseCurrent];
      while (++parseCurrent != num)
      {
        if ((int) chArray[parseCurrent] == 42 && parseCurrent + 1 != num && (int) chArray[parseCurrent + 1] == 47)
        {
          ++parseCurrent;
          if (++parseCurrent == num)
            return ch;
          if (this.tokenBuilder.IsStarted)
            this.PrepareAndAddRun(CssRunKind.Space, start, ref parseCurrent);
          ch = chArray[parseCurrent];
          charClass = ParseSupport.GetCharClass(ch);
          return ch;
        }
      }
      return ch;
    }

    private void PrepareAndAddRun(CssRunKind runKind, int start, ref int parseCurrent)
    {
      if (this.tokenBuilder.PrepareAndAddRun(runKind, start, parseCurrent))
        return;
      parseCurrent = this.parseEnd;
    }

    private void PrepareAndAddInvalidRun(CssRunKind runKind, ref int parseCurrent)
    {
      if (this.tokenBuilder.PrepareAndAddInvalidRun(runKind, parseCurrent))
        return;
      parseCurrent = this.parseEnd;
    }

    private void PrepareAndAddLiteralRun(CssRunKind runKind, int start, ref int parseCurrent, int value)
    {
      if (this.tokenBuilder.PrepareAndAddLiteralRun(runKind, start, parseCurrent, value))
        return;
      parseCurrent = this.parseEnd;
    }

    private char SkipToNextRule(char ch, ref CharClass charClass, ref int parseCurrent)
    {
      int num = this.parseEnd;
      char[] chArray = this.parseBuffer;
      while (true)
      {
        while ((int) ch == 34 || (int) ch == 39)
        {
          ch = this.ScanString(ch, ref charClass, ref parseCurrent, false);
          if (parseCurrent == num)
            return ch;
        }
        if ((int) ch == 123)
          ++this.ruleDepth;
        else if ((int) ch == 125)
        {
          if (this.ruleDepth > 0)
            --this.ruleDepth;
          if (this.ruleDepth == 0)
            break;
        }
        else if ((int) ch == 59 && this.ruleDepth == 0)
          goto label_12;
        if (++parseCurrent != num)
          ch = chArray[parseCurrent];
        else
          goto label_14;
      }
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
label_12:
      ch = chArray[++parseCurrent];
      charClass = ParseSupport.GetCharClass(ch);
      return ch;
label_14:
      return ch;
    }

    private bool IsSafeIdentifier(string[] table, int start, int end)
    {
      int length = end - start;
      for (int index = 0; index < table.Length; ++index)
      {
        if (this.IsNameEqual(table[index], start, length))
          return true;
      }
      return false;
    }

    private bool IsNameEqual(string name, int start, int length)
    {
      return name.Equals(new string(this.parseBuffer, start, length), StringComparison.OrdinalIgnoreCase);
    }

    private bool IsNameCharacter(char ch, CharClass charClass, int parseCurrent)
    {
      if (!this.IsNameStartCharacter(ch, charClass, parseCurrent) && !ParseSupport.NumericCharacter(charClass))
        return (int) ch == 45;
      return true;
    }

    private bool IsIdentStartCharacter(char ch, CharClass charClass, int parseCurrent)
    {
      if (CssParser.IsNameStartCharacterNoEscape(ch, charClass) || (int) ch == 45)
        return true;
      if (!CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
        return false;
      charClass = ParseSupport.GetCharClass(ch);
      if (!CssParser.IsNameStartCharacterNoEscape(ch, charClass))
        return (int) ch == 45;
      return true;
    }

    private bool IsNameStartCharacter(char ch, CharClass charClass, int parseCurrent)
    {
      if (CssParser.IsNameStartCharacterNoEscape(ch, charClass))
        return true;
      if (!CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
        return false;
      charClass = ParseSupport.GetCharClass(ch);
      return CssParser.IsNameStartCharacterNoEscape(ch, charClass);
    }

    private bool IsUrlCharacter(char ch, CharClass charClass, int parseCurrent)
    {
      if (!CssParser.IsUrlCharacterNoEscape(ch, charClass))
        return this.IsEscape(ch, parseCurrent);
      return true;
    }

    private bool IsEscape(char ch, int parseCurrent)
    {
      return CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent);
    }
  }
}
