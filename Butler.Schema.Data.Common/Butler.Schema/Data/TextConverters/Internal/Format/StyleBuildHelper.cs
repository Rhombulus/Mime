// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.StyleBuildHelper
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct StyleBuildHelper
  {
    internal FormatStore Store;
    internal PropertyBitMask PropertyMask;
    private StyleBuildHelper.PrecedenceEntry[] entries;
    private int topEntry;
    private int currentEntry;
    private int nonFlagPropertiesCount;

    internal StyleBuildHelper(FormatStore store)
    {
      this.Store = store;
      this.PropertyMask = new PropertyBitMask();
      this.entries = (StyleBuildHelper.PrecedenceEntry[]) null;
      this.topEntry = 0;
      this.currentEntry = -1;
      this.nonFlagPropertiesCount = 0;
    }

    public void Clean()
    {
      this.PropertyMask.ClearAll();
      this.topEntry = 0;
      this.currentEntry = -1;
      this.nonFlagPropertiesCount = 0;
    }

    public void SetProperty(int precedence, PropertyId id, PropertyValue value)
    {
      this.SetPropertyImpl(this.GetEntry(precedence), id, value);
    }

    public void SetProperty(int precedence, Property prop)
    {
      this.SetPropertyImpl(this.GetEntry(precedence), prop.Id, prop.Value);
    }

    public void AddStyle(int precedence, int handle)
    {
      int entry = this.GetEntry(precedence);
      FormatStyle style = this.Store.GetStyle(handle);
      this.entries[entry].FlagProperties.Merge(style.FlagProperties);
      Property[] propertyList = style.PropertyList;
      if (propertyList == null)
        return;
      for (int index = 0; index < propertyList.Length; ++index)
      {
        Property property = propertyList[index];
        if (property.Value.IsRefCountedHandle)
          this.Store.AddRefValue(property.Value);
        this.SetPropertyImpl(entry, property.Id, property.Value);
      }
    }

    public void AddProperties(int precedence, FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] propList)
    {
      int entry = this.GetEntry(precedence);
      this.entries[entry].FlagProperties.Merge(flagProperties);
      if (propList == null)
        return;
      for (int index = 0; index < propList.Length; ++index)
      {
        Property property = propList[index];
        if (propertyMask.IsSet(property.Id))
        {
          if (property.Value.IsRefCountedHandle)
            this.Store.AddRefValue(property.Value);
          this.SetPropertyImpl(entry, property.Id, property.Value);
        }
      }
    }

    public PropertyValue GetProperty(PropertyId id)
    {
      if (FlagProperties.IsFlagProperty(id))
      {
        for (int index = 0; index < this.topEntry; ++index)
        {
          if (this.entries[index].FlagProperties.IsDefined(id))
            return this.entries[index].FlagProperties.GetPropertyValue(id);
        }
      }
      else if (this.PropertyMask.IsSet(id))
      {
        int entryFound;
        int indexFound;
        this.FindProperty(id, out entryFound, out indexFound);
        return this.entries[entryFound].Properties[indexFound].Value;
      }
      return PropertyValue.Null;
    }

    public void GetPropertyList(out Property[] propertyList, out FlagProperties effectiveFlagProperties, out PropertyBitMask effectivePropertyMask)
    {
      effectiveFlagProperties = new FlagProperties();
      effectivePropertyMask = this.PropertyMask;
      for (int index = this.topEntry - 1; index >= 0; --index)
        effectiveFlagProperties.Merge(this.entries[index].FlagProperties);
      if (this.nonFlagPropertiesCount != 0)
      {
        propertyList = new Property[this.nonFlagPropertiesCount];
        if (this.topEntry == 1)
          Array.Copy((Array) this.entries[0].Properties, 0, (Array) propertyList, 0, this.nonFlagPropertiesCount);
        else if (this.topEntry == 2)
        {
          Property[] propertyArray1 = this.entries[0].Properties;
          Property[] propertyArray2 = this.entries[1].Properties;
          int index1 = 0;
          int index2 = 0;
          int num1 = this.entries[0].Count;
          int num2 = this.entries[1].Count;
          int num3 = 0;
          while (true)
          {
            while (index1 >= num1)
            {
              if (index2 < num2)
                propertyList[num3++] = propertyArray2[index2++];
              else
                goto label_17;
            }
            propertyList[num3++] = index2 == num2 || propertyArray1[index1].Id <= propertyArray2[index2].Id ? propertyArray1[index1++] : propertyArray2[index2++];
          }
        }
        else
        {
          PropertyId id = PropertyId.MergedCell;
          int num = 0;
          while (++id < PropertyId.MaxValue)
          {
            if (this.PropertyMask.IsSet(id))
            {
              int entryFound;
              int indexFound;
              this.FindProperty(id, out entryFound, out indexFound);
              propertyList[num++] = this.entries[entryFound].Properties[indexFound];
            }
          }
        }
      }
      else
        propertyList = (Property[]) null;
label_17:
      this.Clean();
    }

    private void InitializeEntry(int entry, int precedence)
    {
      if (this.entries == null)
        this.entries = new StyleBuildHelper.PrecedenceEntry[4];
      else if (entry == this.entries.Length)
      {
        if (entry == 16)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        StyleBuildHelper.PrecedenceEntry[] precedenceEntryArray = new StyleBuildHelper.PrecedenceEntry[this.entries.Length * 2];
        Array.Copy((Array) this.entries, 0, (Array) precedenceEntryArray, 0, this.entries.Length);
        this.entries = precedenceEntryArray;
      }
      if (this.entries[entry] == null)
        this.entries[entry] = new StyleBuildHelper.PrecedenceEntry(precedence);
      else
        this.entries[entry].ReInitialize(precedence);
    }

    private int GetEntry(int precedence)
    {
      int entry = this.currentEntry;
      if (entry != -1)
      {
        if (this.entries[entry].Precedence != precedence)
        {
          int index1 = this.topEntry - 1;
          while (index1 >= 0 && this.entries[index1].Precedence >= precedence)
            --index1;
          entry = index1 + 1;
          if (entry == this.topEntry || this.entries[entry].Precedence != precedence)
          {
            this.InitializeEntry(this.topEntry, precedence);
            if (entry < this.topEntry)
            {
              StyleBuildHelper.PrecedenceEntry precedenceEntry = this.entries[this.topEntry];
              for (int index2 = this.topEntry - 1; index2 >= entry; --index2)
                this.entries[index2 + 1] = this.entries[index2];
              this.entries[entry] = precedenceEntry;
            }
            ++this.topEntry;
          }
        }
      }
      else
      {
        entry = this.topEntry++;
        this.InitializeEntry(entry, precedence);
      }
      this.currentEntry = entry;
      return entry;
    }

    private void AddProperty(int entry, PropertyId id, PropertyValue value)
    {
      int index;
      for (index = this.entries[entry].Count - 1; index >= 0 && this.entries[entry].Properties[index].Id > id; --index)
        this.entries[entry].Properties[index + 1] = this.entries[entry].Properties[index];
      ++this.entries[entry].Count;
      this.entries[entry].Properties[index + 1].Set(id, value);
      this.entries[entry].PropertyMask.Set(id);
      this.PropertyMask.Set(id);
      ++this.nonFlagPropertiesCount;
    }

    private void RemoveProperty(int entry, int index)
    {
      this.entries[entry].PropertyMask.Clear(this.entries[entry].Properties[index].Id);
      if (this.entries[entry].Properties[index].Value.IsRefCountedHandle)
        this.Store.ReleaseValue(this.entries[entry].Properties[index].Value);
      --this.entries[entry].Count;
      for (int index1 = index; index1 < this.entries[entry].Count; ++index1)
        this.entries[entry].Properties[index1] = this.entries[entry].Properties[index1 + 1];
      --this.nonFlagPropertiesCount;
    }

    private void FindProperty(PropertyId id, out int entryFound, out int indexFound)
    {
      entryFound = 0;
      while (entryFound < this.topEntry)
      {
        if (this.entries[entryFound].PropertyMask.IsSet(id))
        {
          int num1 = 0;
          int num2 = this.entries[entryFound].Count;
          Property[] propertyArray = this.entries[entryFound].Properties;
          indexFound = this.entries[entryFound].NextSearchIndex;
          if (indexFound < num2)
          {
            if (propertyArray[indexFound].Id == id)
            {
              ++this.entries[entryFound].NextSearchIndex;
              return;
            }
            if (propertyArray[indexFound].Id < id)
              num1 = indexFound + 1;
          }
          indexFound = num1;
          while (indexFound < num2)
          {
            if (propertyArray[indexFound].Id == id)
            {
              this.entries[entryFound].NextSearchIndex = indexFound + 1;
              return;
            }
            ++indexFound;
          }
        }
        ++entryFound;
      }
      indexFound = -1;
    }

    private void SetPropertyImpl(int entry, PropertyId id, PropertyValue value)
    {
      if (FlagProperties.IsFlagProperty(id))
      {
        if (!value.IsNull)
          this.entries[entry].FlagProperties.Set(id, value.Bool);
        else
          this.entries[entry].FlagProperties.Remove(id);
      }
      else
      {
        if (this.PropertyMask.IsSet(id))
        {
          int entryFound;
          int indexFound;
          this.FindProperty(id, out entryFound, out indexFound);
          if (entryFound < entry)
            return;
          if (entryFound == entry)
          {
            if (this.entries[entry].Properties[indexFound].Value.IsRefCountedHandle)
              this.Store.ReleaseValue(this.entries[entry].Properties[indexFound].Value);
            else if (this.entries[entry].Properties[indexFound].Value.IsRelativeHtmlFontUnits && value.IsRelativeHtmlFontUnits)
              value = new PropertyValue(PropertyType.RelHtmlFontUnits, this.entries[entry].Properties[indexFound].Value.RelativeHtmlFontUnits + value.RelativeHtmlFontUnits);
            this.entries[entry].Properties[indexFound].Value = value;
            return;
          }
          this.RemoveProperty(entryFound, indexFound);
        }
        if (value.IsNull)
          return;
        this.AddProperty(entry, id, value);
      }
    }

    private class PrecedenceEntry
    {
      public int Precedence;
      public FlagProperties FlagProperties;
      public PropertyBitMask PropertyMask;
      public Property[] Properties;
      public int Count;
      public int NextSearchIndex;

      public PrecedenceEntry(int precedence)
      {
        this.Precedence = precedence;
        this.Properties = new Property[71];
      }

      public void ReInitialize(int precedence)
      {
        this.Precedence = precedence;
        this.FlagProperties.ClearAll();
        this.PropertyMask.ClearAll();
        this.Count = 0;
        this.NextSearchIndex = 0;
      }
    }
  }
}
