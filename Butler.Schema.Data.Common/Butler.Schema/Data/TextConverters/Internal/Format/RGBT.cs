// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.RGBT
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct RGBT
  {

      public uint RawValue { get; }

      public uint RGB => this.RawValue & 16777215U;

      public bool IsTransparent => (int) this.Transparency == 7;

      public bool IsOpaque => (int) this.Transparency == 0;

      public uint Transparency => this.RawValue >> 24 & 7U;

      public uint Red => this.RawValue >> 16 & (uint) byte.MaxValue;

      public uint Green => this.RawValue >> 8 & (uint) byte.MaxValue;

      public uint Blue => this.RawValue & (uint) byte.MaxValue;

      public float RedPercentage => (float) (this.RawValue >> 16 & (uint) byte.MaxValue) * 0.3921569f;

      public float GreenPercentage => (float) (this.RawValue >> 8 & (uint) byte.MaxValue) * 0.3921569f;

      public float BluePercentage => (float) (this.RawValue & (uint) byte.MaxValue) * 0.3921569f;

      public float TransparencyPercentage => (float) (this.RawValue >> 24 & 7U) * 14.28571f;

      public RGBT(uint rawValue)
    {
      this.RawValue = rawValue;
    }

    public RGBT(uint red, uint green, uint blue)
    {
      this.RawValue = (uint) ((int) red << 16 | (int) green << 8) | blue;
    }

    public RGBT(float redPercentage, float greenPercentage, float bluePercentage)
    {
      this.RawValue = (uint) ((int) (uint) ((double) redPercentage * (double) byte.MaxValue / 100.0) << 16 | (int) (uint) ((double) greenPercentage * (double) byte.MaxValue / 100.0) << 8) | (uint) ((double) bluePercentage * (double) byte.MaxValue / 100.0);
    }

    public RGBT(uint red, uint green, uint blue, uint transparency)
    {
      this.RawValue = (uint) ((int) red << 16 | (int) green << 8 | (int) blue | (int) transparency << 24);
    }

    public RGBT(float redPercentage, float greenPercentage, float bluePercentage, float transparencyPercentage)
    {
      this.RawValue = (uint) ((int) (uint) ((double) redPercentage * (double) byte.MaxValue / 100.0) << 16 | (int) (uint) ((double) greenPercentage * (double) byte.MaxValue / 100.0) << 8) | (uint) ((double) bluePercentage * (double) byte.MaxValue / 100.0) | (uint) ((double) transparencyPercentage * 7.0 / 100.0);
    }

    public override string ToString()
    {
      if (this.IsTransparent)
        return "transparent";
      return "rgb(" + this.Red.ToString() + ", " + this.Green.ToString() + ", " + this.Blue.ToString() + ")" + ((int) this.Transparency != 0 ? "+t" + this.Transparency.ToString() : string.Empty);
    }
  }
}
