// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.FormatStore
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal class FormatStore
  {
    internal FormatStore.NodeStore Nodes;
    internal FormatStore.StyleStore Styles;
    internal FormatStore.StringValueStore Strings;
    internal FormatStore.MultiValueStore MultiValues;
    internal FormatStore.TextStore Text;

    public FormatNode RootNode => new FormatNode(this, 1);

      public uint CurrentTextPosition => this.Text.CurrentPosition;

      public FormatStore()
    {
      this.Nodes = new FormatStore.NodeStore(this);
      this.Styles = new FormatStore.StyleStore(this, FormatStoreData.GlobalStyles);
      this.Strings = new FormatStore.StringValueStore(FormatStoreData.GlobalStringValues);
      this.MultiValues = new FormatStore.MultiValueStore(this, FormatStoreData.GlobalMultiValues);
      this.Text = new FormatStore.TextStore();
      this.Initialize();
    }

    public void Initialize()
    {
      this.Nodes.Initialize();
      this.Styles.Initialize(FormatStoreData.GlobalStyles);
      this.Strings.Initialize(FormatStoreData.GlobalStringValues);
      this.MultiValues.Initialize(FormatStoreData.GlobalMultiValues);
      this.Text.Initialize();
    }

    public void ReleaseValue(PropertyValue value)
    {
      if (value.IsString)
      {
        this.GetStringValue(value).Release();
      }
      else
      {
        if (!value.IsMultiValue)
          return;
        this.GetMultiValue(value).Release();
      }
    }

    public void AddRefValue(PropertyValue value)
    {
      if (value.IsString)
      {
        this.GetStringValue(value).AddRef();
      }
      else
      {
        if (!value.IsMultiValue)
          return;
        this.GetMultiValue(value).AddRef();
      }
    }

    public FormatNode GetNode(int nodeHandle)
    {
      return new FormatNode(this, nodeHandle);
    }

    public FormatNode AllocateNode(FormatContainerType type)
    {
      return new FormatNode(this.Nodes, this.Nodes.Allocate(type, this.CurrentTextPosition));
    }

    public FormatNode AllocateNode(FormatContainerType type, uint beginTextPosition)
    {
      return new FormatNode(this.Nodes, this.Nodes.Allocate(type, beginTextPosition));
    }

    public void FreeNode(FormatNode node)
    {
      this.Nodes.Free(node.Handle);
    }

    public FormatStyle AllocateStyle(bool isStatic)
    {
      return new FormatStyle(this, this.Styles.Allocate(isStatic));
    }

    public FormatStyle GetStyle(int styleHandle)
    {
      return new FormatStyle(this, styleHandle);
    }

    public void FreeStyle(FormatStyle style)
    {
      this.Styles.Free(style.Handle);
    }

    public StringValue AllocateStringValue(bool isStatic)
    {
      return new StringValue(this, this.Strings.Allocate(isStatic));
    }

    public StringValue AllocateStringValue(bool isStatic, string value)
    {
      StringValue stringValue = this.AllocateStringValue(isStatic);
      stringValue.SetString(value);
      return stringValue;
    }

    public StringValue GetStringValue(PropertyValue propertyValue)
    {
      return new StringValue(this, propertyValue.StringHandle);
    }

    public void FreeStringValue(StringValue str)
    {
      this.Strings.Free(str.Handle);
    }

    public MultiValue AllocateMultiValue(bool isStatic)
    {
      return new MultiValue(this, this.MultiValues.Allocate(isStatic));
    }

    public MultiValue GetMultiValue(PropertyValue propertyValue)
    {
      return new MultiValue(this, propertyValue.MultiValueHandle);
    }

    public void FreeMultiValue(MultiValue multi)
    {
      this.MultiValues.Free(multi.Handle);
    }

    public void InitializeCodepageDetector()
    {
      this.Text.InitializeCodepageDetector();
    }

    public int GetBestWindowsCodePage()
    {
      return this.Text.GetBestWindowsCodePage();
    }

    public int GetBestWindowsCodePage(int preferredCodePage)
    {
      return this.Text.GetBestWindowsCodePage(preferredCodePage);
    }

    public void SetTextBoundary()
    {
      this.Text.DoNotMergeNextRun();
    }

    public void AddBlockBoundary()
    {
      if (this.Text.LastRunType == TextRunType.BlockBoundary)
        return;
      this.Text.AddSimpleRun(TextRunType.BlockBoundary, 1);
    }

    public void AddMarkupText(char[] textBuffer, int offset, int count)
    {
      this.Text.AddText(TextRunType.Markup, textBuffer, offset, count);
    }

    public void AddText(char[] textBuffer, int offset, int count)
    {
      this.Text.AddText(TextRunType.NonSpace, textBuffer, offset, count);
    }

    public void AddInlineObject()
    {
      this.Text.AddSimpleRun(TextRunType.FirstShort, 1);
      this.Text.DoNotMergeNextRun();
    }

    public void AddSpace(int count)
    {
      this.Text.AddSimpleRun(TextRunType.Space, count);
    }

    public void AddLineBreak(int count)
    {
      this.Text.AddSimpleRun(TextRunType.NewLine, count);
    }

    public void AddNbsp(int count)
    {
      this.Text.AddSimpleRun(TextRunType.NbSp, count);
    }

    public void AddTabulation(int count)
    {
      this.Text.AddSimpleRun(TextRunType.Tabulation, count);
    }

    internal TextRun GetTextRun(uint position)
    {
      if (position < this.CurrentTextPosition)
        return this.GetTextRunReally(position);
      return TextRun.Invalid;
    }

    internal TextRun GetTextRunReally(uint position)
    {
      return new TextRun(this.Text, position);
    }

    [Flags]
    internal enum NodeFlags : byte
    {
      OnRightEdge = (byte) 1,
      OnLeftEdge = (byte) 2,
      CanFlush = (byte) 4,
      OutOfOrder = (byte) 8,
      Visited = (byte) 16,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct NodeEntry
    {
      internal FormatContainerType Type;
      internal FormatStore.NodeFlags NodeFlags;
      internal TextMapping TextMapping;
      internal int Parent;
      internal int LastChild;
      internal int NextSibling;
      internal uint BeginTextPosition;
      internal uint EndTextPosition;
      internal int InheritanceMaskIndex;
      internal FlagProperties FlagProperties;
      internal PropertyBitMask PropertyMask;
      internal Property[] Properties;

      internal int NextFree
      {
        get
        {
          return this.NextSibling;
        }
        set
        {
          this.NextSibling = value;
        }
      }

      public void Clean()
      {
        this = new FormatStore.NodeEntry();
      }

      public override string ToString()
      {
        return this.Type.ToString() + " (" + this.Parent.ToString("X") + ", " + this.LastChild.ToString("X") + ", " + this.NextSibling.ToString("X") + ") " + this.BeginTextPosition.ToString("X") + " - " + this.EndTextPosition.ToString("X");
      }
    }

    internal struct StyleEntry
    {
      internal int RefCount;
      internal FlagProperties FlagProperties;
      internal PropertyBitMask PropertyMask;
      internal Property[] PropertyList;

      internal int NextFree
      {
        get
        {
          return this.FlagProperties.IntegerBag;
        }
        set
        {
          this.FlagProperties.IntegerBag = value;
        }
      }

      public StyleEntry(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] propertyList)
      {
        this.RefCount = int.MaxValue;
        this.FlagProperties = flagProperties;
        this.PropertyMask = propertyMask;
        this.PropertyList = propertyList;
      }

      public void Clean()
      {
        this = new FormatStore.StyleEntry();
      }
    }

    internal struct MultiValueEntry
    {
      internal int RefCount;
      internal int NextFree;
      internal PropertyValue[] Values;

      public MultiValueEntry(PropertyValue[] values)
      {
        this.RefCount = int.MaxValue;
        this.Values = values;
        this.NextFree = 0;
      }

      public void Clean()
      {
        this = new FormatStore.MultiValueEntry();
      }
    }

    internal struct StringValueEntry
    {
      internal int RefCount;
      internal int NextFree;
      internal string Str;

      public StringValueEntry(string str)
      {
        this.RefCount = int.MaxValue;
        this.Str = str;
        this.NextFree = 0;
      }

      public void Clean()
      {
        this = new FormatStore.StringValueEntry();
      }
    }

    internal struct InheritaceMask
    {
      internal FlagProperties FlagProperties;
      internal PropertyBitMask PropertyMask;

      public InheritaceMask(FlagProperties flagProperties, PropertyBitMask propertyMask)
      {
        this.FlagProperties = flagProperties;
        this.PropertyMask = propertyMask;
      }
    }

    internal class NodeStore
    {
      internal const int MaxElementsPerPlane = 1024;
      internal const int MaxPlanes = 1024;
      internal const int InitialPlanes = 32;
      internal const int InitialElements = 16;
      private FormatStore store;
      private FormatStore.NodeEntry[][] planes;
      private int freeListHead;
      private int top;

      public NodeStore(FormatStore store)
      {
        this.store = store;
        this.planes = new FormatStore.NodeEntry[32][];
        this.planes[0] = new FormatStore.NodeEntry[16];
        this.freeListHead = 0;
        this.top = 0;
      }

      public FormatStore.NodeEntry[] Plane(int handle)
      {
        return this.planes[handle / 1024];
      }

      public int Index(int handle)
      {
        return handle % 1024;
      }

      public void Initialize()
      {
        this.freeListHead = -1;
        this.top = 1;
        this.planes[0][1].Type = FormatContainerType.Root;
        this.planes[0][1].NodeFlags = FormatStore.NodeFlags.OnRightEdge | FormatStore.NodeFlags.CanFlush;
        this.planes[0][1].TextMapping = TextMapping.Unicode;
        this.planes[0][1].Parent = 0;
        this.planes[0][1].NextSibling = 1;
        this.planes[0][1].LastChild = 0;
        this.planes[0][1].BeginTextPosition = 0U;
        this.planes[0][1].EndTextPosition = uint.MaxValue;
        this.planes[0][1].FlagProperties = new FlagProperties();
        this.planes[0][1].PropertyMask = new PropertyBitMask();
        this.planes[0][1].Properties = (Property[]) null;
        ++this.top;
      }

      public int Allocate(FormatContainerType type, uint currentTextPosition)
      {
        int num = this.freeListHead;
        int index1;
        FormatStore.NodeEntry[] nodeEntryArray1;
        if (num != -1)
        {
          index1 = num % 1024;
          nodeEntryArray1 = this.planes[num / 1024];
          this.freeListHead = nodeEntryArray1[index1].NextFree;
        }
        else
        {
          num = this.top++;
          index1 = num % 1024;
          int index2 = num / 1024;
          if (index1 == 0)
          {
            if (index2 == 1024)
              throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
            if (index2 == this.planes.Length)
            {
              FormatStore.NodeEntry[][] nodeEntryArray2 = new FormatStore.NodeEntry[Math.Min(this.planes.Length * 2, 1024)][];
              Array.Copy((Array) this.planes, 0, (Array) nodeEntryArray2, 0, this.planes.Length);
              this.planes = nodeEntryArray2;
            }
            if (this.planes[index2] == null)
              this.planes[index2] = new FormatStore.NodeEntry[1024];
          }
          else if (index2 == 0 && index1 == this.planes[index2].Length)
          {
            FormatStore.NodeEntry[] nodeEntryArray2 = new FormatStore.NodeEntry[Math.Min(this.planes[0].Length * 2, 1024)];
            Array.Copy((Array) this.planes[0], 0, (Array) nodeEntryArray2, 0, this.planes[0].Length);
            this.planes[0] = nodeEntryArray2;
          }
          nodeEntryArray1 = this.planes[index2];
        }
        nodeEntryArray1[index1].Type = type;
        nodeEntryArray1[index1].NodeFlags = (FormatStore.NodeFlags) 0;
        nodeEntryArray1[index1].TextMapping = TextMapping.Unicode;
        nodeEntryArray1[index1].Parent = 0;
        nodeEntryArray1[index1].LastChild = 0;
        nodeEntryArray1[index1].NextSibling = num;
        nodeEntryArray1[index1].BeginTextPosition = currentTextPosition;
        nodeEntryArray1[index1].EndTextPosition = uint.MaxValue;
        nodeEntryArray1[index1].FlagProperties.ClearAll();
        nodeEntryArray1[index1].PropertyMask.ClearAll();
        nodeEntryArray1[index1].Properties = (Property[]) null;
        return num;
      }

      public void Free(int handle)
      {
        int index1 = handle % 1024;
        FormatStore.NodeEntry[] nodeEntryArray = this.planes[handle / 1024];
        if (nodeEntryArray[index1].Properties != null)
        {
          for (int index2 = 0; index2 < nodeEntryArray[index1].Properties.Length; ++index2)
          {
            if (nodeEntryArray[index1].Properties[index2].Value.IsRefCountedHandle)
              this.store.ReleaseValue(nodeEntryArray[index1].Properties[index2].Value);
          }
        }
        nodeEntryArray[index1].NextFree = this.freeListHead;
        this.freeListHead = handle;
      }

      public long DumpStat(TextWriter dumpWriter)
      {
        int num1 = this.top < 1024 ? 1 : (this.top % 1024 == 0 ? this.top / 1024 : this.top / 1024 + 1);
        long num2 = num1 == 1 ? (long) this.planes[0].Length : (long) (num1 * 1024);
        long num3 = (long) (12 + this.planes.Length * 4 + 12 * num1) + num2 * (long) Marshal.SizeOf(typeof (FormatStore.NodeEntry));
        long num4 = 0L;
        long num5 = 0L;
        long num6 = 0L;
        long num7 = 0L;
        for (int index1 = 0; index1 < this.top; ++index1)
        {
          int index2 = index1 % 1024;
          FormatStore.NodeEntry[] nodeEntryArray = this.planes[index1 / 1024];
          if (nodeEntryArray[index2].Type != FormatContainerType.Null)
          {
            ++num4;
            if (nodeEntryArray[index2].Properties != null)
            {
              num3 += (long) (12 + nodeEntryArray[index2].Properties.Length * Marshal.SizeOf(typeof (Property)));
              ++num5;
              num6 += (long) nodeEntryArray[index2].Properties.Length;
              if ((long) nodeEntryArray[index2].Properties.Length > num7)
                num7 = (long) nodeEntryArray[index2].Properties.Length;
            }
          }
        }
        long num8 = num5 == 0L ? 0L : (num6 + num5 - 1L) / num5;
        if (dumpWriter != null)
        {
          dumpWriter.WriteLine("Nodes alloc: {0}", (object) num2);
          dumpWriter.WriteLine("Nodes used: {0}", (object) num4);
          dumpWriter.WriteLine("Nodes proplists: {0}", (object) num5);
          if (num5 != 0L)
          {
            dumpWriter.WriteLine("Nodes props: {0}", (object) num6);
            dumpWriter.WriteLine("Nodes average proplist: {0}", (object) num8);
            dumpWriter.WriteLine("Nodes max proplist: {0}", (object) num7);
          }
          dumpWriter.WriteLine("Nodes bytes: {0}", (object) num3);
        }
        return num3;
      }
    }

    internal class StyleStore
    {
      internal const int MaxElementsPerPlane = 2048;
      internal const int MaxPlanes = 512;
      internal const int InitialPlanes = 16;
      internal const int InitialElements = 32;
      private FormatStore store;
      private FormatStore.StyleEntry[][] planes;
      private int freeListHead;
      private int top;

      public StyleStore(FormatStore store, FormatStore.StyleEntry[] globalStyles)
      {
        this.store = store;
        this.planes = new FormatStore.StyleEntry[16][];
        this.planes[0] = new FormatStore.StyleEntry[Math.Max(32, globalStyles.Length + 1)];
        this.freeListHead = 0;
        this.top = 0;
        if (globalStyles == null || globalStyles.Length == 0)
          return;
        Array.Copy((Array) globalStyles, 0, (Array) this.planes[0], 0, globalStyles.Length);
      }

      public FormatStore.StyleEntry[] Plane(int handle)
      {
        return this.planes[handle / 2048];
      }

      public int Index(int handle)
      {
        return handle % 2048;
      }

      public void Initialize(FormatStore.StyleEntry[] globalStyles)
      {
        this.freeListHead = -1;
        if (globalStyles != null && globalStyles.Length != 0)
          this.top = globalStyles.Length;
        else
          this.top = 1;
      }

      public int Allocate(bool isStatic)
      {
        int num = this.freeListHead;
        int index1;
        FormatStore.StyleEntry[] styleEntryArray1;
        if (num != -1)
        {
          index1 = num % 2048;
          styleEntryArray1 = this.planes[num / 2048];
          this.freeListHead = styleEntryArray1[index1].NextFree;
        }
        else
        {
          num = this.top++;
          index1 = num % 2048;
          int index2 = num / 2048;
          if (index1 == 0)
          {
            if (index2 == 512)
              throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
            if (index2 == this.planes.Length)
            {
              FormatStore.StyleEntry[][] styleEntryArray2 = new FormatStore.StyleEntry[Math.Min(this.planes.Length * 2, 512)][];
              Array.Copy((Array) this.planes, 0, (Array) styleEntryArray2, 0, this.planes.Length);
              this.planes = styleEntryArray2;
            }
            if (this.planes[index2] == null)
              this.planes[index2] = new FormatStore.StyleEntry[2048];
          }
          else if (index2 == 0 && index1 == this.planes[index2].Length)
          {
            FormatStore.StyleEntry[] styleEntryArray2 = new FormatStore.StyleEntry[Math.Min(this.planes[0].Length * 2, 2048)];
            Array.Copy((Array) this.planes[0], 0, (Array) styleEntryArray2, 0, this.planes[0].Length);
            this.planes[0] = styleEntryArray2;
          }
          styleEntryArray1 = this.planes[index2];
        }
        styleEntryArray1[index1].PropertyList = (Property[]) null;
        styleEntryArray1[index1].RefCount = isStatic ? int.MaxValue : 1;
        styleEntryArray1[index1].FlagProperties.ClearAll();
        styleEntryArray1[index1].PropertyMask.ClearAll();
        return num;
      }

      public void Free(int handle)
      {
        int index1 = handle % 2048;
        FormatStore.StyleEntry[] styleEntryArray = this.planes[handle / 2048];
        if (styleEntryArray[index1].PropertyList != null)
        {
          for (int index2 = 0; index2 < styleEntryArray[index1].PropertyList.Length; ++index2)
          {
            if (styleEntryArray[index1].PropertyList[index2].Value.IsRefCountedHandle)
              this.store.ReleaseValue(styleEntryArray[index1].PropertyList[index2].Value);
          }
        }
        styleEntryArray[index1].NextFree = this.freeListHead;
        this.freeListHead = handle;
      }

      public long DumpStat(TextWriter dumpWriter)
      {
        int num1 = this.top < 2048 ? 1 : (this.top % 2048 == 0 ? this.top / 2048 : this.top / 2048 + 1);
        long num2 = num1 == 1 ? (long) this.planes[0].Length : (long) (num1 * 2048);
        long num3 = (long) (12 + this.planes.Length * 4 + 12 * num1) + num2 * (long) Marshal.SizeOf(typeof (FormatStore.StyleEntry));
        long num4 = 0L;
        long num5 = 0L;
        long num6 = 0L;
        long num7 = 0L;
        for (int index1 = 0; index1 < this.top; ++index1)
        {
          int index2 = index1 % 2048;
          FormatStore.StyleEntry[] styleEntryArray = this.planes[index1 / 2048];
          if (styleEntryArray[index2].RefCount != 0)
          {
            ++num4;
            if (styleEntryArray[index2].PropertyList != null)
            {
              num3 += (long) (12 + styleEntryArray[index2].PropertyList.Length * Marshal.SizeOf(typeof (Property)));
              ++num5;
              num6 += (long) styleEntryArray[index2].PropertyList.Length;
              if ((long) styleEntryArray[index2].PropertyList.Length > num7)
                num7 = (long) styleEntryArray[index2].PropertyList.Length;
            }
          }
        }
        long num8 = num5 == 0L ? 0L : (num6 + num5 - 1L) / num5;
        if (dumpWriter != null)
        {
          dumpWriter.WriteLine("Styles alloc: {0}", (object) num2);
          dumpWriter.WriteLine("Styles used: {0}", (object) num4);
          dumpWriter.WriteLine("Styles non-null prop lists: {0}", (object) num5);
          if (num5 != 0L)
          {
            dumpWriter.WriteLine("Styles total prop lists length: {0}", (object) num6);
            dumpWriter.WriteLine("Styles average prop list length: {0}", (object) num8);
            dumpWriter.WriteLine("Styles max prop list length: {0}", (object) num7);
          }
          dumpWriter.WriteLine("Styles bytes: {0}", (object) num3);
        }
        return num3;
      }
    }

    internal class StringValueStore
    {
      internal const int MaxElementsPerPlane = 4096;
      internal const int MaxPlanes = 256;
      internal const int InitialPlanes = 16;
      internal const int InitialElements = 16;
      private FormatStore.StringValueEntry[][] planes;
      private int freeListHead;
      private int top;

      public StringValueStore(FormatStore.StringValueEntry[] globalStrings)
      {
        this.planes = new FormatStore.StringValueEntry[16][];
        this.planes[0] = new FormatStore.StringValueEntry[Math.Max(16, globalStrings.Length + 1)];
        this.freeListHead = 0;
        this.top = 0;
        if (globalStrings == null || globalStrings.Length == 0)
          return;
        Array.Copy((Array) globalStrings, 0, (Array) this.planes[0], 0, globalStrings.Length);
      }

      public FormatStore.StringValueEntry[] Plane(int handle)
      {
        return this.planes[handle / 4096];
      }

      public int Index(int handle)
      {
        return handle % 4096;
      }

      public void Initialize(FormatStore.StringValueEntry[] globalStrings)
      {
        this.freeListHead = -1;
        if (globalStrings != null && globalStrings.Length != 0)
          this.top = globalStrings.Length;
        else
          this.top = 1;
      }

      public int Allocate(bool isStatic)
      {
        int num = this.freeListHead;
        int index1;
        FormatStore.StringValueEntry[] stringValueEntryArray1;
        if (num != -1)
        {
          index1 = num % 4096;
          stringValueEntryArray1 = this.planes[num / 4096];
          this.freeListHead = stringValueEntryArray1[index1].NextFree;
        }
        else
        {
          num = this.top++;
          index1 = num % 4096;
          int index2 = num / 4096;
          if (index1 == 0)
          {
            if (index2 == 256)
              throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
            if (index2 == this.planes.Length)
            {
              FormatStore.StringValueEntry[][] stringValueEntryArray2 = new FormatStore.StringValueEntry[Math.Min(this.planes.Length * 2, 256)][];
              Array.Copy((Array) this.planes, 0, (Array) stringValueEntryArray2, 0, this.planes.Length);
              this.planes = stringValueEntryArray2;
            }
            if (this.planes[index2] == null)
              this.planes[index2] = new FormatStore.StringValueEntry[4096];
          }
          else if (index2 == 0 && index1 == this.planes[index2].Length)
          {
            FormatStore.StringValueEntry[] stringValueEntryArray2 = new FormatStore.StringValueEntry[Math.Min(this.planes[0].Length * 2, 4096)];
            Array.Copy((Array) this.planes[0], 0, (Array) stringValueEntryArray2, 0, this.planes[0].Length);
            this.planes[0] = stringValueEntryArray2;
          }
          stringValueEntryArray1 = this.planes[index2];
        }
        stringValueEntryArray1[index1].Str = (string) null;
        stringValueEntryArray1[index1].RefCount = isStatic ? int.MaxValue : 1;
        stringValueEntryArray1[index1].NextFree = -1;
        return num;
      }

      public void Free(int handle)
      {
        int index = handle % 4096;
        this.planes[handle / 4096][index].NextFree = this.freeListHead;
        this.freeListHead = handle;
      }

      public long DumpStat(TextWriter dumpWriter)
      {
        int num1 = this.top < 4096 ? 1 : (this.top % 4096 == 0 ? this.top / 4096 : this.top / 4096 + 1);
        long num2 = num1 == 1 ? (long) this.planes[0].Length : (long) (num1 * 4096);
        long num3 = (long) (12 + this.planes.Length * 4 + 12 * num1) + num2 * (long) Marshal.SizeOf(typeof (FormatStore.StringValueEntry));
        long num4 = 0L;
        long num5 = 0L;
        long num6 = 0L;
        long num7 = 0L;
        for (int index1 = 0; index1 < this.top; ++index1)
        {
          int index2 = index1 % 4096;
          FormatStore.StringValueEntry[] stringValueEntryArray = this.planes[index1 / 4096];
          if (stringValueEntryArray[index2].RefCount != 0)
          {
            ++num4;
            if (stringValueEntryArray[index2].Str != null)
            {
              num3 += (long) (12 + stringValueEntryArray[index2].Str.Length * 2);
              ++num5;
              num6 += (long) stringValueEntryArray[index2].Str.Length;
              if ((long) stringValueEntryArray[index2].Str.Length > num7)
                num7 = (long) stringValueEntryArray[index2].Str.Length;
            }
          }
        }
        long num8 = num5 == 0L ? 0L : (num6 + num5 - 1L) / num5;
        if (dumpWriter != null)
        {
          dumpWriter.WriteLine("StringValues alloc: {0}", (object) num2);
          dumpWriter.WriteLine("StringValues used: {0}", (object) num4);
          dumpWriter.WriteLine("StringValues non-null strings: {0}", (object) num5);
          if (num5 != 0L)
          {
            dumpWriter.WriteLine("StringValues total string length: {0}", (object) num6);
            dumpWriter.WriteLine("StringValues average string length: {0}", (object) num8);
            dumpWriter.WriteLine("StringValues max string length: {0}", (object) num7);
          }
          dumpWriter.WriteLine("StringValues bytes: {0}", (object) num3);
        }
        return num3;
      }
    }

    internal class MultiValueStore
    {
      internal const int MaxElementsPerPlane = 4096;
      internal const int MaxPlanes = 256;
      internal const int InitialPlanes = 16;
      internal const int InitialElements = 16;
        private FormatStore.MultiValueEntry[][] planes;
      private int freeListHead;
      private int top;

      public FormatStore Store { get; }

        public MultiValueStore(FormatStore store, FormatStore.MultiValueEntry[] globaMultiValues)
      {
        this.Store = store;
        this.planes = new FormatStore.MultiValueEntry[16][];
        this.planes[0] = new FormatStore.MultiValueEntry[Math.Max(16, globaMultiValues.Length + 1)];
        this.freeListHead = 0;
        this.top = 0;
        if (globaMultiValues == null || globaMultiValues.Length == 0)
          return;
        Array.Copy((Array) globaMultiValues, 0, (Array) this.planes[0], 0, globaMultiValues.Length);
      }

      public FormatStore.MultiValueEntry[] Plane(int handle)
      {
        return this.planes[handle / 4096];
      }

      public int Index(int handle)
      {
        return handle % 4096;
      }

      public void Initialize(FormatStore.MultiValueEntry[] globaMultiValues)
      {
        this.freeListHead = -1;
        if (globaMultiValues != null && globaMultiValues.Length != 0)
          this.top = globaMultiValues.Length;
        else
          this.top = 1;
      }

      public int Allocate(bool isStatic)
      {
        int num = this.freeListHead;
        int index1;
        FormatStore.MultiValueEntry[] multiValueEntryArray1;
        if (num != -1)
        {
          index1 = num % 4096;
          multiValueEntryArray1 = this.planes[num / 4096];
          this.freeListHead = multiValueEntryArray1[index1].NextFree;
        }
        else
        {
          num = this.top++;
          index1 = num % 4096;
          int index2 = num / 4096;
          if (index1 == 0)
          {
            if (index2 == 256)
              throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
            if (index2 == this.planes.Length)
            {
              FormatStore.MultiValueEntry[][] multiValueEntryArray2 = new FormatStore.MultiValueEntry[Math.Min(this.planes.Length * 2, 256)][];
              Array.Copy((Array) this.planes, 0, (Array) multiValueEntryArray2, 0, this.planes.Length);
              this.planes = multiValueEntryArray2;
            }
            if (this.planes[index2] == null)
              this.planes[index2] = new FormatStore.MultiValueEntry[4096];
          }
          else if (index2 == 0 && index1 == this.planes[index2].Length)
          {
            FormatStore.MultiValueEntry[] multiValueEntryArray2 = new FormatStore.MultiValueEntry[Math.Min(this.planes[0].Length * 2, 4096)];
            Array.Copy((Array) this.planes[0], 0, (Array) multiValueEntryArray2, 0, this.planes[0].Length);
            this.planes[0] = multiValueEntryArray2;
          }
          multiValueEntryArray1 = this.planes[index2];
        }
        multiValueEntryArray1[index1].Values = (PropertyValue[]) null;
        multiValueEntryArray1[index1].RefCount = isStatic ? int.MaxValue : 1;
        multiValueEntryArray1[index1].NextFree = -1;
        return num;
      }

      public void Free(int handle)
      {
        int index1 = handle % 4096;
        FormatStore.MultiValueEntry[] multiValueEntryArray = this.planes[handle / 4096];
        if (multiValueEntryArray[index1].Values != null)
        {
          for (int index2 = 0; index2 < multiValueEntryArray[index1].Values.Length; ++index2)
          {
            if (multiValueEntryArray[index1].Values[index2].IsRefCountedHandle)
              this.Store.ReleaseValue(multiValueEntryArray[index1].Values[index2]);
          }
        }
        multiValueEntryArray[index1].NextFree = this.freeListHead;
        this.freeListHead = handle;
      }

      public long DumpStat(TextWriter dumpWriter)
      {
        int num1 = this.top < 4096 ? 1 : (this.top % 4096 == 0 ? this.top / 4096 : this.top / 4096 + 1);
        long num2 = num1 == 1 ? (long) this.planes[0].Length : (long) (num1 * 4096);
        long num3 = (long) (12 + this.planes.Length * 4 + 12 * num1) + num2 * (long) Marshal.SizeOf(typeof (FormatStore.MultiValueEntry));
        long num4 = 0L;
        long num5 = 0L;
        long num6 = 0L;
        long num7 = 0L;
        for (int index1 = 0; index1 < this.top; ++index1)
        {
          int index2 = index1 % 4096;
          FormatStore.MultiValueEntry[] multiValueEntryArray = this.planes[index1 / 4096];
          if (multiValueEntryArray[index2].RefCount != 0)
          {
            ++num4;
            if (multiValueEntryArray[index2].Values != null)
            {
              num3 += (long) (12 + multiValueEntryArray[index2].Values.Length * Marshal.SizeOf(typeof (PropertyValue)));
              ++num5;
              num6 += (long) multiValueEntryArray[index2].Values.Length;
              if ((long) multiValueEntryArray[index2].Values.Length > num7)
                num7 = (long) multiValueEntryArray[index2].Values.Length;
            }
          }
        }
        long num8 = num5 == 0L ? 0L : (num6 + num5 - 1L) / num5;
        if (dumpWriter != null)
        {
          dumpWriter.WriteLine("MultiValues alloc: {0}", (object) num2);
          dumpWriter.WriteLine("MultiValues used: {0}", (object) num4);
          dumpWriter.WriteLine("MultiValues non-null value lists: {0}", (object) num5);
          if (num5 != 0L)
          {
            dumpWriter.WriteLine("MultiValues total value lists length: {0}", (object) num6);
            dumpWriter.WriteLine("MultiValues average value list length: {0}", (object) num8);
            dumpWriter.WriteLine("MultiValues max value list length: {0}", (object) num7);
          }
          dumpWriter.WriteLine("MultiValues bytes: {0}", (object) num3);
        }
        return num3;
      }
    }

    internal class TextStore
    {
      internal const int LogMaxCharactersPerPlane = 15;
      internal const int MaxCharactersPerPlane = 32768;
      internal const int MaxPlanes = 640;
      internal const int InitialPlanes = 16;
      internal const int InitialCharacters = 1024;
      internal const int MaxRunEffectivelength = 4095;
      private Globalization.OutboundCodePageDetector detector;
      private char[][] planes;
        private uint lastRunPosition;

      public uint CurrentPosition { get; private set; }

        public TextRunType LastRunType { get; private set; }

        public TextStore()
      {
        this.planes = new char[16][];
        this.planes[0] = new char[1024];
      }

      public char[] Plane(uint position)
      {
        return this.planes[(position >> 15)];
      }

      public int Index(uint position)
      {
        return (int) position & (int) short.MaxValue;
      }

      public char Pick(uint position)
      {
        return this.planes[(position >> 15)][(position & (uint) short.MaxValue)];
      }

      public void Initialize()
      {
        this.CurrentPosition = 0U;
        this.LastRunType = TextRunType.Invalid;
        this.lastRunPosition = 0U;
          this.detector?.Reset();
      }

      public void InitializeCodepageDetector()
      {
        if (this.detector != null)
          return;
        this.detector = new Globalization.OutboundCodePageDetector();
      }

      public int GetBestWindowsCodePage()
      {
        return this.detector.GetBestWindowsCodePage();
      }

      public int GetBestWindowsCodePage(int preferredCodePage)
      {
        return this.detector.GetBestWindowsCodePage(preferredCodePage);
      }

      public void AddText(TextRunType runType, char[] textBuffer, int offset, int count)
      {
        if (this.detector != null && runType != TextRunType.Markup)
          this.detector.AddText(textBuffer, offset, count);
        int num1 = (int) this.CurrentPosition & (int) short.MaxValue;
        int index1 = (int) (this.CurrentPosition >> 15);
        if (this.LastRunType == runType && num1 != 0)
        {
          char[] chArray1 = this.planes[index1];
          int index2 = (int) this.lastRunPosition & (int) short.MaxValue;
          int num2 = Math.Min(Math.Min(count, 4095 - FormatStore.TextStore.LengthFromRunHeader(chArray1[index2])), 32768 - num1);
          if (num2 != 0)
          {
            if (index1 == 0 && num1 + num2 > chArray1.Length)
            {
              char[] chArray2 = new char[Math.Min(Math.Max(this.planes[0].Length * 2, num1 + num2), 32768)];
              Buffer.BlockCopy((Array) this.planes[0], 0, (Array) chArray2, 0, (int) this.CurrentPosition * 2);
              this.planes[0] = chArray1 = chArray2;
            }
            chArray1[index2] = this.MakeTextRunHeader(runType, num2 + FormatStore.TextStore.LengthFromRunHeader(chArray1[index2]));
            Buffer.BlockCopy((Array) textBuffer, offset * 2, (Array) chArray1, num1 * 2, num2 * 2);
            offset += num2;
            count -= num2;
            this.CurrentPosition += (uint) num2;
          }
        }
        while (count != 0)
        {
          int index2 = (int) this.CurrentPosition & (int) short.MaxValue;
          int index3 = (int) (this.CurrentPosition >> 15);
          if (32768 - index2 < 21)
          {
            this.planes[index3][index2] = this.MakeTextRunHeader(TextRunType.Invalid, 32768 - index2 - 1);
            this.CurrentPosition += (uint) (32768 - index2);
          }
          else
          {
            int length = Math.Min(Math.Min(count, 4095), 32768 - index2 - 1);
            if (index3 == 0 && index2 + length + 1 > this.planes[0].Length)
            {
              char[] chArray = new char[Math.Min(Math.Max(this.planes[0].Length * 2, index2 + length + 1), 32768)];
              Buffer.BlockCopy((Array) this.planes[0], 0, (Array) chArray, 0, (int) this.CurrentPosition * 2);
              this.planes[0] = chArray;
            }
            else if (index2 == 0)
            {
              if (index3 == this.planes.Length)
              {
                if (index3 == 640)
                  throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
                char[][] chArray = new char[Math.Min(this.planes.Length * 2, 640)][];
                Array.Copy((Array) this.planes, 0, (Array) chArray, 0, this.planes.Length);
                this.planes = chArray;
              }
              if (this.planes[index3] == null)
                this.planes[index3] = new char[32768];
            }
            this.LastRunType = runType;
            this.lastRunPosition = this.CurrentPosition;
            this.planes[index3][index2] = this.MakeTextRunHeader(runType, length);
            Buffer.BlockCopy((Array) textBuffer, offset * 2, (Array) this.planes[index3], (index2 + 1) * 2, length * 2);
            offset += length;
            count -= length;
            this.CurrentPosition += (uint) (length + 1);
          }
        }
      }

      public void AddSimpleRun(TextRunType runType, int count)
      {
        if (this.LastRunType == runType)
        {
          char[] chArray = this.planes[(this.lastRunPosition >> 15)];
          int index = (int) this.lastRunPosition & (int) short.MaxValue;
          int num = Math.Min(count, 4095 - FormatStore.TextStore.LengthFromRunHeader(chArray[index]));
          if (num != 0)
          {
            chArray[index] = this.MakeTextRunHeader(runType, num + FormatStore.TextStore.LengthFromRunHeader(chArray[index]));
            count -= num;
          }
        }
        if (count == 0)
          return;
        int index1 = (int) this.CurrentPosition & (int) short.MaxValue;
        int index2 = (int) (this.CurrentPosition >> 15);
        if (index1 == 0)
        {
          if (index2 == this.planes.Length)
          {
            if (index2 == 640)
              throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
            char[][] chArray = new char[Math.Min(this.planes.Length * 2, 640)][];
            Array.Copy((Array) this.planes, 0, (Array) chArray, 0, this.planes.Length);
            this.planes = chArray;
          }
          if (this.planes[index2] == null)
            this.planes[index2] = new char[32768];
        }
        else if (index2 == 0 && (long) (this.CurrentPosition + 1U) > (long) this.planes[0].Length)
        {
          char[] chArray = new char[Math.Min(this.planes[0].Length * 2, 32768)];
          Buffer.BlockCopy((Array) this.planes[0], 0, (Array) chArray, 0, (int) this.CurrentPosition * 2);
          this.planes[0] = chArray;
        }
        this.LastRunType = runType;
        this.lastRunPosition = this.CurrentPosition;
        this.planes[index2][index1] = this.MakeTextRunHeader(runType, count);
        ++this.CurrentPosition;
      }

      public void ConvertToInvalid(uint startPosition)
      {
        char[] chArray = this.Plane(startPosition);
        int index = this.Index(startPosition);
        int num = (int) chArray[index] >= 12288 ? 1 : FormatStore.TextStore.LengthFromRunHeader(chArray[index]) + 1;
        chArray[index] = this.MakeTextRunHeader(TextRunType.Invalid, num - 1);
      }

      public void ConvertToInvalid(uint startPosition, int countToConvert)
      {
        char[] chArray = this.Plane(startPosition);
        int index = this.Index(startPosition);
        int num1 = FormatStore.TextStore.LengthFromRunHeader(chArray[index]);
        int length = num1 - countToConvert;
        int num2 = num1 + 1 - (length + 1);
        chArray[index] = this.MakeTextRunHeader(TextRunType.Invalid, num2 - 1);
        chArray[index + num2] = this.MakeTextRunHeader(TextRunType.NonSpace, length);
      }

      public void ConvertShortRun(uint startPosition, TextRunType type, int newEffectiveLength)
      {
        this.Plane(startPosition)[this.Index(startPosition)] = this.MakeTextRunHeader(type, newEffectiveLength);
      }

      public void DoNotMergeNextRun()
      {
        if (this.LastRunType == TextRunType.BlockBoundary)
          return;
        this.LastRunType = TextRunType.Invalid;
      }

      public long DumpStat(TextWriter dumpWriter)
      {
        int num1 = (int) ((uint) ((int) this.CurrentPosition + 32768 - 1) >> 15);
        if (num1 == 0)
          num1 = 1;
        long num2 = num1 == 1 ? (long) this.planes[0].Length : (long) (num1 * 32768);
        long num3 = (long) this.CurrentPosition;
        long num4 = (long) (12 + this.planes.Length * 4 + num1 * 12) + num2 * 2L;
        if (dumpWriter != null)
        {
          dumpWriter.WriteLine("Text alloc: {0}", (object) num2);
          dumpWriter.WriteLine("Text used: {0}", (object) num3);
          dumpWriter.WriteLine("Text bytes: {0}", (object) num4);
        }
        return num4;
      }

      internal static TextRunType TypeFromRunHeader(char runHeader)
      {
        return (TextRunType) ((uint) runHeader & 61440U);
      }

      internal static int LengthFromRunHeader(char runHeader)
      {
        return (int) runHeader & 4095;
      }

      internal char MakeTextRunHeader(TextRunType runType, int length)
      {
        return (char) (runType | (TextRunType) length);
      }
    }
  }
}
