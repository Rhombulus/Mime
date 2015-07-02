// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.PropertyValue
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct PropertyValue
  {
    public static readonly PropertyValue Null = new PropertyValue();
    public static readonly PropertyValue True = new PropertyValue(true);
    public static readonly PropertyValue False = new PropertyValue(false);
    private static readonly int[] sizesInTwips = new int[7]
    {
      151,
      200,
      240,
      271,
      360,
      480,
      720
    };
    private static readonly int[] maxSizesInTwips = new int[6]
    {
      160,
      220,
      260,
      320,
      420,
      620
    };
    public const int ValueMax = 67108863;
    public const int ValueMin = -67108863;
    private const uint TypeMask = 4160749568U;
    private const int TypeShift = 27;
    private const uint ValueMask = 134217727U;
    private const int ValueShift = 5;

      public uint RawValue { get; }

      public uint RawType => this.RawValue & 4160749568U;

      public PropertyType Type => (PropertyType) ((this.RawValue & 4160749568U) >> 27);

      public int Value => ((int) this.RawValue & 134217727) << 5 >> 5;

      public uint UnsignedValue => this.RawValue & 134217727U;

      public bool IsNull => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Null);

      public bool IsCalculated => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Calculated);

      public bool IsBool => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Bool);

      public bool IsEnum => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Enum);

      public bool IsString => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.String);

      public bool IsMultiValue => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.MultiValue);

      public bool IsRefCountedHandle
    {
      get
      {
        if (!this.IsString)
          return this.IsMultiValue;
        return true;
      }
    }

    public bool IsColor => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Color);

      public bool IsInteger => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Integer);

      public bool IsFractional => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Fractional);

      public bool IsPercentage => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Percentage);

      public bool IsAbsLength => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.AbsLength);

      public bool IsPixels => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Pixels);

      public bool IsEms => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Ems);

      public bool IsExs => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Exs);

      public bool IsMilliseconds => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Milliseconds);

      public bool IsKHz => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.kHz);

      public bool IsDegrees => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Degrees);

      public bool IsHtmlFontUnits => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.HtmlFontUnits);

      public bool IsRelativeHtmlFontUnits => (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.RelHtmlFontUnits);

      public bool IsAbsRelLength
    {
      get
      {
        if ((int) this.RawType != (int) PropertyValue.GetRawType(PropertyType.AbsLength) && (int) this.RawType != (int) PropertyValue.GetRawType(PropertyType.RelLength))
          return (int) this.RawType == (int) PropertyValue.GetRawType(PropertyType.Pixels);
        return true;
      }
    }

    public int StringHandle => this.Value;

      public int MultiValueHandle => this.Value;

      public bool Bool => (int) this.UnsignedValue != 0;

      public int Enum => (int) this.UnsignedValue;

      public RGBT Color => new RGBT(this.UnsignedValue);

      public float Percentage => (float) this.Value / 10000f;

      public int Percentage10K => this.Value;

      public float Fractional => (float) this.Value / 10000f;

      public int Integer => this.Value;

      public int Milliseconds => this.Value;

      public int KHz => this.Value;

      public int Degrees => this.Value;

      public int BaseUnits => this.Value;

      public float Twips => (float) this.Value / 8f;

      public int TwipsInteger => this.Value / 8;

      public float Points => (float) this.Value / 160f;

      public int PointsInteger => this.Value / 160;

      public int PointsInteger160 => this.Value;

      public float Picas => (float) this.Value / 1920f;

      public float Inches => (float) this.Value / 11520f;

      public float Centimeters => (float) this.Value / 4535.433f;

      public int MillimetersInteger => this.Value / 454;

      public float Millimeters => (float) this.Value / 453.5433f;

      public int HtmlFontUnits => this.Value;

      public float Pixels => (float) this.Value / 96f;

      public int PixelsInteger => this.Value / 96;

      public int PixelsInteger96 => this.Value;

      public float Ems => (float) this.Value / 160f;

      public int EmsInteger => this.Value / 160;

      public int EmsInteger160 => this.Value;

      public float Exs => (float) this.Value / 160f;

      public int ExsInteger => this.Value / 160;

      public int ExsInteger160 => this.Value;

      public int RelativeHtmlFontUnits => this.Value;

      public PropertyValue(uint rawValue)
    {
      this.RawValue = rawValue;
    }

    public PropertyValue(bool value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(value);
    }

    public PropertyValue(PropertyType type, int value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(type, value);
    }

    public PropertyValue(PropertyType type, uint value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(type, value);
    }

    public PropertyValue(LengthUnits lengthUnits, float value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(lengthUnits, value);
    }

    public PropertyValue(LengthUnits lengthUnits, int value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(lengthUnits, value);
    }

    public PropertyValue(PropertyType type, float value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(type, value);
    }

    public PropertyValue(RGBT color)
    {
      this.RawValue = PropertyValue.ComposeRawValue(color);
    }

    public PropertyValue(System.Enum value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(value);
    }

    public static bool operator ==(PropertyValue x, PropertyValue y)
    {
      return (int) x.RawValue == (int) y.RawValue;
    }

    public static bool operator !=(PropertyValue x, PropertyValue y)
    {
      return (int) x.RawValue != (int) y.RawValue;
    }

    public static uint GetRawType(PropertyType type)
    {
      return (uint) type << 27;
    }

    public void Set(uint rawValue)
    {
      this.RawValue = rawValue;
    }

    public void Set(bool value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(value);
    }

    public void Set(PropertyType type, int value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(type, value);
    }

    public void Set(PropertyType type, uint value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(type, value);
    }

    public void Set(LengthUnits lengthUnits, float value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(lengthUnits, value);
    }

    public void Set(PropertyType type, float value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(type, value);
    }

    public void Set(RGBT color)
    {
      this.RawValue = PropertyValue.ComposeRawValue(color);
    }

    public void Set(System.Enum value)
    {
      this.RawValue = PropertyValue.ComposeRawValue(value);
    }

    public override string ToString()
    {
      switch (this.Type)
      {
        case PropertyType.Null:
          return "null";
        case PropertyType.Calculated:
          return "calculated";
        case PropertyType.Bool:
          return this.Bool.ToString();
        case PropertyType.String:
          return "string: " + this.StringHandle.ToString();
        case PropertyType.MultiValue:
          return "multi: " + this.MultiValueHandle.ToString();
        case PropertyType.Enum:
          return "enum: " + this.Enum.ToString();
        case PropertyType.Color:
          return "color: " + this.Color.ToString();
        case PropertyType.Integer:
          return this.Integer.ToString() + " (integer)";
        case PropertyType.Fractional:
          return this.Fractional.ToString() + " (fractional)";
        case PropertyType.Percentage:
          return this.Percentage.ToString() + "%";
        case PropertyType.AbsLength:
          return this.Points.ToString() + "pt (" + this.Inches.ToString() + "in, " + this.Millimeters.ToString() + "mm) (abs)";
        case PropertyType.RelLength:
          return this.Points.ToString() + "pt (" + this.Inches.ToString() + "in, " + this.Millimeters.ToString() + "mm) (rel)";
        case PropertyType.Pixels:
          return this.Pixels.ToString() + "px";
        case PropertyType.Ems:
          return this.Ems.ToString() + "em";
        case PropertyType.Exs:
          return this.Exs.ToString() + "ex";
        case PropertyType.HtmlFontUnits:
          return this.HtmlFontUnits.ToString() + " (html font units)";
        case PropertyType.RelHtmlFontUnits:
          return this.RelativeHtmlFontUnits.ToString() + " (relative html font units)";
        case PropertyType.Multiple:
          return this.Integer.ToString() + "*";
        case PropertyType.Milliseconds:
          return this.Milliseconds.ToString() + "ms";
        case PropertyType.kHz:
          return this.KHz.ToString() + "kHz";
        case PropertyType.Degrees:
          return this.Degrees.ToString() + "deg";
        default:
          return "unknown value type";
      }
    }

    public override bool Equals(object obj)
    {
      if (obj is PropertyValue)
        return (int) this.RawValue == (int) ((PropertyValue) obj).RawValue;
      return false;
    }

    public override int GetHashCode()
    {
      return (int) this.RawValue;
    }

    internal static int ConvertHtmlFontUnitsToTwips(int nHtmlSize)
    {
      nHtmlSize = Math.Max(1, Math.Min(7, nHtmlSize));
      return PropertyValue.sizesInTwips[nHtmlSize - 1];
    }

    internal static int ConvertTwipsToHtmlFontUnits(int twips)
    {
      for (int index = 0; index < PropertyValue.maxSizesInTwips.Length; ++index)
      {
        if (twips <= PropertyValue.maxSizesInTwips[index])
          return index + 1;
      }
      return PropertyValue.maxSizesInTwips.Length + 1;
    }

    private static uint ComposeRawValue(bool value)
    {
      return PropertyValue.GetRawType(PropertyType.Bool) | (value ? 1U : 0U);
    }

    private static uint ComposeRawValue(PropertyType type, int value)
    {
      return (uint) ((int) type << 27 | value & 134217727);
    }

    private static uint ComposeRawValue(PropertyType type, uint value)
    {
      return (uint) type << 27 | value;
    }

    private static uint ComposeRawValue(LengthUnits lengthUnits, float len)
    {
      switch (lengthUnits)
      {
        case LengthUnits.BaseUnits:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) len & 134217727U;
        case LengthUnits.Twips:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) ((double) len * 8.0) & 134217727U;
        case LengthUnits.Points:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) ((double) len * 160.0) & 134217727U;
        case LengthUnits.Picas:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) ((double) len * 1920.0) & 134217727U;
        case LengthUnits.Inches:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) ((double) len * 11520.0) & 134217727U;
        case LengthUnits.Centimeters:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) ((double) len * 4535.43310546875) & 134217727U;
        case LengthUnits.Millimeters:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) ((double) len * 453.543304443359) & 134217727U;
        case LengthUnits.HtmlFontUnits:
          return PropertyValue.GetRawType(PropertyType.HtmlFontUnits) | (uint) len & 134217727U;
        case LengthUnits.Pixels:
          return PropertyValue.GetRawType(PropertyType.Pixels) | (uint) ((double) len * 96.0) & 134217727U;
        case LengthUnits.Ems:
          return PropertyValue.GetRawType(PropertyType.Ems) | (uint) ((double) len * 160.0) & 134217727U;
        case LengthUnits.Exs:
          return PropertyValue.GetRawType(PropertyType.Exs) | (uint) ((double) len * 160.0) & 134217727U;
        case LengthUnits.RelativeHtmlFontUnits:
          return PropertyValue.GetRawType(PropertyType.RelHtmlFontUnits) | (uint) (int) len & 134217727U;
        case LengthUnits.Percents:
          return PropertyValue.GetRawType(PropertyType.Percentage) | (uint) len & 134217727U;
        default:
          return 0U;
      }
    }

    private static uint ComposeRawValue(LengthUnits lengthUnits, int len)
    {
      switch (lengthUnits)
      {
        case LengthUnits.BaseUnits:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) (len & 134217727);
        case LengthUnits.Twips:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) (len * 8 & 134217727);
        case LengthUnits.Points:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) (len * 160 & 134217727);
        case LengthUnits.Picas:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) (len * 1920 & 134217727);
        case LengthUnits.Inches:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) (len * 11520 & 134217727);
        case LengthUnits.Centimeters:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) (len * 4535 & 134217727);
        case LengthUnits.Millimeters:
          return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint) (len * 453 & 134217727);
        case LengthUnits.HtmlFontUnits:
          return PropertyValue.GetRawType(PropertyType.HtmlFontUnits) | (uint) (len & 134217727);
        case LengthUnits.Pixels:
          return PropertyValue.GetRawType(PropertyType.Pixels) | (uint) (len * 96 & 134217727);
        case LengthUnits.Ems:
          return PropertyValue.GetRawType(PropertyType.Ems) | (uint) (len * 160 & 134217727);
        case LengthUnits.Exs:
          return PropertyValue.GetRawType(PropertyType.Exs) | (uint) (len * 160 & 134217727);
        case LengthUnits.RelativeHtmlFontUnits:
          return PropertyValue.GetRawType(PropertyType.RelHtmlFontUnits) | (uint) (len & 134217727);
        case LengthUnits.Percents:
          return PropertyValue.GetRawType(PropertyType.Percentage) | (uint) (len & 134217727);
        default:
          return 0U;
      }
    }

    private static uint ComposeRawValue(PropertyType type, float value)
    {
      return PropertyValue.GetRawType(type) | (uint) ((double) value * 10000.0) & 134217727U;
    }

    private static uint ComposeRawValue(RGBT color)
    {
      return PropertyValue.GetRawType(PropertyType.Color) | color.RawValue & 134217727U;
    }

    private static uint ComposeRawValue(System.Enum value)
    {
      return PropertyValue.GetRawType(PropertyType.Enum) | Convert.ToUInt32((object) value) & 134217727U;
    }
  }
}
