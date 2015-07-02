// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.FormatConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal abstract class FormatConverter : IProgressMonitor
  {
    internal FormatStore Store;
    internal FormatConverter.BuildStackEntry[] BuildStack;
    internal int BuildStackTop;
    internal int LastNodeInternal;
    internal bool EmptyContainer;
    internal bool ContainerFlushed;
    internal StyleBuildHelper ContainerStyleBuildHelper;
    internal StyleBuildHelper StyleBuildHelper;
    internal bool StyleBuildHelperLocked;
    internal MultiValueBuildHelper MultiValueBuildHelper;
    internal bool MultiValueBuildHelperLocked;
    internal Dictionary<string, PropertyValue> FontFaceDictionary;
    protected bool madeProgress;
    private bool mustFlush;
    private bool endOfFile;
    private bool newLine;
    private bool textQuotingExpected;

    public FormatConverterContainer Root => new FormatConverterContainer(this, 0);

      public FormatConverterContainer Last => new FormatConverterContainer(this, this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);

      public FormatNode LastNode => new FormatNode(this.Store, this.LastNodeInternal);

      public FormatConverterContainer LastNonEmpty => new FormatConverterContainer(this, this.BuildStackTop - 1);

      public bool EndOfFile => this.endOfFile;

      protected bool MustFlush
    {
      get
      {
        return this.mustFlush;
      }
      set
      {
        this.mustFlush = value;
      }
    }

    internal FormatConverter(Stream formatConverterTraceStream)
    {
      this.Store = new FormatStore();
      this.BuildStack = new FormatConverter.BuildStackEntry[16];
      this.ContainerStyleBuildHelper = new StyleBuildHelper(this.Store);
      this.StyleBuildHelper = new StyleBuildHelper(this.Store);
      this.MultiValueBuildHelper = new MultiValueBuildHelper(this.Store);
      this.FontFaceDictionary = new Dictionary<string, PropertyValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    internal FormatConverter(FormatStore formatStore, Stream formatConverterTraceStream)
    {
      this.Store = formatStore;
      this.BuildStack = new FormatConverter.BuildStackEntry[16];
      this.ContainerStyleBuildHelper = new StyleBuildHelper(this.Store);
      this.StyleBuildHelper = new StyleBuildHelper(this.Store);
      this.MultiValueBuildHelper = new MultiValueBuildHelper(this.Store);
      this.FontFaceDictionary = new Dictionary<string, PropertyValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public abstract void Run();

    public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty)
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      return new FormatConverterContainer(this, this.PushContainer(nodeType, empty, 1));
    }

    public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, Html.HtmlNameIndex tagName)
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      int level = this.PushContainer(nodeType, empty, inheritanceMaskIndex);
      if (!baseStyle.IsNull)
      {
        baseStyle.AddRef();
        this.ContainerStyleBuildHelper.AddStyle(10, baseStyle.Handle);
      }
      this.BuildStack[level].TagName = tagName;
      return new FormatConverterContainer(this, level);
    }

    public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, Html.HtmlTagIndex tagIndex)
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      int level = this.PushContainer(nodeType, empty, inheritanceMaskIndex);
      if (!baseStyle.IsNull)
      {
        baseStyle.AddRef();
        this.ContainerStyleBuildHelper.AddStyle(10, baseStyle.Handle);
      }
      this.BuildStack[level].TagIndex = tagIndex;
      return new FormatConverterContainer(this, level);
    }

    public void OpenTextContainer()
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      this.PrepareToAddText();
    }

    public void CloseContainer()
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      this.PopContainer();
    }

    public void CloseOverlappingContainer(int countLevelsToKeepOpen)
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      this.PopContainer(this.BuildStackTop - 1 - countLevelsToKeepOpen);
    }

    public void CloseAllContainersAndSetEOF()
    {
      while (this.BuildStackTop > 1)
        this.CloseContainer();
      this.Store.GetNode(this.BuildStack[0].Node).PrepareToClose(this.Store.CurrentTextPosition);
      this.mustFlush = true;
      this.endOfFile = true;
    }

    public void AddNonSpaceText(char[] buffer, int offset, int count)
    {
      this.PrepareToAddText();
      if (!this.ContainerFlushed)
        this.FlushContainer(this.BuildStackTop);
      this.newLine = false;
      if (this.textQuotingExpected)
      {
        if ((int) buffer[offset] == 62)
        {
          do
          {
            this.Store.AddText(buffer, offset, 1);
            ++offset;
            --count;
          }
          while (count != 0 && (int) buffer[offset] == 62);
          if (count == 0)
            return;
        }
        this.Store.SetTextBoundary();
        this.textQuotingExpected = false;
      }
      this.Store.AddText(buffer, offset, count);
    }

    public void AddSpace(int count)
    {
      this.PrepareToAddText();
      if (!this.ContainerFlushed)
        this.FlushContainer(this.BuildStackTop);
      this.Store.AddSpace(count);
      this.newLine = false;
    }

    public void AddLineBreak(int count)
    {
      this.PrepareToAddText();
      if (!this.ContainerFlushed)
        this.FlushContainer(this.BuildStackTop);
      if (!this.newLine)
      {
        this.Store.AddLineBreak(1);
        this.Store.SetTextBoundary();
        if (count > 1)
          this.Store.AddLineBreak(count - 1);
        this.newLine = true;
        this.textQuotingExpected = true;
      }
      else
        this.Store.AddLineBreak(count);
    }

    public void AddNbsp(int count)
    {
      this.PrepareToAddText();
      if (!this.ContainerFlushed)
        this.FlushContainer(this.BuildStackTop);
      this.Store.AddNbsp(count);
      this.newLine = false;
    }

    public void AddTabulation(int count)
    {
      this.PrepareToAddText();
      if (!this.ContainerFlushed)
        this.FlushContainer(this.BuildStackTop);
      this.Store.AddTabulation(count);
      this.newLine = false;
      this.textQuotingExpected = false;
    }

    public StringValue RegisterStringValue(bool isStatic, string value)
    {
      return this.Store.AllocateStringValue(isStatic, value);
    }

    public StringValue RegisterStringValue(bool isStatic, string str, int offset, int count)
    {
      string str1 = str;
      if (offset != 0 || count != str.Length)
        str1 = str.Substring(offset, count);
      return this.Store.AllocateStringValue(isStatic, str1);
    }

    public StringValue RegisterStringValue(bool isStatic, BufferString value)
    {
      return this.Store.AllocateStringValue(isStatic, value.ToString());
    }

    public PropertyValue RegisterFaceName(bool isStatic, BufferString value)
    {
      if (value.Length == 0)
        return PropertyValue.Null;
      return this.RegisterFaceName(isStatic, value.ToString());
    }

    public PropertyValue RegisterFaceName(bool isStatic, string faceName)
    {
      if (string.IsNullOrEmpty(faceName))
        return PropertyValue.Null;
      PropertyValue propertyValue1;
      if (this.FontFaceDictionary.TryGetValue(faceName, out propertyValue1))
      {
        if (propertyValue1.IsString)
          this.Store.AddRefValue(propertyValue1);
        return propertyValue1;
      }
      StringValue stringValue = this.RegisterStringValue(isStatic, faceName);
      PropertyValue propertyValue2 = stringValue.PropertyValue;
      if (this.FontFaceDictionary.Count < 100)
      {
        stringValue.AddRef();
        this.FontFaceDictionary.Add(faceName, propertyValue2);
      }
      return propertyValue2;
    }

    public MultiValue RegisterMultiValue(bool isStatic, out MultiValueBuilder builder)
    {
      MultiValue multiValue = this.Store.AllocateMultiValue(isStatic);
      builder = new MultiValueBuilder(this, multiValue.Handle);
      return multiValue;
    }

    public FormatStyle RegisterStyle(bool isStatic, out StyleBuilder builder)
    {
      FormatStyle formatStyle = this.Store.AllocateStyle(isStatic);
      builder = new StyleBuilder(this, formatStyle.Handle);
      return formatStyle;
    }

    public FormatStyle GetStyle(int styleHandle)
    {
      return this.Store.GetStyle(styleHandle);
    }

    public StringValue GetStringValue(PropertyValue pv)
    {
      return this.Store.GetStringValue(pv);
    }

    public MultiValue GetMultiValue(PropertyValue pv)
    {
      return this.Store.GetMultiValue(pv);
    }

    public void ReleasePropertyValue(PropertyValue pv)
    {
      this.Store.ReleaseValue(pv);
    }

    void IProgressMonitor.ReportProgress()
    {
      this.madeProgress = true;
    }

    internal FormatNode InitializeDocument()
    {
      this.Initialize();
      return this.OpenContainer(FormatContainerType.Document, false).Node;
    }

    internal FormatNode InitializeFragment()
    {
      this.Initialize();
      FormatConverterContainer converterContainer = this.OpenContainer(FormatContainerType.Fragment, false);
      this.OpenContainer(FormatContainerType.PropertyContainer, false);
      this.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, this.RegisterFaceName(false, "Times New Roman"));
      this.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 11));
      return converterContainer.Node;
    }

    protected void CloseContainer(FormatContainerType containerType)
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      for (int level = this.BuildStackTop - 1; level > 0; --level)
      {
        if (this.BuildStack[level].Type == containerType)
        {
          this.PopContainer(level);
          break;
        }
      }
    }

    protected void CloseContainer(Html.HtmlNameIndex tagName)
    {
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      for (int level = this.BuildStackTop - 1; level > 0; --level)
      {
        if (this.BuildStack[level].TagName == tagName)
        {
          this.PopContainer(level);
          break;
        }
      }
    }

    protected FormatNode CreateNode(FormatContainerType type)
    {
      FormatNode formatNode = this.Store.AllocateNode(type);
      formatNode.EndTextPosition = formatNode.BeginTextPosition;
      formatNode.SetOutOfOrder();
      return formatNode;
    }

    protected virtual FormatContainerType FixContainerType(FormatContainerType type, StyleBuildHelper styleBuilderWithContainerProperties)
    {
      return type;
    }

    protected virtual FormatNode GetParentForNewNode(FormatNode node, FormatNode defaultParent, int stackPos, out int propContainerInheritanceStopLevel)
    {
      propContainerInheritanceStopLevel = this.DefaultPropContainerInheritanceStopLevel(stackPos);
      return defaultParent;
    }

    protected int DefaultPropContainerInheritanceStopLevel(int stackPos)
    {
      int index = stackPos - 1;
      while (index >= 0 && this.BuildStack[index].Node == 0)
        --index;
      return index + 1;
    }

    private static string Indent(int level)
    {
      return "                                                  ".Substring(0, Math.Min("                                                  ".Length, level * 2));
    }

    private void Initialize()
    {
      this.BuildStackTop = 0;
      this.ContainerStyleBuildHelper.Clean();
      this.StyleBuildHelper.Clean();
      this.StyleBuildHelperLocked = false;
      this.MultiValueBuildHelper.Cancel();
      this.MultiValueBuildHelperLocked = false;
      this.FontFaceDictionary.Clear();
      this.LastNodeInternal = this.Store.RootNode.Handle;
      this.BuildStack[this.BuildStackTop].Type = FormatContainerType.Root;
      this.BuildStack[this.BuildStackTop].Node = this.Store.RootNode.Handle;
      ++this.BuildStackTop;
      this.EmptyContainer = false;
      this.ContainerFlushed = true;
      this.mustFlush = false;
      this.endOfFile = false;
      this.newLine = true;
      this.textQuotingExpected = true;
    }

    public void AddMarkupText(char[] buffer, int offset, int count)
    {
      this.Store.AddMarkupText(buffer, offset, count);
    }

    private void PrepareToAddText()
    {
      if (this.EmptyContainer && this.BuildStack[this.BuildStackTop].IsText)
        return;
      if (!this.ContainerFlushed)
        this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : this.BuildStackTop - 1);
      if (this.EmptyContainer)
        this.PrepareToCloseContainer(this.BuildStackTop);
      this.PushContainer(FormatContainerType.Text, true, 5);
    }

    private void FlushContainer(int stackPos)
    {
      FormatContainerType formatContainerType = this.FixContainerType(this.BuildStack[stackPos].Type, this.ContainerStyleBuildHelper);
      if (formatContainerType != this.BuildStack[stackPos].Type)
        this.BuildStack[stackPos].Type = formatContainerType;
      this.ContainerStyleBuildHelper.GetPropertyList(out this.BuildStack[stackPos].Properties, out this.BuildStack[stackPos].FlagProperties, out this.BuildStack[stackPos].PropertyMask);
      if (!this.BuildStack[stackPos].IsPropertyContainerOrNull)
      {
        if (!this.newLine && (this.BuildStack[stackPos].Type & FormatContainerType.BlockFlag) != FormatContainerType.Null)
        {
          this.Store.AddBlockBoundary();
          this.newLine = true;
          this.textQuotingExpected = true;
        }
        FormatNode formatNode = formatContainerType != FormatContainerType.Document ? this.Store.AllocateNode(this.BuildStack[stackPos].Type) : this.Store.AllocateNode(this.BuildStack[stackPos].Type, 0U);
        formatNode.SetOnRightEdge();
        if ((this.BuildStack[stackPos].Type & FormatContainerType.InlineObjectFlag) != FormatContainerType.Null)
          this.Store.AddInlineObject();
        FormatNode node = this.Store.GetNode(this.LastNodeInternal);
        int propContainerInheritanceStopLevel;
        this.GetParentForNewNode(formatNode, node, stackPos, out propContainerInheritanceStopLevel).AppendChild(formatNode);
        this.BuildStack[stackPos].Node = formatNode.Handle;
        this.LastNodeInternal = formatNode.Handle;
        FlagProperties effectiveFlagProperties;
        Property[] propertyList;
        PropertyBitMask effectivePropertyMask;
        if (propContainerInheritanceStopLevel < stackPos)
        {
          FlagProperties flagProperties = FlagProperties.AllOn;
          PropertyBitMask propertyMask = PropertyBitMask.AllOn;
          for (int index = stackPos; index >= propContainerInheritanceStopLevel && (!flagProperties.IsClear || !propertyMask.IsClear); --index)
          {
            if (index == stackPos || this.BuildStack[index].Type == FormatContainerType.PropertyContainer)
            {
              effectiveFlagProperties = this.BuildStack[index].FlagProperties & flagProperties;
              this.ContainerStyleBuildHelper.AddProperties(11, effectiveFlagProperties, propertyMask, this.BuildStack[index].Properties);
              flagProperties &= ~this.BuildStack[index].FlagProperties;
              propertyMask &= ~this.BuildStack[index].PropertyMask;
              flagProperties &= FormatStoreData.GlobalInheritanceMasks[this.BuildStack[index].InheritanceMaskIndex].FlagProperties;
              propertyMask &= FormatStoreData.GlobalInheritanceMasks[this.BuildStack[index].InheritanceMaskIndex].PropertyMask;
            }
          }
          this.ContainerStyleBuildHelper.GetPropertyList(out propertyList, out effectiveFlagProperties, out effectivePropertyMask);
        }
        else
        {
          effectiveFlagProperties = this.BuildStack[stackPos].FlagProperties;
          effectivePropertyMask = this.BuildStack[stackPos].PropertyMask;
          propertyList = this.BuildStack[stackPos].Properties;
          if (propertyList != null)
          {
            for (int index = 0; index < propertyList.Length; ++index)
            {
              if (propertyList[index].Value.IsRefCountedHandle)
                this.Store.AddRefValue(propertyList[index].Value);
            }
          }
        }
        formatNode.SetProps(effectiveFlagProperties, effectivePropertyMask, propertyList, this.BuildStack[stackPos].InheritanceMaskIndex);
      }
      this.ContainerStyleBuildHelper.Clean();
      this.ContainerFlushed = true;
    }

    private int PushContainer(FormatContainerType type, bool empty, int inheritanceMaskIndex)
    {
      int index = this.BuildStackTop;
      if (index == this.BuildStack.Length)
      {
        if (this.BuildStack.Length >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        FormatConverter.BuildStackEntry[] buildStackEntryArray = new FormatConverter.BuildStackEntry[2048 > this.BuildStack.Length ? this.BuildStack.Length * 2 : 4096];
        Array.Copy((Array) this.BuildStack, 0, (Array) buildStackEntryArray, 0, this.BuildStackTop);
        this.BuildStack = buildStackEntryArray;
      }
      this.Store.SetTextBoundary();
      this.BuildStack[index].Type = type;
      this.BuildStack[index].TagName = Html.HtmlNameIndex._NOTANAME;
      this.BuildStack[index].InheritanceMaskIndex = inheritanceMaskIndex;
      this.BuildStack[index].TagIndex = Html.HtmlTagIndex._NULL;
      this.BuildStack[index].FlagProperties.ClearAll();
      this.BuildStack[index].PropertyMask.ClearAll();
      this.BuildStack[index].Properties = (Property[]) null;
      this.BuildStack[index].Node = 0;
      if (!empty)
        ++this.BuildStackTop;
      this.EmptyContainer = empty;
      this.ContainerFlushed = false;
      return index;
    }

    private void PopContainer()
    {
      this.PrepareToCloseContainer(this.BuildStackTop - 1);
      --this.BuildStackTop;
    }

    private void PopContainer(int level)
    {
      this.PrepareToCloseContainer(level);
      Array.Copy((Array) this.BuildStack, level + 1, (Array) this.BuildStack, level, this.BuildStackTop - level - 1);
      --this.BuildStackTop;
    }

    private void PrepareToCloseContainer(int stackPosition)
    {
      if (this.BuildStack[stackPosition].Properties != null)
      {
        for (int index = 0; index < this.BuildStack[stackPosition].Properties.Length; ++index)
        {
          if (this.BuildStack[stackPosition].Properties[index].Value.IsRefCountedHandle)
            this.Store.ReleaseValue(this.BuildStack[stackPosition].Properties[index].Value);
        }
        this.BuildStack[stackPosition].Properties = (Property[]) null;
      }
      if (this.BuildStack[stackPosition].Node != 0)
      {
        FormatNode node = this.Store.GetNode(this.BuildStack[stackPosition].Node);
        if (!this.newLine && (node.NodeType & FormatContainerType.BlockFlag) != FormatContainerType.Null)
        {
          this.Store.AddBlockBoundary();
          this.newLine = true;
          this.textQuotingExpected = true;
        }
        node.PrepareToClose(this.Store.CurrentTextPosition);
        if (!node.Parent.IsNull && node.Parent.NodeType == FormatContainerType.TableContainer)
          node.Parent.PrepareToClose(this.Store.CurrentTextPosition);
        if (this.BuildStack[stackPosition].Node == this.LastNodeInternal)
        {
          for (int index = stackPosition - 1; index >= 0; --index)
          {
            if (this.BuildStack[index].Node != 0)
            {
              this.LastNodeInternal = this.BuildStack[index].Node;
              break;
            }
          }
        }
      }
      this.Store.SetTextBoundary();
      this.EmptyContainer = false;
    }

    public virtual FormatStore ConvertToStore()
    {
      long num = 0L;
      while (!this.endOfFile)
      {
        this.Run();
        if (this.madeProgress)
        {
          this.madeProgress = false;
          num = 0L;
        }
        else if (200000L == num++)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.TooManyIterationsToProduceOutput);
      }
      return this.Store;
    }

    internal struct BuildStackEntry
    {
      internal FormatContainerType Type;
      internal Html.HtmlNameIndex TagName;
      internal Html.HtmlTagIndex TagIndex;
      internal int Node;
      internal int InheritanceMaskIndex;
      internal Property[] Properties;
      internal FlagProperties FlagProperties;
      internal PropertyBitMask PropertyMask;

      public bool IsText => this.Type == FormatContainerType.Text;

        public bool IsPropertyContainer => this.Type == FormatContainerType.PropertyContainer;

        public bool IsPropertyContainerOrNull
      {
        get
        {
          if (this.Type != FormatContainerType.PropertyContainer)
            return this.Type == FormatContainerType.Null;
          return true;
        }
      }

      public FormatContainerType NodeType
      {
        get
        {
          return this.Type;
        }
        set
        {
          this.Type = value;
        }
      }

      public void Clean()
      {
        this = new FormatConverter.BuildStackEntry();
      }
    }
  }
}
