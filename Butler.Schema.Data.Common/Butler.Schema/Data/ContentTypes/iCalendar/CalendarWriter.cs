// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarWriter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  public class CalendarWriter : IDisposable
  {
    private static readonly char[] ParameterValueSpecials = new char[4]
    {
      ',',
      ':',
      ';',
      '"'
    };
    private static readonly char[] PropertyValueSpecials = new char[5]
    {
      ',',
      ';',
      '\\',
      '\r',
      '\n'
    };
    private ParameterId parameter = ParameterId.Unknown;
    private CalendarValueType valueType = CalendarValueType.Unknown;
    private WriteState state = WriteState.Start;
    private Stack<CalendarWriter.WriterState> stateStack = new Stack<CalendarWriter.WriterState>();
    private bool validate = true;
    private const char CR = '\r';
    private const char LF = '\n';
    private const string ComponentStartTag = "BEGIN";
    private const string ComponentEndTag = "END";
    private ComponentId componentId;
    private PropertyId property;
    private string componentName;
    private string propertyName;
    private Internal.ContentLineWriter writer;
    private bool firstPropertyValue;
    private bool firstParameterValue;

    public CalendarWriter(Stream stream)
      : this(stream, "utf-8")
    {
    }

    public CalendarWriter(Stream stream, string encodingName)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!stream.CanWrite)
        throw new ArgumentException(CtsResources.CalendarStrings.StreamMustAllowWrite, "stream");
      if (encodingName == null)
        throw new ArgumentNullException("encodingName");
      this.writer = new Internal.ContentLineWriter(stream, Globalization.Charset.GetEncoding(encodingName));
    }

    public void Flush()
    {
      this.AssertValidState(WriteState.Component | WriteState.Property | WriteState.Parameter);
      this.writer.Flush();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual void Close()
    {
      this.Dispose();
    }

    public void StartComponent(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (this.validate && name.Length == 0)
        throw new ArgumentException();
      this.EndProperty();
      this.AssertValidState(WriteState.Start | WriteState.Component);
      this.Save();
      this.componentName = name.ToUpper();
      this.componentId = CalendarCommon.GetComponentEnum(name);
      this.writer.WriteProperty("BEGIN", this.componentName);
      this.state = WriteState.Component;
    }

    public void StartComponent(ComponentId componentId)
    {
      if (componentId == ComponentId.Unknown || componentId == ComponentId.None)
        throw new ArgumentException(CtsResources.CalendarStrings.InvalidComponentId);
      this.StartComponent(CalendarCommon.GetComponentString(componentId));
    }

    public void EndComponent()
    {
      this.EndProperty();
      this.AssertValidState(WriteState.Component);
      this.writer.WriteProperty("END", this.componentName);
      this.Load();
      this.state = WriteState.Component;
    }

    public void StartProperty(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (this.validate && name.Length == 0)
        throw new ArgumentException();
      this.EndProperty();
      this.AssertValidState(WriteState.Component);
      PropertyId propertyEnum = CalendarCommon.GetPropertyEnum(name);
      this.propertyName = name.ToUpper();
      this.property = propertyEnum;
      this.Save();
      this.valueType = CalendarCommon.GetDefaultValueType(propertyEnum);
      this.writer.StartProperty(this.propertyName);
      this.firstPropertyValue = true;
      this.state = WriteState.Property;
    }

    public void StartProperty(PropertyId propertyId)
    {
      string propertyString = CalendarCommon.GetPropertyString(propertyId);
      if (propertyString == null)
        throw new ArgumentException(CtsResources.CalendarStrings.InvalidPropertyId);
      this.StartProperty(propertyString);
    }

    public void StartParameter(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (this.validate && name.Length == 0)
        throw new ArgumentException();
      this.EndParameter();
      this.AssertValidState(WriteState.Property);
      this.parameter = CalendarCommon.GetParameterEnum(name);
      this.writer.StartParameter(name);
      this.firstParameterValue = true;
      this.state = WriteState.Parameter;
    }

    public void StartParameter(ParameterId parameterId)
    {
      string parameterString = CalendarCommon.GetParameterString(parameterId);
      if (parameterString == null)
        throw new ArgumentException(CtsResources.CalendarStrings.InvalidParameterId);
      this.StartParameter(parameterString);
    }

    public void WriteParameterValue(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.AssertValidState(WriteState.Parameter);
      if (this.firstParameterValue)
      {
        this.writer.WriteStartValue();
        this.firstParameterValue = false;
      }
      else
        this.writer.WriteNextValue(Internal.ContentLineParser.Separators.Comma);
      if (this.parameter == ParameterId.ValueType && value.Length > 0)
        this.valueType = CalendarCommon.GetValueTypeEnum(value);
      bool flag = this.IsQuotingRequired(value);
      if (flag)
        this.writer.WriteToStream((byte) 34);
      this.writer.WriteToStream(value);
      if (!flag)
        return;
      this.writer.WriteToStream((byte) 34);
    }

    public void WritePropertyValue(string value)
    {
      this.WritePropertyValue(value, CalendarValueSeparators.Comma);
    }

    public void WritePropertyValue(object value)
    {
      this.WritePropertyValue(value, CalendarValueSeparators.Comma);
    }

    public void WritePropertyValue(CalendarPeriod value)
    {
      this.WritePropertyValue(value.ToString());
    }

    public void WritePropertyValue(Recurrence value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.WritePropertyValue(value.ToString());
    }

    public void WritePropertyValue(int value)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }

    public void WritePropertyValue(float value)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }

    public void WritePropertyValue(bool value)
    {
      this.WritePropertyValue(value ? "TRUE" : "FALSE");
    }

    public void WritePropertyValue(DateTime value)
    {
      this.WritePropertyValue(value, this.valueType);
    }

    public void WritePropertyValue(DateTime value, CalendarValueType valueType)
    {
      this.WritePropertyValue(value, valueType, CalendarValueSeparators.Comma);
    }

    public void WritePropertyValue(CalendarTime value)
    {
      this.WritePropertyValue(value.ToString());
    }

    public void WritePropertyValue(TimeSpan value)
    {
      this.WritePropertyValue(value, this.valueType, CalendarValueSeparators.Comma);
    }

    public void WriteComponent(CalendarReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.Depth > 100)
        return;
      this.StartComponent(reader.ComponentName);
      CalendarPropertyReader propertyReader = reader.PropertyReader;
      while (propertyReader.ReadNextProperty())
        this.WriteProperty(propertyReader);
      if (reader.ReadFirstChildComponent())
      {
        this.WriteComponent(reader);
        while (reader.ReadNextSiblingComponent())
          this.WriteComponent(reader);
      }
      this.EndComponent();
    }

    public void WriteProperty(CalendarPropertyReader reader)
    {
      CalendarParameterReader parameterReader = reader.ParameterReader;
      this.StartProperty(reader.Name);
      while (parameterReader.ReadNextParameter())
        this.WriteParameter(parameterReader);
      CalendarValueSeparators separator = CalendarValueSeparators.None;
      while (reader.ReadNextValue())
      {
        this.WritePropertyValue(reader.ReadValue(CalendarValueSeparators.Comma | CalendarValueSeparators.Semicolon), separator);
        separator = reader.LastValueSeparator;
      }
      this.EndProperty();
    }

    public void WriteProperty(string name, string value)
    {
      this.StartProperty(name);
      this.WritePropertyValue(value);
      this.EndProperty();
    }

    public void WriteProperty(PropertyId propertyId, string value)
    {
      this.StartProperty(propertyId);
      this.WritePropertyValue(value);
      this.EndProperty();
    }

    public void WriteParameter(CalendarParameterReader reader)
    {
      this.StartParameter(reader.Name);
      while (reader.ReadNextValue())
        this.WriteParameterValue(reader.ReadValue());
      if (this.firstParameterValue)
        this.WriteParameterValue(string.Empty);
      this.EndParameter();
    }

    public void WriteParameter(string name, string value)
    {
      this.StartParameter(name);
      this.WriteParameterValue(value);
      this.EndParameter();
    }

    public void WriteParameter(ParameterId parameterId, string value)
    {
      this.StartParameter(parameterId);
      this.WriteParameterValue(value);
      this.EndParameter();
    }

    internal void WritePropertyValue(string value, CalendarValueSeparators separator)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.PrepareStartPropertyValue((Internal.ContentLineParser.Separators) separator);
      if (CalendarValueType.Text == this.valueType)
        value = CalendarWriter.GetEscapedText(value);
      this.writer.WriteToStream(value);
    }

    internal void WritePropertyValue(object value, CalendarValueSeparators separator)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      switch (this.valueType)
      {
        case CalendarValueType.Uri:
        case CalendarValueType.Text:
        case CalendarValueType.Unknown:
        case CalendarValueType.CalAddress:
          string str = value as string;
          if (str == null)
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue(str, separator);
          break;
        case CalendarValueType.UtcOffset:
          if (!(value is TimeSpan))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((TimeSpan) value, CalendarValueType.UtcOffset, separator);
          break;
        case CalendarValueType.Time:
          if (!(value is CalendarTime))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((CalendarTime) value, separator);
          break;
        case CalendarValueType.Integer:
          if (!(value is int))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((int) value, separator);
          break;
        case CalendarValueType.Period:
          if (!(value is CalendarPeriod))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((CalendarPeriod) value, separator);
          break;
        case CalendarValueType.Recurrence:
          Recurrence recurrence = value as Recurrence;
          if (recurrence == null)
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue(recurrence);
          break;
        case CalendarValueType.DateTime:
          if (!(value is DateTime))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((DateTime) value, separator);
          break;
        case CalendarValueType.Duration:
          if (!(value is TimeSpan))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((TimeSpan) value, CalendarValueType.Duration, separator);
          break;
        case CalendarValueType.Float:
          if (!(value is float))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((float) value, separator);
          break;
        case CalendarValueType.Binary:
          byte[] data = value as byte[];
          if (data == null)
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.PrepareStartPropertyValue(Internal.ContentLineParser.Separators.None);
          this.writer.WriteToStream(data);
          this.EndProperty();
          break;
        case CalendarValueType.Boolean:
          if (!(value is bool))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((bool) value, separator);
          break;
        case CalendarValueType.Date:
          if (!(value is DateTime))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((DateTime) value, CalendarValueType.Date, separator);
          break;
        default:
          throw new InvalidDataException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
      }
    }

    internal void WritePropertyValue(CalendarPeriod value, CalendarValueSeparators separator)
    {
      this.WritePropertyValue(value.ToString(), separator);
    }

    internal void WritePropertyValue(int value, CalendarValueSeparators separator)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), separator);
    }

    internal void WritePropertyValue(float value, CalendarValueSeparators separator)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), separator);
    }

    internal void WritePropertyValue(bool value, CalendarValueSeparators separator)
    {
      this.WritePropertyValue(value ? "TRUE" : "FALSE", separator);
    }

    internal void WritePropertyValue(DateTime value, CalendarValueSeparators separator)
    {
      this.WritePropertyValue(value, this.valueType, separator);
    }

    internal void WritePropertyValue(CalendarTime value, CalendarValueSeparators separator)
    {
      this.WritePropertyValue(value.ToString(), separator);
    }

    internal void WritePropertyValue(TimeSpan value, CalendarValueSeparators separator)
    {
      this.WritePropertyValue(value, this.valueType, separator);
    }

    internal void SetLooseMode()
    {
      this.validate = false;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && this.state != WriteState.Closed)
        this.writer.Dispose();
      this.state = WriteState.Closed;
    }

    private static string GetEscapedText(string data)
    {
      int length1 = data.IndexOfAny(CalendarWriter.PropertyValueSpecials);
      if (-1 == length1)
        return data;
      int length2 = data.Length;
      StringBuilder stringBuilder1 = new StringBuilder(data, 0, length1, length2);
      int startIndex;
      while (true)
      {
        stringBuilder1.Append('\\');
        bool flag = 13 == (int) data[length1];
        if (flag || 10 == (int) data[length1])
        {
          stringBuilder1.Append('n');
          startIndex = length1 + 1;
          if (flag && startIndex < data.Length && 10 == (int) data[startIndex])
            ++startIndex;
        }
        else
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          string str = data;
          int index = length1;
          int num1 = 1;
          startIndex = index + num1;
          int num2 = (int) str[index];
          stringBuilder2.Append((char) num2);
        }
        if (startIndex != data.Length)
        {
          int num = data.IndexOfAny(CalendarWriter.PropertyValueSpecials, startIndex);
          if (-1 != num)
          {
            stringBuilder1.Append(data, startIndex, num - startIndex);
            length1 = num;
          }
          else
            break;
        }
        else
          goto label_11;
      }
      stringBuilder1.Append(data, startIndex, length2 - startIndex);
label_11:
      return stringBuilder1.ToString();
    }

    private void WritePropertyValue(DateTime value, CalendarValueType valueType, CalendarValueSeparators separator)
    {
      string str;
      if (CalendarValueType.DateTime == valueType || CalendarValueType.Text == valueType)
      {
        str = CalendarCommon.FormatDateTime(value);
      }
      else
      {
        if (CalendarValueType.Date != valueType)
          throw new ArgumentOutOfRangeException("valueType");
        str = CalendarCommon.FormatDate(value);
      }
      this.WritePropertyValue(str, separator);
    }

    private void WritePropertyValue(TimeSpan value, CalendarValueType valueType, CalendarValueSeparators separator)
    {
      string str;
      if (CalendarValueType.Duration == valueType)
      {
        str = CalendarCommon.FormatDuration(value);
      }
      else
      {
        if (CalendarValueType.UtcOffset != valueType)
          throw new ArgumentOutOfRangeException("valueType");
        if (value.Days > 0 && this.validate)
          throw new ArgumentException(CtsResources.CalendarStrings.UtcOffsetTimespanCannotContainDays, "value");
        str = CalendarCommon.FormatUtcOffset(value);
      }
      this.WritePropertyValue(str, separator);
    }

    private void EndProperty()
    {
      this.EndParameter();
      if (this.state != WriteState.Property)
        return;
      if (this.writer.State != Internal.ContentLineWriteState.PropertyValue)
        this.writer.WriteStartValue();
      this.writer.EndProperty();
      this.Load();
    }

    private void EndParameter()
    {
      if (this.state != WriteState.Parameter)
        return;
      this.writer.EndParameter();
      this.state = WriteState.Property;
    }

    private bool IsQuotingRequired(string value)
    {
      int startIndex = value.IndexOfAny(CalendarWriter.ParameterValueSpecials);
      if (-1 == startIndex)
        return false;
      if (-1 != value.IndexOf('"', startIndex) && this.validate)
        throw new ArgumentException(CtsResources.CalendarStrings.ParameterValuesCannotContainDoubleQuote);
      return true;
    }

    private void PrepareStartPropertyValue(Internal.ContentLineParser.Separators separator)
    {
      this.EndParameter();
      this.AssertValidState(WriteState.Property);
      if (this.firstPropertyValue)
      {
        this.writer.WriteStartValue();
        this.firstPropertyValue = false;
      }
      else
        this.writer.WriteNextValue(separator);
    }

    private void AssertValidState(WriteState state)
    {
      if (this.state == WriteState.Closed)
        throw new ObjectDisposedException("CalendarWriter");
      if ((state & this.state) == (WriteState) 0)
        throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidStateForOperation);
    }

    private void Save()
    {
      CalendarWriter.WriterState writerState;
      writerState.ComponentId = this.componentId;
      writerState.Property = this.property;
      writerState.PropertyName = this.propertyName;
      writerState.ComponentName = this.componentName;
      writerState.ValueType = this.valueType;
      writerState.State = this.state;
      this.stateStack.Push(writerState);
    }

    private void Load()
    {
      CalendarWriter.WriterState writerState = this.stateStack.Pop();
      this.componentId = writerState.ComponentId;
      this.property = writerState.Property;
      this.propertyName = writerState.PropertyName;
      this.componentName = writerState.ComponentName;
      this.valueType = writerState.ValueType;
      this.state = writerState.State;
    }

    private struct WriterState
    {
      public ComponentId ComponentId;
      public PropertyId Property;
      public CalendarValueType ValueType;
      public string ComponentName;
      public string PropertyName;
      public WriteState State;
    }
  }
}
