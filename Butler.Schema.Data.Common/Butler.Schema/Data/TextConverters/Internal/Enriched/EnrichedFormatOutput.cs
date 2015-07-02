// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Enriched.EnrichedFormatOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Enriched
{
  internal class EnrichedFormatOutput : Format.FormatOutput, IRestartable, IFallback
  {
    private bool blockEmpty = true;
    private ConverterOutput output;
    private Injection injection;
    private bool fallbacks;
    private bool blockEnd;
    private int lineLength;
    private int insideNofill;
    private int listLevel;
    private int listIndex;
    private int spaceBefore;

    public override bool OutputCodePageSameAsInput => false;

      public override Encoding OutputEncoding
    {
      set
      {
        throw new InvalidOperationException();
      }
    }

    public override bool CanAcceptMoreOutput => this.output.CanAcceptMore;

      public EnrichedFormatOutput(ConverterOutput output, Injection injection, bool fallbacks, Stream formatTraceStream, Stream formatOutputTraceStream)
      : base(formatOutputTraceStream)
    {
      this.output = output;
      this.injection = injection;
      this.fallbacks = fallbacks;
    }

    bool IRestartable.CanRestart()
    {
      if (this.output is IRestartable)
        return ((IRestartable) this.output).CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      ((IRestartable) this.output).Restart();
      this.Restart();
      this.blockEmpty = true;
      this.blockEnd = false;
      this.lineLength = 0;
      this.insideNofill = 0;
      this.listLevel = 0;
      this.listIndex = 0;
      this.spaceBefore = 0;
      if (this.injection == null)
        return;
      this.injection.Reset();
    }

    void IRestartable.DisableRestart()
    {
      if (!(this.output is IRestartable))
        return;
      ((IRestartable) this.output).DisableRestart();
    }

    public override bool Flush()
    {
      if (!base.Flush())
        return false;
      this.output.Flush();
      return true;
    }

    byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
    {
      unsafeAsciiMask = (byte) 1;
      return Html.HtmlSupport.UnsafeAsciiMap;
    }

    bool IFallback.HasUnsafeUnicode()
    {
      return false;
    }

    bool IFallback.TreatNonAsciiAsUnsafe(string charset)
    {
      return false;
    }

    bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
    {
      return false;
    }

    bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
    {
      string str = (string) null;
      if ((int) ch == 60)
        str = "<<";
      else if (this.fallbacks)
        str = this.GetSubstitute(ch);
      if (str != null)
      {
        if (outputEnd - outputBufferCount < str.Length)
          return false;
        str.CopyTo(0, outputBuffer, outputBufferCount, str.Length);
        outputBufferCount += str.Length;
      }
      else
        outputBuffer[outputBufferCount++] = ch;
      return true;
    }

    protected override bool StartDocument()
    {
      if (this.injection != null)
      {
        int num = this.injection.HaveHead ? 1 : 0;
      }
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndDocument()
    {
      this.RevertCharFormat();
      if (this.lineLength != 0)
      {
        this.output.Write("\r\n");
        if (!this.blockEnd)
          this.output.Write("\r\n");
      }
      if (this.injection == null)
        return;
      int num = this.injection.HaveTail ? 1 : 0;
    }

    protected override bool StartBlockContainer()
    {
      if (!this.blockEmpty)
      {
        if (this.lineLength != 0 && this.insideNofill == 0)
          this.output.Write("\r\n");
        this.output.Write("\r\n");
        this.lineLength = 0;
        this.blockEmpty = true;
      }
      this.blockEnd = false;
      Format.PropertyValue effectiveProperty1 = this.GetEffectiveProperty(Format.PropertyId.Margins);
      if (!effectiveProperty1.IsNull && effectiveProperty1.IsAbsLength)
        this.spaceBefore = Math.Max(this.spaceBefore, effectiveProperty1.PointsInteger);
      Format.PropertyValue effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.Preformatted);
      if (!effectiveProperty2.IsNull && effectiveProperty2.Bool)
      {
        this.output.Write("<Nofill>");
        this.lineLength = "<Nofill>".Length;
        ++this.insideNofill;
      }
      else
      {
        StringBuilder stringBuilder = (StringBuilder) null;
        Format.PropertyValue effectiveProperty3 = this.GetEffectiveProperty(Format.PropertyId.QuotingLevelDelta);
        if (!effectiveProperty3.IsNull && effectiveProperty3.IsInteger && effectiveProperty3.Integer > 0)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          for (int index = 0; index < effectiveProperty3.Integer; ++index)
            stringBuilder.Append("<Excerpt>");
        }
        Format.PropertyValue effectiveProperty4 = this.GetEffectiveProperty(Format.PropertyId.RightToLeft);
        bool flag1 = effectiveProperty4.IsNull || !effectiveProperty4.Bool;
        Format.PropertyValue propertyValue1 = flag1 ? this.GetEffectiveProperty(Format.PropertyId.LeftPadding) : this.GetEffectiveProperty(Format.PropertyId.RightPadding);
        Format.PropertyValue propertyValue2 = flag1 ? this.GetEffectiveProperty(Format.PropertyId.RightPadding) : this.GetEffectiveProperty(Format.PropertyId.LeftPadding);
        Format.PropertyValue effectiveProperty5 = this.GetEffectiveProperty(Format.PropertyId.FirstLineIndent);
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        if (!propertyValue1.IsNull && propertyValue1.IsAbsLength)
          num1 = EnrichedFormatOutput.CheckRange(0, (propertyValue1.PointsInteger + 12) / 30, 50);
        if (!effectiveProperty5.IsNull && effectiveProperty5.IsAbsLength)
          num3 = EnrichedFormatOutput.CheckRange(-50, (effectiveProperty5.PointsInteger + (effectiveProperty5.PointsInteger > 0 ? 12 : -12)) / 30, 50);
        if (!propertyValue2.IsNull && propertyValue2.IsAbsLength)
          num2 = EnrichedFormatOutput.CheckRange(0, propertyValue2.PointsInteger / 30, 50);
        if (num1 != 0 || num2 != 0 || num3 != 0)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          int num4 = 0;
          if (num3 < 0)
          {
            num4 = -num3;
            num3 = 0;
            num1 -= num4;
            if (num1 < 0)
              num1 = 0;
          }
          stringBuilder.Append("<ParaIndent><Param>");
          bool flag2 = false;
          while (num1-- != 0)
          {
            if (flag2)
              stringBuilder.Append(',');
            stringBuilder.Append("Left");
            flag2 = true;
          }
          while (num2-- != 0)
          {
            if (flag2)
              stringBuilder.Append(',');
            stringBuilder.Append("Right");
            flag2 = true;
          }
          while (num3-- != 0)
          {
            if (flag2)
              stringBuilder.Append(',');
            stringBuilder.Append("In");
            flag2 = true;
          }
          while (num4-- != 0)
          {
            if (flag2)
              stringBuilder.Append(',');
            stringBuilder.Append("Out");
            flag2 = true;
          }
          stringBuilder.Append("</Param>");
        }
        Format.PropertyValue effectiveProperty6 = this.GetEffectiveProperty(Format.PropertyId.TextAlignment);
        if (!effectiveProperty6.IsNull)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          switch (effectiveProperty6.Enum)
          {
            case 1:
              stringBuilder.Append("<Center>");
              break;
            case 3:
              stringBuilder.Append("<FlushLeft>");
              break;
            case 4:
              stringBuilder.Append("<FlushRight>");
              break;
            case 6:
              stringBuilder.Append("<FlushBoth>");
              break;
          }
        }
        if (stringBuilder != null && stringBuilder.Length != 0)
        {
          this.lineLength += stringBuilder.Length;
          this.output.Write(stringBuilder.ToString());
        }
      }
      return true;
    }

    protected override void EndBlockContainer()
    {
      Format.PropertyValue effectiveProperty1 = this.GetEffectiveProperty(Format.PropertyId.Preformatted);
      if (!effectiveProperty1.IsNull && effectiveProperty1.Bool)
      {
        --this.insideNofill;
        this.output.Write("</Nofill>");
        this.lineLength += "</Nofill>".Length;
      }
      else
      {
        StringBuilder stringBuilder = (StringBuilder) null;
        Format.PropertyValue effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.TextAlignment);
        if (!effectiveProperty2.IsNull)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          switch (effectiveProperty2.Enum)
          {
            case 1:
              stringBuilder.Append("</Center>");
              break;
            case 3:
              stringBuilder.Append("</FlushLeft>");
              break;
            case 4:
              stringBuilder.Append("</FlushRight>");
              break;
            case 6:
              stringBuilder.Append("</FlushBoth>");
              break;
          }
        }
        Format.PropertyValue effectiveProperty3 = this.GetEffectiveProperty(Format.PropertyId.LeftPadding);
        Format.PropertyValue effectiveProperty4 = this.GetEffectiveProperty(Format.PropertyId.RightPadding);
        Format.PropertyValue effectiveProperty5 = this.GetEffectiveProperty(Format.PropertyId.FirstLineIndent);
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        if (!effectiveProperty3.IsNull && effectiveProperty3.IsAbsLength)
          num1 = EnrichedFormatOutput.CheckRange(0, (effectiveProperty3.PointsInteger + 12) / 30, 50);
        if (!effectiveProperty5.IsNull && effectiveProperty5.IsAbsLength)
          num3 = EnrichedFormatOutput.CheckRange(-50, (effectiveProperty5.PointsInteger + (effectiveProperty5.PointsInteger > 0 ? 12 : -12)) / 30, 50);
        if (!effectiveProperty4.IsNull && effectiveProperty4.IsAbsLength)
          num2 = EnrichedFormatOutput.CheckRange(0, effectiveProperty4.PointsInteger / 30, 50);
        if (num1 != 0 || num2 != 0 || num3 != 0)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          stringBuilder.Append("</ParaIndent>");
        }
        Format.PropertyValue effectiveProperty6 = this.GetEffectiveProperty(Format.PropertyId.QuotingLevelDelta);
        if (!effectiveProperty6.IsNull && effectiveProperty6.IsInteger && effectiveProperty6.Integer > 0)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          for (int index = 0; index < effectiveProperty6.Integer; ++index)
            stringBuilder.Append("</Excerpt>");
        }
        if (stringBuilder != null && stringBuilder.Length != 0)
        {
          this.lineLength += stringBuilder.Length;
          this.output.Write(stringBuilder.ToString());
        }
      }
      this.blockEnd = true;
      Format.PropertyValue effectiveProperty7 = this.GetEffectiveProperty(Format.PropertyId.BottomMargin);
      if (effectiveProperty7.IsNull || !effectiveProperty7.IsAbsLength)
        return;
      this.spaceBefore = Math.Max(this.spaceBefore, effectiveProperty7.PointsInteger);
    }

    protected override bool StartTable()
    {
      if (!this.blockEmpty)
      {
        if (this.lineLength != 0 && this.insideNofill == 0)
          this.output.Write("\r\n");
        this.output.Write("\r\n");
        this.lineLength = 0;
        this.blockEmpty = true;
      }
      this.blockEnd = false;
      return true;
    }

    protected override void EndTable()
    {
      this.blockEnd = true;
    }

    protected override bool StartTableCaption()
    {
      if (!this.blockEmpty)
      {
        if (this.lineLength != 0 && this.insideNofill == 0)
          this.output.Write("\r\n");
        this.output.Write("\r\n");
        this.lineLength = 0;
        this.blockEmpty = true;
      }
      this.blockEnd = false;
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndTableCaption()
    {
      this.RevertCharFormat();
      this.blockEnd = true;
    }

    protected override bool StartTableExtraContent()
    {
      if (!this.blockEmpty)
      {
        if (this.lineLength != 0 && this.insideNofill == 0)
          this.output.Write("\r\n");
        this.output.Write("\r\n");
        this.lineLength = 0;
        this.blockEmpty = true;
      }
      this.blockEnd = false;
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndTableExtraContent()
    {
      this.RevertCharFormat();
      this.blockEnd = true;
    }

    protected override bool StartTableRow()
    {
      if (!this.blockEmpty)
      {
        if (this.lineLength != 0 && this.insideNofill == 0)
          this.output.Write("\r\n");
        this.output.Write("\r\n");
        this.lineLength = 0;
        this.blockEmpty = true;
      }
      this.blockEnd = false;
      return true;
    }

    protected override void EndTableRow()
    {
      this.blockEnd = true;
    }

    protected override bool StartTableCell()
    {
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndTableCell()
    {
      this.RevertCharFormat();
      if (this.CurrentNode.NextSibling.IsNull)
        return;
      this.output.Write("\t");
    }

    protected override bool StartList()
    {
      ++this.listLevel;
      this.StartBlockContainer();
      if (this.listLevel == 1)
      {
        Format.PropertyValue property = this.CurrentNode.Parent.GetProperty(Format.PropertyId.ListStart);
        this.listIndex = property.IsNull ? 1 : property.Integer;
      }
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.RightToLeft);
      bool flag1 = effectiveProperty.IsNull || !effectiveProperty.Bool;
      int num1 = 0;
      Format.PropertyValue propertyValue = flag1 ? this.CurrentNode.Parent.GetProperty(Format.PropertyId.LeftMargin) : this.CurrentNode.Parent.GetProperty(Format.PropertyId.RightMargin);
      if (!propertyValue.IsNull && propertyValue.IsAbsLength)
        num1 = EnrichedFormatOutput.CheckRange(0, propertyValue.PointsInteger / 30, 50);
      int num2 = num1 + 1;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<ParaIndent><Param>");
      bool flag2 = false;
      while (num2-- != 0)
      {
        if (flag2)
          stringBuilder.Append(',');
        stringBuilder.Append("Left");
        flag2 = true;
      }
      stringBuilder.Append("</Param>");
      this.lineLength += stringBuilder.Length;
      this.output.Write(stringBuilder.ToString());
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndList()
    {
      this.RevertCharFormat();
      this.output.Write("</ParaIndent>");
      this.lineLength += "</Paraindent>".Length;
      this.EndBlockContainer();
      --this.listLevel;
    }

    protected override bool StartListItem()
    {
      this.StartBlockContainer();
      this.ApplyCharFormat();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.ListStyle);
      if (effectiveProperty.IsNull || effectiveProperty.Enum == 1 || this.listLevel > 1)
      {
        this.output.Write("*   ");
        this.lineLength += 2;
      }
      else if (effectiveProperty.Enum != 2)
      {
        this.output.Write("*   ");
        this.lineLength += 2;
      }
      else
      {
        string text = this.listIndex.ToString();
        this.output.Write(text);
        this.output.Write(". ");
        if (text.Length == 1)
          this.output.Write(' ');
        ++this.listIndex;
        this.lineLength += text.Length + (text.Length == 1 ? 3 : 2);
      }
      this.blockEmpty = false;
      return true;
    }

    protected override void EndListItem()
    {
      this.RevertCharFormat();
      this.EndBlockContainer();
    }

    protected override bool StartHyperLink()
    {
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndHyperLink()
    {
      this.RevertCharFormat();
    }

    protected override void StartEndImage()
    {
    }

    protected override void StartEndHorizontalLine()
    {
      if (!this.blockEmpty)
      {
        if (this.lineLength != 0 && this.insideNofill == 0)
          this.output.Write("\r\n");
        this.output.Write("\r\n");
      }
      this.output.Write("________________________________\r\n\r\n");
      this.lineLength = 0;
      this.blockEmpty = true;
    }

    protected override bool StartInlineContainer()
    {
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndInlineContainer()
    {
      this.RevertCharFormat();
    }

    protected override void StartEndArea()
    {
    }

    protected override bool StartOption()
    {
      return true;
    }

    protected override bool StartText()
    {
      if (this.blockEnd)
      {
        if (this.lineLength != 0 && this.insideNofill == 0)
        {
          this.output.Write("\r\n");
          this.lineLength = 0;
        }
        this.output.Write("\r\n");
        this.blockEnd = false;
      }
      this.blockEmpty = false;
      this.ApplyCharFormat();
      return true;
    }

    protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
    {
      if ((int) beginTextPosition != (int) endTextPosition)
      {
        Format.TextRun textRun = this.FormatStore.GetTextRun(beginTextPosition);
        do
        {
          int effectiveLength = textRun.EffectiveLength;
          Format.TextRunType type = textRun.Type;
          if ((uint) type <= 16384U)
          {
            if (type != Format.TextRunType.NonSpace)
            {
              if (type == Format.TextRunType.NbSp)
              {
                this.lineLength += effectiveLength;
                while (effectiveLength-- != 0)
                  this.output.Write(' ');
              }
            }
            else
            {
              this.lineLength += effectiveLength;
              int start = 0;
              do
              {
                char[] buffer;
                int offset;
                int count;
                textRun.GetChunk(start, out buffer, out offset, out count);
                this.output.Write(buffer, offset, count, (IFallback) this);
                start += count;
              }
              while (start != effectiveLength);
            }
          }
          else if (type != Format.TextRunType.Space)
          {
            if (type != Format.TextRunType.Tabulation)
            {
              if (type == Format.TextRunType.NewLine)
              {
                if (this.insideNofill != 0)
                {
                  while (effectiveLength-- != 0)
                    this.output.Write("\r\n");
                }
                else
                {
                  if (this.lineLength != 0)
                    this.output.Write("\r\n");
                  while (effectiveLength-- != 0)
                    this.output.Write("\r\n");
                }
                this.lineLength = 0;
                this.blockEmpty = true;
              }
            }
            else
            {
              while (effectiveLength-- != 0)
              {
                this.output.Write('\t');
                this.lineLength = (this.lineLength + 8) / 8 * 8;
              }
            }
          }
          else
          {
            if (this.lineLength + effectiveLength > 80 && this.lineLength != 0 && this.insideNofill == 0)
            {
              this.output.Write("\r\n");
              --effectiveLength;
              this.lineLength = 0;
            }
            if (effectiveLength != 0)
            {
              this.lineLength += effectiveLength;
              while (effectiveLength-- != 0)
                this.output.Write(' ');
            }
          }
          textRun.MoveNext();
        }
        while (textRun.Position < endTextPosition);
      }
      return true;
    }

    protected override void EndText()
    {
      this.RevertCharFormat();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.output != null && this.output != null)
        this.output.Dispose();
      this.output = (ConverterOutput) null;
      base.Dispose(disposing);
    }

    private static int CheckRange(int min, int value, int max)
    {
      if (value < min)
        return min;
      if (value > max)
        return max;
      return value;
    }

    private void ApplyCharFormat()
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      Format.FlagProperties effectiveFlags = this.GetEffectiveFlags();
      Format.PropertyValue effectiveProperty1 = this.GetEffectiveProperty(Format.PropertyId.FontFace);
      Format.PropertyValue effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.FontSize);
      if (!effectiveProperty1.IsNull && effectiveProperty1.IsString && (this.FormatStore.GetStringValue(effectiveProperty1).GetString().Equals("Courier New") && !effectiveProperty2.IsNull && (effectiveProperty2.IsRelativeHtmlFontUnits && effectiveProperty2.RelativeHtmlFontUnits == -1)))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("<Fixed>");
      }
      else
      {
        if (!effectiveProperty1.IsNull)
        {
          string str = (string) null;
          if (effectiveProperty1.IsMultiValue)
          {
            Format.MultiValue multiValue = this.FormatStore.GetMultiValue(effectiveProperty1);
            if (multiValue.Length != 0)
              str = multiValue.GetStringValue(0).GetString();
          }
          else
            str = this.FormatStore.GetStringValue(effectiveProperty1).GetString();
          if (str != null)
          {
            if (stringBuilder == null)
              stringBuilder = new StringBuilder();
            stringBuilder.Append("<FontFamily><Param>");
            stringBuilder.Append(str);
            stringBuilder.Append("</Param>");
          }
        }
        if (!effectiveProperty2.IsNull && !effectiveProperty2.IsAbsLength && (!effectiveProperty2.IsHtmlFontUnits && effectiveProperty2.IsRelativeHtmlFontUnits) && effectiveProperty2.RelativeHtmlFontUnits != 0)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          if (effectiveProperty2.RelativeHtmlFontUnits > 0)
          {
            stringBuilder.Append("<Bigger>");
            for (int index = 1; index < effectiveProperty2.RelativeHtmlFontUnits; ++index)
              stringBuilder.Append("<Bigger>");
          }
          else
          {
            stringBuilder.Append("<Smaller>");
            for (int index = -1; index > effectiveProperty2.RelativeHtmlFontUnits; --index)
              stringBuilder.Append("<Smaller>");
          }
        }
      }
      Format.PropertyValue propertyValue = this.GetEffectiveProperty(Format.PropertyId.FontColor);
      if (propertyValue.IsEnum)
        propertyValue = Html.HtmlSupport.TranslateSystemColor(propertyValue);
      if (propertyValue.IsColor && (int) propertyValue.Color.RGB != 0)
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        uint num1 = propertyValue.Color.Red << 8;
        uint num2 = propertyValue.Color.Green << 8;
        uint num3 = propertyValue.Color.Blue << 8;
        if (((int) num1 & 256) != 0)
          num1 += (uint) byte.MaxValue;
        if (((int) num2 & 256) != 0)
          num2 += (uint) byte.MaxValue;
        if (((int) num3 & 256) != 0)
          num3 += (uint) byte.MaxValue;
        stringBuilder.Append("<Color><Param>");
        stringBuilder.Append(string.Format("{0:X4},{1:X4},{2:X4}", (object) num1, (object) num2, (object) num3));
        stringBuilder.Append("</Param>");
      }
      if (effectiveFlags.IsDefinedAndOn(Format.PropertyId.FirstFlag))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("<Bold>");
      }
      if (effectiveFlags.IsDefinedAndOn(Format.PropertyId.Italic))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("<Italic>");
      }
      if (effectiveFlags.IsDefinedAndOn(Format.PropertyId.Underline))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("<Underline>");
      }
      if (stringBuilder == null || stringBuilder.Length == 0)
        return;
      this.lineLength += stringBuilder.Length;
      this.output.Write(stringBuilder.ToString());
    }

    private void RevertCharFormat()
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      Format.FlagProperties effectiveFlags = this.GetEffectiveFlags();
      if (effectiveFlags.IsDefinedAndOn(Format.PropertyId.Underline))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("</Underline>");
      }
      if (effectiveFlags.IsDefinedAndOn(Format.PropertyId.Italic))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("</Italic>");
      }
      if (effectiveFlags.IsDefinedAndOn(Format.PropertyId.FirstFlag))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("</Bold>");
      }
      Format.PropertyValue propertyValue = this.GetEffectiveProperty(Format.PropertyId.FontColor);
      if (propertyValue.IsEnum)
        propertyValue = Html.HtmlSupport.TranslateSystemColor(propertyValue);
      if (propertyValue.IsColor && (int) propertyValue.Color.RGB != 0)
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("</Color>");
      }
      Format.PropertyValue effectiveProperty1 = this.GetEffectiveProperty(Format.PropertyId.FontFace);
      Format.PropertyValue effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.FontSize);
      if (!effectiveProperty1.IsNull && effectiveProperty1.IsString && (this.FormatStore.GetStringValue(effectiveProperty1).GetString().Equals("Courier New") && !effectiveProperty2.IsNull && (effectiveProperty2.IsRelativeHtmlFontUnits && effectiveProperty2.RelativeHtmlFontUnits == -1)))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        stringBuilder.Append("</Fixed>");
      }
      else
      {
        if (!effectiveProperty2.IsNull && !effectiveProperty2.IsAbsLength && (!effectiveProperty2.IsHtmlFontUnits && effectiveProperty2.IsRelativeHtmlFontUnits) && effectiveProperty2.RelativeHtmlFontUnits != 0)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          if (effectiveProperty2.RelativeHtmlFontUnits > 0)
          {
            stringBuilder.Append("</Bigger>");
            for (int index = 1; index < effectiveProperty2.RelativeHtmlFontUnits; ++index)
              stringBuilder.Append("</Bigger>");
          }
          else
          {
            stringBuilder.Append("</Smaller>");
            for (int index = -1; index > effectiveProperty2.RelativeHtmlFontUnits; --index)
              stringBuilder.Append("</Smaller>");
          }
        }
        if (!effectiveProperty1.IsNull)
        {
          string str = (string) null;
          if (effectiveProperty1.IsMultiValue)
          {
            Format.MultiValue multiValue = this.FormatStore.GetMultiValue(effectiveProperty1);
            if (multiValue.Length != 0)
              str = multiValue.GetStringValue(0).GetString();
          }
          else
            str = this.FormatStore.GetStringValue(effectiveProperty1).GetString();
          if (str != null)
          {
            if (stringBuilder == null)
              stringBuilder = new StringBuilder();
            stringBuilder.Append("</FontFamily>");
          }
        }
      }
      if (stringBuilder == null || stringBuilder.Length == 0)
        return;
      this.lineLength += stringBuilder.Length;
      this.output.Write(stringBuilder.ToString());
    }

    private string GetSubstitute(char ch)
    {
      return Globalization.AsciiEncoderFallback.GetCharacterFallback(ch);
    }
  }
}
