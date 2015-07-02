// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlToHtmlConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlToHtmlConverter : IProducerConsumer, IRestartable, IDisposable
  {
    private static readonly string NamePrefix = "x_";
    private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteNameWithCompleteValue);
    private static object lockObject = new object();
    private int dropLevel = int.MaxValue;
    private static bool textConvertersConfigured;
    private static Dictionary<string, string> safeUrlDictionary;
    private HtmlToken token;
    private HtmlWriter writer;
    private bool convertFragment;
    private bool outputFragment;
    private bool filterForFragment;
    private bool filterHtml;
    private bool truncateForCallback;
    private int smallCssBlockThreshold;
    private bool preserveDisplayNoneStyle;
    private bool hasTailInjection;
    private IHtmlParser parser;
    private bool endOfFile;
    private bool normalizedInput;
    private HtmlTagCallback callback;
    private HtmlToHtmlTagContext callbackContext;
    private bool headDivUnterminated;
    private int currentLevel;
    private int currentLevelDelta;
    private bool insideCSS;
    private HtmlToHtmlConverter.EndTagActionEntry[] endTagActionStack;
    private int endTagActionStackTop;
    private bool tagDropped;
    private bool justTruncated;
    private bool tagCallbackRequested;
    private bool attributeTriggeredCallback;
    private bool endTagCallbackRequested;
    private bool ignoreAttrCallback;
    private bool styleIsCSS;
    private HtmlFilterData.FilterAction attrContinuationAction;
    private HtmlToHtmlConverter.CopyPendingState copyPendingState;
    private HtmlTagIndex tagIndex;
    private int attributeCount;
    private int attributeSkipCount;
    private bool attributeIndirect;
    private HtmlToHtmlConverter.AttributeIndirectEntry[] attributeIndirectIndex;
    private HtmlToHtmlConverter.AttributeVirtualEntry[] attributeVirtualList;
    private int attributeVirtualCount;
    private ScratchBuffer attributeVirtualScratch;
    private ScratchBuffer attributeActionScratch;
    private bool attributeLeadingSpaces;
    private bool metaInjected;
    private bool insideHtml;
    private bool insideHead;
    private bool insideBody;
    private bool tagHasFilteredStyleAttribute;
    private Css.CssParser cssParser;
    private ConverterBufferInput cssParserInput;
    private HtmlToHtmlConverter.VirtualScratchSink virtualScratchSink;
    private IProgressMonitor progressMonitor;

    internal HtmlToken InternalToken
    {
      get
      {
        return this.token;
      }
    }

    private HtmlToHtmlConverter.CopyPendingState CopyPendingStateFlag
    {
      get
      {
        return this.copyPendingState;
      }
      set
      {
        this.writer.SetCopyPending(value != HtmlToHtmlConverter.CopyPendingState.NotPending);
        this.copyPendingState = value;
      }
    }

    public HtmlToHtmlConverter(IHtmlParser parser, HtmlWriter writer, bool convertFragment, bool outputFragment, bool filterHtml, HtmlTagCallback callback, bool truncateForCallback, bool hasTailInjection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, int smallCssBlockThreshold, bool preserveDisplayNoneStyle, IProgressMonitor progressMonitor)
    {
      this.writer = writer;
      this.normalizedInput = parser is HtmlNormalizingParser;
      this.progressMonitor = progressMonitor;
      this.convertFragment = convertFragment;
      this.outputFragment = outputFragment;
      this.filterForFragment = outputFragment || convertFragment;
      this.filterHtml = filterHtml || this.filterForFragment;
      this.callback = callback;
      this.parser = parser;
      if (!convertFragment)
        this.parser.SetRestartConsumer((IRestartable) this);
      this.truncateForCallback = truncateForCallback;
      this.hasTailInjection = hasTailInjection;
      this.smallCssBlockThreshold = smallCssBlockThreshold;
      this.preserveDisplayNoneStyle = preserveDisplayNoneStyle;
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null && this.parser is IDisposable)
        ((IDisposable) this.parser).Dispose();
      if (!this.convertFragment && this.writer != null && this.writer != null)
        this.writer.Dispose();
      if (this.token != null && this.token is IDisposable)
        ((IDisposable) this.token).Dispose();
      this.parser = (IHtmlParser) null;
      this.writer = (HtmlWriter) null;
      this.token = (HtmlToken) null;
      GC.SuppressFinalize((object) this);
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      HtmlTokenId tokenId = this.parser.Parse();
      if (tokenId == HtmlTokenId.None)
        return;
      this.Process(tokenId);
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    public void Initialize(string fragment, bool preformatedText)
    {
      if (this.parser is HtmlNormalizingParser)
        ((HtmlNormalizingParser) this.parser).Initialize(fragment, preformatedText);
      else
        ((HtmlParser) this.parser).Initialize(fragment, preformatedText);
      ((IRestartable) this).Restart();
    }

    bool IRestartable.CanRestart()
    {
      if (this.writer != null)
        return ((IRestartable) this.writer).CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      if (this.writer != null && !this.convertFragment)
        ((IRestartable) this.writer).Restart();
      this.endOfFile = false;
      this.token = (HtmlToken) null;
      this.styleIsCSS = true;
      this.insideCSS = false;
      this.headDivUnterminated = false;
      this.tagDropped = false;
      this.justTruncated = false;
      this.tagCallbackRequested = false;
      this.endTagCallbackRequested = false;
      this.ignoreAttrCallback = false;
      this.attrContinuationAction = HtmlFilterData.FilterAction.Unknown;
      this.currentLevel = 0;
      this.currentLevelDelta = 0;
      this.dropLevel = int.MaxValue;
      this.endTagActionStackTop = 0;
      this.copyPendingState = HtmlToHtmlConverter.CopyPendingState.NotPending;
      this.metaInjected = false;
      this.insideHtml = false;
      this.insideHead = false;
      this.insideBody = false;
    }

    void IRestartable.DisableRestart()
    {
      if (this.writer == null)
        return;
      ((IRestartable) this.writer).DisableRestart();
    }

    internal static void RefreshConfiguration()
    {
      HtmlToHtmlConverter.textConvertersConfigured = false;
    }

    internal static bool TestSafeUrlSchema(string schema)
    {
      if (schema.Length < 2 || schema.Length > 20)
        return false;
      if (!HtmlToHtmlConverter.textConvertersConfigured)
        HtmlToHtmlConverter.ConfigureTextConverters();
      return HtmlToHtmlConverter.safeUrlDictionary.ContainsKey(schema);
    }

    internal static bool IsUrlSafe(string url, bool callbackRequested)
    {
      char[] urlBuffer = url.ToCharArray();
      switch (HtmlToHtmlConverter.CheckUrl(urlBuffer, urlBuffer.Length, callbackRequested))
      {
        case HtmlToHtmlConverter.CheckUrlResult.Inconclusive:
        case HtmlToHtmlConverter.CheckUrlResult.Safe:
        case HtmlToHtmlConverter.CheckUrlResult.LocalHyperlink:
          return true;
        default:
          return false;
      }
    }

    internal void CopyInputTag(bool copyTagAttributes)
    {
      if (this.token.IsTagBegin)
        this.writer.WriteTagBegin(HtmlDtd.tags[(int) this.tagIndex].NameIndex, (string) null, this.token.IsEndTag, this.token.IsAllowWspLeft, this.token.IsAllowWspRight);
      if (this.tagIndex <= HtmlTagIndex.Unknown)
      {
        if (this.tagIndex < HtmlTagIndex.Unknown)
        {
          this.token.UnstructuredContent.WriteTo((ITextSink) this.writer.WriteUnstructuredTagContent());
          if (this.token.IsTagEnd)
          {
            this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
            return;
          }
          this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.TagCopyPending;
          return;
        }
        if (this.token.HasNameFragment)
        {
          this.token.Name.WriteTo((ITextSink) this.writer.WriteTagName());
          if (!this.token.IsTagNameEnd && !copyTagAttributes)
          {
            this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.TagNameCopyPending;
            return;
          }
        }
      }
      if (!copyTagAttributes)
      {
        this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
      }
      else
      {
        if (this.attributeCount != 0)
          this.CopyInputTagAttributes();
        if (this.token.IsTagEnd)
          this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
        else
          this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.TagCopyPending;
      }
    }

    internal void CopyInputAttribute(int index)
    {
      HtmlToHtmlConverter.AttributeIndirectKind attributeIndirectKind = this.GetAttributeIndirectKind(index);
      if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle)
      {
        if (!this.tagCallbackRequested)
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Style);
          if (!this.cssParserInput.IsEmpty)
          {
            this.FlushCssInStyleAttribute(this.writer);
            return;
          }
          this.writer.WriteAttributeValueInternal(string.Empty);
          return;
        }
        this.VirtualizeFilteredStyle(index);
        attributeIndirectKind = HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle;
      }
      bool flag = true;
      if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle)
      {
        this.writer.WriteAttributeName(HtmlNameIndex.Style);
        int virtualEntryIndex = this.GetAttributeVirtualEntryIndex(index);
        if (this.attributeVirtualList[virtualEntryIndex].Length != 0)
          this.writer.WriteAttributeValueInternal(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[virtualEntryIndex].Offset, this.attributeVirtualList[virtualEntryIndex].Length);
        else
          this.writer.WriteAttributeValueInternal(string.Empty);
      }
      else
      {
        HtmlAttribute attribute = this.GetAttribute(index);
        if (attribute.IsAttrBegin && attribute.NameIndex != HtmlNameIndex.Unknown)
          this.writer.WriteAttributeName(attribute.NameIndex);
        if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
          attribute.Name.WriteTo((ITextSink) this.writer.WriteAttributeName());
        if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.NameOnlyFragment)
          flag = false;
        else if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.EmptyValue)
          this.writer.WriteAttributeValueInternal(string.Empty);
        else if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.Virtual)
        {
          int virtualEntryIndex = this.GetAttributeVirtualEntryIndex(index);
          if (this.attributeVirtualList[virtualEntryIndex].Length != 0)
            this.writer.WriteAttributeValueInternal(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[virtualEntryIndex].Offset, this.attributeVirtualList[virtualEntryIndex].Length);
          else
            this.writer.WriteAttributeValueInternal(string.Empty);
        }
        else
        {
          if (attribute.HasValueFragment)
            attribute.Value.WriteTo((ITextSink) this.writer.WriteAttributeValue());
          flag = attribute.IsAttrEnd;
        }
      }
      if (flag)
        this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
      else
        this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.AttributeCopyPending;
    }

    internal void CopyInputAttributeName(int index)
    {
      switch (this.GetAttributeIndirectKind(index))
      {
        case HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle:
        case HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle:
          this.writer.WriteAttributeName(HtmlNameIndex.Style);
          break;
        default:
          HtmlAttribute attribute = this.GetAttribute(index);
          if (attribute.IsAttrBegin && attribute.NameIndex != HtmlNameIndex.Unknown)
            this.writer.WriteAttributeName(attribute.NameIndex);
          if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
            attribute.Name.WriteTo((ITextSink) this.writer.WriteAttributeName());
          if (attribute.IsAttrNameEnd)
          {
            this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
            break;
          }
          this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.AttributeNameCopyPending;
          break;
      }
    }

    internal void CopyInputAttributeValue(int index)
    {
      HtmlToHtmlConverter.AttributeIndirectKind attributeIndirectKind = this.GetAttributeIndirectKind(index);
      bool flag = true;
      if (attributeIndirectKind != HtmlToHtmlConverter.AttributeIndirectKind.PassThrough)
      {
        if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle)
        {
          if (!this.tagCallbackRequested)
          {
            if (!this.cssParserInput.IsEmpty)
            {
              this.FlushCssInStyleAttribute(this.writer);
              return;
            }
            this.writer.WriteAttributeValueInternal(string.Empty);
            return;
          }
          this.VirtualizeFilteredStyle(index);
          attributeIndirectKind = HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle;
        }
        if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.Virtual || attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle)
        {
          int virtualEntryIndex = this.GetAttributeVirtualEntryIndex(index);
          if (this.attributeVirtualList[virtualEntryIndex].Length != 0)
            this.writer.WriteAttributeValueInternal(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[virtualEntryIndex].Offset, this.attributeVirtualList[virtualEntryIndex].Length);
          else
            this.writer.WriteAttributeValueInternal(string.Empty);
        }
        else if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.NameOnlyFragment)
          flag = false;
        else if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.EmptyValue)
          this.writer.WriteAttributeValueInternal(string.Empty);
      }
      else
      {
        HtmlAttribute attribute = this.GetAttribute(index);
        if (attribute.HasValueFragment)
          attribute.Value.WriteTo((ITextSink) this.writer.WriteAttributeValue());
        flag = attribute.IsAttrEnd;
      }
      if (flag)
        this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
      else
        this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.AttributeValueCopyPending;
    }

    internal HtmlAttributeId GetAttributeNameId(int index)
    {
      switch (this.GetAttributeIndirectKind(index))
      {
        case HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle:
        case HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle:
          return HtmlAttributeId.Style;
        default:
          HtmlAttribute attribute = this.GetAttribute(index);
          return HtmlNameData.Names[(int) attribute.NameIndex].PublicAttributeId;
      }
    }

    internal HtmlAttributeParts GetAttributeParts(int index)
    {
      HtmlToHtmlConverter.AttributeIndirectKind attributeIndirectKind = this.GetAttributeIndirectKind(index);
      switch (attributeIndirectKind)
      {
        case HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle:
        case HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle:
          return HtmlToHtmlConverter.CompleteAttributeParts;
        default:
          HtmlAttribute attribute = this.GetAttribute(index);
          if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.NameOnlyFragment)
            return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart & (HtmlToken.AttrPartMinor) 199);
          if (attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.EmptyValue || attributeIndirectKind == HtmlToHtmlConverter.AttributeIndirectKind.Virtual)
            return new HtmlAttributeParts(attribute.MajorPart | HtmlToken.AttrPartMajor.End, attribute.MinorPart | HtmlToken.AttrPartMinor.CompleteValue);
          return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart);
      }
    }

    internal string GetAttributeName(int index)
    {
      switch (this.GetAttributeIndirectKind(index))
      {
        case HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle:
        case HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle:
          return HtmlNameData.Names[40].Name;
        default:
          HtmlAttribute attribute = this.GetAttribute(index);
          if (attribute.NameIndex > HtmlNameIndex.Unknown)
          {
            if (!attribute.IsAttrBegin)
              return string.Empty;
            return HtmlNameData.Names[(int) attribute.NameIndex].Name;
          }
          if (attribute.HasNameFragment)
            return attribute.Name.GetString(int.MaxValue);
          if (!attribute.IsAttrBegin)
            return string.Empty;
          return "?";
      }
    }

    internal string GetAttributeValue(int index)
    {
      HtmlToHtmlConverter.AttributeIndirectKind attributeIndirectKind = this.GetAttributeIndirectKind(index);
      switch (attributeIndirectKind)
      {
        case HtmlToHtmlConverter.AttributeIndirectKind.PassThrough:
          HtmlAttribute attribute = this.GetAttribute(index);
          if (!attribute.HasValueFragment)
            return string.Empty;
          return attribute.Value.GetString(int.MaxValue);
        case HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle:
          this.VirtualizeFilteredStyle(index);
          attributeIndirectKind = HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle;
          break;
      }
      if (attributeIndirectKind != HtmlToHtmlConverter.AttributeIndirectKind.Virtual && attributeIndirectKind != HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle)
        return string.Empty;
      int virtualEntryIndex = this.GetAttributeVirtualEntryIndex(index);
      if (this.attributeVirtualList[virtualEntryIndex].Length != 0)
        return new string(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[virtualEntryIndex].Offset, this.attributeVirtualList[virtualEntryIndex].Length);
      return string.Empty;
    }

    internal int ReadAttributeValue(int index, char[] buffer, int offset, int count)
    {
      HtmlToHtmlConverter.AttributeIndirectKind attributeIndirectKind = this.GetAttributeIndirectKind(index);
      switch (attributeIndirectKind)
      {
        case HtmlToHtmlConverter.AttributeIndirectKind.PassThrough:
          HtmlAttribute attribute = this.GetAttribute(index);
          if (!attribute.HasValueFragment)
            return 0;
          return attribute.Value.Read(buffer, offset, count);
        case HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle:
          this.VirtualizeFilteredStyle(index);
          attributeIndirectKind = HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle;
          break;
      }
      if (attributeIndirectKind != HtmlToHtmlConverter.AttributeIndirectKind.Virtual && attributeIndirectKind != HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle)
        return 0;
      int virtualEntryIndex = this.GetAttributeVirtualEntryIndex(index);
      int num = Math.Min(this.attributeVirtualList[virtualEntryIndex].Length - this.attributeVirtualList[virtualEntryIndex].Position, count);
      if (num != 0)
      {
        Buffer.BlockCopy((Array) this.attributeVirtualScratch.Buffer, 2 * (this.attributeVirtualList[virtualEntryIndex].Offset + this.attributeVirtualList[virtualEntryIndex].Position), (Array) buffer, offset, 2 * num);
        this.attributeVirtualList[virtualEntryIndex].Position += num;
      }
      return num;
    }

    internal void WriteTag(bool copyTagAttributes)
    {
      this.CopyInputTag(copyTagAttributes);
    }

    internal void WriteAttribute(int index, bool writeName, bool writeValue)
    {
      if (writeName)
      {
        if (writeValue)
          this.CopyInputAttribute(index);
        else
          this.CopyInputAttributeName(index);
      }
      else
      {
        if (!writeValue)
          return;
        this.CopyInputAttributeValue(index);
      }
    }

    private static int WhitespaceLength(char[] buffer, int offset, int remainingLength)
    {
      int num = 0;
      for (; remainingLength != 0 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])); --remainingLength)
        ++num;
      return num;
    }

    private static int NonWhitespaceLength(char[] buffer, int offset, int remainingLength)
    {
      int num = 0;
      for (; remainingLength != 0 && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])); --remainingLength)
        ++num;
      return num;
    }

    private static void ConfigureTextConverters()
    {
      lock (HtmlToHtmlConverter.lockObject)
      {
        if (HtmlToHtmlConverter.textConvertersConfigured)
          return;
        HtmlToHtmlConverter.safeUrlDictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        bool local_0 = false;
        bool local_1 = true;
        Data.Internal.CtsConfigurationSetting local_2 = Data.Internal.ApplicationServices.GetSimpleConfigurationSetting("TextConverters", "SafeUrlScheme");
        if (local_2 != null)
        {
          if (local_2.Arguments.Count != 1 || !local_2.Arguments[0].Name.Equals("Add", StringComparison.OrdinalIgnoreCase) && !local_2.Arguments[0].Name.Equals("Override", StringComparison.OrdinalIgnoreCase))
          {
            Data.Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
          }
          else
          {
            local_1 = local_2.Arguments[0].Name.Equals("Add", StringComparison.OrdinalIgnoreCase);
            string[] local_3 = local_2.Arguments[0].Value.Split(new char[4]
            {
              ',',
              ' ',
              ';',
              ':'
            }, StringSplitOptions.RemoveEmptyEntries);
            bool local_4 = false;
            foreach (string item_1 in local_3)
            {
              string local_6 = item_1.Trim().ToLower();
              bool local_7 = false;
              foreach (char item_0 in local_6)
              {
                if ((int) item_0 > (int) sbyte.MaxValue || !char.IsLetterOrDigit(item_0) && (int) item_0 != 95 && ((int) item_0 != 45 && (int) item_0 != 43))
                {
                  local_4 = true;
                  local_7 = true;
                  break;
                }
              }
              if (!local_7 && !HtmlToHtmlConverter.safeUrlDictionary.ContainsKey(local_6))
                HtmlToHtmlConverter.safeUrlDictionary.Add(local_6, (string) null);
            }
            if (local_4)
              Data.Internal.ApplicationServices.Provider.LogConfigurationErrorEvent();
            local_0 = true;
          }
        }
        if (!local_0 || local_1)
        {
          HtmlToHtmlConverter.safeUrlDictionary["http"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["https"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["ftp"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["file"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["mailto"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["news"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["gopher"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["about"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["wais"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["cid"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["mhtml"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["ipp"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["msdaipp"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["meet"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["tel"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["sip"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["conf"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["im"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["callto"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["notes"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["onenote"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["groove"] = (string) null;
          HtmlToHtmlConverter.safeUrlDictionary["mms"] = (string) null;
        }
        HtmlToHtmlConverter.textConvertersConfigured = true;
      }
    }

    private static bool SafeUrlSchema(char[] urlBuffer, int schemaLength)
    {
      if (schemaLength < 2 || schemaLength > 20)
        return false;
      if (!HtmlToHtmlConverter.textConvertersConfigured)
        HtmlToHtmlConverter.ConfigureTextConverters();
      return HtmlToHtmlConverter.safeUrlDictionary.ContainsKey(new string(urlBuffer, 0, schemaLength));
    }

    private static HtmlToHtmlConverter.CheckUrlResult CheckUrl(char[] urlBuffer, int urlLength, bool callbackRequested)
    {
      if (urlLength > 0 && (int) urlBuffer[0] == 35)
        return HtmlToHtmlConverter.CheckUrlResult.LocalHyperlink;
      if (urlLength > 10 && new BufferString(urlBuffer, 0, 10).EqualsToLowerCaseStringIgnoreCase("data:image"))
        return !callbackRequested ? HtmlToHtmlConverter.CheckUrlResult.Unsafe : HtmlToHtmlConverter.CheckUrlResult.Safe;
      for (int schemaLength = 0; schemaLength < urlLength; ++schemaLength)
      {
        if ((int) urlBuffer[schemaLength] == 47 || (int) urlBuffer[schemaLength] == 92)
          return schemaLength == 0 && urlLength > 1 && ((int) urlBuffer[1] == 47 || (int) urlBuffer[1] == 92) && !callbackRequested ? HtmlToHtmlConverter.CheckUrlResult.Unsafe : HtmlToHtmlConverter.CheckUrlResult.Safe;
        if ((int) urlBuffer[schemaLength] == 63 || (int) urlBuffer[schemaLength] == 35 || (int) urlBuffer[schemaLength] == 59)
          return HtmlToHtmlConverter.CheckUrlResult.Safe;
        if ((int) urlBuffer[schemaLength] == 58)
        {
          if (HtmlToHtmlConverter.SafeUrlSchema(urlBuffer, schemaLength))
            return HtmlToHtmlConverter.CheckUrlResult.Safe;
          if (callbackRequested)
          {
            if (schemaLength == 1 && urlLength > 2 && ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(urlBuffer[0])) && ((int) urlBuffer[2] == 47 || (int) urlBuffer[2] == 92))
              return HtmlToHtmlConverter.CheckUrlResult.Safe;
            BufferString bufferString = new BufferString(urlBuffer, 0, urlLength);
            if (bufferString.EqualsToLowerCaseStringIgnoreCase("objattph://") || bufferString.EqualsToLowerCaseStringIgnoreCase("rtfimage://"))
              return HtmlToHtmlConverter.CheckUrlResult.Safe;
          }
          return HtmlToHtmlConverter.CheckUrlResult.Unsafe;
        }
      }
      return HtmlToHtmlConverter.CheckUrlResult.Inconclusive;
    }

    private static void CopyInputCssProperty(Css.CssProperty property, ITextSinkEx sink)
    {
      if (property.IsPropertyBegin && property.NameId != Css.CssNameIndex.Unknown)
        sink.Write(Css.CssData.names[(int) property.NameId].Name);
      if (property.NameId == Css.CssNameIndex.Unknown && property.HasNameFragment)
        property.Name.WriteOriginalTo((ITextSink) sink);
      if (property.IsPropertyNameEnd)
        sink.Write(":");
      if (!property.HasValueFragment)
        return;
      property.Value.WriteEscapedOriginalTo((ITextSink) sink);
    }

    private void Process(HtmlTokenId tokenId)
    {
      this.token = this.parser.Token;
      if (!this.metaInjected && !this.InjectMetaTagIfNecessary())
        return;
      switch (tokenId)
      {
        case HtmlTokenId.EndOfFile:
          this.ProcessEof();
          break;
        case HtmlTokenId.Text:
          this.ProcessText();
          break;
        case HtmlTokenId.EncodingChange:
          if (!this.writer.HasEncoding || !this.writer.CodePageSameAsInput)
            break;
          this.writer.Encoding = this.token.TokenEncoding;
          break;
        case HtmlTokenId.Tag:
          if (!this.token.IsEndTag)
          {
            this.ProcessStartTag();
            break;
          }
          this.ProcessEndTag();
          break;
        case HtmlTokenId.OverlappedClose:
          this.ProcessOverlappedClose();
          break;
        case HtmlTokenId.OverlappedReopen:
          this.ProcessOverlappedReopen();
          break;
        case HtmlTokenId.InjectionBegin:
          this.ProcessInjectionBegin();
          break;
        case HtmlTokenId.InjectionEnd:
          this.ProcessInjectionEnd();
          break;
      }
    }

    private void ProcessStartTag()
    {
      HtmlToHtmlConverter.AvailableTagParts availableTagParts = HtmlToHtmlConverter.AvailableTagParts.None;
      if (this.insideCSS && this.token.TagIndex == HtmlTagIndex._COMMENT && this.filterHtml)
      {
        this.AppendCssFromTokenText();
      }
      else
      {
        if (this.token.IsTagBegin)
        {
          ++this.currentLevel;
          this.tagIndex = this.token.TagIndex;
          this.tagDropped = false;
          this.justTruncated = false;
          this.endTagCallbackRequested = false;
          this.PreProcessStartTag();
          if (this.currentLevel >= this.dropLevel)
            this.tagDropped = true;
          else if (!this.tagDropped)
          {
            this.tagCallbackRequested = false;
            this.ignoreAttrCallback = false;
            if (this.filterHtml || this.callback != null)
            {
              HtmlFilterData.FilterAction filterAction = this.filterForFragment ? HtmlFilterData.filterInstructions[(int) this.token.NameIndex].tagFragmentAction : HtmlFilterData.filterInstructions[(int) this.token.NameIndex].tagAction;
              if (this.callback != null && (filterAction & HtmlFilterData.FilterAction.Callback) != HtmlFilterData.FilterAction.Unknown)
                this.tagCallbackRequested = true;
              else if (this.filterHtml)
              {
                this.ignoreAttrCallback = HtmlFilterData.FilterAction.Unknown != (filterAction & HtmlFilterData.FilterAction.IgnoreAttrCallbacks);
                switch (filterAction & HtmlFilterData.FilterAction.ActionMask)
                {
                  case HtmlFilterData.FilterAction.Drop:
                    this.tagDropped = true;
                    this.dropLevel = this.currentLevel;
                    break;
                  case HtmlFilterData.FilterAction.DropKeepContent:
                    this.tagDropped = true;
                    break;
                  case HtmlFilterData.FilterAction.KeepDropContent:
                    this.dropLevel = this.currentLevel + 1;
                    break;
                }
              }
            }
            if (!this.tagDropped)
            {
              this.attributeTriggeredCallback = false;
              this.tagHasFilteredStyleAttribute = false;
              availableTagParts = HtmlToHtmlConverter.AvailableTagParts.TagBegin;
            }
          }
        }
        if (!this.tagDropped)
        {
          HtmlToken.TagPartMinor minorPart = this.token.MinorPart;
          if (this.token.IsTagEnd)
            availableTagParts |= HtmlToHtmlConverter.AvailableTagParts.TagEnd;
          if (this.tagIndex < HtmlTagIndex.Unknown)
          {
            availableTagParts |= HtmlToHtmlConverter.AvailableTagParts.UnstructuredContent;
            this.attributeCount = 0;
          }
          else
          {
            if (this.token.HasNameFragment || this.token.IsTagNameEnd)
              availableTagParts |= HtmlToHtmlConverter.AvailableTagParts.TagName;
            this.ProcessTagAttributes();
            if (this.attributeCount != 0)
              availableTagParts |= HtmlToHtmlConverter.AvailableTagParts.Attributes;
          }
          if (availableTagParts != HtmlToHtmlConverter.AvailableTagParts.None)
          {
            HtmlToken.TagPartMinor tagPartMinor;
            if (this.CopyPendingStateFlag != HtmlToHtmlConverter.CopyPendingState.NotPending)
            {
              switch (this.CopyPendingStateFlag)
              {
                case HtmlToHtmlConverter.CopyPendingState.TagCopyPending:
                  this.CopyInputTag(true);
                  if (this.tagCallbackRequested && (availableTagParts & HtmlToHtmlConverter.AvailableTagParts.TagEnd) != HtmlToHtmlConverter.AvailableTagParts.None)
                  {
                    this.attributeCount = 0;
                    this.token.Name.MakeEmpty();
                    availableTagParts &= ~HtmlToHtmlConverter.AvailableTagParts.TagEnd;
                    tagPartMinor = HtmlToken.TagPartMinor.Empty;
                    break;
                  }
                  availableTagParts = HtmlToHtmlConverter.AvailableTagParts.None;
                  break;
                case HtmlToHtmlConverter.CopyPendingState.TagNameCopyPending:
                  this.token.Name.WriteTo((ITextSink) this.writer.WriteTagName());
                  if (this.token.IsTagNameEnd)
                    this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
                  this.token.Name.MakeEmpty();
                  availableTagParts &= ~HtmlToHtmlConverter.AvailableTagParts.TagName;
                  tagPartMinor = minorPart & (HtmlToken.TagPartMinor) 248;
                  break;
                case HtmlToHtmlConverter.CopyPendingState.AttributeCopyPending:
                  this.CopyInputAttribute(0);
                  this.attributeSkipCount = 1;
                  --this.attributeCount;
                  if (this.attributeCount == 0)
                    availableTagParts &= ~HtmlToHtmlConverter.AvailableTagParts.Attributes;
                  tagPartMinor = minorPart & (HtmlToken.TagPartMinor) 199;
                  break;
                case HtmlToHtmlConverter.CopyPendingState.AttributeNameCopyPending:
                  this.CopyInputAttributeName(0);
                  if (1 == this.attributeCount && (this.token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueValue) == HtmlToken.AttrPartMinor.Empty)
                  {
                    this.attributeSkipCount = 1;
                    --this.attributeCount;
                    availableTagParts &= ~HtmlToHtmlConverter.AvailableTagParts.Attributes;
                    tagPartMinor = minorPart & (HtmlToken.TagPartMinor) 199;
                    break;
                  }
                  this.token.Attributes[0].Name.MakeEmpty();
                  this.token.Attributes[0].SetMinorPart(this.token.Attributes[0].MinorPart & (HtmlToken.AttrPartMinor) 248);
                  break;
                case HtmlToHtmlConverter.CopyPendingState.AttributeValueCopyPending:
                  this.CopyInputAttributeValue(0);
                  this.attributeSkipCount = 1;
                  --this.attributeCount;
                  if (this.attributeCount == 0)
                    availableTagParts &= ~HtmlToHtmlConverter.AvailableTagParts.Attributes;
                  tagPartMinor = minorPart & (HtmlToken.TagPartMinor) 199;
                  break;
              }
            }
            if (availableTagParts != HtmlToHtmlConverter.AvailableTagParts.None)
            {
              if (this.tagCallbackRequested)
              {
                if (this.callbackContext == null)
                  this.callbackContext = new HtmlToHtmlTagContext(this);
                if (this.token.IsTagBegin || this.attributeTriggeredCallback)
                {
                  this.callbackContext.InitializeTag(false, HtmlDtd.tags[(int) this.tagIndex].NameIndex, false);
                  this.attributeTriggeredCallback = false;
                }
                this.callbackContext.InitializeFragment(this.token.IsEmptyScope, this.attributeCount, new HtmlTagParts(this.token.MajorPart, this.token.MinorPart));
                this.callback((HtmlTagContext) this.callbackContext, this.writer);
                this.callbackContext.UninitializeFragment();
                if (this.token.IsTagEnd || this.truncateForCallback)
                {
                  if (this.callbackContext.IsInvokeCallbackForEndTag)
                    this.endTagCallbackRequested = true;
                  if (this.callbackContext.IsDeleteInnerContent)
                    this.dropLevel = this.currentLevel + 1;
                  if (this.token.IsTagBegin && this.callbackContext.IsDeleteEndTag)
                    this.tagDropped = true;
                  if (!this.tagDropped && !this.token.IsTagEnd)
                  {
                    this.tagDropped = true;
                    this.justTruncated = true;
                    this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
                  }
                }
              }
              else
              {
                if (this.token.IsTagBegin)
                  this.CopyInputTag(false);
                if (this.attributeCount != 0)
                  this.CopyInputTagAttributes();
                if (this.token.IsTagEnd && this.tagIndex == HtmlTagIndex.Unknown)
                  this.writer.WriteTagEnd(this.token.IsEmptyScope);
              }
            }
          }
        }
        if (!this.token.IsTagEnd)
          return;
        if (this.writer.IsTagOpen)
          this.writer.WriteTagEnd();
        if (!this.token.IsEmptyScope && this.tagIndex > HtmlTagIndex.Unknown)
        {
          if (this.normalizedInput && this.currentLevel < this.dropLevel && (this.tagDropped && !this.justTruncated || this.endTagCallbackRequested))
          {
            if (this.endTagActionStack == null)
              this.endTagActionStack = new HtmlToHtmlConverter.EndTagActionEntry[4];
            else if (this.endTagActionStack.Length == this.endTagActionStackTop)
            {
              HtmlToHtmlConverter.EndTagActionEntry[] endTagActionEntryArray = new HtmlToHtmlConverter.EndTagActionEntry[this.endTagActionStack.Length * 2];
              Array.Copy((Array) this.endTagActionStack, 0, (Array) endTagActionEntryArray, 0, this.endTagActionStackTop);
              this.endTagActionStack = endTagActionEntryArray;
            }
            this.endTagActionStack[this.endTagActionStackTop].TagLevel = this.currentLevel;
            this.endTagActionStack[this.endTagActionStackTop].Drop = this.tagDropped && !this.justTruncated;
            this.endTagActionStack[this.endTagActionStackTop].Callback = this.endTagCallbackRequested;
            ++this.endTagActionStackTop;
          }
          ++this.currentLevel;
          this.PostProcessStartTag();
        }
        else
        {
          --this.currentLevel;
          if (this.dropLevel == int.MaxValue || this.currentLevel >= this.dropLevel)
            return;
          this.dropLevel = int.MaxValue;
        }
      }
    }

    private void ProcessEndTag()
    {
      HtmlToHtmlConverter.AvailableTagParts availableTagParts = HtmlToHtmlConverter.AvailableTagParts.None;
      if (this.token.IsTagBegin)
      {
        if (this.currentLevel > 0)
          --this.currentLevel;
        this.tagIndex = this.token.TagIndex;
        this.tagDropped = false;
        this.tagCallbackRequested = false;
        this.tagHasFilteredStyleAttribute = false;
        availableTagParts = HtmlToHtmlConverter.AvailableTagParts.TagBegin;
        this.PreProcessEndTag();
        if (this.currentLevel >= this.dropLevel)
        {
          this.tagDropped = true;
        }
        else
        {
          if (this.endTagActionStackTop != 0 && this.tagIndex > HtmlTagIndex.Unknown && this.endTagActionStack[this.endTagActionStackTop - 1].TagLevel >= this.currentLevel)
          {
            if (this.endTagActionStack[this.endTagActionStackTop - 1].TagLevel == this.currentLevel)
            {
              --this.endTagActionStackTop;
              this.tagDropped = this.endTagActionStack[this.endTagActionStackTop].Drop;
              this.tagCallbackRequested = this.endTagActionStack[this.endTagActionStackTop].Callback;
            }
            else
            {
              int index1 = this.endTagActionStackTop;
              while (index1 > 0 && this.endTagActionStack[index1 - 1].TagLevel > this.currentLevel)
                --index1;
              for (int index2 = index1; index2 < this.endTagActionStackTop; ++index2)
                this.endTagActionStack[index2].TagLevel -= 2;
              if (index1 > 0 && this.endTagActionStack[index1 - 1].TagLevel == this.currentLevel)
              {
                this.tagDropped = this.endTagActionStack[index1 - 1].Drop;
                this.tagCallbackRequested = this.endTagActionStack[index1 - 1].Callback;
                for (; index1 < this.endTagActionStackTop; ++index1)
                  this.endTagActionStack[index1 - 1] = this.endTagActionStack[index1];
                --this.endTagActionStackTop;
              }
            }
          }
          if (this.token.Argument == 1 && this.tagIndex == HtmlTagIndex.Unknown)
            this.tagDropped = true;
        }
      }
      if (!this.tagDropped)
      {
        HtmlToken.TagPartMinor minor = this.token.MinorPart & (HtmlToken.TagPartMinor) 71;
        if (this.token.IsTagEnd)
          availableTagParts |= HtmlToHtmlConverter.AvailableTagParts.TagEnd;
        if (this.token.HasNameFragment)
          availableTagParts |= HtmlToHtmlConverter.AvailableTagParts.TagName;
        if (this.CopyPendingStateFlag == HtmlToHtmlConverter.CopyPendingState.TagNameCopyPending)
        {
          this.token.Name.WriteTo((ITextSink) this.writer.WriteTagName());
          if (this.token.IsTagNameEnd)
            this.CopyPendingStateFlag = HtmlToHtmlConverter.CopyPendingState.NotPending;
          this.token.Name.MakeEmpty();
          availableTagParts &= ~HtmlToHtmlConverter.AvailableTagParts.TagName;
          minor &= (HtmlToken.TagPartMinor) 248;
        }
        if (availableTagParts != HtmlToHtmlConverter.AvailableTagParts.None)
        {
          if (this.tagCallbackRequested)
          {
            if (this.token.IsTagBegin)
              this.callbackContext.InitializeTag(true, HtmlDtd.tags[(int) this.tagIndex].NameIndex, false);
            this.callbackContext.InitializeFragment(false, 0, new HtmlTagParts(this.token.MajorPart, minor));
            this.callback((HtmlTagContext) this.callbackContext, this.writer);
            this.callbackContext.UninitializeFragment();
          }
          else if (this.token.IsTagBegin)
            this.CopyInputTag(false);
        }
      }
      else if (this.tagCallbackRequested)
      {
        HtmlToken.TagPartMinor minor = this.token.MinorPart & (HtmlToken.TagPartMinor) 71;
        if (this.token.IsTagBegin)
          this.callbackContext.InitializeTag(true, HtmlDtd.tags[(int) this.tagIndex].NameIndex, true);
        this.callbackContext.InitializeFragment(false, 0, new HtmlTagParts(this.token.MajorPart, minor));
        this.callback((HtmlTagContext) this.callbackContext, this.writer);
        this.callbackContext.UninitializeFragment();
      }
      if (!this.token.IsTagEnd)
        return;
      if (this.writer.IsTagOpen)
        this.writer.WriteTagEnd();
      if (this.tagIndex > HtmlTagIndex.Unknown)
      {
        if (this.currentLevel > 0)
          --this.currentLevel;
        if (this.dropLevel == int.MaxValue || this.currentLevel >= this.dropLevel)
          return;
        this.dropLevel = int.MaxValue;
      }
      else
      {
        if (this.currentLevel <= 0)
          return;
        ++this.currentLevel;
      }
    }

    private void ProcessOverlappedClose()
    {
      this.currentLevelDelta = this.token.Argument * 2;
      this.currentLevel -= this.currentLevelDelta;
    }

    private void ProcessOverlappedReopen()
    {
      this.currentLevel += this.token.Argument * 2;
      this.currentLevelDelta = 0;
    }

    private void ProcessText()
    {
      if (this.currentLevel >= this.dropLevel)
        return;
      if (this.insideCSS && this.filterHtml)
        this.AppendCssFromTokenText();
      else if (this.token.Argument == 1)
      {
        this.writer.WriteCollapsedWhitespace();
      }
      else
      {
        if (!this.token.Runs.MoveNext(true))
          return;
        this.token.Text.WriteTo((ITextSink) this.writer.WriteText());
      }
    }

    private void ProcessInjectionBegin()
    {
      if (this.token.Argument != 0 || !this.headDivUnterminated)
        return;
      this.writer.WriteEndTag(HtmlNameIndex.Div);
      this.writer.WriteAutoNewLine(true);
      this.headDivUnterminated = false;
    }

    private void ProcessInjectionEnd()
    {
      if (this.token.Argument == 0)
        return;
      this.writer.WriteAutoNewLine(true);
      this.writer.WriteStartTag(HtmlNameIndex.Div);
      this.headDivUnterminated = true;
    }

    private void ProcessEof()
    {
      this.writer.SetCopyPending(false);
      if (this.headDivUnterminated && this.dropLevel != 0)
      {
        this.writer.WriteEndTag(HtmlNameIndex.Div);
        this.writer.WriteAutoNewLine(true);
        this.headDivUnterminated = false;
      }
      if (this.outputFragment && !this.insideBody)
      {
        this.writer.WriteStartTag(HtmlNameIndex.Div);
        this.writer.WriteEndTag(HtmlNameIndex.Div);
        this.writer.WriteAutoNewLine(true);
      }
      if (!this.convertFragment)
        this.writer.Flush();
      this.endOfFile = true;
    }

    private void PreProcessStartTag()
    {
      if (this.tagIndex <= HtmlTagIndex.Unknown)
        return;
      if (this.tagIndex == HtmlTagIndex.Body)
      {
        if (!this.outputFragment)
          return;
        this.insideBody = true;
        this.tagIndex = HtmlTagIndex.Div;
      }
      else if (this.tagIndex == HtmlTagIndex.Meta)
      {
        if (this.filterHtml)
          return;
        this.token.Attributes.Rewind();
        foreach (HtmlAttribute htmlAttribute in this.token.Attributes)
        {
          if (htmlAttribute.NameIndex == HtmlNameIndex.HttpEquiv)
          {
            if (htmlAttribute.Value.CaseInsensitiveCompareEqual("content-type") || htmlAttribute.Value.CaseInsensitiveCompareEqual("charset"))
            {
              this.tagDropped = true;
              break;
            }
          }
          else if (htmlAttribute.NameIndex == HtmlNameIndex.Charset)
          {
            this.tagDropped = true;
            break;
          }
        }
      }
      else if (this.tagIndex == HtmlTagIndex.Style)
      {
        this.styleIsCSS = true;
        if (!this.token.Attributes.Find(HtmlNameIndex.Type) || this.token.Attributes.Current.Value.CaseInsensitiveCompareEqual("text/css"))
          return;
        this.styleIsCSS = false;
      }
      else if (this.tagIndex == HtmlTagIndex.TC)
        this.tagDropped = true;
      else if (this.tagIndex == HtmlTagIndex.PlainText || this.tagIndex == HtmlTagIndex.Xmp)
      {
        if (!this.filterHtml && (!this.hasTailInjection || this.tagIndex != HtmlTagIndex.PlainText))
          return;
        this.tagDropped = true;
        this.writer.WriteAutoNewLine(true);
        this.writer.WriteStartTag(HtmlNameIndex.TT);
        this.writer.WriteStartTag(HtmlNameIndex.Pre);
        this.writer.WriteAutoNewLine();
      }
      else
      {
        if (this.tagIndex != HtmlTagIndex.Image || !this.filterHtml)
          return;
        this.tagIndex = HtmlTagIndex.Img;
      }
    }

    private void ProcessTagAttributes()
    {
      this.attributeSkipCount = 0;
      HtmlToken.AttributeEnumerator attributes = this.token.Attributes;
      if (this.filterHtml)
      {
        this.attributeCount = 0;
        this.attributeIndirect = true;
        this.attributeVirtualCount = 0;
        this.attributeVirtualScratch.Reset();
        if (this.attributeIndirectIndex == null)
          this.attributeIndirectIndex = new HtmlToHtmlConverter.AttributeIndirectEntry[Math.Max(attributes.Count + 1, 32)];
        else if (this.attributeIndirectIndex.Length <= attributes.Count)
          this.attributeIndirectIndex = new HtmlToHtmlConverter.AttributeIndirectEntry[Math.Max(this.attributeIndirectIndex.Length * 2, attributes.Count + 1)];
        for (int index1 = 0; index1 < attributes.Count; ++index1)
        {
          HtmlAttribute htmlAttribute = attributes[index1];
          HtmlFilterData.FilterAction filterAction1;
          if (htmlAttribute.IsAttrBegin)
          {
            HtmlFilterData.FilterAction filterAction2 = this.filterForFragment ? HtmlFilterData.filterInstructions[(int) htmlAttribute.NameIndex].attrFragmentAction : HtmlFilterData.filterInstructions[(int) htmlAttribute.NameIndex].attrAction;
            if ((filterAction2 & HtmlFilterData.FilterAction.HasExceptions) != HtmlFilterData.FilterAction.Unknown && (HtmlFilterData.filterInstructions[(int) this.token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions) != HtmlFilterData.FilterAction.Unknown)
            {
              for (int index2 = 0; index2 < HtmlFilterData.filterExceptions.Length; ++index2)
              {
                if (HtmlFilterData.filterExceptions[index2].tagNameIndex == this.token.NameIndex && HtmlFilterData.filterExceptions[index2].attrNameIndex == htmlAttribute.NameIndex)
                {
                  filterAction2 = this.filterForFragment ? HtmlFilterData.filterExceptions[index2].fragmentAction : HtmlFilterData.filterExceptions[index2].action;
                  break;
                }
              }
            }
            if (htmlAttribute.AttributeValueContainsDangerousCharacter)
            {
              if (htmlAttribute.AttributeValueContainsBackquote)
                filterAction2 = HtmlFilterData.FilterAction.Drop;
              if (htmlAttribute.AttributeValueContainsBackslash && (filterAction2 & HtmlFilterData.FilterAction.SanitizeUrl) == HtmlFilterData.FilterAction.Unknown)
                filterAction2 = HtmlFilterData.FilterAction.Drop;
            }
            if (!this.outputFragment && (filterAction2 == HtmlFilterData.FilterAction.PrefixName || filterAction2 == HtmlFilterData.FilterAction.PrefixNameList))
              filterAction2 = HtmlFilterData.FilterAction.Keep;
            if (this.callback != null && !this.ignoreAttrCallback && (filterAction2 & HtmlFilterData.FilterAction.Callback) != HtmlFilterData.FilterAction.Unknown)
            {
              if (this.token.IsTagBegin || !this.truncateForCallback)
              {
                this.attributeTriggeredCallback = this.attributeTriggeredCallback || !this.tagCallbackRequested;
                this.tagCallbackRequested = true;
              }
              else
                filterAction2 = HtmlFilterData.FilterAction.KeepDropContent;
            }
            filterAction1 = filterAction2 & HtmlFilterData.FilterAction.ActionMask;
            if (!htmlAttribute.IsAttrEnd)
              this.attrContinuationAction = filterAction1;
          }
          else
          {
            filterAction1 = this.attrContinuationAction;
            if (htmlAttribute.AttributeValueContainsDangerousCharacter)
            {
              if (htmlAttribute.AttributeValueContainsBackquote)
                filterAction1 = HtmlFilterData.FilterAction.Drop;
              if (htmlAttribute.AttributeValueContainsBackslash && (filterAction1 & HtmlFilterData.FilterAction.SanitizeUrl) == HtmlFilterData.FilterAction.Unknown)
                filterAction1 = HtmlFilterData.FilterAction.Drop;
            }
          }
          if (filterAction1 != HtmlFilterData.FilterAction.Drop)
          {
            if (filterAction1 == HtmlFilterData.FilterAction.Keep)
            {
              this.attributeIndirectIndex[this.attributeCount].Index = (short) index1;
              this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.PassThrough;
              ++this.attributeCount;
            }
            else if (filterAction1 == HtmlFilterData.FilterAction.KeepDropContent)
            {
              this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;
              this.attributeIndirectIndex[this.attributeCount].Index = (short) index1;
              this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.EmptyValue;
              ++this.attributeCount;
            }
            else if (filterAction1 == HtmlFilterData.FilterAction.FilterStyleAttribute)
            {
              if (htmlAttribute.IsAttrBegin)
              {
                if (this.tagHasFilteredStyleAttribute)
                  this.AppendCss(";");
                this.tagHasFilteredStyleAttribute = true;
              }
              this.AppendCssFromAttribute(htmlAttribute);
            }
            else if (filterAction1 == HtmlFilterData.FilterAction.ConvertBgcolorIntoStyle)
            {
              if (htmlAttribute.IsAttrBegin)
              {
                if (this.tagHasFilteredStyleAttribute)
                  this.AppendCss(";");
                this.tagHasFilteredStyleAttribute = true;
              }
              this.AppendCss("background-color:");
              this.AppendCssFromAttribute(htmlAttribute);
            }
            else
            {
              if (htmlAttribute.IsAttrBegin)
                this.attributeLeadingSpaces = true;
              if (this.attributeLeadingSpaces)
              {
                if (!htmlAttribute.Value.SkipLeadingWhitespace() && !htmlAttribute.IsAttrEnd)
                {
                  if (htmlAttribute.IsAttrBegin || htmlAttribute.HasNameFragment)
                  {
                    this.attributeIndirectIndex[this.attributeCount].Index = (short) index1;
                    this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.NameOnlyFragment;
                    ++this.attributeCount;
                    continue;
                  }
                  continue;
                }
                this.attributeLeadingSpaces = false;
                this.attributeActionScratch.Reset();
              }
              bool flag = false;
              if (!this.attributeActionScratch.AppendHtmlAttributeValue(htmlAttribute, 4096))
                flag = true;
              if (!htmlAttribute.IsAttrEnd && !flag)
              {
                if (htmlAttribute.IsAttrBegin || htmlAttribute.HasNameFragment)
                {
                  this.attributeIndirectIndex[this.attributeCount].Index = (short) index1;
                  this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.NameOnlyFragment;
                  ++this.attributeCount;
                }
              }
              else
              {
                this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;
                if (filterAction1 == HtmlFilterData.FilterAction.SanitizeUrl)
                {
                  switch (HtmlToHtmlConverter.CheckUrl(this.attributeActionScratch.Buffer, this.attributeActionScratch.Length, this.tagCallbackRequested))
                  {
                    case HtmlToHtmlConverter.CheckUrlResult.Inconclusive:
                      if (this.attributeActionScratch.Length > 256 || !htmlAttribute.IsAttrEnd)
                        break;
                      goto case 2;
                    case HtmlToHtmlConverter.CheckUrlResult.Safe:
                      if (htmlAttribute.IsCompleteAttr)
                      {
                        this.attributeIndirectIndex[this.attributeCount].Index = (short) index1;
                        this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.PassThrough;
                        ++this.attributeCount;
                        continue;
                      }
                      int length1 = this.attributeVirtualScratch.Length;
                      int length2 = this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, 0, this.attributeActionScratch.Length, int.MaxValue);
                      this.attributeIndirectIndex[this.attributeCount].Index = (short) this.AllocateVirtualEntry(index1, length1, length2);
                      this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.Virtual;
                      ++this.attributeCount;
                      continue;
                    case HtmlToHtmlConverter.CheckUrlResult.LocalHyperlink:
                      if (this.outputFragment)
                      {
                        int length3 = HtmlToHtmlConverter.NonWhitespaceLength(this.attributeActionScratch.Buffer, 1, this.attributeActionScratch.Length - 1);
                        if (length3 != 0)
                        {
                          int length4 = this.attributeVirtualScratch.Length;
                          int length5 = 0 + this.attributeVirtualScratch.Append('#', int.MaxValue) + this.attributeVirtualScratch.Append(HtmlToHtmlConverter.NamePrefix, int.MaxValue) + this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, 1, length3, int.MaxValue);
                          this.attributeIndirectIndex[this.attributeCount].Index = (short) this.AllocateVirtualEntry(index1, length4, length5);
                          this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.Virtual;
                          ++this.attributeCount;
                          continue;
                        }
                        break;
                      }
                      goto case 2;
                  }
                  this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;
                  this.attributeIndirectIndex[this.attributeCount].Index = (short) index1;
                  this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.EmptyValue;
                  ++this.attributeCount;
                }
                else if (filterAction1 == HtmlFilterData.FilterAction.PrefixName)
                {
                  int length1 = this.attributeVirtualScratch.Length;
                  int length2 = 0;
                  int length3 = HtmlToHtmlConverter.NonWhitespaceLength(this.attributeActionScratch.Buffer, 0, this.attributeActionScratch.Length);
                  if (length3 != 0)
                    length2 = length2 + this.attributeVirtualScratch.Append(HtmlToHtmlConverter.NamePrefix, int.MaxValue) + this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, 0, length3, int.MaxValue);
                  this.attributeIndirectIndex[this.attributeCount].Index = (short) this.AllocateVirtualEntry(index1, length1, length2);
                  this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.Virtual;
                  ++this.attributeCount;
                }
                else if (filterAction1 == HtmlFilterData.FilterAction.PrefixNameList)
                {
                  int length1 = this.attributeVirtualScratch.Length;
                  int length2 = 0;
                  int offset1 = 0;
                  int length3 = HtmlToHtmlConverter.NonWhitespaceLength(this.attributeActionScratch.Buffer, offset1, this.attributeActionScratch.Length - offset1);
                  if (length3 != 0)
                  {
                    do
                    {
                      length2 = length2 + this.attributeVirtualScratch.Append(HtmlToHtmlConverter.NamePrefix, int.MaxValue) + this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, offset1, length3, int.MaxValue);
                      int offset2 = offset1 + length3;
                      offset1 = offset2 + HtmlToHtmlConverter.WhitespaceLength(this.attributeActionScratch.Buffer, offset2, this.attributeActionScratch.Length - offset2);
                      length3 = HtmlToHtmlConverter.NonWhitespaceLength(this.attributeActionScratch.Buffer, offset1, this.attributeActionScratch.Length - offset1);
                      if (length3 != 0)
                        length2 += this.attributeVirtualScratch.Append(' ', int.MaxValue);
                    }
                    while (length3 != 0);
                  }
                  this.attributeIndirectIndex[this.attributeCount].Index = (short) this.AllocateVirtualEntry(index1, length1, length2);
                  this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.Virtual;
                  ++this.attributeCount;
                }
                else
                {
                  this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;
                  this.attributeIndirectIndex[this.attributeCount].Index = (short) index1;
                  this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.EmptyValue;
                  ++this.attributeCount;
                }
              }
            }
          }
        }
        if (!this.tagHasFilteredStyleAttribute || !this.token.IsTagEnd && (!this.tagCallbackRequested || !this.truncateForCallback))
          return;
        this.attributeIndirectIndex[this.attributeCount].Index = (short) -1;
        this.attributeIndirectIndex[this.attributeCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.FilteredStyle;
        ++this.attributeCount;
      }
      else
      {
        this.attributeCount = attributes.Count;
        this.attributeIndirect = false;
        if (this.callback == null || this.tagCallbackRequested || this.ignoreAttrCallback)
          return;
        for (int index1 = 0; index1 < attributes.Count; ++index1)
        {
          HtmlAttribute htmlAttribute = attributes[index1];
          if (htmlAttribute.IsAttrBegin)
          {
            HtmlFilterData.FilterAction filterAction = HtmlFilterData.filterInstructions[(int) htmlAttribute.NameIndex].attrAction;
            if ((filterAction & HtmlFilterData.FilterAction.HasExceptions) != HtmlFilterData.FilterAction.Unknown && (HtmlFilterData.filterInstructions[(int) this.token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions) != HtmlFilterData.FilterAction.Unknown)
            {
              for (int index2 = 0; index2 < HtmlFilterData.filterExceptions.Length; ++index2)
              {
                if (HtmlFilterData.filterExceptions[index2].tagNameIndex == this.token.NameIndex && HtmlFilterData.filterExceptions[index2].attrNameIndex == htmlAttribute.NameIndex)
                {
                  filterAction = HtmlFilterData.filterExceptions[index2].action;
                  break;
                }
              }
            }
            if ((filterAction & HtmlFilterData.FilterAction.Callback) != HtmlFilterData.FilterAction.Unknown && (this.token.IsTagBegin || !this.truncateForCallback))
            {
              this.attributeTriggeredCallback = this.attributeTriggeredCallback || !this.tagCallbackRequested;
              this.tagCallbackRequested = true;
              break;
            }
          }
        }
      }
    }

    private void PostProcessStartTag()
    {
      if (this.tagIndex != HtmlTagIndex.Style || !this.styleIsCSS)
        return;
      this.insideCSS = true;
    }

    private void PreProcessEndTag()
    {
      if (this.tagIndex > HtmlTagIndex.Unknown)
      {
        if ((HtmlDtd.tags[(int) this.tagIndex].Literal & HtmlDtd.Literal.Entities) != HtmlDtd.Literal.None)
        {
          if (this.tagIndex == HtmlTagIndex.Style && this.insideCSS && this.filterHtml)
            this.FlushCssInStyleTag();
          this.insideCSS = false;
          this.styleIsCSS = true;
        }
        if (this.tagIndex == HtmlTagIndex.PlainText || this.tagIndex == HtmlTagIndex.Xmp)
        {
          if (this.filterHtml || this.hasTailInjection && this.tagIndex == HtmlTagIndex.PlainText)
          {
            this.tagDropped = true;
            this.writer.WriteEndTag(HtmlNameIndex.Pre);
            this.writer.WriteEndTag(HtmlNameIndex.TT);
          }
          else
          {
            if (this.tagIndex != HtmlTagIndex.PlainText || !this.normalizedInput)
              return;
            this.tagDropped = true;
            this.dropLevel = 0;
            this.endTagActionStackTop = 0;
          }
        }
        else if (this.tagIndex == HtmlTagIndex.Body)
        {
          if (this.headDivUnterminated && this.dropLevel != 0)
          {
            this.writer.WriteEndTag(HtmlNameIndex.Div);
            this.writer.WriteAutoNewLine(true);
            this.headDivUnterminated = false;
          }
          if (!this.outputFragment)
            return;
          this.tagIndex = HtmlTagIndex.Div;
        }
        else if (this.tagIndex == HtmlTagIndex.TC)
        {
          this.tagDropped = true;
        }
        else
        {
          if (this.tagIndex != HtmlTagIndex.Image || !this.filterHtml)
            return;
          this.tagIndex = HtmlTagIndex.Img;
        }
      }
      else
      {
        if (this.tagIndex != HtmlTagIndex.Unknown || !this.filterHtml)
          return;
        this.tagDropped = true;
      }
    }

    private void CopyInputTagAttributes()
    {
      for (int index = 0; index < this.attributeCount; ++index)
        this.CopyInputAttribute(index);
    }

    private int AllocateVirtualEntry(int index, int offset, int length)
    {
      if (this.attributeVirtualList == null)
        this.attributeVirtualList = new HtmlToHtmlConverter.AttributeVirtualEntry[4];
      else if (this.attributeVirtualList.Length == this.attributeVirtualCount)
      {
        HtmlToHtmlConverter.AttributeVirtualEntry[] attributeVirtualEntryArray = new HtmlToHtmlConverter.AttributeVirtualEntry[this.attributeVirtualList.Length * 2];
        Array.Copy((Array) this.attributeVirtualList, 0, (Array) attributeVirtualEntryArray, 0, this.attributeVirtualCount);
        this.attributeVirtualList = attributeVirtualEntryArray;
      }
      int index1 = this.attributeVirtualCount++;
      this.attributeVirtualList[index1].Index = (short) index;
      this.attributeVirtualList[index1].Offset = offset;
      this.attributeVirtualList[index1].Length = length;
      this.attributeVirtualList[index1].Position = 0;
      return index1;
    }

    private void VirtualizeFilteredStyle(int index)
    {
      int length1 = this.attributeVirtualScratch.Length;
      this.FlushCssInStyleAttributeToVirtualScratch();
      int length2 = this.attributeVirtualScratch.Length - length1;
      int num = this.AllocateVirtualEntry((int) this.attributeIndirectIndex[index + this.attributeSkipCount].Index, length1, length2);
      this.attributeIndirectIndex[index + this.attributeSkipCount].Index = (short) num;
      this.attributeIndirectIndex[index + this.attributeSkipCount].Kind = HtmlToHtmlConverter.AttributeIndirectKind.VirtualFilteredStyle;
    }

    private bool InjectMetaTagIfNecessary()
    {
      if (this.filterForFragment || !this.writer.HasEncoding)
        this.metaInjected = true;
      else if (this.token.HtmlTokenId != HtmlTokenId.Restart && this.token.HtmlTokenId != HtmlTokenId.EncodingChange)
      {
        if (string.Compare(this.writer.Encoding.WebName, "utf-7", StringComparison.OrdinalIgnoreCase) == 0)
        {
          this.OutputMetaTag();
          this.metaInjected = true;
        }
        else if (this.token.HtmlTokenId == HtmlTokenId.Tag)
        {
          if (!this.insideHtml && this.token.TagIndex == HtmlTagIndex.Html)
          {
            if (this.token.IsTagEnd)
              this.insideHtml = true;
          }
          else if (!this.insideHead && this.token.TagIndex == HtmlTagIndex.Head)
          {
            if (this.token.IsTagEnd)
              this.insideHead = true;
          }
          else if (this.token.TagIndex > HtmlTagIndex._ASP)
          {
            if (this.insideHtml && !this.insideHead)
            {
              this.writer.WriteNewLine(true);
              this.writer.WriteStartTag(HtmlNameIndex.Head);
              this.writer.WriteNewLine(true);
              this.OutputMetaTag();
              this.writer.WriteEndTag(HtmlNameIndex.Head);
              this.writer.WriteNewLine(true);
            }
            else
            {
              if (this.insideHead)
                this.writer.WriteNewLine(true);
              this.OutputMetaTag();
            }
            this.metaInjected = true;
          }
        }
        else if (this.token.HtmlTokenId == HtmlTokenId.Text)
        {
          if (this.token.IsWhitespaceOnly)
            return false;
          this.token.Text.StripLeadingWhitespace();
          if (this.insideHtml && !this.insideHead)
          {
            this.writer.WriteNewLine(true);
            this.writer.WriteStartTag(HtmlNameIndex.Head);
            this.writer.WriteNewLine(true);
            this.OutputMetaTag();
            this.writer.WriteEndTag(HtmlNameIndex.Head);
            this.writer.WriteNewLine(true);
          }
          else
          {
            if (this.insideHead)
              this.writer.WriteNewLine(true);
            this.OutputMetaTag();
          }
          this.metaInjected = true;
        }
      }
      return true;
    }

    private void OutputMetaTag()
    {
      Encoding encoding = this.writer.Encoding;
      if (string.Compare(encoding.WebName, "utf-7", StringComparison.OrdinalIgnoreCase) == 0)
        this.writer.Encoding = CTSGlobals.AsciiEncoding;
      this.writer.WriteStartTag(HtmlNameIndex.Meta);
      this.writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
      this.writer.WriteAttributeName(HtmlNameIndex.Content);
      this.writer.WriteAttributeValueInternal("text/html; charset=");
      this.writer.WriteAttributeValueInternal(Globalization.Charset.GetCharset(encoding).Name);
      if (string.Compare(encoding.WebName, "utf-7", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      this.writer.WriteTagEnd();
      this.writer.Encoding = encoding;
    }

    private HtmlToHtmlConverter.AttributeIndirectKind GetAttributeIndirectKind(int index)
    {
      if (!this.attributeIndirect)
        return HtmlToHtmlConverter.AttributeIndirectKind.PassThrough;
      return this.attributeIndirectIndex[index + this.attributeSkipCount].Kind;
    }

    private int GetAttributeVirtualEntryIndex(int index)
    {
      return (int) this.attributeIndirectIndex[index + this.attributeSkipCount].Index;
    }

    private HtmlAttribute GetAttribute(int index)
    {
      if (!this.attributeIndirect)
        return this.token.Attributes[index + this.attributeSkipCount];
      if (this.attributeIndirectIndex[index + this.attributeSkipCount].Kind != HtmlToHtmlConverter.AttributeIndirectKind.Virtual)
        return this.token.Attributes[(int) this.attributeIndirectIndex[index + this.attributeSkipCount].Index];
      return this.token.Attributes[(int) this.attributeVirtualList[(int) this.attributeIndirectIndex[index + this.attributeSkipCount].Index].Index];
    }

    private void AppendCssFromTokenText()
    {
      if (this.cssParserInput == null)
      {
        this.cssParserInput = new ConverterBufferInput(524288, this.progressMonitor);
        this.cssParser = new Css.CssParser((ConverterInput) this.cssParserInput, 4096, false);
      }
      this.token.Text.WriteTo((ITextSink) this.cssParserInput);
    }

    private void AppendCss(string css)
    {
      if (this.cssParserInput == null)
      {
        this.cssParserInput = new ConverterBufferInput(524288, this.progressMonitor);
        this.cssParser = new Css.CssParser((ConverterInput) this.cssParserInput, 4096, false);
      }
      this.cssParserInput.Write(css);
    }

    private void AppendCssFromAttribute(HtmlAttribute attribute)
    {
      if (this.cssParserInput == null)
      {
        this.cssParserInput = new ConverterBufferInput(524288, this.progressMonitor);
        this.cssParser = new Css.CssParser((ConverterInput) this.cssParserInput, 4096, false);
      }
      attribute.Value.Rewind();
      attribute.Value.WriteTo((ITextSink) this.cssParserInput);
    }

    private void FlushCssInStyleTag()
    {
      if (this.cssParserInput == null)
        return;
      this.writer.WriteNewLine();
      this.writer.WriteMarkupText("<!--");
      this.writer.WriteNewLine();
      bool agressiveFiltering = false;
      if (this.smallCssBlockThreshold != -1 && this.cssParserInput.MaxTokenSize > this.smallCssBlockThreshold)
        agressiveFiltering = true;
      this.cssParser.SetParseMode(Css.CssParseMode.StyleTag);
      bool firstProperty = true;
      ITextSinkEx sink = this.writer.WriteText();
      Css.CssTokenId cssTokenId;
      do
      {
        cssTokenId = this.cssParser.Parse();
        if ((Css.CssTokenId.RuleSet == cssTokenId || Css.CssTokenId.AtRule == cssTokenId) && this.cssParser.Token.Selectors.ValidCount != 0 && (this.cssParser.Token.Properties.ValidCount != 0 && this.CopyInputCssSelectors(this.cssParser.Token.Selectors, sink, agressiveFiltering)))
        {
          if (this.cssParser.Token.IsPropertyListBegin)
            sink.Write("\r\n\t{");
          this.CopyInputCssProperties(true, this.cssParser.Token.Properties, sink, ref firstProperty);
          if (this.cssParser.Token.IsPropertyListEnd)
          {
            sink.Write("}\r\n");
            firstProperty = true;
          }
        }
      }
      while (Css.CssTokenId.EndOfFile != cssTokenId);
      this.cssParserInput.Reset();
      this.cssParser.Reset();
      this.writer.WriteMarkupText("-->");
      this.writer.WriteNewLine();
    }

    private void FlushCssInStyleAttributeToVirtualScratch()
    {
      this.cssParser.SetParseMode(Css.CssParseMode.StyleAttribute);
      if (this.virtualScratchSink == null)
        this.virtualScratchSink = new HtmlToHtmlConverter.VirtualScratchSink(this, int.MaxValue);
      bool firstProperty = true;
      Css.CssTokenId cssTokenId;
      do
      {
        cssTokenId = this.cssParser.Parse();
        if (Css.CssTokenId.Declarations == cssTokenId && this.cssParser.Token.Properties.ValidCount != 0)
          this.CopyInputCssProperties(false, this.cssParser.Token.Properties, (ITextSinkEx) this.virtualScratchSink, ref firstProperty);
      }
      while (Css.CssTokenId.EndOfFile != cssTokenId);
      this.cssParserInput.Reset();
      this.cssParser.Reset();
    }

    private void FlushCssInStyleAttribute(HtmlWriter writer)
    {
      this.cssParser.SetParseMode(Css.CssParseMode.StyleAttribute);
      ITextSinkEx sink = writer.WriteAttributeValue();
      bool firstProperty = true;
      Css.CssTokenId cssTokenId;
      do
      {
        cssTokenId = this.cssParser.Parse();
        if (Css.CssTokenId.Declarations == cssTokenId && this.cssParser.Token.Properties.ValidCount != 0)
          this.CopyInputCssProperties(false, this.cssParser.Token.Properties, sink, ref firstProperty);
      }
      while (Css.CssTokenId.EndOfFile != cssTokenId);
      this.cssParserInput.Reset();
      this.cssParser.Reset();
    }

    private bool CopyInputCssSelectors(Css.CssToken.SelectorEnumerator selectors, ITextSinkEx sink, bool agressiveFiltering)
    {
      bool flag1 = false;
      bool flag2 = false;
      selectors.Rewind();
      foreach (Css.CssSelector selector in selectors)
      {
        if (!selector.IsDeleted)
        {
          if (flag2)
          {
            if (selector.Combinator == Css.CssSelectorCombinator.None)
              sink.Write(", ");
            else if (selector.Combinator == Css.CssSelectorCombinator.Descendant)
              sink.Write(32);
            else if (selector.Combinator == Css.CssSelectorCombinator.Adjacent)
              sink.Write(" + ");
            else
              sink.Write(" > ");
          }
          flag2 = this.CopyInputCssSelector(selector, sink, agressiveFiltering);
          flag1 = flag1 || flag2;
        }
      }
      return flag1;
    }

    private bool CopyInputCssSelector(Css.CssSelector selector, ITextSinkEx sink, bool agressiveFiltering)
    {
      if (this.filterForFragment && (!selector.HasClassFragment || selector.ClassType != Css.CssSelectorClassType.Regular && selector.ClassType != Css.CssSelectorClassType.Hash) || agressiveFiltering && (!selector.HasClassFragment || selector.ClassType != Css.CssSelectorClassType.Regular || !selector.ClassName.GetString(256).Equals("MsoNormal", StringComparison.Ordinal)))
        return false;
      if (selector.NameId != HtmlNameIndex.Unknown && selector.NameId != HtmlNameIndex._NOTANAME)
        sink.Write(HtmlNameData.Names[(int) selector.NameId].Name);
      else if (selector.HasNameFragment)
        selector.Name.WriteOriginalTo((ITextSink) sink);
      if (selector.HasClassFragment)
      {
        if (selector.ClassType == Css.CssSelectorClassType.Regular)
          sink.Write(".");
        else if (selector.ClassType == Css.CssSelectorClassType.Hash)
          sink.Write("#");
        else if (selector.ClassType == Css.CssSelectorClassType.Pseudo)
          sink.Write(":");
        if (this.outputFragment)
          sink.Write(HtmlToHtmlConverter.NamePrefix);
        selector.ClassName.WriteOriginalTo((ITextSink) sink);
      }
      return true;
    }

    private void CopyInputCssProperties(bool inTag, Css.CssToken.PropertyEnumerator properties, ITextSinkEx sink, ref bool firstProperty)
    {
      properties.Rewind();
      foreach (Css.CssProperty property in properties)
      {
        if (property.IsPropertyBegin && !property.IsDeleted)
        {
          Css.CssData.FilterAction filterAction = Css.CssData.filterInstructions[(int) property.NameId].propertyAction;
          if (Css.CssData.FilterAction.CheckContent == filterAction)
            filterAction = property.NameId != Css.CssNameIndex.Display || !property.HasValueFragment || (!property.Value.CaseInsensitiveContainsSubstring("none") || this.preserveDisplayNoneStyle) ? (property.NameId != Css.CssNameIndex.Position || !property.HasValueFragment || !property.Value.CaseInsensitiveContainsSubstring("absolute") && !property.Value.CaseInsensitiveContainsSubstring("relative") || !this.outputFragment ? Css.CssData.FilterAction.Keep : Css.CssData.FilterAction.Drop) : Css.CssData.FilterAction.Drop;
          if (Css.CssData.FilterAction.Keep == filterAction)
          {
            if (firstProperty)
              firstProperty = false;
            else
              sink.Write(inTag ? ";\r\n\t" : "; ");
            HtmlToHtmlConverter.CopyInputCssProperty(property, sink);
          }
        }
      }
    }

    internal enum CopyPendingState : byte
    {
      NotPending,
      TagCopyPending,
      TagContentCopyPending,
      TagNameCopyPending,
      AttributeCopyPending,
      AttributeNameCopyPending,
      AttributeValueCopyPending,
    }

    [Flags]
    private enum AvailableTagParts : byte
    {
      None = (byte) 0,
      TagBegin = (byte) 1,
      TagEnd = (byte) 2,
      TagName = (byte) 4,
      Attributes = (byte) 8,
      UnstructuredContent = (byte) 16,
    }

    private enum AttributeIndirectKind
    {
      PassThrough,
      EmptyValue,
      FilteredStyle,
      Virtual,
      VirtualFilteredStyle,
      NameOnlyFragment,
    }

    private enum CheckUrlResult
    {
      Inconclusive,
      Unsafe,
      Safe,
      LocalHyperlink,
    }

    private struct AttributeIndirectEntry
    {
      public HtmlToHtmlConverter.AttributeIndirectKind Kind;
      public short Index;
    }

    private struct AttributeVirtualEntry
    {
      public short Index;
      public int Offset;
      public int Length;
      public int Position;
    }

    private struct EndTagActionEntry
    {
      public int TagLevel;
      public bool Drop;
      public bool Callback;
    }

    internal class VirtualScratchSink : ITextSinkEx, ITextSink
    {
      private HtmlToHtmlConverter converter;
      private int maxLength;

      public bool IsEnough
      {
        get
        {
          return this.converter.attributeVirtualScratch.Length >= this.maxLength;
        }
      }

      public VirtualScratchSink(HtmlToHtmlConverter converter, int maxLength)
      {
        this.converter = converter;
        this.maxLength = maxLength;
      }

      public void Write(char[] buffer, int offset, int count)
      {
        this.converter.attributeVirtualScratch.Append(buffer, offset, count, this.maxLength);
      }

      public void Write(int ucs32Char)
      {
        if (Token.LiteralLength(ucs32Char) == 1)
        {
          this.converter.attributeVirtualScratch.Append((char) ucs32Char, this.maxLength);
        }
        else
        {
          this.converter.attributeVirtualScratch.Append(Token.LiteralFirstChar(ucs32Char), this.maxLength);
          if (this.IsEnough)
            return;
          this.converter.attributeVirtualScratch.Append(Token.LiteralLastChar(ucs32Char), this.maxLength);
        }
      }

      public void Write(string value)
      {
        this.converter.attributeVirtualScratch.Append(value, this.maxLength);
      }

      public void WriteNewLine()
      {
        this.converter.attributeVirtualScratch.Append('\r', this.maxLength);
        if (this.IsEnough)
          return;
        this.converter.attributeVirtualScratch.Append('\n', this.maxLength);
      }
    }
  }
}
