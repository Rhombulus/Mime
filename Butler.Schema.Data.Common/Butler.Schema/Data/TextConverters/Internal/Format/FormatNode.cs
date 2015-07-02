// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.FormatNode
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct FormatNode
  {
    public static readonly FormatNode Null = new FormatNode();
    private FormatStore.NodeStore nodes;
    private int nodeHandle;

    public int Handle
    {
      get
      {
        return this.nodeHandle;
      }
    }

    public bool IsNull
    {
      get
      {
        return this.nodeHandle == 0;
      }
    }

    public bool IsInOrder
    {
      get
      {
        return (FormatStore.NodeFlags) 0 == (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.OutOfOrder);
      }
    }

    public bool OnRightEdge
    {
      get
      {
        return (FormatStore.NodeFlags) 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.OnRightEdge);
      }
    }

    public bool OnLeftEdge
    {
      get
      {
        return (FormatStore.NodeFlags) 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.OnLeftEdge);
      }
    }

    public bool IsVisited
    {
      get
      {
        return (FormatStore.NodeFlags) 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.Visited);
      }
    }

    public bool IsEmptyBlockNode
    {
      get
      {
        if ((this.NodeType & FormatContainerType.BlockFlag) != FormatContainerType.Null)
          return (int) this.BeginTextPosition + 1 == (int) this.EndTextPosition;
        return false;
      }
    }

    public bool CanFlush
    {
      get
      {
        return (FormatStore.NodeFlags) 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.CanFlush);
      }
    }

    public FormatContainerType NodeType
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type;
      }
      set
      {
        this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type = value;
      }
    }

    public bool IsText
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type == FormatContainerType.Text;
      }
    }

    public FormatNode Parent
    {
      get
      {
        int nodeHandle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Parent;
        if (nodeHandle != 0)
          return new FormatNode(this.nodes, nodeHandle);
        return FormatNode.Null;
      }
    }

    public bool IsOnlySibling
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NextSibling == this.nodeHandle;
      }
    }

    public FormatNode FirstChild
    {
      get
      {
        int handle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].LastChild;
        if (handle != 0)
          return new FormatNode(this.nodes, this.nodes.Plane(handle)[this.nodes.Index(handle)].NextSibling);
        return FormatNode.Null;
      }
    }

    public FormatNode LastChild
    {
      get
      {
        int nodeHandle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].LastChild;
        if (nodeHandle != 0)
          return new FormatNode(this.nodes, nodeHandle);
        return FormatNode.Null;
      }
    }

    public FormatNode NextSibling
    {
      get
      {
        int handle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Parent;
        if (handle != 0 && this.nodeHandle != this.nodes.Plane(handle)[this.nodes.Index(handle)].LastChild)
          return new FormatNode(this.nodes, this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NextSibling);
        return FormatNode.Null;
      }
    }

    public FormatNode PreviousSibling
    {
      get
      {
        FormatNode formatNode = this.Parent.FirstChild;
        if (this == formatNode)
          return FormatNode.Null;
        while (formatNode.NextSibling != this)
          formatNode = formatNode.NextSibling;
        return formatNode;
      }
    }

    public uint BeginTextPosition
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].BeginTextPosition;
      }
      set
      {
        this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].BeginTextPosition = value;
      }
    }

    public uint EndTextPosition
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].EndTextPosition;
      }
      set
      {
        this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].EndTextPosition = value;
      }
    }

    public int InheritanceMaskIndex
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].InheritanceMaskIndex;
      }
      set
      {
        this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].InheritanceMaskIndex = value;
      }
    }

    public FlagProperties FlagProperties
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].FlagProperties;
      }
      set
      {
        this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].FlagProperties = value;
      }
    }

    public PropertyBitMask PropertyMask
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].PropertyMask;
      }
      set
      {
        this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].PropertyMask = value;
      }
    }

    public Property[] Properties
    {
      get
      {
        return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Properties;
      }
      set
      {
        this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Properties = value;
      }
    }

    public NodePropertiesEnumerator PropertiesEnumerator
    {
      get
      {
        return new NodePropertiesEnumerator(this);
      }
    }

    public bool IsBlockNode
    {
      get
      {
        return FormatContainerType.Null != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type & FormatContainerType.BlockFlag);
      }
    }

    public FormatNode.NodeSubtree Subtree
    {
      get
      {
        return new FormatNode.NodeSubtree(this);
      }
    }

    public FormatNode.NodeChildren Children
    {
      get
      {
        return new FormatNode.NodeChildren(this);
      }
    }

    internal FormatNode(FormatStore.NodeStore nodes, int nodeHandle)
    {
      this.nodes = nodes;
      this.nodeHandle = nodeHandle;
    }

    internal FormatNode(FormatStore store, int nodeHandle)
    {
      this.nodes = store.Nodes;
      this.nodeHandle = nodeHandle;
    }

    public static bool operator ==(FormatNode x, FormatNode y)
    {
      if (x.nodes == y.nodes)
        return x.nodeHandle == y.nodeHandle;
      return false;
    }

    public static bool operator !=(FormatNode x, FormatNode y)
    {
      if (x.nodes == y.nodes)
        return x.nodeHandle != y.nodeHandle;
      return true;
    }

    public void SetOutOfOrder()
    {
      this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags |= FormatStore.NodeFlags.OutOfOrder;
    }

    public void SetOnLeftEdge()
    {
      this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags |= FormatStore.NodeFlags.OnLeftEdge;
    }

    public void ResetOnLeftEdge()
    {
      this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags &= ~FormatStore.NodeFlags.OnLeftEdge;
    }

    public void SetOnRightEdge()
    {
      this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags |= FormatStore.NodeFlags.OnRightEdge;
    }

    public void SetVisited()
    {
      this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags |= FormatStore.NodeFlags.Visited;
    }

    public void ResetVisited()
    {
      this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags &= ~FormatStore.NodeFlags.Visited;
    }

    public PropertyValue GetProperty(PropertyId id)
    {
      FormatStore.NodeEntry[] nodeEntryArray = this.nodes.Plane(this.nodeHandle);
      int index1 = this.nodes.Index(this.nodeHandle);
      if (FlagProperties.IsFlagProperty(id))
        return nodeEntryArray[index1].FlagProperties.GetPropertyValue(id);
      if (nodeEntryArray[index1].PropertyMask.IsSet(id))
      {
        for (int index2 = 0; index2 < nodeEntryArray[index1].Properties.Length; ++index2)
        {
          Property property = nodeEntryArray[index1].Properties[index2];
          if (property.Id == id)
            return property.Value;
          if (property.Id > id)
            break;
        }
      }
      return PropertyValue.Null;
    }

    public void SetProperty(PropertyId id, PropertyValue value)
    {
      FormatStore.NodeEntry[] nodeEntryArray = this.nodes.Plane(this.nodeHandle);
      int index1 = this.nodes.Index(this.nodeHandle);
      if (FlagProperties.IsFlagProperty(id))
      {
        nodeEntryArray[index1].FlagProperties.SetPropertyValue(id, value);
      }
      else
      {
        int index2 = 0;
        if (nodeEntryArray[index1].Properties != null)
        {
          for (; index2 < nodeEntryArray[index1].Properties.Length; ++index2)
          {
            Property property = nodeEntryArray[index1].Properties[index2];
            if (property.Id == id)
            {
              nodeEntryArray[index1].Properties[index2].Set(id, value);
              return;
            }
            if (property.Id > id)
              break;
          }
        }
        if (nodeEntryArray[index1].Properties == null)
        {
          nodeEntryArray[index1].Properties = new Property[1];
          nodeEntryArray[index1].Properties[0].Set(id, value);
          nodeEntryArray[index1].PropertyMask.Set(id);
        }
        else
        {
          Property[] propertyArray = new Property[nodeEntryArray[index1].Properties.Length + 1];
          if (index2 != 0)
            Array.Copy((Array) nodeEntryArray[index1].Properties, 0, (Array) propertyArray, 0, index2);
          if (index2 != nodeEntryArray[index1].Properties.Length)
            Array.Copy((Array) nodeEntryArray[index1].Properties, index2, (Array) propertyArray, index2 + 1, nodeEntryArray[index1].Properties.Length - index2);
          propertyArray[index2].Set(id, value);
          nodeEntryArray[index1].Properties = propertyArray;
          nodeEntryArray[index1].PropertyMask.Set(id);
        }
      }
    }

    public void AppendChild(FormatNode newChildNode)
    {
      FormatNode.InternalAppendChild(this.nodes, this.nodeHandle, newChildNode.Handle);
    }

    public void PrependChild(FormatNode newChildNode)
    {
      FormatNode.InternalPrependChild(this.nodes, this.nodeHandle, newChildNode.Handle);
    }

    public void InsertSiblingAfter(FormatNode newSiblingNode)
    {
      int handle1 = newSiblingNode.nodeHandle;
      FormatStore.NodeEntry[] nodeEntryArray1 = this.nodes.Plane(handle1);
      int index1 = this.nodes.Index(handle1);
      FormatStore.NodeEntry[] nodeEntryArray2 = this.nodes.Plane(this.nodeHandle);
      int index2 = this.nodes.Index(this.nodeHandle);
      int handle2 = nodeEntryArray2[index2].Parent;
      FormatStore.NodeEntry[] nodeEntryArray3 = this.nodes.Plane(handle2);
      int index3 = this.nodes.Index(handle2);
      nodeEntryArray1[index1].Parent = handle2;
      nodeEntryArray1[index1].NextSibling = nodeEntryArray2[index2].NextSibling;
      nodeEntryArray2[index2].NextSibling = handle1;
      if (this.nodeHandle != nodeEntryArray3[index3].LastChild)
        return;
      nodeEntryArray3[index3].LastChild = handle1;
    }

    public void InsertSiblingBefore(FormatNode newSiblingNode)
    {
      int handle1 = newSiblingNode.nodeHandle;
      FormatStore.NodeEntry[] nodeEntryArray1 = this.nodes.Plane(handle1);
      int index1 = this.nodes.Index(handle1);
      int handle2 = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Parent;
      int handle3 = this.nodes.Plane(handle2)[this.nodes.Index(handle2)].LastChild;
      FormatStore.NodeEntry[] nodeEntryArray2 = this.nodes.Plane(handle3);
      int index2;
      int handle4;
      for (index2 = this.nodes.Index(handle3); nodeEntryArray2[index2].NextSibling != this.nodeHandle; index2 = this.nodes.Index(handle4))
      {
        handle4 = nodeEntryArray2[index2].NextSibling;
        nodeEntryArray2 = this.nodes.Plane(handle4);
      }
      nodeEntryArray1[index1].Parent = handle2;
      nodeEntryArray1[index1].NextSibling = this.nodeHandle;
      nodeEntryArray2[index2].NextSibling = handle1;
    }

    public void RemoveFromParent()
    {
      FormatStore.NodeEntry[] nodeEntryArray1 = this.nodes.Plane(this.nodeHandle);
      int index1 = this.nodes.Index(this.nodeHandle);
      int handle1 = nodeEntryArray1[index1].Parent;
      FormatStore.NodeEntry[] nodeEntryArray2 = this.nodes.Plane(handle1);
      int index2 = this.nodes.Index(handle1);
      if (this.nodeHandle == nodeEntryArray1[index1].NextSibling)
      {
        nodeEntryArray2[index2].LastChild = 0;
      }
      else
      {
        int handle2 = nodeEntryArray2[index2].LastChild;
        FormatStore.NodeEntry[] nodeEntryArray3 = this.nodes.Plane(handle2);
        int index3;
        for (index3 = this.nodes.Index(handle2); nodeEntryArray3[index3].NextSibling != this.nodeHandle; index3 = this.nodes.Index(handle2))
        {
          handle2 = nodeEntryArray3[index3].NextSibling;
          nodeEntryArray3 = this.nodes.Plane(handle2);
        }
        nodeEntryArray3[index3].NextSibling = nodeEntryArray1[index1].NextSibling;
        if (nodeEntryArray2[index2].LastChild == this.nodeHandle)
          nodeEntryArray2[index2].LastChild = handle2;
      }
      nodeEntryArray1[index1].Parent = 0;
    }

    public void MoveAllChildrenToNewParent(FormatNode newParent)
    {
      while (!this.FirstChild.IsNull)
      {
        FormatNode firstChild = this.FirstChild;
        firstChild.RemoveFromParent();
        newParent.AppendChild(firstChild);
      }
    }

    public void ChangeNodeType(FormatContainerType newType)
    {
      this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type = newType;
    }

    public void PrepareToClose(uint endTextPosition)
    {
      FormatStore.NodeEntry[] nodeEntryArray = this.nodes.Plane(this.nodeHandle);
      int index = this.nodes.Index(this.nodeHandle);
      nodeEntryArray[index].EndTextPosition = endTextPosition;
      nodeEntryArray[index].NodeFlags &= ~FormatStore.NodeFlags.OnRightEdge;
      nodeEntryArray[index].NodeFlags |= FormatStore.NodeFlags.CanFlush;
    }

    public void SetProps(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] properties, int inheritanceMaskIndex)
    {
      FormatStore.NodeEntry[] nodeEntryArray = this.nodes.Plane(this.nodeHandle);
      int index = this.nodes.Index(this.nodeHandle);
      nodeEntryArray[index].FlagProperties = flagProperties;
      nodeEntryArray[index].PropertyMask = propertyMask;
      nodeEntryArray[index].Properties = properties;
      nodeEntryArray[index].InheritanceMaskIndex = inheritanceMaskIndex;
    }

    public FormatNode SplitTextNode(uint splitPosition)
    {
      FormatStore.NodeEntry[] nodeEntryArray1 = this.nodes.Plane(this.nodeHandle);
      int index1 = this.nodes.Index(this.nodeHandle);
      int num = this.nodes.Allocate(FormatContainerType.Text, nodeEntryArray1[index1].BeginTextPosition);
      FormatStore.NodeEntry[] nodeEntryArray2 = this.nodes.Plane(num);
      int index2 = this.nodes.Index(num);
      nodeEntryArray2[index2].NodeFlags = nodeEntryArray1[index1].NodeFlags;
      nodeEntryArray2[index2].TextMapping = nodeEntryArray1[index1].TextMapping;
      nodeEntryArray2[index2].EndTextPosition = splitPosition;
      nodeEntryArray2[index2].FlagProperties = nodeEntryArray1[index1].FlagProperties;
      nodeEntryArray2[index2].PropertyMask = nodeEntryArray1[index1].PropertyMask;
      nodeEntryArray2[index2].Properties = nodeEntryArray1[index1].Properties;
      nodeEntryArray1[index1].BeginTextPosition = splitPosition;
      FormatNode newSiblingNode = new FormatNode(this.nodes, num);
      this.InsertSiblingBefore(newSiblingNode);
      return newSiblingNode;
    }

    public FormatNode SplitNodeBeforeChild(FormatNode child)
    {
      FormatStore.NodeEntry[] nodeEntryArray1 = this.nodes.Plane(this.nodeHandle);
      int index1 = this.nodes.Index(this.nodeHandle);
      int num = this.nodes.Allocate(this.NodeType, nodeEntryArray1[index1].BeginTextPosition);
      FormatStore.NodeEntry[] nodeEntryArray2 = this.nodes.Plane(num);
      int index2 = this.nodes.Index(num);
      nodeEntryArray2[index2].NodeFlags = nodeEntryArray1[index1].NodeFlags;
      nodeEntryArray2[index2].TextMapping = nodeEntryArray1[index1].TextMapping;
      nodeEntryArray2[index2].EndTextPosition = child.BeginTextPosition;
      nodeEntryArray2[index2].FlagProperties = nodeEntryArray1[index1].FlagProperties;
      nodeEntryArray2[index2].PropertyMask = nodeEntryArray1[index1].PropertyMask;
      nodeEntryArray2[index2].Properties = nodeEntryArray1[index1].Properties;
      nodeEntryArray1[index1].BeginTextPosition = child.BeginTextPosition;
      FormatNode newSiblingNode = new FormatNode(this.nodes, num);
      do
      {
        FormatNode firstChild = this.FirstChild;
        firstChild.RemoveFromParent();
        newSiblingNode.AppendChild(firstChild);
      }
      while (this.FirstChild != child);
      this.InsertSiblingBefore(newSiblingNode);
      return newSiblingNode;
    }

    public FormatNode DuplicateInsertAsChild()
    {
      int num = this.nodes.Allocate(this.NodeType, this.BeginTextPosition);
      FormatStore.NodeEntry[] nodeEntryArray1 = this.nodes.Plane(this.nodeHandle);
      int index1 = this.nodes.Index(this.nodeHandle);
      FormatStore.NodeEntry[] nodeEntryArray2 = this.nodes.Plane(num);
      int index2 = this.nodes.Index(num);
      nodeEntryArray2[index2].NodeFlags = nodeEntryArray1[index1].NodeFlags;
      nodeEntryArray2[index2].TextMapping = nodeEntryArray1[index1].TextMapping;
      nodeEntryArray2[index2].EndTextPosition = nodeEntryArray1[index1].EndTextPosition;
      nodeEntryArray2[index2].FlagProperties = nodeEntryArray1[index1].FlagProperties;
      nodeEntryArray2[index2].PropertyMask = nodeEntryArray1[index1].PropertyMask;
      nodeEntryArray2[index2].Properties = nodeEntryArray1[index1].Properties;
      FormatNode formatNode = new FormatNode(this.nodes, num);
      this.MoveAllChildrenToNewParent(formatNode);
      this.AppendChild(formatNode);
      return formatNode;
    }

    public override bool Equals(object obj)
    {
      if (obj is FormatNode && this.nodes == ((FormatNode) obj).nodes)
        return this.nodeHandle == ((FormatNode) obj).nodeHandle;
      return false;
    }

    public override int GetHashCode()
    {
      return this.nodeHandle;
    }

    internal static void InternalAppendChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
    {
      FormatNode.InternalPrependChild(nodes, thisNode, newChildNode);
      nodes.Plane(thisNode)[nodes.Index(thisNode)].LastChild = newChildNode;
    }

    internal static void InternalPrependChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
    {
      FormatStore.NodeEntry[] nodeEntryArray1 = nodes.Plane(thisNode);
      int index1 = nodes.Index(thisNode);
      FormatStore.NodeEntry[] nodeEntryArray2 = nodes.Plane(newChildNode);
      int index2 = nodes.Index(newChildNode);
      if (nodeEntryArray1[index1].LastChild != 0)
      {
        int handle = nodeEntryArray1[index1].LastChild;
        FormatStore.NodeEntry[] nodeEntryArray3 = nodes.Plane(handle);
        int index3 = nodes.Index(handle);
        nodeEntryArray2[index2].NextSibling = nodeEntryArray3[index3].NextSibling;
        nodeEntryArray3[index3].NextSibling = newChildNode;
        nodeEntryArray2[index2].Parent = thisNode;
      }
      else
      {
        nodeEntryArray2[index2].NextSibling = newChildNode;
        nodeEntryArray2[index2].Parent = thisNode;
        nodeEntryArray1[index1].LastChild = newChildNode;
      }
    }

    internal struct NodeSubtree : IEnumerable<FormatNode>, IEnumerable
    {
      private FormatNode node;

      internal NodeSubtree(FormatNode node)
      {
        this.node = node;
      }

      public FormatNode.SubtreeEnumerator GetEnumerator()
      {
        return new FormatNode.SubtreeEnumerator(this.node, false);
      }

      public FormatNode.SubtreeEnumerator GetEnumerator(bool revisitParent)
      {
        return new FormatNode.SubtreeEnumerator(this.node, revisitParent);
      }

      IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
      {
        return (IEnumerator<FormatNode>) new FormatNode.SubtreeEnumerator(this.node, false);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new FormatNode.SubtreeEnumerator(this.node, false);
      }
    }

    internal struct NodeChildren : IEnumerable<FormatNode>, IEnumerable
    {
      private FormatNode node;

      internal NodeChildren(FormatNode node)
      {
        this.node = node;
      }

      public FormatNode.ChildrenEnumerator GetEnumerator()
      {
        return new FormatNode.ChildrenEnumerator(this.node);
      }

      IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
      {
        return (IEnumerator<FormatNode>) new FormatNode.ChildrenEnumerator(this.node);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) new FormatNode.ChildrenEnumerator(this.node);
      }
    }

    internal struct ChildrenEnumerator : IEnumerator<FormatNode>, IDisposable, IEnumerator
    {
      private FormatNode node;
      private FormatNode current;
      private FormatNode next;

      public FormatNode Current
      {
        get
        {
          if (this.current.IsNull)
            throw new InvalidOperationException(this.next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");
          return this.current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          if (this.current.IsNull)
            throw new InvalidOperationException(this.next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");
          return (object) this.current;
        }
      }

      internal ChildrenEnumerator(FormatNode node)
      {
        this.node = node;
        this.current = FormatNode.Null;
        this.next = this.node.FirstChild;
      }

      public bool MoveNext()
      {
        this.current = this.next;
        if (this.current.IsNull)
          return false;
        this.next = this.current.NextSibling;
        return true;
      }

      public void Reset()
      {
        this.current = FormatNode.Null;
        this.next = this.node.FirstChild;
      }

      public void Dispose()
      {
        this.Reset();
      }
    }

    internal struct SubtreeEnumerator : IEnumerator<FormatNode>, IDisposable, IEnumerator
    {
      private bool revisitParent;
      private FormatNode.SubtreeEnumerator.EnumeratorDisposition currentDisposition;
      private FormatNode root;
      private FormatNode current;
      private FormatNode nextChild;
      private int depth;

      public FormatNode Current
      {
        get
        {
          return this.current;
        }
      }

      public bool FirstVisit
      {
        get
        {
          return (FormatNode.SubtreeEnumerator.EnumeratorDisposition) 0 != (this.currentDisposition & FormatNode.SubtreeEnumerator.EnumeratorDisposition.Begin);
        }
      }

      public bool LastVisit
      {
        get
        {
          return (FormatNode.SubtreeEnumerator.EnumeratorDisposition) 0 != (this.currentDisposition & FormatNode.SubtreeEnumerator.EnumeratorDisposition.End);
        }
      }

      public int Depth
      {
        get
        {
          return this.depth;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.current;
        }
      }

      internal SubtreeEnumerator(FormatNode node, bool revisitParent)
      {
        this.revisitParent = revisitParent;
        this.root = node;
        this.current = FormatNode.Null;
        this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition) 0;
        this.nextChild = node;
        this.depth = -1;
      }

      public bool MoveNext()
      {
        if (this.nextChild != FormatNode.Null)
        {
          ++this.depth;
          this.current = this.nextChild;
          this.nextChild = this.current.FirstChild;
          this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition) (1 | (this.nextChild == FormatNode.Null ? 2 : 0));
          return true;
        }
        if (this.depth < 0)
          return false;
        do
        {
          --this.depth;
          if (this.depth < 0)
          {
            this.current = FormatNode.Null;
            this.nextChild = FormatNode.Null;
            this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition) 0;
            return false;
          }
          this.nextChild = this.current.NextSibling;
          this.current = this.current.Parent;
          this.currentDisposition = this.nextChild == FormatNode.Null ? FormatNode.SubtreeEnumerator.EnumeratorDisposition.End : (FormatNode.SubtreeEnumerator.EnumeratorDisposition) 0;
        }
        while (!this.revisitParent && this.nextChild == FormatNode.Null);
        if (!this.revisitParent)
          return this.MoveNext();
        return true;
      }

      public FormatNode PreviewNextNode()
      {
        if (this.nextChild != FormatNode.Null)
          return this.nextChild;
        if (this.depth < 0)
          return FormatNode.Null;
        int num = this.depth;
        FormatNode formatNode = this.current;
        FormatNode nextSibling;
        do
        {
          --num;
          if (num < 0)
            return FormatNode.Null;
          nextSibling = formatNode.NextSibling;
          formatNode = formatNode.Parent;
        }
        while (!this.revisitParent && nextSibling == FormatNode.Null);
        if (!this.revisitParent)
          return nextSibling;
        return formatNode;
      }

      public void SkipChildren()
      {
        if (!(this.nextChild != FormatNode.Null))
          return;
        this.nextChild = FormatNode.Null;
        this.currentDisposition |= FormatNode.SubtreeEnumerator.EnumeratorDisposition.End;
      }

      void IEnumerator.Reset()
      {
        this.current = FormatNode.Null;
        this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition) 0;
        this.nextChild = this.root;
        this.depth = -1;
      }

      void IDisposable.Dispose()
      {
        ((IEnumerator) this).Reset();
        GC.SuppressFinalize((object) this);
      }

      [Flags]
      private enum EnumeratorDisposition : byte
      {
        Begin = (byte) 1,
        End = (byte) 2,
      }
    }
  }
}
