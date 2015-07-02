// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefBitConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  internal static class TnefBitConverter
  {
    private static bool niceLittleEndianGuidLayout = TnefBitConverter.InitNiceLittleEndianGuidLayout();

    public static void GetBytes(byte[] buffer, int offset, short value)
    {
      buffer[offset] = (byte) ((uint) value & (uint) byte.MaxValue);
      buffer[offset + 1] = (byte) (((int) value & 65280) >> 8);
    }

    public static unsafe void GetBytes(byte[] buffer, int offset, int value)
    {
      fixed (byte* numPtr1 = buffer)
      {
        if (BitConverter.IsLittleEndian)
        {
          *(int*) (numPtr1 + offset) = value;
        }
        else
        {
          byte* numPtr2 = numPtr1 + offset;
          byte* numPtr3 = (byte*) &value;
          *numPtr2 = numPtr3[3];
          numPtr2[1] = numPtr3[2];
          numPtr2[2] = numPtr3[1];
          numPtr2[3] = *numPtr3;
        }
      }
    }

    public static unsafe void GetBytes(byte[] buffer, int offset, long value)
    {
      fixed (byte* numPtr1 = buffer)
      {
        if (BitConverter.IsLittleEndian)
        {
          *(long*) (numPtr1 + offset) = value;
        }
        else
        {
          byte* numPtr2 = numPtr1 + offset;
          byte* numPtr3 = (byte*) &value;
          *numPtr2 = numPtr3[7];
          numPtr2[1] = numPtr3[6];
          numPtr2[2] = numPtr3[5];
          numPtr2[3] = numPtr3[4];
          numPtr2[4] = numPtr3[3];
          numPtr2[5] = numPtr3[2];
          numPtr2[6] = numPtr3[1];
          numPtr2[7] = *numPtr3;
        }
      }
    }

    public static unsafe void GetBytes(byte[] buffer, int offset, float value)
    {
      fixed (byte* numPtr1 = buffer)
      {
        if (BitConverter.IsLittleEndian)
        {
          *(float*) (numPtr1 + offset) = value;
        }
        else
        {
          byte* numPtr2 = numPtr1 + offset;
          byte* numPtr3 = (byte*) &value;
          *numPtr2 = numPtr3[3];
          numPtr2[1] = numPtr3[2];
          numPtr2[2] = numPtr3[1];
          numPtr2[3] = *numPtr3;
        }
      }
    }

    public static unsafe void GetBytes(byte[] buffer, int offset, double value)
    {
      fixed (byte* numPtr1 = buffer)
      {
        if (BitConverter.IsLittleEndian)
        {
          *(double*) (numPtr1 + offset) = value;
        }
        else
        {
          byte* numPtr2 = numPtr1 + offset;
          byte* numPtr3 = (byte*) &value;
          *numPtr2 = numPtr3[7];
          numPtr2[1] = numPtr3[6];
          numPtr2[2] = numPtr3[5];
          numPtr2[3] = numPtr3[4];
          numPtr2[4] = numPtr3[3];
          numPtr2[5] = numPtr3[2];
          numPtr2[6] = numPtr3[1];
          numPtr2[7] = *numPtr3;
        }
      }
    }

    private static unsafe bool InitNiceLittleEndianGuidLayout()
    {
      Guid guid = new Guid(1732584193, (short) -21623, (short) -4147, (byte) 254, (byte) 220, (byte) 186, (byte) 152, (byte) 118, (byte) 84, (byte) 50, (byte) 16);
      byte[] numArray1 = new byte[16]
      {
        (byte) 1,
        (byte) 35,
        (byte) 69,
        (byte) 103,
        (byte) 137,
        (byte) 171,
        (byte) 205,
        (byte) 239,
        (byte) 254,
        (byte) 220,
        (byte) 186,
        (byte) 152,
        (byte) 118,
        (byte) 84,
        (byte) 50,
        (byte) 16
      };
      if (Marshal.SizeOf(typeof (Guid)) == 16)
      {
        fixed (byte* numPtr1 = numArray1)
        {
          byte* numPtr2 = numPtr1;
          byte* numPtr3 = numPtr2 + 16;
          for (byte* numPtr4 = (byte*) &guid; numPtr2 != numPtr3 && (int) *numPtr2 == (int) *numPtr4; ++numPtr4)
            ++numPtr2;
          if (numPtr2 == numPtr3)
            return true;
        }
      }
      byte[] numArray2 = guid.ToByteArray();
      int index = 0;
      if (numArray2.Length == 16)
      {
        while (index < 16 && (int) numArray2[index] == (int) numArray1[index])
          ++index;
      }
      if (index != 16)
        throw new NotSupportedException("The application cannot run on this platform, Guid format is incompatible.");
      return false;
    }

    public static unsafe void GetBytes(byte[] buffer, int offset, Guid value)
    {
      if (TnefBitConverter.niceLittleEndianGuidLayout)
      {
        fixed (byte* numPtr = buffer)
          *(Guid*) (numPtr + offset) = value;
      }
      else
        Buffer.BlockCopy((Array) value.ToByteArray(), 0, (Array) buffer, offset, 16);
    }
  }
}
