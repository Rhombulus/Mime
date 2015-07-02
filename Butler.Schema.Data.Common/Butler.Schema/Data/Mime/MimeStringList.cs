// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeStringList
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Mime
{
  internal struct MimeStringList
  {
    public static readonly MimeStringList Empty = new MimeStringList();
    public const uint MaskAny = 4026531840U;
    public const uint MaskRawOnly = 268435456U;
    public const uint MaskJis = 536870912U;
    private MimeString first;
    private MimeStringList.ListEntry[] overflow;

    public int Count
    {
      get
      {
        if (this.overflow != null)
          return this.overflow[0].HeaderCount;
        return this.first.Data == null ? 0 : 1;
      }
    }

    public int Length
    {
      get
      {
        if (this.overflow == null)
          return this.first.Length;
        return this.overflow[0].HeaderTotalLength;
      }
    }

    public MimeString this[int index]
    {
      get
      {
        if (index == 0)
          return this.first;
        if (index < 4096)
          return this.overflow[index].Str;
        return this.overflow[4096 + index / 4096 - 1].Secondary[index % 4096];
      }
      set
      {
        if (this.overflow != null)
          this.overflow[0].HeaderTotalLength += value.Length - this[index].Length;
        if (index == 0)
          this.first = value;
        else if (index < 4096)
          this.overflow[index].Str = value;
        else
          this.overflow[4096 + index / 4096 - 1].Secondary[index % 4096] = value;
      }
    }

    public MimeStringList(byte[] data)
    {
      this.first = new MimeString(data, 0, data.Length);
      this.overflow = (MimeStringList.ListEntry[]) null;
    }

    public MimeStringList(byte[] data, int offset, int count)
    {
      this.first = new MimeString(data, offset, count);
      this.overflow = (MimeStringList.ListEntry[]) null;
    }

    public MimeStringList(MimeString str)
    {
      this.first = str;
      this.overflow = (MimeStringList.ListEntry[]) null;
    }

    public void Append(MimeString value)
    {
      if (value.Length == 0)
        return;
      int count = this.Count;
      if (count == 0)
      {
        this.first = value;
        if (this.overflow == null)
          return;
        ++this.overflow[0].HeaderCount;
        this.overflow[0].HeaderTotalLength += value.Length;
      }
      else if (count < 4096)
      {
        if (this.overflow == null)
        {
          this.overflow = new MimeStringList.ListEntry[8];
          this.overflow[0].HeaderCount = 1;
          this.overflow[0].HeaderTotalLength = this.first.Length;
        }
        else if (count == this.overflow.Length)
        {
          int length = count * 2;
          if (length >= 4096)
            length = 4128;
          MimeStringList.ListEntry[] listEntryArray = new MimeStringList.ListEntry[length];
          Array.Copy((Array) this.overflow, 0, (Array) listEntryArray, 0, this.overflow.Length);
          this.overflow = listEntryArray;
        }
        this.overflow[count].Str = value;
        ++this.overflow[0].HeaderCount;
        this.overflow[0].HeaderTotalLength += value.Length;
      }
      else
      {
        int index1 = 4096 + count / 4096 - 1;
        int index2 = count % 4096;
        if (index1 >= this.overflow.Length)
          throw new MimeException("MIME is too complex (header value is too long)");
        if (this.overflow[index1].Secondary == null)
          this.overflow[index1].Secondary = new MimeString[4096];
        this.overflow[index1].Secondary[index2] = value;
        ++this.overflow[0].HeaderCount;
        this.overflow[0].HeaderTotalLength += value.Length;
      }
    }

    public void TakeOver(ref MimeStringList list)
    {
      this.TakeOver(ref list, 4026531840U);
    }

    public void TakeOver(ref MimeStringList list, uint mask)
    {
      if ((int) mask == -268435456)
      {
        this.first = list.first;
        this.overflow = list.overflow;
        list.first = new MimeString();
        list.overflow = (MimeStringList.ListEntry[]) null;
      }
      else
      {
        this.Reset();
        this.TakeOverAppend(ref list, mask);
      }
    }

    public void TakeOverAppend(ref MimeStringList list)
    {
      this.TakeOverAppend(ref list, 4026531840U);
    }

    public void TakeOverAppend(ref MimeStringList list, uint mask)
    {
      if (this.Count == 0 && (int) mask == -268435456)
      {
        this.TakeOver(ref list, mask);
      }
      else
      {
        for (int index = 0; index < list.Count; ++index)
        {
          MimeString refLine = list[index];
          if ((int) mask == -268435456 || ((int) refLine.Mask & (int) mask) != 0)
            this.AppendFragment(refLine);
        }
        list.Reset();
      }
    }

    public void AppendFragment(MimeString refLine)
    {
      int count = this.Count;
      if (count != 0)
      {
        int index = count - 1;
        if (index == 0)
        {
          if (this.first.MergeIfAdjacent(refLine))
          {
            if (this.overflow == null)
              return;
            this.overflow[0].HeaderTotalLength += refLine.Length;
            return;
          }
        }
        else if (index < 4096)
        {
          if (this.overflow[index].StrMergeIfAdjacent(refLine))
          {
            this.overflow[0].HeaderTotalLength += refLine.Length;
            return;
          }
        }
        else if (this.overflow[4096 + index / 4096 - 1].Secondary[index % 4096].MergeIfAdjacent(refLine))
        {
          this.overflow[0].HeaderTotalLength += refLine.Length;
          return;
        }
      }
      this.Append(refLine);
    }

    public int GetLength(uint mask)
    {
      if ((int) mask == -268435456)
        return this.Length;
      int num = 0;
      for (int index = 0; index < this.Count; ++index)
      {
        MimeString mimeString = this[index];
        if (((int) mimeString.Mask & (int) mask) != 0)
          num += mimeString.Length;
      }
      return num;
    }

    public byte[] GetSz()
    {
      int count = this.Count;
      if (count <= 1)
        return this.first.GetSz();
      byte[] destination = new byte[this.Length];
      int destinationIndex = 0;
      for (int index = 0; index < count; ++index)
        destinationIndex += this[index].CopyTo(destination, destinationIndex);
      return destination;
    }

    public byte[] GetSz(uint mask)
    {
      if ((int) mask == -268435456)
        return this.GetSz();
      int count = this.Count;
      switch (count)
      {
        case 0:
          return (byte[]) null;
        case 1:
          if (((int) this.first.Mask & (int) mask) == 0)
            return MimeString.EmptyByteArray;
          return this.first.GetSz();
        default:
          byte[] destination = new byte[this.GetLength(mask)];
          int destinationIndex = 0;
          for (int index = 0; index < count; ++index)
          {
            MimeString mimeString = this[index];
            if (((int) mimeString.Mask & (int) mask) != 0)
              destinationIndex += mimeString.CopyTo(destination, destinationIndex);
          }
          return destination;
      }
    }

    public void WriteTo(Stream stream)
    {
      for (int index = 0; index < this.Count; ++index)
        this[index].WriteTo(stream);
    }

    public override string ToString()
    {
      int count = this.Count;
      if (count <= 1)
        return this.first.ToString();
      StringBuilder stringBuilder = Internal.ScratchPad.GetStringBuilder(this.Length);
      for (int index = 0; index < count; ++index)
      {
        MimeString mimeString = this[index];
        string str = Internal.ByteString.BytesToString(mimeString.Data, mimeString.Offset, mimeString.Length, true);
        stringBuilder.Append(str);
      }
      Internal.ScratchPad.ReleaseStringBuilder();
      return stringBuilder.ToString();
    }

    public MimeStringList Clone()
    {
      MimeStringList mimeStringList = new MimeStringList();
      mimeStringList.first = this.first;
      if (this.overflow != null && this.overflow[0].HeaderCount > 1)
      {
        mimeStringList.overflow = new MimeStringList.ListEntry[this.overflow.Length];
        int length1 = Math.Min(this.Count, 4096);
        Array.Copy((Array) this.overflow, 0, (Array) mimeStringList.overflow, 0, length1);
        if (this.Count > 4096)
        {
          int index = 4096;
          int num = 4096;
          while (num < this.Count)
          {
            mimeStringList.overflow[index].Secondary = new MimeString[4096];
            int length2 = Math.Min(this.Count - num, 4096);
            Array.Copy((Array) this.overflow[index].Secondary, 0, (Array) mimeStringList.overflow[index].Secondary, 0, length2);
            num += 4096;
            ++index;
          }
        }
      }
      return mimeStringList;
    }

    public void Reset()
    {
      this.first = new MimeString();
      if (this.overflow == null)
        return;
      this.overflow[0].Reset();
    }

    private struct ListEntry
    {
      private object obj;
      private int int1;
      private int int2;

      public int HeaderCount
      {
        get
        {
          return this.int1;
        }
        set
        {
          this.int1 = value;
        }
      }

      public int HeaderTotalLength
      {
        get
        {
          return this.int2;
        }
        set
        {
          this.int2 = value;
        }
      }

      public MimeString Str
      {
        get
        {
          return new MimeString((byte[]) this.obj, this.int1, (uint) this.int2);
        }
        set
        {
          this.obj = (object) value.Data;
          this.int1 = value.Offset;
          this.int2 = (int) value.LengthAndMask;
        }
      }

      public MimeString[] Secondary
      {
        get
        {
          return (MimeString[]) this.obj;
        }
        set
        {
          this.obj = (object) value;
          this.int1 = 0;
          this.int2 = 0;
        }
      }

      public void Reset()
      {
        this.obj = (object) null;
        this.int1 = 0;
        this.int2 = 0;
      }

      public bool StrMergeIfAdjacent(MimeString refLine)
      {
        MimeString str = this.Str;
        if (!str.MergeIfAdjacent(refLine))
          return false;
        this.Str = str;
        return true;
      }
    }
  }
}
