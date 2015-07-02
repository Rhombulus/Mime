// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.FormatOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal abstract class FormatOutput : IDisposable
  {
    private PropertyState propertyState = new PropertyState();
    private SourceFormat sourceFormat;
    private string comment;
    private FormatStore formatStore;
    private FormatNode rootNode;
    private FormatOutput.OutputStackEntry currentOutputLevel;
    private FormatOutput.OutputStackEntry[] outputStack;
    private int outputStackTop;
    protected ScratchBuffer scratchBuffer;
    protected ScratchBuffer scratchValueBuffer;

    public virtual bool OutputCodePageSameAsInput => false;

      public virtual Encoding OutputEncoding
    {
      set
      {
        throw new InvalidOperationException();
      }
    }

    public virtual bool CanAcceptMoreOutput => true;

      protected FormatStore FormatStore => this.formatStore;

      protected SourceFormat SourceFormat => this.sourceFormat;

      protected string Comment => this.comment;

      protected FormatNode CurrentNode => this.currentOutputLevel.Node;

      protected int CurrentNodeIndex => this.currentOutputLevel.Index;

      protected FormatOutput(Stream formatOutputTraceStream)
    {
    }

    public virtual void Initialize(FormatStore store, SourceFormat sourceFormat, string comment)
    {
      this.sourceFormat = sourceFormat;
      this.comment = comment;
      this.formatStore = store;
      this.Restart(this.formatStore.RootNode);
    }

    public void Restart(FormatNode rootNode)
    {
      this.outputStackTop = 0;
      this.currentOutputLevel.Node = rootNode;
      this.currentOutputLevel.State = FormatOutput.OutputState.NotStarted;
      this.rootNode = rootNode;
    }

    protected void Restart()
    {
      this.Restart(this.rootNode);
    }

    public bool HaveSomethingToFlush()
    {
      return this.currentOutputLevel.Node.CanFlush;
    }

    public FlagProperties GetEffectiveFlags()
    {
      return this.propertyState.GetEffectiveFlags();
    }

    public FlagProperties GetDistinctFlags()
    {
      return this.propertyState.GetDistinctFlags();
    }

    public PropertyValue GetEffectiveProperty(PropertyId id)
    {
      return this.propertyState.GetEffectiveProperty(id);
    }

    public PropertyValue GetDistinctProperty(PropertyId id)
    {
      return this.propertyState.GetDistinctProperty(id);
    }

    public void SubtractDefaultContainerPropertiesFromDistinct(FlagProperties flags, Property[] properties)
    {
      this.propertyState.SubtractDefaultFromDistinct(flags, properties);
    }

    public virtual bool Flush()
    {
      while (this.CanAcceptMoreOutput && this.currentOutputLevel.State != FormatOutput.OutputState.Ended)
      {
        if (this.currentOutputLevel.State == FormatOutput.OutputState.NotStarted)
        {
          if (this.StartCurrentLevel())
            this.PushFirstChild();
          else
            this.PopPushNextSibling();
        }
        else if (this.currentOutputLevel.State == FormatOutput.OutputState.Started)
        {
          if (this.ContinueCurrentLevel())
            this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
        }
        else
        {
          this.EndCurrentLevel();
          this.currentOutputLevel.State = FormatOutput.OutputState.Ended;
          if (this.outputStackTop != 0)
            this.PopPushNextSibling();
        }
      }
      return this.currentOutputLevel.State == FormatOutput.OutputState.Ended;
    }

    public void OutputFragment(FormatNode fragmentNode)
    {
      this.Restart(fragmentNode);
      this.FlushFragment();
    }

    public void OutputFragment(FormatNode beginNode, uint beginTextPosition, FormatNode endNode, uint endTextPosition)
    {
      this.Restart(this.rootNode);
      FormatNode formatNode1 = beginNode;
      int val2 = 0;
      for (; formatNode1 != this.rootNode; formatNode1 = formatNode1.Parent)
        ++val2;
      if (this.outputStack == null)
        this.outputStack = new FormatOutput.OutputStackEntry[Math.Max(32, val2)];
      else if (this.outputStack.Length < val2)
      {
        if (this.outputStackTop >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        this.outputStack = new FormatOutput.OutputStackEntry[Math.Max(this.outputStack.Length * 2, val2)];
      }
      FormatNode formatNode2 = beginNode;
      int num = val2 - 1;
      for (; formatNode2 != this.rootNode; formatNode2 = formatNode2.Parent)
        this.outputStack[num--].Node = formatNode2;
      for (int index = 0; index < val2; ++index)
      {
        if (!this.StartCurrentLevel())
        {
          this.PopPushNextSibling();
          break;
        }
        this.currentOutputLevel.State = FormatOutput.OutputState.Started;
        this.Push(this.outputStack[index].Node);
      }
      bool flag = false;
      while (this.currentOutputLevel.State != FormatOutput.OutputState.Ended)
      {
        if (this.currentOutputLevel.State == FormatOutput.OutputState.NotStarted)
        {
          if (this.StartCurrentLevel())
            this.PushFirstChild();
          else
            this.PopPushNextSibling();
        }
        else if (this.currentOutputLevel.State == FormatOutput.OutputState.Started)
        {
          uint beginTextPosition1 = this.currentOutputLevel.Node == beginNode ? beginTextPosition : this.currentOutputLevel.Node.BeginTextPosition;
          uint endTextPosition1 = this.currentOutputLevel.Node == endNode ? endTextPosition : this.currentOutputLevel.Node.EndTextPosition;
          if (beginTextPosition1 <= endTextPosition1)
            this.ContinueText(beginTextPosition1, endTextPosition1);
          this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
        }
        else
        {
          this.EndCurrentLevel();
          this.currentOutputLevel.State = FormatOutput.OutputState.Ended;
          if (this.outputStackTop != 0)
          {
            if (!flag && this.currentOutputLevel.Node != endNode && (this.currentOutputLevel.Node.NextSibling.IsNull || this.currentOutputLevel.Node.NextSibling != endNode || this.currentOutputLevel.Node.NextSibling.NodeType == FormatContainerType.Text && this.currentOutputLevel.Node.NextSibling.BeginTextPosition < endTextPosition))
            {
              this.PopPushNextSibling();
            }
            else
            {
              this.Pop();
              this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
              flag = true;
            }
          }
        }
      }
    }

    private void FlushFragment()
    {
      while (this.currentOutputLevel.State != FormatOutput.OutputState.Ended)
      {
        if (this.currentOutputLevel.State == FormatOutput.OutputState.NotStarted)
        {
          if (this.StartCurrentLevel())
            this.PushFirstChild();
          else
            this.PopPushNextSibling();
        }
        else if (this.currentOutputLevel.State == FormatOutput.OutputState.Started)
        {
          if (this.ContinueCurrentLevel())
            this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
        }
        else
        {
          this.EndCurrentLevel();
          this.currentOutputLevel.State = FormatOutput.OutputState.Ended;
          if (this.outputStackTop != 0)
            this.PopPushNextSibling();
        }
      }
    }

    private bool StartCurrentLevel()
    {
      switch (this.currentOutputLevel.Node.NodeType)
      {
        case FormatContainerType.TableContainer:
          return this.StartTableContainer();
        case FormatContainerType.TableDefinition:
          return this.StartTableDefinition();
        case FormatContainerType.TableColumnGroup:
          return this.StartTableColumnGroup();
        case FormatContainerType.TableColumn:
          this.StartEndTableColumn();
          return false;
        case FormatContainerType.Inline:
          return this.StartInline();
        case FormatContainerType.HyperLink:
          return this.StartHyperLink();
        case FormatContainerType.Bookmark:
          return this.StartBookmark();
        case FormatContainerType.Area:
          this.StartEndArea();
          return false;
        case FormatContainerType.BaseFont:
          this.StartEndBaseFont();
          return false;
        case FormatContainerType.Form:
          return this.StartForm();
        case FormatContainerType.FieldSet:
          return this.StartFieldSet();
        case FormatContainerType.Label:
          return this.StartLabel();
        case FormatContainerType.Input:
          return this.StartInput();
        case FormatContainerType.Button:
          return this.StartButton();
        case FormatContainerType.Legend:
          return this.StartLegend();
        case FormatContainerType.TextArea:
          return this.StartTextArea();
        case FormatContainerType.Select:
          return this.StartSelect();
        case FormatContainerType.OptionGroup:
          return this.StartOptionGroup();
        case FormatContainerType.Option:
          return this.StartOption();
        case FormatContainerType.Text:
          return this.StartText();
        case FormatContainerType.Image:
          this.StartEndImage();
          return false;
        case FormatContainerType.Root:
          return this.StartRoot();
        case FormatContainerType.Document:
          return this.StartDocument();
        case FormatContainerType.Fragment:
          return this.StartFragment();
        case FormatContainerType.Block:
          return this.StartBlock();
        case FormatContainerType.BlockQuote:
          return this.StartBlockQuote();
        case FormatContainerType.HorizontalLine:
          this.StartEndHorizontalLine();
          return false;
        case FormatContainerType.TableCaption:
          return this.StartTableCaption();
        case FormatContainerType.TableExtraContent:
          return this.StartTableExtraContent();
        case FormatContainerType.Table:
          return this.StartTable();
        case FormatContainerType.TableRow:
          return this.StartTableRow();
        case FormatContainerType.TableCell:
          return this.StartTableCell();
        case FormatContainerType.List:
          return this.StartList();
        case FormatContainerType.ListItem:
          return this.StartListItem();
        case FormatContainerType.Map:
          return this.StartMap();
        default:
          return true;
      }
    }

    private bool ContinueCurrentLevel()
    {
      return this.ContinueText(this.currentOutputLevel.Node.BeginTextPosition, this.currentOutputLevel.Node.EndTextPosition);
    }

    private void EndCurrentLevel()
    {
      switch (this.currentOutputLevel.Node.NodeType)
      {
        case FormatContainerType.TableContainer:
          this.EndTableContainer();
          break;
        case FormatContainerType.TableDefinition:
          this.EndTableDefinition();
          break;
        case FormatContainerType.TableColumnGroup:
          this.EndTableColumnGroup();
          break;
        case FormatContainerType.Inline:
          this.EndInline();
          break;
        case FormatContainerType.HyperLink:
          this.EndHyperLink();
          break;
        case FormatContainerType.Bookmark:
          this.EndBookmark();
          break;
        case FormatContainerType.Form:
          this.EndForm();
          break;
        case FormatContainerType.FieldSet:
          this.EndFieldSet();
          break;
        case FormatContainerType.Label:
          this.EndLabel();
          break;
        case FormatContainerType.Input:
          this.EndInput();
          break;
        case FormatContainerType.Button:
          this.EndButton();
          break;
        case FormatContainerType.Legend:
          this.EndLegend();
          break;
        case FormatContainerType.TextArea:
          this.EndTextArea();
          break;
        case FormatContainerType.Select:
          this.EndSelect();
          break;
        case FormatContainerType.OptionGroup:
          this.EndOptionGroup();
          break;
        case FormatContainerType.Option:
          this.EndOption();
          break;
        case FormatContainerType.Text:
          this.EndText();
          break;
        case FormatContainerType.Root:
          this.EndRoot();
          break;
        case FormatContainerType.Document:
          this.EndDocument();
          break;
        case FormatContainerType.Fragment:
          this.EndFragment();
          break;
        case FormatContainerType.Block:
          this.EndBlock();
          break;
        case FormatContainerType.BlockQuote:
          this.EndBlockQuote();
          break;
        case FormatContainerType.TableCaption:
          this.EndTableCaption();
          break;
        case FormatContainerType.TableExtraContent:
          this.EndTableExtraContent();
          break;
        case FormatContainerType.Table:
          this.EndTable();
          break;
        case FormatContainerType.TableRow:
          this.EndTableRow();
          break;
        case FormatContainerType.TableCell:
          this.EndTableCell();
          break;
        case FormatContainerType.List:
          this.EndList();
          break;
        case FormatContainerType.ListItem:
          this.EndListItem();
          break;
        case FormatContainerType.Map:
          this.EndMap();
          break;
      }
    }

    private void PushFirstChild()
    {
      FormatNode firstChild = this.currentOutputLevel.Node.FirstChild;
      if (!firstChild.IsNull)
      {
        this.currentOutputLevel.State = FormatOutput.OutputState.Started;
        this.Push(firstChild);
      }
      else if (this.currentOutputLevel.Node.IsText)
        this.currentOutputLevel.State = FormatOutput.OutputState.Started;
      else
        this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
    }

    private void PopPushNextSibling()
    {
      FormatNode nextSibling = this.currentOutputLevel.Node.NextSibling;
      this.Pop();
      ++this.currentOutputLevel.ChildIndex;
      if (!nextSibling.IsNull)
        this.Push(nextSibling);
      else
        this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
    }

    private void Push(FormatNode node)
    {
      if (this.outputStack == null)
        this.outputStack = new FormatOutput.OutputStackEntry[32];
      else if (this.outputStackTop == this.outputStack.Length)
      {
        if (this.outputStackTop >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        FormatOutput.OutputStackEntry[] outputStackEntryArray = new FormatOutput.OutputStackEntry[this.outputStack.Length * 2];
        Array.Copy((Array) this.outputStack, 0, (Array) outputStackEntryArray, 0, this.outputStackTop);
        this.outputStack = outputStackEntryArray;
      }
      this.outputStack[this.outputStackTop++] = this.currentOutputLevel;
      this.currentOutputLevel.Node = node;
      this.currentOutputLevel.State = FormatOutput.OutputState.NotStarted;
      this.currentOutputLevel.Index = this.currentOutputLevel.ChildIndex;
      this.currentOutputLevel.ChildIndex = 0;
      this.currentOutputLevel.PropertyUndoLevel = this.propertyState.ApplyProperties(node.FlagProperties, node.Properties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].FlagProperties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].PropertyMask);
      node.SetOnLeftEdge();
    }

    private void Pop()
    {
      if (this.outputStackTop == 0)
        return;
      this.currentOutputLevel.Node.ResetOnLeftEdge();
      this.propertyState.UndoProperties(this.currentOutputLevel.PropertyUndoLevel);
      this.currentOutputLevel = this.outputStack[--this.outputStackTop];
    }

    protected virtual bool StartRoot()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndRoot()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartDocument()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndDocument()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartFragment()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndFragment()
    {
      this.EndBlockContainer();
    }

    protected virtual void StartEndBaseFont()
    {
    }

    protected virtual bool StartBlock()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndBlock()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartBlockQuote()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndBlockQuote()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartTableContainer()
    {
      return true;
    }

    protected virtual void EndTableContainer()
    {
    }

    protected virtual bool StartTableDefinition()
    {
      return true;
    }

    protected virtual void EndTableDefinition()
    {
    }

    protected virtual bool StartTableColumnGroup()
    {
      return true;
    }

    protected virtual void EndTableColumnGroup()
    {
    }

    protected virtual void StartEndTableColumn()
    {
    }

    protected virtual bool StartTableCaption()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndTableCaption()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartTableExtraContent()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndTableExtraContent()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartTable()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndTable()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartTableRow()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndTableRow()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartTableCell()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndTableCell()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartList()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndList()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartListItem()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndListItem()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartHyperLink()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndHyperLink()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartBookmark()
    {
      return true;
    }

    protected virtual void EndBookmark()
    {
    }

    protected virtual void StartEndImage()
    {
    }

    protected virtual void StartEndHorizontalLine()
    {
    }

    protected virtual bool StartInline()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndInline()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartMap()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndMap()
    {
      this.EndBlockContainer();
    }

    protected virtual void StartEndArea()
    {
    }

    protected virtual bool StartForm()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndForm()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartFieldSet()
    {
      return this.StartBlockContainer();
    }

    protected virtual void EndFieldSet()
    {
      this.EndBlockContainer();
    }

    protected virtual bool StartLabel()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndLabel()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartInput()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndInput()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartButton()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndButton()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartLegend()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndLegend()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartTextArea()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndTextArea()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartSelect()
    {
      return this.StartInlineContainer();
    }

    protected virtual void EndSelect()
    {
      this.EndInlineContainer();
    }

    protected virtual bool StartOptionGroup()
    {
      return true;
    }

    protected virtual void EndOptionGroup()
    {
    }

    protected virtual bool StartOption()
    {
      return true;
    }

    protected virtual void EndOption()
    {
    }

    protected virtual bool StartText()
    {
      return this.StartInlineContainer();
    }

    protected virtual bool ContinueText(uint beginTextPosition, uint endTextPosition)
    {
      return true;
    }

    protected virtual void EndText()
    {
      this.EndInlineContainer();
    }

    private static string Indent(int level)
    {
      return "                                                  ".Substring(0, Math.Min("                                                  ".Length, level * 2));
    }

    protected virtual bool StartBlockContainer()
    {
      return true;
    }

    protected virtual void EndBlockContainer()
    {
    }

    protected virtual bool StartInlineContainer()
    {
      return true;
    }

    protected virtual void EndInlineContainer()
    {
    }

    protected virtual void Dispose(bool disposing)
    {
      this.currentOutputLevel.Node = FormatNode.Null;
      this.outputStack = (FormatOutput.OutputStackEntry[]) null;
      this.formatStore = (FormatStore) null;
    }

    void IDisposable.Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private enum OutputState : byte
    {
      NotStarted,
      Started,
      EndPending,
      Ended,
    }

    private struct OutputStackEntry
    {
      public FormatOutput.OutputState State;
      public FormatNode Node;
      public int Index;
      public int ChildIndex;
      public int PropertyUndoLevel;
    }
  }
}
