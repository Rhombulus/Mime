// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.ContentLineReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  internal class ContentLineReader : IDisposable
  {
    private Stack<string> componentStack = new Stack<string>();
    private char[] charBuffer = new char[256];
    private StringBuilder stringBuilder = new StringBuilder();
    private const int CharBufferSize = 256;
    private const int MaxNameLength = 255;
    private ContentLineParser parser;
    private ComplianceTracker complianceTracker;
    private ContentLineNodeType nodeType;
    private string parameterName;
    private string unnamedParameterValue;
    private string propertyName;
    private string componentName;
    private Stream valueStream;
    private ValueTypeContainer valueType;
    private ContentLineParser.Separators propertyValueSeparator;
    private ContentLineParser.Separators parameterValueSeparator;
    private bool parameterValueRead;
    private bool propertyValueRead;
    private bool isDisposed;

    public int Depth
    {
      get
      {
        this.CheckDisposed("Depth::get");
        if (ContentLineNodeType.BeforeComponentStart != this.nodeType)
          return this.componentStack.Count;
        return this.componentStack.Count - 1;
      }
    }

    public string ComponentName
    {
      get
      {
        this.CheckDisposed("ComponentName::get");
        return this.componentName;
      }
    }

    public string PropertyName
    {
      get
      {
        this.CheckDisposed("PropertyName::get");
        return this.propertyName;
      }
    }

    public string ParameterName
    {
      get
      {
        this.CheckDisposed("ParameterName::get");
        return this.parameterName;
      }
    }

    public ContentLineNodeType Type
    {
      get
      {
        this.CheckDisposed("Type::get");
        return this.nodeType;
      }
    }

    public ComplianceTracker ComplianceTracker
    {
      get
      {
        this.CheckDisposed("ComplianceTracker::get");
        return this.complianceTracker;
      }
    }

    public ValueTypeContainer ValueType
    {
      get
      {
        this.CheckDisposed("ValueType::get");
        if (!this.valueType.IsInitialized)
          this.valueType.SetPropertyName(this.PropertyName);
        return this.valueType;
      }
    }

    public Encoding CurrentCharsetEncoding
    {
      get
      {
        this.CheckDisposed("CurrentEncoding::get");
        return this.parser.CurrentCharsetEncoding;
      }
    }

    public ContentLineReader(Stream s, Encoding encoding, ComplianceTracker complianceTracker, ValueTypeContainer container)
    {
      this.valueType = container;
      this.parser = new ContentLineParser(s, encoding, complianceTracker);
      this.complianceTracker = complianceTracker;
    }

    protected virtual void CheckDisposed(string methodName)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("ContentLineReader", methodName);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && !this.isDisposed && this.parser != null)
      {
        this.parser.Dispose();
        this.parser = (ContentLineParser) null;
      }
      this.isDisposed = true;
    }

    public bool ReadNextComponent()
    {
      this.CheckDisposed("ReadNextComponent");
      this.DrainValueStream();
      while (this.Read())
      {
        if (this.nodeType == ContentLineNodeType.ComponentStart)
          return true;
      }
      return false;
    }

    public bool ReadFirstChildComponent()
    {
      this.CheckDisposed("ReadFirstChildComponent");
      this.DrainValueStream();
      if (this.nodeType == ContentLineNodeType.ComponentEnd || this.nodeType == ContentLineNodeType.BeforeComponentEnd)
        return false;
      while (this.Read())
      {
        if (this.nodeType == ContentLineNodeType.ComponentStart)
          return true;
        if (this.nodeType == ContentLineNodeType.ComponentEnd)
          return false;
      }
      return false;
    }

    public bool ReadNextSiblingComponent()
    {
      this.CheckDisposed("ReadNextSiblingComponent");
      this.DrainValueStream();
      int depth = this.Depth;
      do
        ;
      while ((this.nodeType != ContentLineNodeType.ComponentEnd || this.Depth > depth) && this.Read());
      if (this.nodeType == ContentLineNodeType.ComponentEnd)
        this.Read();
      do
        ;
      while (this.nodeType == ContentLineNodeType.Property && this.Read());
      if (this.nodeType == ContentLineNodeType.BeforeComponentStart)
        this.Read();
      return this.nodeType == ContentLineNodeType.ComponentStart;
    }

    private bool Read()
    {
      switch (this.nodeType)
      {
        case ContentLineNodeType.DocumentStart:
        case ContentLineNodeType.ComponentStart:
          if (this.parser.State == ContentLineParser.States.End)
          {
            if (this.componentStack.Count != 0)
            {
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.NotAllComponentsClosed, CtsResources.CalendarStrings.NotAllComponentsClosed);
              this.nodeType = ContentLineNodeType.BeforeComponentEnd;
            }
            else
              this.nodeType = ContentLineNodeType.DocumentEnd;
            return false;
          }
          this.nodeType = ContentLineNodeType.Property;
          this.propertyName = this.ReadName();
          this.parameterValueRead = false;
          this.propertyValueRead = false;
          if (this.parser.State == ContentLineParser.States.End)
          {
            if (0 < this.propertyName.Length)
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
            if (0 < this.componentStack.Count)
            {
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.NotAllComponentsClosed, CtsResources.CalendarStrings.NotAllComponentsClosed);
              this.nodeType = ContentLineNodeType.BeforeComponentEnd;
            }
            else
              this.nodeType = ContentLineNodeType.DocumentEnd;
            return false;
          }
          if (this.propertyName.Equals("BEGIN", StringComparison.OrdinalIgnoreCase))
          {
            if (this.parser.State == ContentLineParser.States.ParamName)
            {
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.ParametersOnComponentTag, CtsResources.CalendarStrings.ParametersNotPermittedOnComponentTag);
              while (this.ReadNextParameter())
                ;
            }
            if (this.parser.State != ContentLineParser.States.End)
            {
              this.componentStack.Push(this.componentName);
              this.componentName = this.ReadPropertyValue(true).Trim();
              if (this.componentName.Length == 0)
                this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyComponentName, CtsResources.CalendarStrings.EmptyComponentName);
              this.nodeType = ContentLineNodeType.BeforeComponentStart;
            }
          }
          else if (this.propertyName.Equals("END", StringComparison.OrdinalIgnoreCase))
          {
            if (this.parser.State == ContentLineParser.States.ParamName)
            {
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.ParametersOnComponentTag, CtsResources.CalendarStrings.ParametersNotPermittedOnComponentTag);
              while (this.ReadNextParameter())
                ;
            }
            if (this.parser.State != ContentLineParser.States.End)
            {
              string str = this.ReadPropertyValue(true).Trim();
              if (this.componentStack.Count == 0)
                this.complianceTracker.SetComplianceStatus(ComplianceStatus.EndTagWithoutBegin, CtsResources.CalendarStrings.EndTagWithoutBegin);
              if (!str.Equals(this.componentName, StringComparison.OrdinalIgnoreCase))
              {
                if (str.Length == 0)
                  this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyComponentName, CtsResources.CalendarStrings.EmptyComponentName);
                this.complianceTracker.SetComplianceStatus(ComplianceStatus.ComponentNameMismatch, CtsResources.CalendarStrings.ComponentNameMismatch);
              }
              this.nodeType = ContentLineNodeType.BeforeComponentEnd;
            }
          }
          else if (this.propertyName.Length == 0)
          {
            if (0 < this.componentStack.Count)
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyPropertyName, CtsResources.CalendarStrings.EmptyPropertyName);
          }
          else if (this.componentStack.Count == 0)
            this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyOutsideOfComponent, CtsResources.CalendarStrings.PropertyOutsideOfComponent);
          return true;
        case ContentLineNodeType.ComponentEnd:
          if (this.componentStack.Count > 0)
          {
            this.componentName = this.componentStack.Pop();
            goto case ContentLineNodeType.DocumentStart;
          }
          else
            goto case ContentLineNodeType.DocumentStart;
        case ContentLineNodeType.Parameter:
        case ContentLineNodeType.Property:
          if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
          {
            while (this.ReadNextParameter())
              ;
          }
          if (this.parser.State == ContentLineParser.States.Value || this.parser.State == ContentLineParser.States.ValueStart)
            this.ReadPropertyValue(false);
          if (this.parser.State == ContentLineParser.States.ValueStartComma || this.parser.State == ContentLineParser.States.ValueStartSemiColon)
          {
            while (this.ReadNextPropertyValue())
              ;
            goto case ContentLineNodeType.DocumentStart;
          }
          else
            goto case ContentLineNodeType.DocumentStart;
        case ContentLineNodeType.BeforeComponentStart:
          this.nodeType = ContentLineNodeType.ComponentStart;
          return true;
        case ContentLineNodeType.BeforeComponentEnd:
          this.nodeType = ContentLineNodeType.ComponentEnd;
          return true;
        default:
          return false;
      }
    }

    public bool ReadNextProperty()
    {
      this.CheckDisposed("ReadNextProperty");
      this.DrainValueStream();
      this.parameterValueRead = false;
      this.propertyValueRead = false;
      if (this.parser.State == ContentLineParser.States.End || this.nodeType == ContentLineNodeType.BeforeComponentEnd || (this.nodeType == ContentLineNodeType.BeforeComponentStart || !this.Read()) || (this.nodeType == ContentLineNodeType.BeforeComponentEnd || this.nodeType == ContentLineNodeType.ComponentEnd || this.nodeType != ContentLineNodeType.Property))
        return false;
      this.parameterName = (string) null;
      this.valueType.Reset();
      this.propertyValueSeparator = ContentLineParser.Separators.Comma;
      return true;
    }

    public bool ReadNextParameter()
    {
      this.CheckDisposed("ReadNextParameter");
      if (this.parser.State == ContentLineParser.States.ParamValueStart || this.parser.State == ContentLineParser.States.ParamValueQuoted || this.parser.State == ContentLineParser.States.ParamValueUnquoted)
      {
        while (this.ReadNextParameterValue())
          ;
      }
      if (this.parser.State != ContentLineParser.States.ParamName)
        return false;
      this.nodeType = ContentLineNodeType.Parameter;
      this.parameterName = this.ReadName();
      this.parameterValueSeparator = ContentLineParser.Separators.Comma;
      if (this.parser.State == ContentLineParser.States.UnnamedParamEnd)
      {
        this.unnamedParameterValue = this.parameterName;
        this.parameterName = (string) null;
        int filled;
        this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
      }
      else if (this.parameterName.Length == 0)
        this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyParameterName, CtsResources.CalendarStrings.EmptyParameterName);
      this.parameterValueRead = false;
      return true;
    }

    public bool ReadNextParameterValue()
    {
      this.CheckDisposed("ReadNextParameterValue");
      if (this.parameterValueSeparator == ContentLineParser.Separators.None)
        throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidReaderState);
      if (!this.parameterValueRead && this.parser.State != ContentLineParser.States.ParamValueStart && this.parser.State != ContentLineParser.States.UnnamedParamEnd)
        this.ReadParameterValue(false);
      bool flag = false;
      if (this.parser.State == ContentLineParser.States.UnnamedParamEnd)
      {
        flag = true;
        int filled;
        this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
      }
      if (this.parser.State == ContentLineParser.States.ParamValueStart)
      {
        int filled;
        this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
      }
      if (flag || this.parser.State == ContentLineParser.States.ParamValueUnquoted || this.parser.State == ContentLineParser.States.ParamValueQuoted)
      {
        this.parameterValueRead = false;
        return true;
      }
      this.parameterValueRead = true;
      return false;
    }

    public string ReadParameterValue(bool returnValue)
    {
      this.CheckDisposed("ReadParameterValue");
      if (this.parameterValueRead)
        throw new InvalidOperationException(CtsResources.CalendarStrings.ValueAlreadyRead);
      if (this.unnamedParameterValue != null && this.parameterName == null)
      {
        string str = (string) null;
        if (returnValue)
          str = this.unnamedParameterValue;
        this.unnamedParameterValue = (string) null;
        this.parameterValueRead = true;
        return str;
      }
      int filled;
      if (this.parser.State == ContentLineParser.States.ParamValueStart)
      {
        this.parameterValueSeparator = ContentLineParser.Separators.None;
        this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
      }
      if (this.parser.State == ContentLineParser.States.Value || this.parser.State == ContentLineParser.States.End)
      {
        this.parameterValueRead = true;
        return string.Empty;
      }
      if (this.parser.State != ContentLineParser.States.ParamValueUnquoted && this.parser.State != ContentLineParser.States.ParamValueQuoted)
        throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidReaderState);
      bool flag1 = string.Compare(this.parameterName, "VALUE", StringComparison.OrdinalIgnoreCase) == 0;
      string str1 = (string) null;
      if (returnValue || flag1)
      {
        this.stringBuilder.Length = 0;
        bool flag2;
        do
        {
          flag2 = this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, this.parameterValueSeparator);
          this.stringBuilder.Append(this.charBuffer, 0, filled);
        }
        while (flag2);
        str1 = this.stringBuilder.ToString();
      }
      else
      {
        while (this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, this.parameterValueSeparator))
          ;
      }
      if (flag1 && str1.Length > 0)
        this.valueType.SetValueTypeParameter(str1);
      this.parameterValueRead = true;
      return str1;
    }

    public bool ReadNextPropertyValue()
    {
      this.CheckDisposed("ReadNextPropertyValue");
      if (this.propertyValueSeparator == ContentLineParser.Separators.None)
        throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidReaderState);
      if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
      {
        while (this.ReadNextParameter())
          ;
      }
      if (!this.propertyValueRead && this.parser.State != ContentLineParser.States.ValueStart)
        this.ReadPropertyValue(false);
      if (this.parser.State == ContentLineParser.States.ValueStart || this.parser.State == ContentLineParser.States.ValueStartComma || this.parser.State == ContentLineParser.States.ValueStartSemiColon)
      {
        int filled;
        this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
      }
      this.DrainValueStream();
      this.parameterName = (string) null;
      bool flag = this.parser.State == ContentLineParser.States.Value;
      this.propertyValueRead = !flag;
      return flag;
    }

    private string ReadPropertyValue(bool returnValue)
    {
      ContentLineParser.Separators endSeparator;
      return this.ReadPropertyValue(returnValue, ContentLineParser.Separators.None, true, out endSeparator);
    }

    public string ReadPropertyValue(bool returnValue, ContentLineParser.Separators expectedSeparators, bool useDefaultSeparator, out ContentLineParser.Separators endSeparator)
    {
      this.CheckDisposed("ReadPropertyValue");
      if (this.propertyValueRead)
        throw new InvalidOperationException(CtsResources.CalendarStrings.ValueAlreadyRead);
      endSeparator = ContentLineParser.Separators.None;
      if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
      {
        while (this.ReadNextParameter())
          ;
      }
      if (!useDefaultSeparator)
        this.propertyValueSeparator = expectedSeparators;
      int filled;
      if (this.parser.State == ContentLineParser.States.ValueStart)
      {
        if (useDefaultSeparator)
          this.propertyValueSeparator = ContentLineParser.Separators.None;
        this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
      }
      this.DrainValueStream();
      if (this.parser.State == ContentLineParser.States.End)
      {
        this.propertyValueRead = true;
        return string.Empty;
      }
      ContentLineParser.Separators separators = this.propertyValueSeparator;
      if (!this.ValueType.CanBeMultivalued)
        separators &= ~ContentLineParser.Separators.Comma;
      if (!this.ValueType.CanBeCompound)
        separators &= ~ContentLineParser.Separators.SemiColon;
      string str = (string) null;
      if (returnValue)
      {
        this.stringBuilder.Length = 0;
        bool flag;
        do
        {
          flag = this.parser.ParseElement(this.charBuffer, 0, 256, out filled, this.ValueType.IsTextType, separators);
          this.stringBuilder.Append(this.charBuffer, 0, filled);
        }
        while (flag);
        str = this.stringBuilder.ToString();
      }
      else
      {
        while (this.parser.ParseElement(this.charBuffer, 0, 256, out filled, this.ValueType.IsTextType, separators))
          ;
      }
      this.propertyValueRead = true;
      this.parameterName = (string) null;
      if (this.parser.State == ContentLineParser.States.ValueStartComma)
        endSeparator = ContentLineParser.Separators.Comma;
      else if (this.parser.State == ContentLineParser.States.ValueStartSemiColon)
        endSeparator = ContentLineParser.Separators.SemiColon;
      return str;
    }

    public void ApplyValueOverrides(Encoding charset, Mime.Encoders.ByteEncoder decoder)
    {
      this.parser.ApplyValueOverrides(charset, decoder);
    }

    public Stream GetValueReadStream()
    {
      this.CheckDisposed("GetValueReadStream");
      if (this.propertyValueRead)
        throw new InvalidOperationException(CtsResources.CalendarStrings.ValueAlreadyRead);
      if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
      {
        while (this.ReadNextParameter())
          ;
      }
      if (this.parser.State == ContentLineParser.States.ValueStart)
      {
        int filled;
        this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
      }
      this.DrainValueStream();
      this.propertyValueRead = true;
      if (this.parser.State != ContentLineParser.States.Value)
        return (Stream) new MemoryStream(new byte[0], false);
      this.valueStream = this.ValueType.IsTextType ? (Stream) new ContentLineReader.ValueStream(this.parser) : this.parser.GetValueReadStream();
      return this.valueStream;
    }

    public void AssertValidState(ContentLineNodeType nodeType)
    {
      this.CheckDisposed("AssertValidState");
      if ((this.nodeType & nodeType) == ContentLineNodeType.DocumentStart)
        throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidReaderState);
    }

    private void DrainValueStream()
    {
      if (this.valueStream == null)
        return;
      this.valueStream.Dispose();
      this.valueStream = (Stream) null;
    }

    private string ReadName()
    {
      this.stringBuilder.Length = 0;
      bool flag;
      do
      {
        int filled;
        flag = this.parser.ParseElement(this.charBuffer, 0, 256, out filled, false, ContentLineParser.Separators.None);
        if (this.stringBuilder.Length < (int) byte.MaxValue)
          this.stringBuilder.Append(this.charBuffer, 0, Math.Min(filled, (int) byte.MaxValue - this.stringBuilder.Length));
      }
      while (flag);
      return this.stringBuilder.ToString().Trim();
    }

    private class ValueStream : Stream
    {
      private char[] charBuffer = new char[256];
      private ContentLineParser parser;
      private bool eof;
      private bool isClosed;
      private int position;
      private byte[] buffer;
      private int bufferOffset;
      private int bufferSize;
      private Encoder encoder;

      public override bool CanRead
      {
        get
        {
          return true;
        }
      }

      public override bool CanWrite
      {
        get
        {
          return false;
        }
      }

      public override bool CanSeek
      {
        get
        {
          return false;
        }
      }

      public override long Length
      {
        get
        {
          throw new NotSupportedException();
        }
      }

      public override long Position
      {
        get
        {
          this.CheckDisposed("Position::get");
          return (long) this.position;
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      public ValueStream(ContentLineParser reader)
      {
        this.parser = reader;
        this.encoder = reader.CurrentCharsetEncoding.GetEncoder();
        this.buffer = new byte[reader.CurrentCharsetEncoding.GetMaxByteCount(this.charBuffer.Length)];
      }

      private void CheckDisposed(string methodName)
      {
        if (this.isClosed)
          throw new ObjectDisposedException("ValueStream", methodName);
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && !this.isClosed)
        {
          byte[] buffer = new byte[1024];
          while (this.Read(buffer, 0, buffer.Length) > 0)
            ;
        }
        this.isClosed = true;
      }

      public override void Flush()
      {
        throw new NotSupportedException();
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        this.CheckDisposed("Read");
        int num = 0;
        if (this.eof)
          return 0;
        if (buffer == null)
          throw new ArgumentNullException("buffer");
        if (offset < 0 || offset > buffer.Length)
          throw new ArgumentOutOfRangeException("offset", CtsResources.CalendarStrings.OffsetOutOfRange);
        if (count < 0)
          throw new ArgumentOutOfRangeException("count", CtsResources.CalendarStrings.CountLessThanZero);
        if (this.bufferSize != 0)
        {
          int count1 = Math.Min(count, this.bufferSize);
          Buffer.BlockCopy((Array) this.buffer, this.bufferOffset, (Array) buffer, offset, count1);
          count -= count1;
          offset += count1;
          this.bufferSize -= count1;
          this.bufferOffset += count1;
          num += count1;
        }
        while (count > 0)
        {
          if (this.bufferSize == 0 && !this.eof)
            this.ReadBuffer();
          if (this.bufferSize != 0)
          {
            int count1 = Math.Min(count, this.bufferSize);
            Buffer.BlockCopy((Array) this.buffer, this.bufferOffset, (Array) buffer, offset, count1);
            this.bufferOffset += count1;
            this.bufferSize -= count1;
            offset += count1;
            count -= count1;
            num += count1;
          }
          else
            break;
        }
        this.position += num;
        return num;
      }

      public override int ReadByte()
      {
        this.CheckDisposed("Read");
        if (this.eof)
          return -1;
        int num;
        if (this.bufferSize != 0)
        {
          num = (int) this.buffer[this.bufferOffset++];
          --this.bufferSize;
          ++this.position;
        }
        else
        {
          if (this.bufferSize == 0 && !this.eof)
            this.ReadBuffer();
          if (this.bufferSize == 0)
            return -1;
          num = (int) this.buffer[this.bufferOffset++];
          --this.bufferSize;
          ++this.position;
        }
        return num;
      }

      public override void SetLength(long value)
      {
        throw new NotSupportedException();
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException();
      }

      public override void WriteByte(byte value)
      {
        throw new NotSupportedException();
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        throw new NotSupportedException();
      }

      private void ReadBuffer()
      {
        this.bufferOffset = 0;
        int filled;
        for (this.bufferSize = 0; !this.eof && this.bufferSize == 0; this.bufferSize = this.encoder.GetBytes(this.charBuffer, 0, filled, this.buffer, 0, this.eof))
          this.eof = !this.parser.ParseElement(this.charBuffer, 0, 256, out filled, true, ContentLineParser.Separators.None);
      }
    }
  }
}
