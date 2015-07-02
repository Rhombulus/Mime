// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlWriter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  public class HtmlWriter : IRestartable, IFallback, IDisposable, ITextSinkEx, ITextSink
  {
    private ConverterOutput output;
    private HtmlWriter.OutputState outputState;
    private bool filterHtml;
    private bool autoNewLines;
    private bool allowWspBeforeFollowingTag;
    private bool lastWhitespace;
      private int longestLineLength;
    private int textLineLength;
      private bool literalTags;
    private bool literalEntities;
    private bool cssEscaping;
    private IFallback fallback;
    private Internal.Html.HtmlNameIndex tagNameIndex;
    private Internal.Html.HtmlNameIndex previousTagNameIndex;
    private bool isEndTag;
    private bool isEmptyScopeTag;

      public HtmlWriterState WriterState
    {
      get
      {
        if (this.outputState == HtmlWriter.OutputState.OutsideTag)
          return HtmlWriterState.Default;
        return this.outputState >= HtmlWriter.OutputState.WritingAttributeName ? HtmlWriterState.Attribute : HtmlWriterState.Tag;
      }
    }

    bool ITextSink.IsEnough => false;

      internal bool HasEncoding => this.output is ConverterEncodingOutput;

      internal bool CodePageSameAsInput => (this.output as ConverterEncodingOutput).CodePageSameAsInput;

      internal Encoding Encoding
    {
      get
      {
        return (this.output as ConverterEncodingOutput).Encoding;
      }
      set
      {
        (this.output as ConverterEncodingOutput).Encoding = value;
      }
    }

    internal bool CanAcceptMore => this.output.CanAcceptMore;

      internal bool IsTagOpen => this.outputState != HtmlWriter.OutputState.OutsideTag;

      internal int LineLength { get; private set; }

      internal int LiteralWhitespaceNesting { get; private set; }

      internal bool IsCopyPending { get; private set; }

      public HtmlWriter(Stream output, Encoding outputEncoding)
    {
      if (output == null)
        throw new ArgumentNullException("output");
      if (outputEncoding == null)
        throw new ArgumentNullException("outputEncoding");
      this.output = (ConverterOutput) new ConverterEncodingOutput(output, true, false, outputEncoding, false, false, (IResultsFeedback) null);
      this.autoNewLines = true;
    }

    public HtmlWriter(TextWriter output)
    {
      if (output == null)
        throw new ArgumentNullException("output");
      this.output = (ConverterOutput) new ConverterUnicodeOutput((object) output, true, false);
      this.autoNewLines = true;
    }

    internal HtmlWriter(ConverterOutput output, bool filterHtml, bool autoNewLines)
    {
      this.output = output;
      this.filterHtml = filterHtml;
      this.autoNewLines = autoNewLines;
    }

    public void Flush()
    {
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      this.output.Flush();
    }

    public void WriteTag(HtmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (reader.TagId != HtmlTagId.Unknown)
      {
        this.WriteTagBegin(Internal.Html.HtmlNameData.TagIndex[(int) reader.TagId], (string) null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
      }
      else
      {
        this.WriteTagBegin(Internal.Html.HtmlNameIndex.Unknown, (string) null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
        reader.WriteTagNameTo((ITextSink) this.WriteTagName());
      }
      this.isEmptyScopeTag = reader.TokenKind == HtmlTokenKind.EmptyElementTag;
      if (reader.TokenKind != HtmlTokenKind.StartTag && reader.TokenKind != HtmlTokenKind.EmptyElementTag)
        return;
      HtmlAttributeReader attributeReader = reader.AttributeReader;
      while (attributeReader.ReadNext())
      {
        if (attributeReader.Id != HtmlAttributeId.Unknown)
          this.OutputAttributeName(Internal.Html.HtmlNameData.Names[(int) Internal.Html.HtmlNameData.attributeIndex[(int) attributeReader.Id]].Name);
        else
          attributeReader.WriteNameTo((ITextSink) this.WriteAttributeName());
        if (attributeReader.HasValue)
          attributeReader.WriteValueTo((ITextSink) this.WriteAttributeValue());
        this.OutputAttributeEnd();
        this.outputState = HtmlWriter.OutputState.BeforeAttribute;
      }
    }

    public void WriteStartTag(HtmlTagId id)
    {
      this.WriteTag(id, false);
    }

    public void WriteStartTag(string name)
    {
      this.WriteTag(name, false);
    }

    public void WriteEndTag(HtmlTagId id)
    {
      this.WriteTag(id, true);
      this.WriteTagEnd();
    }

    public void WriteEndTag(string name)
    {
      this.WriteTag(name, true);
      this.WriteTagEnd();
    }

    public void WriteEmptyElementTag(HtmlTagId id)
    {
      this.WriteTag(id, false);
      this.isEmptyScopeTag = true;
    }

    public void WriteEmptyElementTag(string name)
    {
      this.WriteTag(name, false);
      this.isEmptyScopeTag = true;
    }

    public void WriteAttribute(HtmlAttributeId id, string value)
    {
      if (id < HtmlAttributeId.Unknown || id >= (HtmlAttributeId) Internal.Html.HtmlNameData.attributeIndex.Length)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeIdInvalid, "id");
      if (id == HtmlAttributeId.Unknown)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeIdIsUnknown, "id");
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(Internal.Html.HtmlNameData.Names[(int) Internal.Html.HtmlNameData.attributeIndex[(int) id]].Name);
      if (value != null)
      {
        this.OutputAttributeValue(value);
        this.OutputAttributeEnd();
      }
      this.outputState = HtmlWriter.OutputState.BeforeAttribute;
    }

    public void WriteAttribute(string name, string value)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeNameIsEmpty, "name");
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(name);
      if (value != null)
      {
        this.OutputAttributeValue(value);
        this.OutputAttributeEnd();
      }
      this.outputState = HtmlWriter.OutputState.BeforeAttribute;
    }

    public void WriteAttribute(HtmlAttributeId id, char[] buffer, int index, int count)
    {
      if (id < HtmlAttributeId.Unknown || id >= (HtmlAttributeId) Internal.Html.HtmlNameData.attributeIndex.Length)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeIdInvalid, "id");
      if (id == HtmlAttributeId.Unknown)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeIdIsUnknown, "id");
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > buffer.Length - index)
        throw new ArgumentOutOfRangeException("count");
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(Internal.Html.HtmlNameData.Names[(int) Internal.Html.HtmlNameData.attributeIndex[(int) id]].Name);
      this.OutputAttributeValue(buffer, index, count);
      this.OutputAttributeEnd();
      this.outputState = HtmlWriter.OutputState.BeforeAttribute;
    }

    public void WriteAttribute(string name, char[] buffer, int index, int count)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeNameIsEmpty, "name");
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > buffer.Length - index)
        throw new ArgumentOutOfRangeException("count");
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(name);
      this.OutputAttributeValue(buffer, index, count);
      this.OutputAttributeEnd();
      this.outputState = HtmlWriter.OutputState.BeforeAttribute;
    }

    public void WriteAttribute(HtmlAttributeReader attributeReader)
    {
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      attributeReader.WriteNameTo((ITextSink) this.WriteAttributeName());
      if (attributeReader.HasValue)
        attributeReader.WriteValueTo((ITextSink) this.WriteAttributeValue());
      this.OutputAttributeEnd();
      this.outputState = HtmlWriter.OutputState.BeforeAttribute;
    }

    public void WriteAttributeName(HtmlAttributeId id)
    {
      if (id < HtmlAttributeId.Unknown || id >= (HtmlAttributeId) Internal.Html.HtmlNameData.attributeIndex.Length)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeIdInvalid, "id");
      if (id == HtmlAttributeId.Unknown)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeIdIsUnknown, "id");
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(Internal.Html.HtmlNameData.Names[(int) Internal.Html.HtmlNameData.attributeIndex[(int) id]].Name);
    }

    public void WriteAttributeName(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        throw new ArgumentException(CtsResources.TextConvertersStrings.AttributeNameIsEmpty, "name");
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(name);
    }

    public void WriteAttributeName(HtmlAttributeReader attributeReader)
    {
      if (this.outputState < HtmlWriter.OutputState.WritingTagName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.isEndTag)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.EndTagCannotHaveAttributes);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      attributeReader.WriteNameTo((ITextSink) this.WriteAttributeName());
    }

    public void WriteAttributeValue(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (this.outputState < HtmlWriter.OutputState.TagStarted)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.outputState < HtmlWriter.OutputState.WritingAttributeName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.AttributeNotStarted);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      this.OutputAttributeValue(value);
    }

    public void WriteAttributeValue(char[] buffer, int index, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > buffer.Length - index)
        throw new ArgumentOutOfRangeException("count");
      if (this.outputState < HtmlWriter.OutputState.TagStarted)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.outputState < HtmlWriter.OutputState.WritingAttributeName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.AttributeNotStarted);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      this.OutputAttributeValue(buffer, index, count);
    }

    public void WriteAttributeValue(HtmlAttributeReader attributeReader)
    {
      if (this.outputState < HtmlWriter.OutputState.TagStarted)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.TagNotStarted);
      if (this.outputState < HtmlWriter.OutputState.WritingAttributeName)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.AttributeNotStarted);
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (!attributeReader.HasValue)
        return;
      attributeReader.WriteValueTo((ITextSink) this.WriteAttributeValue());
    }

    public void WriteText(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (value.Length == 0)
        return;
      if (this.lastWhitespace)
        this.OutputLastWhitespace(value[0]);
      this.output.Write(value, (IFallback) this);
      this.LineLength += value.Length;
      this.textLineLength += value.Length;
      this.allowWspBeforeFollowingTag = false;
    }

    public void WriteText(char[] buffer, int index, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > buffer.Length - index)
        throw new ArgumentOutOfRangeException("count");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      this.WriteTextInternal(buffer, index, count);
    }

    public void WriteText(HtmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      reader.WriteTextTo((ITextSink) this.WriteText());
    }

    public void WriteMarkupText(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (this.lastWhitespace)
        this.OutputLastWhitespace(value[0]);
      this.output.Write(value, (IFallback) null);
      this.LineLength += value.Length;
      this.allowWspBeforeFollowingTag = false;
    }

    public void WriteMarkupText(char[] buffer, int index, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (index < 0 || index > buffer.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > buffer.Length - index)
        throw new ArgumentOutOfRangeException("count");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (this.lastWhitespace)
        this.OutputLastWhitespace(buffer[index]);
      this.output.Write(buffer, index, count, (IFallback) null);
      this.LineLength += count;
      this.allowWspBeforeFollowingTag = false;
    }

    public void WriteMarkupText(HtmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      reader.WriteMarkupTextTo((ITextSink) this.WriteMarkupText());
    }

    bool IRestartable.CanRestart()
    {
      if (this.output is IRestartable)
        return ((IRestartable) this.output).CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      if (this.output is IRestartable)
        ((IRestartable) this.output).Restart();
      this.allowWspBeforeFollowingTag = false;
      this.lastWhitespace = false;
      this.LineLength = 0;
      this.longestLineLength = 0;
      this.LiteralWhitespaceNesting = 0;
      this.literalTags = false;
      this.literalEntities = false;
      this.cssEscaping = false;
      this.tagNameIndex = Internal.Html.HtmlNameIndex._NOTANAME;
      this.previousTagNameIndex = Internal.Html.HtmlNameIndex._NOTANAME;
      this.isEndTag = false;
      this.isEmptyScopeTag = false;
      this.IsCopyPending = false;
      this.outputState = HtmlWriter.OutputState.OutsideTag;
    }

    void IRestartable.DisableRestart()
    {
      if (!(this.output is IRestartable))
        return;
      ((IRestartable) this.output).DisableRestart();
    }

    byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
    {
      if (this.literalEntities)
      {
        unsafeAsciiMask = (byte) 0;
        return (byte[]) null;
      }
      unsafeAsciiMask = !this.filterHtml ? (byte) 1 : (byte) 1;
      return Internal.Html.HtmlSupport.UnsafeAsciiMap;
    }

    bool IFallback.HasUnsafeUnicode()
    {
      return this.filterHtml;
    }

    bool IFallback.TreatNonAsciiAsUnsafe(string charset)
    {
      if (this.filterHtml)
        return charset.StartsWith("x-", StringComparison.OrdinalIgnoreCase);
      return false;
    }

    bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
    {
      if (!this.filterHtml || (int) ch >= 55296 && (int) ch < 57344)
        return false;
      if ((int) (byte) ((uint) ch & (uint) byte.MaxValue) != 60 && (int) (byte) ((int) ch >> 8 & (int) byte.MaxValue) != 60 && (isFirstChar || (int) ch != 65279))
        return CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.PrivateUse;
      return true;
    }

    bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
    {
      if (this.literalEntities)
      {
        if (this.cssEscaping)
        {
          uint num1 = (uint) ch;
          int num2 = num1 < 16U ? 1 : (num1 < 256U ? 2 : (num1 < 4096U ? 3 : 4));
          if (outputEnd - outputBufferCount < num2 + 2)
            return false;
          outputBuffer[outputBufferCount++] = '\\';
          int num3 = outputBufferCount + num2;
          while ((int) num1 != 0)
          {
            uint num4 = num1 & 15U;
            outputBuffer[--num3] = (char) ((long) num4 + (num4 < 10U ? 48L : 55L));
            num1 >>= 4;
          }
          outputBufferCount += num2;
          outputBuffer[outputBufferCount++] = ' ';
        }
        else
        {
          if (outputEnd - outputBufferCount < 1)
            return false;
          outputBuffer[outputBufferCount++] = this.filterHtml ? '?' : ch;
        }
      }
      else
      {
        Internal.Html.HtmlEntityIndex htmlEntityIndex = (Internal.Html.HtmlEntityIndex) 0;
        if ((int) ch <= 62)
        {
          if ((int) ch == 62)
            htmlEntityIndex = Internal.Html.HtmlEntityIndex.gt;
          else if ((int) ch == 60)
            htmlEntityIndex = Internal.Html.HtmlEntityIndex.lt;
          else if ((int) ch == 38)
            htmlEntityIndex = Internal.Html.HtmlEntityIndex.amp;
          else if ((int) ch == 34)
            htmlEntityIndex = Internal.Html.HtmlEntityIndex.quot;
        }
        else if (160 <= (int) ch && (int) ch <= (int) byte.MaxValue)
          htmlEntityIndex = Internal.Html.HtmlSupport.EntityMap[(int) ch - 160];
        if (htmlEntityIndex != (Internal.Html.HtmlEntityIndex) 0)
        {
          string str = Internal.Html.HtmlNameData.entities[(int) htmlEntityIndex].Name;
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
          int num2 = num1 < 10U ? 1 : (num1 < 100U ? 2 : (num1 < 1000U ? 3 : (num1 < 10000U ? 4 : 5)));
          if (outputEnd - outputBufferCount < num2 + 3)
            return false;
          outputBuffer[outputBufferCount++] = '&';
          outputBuffer[outputBufferCount++] = '#';
          int num3 = outputBufferCount + num2;
          while ((int) num1 != 0)
          {
            uint num4 = num1 % 10U;
            outputBuffer[--num3] = (char) (num4 + 48U);
            num1 /= 10U;
          }
          outputBufferCount += num2;
          outputBuffer[outputBufferCount++] = ';';
        }
      }
      return true;
    }

    void ITextSink.Write(char[] buffer, int offset, int count)
    {
      this.LineLength += count;
      this.textLineLength += count;
      this.output.Write(buffer, offset, count, this.fallback);
    }

    void ITextSink.Write(int ucs32Char)
    {
      ++this.LineLength;
      ++this.textLineLength;
      this.output.Write(ucs32Char, this.fallback);
    }

    void ITextSinkEx.Write(string text)
    {
      this.LineLength += text.Length;
      this.textLineLength += text.Length;
      this.output.Write(text, this.fallback);
    }

    void ITextSinkEx.WriteNewLine()
    {
      if (this.LineLength > this.longestLineLength)
        this.longestLineLength = this.LineLength;
      this.output.Write("\r\n");
      this.LineLength = 0;
    }

    public void Close()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal static Internal.Html.HtmlNameIndex LookupName(string name)
    {
      if (name.Length <= 14)
      {
        short num = (short) ((long) (uint) (HashCode.CalculateLowerCase(name) ^ 221) % 601L);
        int index = (int) Internal.Html.HtmlNameData.nameHashTable[(int) num];
        if (index > 0)
        {
          do
          {
            string str = Internal.Html.HtmlNameData.Names[index].Name;
            if (str.Length == name.Length && (int) str[0] == (int) ParseSupport.ToLowerCase(name[0]) && (name.Length == 1 || str.Equals(name, StringComparison.OrdinalIgnoreCase)))
              return (Internal.Html.HtmlNameIndex) index;
          }
          while ((int) Internal.Html.HtmlNameData.Names[++index].Hash == (int) num);
        }
      }
      return Internal.Html.HtmlNameIndex.Unknown;
    }

    internal void SetCopyPending(bool copyPending)
    {
      this.IsCopyPending = copyPending;
    }

    internal void WriteStartTag(Internal.Html.HtmlNameIndex nameIndex)
    {
      this.WriteTagBegin(nameIndex, (string) null, false, false, false);
    }

    internal void WriteEndTag(Internal.Html.HtmlNameIndex nameIndex)
    {
      this.WriteTagBegin(nameIndex, (string) null, true, false, false);
      this.WriteTagEnd();
    }

    internal void WriteEmptyElementTag(Internal.Html.HtmlNameIndex nameIndex)
    {
      this.WriteTagBegin(nameIndex, (string) null, true, false, false);
      this.isEmptyScopeTag = true;
    }

    internal void WriteTagBegin(Internal.Html.HtmlNameIndex nameIndex, string name, bool isEndTag, bool allowWspLeft, bool allowWspRight)
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (this.literalTags && nameIndex >= Internal.Html.HtmlNameIndex.Unknown && (!isEndTag || nameIndex != this.tagNameIndex))
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteOtherTagsInsideElement(Internal.Html.HtmlNameData.Names[(int) this.tagNameIndex].Name));
      Internal.Html.HtmlTagIndex htmlTagIndex = Internal.Html.HtmlNameData.Names[(int) nameIndex].TagIndex;
      if (nameIndex > Internal.Html.HtmlNameIndex.Unknown)
      {
        this.isEmptyScopeTag = Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].Scope == Internal.Html.HtmlDtd.TagScope.EMPTY;
        if (isEndTag && this.isEmptyScopeTag)
        {
          if (Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].UnmatchedSubstitute != Internal.Html.HtmlTagIndex._IMPLICIT_BEGIN)
          {
            this.output.Write("<!-- </");
            this.LineLength += 7;
            if (nameIndex > Internal.Html.HtmlNameIndex.Unknown)
            {
              this.output.Write(Internal.Html.HtmlNameData.Names[(int) nameIndex].Name);
              this.LineLength += Internal.Html.HtmlNameData.Names[(int) nameIndex].Name.Length;
            }
            else
            {
              this.output.Write(name != null ? name : "???");
              this.LineLength += name != null ? name.Length : 3;
            }
            this.output.Write("> ");
            this.LineLength += 2;
            this.tagNameIndex = Internal.Html.HtmlNameIndex._COMMENT;
            this.outputState = HtmlWriter.OutputState.WritingUnstructuredTagContent;
            return;
          }
          isEndTag = false;
        }
      }
      if (this.autoNewLines && this.LiteralWhitespaceNesting == 0)
      {
        bool flag = this.lastWhitespace;
        Internal.Html.HtmlDtd.TagFill tagFill = Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].Fill;
        if (this.LineLength != 0)
        {
          Internal.Html.HtmlDtd.TagFmt tagFmt = Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].Fmt;
          if (!isEndTag && tagFmt.LB == Internal.Html.HtmlDtd.FmtCode.BRK || isEndTag && tagFmt.LE == Internal.Html.HtmlDtd.FmtCode.BRK || this.LineLength > 80 && (this.lastWhitespace || this.allowWspBeforeFollowingTag || !isEndTag && tagFill.LB == Internal.Html.HtmlDtd.FillCode.EAT || isEndTag && tagFill.LE == Internal.Html.HtmlDtd.FillCode.EAT))
          {
            if (this.LineLength > this.longestLineLength)
              this.longestLineLength = this.LineLength;
            this.output.Write("\r\n");
            this.LineLength = 0;
            this.lastWhitespace = false;
          }
        }
        this.allowWspBeforeFollowingTag = (!isEndTag && tagFill.RB == Internal.Html.HtmlDtd.FillCode.EAT || isEndTag && tagFill.RE == Internal.Html.HtmlDtd.FillCode.EAT || flag && (!isEndTag && tagFill.RB == Internal.Html.HtmlDtd.FillCode.NUL || isEndTag && tagFill.RE == Internal.Html.HtmlDtd.FillCode.NUL)) && (nameIndex != Internal.Html.HtmlNameIndex.Body || !isEndTag);
      }
      if (this.lastWhitespace)
      {
        this.output.Write(' ');
        ++this.LineLength;
        this.lastWhitespace = false;
      }
      if (Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].BlockElement || htmlTagIndex == Internal.Html.HtmlTagIndex.BR)
        this.textLineLength = 0;
      this.output.Write('<');
      ++this.LineLength;
      if (nameIndex >= Internal.Html.HtmlNameIndex.Unknown)
      {
        if (isEndTag)
        {
          if ((Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].Literal & Internal.Html.HtmlDtd.Literal.Tags) != Internal.Html.HtmlDtd.Literal.None)
          {
            this.literalTags = false;
            this.literalEntities = false;
            this.cssEscaping = false;
          }
          if (Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].ContextTextType == Internal.Html.HtmlDtd.ContextTextType.Literal)
            --this.LiteralWhitespaceNesting;
          this.output.Write('/');
          ++this.LineLength;
        }
        if (nameIndex != Internal.Html.HtmlNameIndex.Unknown)
        {
          this.output.Write(Internal.Html.HtmlNameData.Names[(int) nameIndex].Name);
          this.LineLength += Internal.Html.HtmlNameData.Names[(int) nameIndex].Name.Length;
          this.outputState = HtmlWriter.OutputState.BeforeAttribute;
        }
        else
        {
          if (name != null)
          {
            this.output.Write(name);
            this.LineLength += name.Length;
            this.outputState = HtmlWriter.OutputState.BeforeAttribute;
          }
          else
            this.outputState = HtmlWriter.OutputState.TagStarted;
          this.isEmptyScopeTag = false;
        }
      }
      else
      {
        this.previousTagNameIndex = this.tagNameIndex;
        if (nameIndex == Internal.Html.HtmlNameIndex._COMMENT)
        {
          this.output.Write("!--");
          this.LineLength += 3;
        }
        else if (nameIndex == Internal.Html.HtmlNameIndex._ASP)
        {
          this.output.Write('%');
          ++this.LineLength;
        }
        else if (nameIndex == Internal.Html.HtmlNameIndex._CONDITIONAL)
        {
          this.output.Write("!--[");
          this.LineLength += 4;
        }
        else if (nameIndex == Internal.Html.HtmlNameIndex._DTD)
        {
          this.output.Write('?');
          ++this.LineLength;
        }
        else
        {
          this.output.Write('!');
          ++this.LineLength;
        }
        this.outputState = HtmlWriter.OutputState.WritingUnstructuredTagContent;
        this.isEmptyScopeTag = true;
      }
      this.tagNameIndex = nameIndex;
      this.isEndTag = isEndTag;
    }

    internal void WriteTagEnd()
    {
      this.WriteTagEnd(this.isEmptyScopeTag);
    }

    internal void WriteTagEnd(bool emptyScopeTag)
    {
      Internal.Html.HtmlTagIndex htmlTagIndex = Internal.Html.HtmlNameData.Names[(int) this.tagNameIndex].TagIndex;
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      if (this.tagNameIndex > Internal.Html.HtmlNameIndex.Unknown)
      {
        this.output.Write('>');
        ++this.LineLength;
      }
      else
      {
        if (this.tagNameIndex == Internal.Html.HtmlNameIndex._COMMENT)
        {
          this.output.Write("-->");
          this.LineLength += 3;
        }
        else if (this.tagNameIndex == Internal.Html.HtmlNameIndex._ASP)
        {
          this.output.Write("%>");
          this.LineLength += 2;
        }
        else if (this.tagNameIndex == Internal.Html.HtmlNameIndex._CONDITIONAL)
        {
          this.output.Write("]-->");
          this.LineLength += 4;
        }
        else if (this.tagNameIndex == Internal.Html.HtmlNameIndex.Unknown && emptyScopeTag)
        {
          this.output.Write(" />");
          this.LineLength += 3;
        }
        else
        {
          this.output.Write('>');
          ++this.LineLength;
        }
        this.tagNameIndex = this.previousTagNameIndex;
      }
      if (this.isEndTag && (htmlTagIndex == Internal.Html.HtmlTagIndex.LI || htmlTagIndex == Internal.Html.HtmlTagIndex.DD || htmlTagIndex == Internal.Html.HtmlTagIndex.DT))
        this.LineLength = 0;
      if (this.autoNewLines && this.LiteralWhitespaceNesting == 0)
      {
        Internal.Html.HtmlDtd.TagFmt tagFmt = Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].Fmt;
        Internal.Html.HtmlDtd.TagFill tagFill = Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].Fill;
        if (!this.isEndTag && tagFmt.RB == Internal.Html.HtmlDtd.FmtCode.BRK || this.isEndTag && tagFmt.RE == Internal.Html.HtmlDtd.FmtCode.BRK || this.LineLength > 80 && (this.allowWspBeforeFollowingTag || !this.isEndTag && tagFill.RB == Internal.Html.HtmlDtd.FillCode.EAT || this.isEndTag && tagFill.RE == Internal.Html.HtmlDtd.FillCode.EAT))
        {
          if (this.LineLength > this.longestLineLength)
            this.longestLineLength = this.LineLength;
          this.output.Write("\r\n");
          this.LineLength = 0;
        }
      }
      if (!this.isEndTag && !emptyScopeTag)
      {
        Internal.Html.HtmlDtd.Literal literal = Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].Literal;
        if ((literal & Internal.Html.HtmlDtd.Literal.Tags) != Internal.Html.HtmlDtd.Literal.None)
        {
          this.literalTags = true;
          this.literalEntities = Internal.Html.HtmlDtd.Literal.None != (literal & Internal.Html.HtmlDtd.Literal.Entities);
          this.cssEscaping = htmlTagIndex == Internal.Html.HtmlTagIndex.Style;
        }
        if (Internal.Html.HtmlDtd.tags[(int) htmlTagIndex].ContextTextType == Internal.Html.HtmlDtd.ContextTextType.Literal)
          ++this.LiteralWhitespaceNesting;
      }
      this.outputState = HtmlWriter.OutputState.OutsideTag;
    }

    internal void WriteAttribute(Internal.Html.HtmlNameIndex nameIndex, string value)
    {
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(Internal.Html.HtmlNameData.Names[(int) nameIndex].Name);
      if (value != null)
      {
        this.OutputAttributeValue(value);
        this.OutputAttributeEnd();
      }
      this.outputState = HtmlWriter.OutputState.BeforeAttribute;
    }

    internal void WriteAttribute(Internal.Html.HtmlNameIndex nameIndex, BufferString value)
    {
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(Internal.Html.HtmlNameData.Names[(int) nameIndex].Name);
      this.OutputAttributeValue(value.Buffer, value.Offset, value.Length);
      this.OutputAttributeEnd();
      this.outputState = HtmlWriter.OutputState.BeforeAttribute;
    }

    internal void WriteAttributeName(Internal.Html.HtmlNameIndex nameIndex)
    {
      if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
        this.OutputAttributeEnd();
      this.OutputAttributeName(Internal.Html.HtmlNameData.Names[(int) nameIndex].Name);
    }

    internal void WriteAttributeValue(BufferString value)
    {
      this.OutputAttributeValue(value.Buffer, value.Offset, value.Length);
    }

    internal void WriteAttributeValueInternal(string value)
    {
      this.OutputAttributeValue(value);
    }

    internal void WriteAttributeValueInternal(char[] buffer, int index, int count)
    {
      this.OutputAttributeValue(buffer, index, count);
    }

    internal void WriteText(char ch)
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (this.lastWhitespace)
        this.OutputLastWhitespace(ch);
      this.output.Write(ch, (IFallback) this);
      ++this.LineLength;
      ++this.textLineLength;
      this.allowWspBeforeFollowingTag = false;
    }

    internal void WriteMarkupText(char ch)
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (this.lastWhitespace)
        this.OutputLastWhitespace(ch);
      this.output.Write(ch, (IFallback) null);
      ++this.LineLength;
      this.allowWspBeforeFollowingTag = false;
    }

    internal ITextSinkEx WriteUnstructuredTagContent()
    {
      this.fallback = (IFallback) null;
      return (ITextSinkEx) this;
    }

    internal ITextSinkEx WriteTagName()
    {
      this.outputState = HtmlWriter.OutputState.WritingTagName;
      this.fallback = (IFallback) null;
      return (ITextSinkEx) this;
    }

    internal ITextSinkEx WriteAttributeName()
    {
      if (this.outputState != HtmlWriter.OutputState.WritingAttributeName)
      {
        if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
          this.OutputAttributeEnd();
        this.output.Write(' ');
        ++this.LineLength;
      }
      this.outputState = HtmlWriter.OutputState.WritingAttributeName;
      this.fallback = (IFallback) null;
      return (ITextSinkEx) this;
    }

    internal ITextSinkEx WriteAttributeValue()
    {
      if (this.outputState != HtmlWriter.OutputState.WritingAttributeValue)
      {
        this.output.Write("=\"");
        this.LineLength += 2;
      }
      this.outputState = HtmlWriter.OutputState.WritingAttributeValue;
      this.fallback = (IFallback) this;
      return (ITextSinkEx) this;
    }

    internal ITextSinkEx WriteText()
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      this.allowWspBeforeFollowingTag = false;
      if (this.lastWhitespace)
        this.OutputLastWhitespace('　');
      this.fallback = (IFallback) this;
      return (ITextSinkEx) this;
    }

    internal ITextSinkEx WriteMarkupText()
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (this.lastWhitespace)
      {
        this.output.Write(' ');
        ++this.LineLength;
        this.lastWhitespace = false;
      }
      this.fallback = (IFallback) null;
      return (ITextSinkEx) this;
    }

    internal void WriteNewLine()
    {
      this.WriteNewLine(false);
    }

    internal void WriteNewLine(bool optional)
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (optional && (this.LineLength == 0 || this.LiteralWhitespaceNesting != 0))
        return;
      if (this.LineLength > this.longestLineLength)
        this.longestLineLength = this.LineLength;
      this.output.Write("\r\n");
      this.LineLength = 0;
      this.lastWhitespace = false;
      this.allowWspBeforeFollowingTag = false;
    }

    internal void WriteAutoNewLine()
    {
      this.WriteNewLine(false);
    }

    internal void WriteAutoNewLine(bool optional)
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      if (!this.autoNewLines || optional && (this.LineLength == 0 || this.LiteralWhitespaceNesting != 0))
        return;
      if (this.LineLength > this.longestLineLength)
        this.longestLineLength = this.LineLength;
      this.output.Write("\r\n");
      this.LineLength = 0;
      this.lastWhitespace = false;
      this.allowWspBeforeFollowingTag = false;
    }

    internal void WriteTabulation(int count)
    {
      this.WriteSpace(this.textLineLength / 8 * 8 + 8 * count - this.textLineLength);
    }

    internal void WriteSpace(int count)
    {
      if (this.LiteralWhitespaceNesting == 0)
      {
        if (this.LineLength == 0 && count == 1)
        {
          this.output.Write(' ', (IFallback) this);
        }
        else
        {
          if (this.lastWhitespace)
          {
            ++this.LineLength;
            this.output.Write(' ', (IFallback) this);
          }
          this.LineLength += count - 1;
          this.textLineLength += count - 1;
          while (--count != 0)
            this.output.Write(' ', (IFallback) this);
          this.lastWhitespace = true;
          this.allowWspBeforeFollowingTag = false;
        }
      }
      else
      {
        while (count-- != 0)
          this.output.Write(' ');
        this.LineLength += count;
        this.textLineLength += count;
        this.lastWhitespace = false;
        this.allowWspBeforeFollowingTag = false;
      }
    }

    internal void WriteNbsp(int count)
    {
      if (this.lastWhitespace)
        this.OutputLastWhitespace(' ');
      this.LineLength += count;
      this.textLineLength += count;
      while (count-- != 0)
        this.output.Write(' ', (IFallback) this);
      this.allowWspBeforeFollowingTag = false;
    }

    internal void WriteTextInternal(char[] buffer, int index, int count)
    {
      if (count == 0)
        return;
      if (this.lastWhitespace)
        this.OutputLastWhitespace(buffer[index]);
      this.output.Write(buffer, index, count, (IFallback) this);
      this.LineLength += count;
      this.textLineLength += count;
      this.allowWspBeforeFollowingTag = false;
    }

    internal void StartTextChunk()
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      this.lastWhitespace = false;
    }

    internal void EndTextChunk()
    {
      if (!this.lastWhitespace)
        return;
      this.OutputLastWhitespace('\n');
    }

    internal void WriteCollapsedWhitespace()
    {
      if (this.outputState != HtmlWriter.OutputState.OutsideTag)
        this.WriteTagEnd();
      this.lastWhitespace = true;
      this.allowWspBeforeFollowingTag = false;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && this.output != null)
      {
        if (!this.IsCopyPending)
          this.Flush();
        if (this.output != null)
          this.output.Dispose();
      }
      this.output = (ConverterOutput) null;
    }

    private void WriteTag(HtmlTagId id, bool isEndTag)
    {
      if (id < HtmlTagId.Unknown || id >= (HtmlTagId) Internal.Html.HtmlNameData.TagIndex.Length)
        throw new ArgumentException(CtsResources.TextConvertersStrings.TagIdInvalid, "id");
      if (id == HtmlTagId.Unknown)
        throw new ArgumentException(CtsResources.TextConvertersStrings.TagIdIsUnknown, "id");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      this.WriteTagBegin(Internal.Html.HtmlNameData.TagIndex[(int) id], (string) null, isEndTag, false, false);
    }

    private void WriteTag(string name, bool isEndTag)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        throw new ArgumentException(CtsResources.TextConvertersStrings.TagNameIsEmpty, "name");
      if (this.IsCopyPending)
        throw new InvalidOperationException(CtsResources.TextConvertersStrings.CannotWriteWhileCopyPending);
      Internal.Html.HtmlNameIndex nameIndex = HtmlWriter.LookupName(name);
      if (nameIndex != Internal.Html.HtmlNameIndex.Unknown)
        name = (string) null;
      this.WriteTagBegin(nameIndex, name, isEndTag, false, false);
    }

    private void OutputLastWhitespace(char nextChar)
    {
      if (this.LineLength > (int) byte.MaxValue && this.autoNewLines)
      {
        if (this.LineLength > this.longestLineLength)
          this.longestLineLength = this.LineLength;
        this.output.Write("\r\n");
        this.LineLength = 0;
        if (ParseSupport.FarEastNonHanguelChar(nextChar))
        {
          this.output.Write(' ');
          ++this.LineLength;
        }
      }
      else
      {
        this.output.Write(' ');
        ++this.LineLength;
      }
      ++this.textLineLength;
      this.lastWhitespace = false;
    }

    private void OutputAttributeName(string name)
    {
      this.output.Write(' ');
      this.output.Write(name);
      this.LineLength += name.Length + 1;
      this.outputState = HtmlWriter.OutputState.AfterAttributeName;
    }

    private void OutputAttributeValue(string value)
    {
      if (this.outputState < HtmlWriter.OutputState.WritingAttributeValue)
      {
        this.output.Write("=\"");
        this.LineLength += 2;
      }
      this.output.Write(value, (IFallback) this);
      this.LineLength += value.Length;
      this.outputState = HtmlWriter.OutputState.WritingAttributeValue;
    }

    private void OutputAttributeValue(char[] value, int index, int count)
    {
      if (this.outputState < HtmlWriter.OutputState.WritingAttributeValue)
      {
        this.output.Write("=\"");
        this.LineLength += 2;
      }
      this.output.Write(value, index, count, (IFallback) this);
      this.LineLength += count;
      this.outputState = HtmlWriter.OutputState.WritingAttributeValue;
    }

    private void OutputAttributeEnd()
    {
      if (this.outputState < HtmlWriter.OutputState.WritingAttributeValue)
      {
        this.output.Write("=\"");
        this.LineLength += 2;
      }
      this.output.Write('"');
      ++this.LineLength;
    }

    internal enum OutputState
    {
      OutsideTag,
      TagStarted,
      WritingUnstructuredTagContent,
      WritingTagName,
      BeforeAttribute,
      WritingAttributeName,
      AfterAttributeName,
      WritingAttributeValue,
    }
  }
}
