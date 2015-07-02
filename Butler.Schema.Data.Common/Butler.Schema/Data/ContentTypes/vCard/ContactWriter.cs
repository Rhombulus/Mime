// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.vCard.ContactWriter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.vCard
{
  public class ContactWriter : IDisposable
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
    private WriteState state = WriteState.Start;
    private bool validate = true;
    private const char CR = '\r';
    private const char LF = '\n';
    private const string ComponentStartTag = "BEGIN";
    private const string ComponentEndTag = "END";
    private ParameterId parameter;
    private ContactValueType valueType;
    private string propertyName;
    private Internal.ContentLineWriter writer;
    private bool firstPropertyValue;
    private bool firstParameterValue;
    private Encoding encoding;

    public ContactWriter(Stream stream)
      : this(stream, Encoding.UTF8)
    {
    }

    public ContactWriter(Stream stream, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!stream.CanWrite)
        throw new ArgumentException(CtsResources.CalendarStrings.StreamMustAllowWrite, "stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      this.writer = new Internal.ContentLineWriter(stream, encoding);
      this.encoding = encoding;
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

    public void StartVCard()
    {
      this.EndProperty();
      this.AssertValidState(WriteState.Start);
      this.writer.WriteProperty("BEGIN", "VCARD");
      this.state = WriteState.Component;
    }

    public void EndVCard()
    {
      this.EndProperty();
      this.AssertValidState(WriteState.Component);
      this.writer.WriteProperty("END", "VCARD");
      this.state = WriteState.Start;
    }

    public void StartProperty(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (this.validate && name.Length == 0)
        throw new ArgumentException();
      PropertyId propertyEnum = ContactCommon.GetPropertyEnum(name);
      this.StartProperty(name, propertyEnum);
    }

    public void StartProperty(PropertyId propertyId)
    {
      string propertyString = ContactCommon.GetPropertyString(propertyId);
      if (propertyString == null)
        throw new ArgumentException(CtsResources.CalendarStrings.InvalidPropertyId);
      this.StartProperty(propertyString, propertyId);
    }

    public void StartParameter(string name)
    {
      if (this.validate)
      {
        if (name == null)
          throw new ArgumentNullException("name");
        if (name.Length == 0)
          throw new ArgumentException();
      }
      this.EndParameter();
      this.AssertValidState(WriteState.Property);
      this.parameter = ContactCommon.GetParameterEnum(name);
      this.writer.StartParameter(name);
      this.firstParameterValue = true;
      this.state = WriteState.Parameter;
    }

    public void StartParameter(ParameterId parameterId)
    {
      string parameterString = ContactCommon.GetParameterString(parameterId);
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
        this.valueType = ContactCommon.GetValueTypeEnum(value);
      bool flag = this.IsQuotingRequired(value);
      if (flag)
        this.writer.WriteToStream((byte) 34);
      this.writer.WriteToStream(value);
      if (!flag)
        return;
      this.writer.WriteToStream((byte) 34);
    }

    public void WritePropertyValue(string value, ContactValueSeparators separator)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.PrepareStartPropertyValue((Internal.ContentLineParser.Separators) separator);
      if (this.valueType == ContactValueType.Text || this.valueType == ContactValueType.PhoneNumber || this.valueType == ContactValueType.VCard)
        value = ContactWriter.GetEscapedText(value);
      this.writer.WriteToStream(value);
    }

    public void WritePropertyValue(string value)
    {
      this.WritePropertyValue(value, ContactValueSeparators.Comma);
    }

    public void WritePropertyValue(object value, ContactValueSeparators separator)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      switch (this.valueType)
      {
        case ContactValueType.Unknown:
        case ContactValueType.Text:
        case ContactValueType.Uri:
        case ContactValueType.VCard:
        case ContactValueType.PhoneNumber:
          string str = value as string;
          if (str == null)
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue(str, separator);
          break;
        case ContactValueType.Binary:
          byte[] numArray = value as byte[];
          if (numArray == null)
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue(numArray);
          break;
        case ContactValueType.Boolean:
          if (!(value is bool))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((bool) value, separator);
          break;
        case ContactValueType.Date:
          if (!(value is DateTime))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((DateTime) value, ContactValueType.Date, separator);
          break;
        case ContactValueType.DateTime:
          if (!(value is DateTime))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((DateTime) value, separator);
          break;
        case ContactValueType.Float:
          if (!(value is double))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((double) value, separator);
          break;
        case ContactValueType.Integer:
          if (!(value is int))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((int) value, separator);
          break;
        case ContactValueType.Time:
          if (!(value is DateTime))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((DateTime) value, ContactValueType.Time, separator);
          break;
        case ContactValueType.UtcOffset:
          if (!(value is TimeSpan))
            throw new ArgumentException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
          this.WritePropertyValue((TimeSpan) value, separator);
          break;
        default:
          throw new InvalidDataException(CtsResources.CalendarStrings.InvalidValueTypeForProperty);
      }
    }

    public void WritePropertyValue(object value)
    {
      this.WritePropertyValue(value, ContactValueSeparators.Comma);
    }

    public void WritePropertyValue(int value, ContactValueSeparators separator)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), separator);
    }

    public void WritePropertyValue(int value)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }

    public void WritePropertyValue(double value, ContactValueSeparators separator)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), separator);
    }

    public void WritePropertyValue(double value)
    {
      this.WritePropertyValue(value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }

    public void WritePropertyValue(bool value, ContactValueSeparators separator)
    {
      this.WritePropertyValue(value ? "TRUE" : "FALSE", separator);
    }

    public void WritePropertyValue(bool value)
    {
      this.WritePropertyValue(value ? "TRUE" : "FALSE");
    }

    public void WritePropertyValue(DateTime value, ContactValueSeparators separator)
    {
      this.WritePropertyValue(value, this.valueType, separator);
    }

    public void WritePropertyValue(DateTime value)
    {
      this.WritePropertyValue(value, this.valueType, ContactValueSeparators.Comma);
    }

    public void WritePropertyValue(byte[] value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.PrepareStartPropertyValue(Internal.ContentLineParser.Separators.None);
      this.writer.WriteToStream(value);
      this.EndProperty();
    }

    public void WritePropertyValue(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!stream.CanRead)
        throw new ArgumentException();
      this.PrepareStartPropertyValue(Internal.ContentLineParser.Separators.None);
      if (this.valueType == ContactValueType.Binary)
      {
        byte[] numArray = new byte[4096];
        while (true)
        {
          int length = stream.Read(numArray, 0, numArray.Length);
          if (length != 0)
            this.writer.WriteToStream(numArray, 0, length);
          else
            break;
        }
      }
      else
      {
        StreamReader streamReader = new StreamReader(stream, this.encoding);
        char[] buffer = new char[256];
        char[] data = new char[buffer.Length * 2];
        bool flag = false;
        while (true)
        {
          int num1 = streamReader.ReadBlock(buffer, 0, buffer.Length);
          if (num1 != 0)
          {
            int size = 0;
            for (int index1 = 0; index1 < num1; ++index1)
            {
              switch (buffer[index1])
              {
                case ',':
                case ';':
                case '\\':
                  char[] chArray1 = data;
                  int index2 = size;
                  int num2 = 1;
                  int num3 = index2 + num2;
                  int num4 = 92;
                  chArray1[index2] = (char) num4;
                  char[] chArray2 = data;
                  int index3 = num3;
                  int num5 = 1;
                  size = index3 + num5;
                  int num6 = (int) buffer[index1];
                  chArray2[index3] = (char) num6;
                  flag = false;
                  break;
                case '\n':
                  if (!flag)
                  {
                    char[] chArray3 = data;
                    int index4 = size;
                    int num7 = 1;
                    int num8 = index4 + num7;
                    int num9 = 92;
                    chArray3[index4] = (char) num9;
                    char[] chArray4 = data;
                    int index5 = num8;
                    int num10 = 1;
                    size = index5 + num10;
                    int num11 = 110;
                    chArray4[index5] = (char) num11;
                  }
                  flag = false;
                  break;
                case '\r':
                  char[] chArray5 = data;
                  int index6 = size;
                  int num12 = 1;
                  int num13 = index6 + num12;
                  int num14 = 92;
                  chArray5[index6] = (char) num14;
                  char[] chArray6 = data;
                  int index7 = num13;
                  int num15 = 1;
                  size = index7 + num15;
                  int num16 = 110;
                  chArray6[index7] = (char) num16;
                  flag = true;
                  break;
                default:
                  data[size++] = buffer[index1];
                  flag = false;
                  break;
              }
            }
            this.writer.WriteChars(data, 0, size);
          }
          else
            break;
        }
      }
      this.EndProperty();
    }

    public void WriteContact(ContactReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.ComplianceMode == ContactComplianceMode.Loose)
        this.SetLooseMode();
      while (reader.ReadNext())
      {
        this.StartVCard();
        ContactPropertyReader propertyReader = reader.PropertyReader;
        while (propertyReader.ReadNextProperty())
          this.WriteProperty(propertyReader);
        this.EndVCard();
      }
    }

    public void WriteProperty(ContactPropertyReader reader)
    {
      this.StartProperty(reader.Name);
      ContactParameterReader parameterReader = reader.ParameterReader;
      while (parameterReader.ReadNextParameter())
        this.WriteParameter(parameterReader);
      ContactValueSeparators separator = ContactValueSeparators.None;
      ContactValueSeparators expectedSeparators = ContactValueSeparators.Comma | ContactValueSeparators.Semicolon;
      switch (reader.ValueType)
      {
        case ContactValueType.Binary:
          expectedSeparators = ContactValueSeparators.None;
          break;
        case ContactValueType.Date:
        case ContactValueType.DateTime:
        case ContactValueType.Time:
          expectedSeparators = ContactValueSeparators.Semicolon;
          break;
      }
      while (reader.ReadNextValue())
      {
        this.WritePropertyValue(reader.ReadValue(expectedSeparators), separator);
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

    public void WriteParameter(ContactParameterReader reader)
    {
      this.StartParameter(reader.Name);
      if (reader.Name != null)
      {
        while (reader.ReadNextValue())
          this.WriteParameterValue(reader.ReadValue());
      }
      else
        this.WriteParameterValue(reader.ReadValue());
      this.EndParameter();
    }

    public void WriteParameter(string name, string value)
    {
      this.StartParameter(name);
      this.WriteParameterValue(value);
      this.EndParameter();
    }

    public void WriteValueTypeParameter(ContactValueType type)
    {
      this.StartParameter(ParameterId.ValueType);
      this.WriteParameterValue(ContactCommon.GetValueTypeString(type));
      this.EndParameter();
    }

    public void WriteParameter(ParameterId parameterId, string value)
    {
      this.StartParameter(parameterId);
      this.WriteParameterValue(value);
      this.EndParameter();
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
      int length1 = data.IndexOfAny(ContactWriter.PropertyValueSpecials);
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
          int num = data.IndexOfAny(ContactWriter.PropertyValueSpecials, startIndex);
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

    private void StartProperty(string name, PropertyId p)
    {
      this.EndProperty();
      this.AssertValidState(WriteState.Component);
      this.propertyName = name.ToUpper();
      this.valueType = ContactCommon.GetDefaultValueType(p);
      this.writer.StartProperty(this.propertyName);
      this.firstPropertyValue = true;
      this.state = WriteState.Property;
    }

    private void WritePropertyValue(DateTime value, ContactValueType valueType, ContactValueSeparators separator)
    {
      string str;
      if (ContactValueType.DateTime == valueType || ContactValueType.Text == valueType)
        str = ContactCommon.FormatDateTime(value);
      else if (ContactValueType.Date == valueType)
      {
        str = ContactCommon.FormatDate(value);
      }
      else
      {
        if (ContactValueType.Time != valueType)
          throw new ArgumentOutOfRangeException("valueType");
        str = ContactCommon.FormatTime(value);
      }
      this.WritePropertyValue(str, separator);
    }

    private void WritePropertyValue(TimeSpan value, ContactValueSeparators separator)
    {
      if (ContactValueType.UtcOffset != this.valueType)
        throw new ArgumentOutOfRangeException("valueType");
      if (value.Days > 0 && this.validate)
        throw new ArgumentException(CtsResources.CalendarStrings.UtcOffsetTimespanCannotContainDays, "value");
      this.WritePropertyValue(ContactCommon.FormatUtcOffset(value), separator);
    }

    private void EndProperty()
    {
      this.EndParameter();
      if (this.state != WriteState.Property)
        return;
      if (this.writer.State != Internal.ContentLineWriteState.PropertyValue)
        this.writer.WriteStartValue();
      this.writer.EndProperty();
      this.state = WriteState.Component;
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
      int startIndex = value.IndexOfAny(ContactWriter.ParameterValueSpecials);
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
        throw new ObjectDisposedException("ContactWriter");
      if ((state & this.state) == (WriteState) 0)
        throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidStateForOperation);
    }
  }
}
