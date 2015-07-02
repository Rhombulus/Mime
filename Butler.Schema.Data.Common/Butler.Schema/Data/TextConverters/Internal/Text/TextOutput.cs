// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Text.TextOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Text
{
  internal class TextOutput : IRestartable, IReusable, IFallback, IDisposable
  {
    private static readonly char[] Whitespaces = new char[5]
    {
      ' ',
      '\t',
      '\r',
      '\n',
      '\f'
    };
    protected bool signaturePossible = true;
    protected ConverterOutput output;
    protected bool lineWrapping;
    protected bool rfc2646;
    protected int longestNonWrappedParagraph;
    protected int wrapBeforePosition;
    protected bool preserveTrailingSpace;
    protected bool preserveTabulation;
    protected bool preserveNbsp;
    protected int lineLength;
    protected int lineLengthBeforeSoftWrap;
    protected int flushedLength;
    protected int tailSpace;
    protected int breakOpportunity;
    protected int nextBreakOpportunity;
    protected int quotingLevel;
    protected bool seenSpace;
    protected bool wrapped;
    protected char[] wrapBuffer;
    protected bool anyNewlines;
    protected bool endParagraph;
    private bool fallbacks;
    private bool htmlEscape;
    private string anchorUrl;
    private int linePosition;
    private ImageRenderingCallbackInternal imageRenderingCallback;

    public bool OutputCodePageSameAsInput
    {
      get
      {
        if (this.output is ConverterEncodingOutput)
          return (this.output as ConverterEncodingOutput).CodePageSameAsInput;
        return false;
      }
    }

    public Encoding OutputEncoding
    {
      set
      {
        if (!(this.output is ConverterEncodingOutput))
          throw new InvalidOperationException();
        (this.output as ConverterEncodingOutput).Encoding = value;
      }
    }

    public bool LineEmpty
    {
      get
      {
        if (this.lineLength == 0)
          return this.tailSpace == 0;
        return false;
      }
    }

    public bool ImageRenderingCallbackDefined => this.imageRenderingCallback != null;

      public TextOutput(ConverterOutput output, bool lineWrapping, bool flowed, int wrapBeforePosition, int longestNonWrappedParagraph, ImageRenderingCallbackInternal imageRenderingCallback, bool fallbacks, bool htmlEscape, bool preserveSpace, Stream testTraceStream)
    {
      this.rfc2646 = flowed;
      this.lineWrapping = lineWrapping;
      this.wrapBeforePosition = wrapBeforePosition;
      this.longestNonWrappedParagraph = longestNonWrappedParagraph;
      if (!this.lineWrapping)
      {
        this.preserveTrailingSpace = preserveSpace;
        this.preserveTabulation = preserveSpace;
        this.preserveNbsp = preserveSpace;
      }
      this.output = output;
      this.fallbacks = fallbacks;
      this.htmlEscape = htmlEscape;
      this.imageRenderingCallback = imageRenderingCallback;
      this.wrapBuffer = new char[(this.longestNonWrappedParagraph + 1) * 5];
    }

    public void OpenDocument()
    {
    }

    public void CloseDocument()
    {
      if (!this.anyNewlines)
        this.output.Write("\r\n");
      this.endParagraph = false;
    }

    public void SetQuotingLevel(int quotingLevel)
    {
      this.quotingLevel = Math.Min(quotingLevel, this.wrapBeforePosition / 2);
    }

    public void CloseParagraph()
    {
      if (this.lineLength != 0 || this.tailSpace != 0)
        this.OutputNewLine();
      this.endParagraph = true;
    }

    public void OutputNewLine()
    {
      if (this.lineWrapping)
      {
        this.FlushLine('\n');
        if (this.signaturePossible && this.lineLength == 2 && this.tailSpace == 1)
        {
          this.output.Write(' ');
          ++this.lineLength;
        }
      }
      else if (this.preserveTrailingSpace && this.tailSpace != 0)
        this.FlushTailSpace();
      if (!this.endParagraph)
      {
        this.output.Write("\r\n");
        this.anyNewlines = true;
        this.linePosition += 2;
      }
      this.linePosition += this.lineLength;
      this.lineLength = 0;
      this.lineLengthBeforeSoftWrap = 0;
      this.flushedLength = 0;
      this.tailSpace = 0;
      this.breakOpportunity = 0;
      this.nextBreakOpportunity = 0;
      this.wrapped = false;
      this.seenSpace = false;
      this.signaturePossible = true;
    }

    public void OutputTabulation(int count)
    {
      if (this.preserveTabulation)
      {
        for (; count != 0; --count)
          this.OutputNonspace("\t", TextMapping.Unicode);
      }
      else
      {
        count = (this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace) / 8 * 8 + 8 * count - (this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace);
        this.OutputSpace(count);
      }
    }

    public void OutputSpace(int count)
    {
      if (this.lineWrapping)
      {
        if (this.breakOpportunity == 0 || this.lineLength + this.tailSpace <= this.WrapBeforePosition())
        {
          this.breakOpportunity = this.lineLength + this.tailSpace;
          if (this.lineLength + this.tailSpace < this.WrapBeforePosition() && count > 1)
            this.breakOpportunity += Math.Min(this.WrapBeforePosition() - (this.lineLength + this.tailSpace), count - 1);
          if (this.breakOpportunity < this.lineLength + this.tailSpace + count - 1)
            this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
          if (this.lineLength > this.flushedLength)
            this.FlushLine(' ');
        }
        else
          this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
      }
      this.tailSpace += count;
    }

    public void OutputNbsp(int count)
    {
      if (this.preserveNbsp)
      {
        for (; count != 0; --count)
          this.OutputNonspace(" ", TextMapping.Unicode);
      }
      else
        this.tailSpace += count;
    }

    public void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
    {
      if (!this.lineWrapping && !this.endParagraph && textMapping == TextMapping.Unicode)
      {
        if (this.tailSpace != 0)
          this.FlushTailSpace();
        this.output.Write(buffer, offset, count, this.fallbacks ? (IFallback) this : (IFallback) null);
        this.lineLength += count;
      }
      else
        this.OutputNonspaceImpl(buffer, offset, count, textMapping);
    }

    public void OutputNonspace(string text, TextMapping textMapping)
    {
      this.OutputNonspace(text, 0, text.Length, textMapping);
    }

    public void OutputNonspace(string text, int offset, int length, TextMapping textMapping)
    {
      if (textMapping != TextMapping.Unicode)
      {
        for (int index = offset; index < length; ++index)
          this.MapAndOutputSymbolCharacter(text[index], textMapping);
      }
      else
      {
        if (this.endParagraph)
        {
          this.output.Write("\r\n");
          this.linePosition += 2;
          this.anyNewlines = true;
          this.endParagraph = false;
        }
        if (this.lineWrapping)
        {
          if (length == 0)
            return;
          this.WrapPrepareToAppendNonspace(length);
          if (this.breakOpportunity == 0)
          {
            this.FlushLine(text[offset]);
            this.output.Write(text, offset, length, this.fallbacks ? (IFallback) this : (IFallback) null);
            this.flushedLength += length;
          }
          else
            text.CopyTo(offset, this.wrapBuffer, this.lineLength - this.flushedLength, length);
          this.lineLength += length;
          if (this.lineLength <= 2 && (int) text[offset] == 45 && (length != 2 || (int) text[offset + 1] == 45))
            return;
          this.signaturePossible = false;
        }
        else
        {
          if (this.tailSpace != 0)
            this.FlushTailSpace();
          this.output.Write(text, offset, length, this.fallbacks ? (IFallback) this : (IFallback) null);
          this.lineLength += length;
        }
      }
    }

    public void OutputNonspace(int ucs32Literal, TextMapping textMapping)
    {
      if (textMapping != TextMapping.Unicode)
      {
        this.MapAndOutputSymbolCharacter((char) ucs32Literal, textMapping);
      }
      else
      {
        if (this.endParagraph)
        {
          this.output.Write("\r\n");
          this.linePosition += 2;
          this.anyNewlines = true;
          this.endParagraph = false;
        }
        if (this.lineWrapping)
        {
          int count = Token.LiteralLength(ucs32Literal);
          this.WrapPrepareToAppendNonspace(count);
          if (this.breakOpportunity == 0)
          {
            this.FlushLine(Token.LiteralFirstChar(ucs32Literal));
            this.output.Write(ucs32Literal, this.fallbacks ? (IFallback) this : (IFallback) null);
            this.flushedLength += count;
          }
          else
          {
            this.wrapBuffer[this.lineLength - this.flushedLength] = Token.LiteralFirstChar(ucs32Literal);
            if (count != 1)
              this.wrapBuffer[this.lineLength - this.flushedLength + 1] = Token.LiteralLastChar(ucs32Literal);
          }
          this.lineLength += count;
          if (this.lineLength <= 2 && count == 1 && (int) (ushort) ucs32Literal == 45)
            return;
          this.signaturePossible = false;
        }
        else
        {
          if (this.tailSpace != 0)
            this.FlushTailSpace();
          this.output.Write(ucs32Literal, this.fallbacks ? (IFallback) this : (IFallback) null);
          this.lineLength += Token.LiteralLength(ucs32Literal);
        }
      }
    }

    public void OpenAnchor(string anchorUrl)
    {
      this.anchorUrl = anchorUrl;
    }

    public void CloseAnchor()
    {
      if (this.anchorUrl == null)
        return;
      bool flag = this.tailSpace != 0;
      string text = this.anchorUrl;
      if (text.IndexOf(' ') != -1)
        text = text.Replace(" ", "%20");
      this.OutputNonspace("<", TextMapping.Unicode);
      this.OutputNonspace(text, TextMapping.Unicode);
      this.OutputNonspace(">", TextMapping.Unicode);
      if (flag)
        this.OutputSpace(1);
      this.anchorUrl = (string) null;
    }

    public void CancelAnchor()
    {
      this.anchorUrl = (string) null;
    }

    public void OutputImage(string imageUrl, string imageAltText, int wdthPixels, int heightPixels)
    {
      if (this.imageRenderingCallback != null && this.imageRenderingCallback(imageUrl, this.RenderingPosition()))
      {
        this.OutputSpace(1);
      }
      else
      {
        if (wdthPixels != 0 && wdthPixels < 8 || heightPixels != 0 && heightPixels < 8)
          return;
        bool flag = this.tailSpace != 0;
        this.OutputNonspace("[", TextMapping.Unicode);
        int num;
        if (!string.IsNullOrEmpty(imageAltText))
        {
          for (int index = 0; index != imageAltText.Length; index = num + 1)
          {
            num = imageAltText.IndexOfAny(TextOutput.Whitespaces, index);
            if (num == -1)
            {
              this.OutputNonspace(imageAltText, index, imageAltText.Length - index, TextMapping.Unicode);
              break;
            }
            if (num != index)
              this.OutputNonspace(imageAltText, index, num - index, TextMapping.Unicode);
            if ((int) imageAltText[index] == 9)
              this.OutputTabulation(1);
            else
              this.OutputSpace(1);
          }
        }
        else if (!string.IsNullOrEmpty(imageUrl))
        {
          if (imageUrl.Contains("/") && !imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            imageUrl = "X";
          else if (imageUrl.IndexOf(' ') != -1)
            imageUrl = imageUrl.Replace(" ", "%20");
          this.OutputNonspace(imageUrl, TextMapping.Unicode);
        }
        else
          this.OutputNonspace("X", TextMapping.Unicode);
        this.OutputNonspace("]", TextMapping.Unicode);
        if (!flag)
          return;
        this.OutputSpace(1);
      }
    }

    public int RenderingPosition()
    {
      return this.linePosition + this.lineLength + this.tailSpace;
    }

    public void Flush()
    {
      if (this.lineWrapping)
      {
        if (this.lineLength != 0)
        {
          this.FlushLine('\r');
          this.OutputNewLine();
        }
      }
      else if (this.lineLength != 0)
        this.OutputNewLine();
      this.output.Flush();
    }

    byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
    {
      if (this.htmlEscape)
      {
        unsafeAsciiMask = (byte) 1;
        return Html.HtmlSupport.UnsafeAsciiMap;
      }
      unsafeAsciiMask = (byte) 0;
      return (byte[]) null;
    }

    bool IFallback.HasUnsafeUnicode()
    {
      return this.htmlEscape;
    }

    bool IFallback.TreatNonAsciiAsUnsafe(string charset)
    {
      return false;
    }

    bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
    {
      if (!this.htmlEscape || (int) ch >= 55296 && (int) ch < 57344)
        return false;
      if ((int) (byte) ((uint) ch & (uint) byte.MaxValue) != 60)
        return (int) (byte) ((int) ch >> 8 & (int) byte.MaxValue) == 60;
      return true;
    }

    bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
    {
      if (this.htmlEscape)
      {
        Html.HtmlEntityIndex htmlEntityIndex = (Html.HtmlEntityIndex) 0;
        if ((int) ch <= 62)
        {
          if ((int) ch == 62)
            htmlEntityIndex = Html.HtmlEntityIndex.gt;
          else if ((int) ch == 60)
            htmlEntityIndex = Html.HtmlEntityIndex.lt;
          else if ((int) ch == 38)
            htmlEntityIndex = Html.HtmlEntityIndex.amp;
          else if ((int) ch == 34)
            htmlEntityIndex = Html.HtmlEntityIndex.quot;
        }
        else if (160 <= (int) ch && (int) ch <= (int) byte.MaxValue)
          htmlEntityIndex = Html.HtmlSupport.EntityMap[(int) ch - 160];
        if (htmlEntityIndex != (Html.HtmlEntityIndex) 0)
        {
          string str = Html.HtmlNameData.entities[(int) htmlEntityIndex].Name;
          if (outputEnd - outputBufferCount < str.Length + 2)
            return false;
          outputBuffer[outputBufferCount++] = '&';
          str.CopyTo(0, outputBuffer, outputBufferCount, str.Length);
          outputBufferCount += str.Length;
          outputBuffer[outputBufferCount++] = ';';
        }
        else
        {
          uint num1 = (uint) ch;
          int num2 = num1 < 16U ? 1 : (num1 < 256U ? 2 : (num1 < 4096U ? 3 : 4));
          if (outputEnd - outputBufferCount < num2 + 4)
            return false;
          outputBuffer[outputBufferCount++] = '&';
          outputBuffer[outputBufferCount++] = '#';
          outputBuffer[outputBufferCount++] = 'x';
          int num3 = outputBufferCount + num2;
          while ((int) num1 != 0)
          {
            uint num4 = num1 & 15U;
            outputBuffer[--num3] = (char) ((long) num4 + (num4 < 10U ? 48L : 55L));
            num1 >>= 4;
          }
          outputBufferCount += num2;
          outputBuffer[outputBufferCount++] = ';';
        }
      }
      else
      {
        string substitute = TextOutput.GetSubstitute(ch);
        if (substitute != null)
        {
          if (outputEnd - outputBufferCount < substitute.Length)
            return false;
          substitute.CopyTo(0, outputBuffer, outputBufferCount, substitute.Length);
          outputBufferCount += substitute.Length;
        }
        else
          outputBuffer[outputBufferCount++] = ch;
      }
      return true;
    }

    void IDisposable.Dispose()
    {
      if (this.output != null)
        this.output.Dispose();
      this.output = (ConverterOutput) null;
      this.wrapBuffer = (char[]) null;
      GC.SuppressFinalize((object) this);
    }

    bool IRestartable.CanRestart()
    {
      if (this.output is IRestartable)
        return ((IRestartable) this.output).CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      ((IRestartable) this.output).Restart();
      this.Reinitialize();
    }

    void IRestartable.DisableRestart()
    {
      if (!(this.output is IRestartable))
        return;
      ((IRestartable) this.output).DisableRestart();
    }

    void IReusable.Initialize(object newSourceOrDestination)
    {
      ((IReusable) this.output).Initialize(newSourceOrDestination);
      this.Reinitialize();
    }

    private void Reinitialize()
    {
      this.anchorUrl = (string) null;
      this.linePosition = 0;
      this.lineLength = 0;
      this.lineLengthBeforeSoftWrap = 0;
      this.flushedLength = 0;
      this.tailSpace = 0;
      this.breakOpportunity = 0;
      this.nextBreakOpportunity = 0;
      this.quotingLevel = 0;
      this.seenSpace = false;
      this.wrapped = false;
      this.signaturePossible = true;
      this.anyNewlines = false;
      this.endParagraph = false;
    }

    private void OutputNonspaceImpl(char[] buffer, int offset, int count, TextMapping textMapping)
    {
      if (count == 0)
        return;
      if (textMapping != TextMapping.Unicode)
      {
        for (int index = 0; index < count; ++index)
          this.MapAndOutputSymbolCharacter(buffer[offset++], textMapping);
      }
      else
      {
        if (this.endParagraph)
        {
          this.output.Write("\r\n");
          this.linePosition += 2;
          this.anyNewlines = true;
          this.endParagraph = false;
        }
        if (this.lineWrapping)
        {
          this.WrapPrepareToAppendNonspace(count);
          if (this.breakOpportunity == 0)
          {
            this.FlushLine(buffer[offset]);
            this.output.Write(buffer, offset, count, this.fallbacks ? (IFallback) this : (IFallback) null);
            this.flushedLength += count;
          }
          else
            Buffer.BlockCopy((Array) buffer, offset * 2, (Array) this.wrapBuffer, (this.lineLength - this.flushedLength) * 2, count * 2);
          this.lineLength += count;
          if (this.lineLength <= 2 && (int) buffer[offset] == 45 && (count != 2 || (int) buffer[offset + 1] == 45))
            return;
          this.signaturePossible = false;
        }
        else
        {
          if (this.tailSpace != 0)
            this.FlushTailSpace();
          this.output.Write(buffer, offset, count, this.fallbacks ? (IFallback) this : (IFallback) null);
          this.lineLength += count;
        }
      }
    }

    private int WrapBeforePosition()
    {
      return this.wrapBeforePosition - (this.rfc2646 ? this.quotingLevel + 1 : 0);
    }

    private int LongestNonWrappedParagraph()
    {
      return this.longestNonWrappedParagraph - (this.rfc2646 ? this.quotingLevel + 1 : 0);
    }

    private void WrapPrepareToAppendNonspace(int count)
    {
      while (this.breakOpportunity != 0 && this.lineLength + this.tailSpace + count > (this.wrapped ? this.WrapBeforePosition() : this.LongestNonWrappedParagraph()))
      {
        if (this.flushedLength == 0 && this.rfc2646)
        {
          for (int index = 0; index < this.quotingLevel; ++index)
            this.output.Write('>');
          if (this.quotingLevel != 0 || (int) this.wrapBuffer[0] == 62 || (int) this.wrapBuffer[0] == 32)
            this.output.Write(' ');
        }
        if (this.breakOpportunity >= this.lineLength)
        {
          do
          {
            if (this.lineLength - this.flushedLength == this.wrapBuffer.Length)
            {
              this.output.Write(this.wrapBuffer, 0, this.wrapBuffer.Length, this.fallbacks ? (IFallback) this : (IFallback) null);
              this.flushedLength += this.wrapBuffer.Length;
            }
            this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';
            ++this.lineLength;
            --this.tailSpace;
          }
          while (this.lineLength != this.breakOpportunity + 1);
        }
        this.output.Write(this.wrapBuffer, 0, this.breakOpportunity + 1 - this.flushedLength, this.fallbacks ? (IFallback) this : (IFallback) null);
        this.anyNewlines = true;
        this.output.Write("\r\n");
        this.wrapped = true;
        this.lineLengthBeforeSoftWrap += this.breakOpportunity + 1;
        this.linePosition += this.breakOpportunity + 1 + 2;
        this.lineLength -= this.breakOpportunity + 1;
        int num = this.flushedLength;
        this.flushedLength = 0;
        if (this.lineLength != 0)
        {
          if (this.nextBreakOpportunity == 0 || this.nextBreakOpportunity - (this.breakOpportunity + 1) >= this.lineLength || this.nextBreakOpportunity - (this.breakOpportunity + 1) == 0)
          {
            if (this.rfc2646)
            {
              for (int index = 0; index < this.quotingLevel; ++index)
                this.output.Write('>');
              if (this.quotingLevel != 0 || (int) this.wrapBuffer[this.breakOpportunity + 1 - num] == 62 || (int) this.wrapBuffer[this.breakOpportunity + 1 - num] == 32)
                this.output.Write(' ');
            }
            this.output.Write(this.wrapBuffer, this.breakOpportunity + 1 - num, this.lineLength, this.fallbacks ? (IFallback) this : (IFallback) null);
            this.flushedLength = this.lineLength;
          }
          else
            Buffer.BlockCopy((Array) this.wrapBuffer, (this.breakOpportunity + 1 - num) * 2, (Array) this.wrapBuffer, 0, this.lineLength * 2);
        }
        if (this.nextBreakOpportunity != 0)
        {
          this.breakOpportunity = this.nextBreakOpportunity - (this.breakOpportunity + 1);
          if (this.breakOpportunity > this.WrapBeforePosition())
          {
            if (this.lineLength < this.WrapBeforePosition())
            {
              this.nextBreakOpportunity = this.breakOpportunity;
              this.breakOpportunity = this.WrapBeforePosition();
            }
            else if (this.breakOpportunity > this.lineLength)
            {
              this.nextBreakOpportunity = this.breakOpportunity;
              this.breakOpportunity = this.lineLength;
            }
            else
              this.nextBreakOpportunity = 0;
          }
          else
            this.nextBreakOpportunity = 0;
        }
        else
          this.breakOpportunity = 0;
      }
      if (this.tailSpace == 0)
        return;
      if (this.breakOpportunity == 0)
      {
        if (this.flushedLength == 0 && this.rfc2646)
        {
          for (int index = 0; index < this.quotingLevel; ++index)
            this.output.Write('>');
          this.output.Write(' ');
        }
        this.flushedLength += this.tailSpace;
        this.FlushTailSpace();
      }
      else
      {
        do
        {
          this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';
          ++this.lineLength;
          --this.tailSpace;
        }
        while (this.tailSpace != 0);
      }
    }

    private void FlushLine(char nextChar)
    {
      if (this.flushedLength == 0 && this.rfc2646)
      {
        for (int index = 0; index < this.quotingLevel; ++index)
          this.output.Write('>');
        char ch = this.lineLength != 0 ? this.wrapBuffer[0] : nextChar;
        if (this.quotingLevel != 0 || (int) ch == 62 || (int) ch == 32)
          this.output.Write(' ');
      }
      if (this.lineLength == this.flushedLength)
        return;
      this.output.Write(this.wrapBuffer, 0, this.lineLength - this.flushedLength, this.fallbacks ? (IFallback) this : (IFallback) null);
      this.flushedLength = this.lineLength;
    }

    private void FlushTailSpace()
    {
      this.lineLength += this.tailSpace;
      do
      {
        this.output.Write(' ');
        --this.tailSpace;
      }
      while (this.tailSpace != 0);
    }

    private void MapAndOutputSymbolCharacter(char ch, TextMapping textMapping)
    {
      if ((int) ch == 32 || (int) ch == 9 || ((int) ch == 13 || (int) ch == 10))
      {
        this.OutputNonspace((int) ch, TextMapping.Unicode);
      }
      else
      {
        string text = (string) null;
        if (textMapping == TextMapping.Wingdings)
        {
          switch (ch)
          {
            case 'ß':
              text = "<--";
              break;
            case 'à':
              text = "-->";
              break;
            case 'ç':
              text = "<==";
              break;
            case 'è':
              text = "==>";
              break;
            case 'ï':
              text = "<=";
              break;
            case 'ð':
              text = "=>";
              break;
            case 'ó':
              text = "<=>";
              break;
            case 'J':
              text = "☺";
              break;
            case 'K':
              text = ":|";
              break;
            case 'L':
              text = "☹";
              break;
            case 'Ø':
              text = ">";
              break;
          }
        }
        if (text == null)
          text = "•";
        this.OutputNonspace(text, TextMapping.Unicode);
      }
    }

    private static string GetSubstitute(char ch)
    {
      return Globalization.AsciiEncoderFallback.GetCharacterFallback(ch);
    }
  }
}
