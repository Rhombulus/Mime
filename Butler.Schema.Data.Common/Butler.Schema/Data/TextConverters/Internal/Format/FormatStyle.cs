// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.FormatStyle
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct FormatStyle
  {
    public static readonly FormatStyle Null = new FormatStyle();
    internal FormatStore.StyleStore Styles;
    internal int StyleHandle;

    public int Handle
    {
      get
      {
        return this.StyleHandle;
      }
    }

    public bool IsNull
    {
      get
      {
        return this.StyleHandle == 0;
      }
    }

    public bool IsEmpty
    {
      get
      {
        if (!this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyMask.IsClear)
          return false;
        if (this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList != null)
          return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList.Length == 0;
        return true;
      }
    }

    public FlagProperties FlagProperties
    {
      get
      {
        return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].FlagProperties;
      }
      set
      {
        this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].FlagProperties = value;
      }
    }

    public PropertyBitMask PropertyMask
    {
      get
      {
        return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyMask;
      }
      set
      {
        this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyMask = value;
      }
    }

    public Property[] PropertyList
    {
      get
      {
        return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList;
      }
      set
      {
        this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList = value;
      }
    }

    internal int RefCount
    {
      get
      {
        return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount;
      }
    }

    internal FormatStyle(FormatStore store, int styleHandle)
    {
      this.Styles = store.Styles;
      this.StyleHandle = styleHandle;
    }

    internal FormatStyle(FormatStore.StyleStore styles, int styleHandle)
    {
      this.Styles = styles;
      this.StyleHandle = styleHandle;
    }

    public void AddRef()
    {
      if (this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount == int.MaxValue)
        return;
      ++this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount;
    }

    public void Release()
    {
      if (this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount != int.MaxValue && --this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount == 0)
        this.Styles.Free(this.StyleHandle);
      this.StyleHandle = -1;
    }
  }
}
