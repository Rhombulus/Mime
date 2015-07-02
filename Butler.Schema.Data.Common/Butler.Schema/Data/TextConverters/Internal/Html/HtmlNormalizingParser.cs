// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlNormalizingParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlNormalizingParser : IHtmlParser, IRestartable, IReusable, IDisposable
  {
    private bool ensureHead = true;
    private HtmlParser parser;
    private IRestartable restartConsumer;
    private int maxElementStack;
    private HtmlNormalizingParser.Context context;
    private HtmlNormalizingParser.Context[] contextStack;
    private int contextStackTop;
    private HtmlTagIndex[] elementStack;
    private int elementStackTop;
    private HtmlNormalizingParser.QueueItem[] queue;
    private int queueHead;
    private int queueTail;
    private int queueStart;
    private int[] closeList;
    private HtmlTagIndex[] openList;
    private bool validRTC;
    private HtmlTagIndex tagIdRTC;
    private HtmlToken token;
    private HtmlToken inputToken;
    private bool ignoreInputTag;
    private int currentRun;
    private int currentRunOffset;
    private int numRuns;
    private bool allowWspLeft;
    private bool allowWspRight;
    private HtmlNormalizingParser.SmallTokenBuilder tokenBuilder;
    private HtmlInjection injection;
    private HtmlNormalizingParser.DocumentState saveState;

    public HtmlToken Token
    {
      get
      {
        return this.token;
      }
    }

    public int CurrentOffset
    {
      get
      {
        return this.parser.CurrentOffset;
      }
    }

    public HtmlNormalizingParser(HtmlParser parser, HtmlInjection injection, bool ensureHead, int maxNesting, bool testBoundaryConditions, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
    {
      this.parser = parser;
      this.parser.SetRestartConsumer((IRestartable) this);
      this.injection = injection;
      if (injection != null)
        this.saveState = new HtmlNormalizingParser.DocumentState();
      this.ensureHead = ensureHead;
      int length = testBoundaryConditions ? 1 : 32;
      this.maxElementStack = testBoundaryConditions ? 30 : maxNesting;
      this.openList = new HtmlTagIndex[8];
      this.closeList = new int[8];
      this.elementStack = new HtmlTagIndex[length];
      this.contextStack = new HtmlNormalizingParser.Context[testBoundaryConditions ? 1 : 4];
      this.elementStack[this.elementStackTop++] = HtmlTagIndex._ROOT;
      this.context.Type = HtmlDtd.ContextType.Root;
      this.context.TextType = HtmlDtd.ContextTextType.Full;
      this.context.Reject = HtmlDtd.SetId.Empty;
      this.queue = new HtmlNormalizingParser.QueueItem[testBoundaryConditions ? 1 : length / 4];
      this.tokenBuilder = new HtmlNormalizingParser.SmallTokenBuilder();
    }

    public void SetRestartConsumer(IRestartable restartConsumer)
    {
      this.restartConsumer = restartConsumer;
    }

    public HtmlTokenId Parse()
    {
      while (this.QueueEmpty())
        this.Process(this.parser.Parse());
      return this.GetTokenFromQueue();
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
    }

    void IReusable.Initialize(object newSourceOrDestination)
    {
      ((IReusable) this.parser).Initialize(newSourceOrDestination);
      this.Reinitialize();
      this.parser.SetRestartConsumer((IRestartable) this);
    }

    public void Initialize(string fragment, bool preformatedText)
    {
      this.parser.Initialize(fragment, preformatedText);
      this.Reinitialize();
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null)
        ((IDisposable) this.parser).Dispose();
      this.parser = (HtmlParser) null;
      this.restartConsumer = (IRestartable) null;
      this.contextStack = (HtmlNormalizingParser.Context[]) null;
      this.queue = (HtmlNormalizingParser.QueueItem[]) null;
      this.closeList = (int[]) null;
      this.openList = (HtmlTagIndex[]) null;
      this.token = (HtmlToken) null;
      this.inputToken = (HtmlToken) null;
      this.tokenBuilder = (HtmlNormalizingParser.SmallTokenBuilder) null;
      GC.SuppressFinalize((object) this);
    }

    private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
    {
      return HtmlDtd.tags[(int) tagIndex];
    }

    private void Reinitialize()
    {
      this.elementStackTop = 0;
      this.contextStackTop = 0;
      this.ignoreInputTag = false;
      this.elementStack[this.elementStackTop++] = HtmlTagIndex._ROOT;
      this.context.TopElement = 0;
      this.context.Type = HtmlDtd.ContextType.Root;
      this.context.TextType = HtmlDtd.ContextTextType.Full;
      this.context.Accept = HtmlDtd.SetId.Null;
      this.context.Reject = HtmlDtd.SetId.Empty;
      this.context.IgnoreEnd = HtmlDtd.SetId.Null;
      this.context.HasSpace = false;
      this.context.EatSpace = false;
      this.context.OneNL = false;
      this.context.LastCh = char.MinValue;
      this.queueHead = 0;
      this.queueTail = 0;
      this.queueStart = 0;
      this.validRTC = false;
      this.tagIdRTC = HtmlTagIndex._NULL;
      this.token = (HtmlToken) null;
      if (this.injection == null)
        return;
      if (this.injection.Active)
        this.parser = (HtmlParser) this.injection.Pop();
      this.injection.Reset();
    }

    private void Process(HtmlTokenId tokenId)
    {
      if (tokenId == HtmlTokenId.None)
      {
        this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.None);
      }
      else
      {
        this.inputToken = this.parser.Token;
        switch (tokenId - (byte) 1)
        {
          case HtmlTokenId.None:
            this.HandleTokenEof();
            break;
          case HtmlTokenId.EndOfFile:
            this.HandleTokenText(this.parser.Token);
            break;
          case HtmlTokenId.Text:
            this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
            break;
          case HtmlTokenId.EncodingChange:
            if (this.parser.Token.NameIndex < HtmlNameIndex.Unknown)
            {
              this.HandleTokenSpecialTag(this.parser.Token);
              break;
            }
            this.HandleTokenTag(this.parser.Token);
            break;
          case HtmlTokenId.Tag:
            this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
            break;
        }
      }
    }

    private void HandleTokenEof()
    {
      if (this.queueHead != this.queueTail && this.queue[this.queueHead].Kind == HtmlNormalizingParser.QueueItemKind.Suspend)
      {
        HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
        this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.EndLastTag, queueItem.TagIndex, (HtmlNormalizingParser.QueueItemFlags) 0 != (queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft), (HtmlNormalizingParser.QueueItemFlags) 0 != (queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight));
      }
      else if (this.injection != null && this.injection.Active)
      {
        this.CloseAllContainers(this.saveState.SavedStackTop);
        if (this.queueHead != this.queueTail)
          return;
        this.saveState.Restore(this);
        this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.InjectionEnd, this.injection.InjectingHead ? 1 : 0);
        this.parser = (HtmlParser) this.injection.Pop();
      }
      else
      {
        if (this.injection != null)
        {
          int container = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
          if (container == -1)
          {
            this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(HtmlTagIndex.Body));
            this.OpenContainer(HtmlTagIndex.Body);
            return;
          }
          this.CloseAllContainers(container + 1);
          if (this.queueHead != this.queueTail)
            return;
          if (this.injection.HaveTail && !this.injection.TailDone)
          {
            this.parser = (HtmlParser) this.injection.Push(false, (IHtmlParser) this.parser);
            this.saveState.Save(this, this.elementStackTop);
            this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 0);
            if (this.injection.HeaderFooterFormat != HeaderFooterFormat.Text)
              return;
            this.OpenContainer(HtmlTagIndex.TT);
            this.OpenContainer(HtmlTagIndex.Pre);
            return;
          }
        }
        this.CloseAllContainers();
        this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Eof);
      }
    }

    private void HandleTokenTag(HtmlToken tag)
    {
      HtmlTagIndex tagIndex = HtmlNameData.Names[(int) tag.NameIndex].TagIndex;
      if (tag.IsTagBegin)
        this.StartTagProcessing(tagIndex, tag);
      else if (!this.ignoreInputTag)
      {
        if (tag.IsTagEnd)
          this.DoDequeueFirst();
        if (this.inputToken.TagIndex != HtmlTagIndex.Unknown)
          this.inputToken.Flags = HtmlNormalizingParser.GetTagDefinition(tagIndex).Scope == HtmlDtd.TagScope.EMPTY ? this.inputToken.Flags | HtmlToken.TagFlags.EmptyScope : this.inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope;
        this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.PassThrough);
      }
      else
      {
        if (!tag.IsTagEnd)
          return;
        this.ignoreInputTag = false;
      }
    }

    private void HandleTokenSpecialTag(HtmlToken tag)
    {
      tag.Flags = tag.Flags | HtmlToken.TagFlags.EmptyScope;
      HtmlTagIndex tagIndex = HtmlNameData.Names[(int) tag.NameIndex].TagIndex;
      if (tag.IsTagBegin)
        this.StartSpecialTagProcessing(tagIndex, tag);
      else if (!this.ignoreInputTag)
      {
        if (tag.IsTagEnd)
          this.DoDequeueFirst();
        this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.PassThrough);
      }
      else
      {
        if (!tag.IsTagEnd)
          return;
        this.ignoreInputTag = false;
      }
    }

    private void HandleTokenText(HtmlToken token)
    {
      HtmlTagIndex tagIndex = this.validRTC ? this.tagIdRTC : this.RequiredTextContainer();
      int num = 0;
      Token.RunEnumerator runs = this.inputToken.Runs;
      if (tagIndex != HtmlTagIndex._NULL)
      {
        do
          ;
        while (runs.MoveNext(true) && runs.Current.TextType <= RunTextType.UnusualWhitespace);
        if (!runs.IsValidPosition)
          return;
        this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(tagIndex));
        this.OpenContainer(tagIndex);
      }
      else if (this.context.TextType != HtmlDtd.ContextTextType.Literal)
      {
        while (runs.MoveNext(true) && runs.Current.TextType <= RunTextType.UnusualWhitespace)
          num += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
      }
      if (this.context.TextType == HtmlDtd.ContextTextType.Literal)
      {
        this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
      }
      else
      {
        if (this.context.TextType != HtmlDtd.ContextTextType.Full)
          return;
        if (num != 0)
          this.AddSpace(num == 1);
        this.currentRun = runs.CurrentIndex;
        this.currentRunOffset = runs.CurrentOffset;
        if (runs.IsValidPosition)
        {
          char firstChar = runs.Current.FirstChar;
          char lastChar;
          do
          {
            lastChar = runs.Current.LastChar;
          }
          while (runs.MoveNext(true) && runs.Current.TextType > RunTextType.UnusualWhitespace);
          this.AddNonspace(firstChar, lastChar);
        }
        this.numRuns = runs.CurrentIndex - this.currentRun;
      }
    }

    private void StartTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
    {
      if (this.context.Reject != HtmlDtd.SetId.Null && !HtmlDtd.IsTagInSet(tagIndex, this.context.Reject) || this.context.Accept != HtmlDtd.SetId.Null && HtmlDtd.IsTagInSet(tagIndex, this.context.Accept))
      {
        if (!tag.IsEndTag)
        {
          if (this.ProcessOpenTag(tagIndex, HtmlNormalizingParser.GetTagDefinition(tagIndex)))
            return;
          this.ProcessIgnoredTag(tagIndex, tag);
        }
        else if (this.context.IgnoreEnd == HtmlDtd.SetId.Null || !HtmlDtd.IsTagInSet(tagIndex, this.context.IgnoreEnd))
        {
          if (this.ProcessEndTag(tagIndex, HtmlNormalizingParser.GetTagDefinition(tagIndex)))
            return;
          this.ProcessIgnoredTag(tagIndex, tag);
        }
        else
          this.ProcessIgnoredTag(tagIndex, tag);
      }
      else if (this.context.Type == HtmlDtd.ContextType.Select && tagIndex == HtmlTagIndex.Select)
      {
        if (this.ProcessEndTag(tagIndex, HtmlNormalizingParser.GetTagDefinition(tagIndex)))
          return;
        this.ProcessIgnoredTag(tagIndex, tag);
      }
      else
        this.ProcessIgnoredTag(tagIndex, tag);
    }

    private void StartSpecialTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
    {
      this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
      if (tag.IsTagEnd)
        return;
      this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
    }

    private void ProcessIgnoredTag(HtmlTagIndex tagIndex, HtmlToken tag)
    {
      if (tag.IsTagEnd)
        return;
      this.ignoreInputTag = true;
    }

    private bool ProcessOpenTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
    {
      if (!this.PrepareContainer(tagIndex, tagDef))
        return false;
      this.PushElement(tagIndex, true, tagDef);
      return true;
    }

    private bool ProcessEndTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
    {
      if (tagIndex == HtmlTagIndex.Unknown)
      {
        this.PushElement(tagIndex, true, tagDef);
        return true;
      }
      bool useInputTag1 = true;
      bool flag = false;
      int stackPos;
      if (tagDef.Match != HtmlDtd.SetId.Null)
      {
        stackPos = this.FindContainer(tagDef.Match, tagDef.EndContainers);
        if (stackPos >= 0 && this.elementStack[stackPos] != tagIndex)
          useInputTag1 = false;
      }
      else
        stackPos = this.FindContainer(tagIndex, tagDef.EndContainers);
      if (stackPos < 0)
      {
        HtmlTagIndex htmlTagIndex = tagDef.UnmatchedSubstitute;
        switch (htmlTagIndex)
        {
          case HtmlTagIndex._NULL:
            return false;
          case HtmlTagIndex._IMPLICIT_BEGIN:
            if (!this.PrepareContainer(tagIndex, tagDef))
              return false;
            this.inputToken.Flags &= ~HtmlToken.TagFlags.EndTag;
            stackPos = this.PushElement(tagIndex, useInputTag1, tagDef);
            flag |= useInputTag1;
            useInputTag1 = false;
            break;
          default:
            stackPos = this.FindContainer(htmlTagIndex, HtmlNormalizingParser.GetTagDefinition(htmlTagIndex).EndContainers);
            if (stackPos < 0)
              return false;
            useInputTag1 = false;
            break;
        }
      }
      if (stackPos >= 0 && stackPos < this.elementStackTop)
      {
        bool useInputTag2 = useInputTag1 & this.inputToken.IsEndTag;
        flag |= useInputTag2;
        this.CloseContainer(stackPos, useInputTag2);
      }
      return flag;
    }

    private bool PrepareContainer(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
    {
      if (tagIndex == HtmlTagIndex.Unknown)
        return true;
      if (tagDef.MaskingContainers != HtmlDtd.SetId.Null && this.FindContainer(tagDef.MaskingContainers, tagDef.BeginContainers) >= 0)
        return false;
      this.CloseAllProhibitedContainers(tagDef);
      HtmlTagIndex tagIndex1 = HtmlTagIndex._NULL;
      if (tagDef.TextType == HtmlDtd.TagTextType.ALWAYS || tagDef.TextType == HtmlDtd.TagTextType.QUERY && this.QueryTextlike(tagIndex))
      {
        tagIndex1 = this.validRTC ? this.tagIdRTC : this.RequiredTextContainer();
        if (tagIndex1 != HtmlTagIndex._NULL)
        {
          this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(tagIndex1));
          if (-1 == this.OpenContainer(tagIndex1))
            return false;
        }
      }
      if (tagIndex1 == HtmlTagIndex._NULL && tagDef.RequiredContainers != HtmlDtd.SetId.Null && this.FindContainer(tagDef.RequiredContainers, tagDef.BeginContainers) < 0)
      {
        this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(tagDef.DefaultContainer));
        if (-1 == this.OpenContainer(tagDef.DefaultContainer))
          return false;
      }
      return true;
    }

    private int OpenContainer(HtmlTagIndex tagIndex)
    {
      int num1 = 0;
      HtmlDtd.TagDefinition tagDefinition;
      for (; tagIndex != HtmlTagIndex._NULL; tagIndex = tagDefinition.DefaultContainer)
      {
        this.openList[num1++] = tagIndex;
        tagDefinition = HtmlNormalizingParser.GetTagDefinition(tagIndex);
        if (tagDefinition.RequiredContainers == HtmlDtd.SetId.Null || this.FindContainer(tagDefinition.RequiredContainers, tagDefinition.BeginContainers) >= 0)
          break;
      }
      if (tagIndex == HtmlTagIndex._NULL)
        return -1;
      int num2 = -1;
      for (int index = num1 - 1; index >= 0; --index)
      {
        tagIndex = this.openList[index];
        num2 = this.PushElement(tagIndex, false, HtmlNormalizingParser.GetTagDefinition(tagIndex));
      }
      return num2;
    }

    private void CloseContainer(int stackPos, bool useInputTag)
    {
      if (stackPos != this.elementStackTop - 1)
      {
        bool flag = false;
        int num1 = 0;
        int[] numArray1 = this.closeList;
        int index1 = num1;
        int num2 = 1;
        int length = index1 + num2;
        int num3 = stackPos;
        numArray1[index1] = num3;
        if (HtmlNormalizingParser.GetTagDefinition(this.elementStack[stackPos]).Scope == HtmlDtd.TagScope.NESTED)
          flag = true;
        for (int index2 = stackPos + 1; index2 < this.elementStackTop; ++index2)
        {
          HtmlDtd.TagDefinition tagDefinition = HtmlNormalizingParser.GetTagDefinition(this.elementStack[index2]);
          if (length == this.closeList.Length)
          {
            int[] numArray2 = new int[this.closeList.Length * 2];
            Array.Copy((Array) this.closeList, 0, (Array) numArray2, 0, length);
            this.closeList = numArray2;
          }
          if (flag && tagDefinition.Scope == HtmlDtd.TagScope.NESTED)
          {
            this.closeList[length++] = index2;
          }
          else
          {
            for (int index3 = 0; index3 < length; ++index3)
            {
              if (HtmlDtd.IsTagInSet(this.elementStack[this.closeList[index3]], tagDefinition.EndContainers))
              {
                this.closeList[length++] = index2;
                flag = flag || tagDefinition.Scope == HtmlDtd.TagScope.NESTED;
                break;
              }
            }
          }
        }
        for (int index2 = length - 1; index2 > 0; --index2)
          this.PopElement(this.closeList[index2], false);
      }
      this.PopElement(stackPos, useInputTag);
    }

    private void CloseAllProhibitedContainers(HtmlDtd.TagDefinition tagDef)
    {
      HtmlDtd.SetId matchSet = tagDef.ProhibitedContainers;
      if (matchSet == HtmlDtd.SetId.Null)
        return;
      while (true)
      {
        int container = this.FindContainer(matchSet, tagDef.BeginContainers);
        if (container >= 0)
          this.CloseContainer(container, false);
        else
          break;
      }
    }

    private void CloseAllContainers()
    {
      for (int stackPos = this.elementStackTop - 1; stackPos > 0; --stackPos)
        this.CloseContainer(stackPos, false);
    }

    private void CloseAllContainers(int level)
    {
      for (int stackPos = this.elementStackTop - 1; stackPos >= level; --stackPos)
        this.CloseContainer(stackPos, false);
    }

    private int FindContainer(HtmlDtd.SetId matchSet, HtmlDtd.SetId stopSet)
    {
      int index;
      for (index = this.elementStackTop - 1; index >= 0 && !HtmlDtd.IsTagInSet(this.elementStack[index], matchSet); --index)
      {
        if (HtmlDtd.IsTagInSet(this.elementStack[index], stopSet))
          return -1;
      }
      return index;
    }

    private int FindContainer(HtmlTagIndex match, HtmlDtd.SetId stopSet)
    {
      int index;
      for (index = this.elementStackTop - 1; index >= 0 && this.elementStack[index] != match; --index)
      {
        if (HtmlDtd.IsTagInSet(this.elementStack[index], stopSet))
          return -1;
      }
      return index;
    }

    private HtmlTagIndex RequiredTextContainer()
    {
      this.validRTC = true;
      for (int index = this.elementStackTop - 1; index >= 0; --index)
      {
        HtmlDtd.TagDefinition tagDefinition = HtmlNormalizingParser.GetTagDefinition(this.elementStack[index]);
        if (tagDefinition.TextScope == HtmlDtd.TagTextScope.INCLUDE)
        {
          this.tagIdRTC = HtmlTagIndex._NULL;
          return this.tagIdRTC;
        }
        if (tagDefinition.TextScope == HtmlDtd.TagTextScope.EXCLUDE)
        {
          this.tagIdRTC = tagDefinition.TextSubcontainer;
          return this.tagIdRTC;
        }
      }
      this.tagIdRTC = HtmlTagIndex._NULL;
      return this.tagIdRTC;
    }

    private int PushElement(HtmlTagIndex tagIndex, bool useInputTag, HtmlDtd.TagDefinition tagDef)
    {
      if (this.ensureHead)
      {
        if (tagIndex == HtmlTagIndex.Body)
          this.PopElement(this.PushElement(HtmlTagIndex.Head, false, HtmlDtd.tags[52]), false);
        else if (tagIndex == HtmlTagIndex.Head)
          this.ensureHead = false;
      }
      if (tagIndex == HtmlTagIndex.Listing)
      {
        tagIndex = HtmlTagIndex.Pre;
        tagDef = HtmlDtd.tags[84];
        useInputTag = false;
      }
      if (tagDef.TextScope != HtmlDtd.TagTextScope.NEUTRAL)
        this.validRTC = false;
      if (this.elementStackTop == this.elementStack.Length && !this.EnsureElementStackSpace())
        throw new TextConvertersException(CtsResources.TextConvertersStrings.HtmlNestingTooDeep);
      bool flag = tagDef.Scope == HtmlDtd.TagScope.EMPTY;
      if (useInputTag)
      {
        if (this.inputToken.TagIndex != HtmlTagIndex.Unknown)
          this.inputToken.Flags = flag ? this.inputToken.Flags | HtmlToken.TagFlags.EmptyScope : this.inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope;
        else
          flag = true;
      }
      int index = this.elementStackTop++;
      this.elementStack[index] = tagIndex;
      this.LFillTagB(tagDef);
      this.EnqueueTail(useInputTag ? HtmlNormalizingParser.QueueItemKind.PassThrough : HtmlNormalizingParser.QueueItemKind.BeginElement, tagIndex, this.allowWspLeft, this.allowWspRight);
      if (useInputTag && !this.inputToken.IsTagEnd)
        this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
      this.RFillTagB(tagDef);
      if (!flag)
      {
        if (tagDef.ContextType != HtmlDtd.ContextType.None)
        {
          if (this.contextStackTop == this.contextStack.Length)
            this.EnsureContextStackSpace();
          this.contextStack[this.contextStackTop++] = this.context;
          this.context.TopElement = index;
          this.context.Type = tagDef.ContextType;
          this.context.TextType = tagDef.ContextTextType;
          this.context.Accept = tagDef.Accept;
          this.context.Reject = tagDef.Reject;
          this.context.IgnoreEnd = tagDef.IgnoreEnd;
          this.context.HasSpace = false;
          this.context.EatSpace = false;
          this.context.OneNL = false;
          this.context.LastCh = char.MinValue;
          if (this.context.TextType != HtmlDtd.ContextTextType.Full)
          {
            this.allowWspLeft = false;
            this.allowWspRight = false;
          }
          this.RFillTagB(tagDef);
        }
      }
      else
        --this.elementStackTop;
      return index;
    }

    private void PopElement(int stackPos, bool useInputTag)
    {
      HtmlTagIndex tagIndex = this.elementStack[stackPos];
      HtmlDtd.TagDefinition tagDefinition = HtmlNormalizingParser.GetTagDefinition(tagIndex);
      if (tagDefinition.TextScope != HtmlDtd.TagTextScope.NEUTRAL)
        this.validRTC = false;
      if (stackPos == this.context.TopElement)
      {
        if (this.context.TextType == HtmlDtd.ContextTextType.Full)
          this.LFillTagE(tagDefinition);
        this.context = this.contextStack[--this.contextStackTop];
      }
      this.LFillTagE(tagDefinition);
      if (stackPos != this.elementStackTop - 1)
        this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.OverlappedClose, this.elementStackTop - stackPos - 1);
      this.EnqueueTail(useInputTag ? HtmlNormalizingParser.QueueItemKind.PassThrough : HtmlNormalizingParser.QueueItemKind.EndElement, tagIndex, this.allowWspLeft, this.allowWspRight);
      if (useInputTag && !this.inputToken.IsTagEnd)
        this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
      this.RFillTagE(tagDefinition);
      if (stackPos != this.elementStackTop - 1)
      {
        this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.OverlappedReopen, this.elementStackTop - stackPos - 1);
        Array.Copy((Array) this.elementStack, stackPos + 1, (Array) this.elementStack, stackPos, this.elementStackTop - stackPos - 1);
        if (this.context.TopElement > stackPos)
        {
          --this.context.TopElement;
          for (int index = this.contextStackTop - 1; index > 0 && this.contextStack[index].TopElement >= stackPos; --index)
            --this.contextStack[index].TopElement;
        }
      }
      --this.elementStackTop;
    }

    private void AddNonspace(char firstChar, char lastChar)
    {
      if (this.context.HasSpace)
      {
        this.context.HasSpace = false;
        if ((int) this.context.LastCh == 0 || !this.context.OneNL || !ParseSupport.TwoFarEastNonHanguelChars(this.context.LastCh, firstChar))
          this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Space);
      }
      this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Text);
      this.context.EatSpace = false;
      this.context.LastCh = lastChar;
      this.context.OneNL = false;
    }

    private void AddSpace(bool oneNL)
    {
      if (!this.context.EatSpace)
        this.context.HasSpace = true;
      if ((int) this.context.LastCh == 0)
        return;
      if (oneNL && !this.context.OneNL)
        this.context.OneNL = true;
      else
        this.context.LastCh = char.MinValue;
    }

    private bool QueryTextlike(HtmlTagIndex tagIndex)
    {
      HtmlDtd.ContextType contextType = this.context.Type;
      for (int index = this.contextStackTop; index != 0; contextType = this.contextStack[--index].Type)
      {
        switch (contextType)
        {
          case HtmlDtd.ContextType.Head:
            if (tagIndex == HtmlTagIndex.Object)
              return false;
            break;
          case HtmlDtd.ContextType.Body:
            HtmlTagIndex htmlTagIndex = tagIndex;
            if ((uint) htmlTagIndex <= 36U)
            {
              if (htmlTagIndex != HtmlTagIndex.A && htmlTagIndex != HtmlTagIndex.Applet && htmlTagIndex != HtmlTagIndex.Div)
                goto label_8;
            }
            else if (htmlTagIndex != HtmlTagIndex.Input && htmlTagIndex != HtmlTagIndex.Object && htmlTagIndex != HtmlTagIndex.Span)
              goto label_8;
            return true;
label_8:
            return false;
        }
      }
      return tagIndex == HtmlTagIndex.Object || tagIndex == HtmlTagIndex.Applet;
    }

    private void LFillTagB(HtmlDtd.TagDefinition tagDef)
    {
      if (this.context.TextType != HtmlDtd.ContextTextType.Full)
        return;
      this.LFill(this.FillCodeFromTag(tagDef).LB, this.FillCodeFromTag(tagDef).RB);
    }

    private void RFillTagB(HtmlDtd.TagDefinition tagDef)
    {
      if (this.context.TextType != HtmlDtd.ContextTextType.Full)
        return;
      this.RFill(this.FillCodeFromTag(tagDef).RB);
    }

    private void LFillTagE(HtmlDtd.TagDefinition tagDef)
    {
      if (this.context.TextType != HtmlDtd.ContextTextType.Full)
        return;
      this.LFill(this.FillCodeFromTag(tagDef).LE, this.FillCodeFromTag(tagDef).RE);
    }

    private void RFillTagE(HtmlDtd.TagDefinition tagDef)
    {
      if (this.context.TextType != HtmlDtd.ContextTextType.Full)
        return;
      this.RFill(this.FillCodeFromTag(tagDef).RE);
    }

    private void LFill(HtmlDtd.FillCode codeLeft, HtmlDtd.FillCode codeRight)
    {
      this.allowWspLeft = this.context.HasSpace || codeLeft == HtmlDtd.FillCode.EAT;
      this.context.LastCh = char.MinValue;
      if (this.context.HasSpace)
      {
        if (codeLeft == HtmlDtd.FillCode.PUT)
        {
          this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Space);
          this.context.EatSpace = true;
        }
        this.context.HasSpace = codeLeft == HtmlDtd.FillCode.NUL;
      }
      this.allowWspRight = this.context.HasSpace || codeRight == HtmlDtd.FillCode.EAT;
    }

    private void RFill(HtmlDtd.FillCode code)
    {
      if (code == HtmlDtd.FillCode.EAT)
      {
        this.context.HasSpace = false;
        this.context.EatSpace = true;
      }
      else
      {
        if (code != HtmlDtd.FillCode.PUT)
          return;
        this.context.EatSpace = false;
      }
    }

    private bool QueueEmpty()
    {
      if (this.queueHead != this.queueTail)
        return this.queue[this.queueHead].Kind == HtmlNormalizingParser.QueueItemKind.Suspend;
      return true;
    }

    private HtmlNormalizingParser.QueueItemKind QueueHeadKind()
    {
      if (this.queueHead == this.queueTail)
        return HtmlNormalizingParser.QueueItemKind.Empty;
      return this.queue[this.queueHead].Kind;
    }

    private void EnqueueTail(HtmlNormalizingParser.QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
    {
      if (this.queueTail == this.queue.Length)
        this.ExpandQueue();
      this.queue[this.queueTail].Kind = kind;
      this.queue[this.queueTail].TagIndex = tagIndex;
      this.queue[this.queueTail].Flags = (HtmlNormalizingParser.QueueItemFlags) ((allowWspLeft ? 1 : 0) | (allowWspRight ? 2 : 0));
      this.queue[this.queueTail].Argument = 0;
      ++this.queueTail;
    }

    private void EnqueueTail(HtmlNormalizingParser.QueueItemKind kind, int argument)
    {
      if (this.queueTail == this.queue.Length)
        this.ExpandQueue();
      this.queue[this.queueTail].Kind = kind;
      this.queue[this.queueTail].TagIndex = HtmlTagIndex._NULL;
      this.queue[this.queueTail].Flags = (HtmlNormalizingParser.QueueItemFlags) 0;
      this.queue[this.queueTail].Argument = argument;
      ++this.queueTail;
    }

    private void EnqueueTail(HtmlNormalizingParser.QueueItemKind kind)
    {
      if (this.queueTail == this.queue.Length)
        this.ExpandQueue();
      this.queue[this.queueTail].Kind = kind;
      this.queue[this.queueTail].TagIndex = HtmlTagIndex._NULL;
      this.queue[this.queueTail].Flags = (HtmlNormalizingParser.QueueItemFlags) 0;
      this.queue[this.queueTail].Argument = 0;
      ++this.queueTail;
    }

    private void EnqueueHead(HtmlNormalizingParser.QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
    {
      if (this.queueHead != this.queueStart)
        --this.queueHead;
      else
        ++this.queueTail;
      this.queue[this.queueHead].Kind = kind;
      this.queue[this.queueHead].TagIndex = tagIndex;
      this.queue[this.queueHead].Flags = (HtmlNormalizingParser.QueueItemFlags) ((allowWspLeft ? 1 : 0) | (allowWspRight ? 2 : 0));
      this.queue[this.queueHead].Argument = 0;
    }

    private void EnqueueHead(HtmlNormalizingParser.QueueItemKind kind)
    {
      this.EnqueueHead(kind, 0);
    }

    private void EnqueueHead(HtmlNormalizingParser.QueueItemKind kind, int argument)
    {
      if (this.queueHead != this.queueStart)
        --this.queueHead;
      else
        ++this.queueTail;
      this.queue[this.queueHead].Kind = kind;
      this.queue[this.queueHead].TagIndex = HtmlTagIndex._NULL;
      this.queue[this.queueHead].Flags = (HtmlNormalizingParser.QueueItemFlags) 0;
      this.queue[this.queueHead].Argument = argument;
    }

    private HtmlTokenId GetTokenFromQueue()
    {
      HtmlNormalizingParser.QueueItem queueItem1;
      switch (this.QueueHeadKind())
      {
        case HtmlNormalizingParser.QueueItemKind.None:
          this.DoDequeueFirst();
          this.token = (HtmlToken) null;
          return HtmlTokenId.None;
        case HtmlNormalizingParser.QueueItemKind.Eof:
          this.tokenBuilder.BuildEofToken();
          this.token = (HtmlToken) this.tokenBuilder;
          break;
        case HtmlNormalizingParser.QueueItemKind.BeginElement:
        case HtmlNormalizingParser.QueueItemKind.EndElement:
          HtmlNormalizingParser.QueueItem queueItem2 = this.DoDequeueFirst();
          this.tokenBuilder.BuildTagToken(queueItem2.TagIndex, queueItem2.Kind == HtmlNormalizingParser.QueueItemKind.EndElement, (queueItem2.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft) == HtmlNormalizingParser.QueueItemFlags.AllowWspLeft, (queueItem2.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight) == HtmlNormalizingParser.QueueItemFlags.AllowWspRight, false);
          this.token = (HtmlToken) this.tokenBuilder;
          if (queueItem2.Kind == HtmlNormalizingParser.QueueItemKind.BeginElement && this.token.OriginalTagId == HtmlTagIndex.Body && (this.injection != null && this.injection.HaveHead) && !this.injection.HeadDone)
          {
            int container = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
            this.parser = (HtmlParser) this.injection.Push(true, (IHtmlParser) this.parser);
            this.saveState.Save(this, container + 1);
            this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 1);
            if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
            {
              this.OpenContainer(HtmlTagIndex.TT);
              this.OpenContainer(HtmlTagIndex.Pre);
            }
          }
          return this.token.HtmlTokenId;
        case HtmlNormalizingParser.QueueItemKind.OverlappedClose:
        case HtmlNormalizingParser.QueueItemKind.OverlappedReopen:
          HtmlNormalizingParser.QueueItem queueItem3 = this.DoDequeueFirst();
          this.tokenBuilder.BuildOverlappedToken(queueItem3.Kind == HtmlNormalizingParser.QueueItemKind.OverlappedClose, queueItem3.Argument);
          this.token = (HtmlToken) this.tokenBuilder;
          return this.token.HtmlTokenId;
        case HtmlNormalizingParser.QueueItemKind.PassThrough:
          HtmlNormalizingParser.QueueItem queueItem4 = this.DoDequeueFirst();
          this.token = this.inputToken;
          if (this.token.HtmlTokenId == HtmlTokenId.Tag)
          {
            this.token.Flags |= (HtmlToken.TagFlags) (((queueItem4.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft) == HtmlNormalizingParser.QueueItemFlags.AllowWspLeft ? 64 : 0) | ((queueItem4.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight) == HtmlNormalizingParser.QueueItemFlags.AllowWspRight ? 128 : 0));
            if (this.token.OriginalTagId == HtmlTagIndex.Body && this.token.IsTagEnd && (this.injection != null && this.injection.HaveHead) && !this.injection.HeadDone)
            {
              int container = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
              this.parser = (HtmlParser) this.injection.Push(true, (IHtmlParser) this.parser);
              this.saveState.Save(this, container + 1);
              this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 1);
              if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
              {
                this.OpenContainer(HtmlTagIndex.TT);
                this.OpenContainer(HtmlTagIndex.Pre);
              }
            }
          }
          return this.token.HtmlTokenId;
        case HtmlNormalizingParser.QueueItemKind.Space:
          queueItem1 = this.DoDequeueFirst();
          this.tokenBuilder.BuildSpaceToken();
          this.token = (HtmlToken) this.tokenBuilder;
          return this.token.HtmlTokenId;
        case HtmlNormalizingParser.QueueItemKind.Text:
          bool flag = false;
          int num1 = 0;
          queueItem1 = this.DoDequeueFirst();
          if (this.queueHead != this.queueTail)
          {
            flag = true;
            num1 = this.queue[this.queueHead].Argument;
            this.DoDequeueFirst();
          }
          this.tokenBuilder.BuildTextSliceToken((Token) this.inputToken, this.currentRun, this.currentRunOffset, this.numRuns);
          this.token = (HtmlToken) this.tokenBuilder;
          Token.RunEnumerator runs = this.inputToken.Runs;
          if (runs.IsValidPosition)
          {
            int num2 = 0;
            do
            {
              num2 += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
            }
            while (runs.MoveNext(true) && runs.Current.TextType <= RunTextType.UnusualWhitespace);
            if (num2 != 0)
              this.AddSpace(num2 == 1);
            this.currentRun = runs.CurrentIndex;
            this.currentRunOffset = runs.CurrentOffset;
            if (runs.IsValidPosition)
            {
              char firstChar = runs.Current.FirstChar;
              char lastChar;
              do
              {
                lastChar = runs.Current.LastChar;
              }
              while (runs.MoveNext(true) && runs.Current.TextType > RunTextType.UnusualWhitespace);
              this.AddNonspace(firstChar, lastChar);
            }
            this.numRuns = runs.CurrentIndex - this.currentRun;
          }
          else
          {
            this.currentRun = runs.CurrentIndex;
            this.currentRunOffset = runs.CurrentOffset;
            this.numRuns = 0;
          }
          if (flag)
            this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionEnd, num1);
          return this.token.HtmlTokenId;
        case HtmlNormalizingParser.QueueItemKind.InjectionBegin:
        case HtmlNormalizingParser.QueueItemKind.InjectionEnd:
          HtmlNormalizingParser.QueueItem queueItem5 = this.DoDequeueFirst();
          this.tokenBuilder.BuildInjectionToken(queueItem5.Kind == HtmlNormalizingParser.QueueItemKind.InjectionBegin, queueItem5.Argument != 0);
          this.token = (HtmlToken) this.tokenBuilder;
          break;
        case HtmlNormalizingParser.QueueItemKind.EndLastTag:
          HtmlNormalizingParser.QueueItem queueItem6 = this.DoDequeueFirst();
          this.tokenBuilder.BuildTagToken(queueItem6.TagIndex, false, (queueItem6.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft) == HtmlNormalizingParser.QueueItemFlags.AllowWspLeft, (queueItem6.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight) == HtmlNormalizingParser.QueueItemFlags.AllowWspRight, true);
          this.token = (HtmlToken) this.tokenBuilder;
          if (queueItem6.Kind == HtmlNormalizingParser.QueueItemKind.BeginElement && this.token.OriginalTagId == HtmlTagIndex.Body && (this.injection != null && this.injection.HaveHead) && !this.injection.HeadDone)
          {
            int container = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
            this.parser = (HtmlParser) this.injection.Push(true, (IHtmlParser) this.parser);
            this.saveState.Save(this, container + 1);
            this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 1);
            if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
            {
              this.OpenContainer(HtmlTagIndex.TT);
              this.OpenContainer(HtmlTagIndex.Pre);
            }
          }
          return this.token.HtmlTokenId;
      }
      return this.token.HtmlTokenId;
    }

    private void ExpandQueue()
    {
      HtmlNormalizingParser.QueueItem[] queueItemArray = new HtmlNormalizingParser.QueueItem[this.queue.Length * 2];
      Array.Copy((Array) this.queue, this.queueHead, (Array) queueItemArray, this.queueHead, this.queueTail - this.queueHead);
      if (this.queueStart != 0)
        Array.Copy((Array) this.queue, 0, (Array) queueItemArray, 0, this.queueStart);
      this.queue = queueItemArray;
    }

    private HtmlNormalizingParser.QueueItem DoDequeueFirst()
    {
      int index = this.queueHead;
      ++this.queueHead;
      if (this.queueHead == this.queueTail)
        this.queueHead = this.queueTail = this.queueStart;
      return this.queue[index];
    }

    private HtmlDtd.TagFill FillCodeFromTag(HtmlDtd.TagDefinition tagDef)
    {
      if (this.context.Type == HtmlDtd.ContextType.Select && tagDef.TagIndex != HtmlTagIndex.Option)
        return HtmlDtd.TagFill.PUT_PUT_PUT_PUT;
      if (this.context.Type == HtmlDtd.ContextType.Title)
        return HtmlDtd.TagFill.NUL_EAT_EAT_NUL;
      return tagDef.Fill;
    }

    private bool EnsureElementStackSpace()
    {
      if (this.elementStackTop == this.elementStack.Length)
      {
        if (this.elementStack.Length >= this.maxElementStack)
          return false;
        HtmlTagIndex[] htmlTagIndexArray = new HtmlTagIndex[this.maxElementStack / 2 > this.elementStack.Length ? this.elementStack.Length * 2 : this.maxElementStack];
        Array.Copy((Array) this.elementStack, 0, (Array) htmlTagIndexArray, 0, this.elementStackTop);
        this.elementStack = htmlTagIndexArray;
      }
      return true;
    }

    private void EnsureContextStackSpace()
    {
      if (this.contextStackTop + 1 <= this.contextStack.Length)
        return;
      HtmlNormalizingParser.Context[] contextArray = new HtmlNormalizingParser.Context[this.contextStack.Length * 2];
      Array.Copy((Array) this.contextStack, 0, (Array) contextArray, 0, this.contextStackTop);
      this.contextStack = contextArray;
    }

    private enum QueueItemKind : byte
    {
      Empty,
      None,
      Eof,
      BeginElement,
      EndElement,
      OverlappedClose,
      OverlappedReopen,
      PassThrough,
      Space,
      Text,
      Suspend,
      InjectionBegin,
      InjectionEnd,
      EndLastTag,
    }

    [Flags]
    private enum QueueItemFlags : byte
    {
      AllowWspLeft = (byte) 1,
      AllowWspRight = (byte) 2,
    }

    private struct Context
    {
      public int TopElement;
      public HtmlDtd.ContextType Type;
      public HtmlDtd.ContextTextType TextType;
      public HtmlDtd.SetId Accept;
      public HtmlDtd.SetId Reject;
      public HtmlDtd.SetId IgnoreEnd;
      public char LastCh;
      public bool OneNL;
      public bool HasSpace;
      public bool EatSpace;
    }

    private struct QueueItem
    {
      public HtmlNormalizingParser.QueueItemKind Kind;
      public HtmlTagIndex TagIndex;
      public HtmlNormalizingParser.QueueItemFlags Flags;
      public int Argument;
    }

    private class DocumentState
    {
      private HtmlTagIndex[] savedElementStackEntries = new HtmlTagIndex[5];
      private int queueHead;
      private int queueTail;
      private HtmlToken inputToken;
      private int elementStackTop;
      private int currentRun;
      private int currentRunOffset;
      private int numRuns;
      private int savedElementStackEntriesCount;
      private bool hasSpace;
      private bool eatSpace;
      private bool validRTC;
      private HtmlTagIndex tagIdRTC;

      public int SavedStackTop
      {
        get
        {
          return this.elementStackTop;
        }
      }

      public void Save(HtmlNormalizingParser document, int stackLevel)
      {
        if (stackLevel != document.elementStackTop)
        {
          Array.Copy((Array) document.elementStack, stackLevel, (Array) this.savedElementStackEntries, 0, document.elementStackTop - stackLevel);
          this.savedElementStackEntriesCount = document.elementStackTop - stackLevel;
          document.elementStackTop = stackLevel;
        }
        else
          this.savedElementStackEntriesCount = 0;
        this.elementStackTop = document.elementStackTop;
        this.queueHead = document.queueHead;
        this.queueTail = document.queueTail;
        this.inputToken = document.inputToken;
        this.currentRun = document.currentRun;
        this.currentRunOffset = document.currentRunOffset;
        this.numRuns = document.numRuns;
        this.hasSpace = document.context.HasSpace;
        this.eatSpace = document.context.EatSpace;
        this.validRTC = document.validRTC;
        this.tagIdRTC = document.tagIdRTC;
        document.queueStart = document.queueTail;
        document.queueHead = document.queueTail = document.queueStart;
      }

      public void Restore(HtmlNormalizingParser document)
      {
        if (this.savedElementStackEntriesCount != 0)
        {
          Array.Copy((Array) this.savedElementStackEntries, 0, (Array) document.elementStack, document.elementStackTop, this.savedElementStackEntriesCount);
          document.elementStackTop += this.savedElementStackEntriesCount;
        }
        document.queueStart = 0;
        document.queueHead = this.queueHead;
        document.queueTail = this.queueTail;
        document.inputToken = this.inputToken;
        document.currentRun = this.currentRun;
        document.currentRunOffset = this.currentRunOffset;
        document.numRuns = this.numRuns;
        document.context.HasSpace = this.hasSpace;
        document.context.EatSpace = this.eatSpace;
        document.validRTC = this.validRTC;
        document.tagIdRTC = this.tagIdRTC;
      }
    }

    private class SmallTokenBuilder : HtmlToken
    {
      private char[] spareBuffer = new char[1];
      private Token.RunEntry[] spareRuns = new Token.RunEntry[1];

      public void BuildTagToken(HtmlTagIndex tagIndex, bool closingTag, bool allowWspLeft, bool allowWspRight, bool endOnly)
      {
        this.TokenId = (TokenId) 4;
        this.Argument = 1;
        this.Buffer = this.spareBuffer;
        this.RunList = this.spareRuns;
        this.Whole.Reset();
        this.WholePosition.Rewind(this.Whole);
        this.TagIndex = this.OriginalTagIndex = tagIndex;
        this.NameIndex = HtmlDtd.tags[(int) tagIndex].NameIndex;
        if (!endOnly)
        {
          this.PartMajor = HtmlToken.TagPartMajor.Complete;
          this.PartMinor = HtmlToken.TagPartMinor.CompleteName;
        }
        else
        {
          this.PartMajor = HtmlToken.TagPartMajor.End;
          this.PartMinor = HtmlToken.TagPartMinor.Empty;
        }
        this.Flags = (HtmlToken.TagFlags) ((int) (byte) ((closingTag ? 16 : 0) | (allowWspLeft ? 64 : 0)) | (allowWspRight ? 128 : 0));
      }

      public void BuildOverlappedToken(bool close, int argument)
      {
        this.TokenId = close ? (TokenId) 6 : (TokenId) 7;
        this.Argument = argument;
        this.Buffer = this.spareBuffer;
        this.RunList = this.spareRuns;
        this.Whole.Reset();
        this.WholePosition.Rewind(this.Whole);
      }

      public void BuildInjectionToken(bool begin, bool head)
      {
        this.TokenId = begin ? (TokenId) 8 : (TokenId) 9;
        this.Argument = head ? 1 : 0;
        this.Buffer = this.spareBuffer;
        this.RunList = this.spareRuns;
        this.Whole.Reset();
        this.WholePosition.Rewind(this.Whole);
      }

      public void BuildSpaceToken()
      {
        this.TokenId = TokenId.Text;
        this.Argument = 1;
        this.Buffer = this.spareBuffer;
        this.RunList = this.spareRuns;
        this.Buffer[0] = ' ';
        this.RunList[0].Initialize(RunType.Normal, RunTextType.Space, 67108864U, 1, 0);
        this.Whole.Reset();
        this.Whole.Tail = 1;
        this.WholePosition.Rewind(this.Whole);
      }

      public void BuildTextSliceToken(Token source, int startRun, int startRunOffset, int numRuns)
      {
        this.TokenId = TokenId.Text;
        this.Argument = 0;
        this.Buffer = source.Buffer;
        this.RunList = source.RunList;
        this.Whole.Initialize(startRun, startRunOffset);
        this.Whole.Tail = this.Whole.Head + numRuns;
        this.WholePosition.Rewind(this.Whole);
      }

      public void BuildEofToken()
      {
        this.TokenId = TokenId.EndOfFile;
        this.Argument = 0;
        this.Buffer = this.spareBuffer;
        this.RunList = this.spareRuns;
        this.Whole.Reset();
        this.WholePosition.Rewind(this.Whole);
      }
    }
  }
}
