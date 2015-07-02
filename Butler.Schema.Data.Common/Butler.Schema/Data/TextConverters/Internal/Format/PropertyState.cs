// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.PropertyState
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal class PropertyState
  {
    private PropertyValue[] properties = new PropertyValue[54];
    private PropertyState.PropertyUndoEntry[] propertyUndoStack = new PropertyState.PropertyUndoEntry[142];
    private const int MaxStackSize = 8960;
    private FlagProperties flagProperties;
    private FlagProperties distinctFlagProperties;
    private PropertyBitMask propertyMask;
    private PropertyBitMask distinctPropertyMask;
    private int propertyUndoStackTop;

    public int UndoStackTop
    {
      get
      {
        return this.propertyUndoStackTop;
      }
    }

    public FlagProperties GetEffectiveFlags()
    {
      return this.flagProperties;
    }

    public FlagProperties GetDistinctFlags()
    {
      return this.distinctFlagProperties;
    }

    public PropertyValue GetEffectiveProperty(PropertyId id)
    {
      if (FlagProperties.IsFlagProperty(id))
        return this.flagProperties.GetPropertyValue(id);
      if (this.propertyMask.IsSet(id))
        return this.properties[(int) (id - (byte) 17)];
      return PropertyValue.Null;
    }

    public PropertyValue GetDistinctProperty(PropertyId id)
    {
      if (FlagProperties.IsFlagProperty(id))
        return this.distinctFlagProperties.GetPropertyValue(id);
      if (this.distinctPropertyMask.IsSet(id))
        return this.properties[(int) (id - (byte) 17)];
      return PropertyValue.Null;
    }

    public void SubtractDefaultFromDistinct(FlagProperties defaultFlags, Property[] defaultProperties)
    {
      FlagProperties flagProperties = this.distinctFlagProperties & (defaultFlags ^ this.distinctFlagProperties) | this.distinctFlagProperties & ~defaultFlags;
      if (this.distinctFlagProperties != flagProperties)
      {
        this.PushUndoEntry((PropertyId) 72, this.distinctFlagProperties);
        this.distinctFlagProperties = flagProperties;
      }
      if (defaultProperties == null)
        return;
      bool flag = false;
      foreach (Property property in defaultProperties)
      {
        if (this.distinctPropertyMask.IsSet(property.Id) && this.properties[(int) (property.Id - (byte) 17)] == property.Value)
        {
          if (!flag)
          {
            this.PushUndoEntry(this.distinctPropertyMask);
            flag = true;
          }
          this.distinctPropertyMask.Clear(property.Id);
        }
      }
    }

    public int ApplyProperties(FlagProperties flagProperties, Property[] propList, FlagProperties flagInheritanceMask, PropertyBitMask propertyInheritanceMask)
    {
      int num = this.propertyUndoStackTop;
      FlagProperties flagProperties1 = this.flagProperties & flagInheritanceMask;
      FlagProperties flagProperties2 = flagProperties1 | flagProperties;
      if (flagProperties2 != this.flagProperties)
      {
        this.PushUndoEntry(PropertyId.MaxValue, this.flagProperties);
        this.flagProperties = flagProperties2;
      }
      FlagProperties flagProperties3 = flagProperties1 ^ flagProperties;
      FlagProperties flagProperties4 = flagProperties & flagProperties3 | flagProperties & ~flagProperties1;
      if (flagProperties4 != this.distinctFlagProperties)
      {
        this.PushUndoEntry((PropertyId) 72, this.distinctFlagProperties);
        this.distinctFlagProperties = flagProperties4;
      }
      PropertyBitMask propertyBitMask1 = this.propertyMask & ~propertyInheritanceMask;
      foreach (PropertyId id in propertyBitMask1)
        this.PushUndoEntry(id, this.properties[(int) (id - (byte) 17)]);
      PropertyBitMask propertyBitMask2 = PropertyBitMask.AllOff;
      this.propertyMask &= propertyInheritanceMask;
      if (propList != null)
      {
        foreach (Property property in propList)
        {
          if (this.propertyMask.IsSet(property.Id))
          {
            if (this.properties[(int) (property.Id - (byte) 17)] != property.Value)
            {
              this.PushUndoEntry(property.Id, this.properties[(int) (property.Id - (byte) 17)]);
              if (property.Value.IsNull)
              {
                this.propertyMask.Clear(property.Id);
              }
              else
              {
                this.properties[(int) (property.Id - (byte) 17)] = property.Value;
                propertyBitMask2.Set(property.Id);
              }
            }
          }
          else if (!property.Value.IsNull)
          {
            if (!propertyBitMask1.IsSet(property.Id))
              this.PushUndoEntry(property.Id, PropertyValue.Null);
            this.properties[(int) (property.Id - (byte) 17)] = property.Value;
            this.propertyMask.Set(property.Id);
            propertyBitMask2.Set(property.Id);
          }
        }
      }
      if (propertyBitMask2 != this.distinctPropertyMask)
      {
        this.PushUndoEntry(this.distinctPropertyMask);
        this.distinctPropertyMask = propertyBitMask2;
      }
      return num;
    }

    public void UndoProperties(int undoLevel)
    {
      for (int index = this.propertyUndoStackTop - 1; index >= undoLevel; --index)
      {
        if (this.propertyUndoStack[index].IsFlags)
          this.flagProperties = this.propertyUndoStack[index].Flags.Flags;
        else if (this.propertyUndoStack[index].IsDistinctFlags)
          this.distinctFlagProperties = this.propertyUndoStack[index].Flags.Flags;
        else if (this.propertyUndoStack[index].IsDistinctMask1)
          this.distinctPropertyMask.Set1(this.propertyUndoStack[index].Bits.Bits);
        else if (this.propertyUndoStack[index].IsDistinctMask2)
          this.distinctPropertyMask.Set2(this.propertyUndoStack[index].Bits.Bits);
        else if (this.propertyUndoStack[index].Property.Value.IsNull)
        {
          this.propertyMask.Clear(this.propertyUndoStack[index].Property.Id);
        }
        else
        {
          this.properties[(int) (this.propertyUndoStack[index].Property.Id - (byte) 17)] = this.propertyUndoStack[index].Property.Value;
          this.propertyMask.Set(this.propertyUndoStack[index].Property.Id);
        }
      }
      this.propertyUndoStackTop = undoLevel;
    }

    public override string ToString()
    {
      return "flags: (" + this.flagProperties.ToString() + "), props: (" + this.propertyMask.ToString() + "), dflags: (" + this.distinctFlagProperties.ToString() + "), dprops: (" + this.distinctPropertyMask.ToString() + ")";
    }

    private void PushUndoEntry(PropertyId id, PropertyValue value)
    {
      if (this.propertyUndoStackTop == this.propertyUndoStack.Length)
      {
        if (this.propertyUndoStack.Length >= 8960)
          throw new TextConvertersException("property undo stack is too large");
        PropertyState.PropertyUndoEntry[] propertyUndoEntryArray = new PropertyState.PropertyUndoEntry[Math.Min(this.propertyUndoStack.Length * 2, 8960)];
        Array.Copy((Array) this.propertyUndoStack, 0, (Array) propertyUndoEntryArray, 0, this.propertyUndoStackTop);
        this.propertyUndoStack = propertyUndoEntryArray;
      }
      this.propertyUndoStack[this.propertyUndoStackTop++].Set(id, value);
    }

    private void PushUndoEntry(PropertyId fakePropId, FlagProperties flagProperties)
    {
      if (this.propertyUndoStackTop == this.propertyUndoStack.Length)
      {
        if (this.propertyUndoStack.Length >= 8960)
          throw new TextConvertersException("property undo stack is too large");
        PropertyState.PropertyUndoEntry[] propertyUndoEntryArray = new PropertyState.PropertyUndoEntry[Math.Min(this.propertyUndoStack.Length * 2, 8960)];
        Array.Copy((Array) this.propertyUndoStack, 0, (Array) propertyUndoEntryArray, 0, this.propertyUndoStackTop);
        this.propertyUndoStack = propertyUndoEntryArray;
      }
      this.propertyUndoStack[this.propertyUndoStackTop++].Set(fakePropId, flagProperties);
    }

    private void PushUndoEntry(PropertyBitMask propertyMask)
    {
      if (this.propertyUndoStackTop + 1 >= this.propertyUndoStack.Length)
      {
        if (this.propertyUndoStackTop + 2 >= 8960)
          throw new TextConvertersException("property undo stack is too large");
        PropertyState.PropertyUndoEntry[] propertyUndoEntryArray = new PropertyState.PropertyUndoEntry[Math.Min(this.propertyUndoStack.Length * 2, 8960)];
        Array.Copy((Array) this.propertyUndoStack, 0, (Array) propertyUndoEntryArray, 0, this.propertyUndoStackTop);
        this.propertyUndoStack = propertyUndoEntryArray;
      }
      this.propertyUndoStack[this.propertyUndoStackTop++].Set((PropertyId) 73, propertyMask.Bits1);
      this.propertyUndoStack[this.propertyUndoStackTop++].Set((PropertyId) 74, propertyMask.Bits2);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct FlagPropertiesUndo
    {
      public PropertyId FakeId;
      public FlagProperties Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct BitsUndo
    {
      public PropertyId FakeId;
      public uint Bits;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    private struct PropertyUndoEntry
    {
      public const PropertyId FlagPropertiesFakeId = PropertyId.MaxValue;
      public const PropertyId DistinctFlagPropertiesFakeId = (PropertyId) 72;
      public const PropertyId DistinctMask1FakeId = (PropertyId) 73;
      public const PropertyId DistinctMask2FakeId = (PropertyId) 74;
      [FieldOffset(0)]
      public Property Property;
      [FieldOffset(0)]
      public PropertyState.FlagPropertiesUndo Flags;
      [FieldOffset(0)]
      public PropertyState.BitsUndo Bits;

      public bool IsFlags
      {
        get
        {
          return this.Property.Id == PropertyId.MaxValue;
        }
      }

      public bool IsDistinctFlags
      {
        get
        {
          return this.Property.Id == (PropertyId) 72;
        }
      }

      public bool IsDistinctMask1
      {
        get
        {
          return this.Property.Id == (PropertyId) 73;
        }
      }

      public bool IsDistinctMask2
      {
        get
        {
          return this.Property.Id == (PropertyId) 74;
        }
      }

      public void Set(PropertyId id, PropertyValue value)
      {
        this.Property.Set(id, value);
      }

      public void Set(PropertyId fakePropId, FlagProperties flagProperties)
      {
        this.Flags.FakeId = fakePropId;
        this.Flags.Flags = flagProperties;
      }

      public void Set(PropertyId fakePropId, uint bits)
      {
        this.Bits.FakeId = fakePropId;
        this.Bits.Bits = bits;
      }
    }
  }
}
