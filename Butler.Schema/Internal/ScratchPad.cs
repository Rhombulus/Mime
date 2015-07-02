// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.ScratchPad
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.Internal
{
  internal static class ScratchPad
  {
    [ThreadStatic]
    private static ScratchPad.ScratchPadContainer pad;

    public static void Begin()
    {
      if (ScratchPad.pad == null)
        ScratchPad.pad = new ScratchPad.ScratchPadContainer();
      else
        ScratchPad.pad.AddRef();
    }

    public static void End()
    {
      if (ScratchPad.pad == null || !ScratchPad.pad.Release())
        return;
      ScratchPad.pad = (ScratchPad.ScratchPadContainer) null;
    }

    public static byte[] GetByteBuffer(int size)
    {
      if (ScratchPad.pad == null)
        return new byte[size];
      return ScratchPad.pad.GetByteBuffer(size);
    }

    [Conditional("DEBUG")]
    public static void ReleaseByteBuffer()
    {
      if (ScratchPad.pad == null)
        return;
      ScratchPad.pad.ReleaseByteBuffer();
    }

    public static char[] GetCharBuffer(int size)
    {
      if (ScratchPad.pad == null)
        return new char[size];
      return ScratchPad.pad.GetCharBuffer(size);
    }

    [Conditional("DEBUG")]
    public static void ReleaseCharBuffer()
    {
      if (ScratchPad.pad == null)
        return;
      ScratchPad.pad.ReleaseCharBuffer();
    }

    public static StringBuilder GetStringBuilder()
    {
      return ScratchPad.GetStringBuilder(16);
    }

    public static StringBuilder GetStringBuilder(int initialCapacity)
    {
      if (ScratchPad.pad == null)
        return new StringBuilder(initialCapacity);
      return ScratchPad.pad.GetStringBuilder(initialCapacity);
    }

    public static void ReleaseStringBuilder()
    {
      if (ScratchPad.pad == null)
        return;
      ScratchPad.pad.ReleaseStringBuilder();
    }

    private class ScratchPadContainer
    {
      public const int ScratchStringBuilderCapacity = 512;
      private int refCount;
      private byte[] byteBuffer;
      private char[] charBuffer;
      private StringBuilder stringBuilder;

      public ScratchPadContainer()
      {
        this.refCount = 1;
      }

      public void AddRef()
      {
        ++this.refCount;
      }

      public bool Release()
      {
        --this.refCount;
        return this.refCount == 0;
      }

      public byte[] GetByteBuffer(int size)
      {
        if (this.byteBuffer == null || this.byteBuffer.Length < size)
          this.byteBuffer = new byte[size];
        return this.byteBuffer;
      }

      public void ReleaseByteBuffer()
      {
      }

      public char[] GetCharBuffer(int size)
      {
        if (this.charBuffer == null || this.charBuffer.Length < size)
          this.charBuffer = new char[size];
        return this.charBuffer;
      }

      public void ReleaseCharBuffer()
      {
      }

      public StringBuilder GetStringBuilder(int initialCapacity)
      {
        if (initialCapacity > 512)
          return new StringBuilder(initialCapacity);
        if (this.stringBuilder == null)
          this.stringBuilder = new StringBuilder(512);
        else
          this.stringBuilder.Length = 0;
        return this.stringBuilder;
      }

      public void ReleaseStringBuilder()
      {
        if (this.stringBuilder == null || this.stringBuilder.Capacity <= 512 && this.stringBuilder.Length * 2 < this.stringBuilder.Capacity + 1)
          return;
        this.stringBuilder = (StringBuilder) null;
      }
    }
  }
}
