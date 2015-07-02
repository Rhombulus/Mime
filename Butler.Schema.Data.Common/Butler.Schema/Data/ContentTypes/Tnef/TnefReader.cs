// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  public class TnefReader : IDisposable
  {
    private static readonly DateTime MinDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    private static readonly byte[] NumFromHex = new byte[256]
    {
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 9,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 15,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 15,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16
    };
    private const int ReadBufferSize = 4096;
    private TnefComplianceMode complianceMode;
    private bool checksumDisabled;
    private TnefComplianceStatus complianceStatus;
    internal Stream InputStream;
    private TnefReader parent;
    internal object Child;
    private int embeddingDepth;
    private int streamMaxLength;
    private bool endOfFile;
    private int streamOffset;
    private byte[] readBuffer;
    private int readOffset;
    private int readEnd;
    private int readEndReal;
    internal TnefReader.ReadState ReadStateValue;
    private bool error;
    private int tnefVersion;
    private short attachmentKey;
    private int messageCodepage;
    private TnefAttributeLevel attributeLevel;
    private TnefAttributeTag attributeTag;
    private bool legacyAttribute;
    private int attributeValueStreamOffset;
    private int attributeValueLength;
    private int attributeValueOffset;
    private ushort checksum;
    private ushort attributeStartChecksum;
    private int rowCount;
    private int rowIndex;
    private int propertyCount;
    private int propertyIndex;
    private TnefPropertyTag propertyTag;
    private TnefNameId propertyNameId;
    private int propertyValueCount;
    private int propertyValueIndex;
    private Guid propertyValueIId;
    private int propertyValueStreamOffset;
    private int propertyValueLength;
    private int propertyValueOffset;
    private int propertyValueFixedLength;
    private int propertyValuePaddingLength;
    private int propertyPaddingLength;
    private bool decoderFlushed;
    private Decoder decoder;
    private char[] decodeBuffer;
    private Decoder unicodeDecoder;
    private Decoder string8Decoder;
    private TnefPropertyReader propertyReader;
    private bool directRead;
    private byte[] fabricatedBuffer;
    private int fabricatedOffset;
    private int fabricatedEnd;
    private bool messageClassSPlus;
    private bool messageClassSPlusResponse;
    private bool currentAttachmentIsOle;
    private bool directReadForMessageClass;
    private int tripleNameOffset;
    private int tripleNameLength;
    private int tripleAddressTypeOffset;
    private int tripleAddressTypeLength;
    private int tripleAddressOffset;
    private int tripleAddressLength;

    public TnefComplianceMode ComplianceMode
    {
      get
      {
        this.AssertGoodToUse(false);
        return this.complianceMode;
      }
    }

    public TnefComplianceStatus ComplianceStatus
    {
      get
      {
        this.AssertGoodToUse(false);
        return this.complianceStatus;
      }
    }

    public int StreamOffset
    {
      get
      {
        this.AssertGoodToUse(true);
        return this.streamOffset + this.readOffset;
      }
    }

    public int TnefVersion
    {
      get
      {
        this.AssertGoodToUse(false);
        return this.tnefVersion;
      }
    }

    public short AttachmentKey
    {
      get
      {
        this.AssertGoodToUse(false);
        return this.attachmentKey;
      }
    }

    public int MessageCodepage
    {
      get
      {
        this.AssertGoodToUse(false);
        return this.messageCodepage;
      }
      set
      {
        this.AssertGoodToUse(false);
        this.messageCodepage = value;
        this.string8Decoder = (Decoder) null;
      }
    }

    public TnefAttributeLevel AttributeLevel
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInAttribute();
        return this.attributeLevel;
      }
    }

    public TnefAttributeTag AttributeTag
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInAttribute();
        return this.attributeTag;
      }
    }

    public TnefPropertyReader PropertyReader
    {
      get
      {
        this.AssertGoodToUse(true);
        this.AssertInAttribute();
        if (this.ReadStateValue == TnefReader.ReadState.ReadAttributeValue)
          throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationPropAfterRaw);
        return this.propertyReader;
      }
    }

    public int AttributeRawValueStreamOffset
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInAttribute();
        return this.attributeValueStreamOffset;
      }
    }

    public int AttributeRawValueLength
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInAttribute();
        return this.attributeValueLength;
      }
    }

    internal int RowCount
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInAttribute();
        if (this.attributeTag != TnefAttributeTag.RecipientTable)
          throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationRowsOnlyInRecipientTable);
        return this.rowCount;
      }
    }

    internal int PropertyCount
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInAttribute();
        return this.propertyCount;
      }
    }

    internal TnefPropertyTag PropertyTag
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInProperty();
        return this.propertyTag;
      }
    }

    internal int PropertyValueCount
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInProperty();
        return this.propertyValueCount;
      }
    }

    internal Guid PropertyValueOleIID
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInPropertyValue();
        if (this.propertyTag.ValueTnefType != TnefPropertyType.Object)
          throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationNotObjectProperty);
        return this.propertyValueIId;
      }
    }

    internal bool IsPropertyEmbeddedMessage
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInProperty();
        if (this.propertyTag.ValueTnefType != TnefPropertyType.Object)
          return false;
        this.AssertInPropertyValue();
        return this.propertyValueIId == TnefCommon.MessageIID;
      }
    }

    internal TnefNameId PropertyNameId
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInProperty();
        if (!this.propertyTag.IsNamed)
          throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationNotNamedProperty);
        return this.propertyNameId;
      }
    }

    internal bool IsComputedProperty
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInProperty();
        return !this.directRead;
      }
    }

    internal int PropertyRawValueStreamOffset
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInPropertyValue();
        if (this.IsComputedProperty)
          return -1;
        return this.propertyValueStreamOffset;
      }
    }

    internal int PropertyRawValueLength
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInPropertyValue();
        return this.propertyValueLength;
      }
    }

    internal bool IsLargePropertyValue
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInProperty();
        if (this.propertyTag.TnefType == TnefPropertyType.Null)
          return false;
        this.AssertInPropertyValue();
        if (this.propertyValueLength > 32768)
          return this.propertyValueFixedLength == 0;
        return false;
      }
    }

    internal Type PropertyValueClrType
    {
      get
      {
        this.AssertGoodToUse(false);
        this.AssertInProperty();
        switch (this.propertyTag.ValueTnefType)
        {
          case TnefPropertyType.SysTime:
            return typeof (DateTime);
          case TnefPropertyType.ClassId:
            return typeof (Guid);
          case TnefPropertyType.Binary:
            return typeof (byte[]);
          case TnefPropertyType.I2:
            return typeof (short);
          case TnefPropertyType.Long:
            return typeof (int);
          case TnefPropertyType.R4:
            return typeof (float);
          case TnefPropertyType.Double:
            return typeof (double);
          case TnefPropertyType.Currency:
            return typeof (long);
          case TnefPropertyType.AppTime:
            return typeof (DateTime);
          case TnefPropertyType.Error:
            return typeof (int);
          case TnefPropertyType.Boolean:
            return typeof (bool);
          case TnefPropertyType.Object:
            return typeof (byte[]);
          case TnefPropertyType.I8:
            return typeof (long);
          case TnefPropertyType.String8:
            return typeof (string);
          case TnefPropertyType.Unicode:
            return typeof (string);
          default:
            return (Type) null;
        }
      }
    }

    public TnefReader(Stream inputStream)
      : this(inputStream, 0, TnefComplianceMode.Strict)
    {
    }

    public TnefReader(Stream inputStream, int defaultMessageCodepage, TnefComplianceMode complianceMode)
    {
      if (inputStream == null)
        throw new ArgumentNullException("inputStream");
      if (!inputStream.CanRead)
        throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportRead);
      this.InputStream = inputStream;
      this.streamMaxLength = int.MaxValue;
      this.complianceMode = complianceMode;
      this.complianceStatus = TnefComplianceStatus.Compliant;
      if (TnefCommon.IsUnicodeCodepage(defaultMessageCodepage))
        defaultMessageCodepage = 0;
      this.messageCodepage = defaultMessageCodepage;
      this.readBuffer = new byte[4096];
      this.fabricatedBuffer = new byte[512];
      this.propertyReader = new TnefPropertyReader(this);
      this.unicodeDecoder = Encoding.Unicode.GetDecoder();
      this.ReadTnefHeader();
    }

    internal TnefReader(TnefReader parent)
    {
      this.embeddingDepth = parent.embeddingDepth + 1;
      this.parent = parent;
      this.parent.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      this.InputStream = parent.InputStream;
      this.streamOffset = -parent.readOffset;
      this.streamMaxLength = Math.Min(parent.propertyValueLength, parent.streamMaxLength - parent.propertyValueStreamOffset);
      this.complianceMode = parent.complianceMode;
      this.complianceStatus = TnefComplianceStatus.Compliant;
      this.messageCodepage = parent.MessageCodepage;
      this.checksumDisabled = parent.checksumDisabled;
      this.readBuffer = parent.readBuffer;
      this.readOffset = parent.readOffset;
      this.readEnd = parent.readEnd;
      this.readEndReal = parent.readEndReal;
      this.endOfFile = parent.endOfFile;
      if (this.streamOffset + this.readEnd > this.streamMaxLength)
      {
        this.readEnd = this.streamMaxLength - this.streamOffset;
        this.endOfFile = true;
      }
      this.propertyReader = new TnefPropertyReader(this);
      this.unicodeDecoder = parent.unicodeDecoder;
      this.string8Decoder = parent.string8Decoder;
      this.fabricatedBuffer = parent.fabricatedBuffer;
      this.decodeBuffer = parent.decodeBuffer;
      this.ReadTnefHeader();
      this.parent.Child = (object) this;
    }

    public int ReadAttributeRawValue(byte[] buffer, int offset, int count)
    {
      this.AssertGoodToUse(true);
      this.AssertInAttribute();
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TnefStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountTooLarge);
      if (this.ReadStateValue == TnefReader.ReadState.BeginAttribute)
        this.ReadStateValue = TnefReader.ReadState.ReadAttributeValue;
      else if (this.ReadStateValue != TnefReader.ReadState.ReadAttributeValue)
      {
        if (this.ReadStateValue == TnefReader.ReadState.EndAttribute)
          return 0;
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationRawAfterProp);
      }
      int num = 0;
      while (this.attributeValueOffset < this.attributeValueLength && count != 0)
      {
        if (!this.EnsureMoreDataLoaded(1))
        {
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
          this.ReadStateValue = TnefReader.ReadState.EndAttribute;
          this.error = true;
          break;
        }
        int count1 = Math.Min(count, Math.Min(this.AttributeRemainingCount(), this.AvailableCount()));
        this.ReadBytes(buffer, offset, count1);
        offset += count1;
        count -= count1;
        num += count1;
      }
      if (this.attributeValueOffset == this.attributeValueLength && !this.error)
        this.VerifyAttributeChecksum();
      return num;
    }

    public bool ReadNextAttribute()
    {
      this.AssertGoodToUse(true);
      if (this.ReadStateValue == TnefReader.ReadState.EndOfFile)
        return false;
      if (this.ReadStateValue > TnefReader.ReadState.EndAttribute && this.attributeValueOffset <= this.attributeValueLength)
        this.SkipRemainderOfCurrentAttribute();
      if (this.error)
      {
        this.ReadStateValue = TnefReader.ReadState.EndOfFile;
        return false;
      }
      if (this.ReadAttributeHeader())
        return true;
      this.ReadStateValue = TnefReader.ReadState.EndOfFile;
      return false;
    }

    public void ResetComplianceStatus()
    {
      this.complianceStatus = TnefComplianceStatus.Compliant;
    }

    public void Close()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.InputStream == null)
          return;
        if (this.Child != null)
        {
          if (this.Child is TnefReaderStreamWrapper)
            (this.Child as TnefReaderStreamWrapper).Dispose();
          else if (this.Child is TnefReader)
            (this.Child as TnefReader).Dispose();
        }
        if (this.parent == null)
        {
          this.InputStream.Dispose();
        }
        else
        {
          this.parent.propertyValueOffset += this.StreamOffset;
          this.parent.attributeValueOffset += this.StreamOffset;
          this.parent.checksum += this.checksum;
          this.parent.streamOffset += this.parent.readOffset + this.streamOffset;
          this.parent.readBuffer = this.readBuffer;
          this.parent.readOffset = this.readOffset;
          this.parent.readEnd = this.readEndReal;
          this.parent.readEndReal = this.readEndReal;
          if (this.parent.streamOffset + this.parent.readEnd > this.parent.streamMaxLength)
          {
            this.parent.readEnd = this.parent.streamMaxLength - this.parent.streamOffset;
            this.parent.endOfFile = true;
          }
          this.parent.Child = (object) null;
        }
      }
      this.InputStream = (Stream) null;
      this.parent = (TnefReader) null;
      this.readBuffer = (byte[]) null;
      this.fabricatedBuffer = (byte[]) null;
      this.decoder = (Decoder) null;
      this.unicodeDecoder = (Decoder) null;
      this.string8Decoder = (Decoder) null;
    }

    private void ReadTnefHeader()
    {
      if (!this.EnsureMoreDataLoaded(6))
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefSignature, CtsResources.TnefStrings.ReaderComplianceInvalidTnefSignature);
        this.ReadStateValue = TnefReader.ReadState.EndOfFile;
        this.error = true;
      }
      else if (this.ReadDword() != 574529400)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefSignature, CtsResources.TnefStrings.ReaderComplianceInvalidTnefSignature);
        this.ReadStateValue = TnefReader.ReadState.EndOfFile;
        this.error = true;
      }
      else
      {
        this.attachmentKey = this.ReadWord();
        if (this.EnsureMoreDataLoaded(15) && (int) this.PeekByte(0) == 1 && (this.PeekDword(1) == 561158 && this.PeekDword(5) == 4))
        {
          this.tnefVersion = this.PeekDword(9);
          if (this.tnefVersion > 65536)
            this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefVersion, CtsResources.TnefStrings.ReaderComplianceInvalidTnefVersion);
        }
        if (this.messageCodepage == 0 && this.EnsureMoreDataLoaded(34) && ((int) this.PeekByte(15) == 1 && this.PeekDword(16) == 430087) && this.PeekDword(20) == 8)
        {
          int messageCodePage = this.PeekDword(24);
          if (!TnefCommon.IsUnicodeCodepage(messageCodePage))
          {
            this.messageCodepage = messageCodePage;
            this.string8Decoder = (Decoder) null;
          }
        }
        this.ReadStateValue = TnefReader.ReadState.Begin;
      }
    }

    private bool ReadAttributeHeader()
    {
      if (!this.EnsureMoreDataLoaded(9))
        return false;
      this.attributeLevel = (TnefAttributeLevel) this.ReadByte();
      this.attributeTag = (TnefAttributeTag) this.ReadDword();
      this.attributeValueLength = this.ReadDword();
      if (this.attributeValueLength < 0)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, CtsResources.TnefStrings.ReaderComplianceInvalidAttributeLength);
        this.error = true;
        return false;
      }
      if (this.attributeLevel != TnefAttributeLevel.Message && this.attributeLevel != TnefAttributeLevel.Attachment)
        this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLevel, CtsResources.TnefStrings.ReaderComplianceInvalidAttributeLevel);
      this.legacyAttribute = this.attributeTag != TnefAttributeTag.MapiProperties && this.attributeTag != TnefAttributeTag.Attachment && this.attributeTag != TnefAttributeTag.RecipientTable;
      this.attributeValueStreamOffset = this.StreamOffset;
      this.attributeValueOffset = 0;
      this.attributeStartChecksum = this.checksum;
      this.ReadStateValue = TnefReader.ReadState.BeginAttribute;
      return this.PreviewAttributeContent();
    }

    private void SkipRemainderOfCurrentAttribute()
    {
      if (this.attributeValueOffset > this.attributeValueLength)
        return;
      if (this.attributeValueOffset < this.attributeValueLength)
        this.EatAttributeBytes(this.AttributeRemainingCount());
      if (this.error)
        return;
      this.VerifyAttributeChecksum();
    }

    private void VerifyAttributeChecksum()
    {
      if (!this.EnsureMoreDataLoaded(2))
      {
        this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.error = true;
      }
      else
      {
        if (this.checksumDisabled || (int) (ushort) ((uint) this.checksum - (uint) this.attributeStartChecksum) == (int) (ushort) this.ReadWord() || (this.attributeTag == TnefAttributeTag.MessageClass || this.attributeTag == TnefAttributeTag.OriginalMessageClass))
          return;
        this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeChecksum, CtsResources.TnefStrings.ReaderComplianceInvalidAttributeChecksum);
      }
    }

    internal bool ReadNextRow()
    {
      this.AssertGoodToUse(true);
      this.AssertInAttribute();
      if (this.attributeTag != TnefAttributeTag.RecipientTable)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationReadNextRowOnlyInRecipientTable);
      if (this.ReadStateValue != TnefReader.ReadState.EndRow)
      {
        if (this.ReadStateValue == TnefReader.ReadState.EndAttribute)
          return false;
        if (this.ReadStateValue == TnefReader.ReadState.BeginAttribute)
        {
          this.ReadDword();
          this.rowIndex = -1;
        }
        else
        {
          if (this.ReadStateValue == TnefReader.ReadState.ReadAttributeValue)
            throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationPropAfterRaw);
          do
            ;
          while (this.ReadNextProperty());
          if (this.error)
          {
            this.ReadStateValue = TnefReader.ReadState.EndAttribute;
            return false;
          }
        }
      }
      if (++this.rowIndex == this.rowCount)
      {
        this.SkipRemainderOfCurrentAttribute();
        this.ReadStateValue = TnefReader.ReadState.EndAttribute;
        return false;
      }
      if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
      {
        this.ReadStateValue = TnefReader.ReadState.EndAttribute;
        return false;
      }
      this.propertyCount = this.PeekDword(0);
      this.propertyIndex = -1;
      this.ReadStateValue = TnefReader.ReadState.BeginRow;
      return true;
    }

    internal bool ReadNextProperty()
    {
      this.AssertGoodToUse(true);
      this.AssertInAttribute();
      if (this.ReadStateValue != TnefReader.ReadState.EndProperty)
      {
        if (this.ReadStateValue == TnefReader.ReadState.EndRow || this.ReadStateValue == TnefReader.ReadState.EndAttribute)
          return false;
        if (this.ReadStateValue == TnefReader.ReadState.BeginAttribute)
        {
          if (!this.PrepareFirstProperty())
          {
            this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
            this.error = true;
            return false;
          }
          this.propertyIndex = -1;
        }
        else if (this.ReadStateValue == TnefReader.ReadState.BeginRow)
        {
          this.ReadDword();
          this.propertyIndex = -1;
        }
        else
        {
          if (this.ReadStateValue == TnefReader.ReadState.ReadAttributeValue)
            throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationPropAfterRaw);
          if (!this.legacyAttribute)
          {
            if (this.propertyTag.IsMultiValued)
            {
              while (this.ReadNextPropertyValue())
                ;
            }
            else if (this.propertyTag.ValueTnefType == TnefPropertyType.Null)
            {
              this.ReadStateValue = TnefReader.ReadState.EndProperty;
            }
            else
            {
              if (this.ReadStateValue == TnefReader.ReadState.BeginProperty && !this.ReadNextPropertyValue())
              {
                this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
                return false;
              }
              if (this.propertyValueOffset != this.propertyValueLength)
                this.EatPropertyBytes(this.PropertyRemainingCount());
              if (this.propertyValuePaddingLength != 0)
              {
                this.EatAttributeBytes(this.propertyValuePaddingLength);
                this.propertyValuePaddingLength = 0;
              }
              if (this.propertyPaddingLength != 0)
              {
                this.EatAttributeBytes(this.propertyPaddingLength);
                this.propertyPaddingLength = 0;
              }
            }
            if (this.error)
            {
              this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
              return false;
            }
          }
        }
      }
      if (++this.propertyIndex == this.propertyCount)
      {
        if (this.attributeTag != TnefAttributeTag.RecipientTable)
        {
          this.SkipRemainderOfCurrentAttribute();
          this.ReadStateValue = TnefReader.ReadState.EndAttribute;
        }
        else
          this.ReadStateValue = TnefReader.ReadState.EndRow;
        return false;
      }
      if (this.legacyAttribute)
      {
        if (!this.PrepareLegacyProperty())
        {
          this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
          this.error = true;
          return false;
        }
        if (!this.CheckPropertyType())
        {
          this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
          return false;
        }
      }
      else
      {
        this.directRead = true;
        if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
        {
          this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
          return false;
        }
        this.propertyTag = (TnefPropertyTag) this.ReadDword();
        if (!this.CheckPropertyType())
        {
          this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
          return false;
        }
        if (this.propertyTag.IsNamed)
        {
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(24))
          {
            this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
            return false;
          }
          Guid propertySetGuid = this.ReadGuid();
          if (this.ReadDword() == 1)
          {
            int bytes = this.ReadDword();
            int count = (4 - bytes % 4) % 4;
            if (bytes <= 0 || bytes > 10240)
            {
              this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidNamedPropertyNameLength);
              this.error = true;
              this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
              return false;
            }
            if (!this.CheckAndEnsureMoreAttributeDataLoaded(bytes + count))
            {
              this.error = true;
              this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
              return false;
            }
            if ((int) this.PeekWord(bytes + count - 2) != 0)
            {
              this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidNamedPropertyNameNotZeroTerminated);
              this.error = true;
              this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
              return false;
            }
            string name = this.ReadAttributeUnicodeString(bytes);
            if (count != 0)
              this.SkipBytes(count);
            this.propertyNameId.Set(propertySetGuid, name);
          }
          else
            this.propertyNameId.Set(propertySetGuid, this.ReadDword());
        }
        this.propertyValueIndex = -1;
        if (this.propertyTag.IsMultiValued)
        {
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
          {
            this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
            return false;
          }
          this.propertyValueCount = this.ReadDword();
          if (this.propertyValueCount < 0)
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyValueCount);
            this.error = true;
            this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
            return false;
          }
        }
        else if (this.propertyTag.ValueTnefType == TnefPropertyType.Null)
        {
          this.propertyValueCount = 0;
        }
        else
        {
          this.propertyValueCount = 1;
          if (this.propertyTag.ValueTnefType == TnefPropertyType.Binary || this.propertyTag.ValueTnefType == TnefPropertyType.String8 || (this.propertyTag.ValueTnefType == TnefPropertyType.Unicode || this.propertyTag.ValueTnefType == TnefPropertyType.Object))
          {
            if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
            {
              this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
              return false;
            }
            if (this.ReadDword() != 1)
            {
              this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyValueCount);
              this.error = true;
              this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
              return false;
            }
          }
          else if (this.propertyValueFixedLength != 0 && !this.CheckAndEnsureMoreAttributeDataLoaded(Math.Max(this.propertyValueFixedLength, 4)))
          {
            this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
            return false;
          }
        }
        this.propertyPaddingLength = (4 - this.propertyValueCount * this.propertyValueFixedLength % 4) % 4;
        if (this.propertyValueFixedLength != 0 && this.propertyValueCount * this.propertyValueFixedLength + this.propertyPaddingLength > this.AttributeRemainingCount())
        {
          this.SetComplianceStatus(TnefComplianceStatus.AttributeOverflow, CtsResources.TnefStrings.ReaderComplianceAttributeValueOverflow);
          this.error = true;
          this.ReadStateValue = this.attributeTag != TnefAttributeTag.RecipientTable ? TnefReader.ReadState.EndAttribute : TnefReader.ReadState.EndRow;
          return false;
        }
      }
      this.ReadStateValue = TnefReader.ReadState.BeginProperty;
      return true;
    }

    internal bool ReadNextPropertyValue()
    {
      this.AssertGoodToUse(true);
      this.AssertInProperty();
      if (this.ReadStateValue != TnefReader.ReadState.EndPropertyValue)
      {
        if (this.ReadStateValue == TnefReader.ReadState.EndProperty)
          return false;
        if (this.ReadStateValue == TnefReader.ReadState.BeginProperty)
        {
          this.propertyValueIndex = -1;
        }
        else
        {
          if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue && !this.propertyTag.IsMultiValued)
            return true;
          if (!this.legacyAttribute)
          {
            if (this.propertyValueOffset != this.propertyValueLength)
              this.EatPropertyBytes(this.PropertyRemainingCount());
            if (this.propertyValuePaddingLength != 0)
            {
              this.EatAttributeBytes(this.propertyValuePaddingLength);
              this.propertyValuePaddingLength = 0;
            }
            if (this.error)
            {
              this.ReadStateValue = TnefReader.ReadState.EndProperty;
              return false;
            }
          }
        }
      }
      if (++this.propertyValueIndex == this.propertyValueCount)
      {
        this.ReadStateValue = TnefReader.ReadState.EndProperty;
        return false;
      }
      this.ReadStateValue = TnefReader.ReadState.BeginPropertyValue;
      this.propertyValueOffset = 0;
      this.propertyValueLength = 0;
      this.propertyValuePaddingLength = 0;
      this.propertyValueStreamOffset = this.StreamOffset;
      if (this.legacyAttribute)
      {
        if (!this.PrepareLegacyPropertyValue())
        {
          this.ReadStateValue = this.propertyTag.IsMultiValued ? TnefReader.ReadState.EndProperty : TnefReader.ReadState.BeginPropertyValue;
          return false;
        }
      }
      else
      {
        this.directRead = true;
        if (!this.CheckAndEnsureMoreAttributeDataLoaded(Math.Max(this.propertyValueFixedLength + this.propertyPaddingLength, 4)))
        {
          this.ReadStateValue = this.propertyTag.IsMultiValued ? TnefReader.ReadState.EndProperty : TnefReader.ReadState.BeginPropertyValue;
          return false;
        }
        this.propertyValueLength = this.GetPropertyValueLength();
        this.propertyValueStreamOffset = this.StreamOffset;
        if (this.error)
        {
          this.ReadStateValue = this.propertyTag.IsMultiValued ? TnefReader.ReadState.EndProperty : TnefReader.ReadState.BeginPropertyValue;
          return false;
        }
      }
      return true;
    }

    internal object ReadPropertyValue()
    {
      this.AssertGoodToUse(true);
      this.AssertInProperty();
      switch (this.propertyTag.ValueTnefType)
      {
        case TnefPropertyType.SysTime:
          return (object) this.ReadPropertyValueAsDateTime();
        case TnefPropertyType.ClassId:
          return (object) this.ReadPropertyValueAsGuid();
        case TnefPropertyType.Binary:
          return (object) this.ReadPropertyValueAsByteArray();
        case TnefPropertyType.I2:
          return (object) this.ReadPropertyValueAsShort();
        case TnefPropertyType.Long:
          return (object) this.ReadPropertyValueAsInt();
        case TnefPropertyType.R4:
          return (object) this.ReadPropertyValueAsFloat();
        case TnefPropertyType.Double:
          return (object) this.ReadPropertyValueAsDouble();
        case TnefPropertyType.Currency:
          return (object) this.ReadPropertyValueAsLong();
        case TnefPropertyType.AppTime:
          return (object) this.ReadPropertyValueAsDateTime();
        case TnefPropertyType.Error:
          return (object) this.ReadPropertyValueAsInt();
        case TnefPropertyType.Boolean:
                    return this.ReadPropertyValueAsBool();
                case TnefPropertyType.Object:
          return (object) this.ReadPropertyValueAsByteArray();
        case TnefPropertyType.I8:
          return (object) this.ReadPropertyValueAsLong();
        case TnefPropertyType.String8:
          return (object) this.ReadPropertyValueAsString();
        case TnefPropertyType.Unicode:
          return (object) this.ReadPropertyValueAsString();
        default:
          return (object) null;
      }
    }

    internal bool ReadPropertyValueAsBool()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.Boolean)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return false;
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      bool flag = 0 != (int) this.ReadPropertyWord();
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return flag;
    }

    internal short ReadPropertyValueAsShort()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.I2 && this.propertyTag.ValueTnefType != TnefPropertyType.Boolean)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return (short) 0;
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      short num = this.ReadPropertyWord();
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return num;
    }

    internal int ReadPropertyValueAsInt()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.I2 && this.propertyTag.ValueTnefType != TnefPropertyType.Boolean && (this.propertyTag.ValueTnefType != TnefPropertyType.Long && this.propertyTag.ValueTnefType != TnefPropertyType.Error))
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return 0;
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      int num;
      if (this.propertyTag.ValueTnefType == TnefPropertyType.I2 || this.propertyTag.ValueTnefType == TnefPropertyType.Boolean)
      {
        if (this.propertyTag.IsMultiValued || this.legacyAttribute)
        {
          num = (int) this.ReadPropertyWord();
        }
        else
        {
          this.propertyValueFixedLength = 4;
          this.propertyPaddingLength = 0;
          this.propertyValueLength = 4;
          num = this.ReadPropertyDword();
        }
      }
      else
        num = this.ReadPropertyDword();
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return num;
    }

    internal long ReadPropertyValueAsLong()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.I2 && this.propertyTag.ValueTnefType != TnefPropertyType.Boolean && (this.propertyTag.ValueTnefType != TnefPropertyType.Long && this.propertyTag.ValueTnefType != TnefPropertyType.Error) && (this.propertyTag.ValueTnefType != TnefPropertyType.Currency && this.propertyTag.ValueTnefType != TnefPropertyType.I8 && (this.propertyTag.ValueTnefType != TnefPropertyType.AppTime && this.propertyTag.ValueTnefType != TnefPropertyType.SysTime)))
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return 0L;
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      long num = this.propertyTag.ValueTnefType == TnefPropertyType.I2 || this.propertyTag.ValueTnefType == TnefPropertyType.Boolean ? (long) this.ReadPropertyWord() : (this.propertyTag.ValueTnefType == TnefPropertyType.Long || this.propertyTag.ValueTnefType == TnefPropertyType.Error ? (long) this.ReadPropertyDword() : this.ReadPropertyQword());
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return num;
    }

    internal Guid ReadPropertyValueAsGuid()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.ClassId)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return new Guid();
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      Guid guid = this.ReadPropertyGuid();
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return guid;
    }

    internal float ReadPropertyValueAsFloat()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.R4)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return 0.0f;
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      float num = this.ReadPropertyFloat();
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return num;
    }

    internal double ReadPropertyValueAsDouble()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.R4 && this.propertyTag.ValueTnefType != TnefPropertyType.Double)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return 0.0;
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      double num = this.propertyTag.ValueTnefType != TnefPropertyType.R4 ? this.ReadPropertyDouble() : (double) this.ReadPropertyFloat();
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return num;
    }

    internal DateTime ReadPropertyValueAsDateTime()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.AppTime && this.propertyTag.ValueTnefType != TnefPropertyType.SysTime)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.error || !this.EnsureMorePropertyDataLoaded(this.propertyValueFixedLength))
      {
        if (!this.error)
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
        this.error = true;
        return TnefReader.MinDateTime;
      }
      this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      DateTime dateTime = this.propertyTag.ValueTnefType != TnefPropertyType.AppTime ? this.ReadPropertySysTime() : this.ReadPropertyAppTime();
      if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return dateTime;
    }

    internal string ReadPropertyValueAsString()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.propertyTag.ValueTnefType != TnefPropertyType.String8 && this.propertyTag.ValueTnefType != TnefPropertyType.Unicode)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.IsLargePropertyValue)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationTextPropertyTooLong);
      if (this.decodeBuffer == null)
        this.decodeBuffer = new char[512];
      int num = this.ReadPropertyTextValue(this.decodeBuffer, 0, this.decodeBuffer.Length);
      if (this.ReadStateValue == TnefReader.ReadState.EndPropertyValue)
      {
        if (num == 0)
          return string.Empty;
        return new string(this.decodeBuffer, 0, num);
      }
      StringBuilder stringBuilder = new StringBuilder();
      do
      {
        stringBuilder.Append(this.decodeBuffer, 0, num);
        num = this.ReadPropertyTextValue(this.decodeBuffer, 0, this.decodeBuffer.Length);
      }
      while (num != 0);
      return stringBuilder.ToString();
    }

    internal byte[] ReadPropertyValueAsByteArray()
    {
      this.AssertAtTheBeginningOfPropertyValue();
      if (this.IsLargePropertyValue)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationPropertyRawValueTooLong);
      byte[] buffer = new byte[this.PropertyRawValueLength];
      this.ReadPropertyRawValue(buffer, 0, buffer.Length, false);
      return buffer;
    }

    internal int ReadPropertyTextValue(char[] buffer, int offset, int count)
    {
      this.AssertGoodToUse(true);
      if (this.ReadStateValue < TnefReader.ReadState.BeginPropertyValue)
      {
        if (this.ReadStateValue == TnefReader.ReadState.EndPropertyValue)
          return 0;
        if (this.ReadStateValue != TnefReader.ReadState.BeginProperty || this.propertyTag.IsMultiValued || this.propertyTag.ValueTnefType == TnefPropertyType.Null)
          throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeInProperty);
        this.ReadNextPropertyValue();
      }
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TnefStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountTooLarge);
      if (this.propertyTag.ValueTnefType != TnefPropertyType.String8 && this.propertyTag.ValueTnefType != TnefPropertyType.Unicode)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationCannotConvertValue);
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        if (this.propertyTag.ValueTnefType == TnefPropertyType.String8)
        {
          if (this.string8Decoder == null)
          {
            Encoding encoding = (Encoding) null;
            if (this.messageCodepage == 0)
            {
              encoding = Globalization.Charset.DefaultWindowsCharset.GetEncoding();
              this.messageCodepage = Globalization.CodePageMap.GetCodePage(encoding);
            }
            else if (!Globalization.Charset.TryGetEncoding(this.messageCodepage, out encoding))
            {
              this.SetComplianceStatus(TnefComplianceStatus.InvalidMessageCodepage, CtsResources.TnefStrings.ReaderComplianceInvalidMessageCodepage);
              encoding = Globalization.Charset.DefaultWindowsCharset.GetEncoding();
            }
            if (TnefCommon.IsUnicodeCodepage(Globalization.CodePageMap.GetCodePage(encoding)))
            {
              this.messageCodepage = 1252;
              encoding = Globalization.Charset.GetEncoding(this.messageCodepage);
            }
            this.string8Decoder = encoding.GetDecoder();
          }
          this.decoder = this.string8Decoder;
        }
        else
          this.decoder = this.unicodeDecoder;
        this.decoder.Reset();
        this.decoderFlushed = false;
        this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      }
      else if (this.decoder == null)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationPropTextAfterRaw);
      int num = 0;
      while ((this.PropertyRemainingCount() != 0 || !this.decoderFlushed) && count > 12)
      {
        if (this.MorePropertyData(1) && !this.EnsureMorePropertyDataLoaded(1))
        {
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
          this.error = true;
          break;
        }
        int byteCount = this.PropertyAvailableCount();
        int bytesUsed;
        int charsUsed;
        if (this.directRead)
          this.decoder.Convert(this.readBuffer, this.readOffset, byteCount, buffer, offset, count, byteCount == this.PropertyRemainingCount(), out bytesUsed, out charsUsed, out this.decoderFlushed);
        else
          this.decoder.Convert(this.fabricatedBuffer, this.fabricatedOffset, byteCount, buffer, offset, count, byteCount == this.PropertyRemainingCount(), out bytesUsed, out charsUsed, out this.decoderFlushed);
        this.SkipPropertyBytes(bytesUsed);
        offset += charsUsed;
        count -= charsUsed;
        num += charsUsed;
        for (int index = offset - charsUsed; index < offset; ++index)
        {
          if ((int) buffer[index] == 0)
          {
            num -= offset - index;
            this.EatPropertyBytes(this.PropertyRemainingCount());
            this.decoderFlushed = true;
            break;
          }
        }
      }
      if (this.error)
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
      else if (this.propertyValueOffset == this.propertyValueLength && this.decoderFlushed)
        this.ProcessEndOfValue();
      return num;
    }

    internal int ReadPropertyRawValue(byte[] buffer, int offset, int count, bool fromWrapper)
    {
      this.AssertGoodToUse(!fromWrapper);
      if (this.ReadStateValue < TnefReader.ReadState.BeginPropertyValue)
      {
        if (this.ReadStateValue == TnefReader.ReadState.EndPropertyValue)
          return 0;
        if (this.ReadStateValue != TnefReader.ReadState.BeginProperty || this.propertyTag.IsMultiValued || this.propertyTag.ValueTnefType == TnefPropertyType.Null)
          throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeInProperty);
        this.ReadNextPropertyValue();
      }
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException("offset", CtsResources.TnefStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountTooLarge);
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        this.decoder = (Decoder) null;
        this.ReadStateValue = TnefReader.ReadState.ReadPropertyValue;
      }
      else if (this.decoder != null)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationPropRawAfterText);
      int num = 0;
      while (this.MorePropertyData(1) && count != 0)
      {
        if (!this.EnsureMorePropertyDataLoaded(1))
        {
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
          this.error = true;
          break;
        }
        int count1 = Math.Min(count, this.PropertyAvailableCount());
        this.ReadPropertyBytes(buffer, offset, count1);
        offset += count1;
        count -= count1;
        num += count1;
      }
      if (this.error)
        this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
      else if (this.propertyValueOffset == this.propertyValueLength)
        this.ProcessEndOfValue();
      return num;
    }

    internal TnefReader GetEmbeddedMessageReader()
    {
      this.AssertGoodToUse(true);
      this.AssertAtTheBeginningOfPropertyValue();
      if (!this.IsPropertyEmbeddedMessage)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationNotEmbeddedMessage);
      if (this.embeddingDepth + 1 == 100)
        this.SetComplianceStatus(TnefComplianceStatus.NestingTooDeep, CtsResources.TnefStrings.ReaderComplianceTooDeepEmbedding);
      return new TnefReader(this);
    }

    internal Stream GetRawPropertyValueReadStream()
    {
      this.AssertGoodToUse(true);
      this.AssertAtTheBeginningOfPropertyValue();
      return (Stream) new TnefReaderStreamWrapper(this);
    }

    private void ProcessEndOfValue()
    {
      if (this.propertyValuePaddingLength != 0)
      {
        if (!this.EnsureMoreDataLoaded(this.propertyValuePaddingLength))
        {
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
          this.error = true;
          this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
          return;
        }
        this.SkipBytes(this.propertyValuePaddingLength);
        this.propertyValuePaddingLength = 0;
      }
      if (!this.propertyTag.IsMultiValued && this.propertyPaddingLength != 0)
      {
        if (!this.EnsureMoreDataLoaded(this.propertyPaddingLength))
        {
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
          this.error = true;
          this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
          return;
        }
        this.SkipBytes(this.propertyPaddingLength);
        this.propertyPaddingLength = 0;
      }
      this.ReadStateValue = TnefReader.ReadState.EndPropertyValue;
    }

    private bool CheckPropertyType()
    {
      switch (this.propertyTag.ValueTnefType)
      {
        case TnefPropertyType.SysTime:
        case TnefPropertyType.Double:
        case TnefPropertyType.Currency:
        case TnefPropertyType.AppTime:
        case TnefPropertyType.I8:
          this.propertyValueFixedLength = 8;
          break;
        case TnefPropertyType.ClassId:
          this.propertyValueFixedLength = 16;
          break;
        case TnefPropertyType.Binary:
        case TnefPropertyType.String8:
        case TnefPropertyType.Unicode:
          this.propertyValueFixedLength = 0;
          break;
        case TnefPropertyType.Null:
          this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyTypeError);
          if (!this.propertyTag.IsMultiValued)
          {
            this.propertyValueFixedLength = 0;
            break;
          }
          goto default;
        case TnefPropertyType.I2:
          this.propertyValueFixedLength = 2;
          break;
        case TnefPropertyType.Long:
        case TnefPropertyType.R4:
          this.propertyValueFixedLength = 4;
          break;
        case TnefPropertyType.Error:
          this.propertyValueFixedLength = 4;
          this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyTypeError);
          break;
        case TnefPropertyType.Boolean:
          this.propertyValueFixedLength = 2;
          if (this.propertyTag.IsMultiValued)
          {
            this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyTypeMvBoolean);
            break;
          }
          break;
        case TnefPropertyType.Object:
          this.propertyValueFixedLength = 0;
          if (this.propertyTag.IsMultiValued)
          {
            this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyTypeMvObject);
            goto default;
          }
          else
          {
            if (this.attributeTag == TnefAttributeTag.RecipientTable)
            {
              this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyTypeObjectInRecipientTable);
              break;
            }
            break;
          }
        default:
          this.propertyValueFixedLength = 0;
          this.SetComplianceStatus(TnefComplianceStatus.UnsupportedPropertyType, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyType);
          this.error = true;
          return false;
      }
      return true;
    }

    private int GetPropertyValueLength()
    {
      switch (this.propertyTag.ValueTnefType)
      {
        case TnefPropertyType.Object:
          int num1 = this.ReadDword();
          this.propertyValuePaddingLength = (4 - num1 % 4) % 4;
          if (num1 < 16 || (ulong) this.attributeValueOffset + (ulong) num1 + (ulong) this.propertyValuePaddingLength > (ulong) this.attributeValueLength)
          {
            if (num1 < 0 || (ulong) this.attributeValueOffset + (ulong) num1 + (ulong) this.propertyValuePaddingLength > (ulong) this.attributeValueLength)
            {
              this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyLength);
              this.error = true;
              return 0;
            }
            this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyLengthObject);
            return num1;
          }
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(16))
          {
            this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
            return num1;
          }
          this.propertyValueIId = this.ReadGuid();
          return num1 - 16;
        case TnefPropertyType.String8:
        case TnefPropertyType.Unicode:
        case TnefPropertyType.Binary:
          int num2 = this.ReadDword();
          this.propertyValuePaddingLength = (4 - num2 % 4) % 4;
          if (num2 >= 0 && (ulong) this.attributeValueOffset + (ulong) num2 + (ulong) this.propertyValuePaddingLength <= (ulong) this.attributeValueLength)
            return num2;
          this.SetComplianceStatus(TnefComplianceStatus.InvalidPropertyLength, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyLength);
          this.error = true;
          return 0;
        default:
          this.propertyValuePaddingLength = 0;
          return this.propertyValueFixedLength;
      }
    }

    private short ReadPropertyWord()
    {
      this.propertyValueOffset += 2;
      if (this.directRead)
        return this.ReadWord();
      short num = BitConverter.ToInt16(this.fabricatedBuffer, this.fabricatedOffset);
      this.fabricatedOffset += 2;
      return num;
    }

    private int ReadPropertyDword()
    {
      this.propertyValueOffset += 4;
      if (this.directRead)
        return this.ReadDword();
      int num = BitConverter.ToInt32(this.fabricatedBuffer, this.fabricatedOffset);
      this.fabricatedOffset += 4;
      return num;
    }

    private long ReadPropertyQword()
    {
      this.propertyValueOffset += 8;
      if (this.directRead)
        return this.ReadQword();
      long num = BitConverter.ToInt64(this.fabricatedBuffer, this.fabricatedOffset);
      this.fabricatedOffset += 8;
      return num;
    }

    private Guid ReadPropertyGuid()
    {
      this.propertyValueOffset += 16;
      if (this.directRead)
        return this.ReadGuid();
      Guid guid = new Guid(BitConverter.ToInt32(this.fabricatedBuffer, this.fabricatedOffset), BitConverter.ToInt16(this.fabricatedBuffer, this.fabricatedOffset + 4), BitConverter.ToInt16(this.fabricatedBuffer, this.fabricatedOffset + 6), this.fabricatedBuffer[this.fabricatedOffset + 8], this.fabricatedBuffer[this.fabricatedOffset + 9], this.fabricatedBuffer[this.fabricatedOffset + 10], this.fabricatedBuffer[this.fabricatedOffset + 11], this.fabricatedBuffer[this.fabricatedOffset + 12], this.fabricatedBuffer[this.fabricatedOffset + 13], this.fabricatedBuffer[this.fabricatedOffset + 14], this.fabricatedBuffer[this.fabricatedOffset + 15]);
      this.fabricatedOffset += 16;
      return guid;
    }

    private float ReadPropertyFloat()
    {
      this.propertyValueOffset += 4;
      if (this.directRead)
        return this.ReadFloat();
      float num = BitConverter.ToSingle(this.fabricatedBuffer, this.fabricatedOffset);
      this.fabricatedOffset += 4;
      return num;
    }

    private double ReadPropertyDouble()
    {
      this.propertyValueOffset += 8;
      if (this.directRead)
        return this.ReadDouble();
      double num = BitConverter.ToDouble(this.fabricatedBuffer, this.fabricatedOffset);
      this.fabricatedOffset += 8;
      return num;
    }

    private DateTime ReadPropertyAppTime()
    {
      double num = this.ReadPropertyDouble();
      DateTime dateTime;
      try
      {
        dateTime = num == 0.0 ? TnefReader.MinDateTime : DateTime.SpecifyKind(TnefReader.FromOADate(num), DateTimeKind.Utc);
      }
      catch (ArgumentException ex)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidDate, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyValueDate);
        dateTime = DateTime.UtcNow;
      }
      return dateTime;
    }

    private DateTime ReadPropertySysTime()
    {
      long fileTime = this.ReadPropertyQword();
      DateTime dateTime;
      try
      {
        dateTime = fileTime == 0L ? TnefReader.MinDateTime : DateTime.FromFileTimeUtc(fileTime);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidDate, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyValueDate);
        dateTime = DateTime.UtcNow;
      }
      return dateTime;
    }

    private void ReadPropertyBytes(byte[] buffer, int offset, int count)
    {
      this.propertyValueOffset += count;
      if (!this.directRead)
      {
        Buffer.BlockCopy((Array) this.fabricatedBuffer, this.fabricatedOffset, (Array) buffer, offset, count);
        this.fabricatedOffset += count;
      }
      else
        this.ReadBytes(buffer, offset, count);
    }

    private void SkipPropertyBytes(int count)
    {
      this.propertyValueOffset += count;
      if (!this.directRead)
        this.fabricatedOffset += count;
      else
        this.SkipBytes(count);
    }

    private void EatPropertyBytes(int count)
    {
      while (this.MorePropertyData(1) && count != 0)
      {
        if (!this.EnsureMorePropertyDataLoaded(1))
        {
          this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
          this.error = true;
          break;
        }
        int count1 = Math.Min(count, this.PropertyAvailableCount());
        this.SkipPropertyBytes(count1);
        count -= count1;
      }
    }

    private int PropertyRemainingCount()
    {
      return this.propertyValueLength - this.propertyValueOffset;
    }

    private int PropertyAvailableCount()
    {
      if (!this.directRead)
        return this.fabricatedEnd - this.fabricatedOffset;
      return Math.Min(this.AvailableCount(), this.PropertyRemainingCount());
    }

    private bool MorePropertyData(int count)
    {
      return this.propertyValueOffset + count <= this.propertyValueLength;
    }

    private bool EnsureMorePropertyDataLoaded(int count)
    {
      if (!this.directRead)
        return this.EnsureMoreFabricatedDataAvailable(count);
      return this.EnsureMoreDataLoaded(count);
    }

    private bool EnsureMoreFabricatedDataAvailable(int count)
    {
      if (this.fabricatedOffset + count > this.fabricatedEnd)
        return this.FabricateMorePropertyData(count);
      return true;
    }

    private bool PreviewAttributeContent()
    {
      this.rowCount = -1;
      this.propertyCount = 0;
      switch (this.attributeTag)
      {
        case TnefAttributeTag.OriginalMessageClass:
        case TnefAttributeTag.MessageClass:
          if (this.attributeValueLength == 0 || this.attributeValueLength > (int) byte.MaxValue)
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidMessageClass, CtsResources.TnefStrings.ReaderComplianceInvalidMessageClassLength);
            this.directReadForMessageClass = true;
            break;
          }
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(this.attributeValueLength))
            return false;
          if ((int) this.PeekByte(this.attributeValueLength - 1) != 0)
            this.SetComplianceStatus(TnefComplianceStatus.InvalidMessageClass, CtsResources.TnefStrings.ReaderComplianceInvalidMessageClassNotZeroTerminated);
          this.propertyCount = 1;
          this.FabricateMessageClass(this.attributeTag == TnefAttributeTag.MessageClass);
          break;
        case TnefAttributeTag.TnefVersion:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, CtsResources.TnefStrings.ReaderComplianceInvalidTnefVersionAttributeLength);
            break;
          }
          this.tnefVersion = this.PeekDword(0);
          if (this.tnefVersion > 65536)
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidTnefVersion, CtsResources.TnefStrings.ReaderComplianceInvalidTnefVersion);
            break;
          }
          break;
        case TnefAttributeTag.AttachData:
          this.propertyCount = 1;
          if (this.currentAttachmentIsOle)
          {
            ++this.propertyCount;
            break;
          }
          break;
        case TnefAttributeTag.AttachMetaFile:
        case TnefAttributeTag.AttachTransportFilename:
        case TnefAttributeTag.AttachCreateDate:
        case TnefAttributeTag.AttachModifyDate:
        case TnefAttributeTag.AttachTitle:
          this.propertyCount = 1;
          break;
        case TnefAttributeTag.AttachRenderData:
          if (this.CheckAndEnsureMoreAttributeDataLoaded(14))
          {
            short num1 = this.PeekWord(0);
            int num2 = this.PeekDword(10);
            this.propertyCount = 2;
            if (num2 == 1)
              ++this.propertyCount;
            this.currentAttachmentIsOle = (int) num1 != 1;
            break;
          }
          break;
        case TnefAttributeTag.MapiProperties:
        case TnefAttributeTag.Attachment:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
            return false;
          this.propertyCount = this.PeekDword(0);
          if (this.propertyCount < 0)
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidRowCount, CtsResources.TnefStrings.ReaderComplianceInvalidPropertyCount);
            this.error = true;
            return false;
          }
          break;
        case TnefAttributeTag.RecipientTable:
          this.propertyCount = -1;
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
            return false;
          this.rowCount = this.PeekDword(0);
          if (this.rowCount < 0)
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidRowCount, CtsResources.TnefStrings.ReaderComplianceInvalidRowCount);
            this.error = true;
            this.rowCount = 0;
            return false;
          }
          break;
        case TnefAttributeTag.OemCodepage:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(8))
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, CtsResources.TnefStrings.ReaderComplianceInvalidOemCodepageAttributeLength);
            break;
          }
          int messageCodePage = this.PeekDword(0);
          if (this.messageCodepage == 0 && !TnefCommon.IsUnicodeCodepage(messageCodePage))
          {
            this.messageCodepage = messageCodePage;
            this.string8Decoder = (Decoder) null;
            break;
          }
          break;
        case TnefAttributeTag.AidOwner:
        case TnefAttributeTag.Delegate:
        case TnefAttributeTag.RequestResponse:
        case TnefAttributeTag.DateStart:
        case TnefAttributeTag.DateEnd:
          this.propertyCount = 1;
          break;
        case TnefAttributeTag.Owner:
        case TnefAttributeTag.SentFor:
          this.propertyCount = !this.messageClassSPlus ? 0 : 2;
          break;
        case TnefAttributeTag.MessageStatus:
        case TnefAttributeTag.Priority:
        case TnefAttributeTag.Body:
        case TnefAttributeTag.MessageId:
        case TnefAttributeTag.ParentId:
        case TnefAttributeTag.ConversationId:
          this.propertyCount = 1;
          break;
        case TnefAttributeTag.DateSent:
        case TnefAttributeTag.DateReceived:
        case TnefAttributeTag.DateModified:
        case TnefAttributeTag.Subject:
          this.propertyCount = 1;
          break;
        case TnefAttributeTag.From:
          this.propertyCount = 2;
          break;
      }
      return true;
    }

    private bool PrepareFirstProperty()
    {
      switch (this.attributeTag)
      {
        case TnefAttributeTag.MapiProperties:
        case TnefAttributeTag.Attachment:
          this.ReadDword();
          break;
        case TnefAttributeTag.RecipientTable:
          throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeInARow);
        case TnefAttributeTag.AidOwner:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(4))
            return false;
          break;
        case TnefAttributeTag.Owner:
        case TnefAttributeTag.SentFor:
          if (this.messageClassSPlus)
          {
            if (this.attributeValueLength <= 4 || this.attributeValueLength > 32768)
            {
              this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidSchedulePlus);
              return false;
            }
            if (!this.CheckAndEnsureMoreAttributeDataLoaded(this.attributeValueLength) || !this.CrackSchedulePlusId())
              return false;
            break;
          }
          break;
        case TnefAttributeTag.MessageStatus:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(1))
            return false;
          break;
        case TnefAttributeTag.RequestResponse:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(2))
            return false;
          break;
        case TnefAttributeTag.Priority:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(2))
            return false;
          break;
        case TnefAttributeTag.DateSent:
        case TnefAttributeTag.DateReceived:
        case TnefAttributeTag.DateModified:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(14))
          {
            this.error = true;
            return false;
          }
          break;
        case TnefAttributeTag.AttachCreateDate:
        case TnefAttributeTag.AttachModifyDate:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(14))
            return false;
          break;
        case TnefAttributeTag.DateStart:
        case TnefAttributeTag.DateEnd:
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(14))
            return false;
          break;
        case TnefAttributeTag.MessageId:
        case TnefAttributeTag.ParentId:
        case TnefAttributeTag.ConversationId:
          if (this.attributeValueLength == 0)
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidAttribute, CtsResources.TnefStrings.ReaderComplianceInvalidConversationId);
            break;
          }
          break;
        case TnefAttributeTag.From:
          if (this.attributeValueLength <= 8 || this.attributeValueLength > 32768)
          {
            this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidFrom);
            return false;
          }
          if (!this.CheckAndEnsureMoreAttributeDataLoaded(this.attributeValueLength) || !this.CrackTriple())
            return false;
          break;
      }
      return true;
    }

    private bool PrepareLegacyProperty()
    {
      this.propertyPaddingLength = 0;
      this.propertyValueCount = 1;
      this.directRead = false;
      switch (this.attributeTag)
      {
        case TnefAttributeTag.AttachTransportFilename:
          this.propertyTag = TnefPropertyTag.AttachTransportNameA;
          this.directRead = true;
          break;
        case TnefAttributeTag.AttachRenderData:
          this.propertyTag = this.propertyIndex != 0 ? (this.propertyIndex != 1 ? TnefPropertyTag.AttachEncoding : TnefPropertyTag.AttachMethod) : TnefPropertyTag.RenderingPosition;
          break;
        case TnefAttributeTag.OriginalMessageClass:
          this.propertyTag = TnefPropertyTag.OrigMessageClassA;
          this.directRead = this.directReadForMessageClass;
          break;
        case TnefAttributeTag.MessageClass:
          this.propertyTag = TnefPropertyTag.MessageClassA;
          this.directRead = this.directReadForMessageClass;
          break;
        case TnefAttributeTag.MessageStatus:
          this.propertyTag = TnefPropertyTag.MessageFlags;
          break;
        case TnefAttributeTag.AttachData:
          if (this.propertyIndex == 0)
          {
            this.propertyTag = TnefPropertyTag.AttachDataBin;
            this.directRead = true;
            break;
          }
          this.propertyTag = TnefPropertyTag.AttachTag;
          break;
        case TnefAttributeTag.AttachMetaFile:
          this.propertyTag = TnefPropertyTag.AttachRendering;
          this.directRead = true;
          break;
        case TnefAttributeTag.AidOwner:
          this.propertyTag = TnefPropertyTag.OwnerApptId;
          this.directRead = true;
          break;
        case TnefAttributeTag.Owner:
          this.propertyTag = this.propertyIndex != 0 ? (this.messageClassSPlusResponse ? TnefPropertyTag.RcvdRepresentingNameA : TnefPropertyTag.SentRepresentingNameA) : (this.messageClassSPlusResponse ? TnefPropertyTag.RcvdRepresentingEntryId : TnefPropertyTag.SentRepresentingEntryId);
          break;
        case TnefAttributeTag.SentFor:
          this.propertyTag = this.propertyIndex != 0 ? TnefPropertyTag.SentRepresentingNameA : TnefPropertyTag.SentRepresentingEntryId;
          break;
        case TnefAttributeTag.Delegate:
          this.propertyTag = TnefPropertyTag.Delegation;
          this.directRead = true;
          break;
        case TnefAttributeTag.RequestResponse:
          this.propertyTag = TnefPropertyTag.ResponseRequested;
          this.directRead = true;
          break;
        case TnefAttributeTag.Priority:
          this.propertyTag = TnefPropertyTag.Importance;
          break;
        case TnefAttributeTag.DateSent:
          this.propertyTag = TnefPropertyTag.ClientSubmitTime;
          break;
        case TnefAttributeTag.DateReceived:
          this.propertyTag = TnefPropertyTag.MessageDeliveryTime;
          break;
        case TnefAttributeTag.AttachCreateDate:
          this.propertyTag = TnefPropertyTag.CreationTime;
          break;
        case TnefAttributeTag.AttachModifyDate:
          this.propertyTag = TnefPropertyTag.LastModificationTime;
          break;
        case TnefAttributeTag.DateModified:
          this.propertyTag = TnefPropertyTag.LastModificationTime;
          break;
        case TnefAttributeTag.Body:
          this.propertyTag = TnefPropertyTag.BodyA;
          this.directRead = true;
          break;
        case TnefAttributeTag.DateStart:
          this.propertyTag = TnefPropertyTag.StartDate;
          break;
        case TnefAttributeTag.DateEnd:
          this.propertyTag = TnefPropertyTag.EndDate;
          break;
        case TnefAttributeTag.MessageId:
          this.propertyTag = TnefPropertyTag.SearchKey;
          break;
        case TnefAttributeTag.ParentId:
          this.propertyTag = TnefPropertyTag.ParentKey;
          break;
        case TnefAttributeTag.ConversationId:
          this.propertyTag = TnefPropertyTag.ConversationKey;
          break;
        case TnefAttributeTag.AttachTitle:
          this.propertyTag = TnefPropertyTag.AttachFilenameA;
          this.directRead = true;
          break;
        case TnefAttributeTag.From:
          this.propertyTag = this.propertyIndex != 0 ? TnefPropertyTag.SenderNameA : TnefPropertyTag.SenderEntryId;
          break;
        case TnefAttributeTag.Subject:
          this.propertyTag = TnefPropertyTag.SubjectA;
          this.directRead = true;
          break;
        default:
          return false;
      }
      return true;
    }

    private bool PrepareLegacyPropertyValue()
    {
      this.propertyValueOffset = 0;
      this.propertyValueLength = this.propertyValueFixedLength;
      this.propertyValueStreamOffset = this.attributeValueStreamOffset;
      this.propertyValuePaddingLength = 0;
      switch (this.attributeTag)
      {
        case TnefAttributeTag.AttachTransportFilename:
          this.propertyValueLength = this.attributeValueLength;
          break;
        case TnefAttributeTag.AttachRenderData:
          if (this.propertyIndex == 0)
          {
            this.FabricateRenderingPositionFromRendData();
            break;
          }
          if (this.propertyIndex == 1)
          {
            this.FabricateAttachMethodFromRendData();
            break;
          }
          this.FabricateAttachEncodingFromRendData();
          break;
        case TnefAttributeTag.OriginalMessageClass:
        case TnefAttributeTag.MessageClass:
          this.propertyValueLength = this.directRead ? this.AttributeRemainingCount() : this.fabricatedEnd - this.fabricatedOffset;
          break;
        case TnefAttributeTag.MessageStatus:
          this.FabricateMessageFlagsFromMessageStatus();
          break;
        case TnefAttributeTag.AttachData:
          if (this.propertyIndex == 0)
          {
            this.propertyValueLength = this.attributeValueLength;
            break;
          }
          this.FabricateAttachTagOle1Storage();
          break;
        case TnefAttributeTag.AttachMetaFile:
          this.propertyValueLength = this.attributeValueLength;
          break;
        case TnefAttributeTag.AidOwner:
          this.propertyValueLength = 4;
          break;
        case TnefAttributeTag.Owner:
          if (this.propertyIndex == 0)
          {
            this.FabricateEntryIdFromTriple();
            break;
          }
          this.FabricateNameFromTriple();
          break;
        case TnefAttributeTag.SentFor:
          if (this.propertyIndex == 0)
          {
            this.FabricateEntryIdFromTriple();
            break;
          }
          this.FabricateNameFromTriple();
          break;
        case TnefAttributeTag.Delegate:
          this.propertyValueLength = this.attributeValueLength;
          break;
        case TnefAttributeTag.RequestResponse:
          this.propertyValueLength = 2;
          break;
        case TnefAttributeTag.Priority:
          this.FabricateImportanceFromPriority();
          break;
        case TnefAttributeTag.DateSent:
          if (!this.FabricateSysTimeFromDTR())
            return false;
          break;
        case TnefAttributeTag.DateReceived:
          if (!this.FabricateSysTimeFromDTR())
            return false;
          break;
        case TnefAttributeTag.AttachCreateDate:
          if (!this.FabricateSysTimeFromDTR())
            return false;
          break;
        case TnefAttributeTag.AttachModifyDate:
          if (!this.FabricateSysTimeFromDTR())
            return false;
          break;
        case TnefAttributeTag.DateModified:
          if (!this.FabricateSysTimeFromDTR())
            return false;
          break;
        case TnefAttributeTag.Body:
          this.propertyValueLength = this.attributeValueLength;
          break;
        case TnefAttributeTag.DateStart:
          if (!this.FabricateSysTimeFromDTR())
            return false;
          break;
        case TnefAttributeTag.DateEnd:
          if (!this.FabricateSysTimeFromDTR())
            return false;
          break;
        case TnefAttributeTag.MessageId:
          if (!this.FabricateTextizedBinary())
            return false;
          break;
        case TnefAttributeTag.ParentId:
          if (!this.FabricateTextizedBinary())
            return false;
          break;
        case TnefAttributeTag.ConversationId:
          if (!this.FabricateTextizedBinary())
            return false;
          break;
        case TnefAttributeTag.AttachTitle:
          this.propertyValueLength = this.attributeValueLength;
          break;
        case TnefAttributeTag.From:
          if (this.propertyIndex == 0)
          {
            this.FabricateEntryIdFromTriple();
            break;
          }
          this.FabricateNameFromTriple();
          break;
        case TnefAttributeTag.Subject:
          this.propertyValueLength = this.attributeValueLength;
          break;
        default:
          return false;
      }
      return true;
    }

    private bool FabricateMorePropertyData(int count)
    {
      switch (this.attributeTag)
      {
        case TnefAttributeTag.AttachRenderData:
        case TnefAttributeTag.OriginalMessageClass:
        case TnefAttributeTag.MessageClass:
        case TnefAttributeTag.MessageStatus:
        case TnefAttributeTag.AttachData:
        case TnefAttributeTag.Priority:
        case TnefAttributeTag.DateSent:
        case TnefAttributeTag.DateReceived:
        case TnefAttributeTag.AttachCreateDate:
        case TnefAttributeTag.AttachModifyDate:
        case TnefAttributeTag.DateModified:
        case TnefAttributeTag.DateStart:
        case TnefAttributeTag.DateEnd:
          this.fabricatedEnd = this.fabricatedOffset = 0;
          return false;
        case TnefAttributeTag.Owner:
        case TnefAttributeTag.SentFor:
        case TnefAttributeTag.From:
          if (this.propertyIndex == 0)
          {
            this.FabricateEntryIdFromTriple();
            break;
          }
          this.FabricateNameFromTriple();
          break;
        case TnefAttributeTag.MessageId:
        case TnefAttributeTag.ParentId:
        case TnefAttributeTag.ConversationId:
          if (!this.FabricateTextizedBinary())
            return false;
          break;
        default:
          return false;
      }
      if (this.fabricatedEnd - this.fabricatedOffset >= count)
        return true;
      this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeLength, CtsResources.TnefStrings.ReaderComplianceInvalidComputedPropertyLength);
      this.error = true;
      return false;
    }

    private bool FabricateSysTimeFromDTR()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        try
        {
          TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, new DateTime((int) this.PeekWord(0), (int) this.PeekWord(2), (int) this.PeekWord(4), (int) this.PeekWord(6), (int) this.PeekWord(8), (int) this.PeekWord(10), DateTimeKind.Utc).ToFileTimeUtc());
        }
        catch (ArgumentException ex)
        {
          this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidDateOrTimeValue);
          TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, DateTime.UtcNow.ToFileTimeUtc());
        }
        this.fabricatedEnd = 8;
        this.propertyValueLength = this.fabricatedEnd;
      }
      else
        this.fabricatedEnd = 0;
      this.fabricatedOffset = 0;
      return true;
    }

    private void FabricateEntryIdFromTriple()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        this.fabricatedOffset = 0;
        this.fabricatedEnd = 0;
        this.propertyValueLength = 24 + this.tripleNameLength + 1 + this.tripleAddressTypeLength + 1 + this.tripleAddressLength + 1;
        TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, 0);
        Buffer.BlockCopy((Array) TnefCommon.MuidOOP, 0, (Array) this.fabricatedBuffer, 4, 16);
        TnefBitConverter.GetBytes(this.fabricatedBuffer, 20, 0);
        this.fabricatedEnd = 24;
      }
      else
      {
        this.fabricatedOffset = 0;
        this.fabricatedEnd = 0;
      }
      int num1 = this.propertyValueOffset + (this.fabricatedEnd - this.fabricatedOffset);
      int val1 = this.fabricatedBuffer.Length - this.fabricatedEnd;
      if (num1 < 24 + this.tripleNameLength && val1 > 0)
      {
        int count = Math.Min(val1, 24 + this.tripleNameLength - num1);
        Buffer.BlockCopy((Array) this.readBuffer, this.readOffset + this.tripleNameOffset + (num1 - 24), (Array) this.fabricatedBuffer, this.fabricatedEnd, count);
        num1 += count;
        val1 -= count;
        this.fabricatedEnd += count;
      }
      if (num1 == 24 + this.tripleNameLength && val1 > 0)
      {
        this.fabricatedBuffer[this.fabricatedEnd++] = (byte) 0;
        ++num1;
        --val1;
      }
      int num2 = 24 + this.tripleNameLength + 1;
      if (num1 < num2 + this.tripleAddressTypeLength && val1 > 0)
      {
        int num3 = Math.Min(val1, num2 + this.tripleAddressTypeLength - num1);
        int num4 = num1 - num2;
        for (int index = 0; index < num3; ++index)
        {
          this.fabricatedBuffer[this.fabricatedEnd + index] = (byte) char.ToUpperInvariant((char) this.readBuffer[this.readOffset + this.tripleAddressTypeOffset + num4]);
          ++num4;
        }
        num1 += num3;
        val1 -= num3;
        this.fabricatedEnd += num3;
      }
      if (num1 == num2 + this.tripleAddressTypeLength && val1 > 0)
      {
        this.fabricatedBuffer[this.fabricatedEnd++] = (byte) 0;
        ++num1;
        --val1;
      }
      int num5 = num2 + this.tripleAddressTypeLength + 1;
      if (num1 < num5 + this.tripleAddressLength && val1 > 0)
      {
        int count = Math.Min(val1, num5 + this.tripleAddressLength - num1);
        Buffer.BlockCopy((Array) this.readBuffer, this.readOffset + this.tripleAddressOffset + (num1 - num5), (Array) this.fabricatedBuffer, this.fabricatedEnd, count);
        num1 += count;
        val1 -= count;
        this.fabricatedEnd += count;
      }
      if (num1 != num5 + this.tripleAddressLength || val1 <= 0)
        return;
      this.fabricatedBuffer[this.fabricatedEnd++] = (byte) 0;
      int num6 = num1 + 1;
      int num7 = val1 - 1;
    }

    private void FabricateNameFromTriple()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        this.fabricatedOffset = 0;
        this.fabricatedEnd = 0;
        this.propertyValueLength = this.tripleNameLength + 1;
      }
      else
      {
        this.fabricatedOffset = 0;
        this.fabricatedEnd = 0;
      }
      int count = Math.Min(this.PropertyRemainingCount() - (this.fabricatedEnd - this.fabricatedOffset), this.fabricatedBuffer.Length - this.fabricatedEnd);
      Buffer.BlockCopy((Array) this.readBuffer, this.readOffset + this.tripleNameOffset + this.propertyValueOffset, (Array) this.fabricatedBuffer, this.fabricatedEnd, count);
      this.fabricatedEnd += count;
    }

    private void FabricateAttachTagOle1Storage()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        Buffer.BlockCopy((Array) TnefCommon.OidOle1Storage, 0, (Array) this.fabricatedBuffer, 0, TnefCommon.OidOle1Storage.Length);
        this.fabricatedEnd = TnefCommon.OidOle1Storage.Length;
        this.propertyValueLength = this.fabricatedEnd;
      }
      else
        this.fabricatedEnd = 0;
      this.fabricatedOffset = 0;
    }

    private void FabricateRenderingPositionFromRendData()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, this.PeekDword(2));
        this.fabricatedEnd = 4;
        this.propertyValueLength = this.fabricatedEnd;
      }
      else
        this.fabricatedEnd = 0;
      this.fabricatedOffset = 0;
    }

    private void FabricateAttachMethodFromRendData()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, (int) this.PeekWord(0) == 1 ? 1 : 6);
        this.fabricatedEnd = 4;
        this.propertyValueLength = this.fabricatedEnd;
      }
      else
        this.fabricatedEnd = 0;
      this.fabricatedOffset = 0;
    }

    private void FabricateAttachEncodingFromRendData()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        Buffer.BlockCopy((Array) TnefCommon.OidMacBinary, 0, (Array) this.fabricatedBuffer, 0, TnefCommon.OidMacBinary.Length);
        this.fabricatedEnd = TnefCommon.OidMacBinary.Length;
        this.propertyValueLength = this.fabricatedEnd;
      }
      else
        this.fabricatedEnd = 0;
      this.fabricatedOffset = 0;
    }

    private bool FabricateTextizedBinary()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        this.fabricatedOffset = 0;
        this.fabricatedEnd = 0;
        this.propertyValueLength = (this.attributeValueLength & -2) / 2;
      }
      else
      {
        this.fabricatedOffset = 0;
        this.fabricatedEnd = 0;
      }
      while (this.attributeValueOffset != this.propertyValueLength * 2 && this.fabricatedEnd < this.fabricatedBuffer.Length)
      {
        if (!this.CheckAndEnsureMoreAttributeDataLoaded(2))
        {
          this.error = true;
          return false;
        }
        int count = Math.Min(2 * Math.Min(this.PropertyRemainingCount() - (this.fabricatedEnd - this.fabricatedOffset), this.fabricatedBuffer.Length - this.fabricatedEnd), this.AvailableCount()) & -2;
        int offsetFromCurrentPosition = 0;
        while (offsetFromCurrentPosition < count)
        {
          this.fabricatedBuffer[this.fabricatedEnd++] = (byte) (((uint) TnefReader.NumFromHex[(int) this.PeekByte(offsetFromCurrentPosition)] << 4) + (uint) TnefReader.NumFromHex[(int) this.PeekByte(offsetFromCurrentPosition + 1)]);
          offsetFromCurrentPosition += 2;
        }
        this.SkipBytes(count);
      }
      return true;
    }

    private void FabricateImportanceFromPriority()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        int num;
        switch (this.PeekWord(0))
        {
          case (short) 1:
            num = 2;
            break;
          case (short) 3:
            num = 0;
            break;
          default:
            num = 1;
            break;
        }
        TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, num);
        this.fabricatedEnd = 4;
        this.propertyValueLength = this.fabricatedEnd;
      }
      else
        this.fabricatedEnd = 0;
      this.fabricatedOffset = 0;
    }

    private void FabricateMessageFlagsFromMessageStatus()
    {
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
      {
        short num = (short) this.PeekByte(0);
        TnefBitConverter.GetBytes(this.fabricatedBuffer, 0, 0 | (((int) num & 32) != 0 ? 1 : 0) | (((int) num & 1) != 0 ? 0 : 2) | (((int) num & 4) != 0 ? 4 : 0) | (((int) num & 128) != 0 ? 16 : 0) | (((int) num & 2) != 0 ? 8 : 0));
        this.fabricatedEnd = 4;
        this.propertyValueLength = this.fabricatedEnd;
      }
      else
        this.fabricatedEnd = 0;
      this.fabricatedOffset = 0;
    }

    private void FabricateMessageClass(bool rememberMessageClass)
    {
      if (TnefCommon.BytesEqualToPattern(this.readBuffer, this.readOffset, TnefCommon.MessageClassLegacyPrefix))
        this.SkipBytes(TnefCommon.MessageClassLegacyPrefix.Length);
      for (int index = 0; index < TnefCommon.MessageClassMappingTable.Length; ++index)
      {
        if (this.AttributeRemainingCount() == TnefCommon.MessageClassMappingTable[index].LegacyName.Length + 1 && TnefCommon.BytesEqualToPattern(this.readBuffer, this.readOffset, TnefCommon.MessageClassMappingTable[index].LegacyName) && (int) this.readBuffer[this.readOffset + this.AttributeRemainingCount() - 1] == 0)
        {
          if (rememberMessageClass)
          {
            this.messageClassSPlus = TnefCommon.MessageClassMappingTable[index].Splus;
            this.messageClassSPlusResponse = TnefCommon.MessageClassMappingTable[index].SplusResponse;
          }
          this.fabricatedOffset = 0;
          this.fabricatedEnd = CTSGlobals.AsciiEncoding.GetBytes(TnefCommon.MessageClassMappingTable[index].MapiName, 0, TnefCommon.MessageClassMappingTable[index].MapiName.Length, this.fabricatedBuffer, 0);
          this.fabricatedBuffer[this.fabricatedEnd++] = (byte) 0;
          this.directReadForMessageClass = false;
          this.propertyValueLength = this.fabricatedEnd;
          return;
        }
      }
      this.directReadForMessageClass = true;
      this.propertyValueLength = this.AttributeRemainingCount();
      this.fabricatedOffset = 0;
    }

    private string ReadAttributeUnicodeString(int bytes)
    {
      int num = 0;
      while (num < bytes - 1 && (int) this.PeekWord(num) != 0)
        num += 2;
      if (this.decodeBuffer == null)
        this.decodeBuffer = new char[512];
      int bytesUsed;
      int charsUsed;
      bool completed;
      this.unicodeDecoder.Convert(this.readBuffer, this.readOffset, num, this.decodeBuffer, 0, this.decodeBuffer.Length, true, out bytesUsed, out charsUsed, out completed);
      if (completed)
      {
        this.SkipBytes(bytes);
        return new string(this.decodeBuffer, 0, charsUsed);
      }
      StringBuilder stringBuilder = new StringBuilder(num / 2 + 1);
      stringBuilder.Append(this.decodeBuffer, 0, charsUsed);
      this.SkipBytes(bytesUsed);
      int byteCount = num - bytesUsed;
      bytes -= bytesUsed;
      do
      {
        this.unicodeDecoder.Convert(this.readBuffer, this.readOffset, byteCount, this.decodeBuffer, 0, this.decodeBuffer.Length, true, out bytesUsed, out charsUsed, out completed);
        stringBuilder.Append(this.decodeBuffer, 0, charsUsed);
        this.SkipBytes(bytesUsed);
        byteCount -= bytesUsed;
        bytes -= bytesUsed;
      }
      while (!completed);
      if (bytes != 0)
        this.SkipBytes(bytes);
      return stringBuilder.ToString();
    }

    private int AttributeRemainingCount()
    {
      return this.attributeValueLength - this.attributeValueOffset;
    }

    private bool MoreAttributeData(int bytes)
    {
      return this.AttributeRemainingCount() >= bytes;
    }

    private bool CheckAndEnsureMoreAttributeDataLoaded(int bytes)
    {
      if (this.AttributeRemainingCount() < bytes)
      {
        this.SetComplianceStatus(TnefComplianceStatus.AttributeOverflow, CtsResources.TnefStrings.ReaderComplianceAttributeValueOverflow);
        this.error = true;
        return false;
      }
      if (this.EnsureMoreDataLoaded(bytes))
        return true;
      this.SetComplianceStatus(TnefComplianceStatus.StreamTruncated, CtsResources.TnefStrings.ReaderComplianceTnefTruncated);
      this.error = true;
      return false;
    }

    private void EatAttributeBytes(int count)
    {
      while (this.attributeValueOffset < this.attributeValueLength && count != 0 && this.CheckAndEnsureMoreAttributeDataLoaded(1))
      {
        int count1 = Math.Min(count, Math.Min(this.AttributeRemainingCount(), this.AvailableCount()));
        this.SkipBytes(count1);
        count -= count1;
      }
    }

    private byte PeekByte(int offsetFromCurrentPosition)
    {
      return this.readBuffer[this.readOffset + offsetFromCurrentPosition];
    }

    private short PeekWord(int offsetFromCurrentPosition)
    {
      return BitConverter.ToInt16(this.readBuffer, this.readOffset + offsetFromCurrentPosition);
    }

    private int PeekDword(int offsetFromCurrentPosition)
    {
      return BitConverter.ToInt32(this.readBuffer, this.readOffset + offsetFromCurrentPosition);
    }

    private short ReadByte()
    {
      byte num = this.readBuffer[this.readOffset];
      this.Checksum1(num);
      ++this.readOffset;
      return (short) num;
    }

    private short ReadWord()
    {
      short num = BitConverter.ToInt16(this.readBuffer, this.readOffset);
      this.Checksum2(num);
      this.readOffset += 2;
      this.attributeValueOffset += 2;
      return num;
    }

    private int ReadDword()
    {
      int num = BitConverter.ToInt32(this.readBuffer, this.readOffset);
      this.Checksum4();
      this.readOffset += 4;
      this.attributeValueOffset += 4;
      return num;
    }

    private long ReadQword()
    {
      long num = BitConverter.ToInt64(this.readBuffer, this.readOffset);
      this.Checksum(8);
      this.readOffset += 8;
      this.attributeValueOffset += 8;
      return num;
    }

    private float ReadFloat()
    {
      float num = BitConverter.ToSingle(this.readBuffer, this.readOffset);
      this.Checksum4();
      this.readOffset += 4;
      this.attributeValueOffset += 4;
      return num;
    }

    private double ReadDouble()
    {
      double num = BitConverter.ToDouble(this.readBuffer, this.readOffset);
      this.Checksum(8);
      this.readOffset += 8;
      this.attributeValueOffset += 8;
      return num;
    }

    private Guid ReadGuid()
    {
      Guid guid = new Guid(BitConverter.ToInt32(this.readBuffer, this.readOffset), BitConverter.ToInt16(this.readBuffer, this.readOffset + 4), BitConverter.ToInt16(this.readBuffer, this.readOffset + 6), this.readBuffer[this.readOffset + 8], this.readBuffer[this.readOffset + 9], this.readBuffer[this.readOffset + 10], this.readBuffer[this.readOffset + 11], this.readBuffer[this.readOffset + 12], this.readBuffer[this.readOffset + 13], this.readBuffer[this.readOffset + 14], this.readBuffer[this.readOffset + 15]);
      this.Checksum(16);
      this.readOffset += 16;
      this.attributeValueOffset += 16;
      return guid;
    }

    private void ReadBytes(byte[] buffer, int offset, int count)
    {
      Buffer.BlockCopy((Array) this.readBuffer, this.readOffset, (Array) buffer, offset, count);
      this.Checksum(count);
      this.readOffset += count;
      this.attributeValueOffset += count;
    }

    private void SkipBytes(int count)
    {
      this.Checksum(count);
      this.readOffset += count;
      this.attributeValueOffset += count;
    }

    private int AvailableCount()
    {
      return this.readEnd - this.readOffset;
    }

    private bool EnsureMoreDataLoaded(int bytes)
    {
      if (this.NeedToLoadMoreData(bytes))
        return this.LoadMoreData(bytes);
      return true;
    }

    private bool NeedToLoadMoreData(int bytes)
    {
      return this.AvailableCount() < bytes;
    }

    private bool LoadMoreData(int bytes)
    {
      if (this.endOfFile)
        return false;
      if (this.readBuffer.Length < bytes)
      {
        byte[] numArray = new byte[Math.Max(this.readBuffer.Length * 2, bytes)];
        if (this.readEndReal - this.readOffset != 0)
          Buffer.BlockCopy((Array) this.readBuffer, this.readOffset, (Array) numArray, 0, this.readEndReal - this.readOffset);
        this.readBuffer = numArray;
      }
      else if (this.readEndReal - this.readOffset != 0 && this.readOffset != 0)
        Buffer.BlockCopy((Array) this.readBuffer, this.readOffset, (Array) this.readBuffer, 0, this.readEndReal - this.readOffset);
      int num1 = this.readOffset;
      this.readEndReal -= this.readOffset;
      this.readOffset = 0;
      this.streamOffset += num1;
      int num2 = this.InputStream.Read(this.readBuffer, this.readEndReal, this.readBuffer.Length - this.readEndReal);
      this.readEndReal += num2;
      this.endOfFile = num2 == 0;
      this.readEnd = this.readEndReal;
      if (this.streamOffset + this.readEnd > this.streamMaxLength)
      {
        this.readEnd = this.streamMaxLength - this.streamOffset;
        this.endOfFile = true;
      }
      return this.readEnd >= bytes;
    }

    private void Checksum1(byte value)
    {
      if (this.checksumDisabled)
        return;
      this.checksum += (ushort) value;
    }

    private void Checksum2(short value)
    {
      if (this.checksumDisabled)
        return;
      this.checksum += (ushort) ((uint) value & (uint) byte.MaxValue);
      this.checksum += (ushort) ((int) value >> 8 & (int) byte.MaxValue);
    }

    private void Checksum4()
    {
      if (this.checksumDisabled)
        return;
      this.checksum += (ushort) this.readBuffer[this.readOffset];
      this.checksum += (ushort) this.readBuffer[this.readOffset + 1];
      this.checksum += (ushort) this.readBuffer[this.readOffset + 2];
      this.checksum += (ushort) this.readBuffer[this.readOffset + 3];
    }

    private void Checksum(int count)
    {
      if (this.checksumDisabled)
        return;
      int num = this.readOffset;
      while (count-- > 0)
        this.checksum += (ushort) this.readBuffer[num++];
    }

    private void SetComplianceStatus(TnefComplianceStatus status, string explanation)
    {
      this.complianceStatus |= status;
      if (this.complianceMode == TnefComplianceMode.Strict)
        throw new TnefException(explanation);
    }

    private bool CrackTriple()
    {
      short num1 = this.PeekWord(0);
      short num2 = this.PeekWord(4);
      short num3 = this.PeekWord(6);
      if (this.attributeValueLength < 8 + (int) num2 + (int) num3 || (int) num2 <= 0 || (int) num3 <= 0)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidFrom);
        return false;
      }
      if ((int) this.PeekByte(8 + (int) num2 + (int) num3 - 1) != 0)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidFrom);
        return false;
      }
      this.tripleNameOffset = 8;
      byte num4;
      while ((int) (num4 = this.PeekByte(this.tripleNameOffset)) == 32 || (int) num4 == 9)
        ++this.tripleNameOffset;
      switch (num1)
      {
        case (short) 3:
        case (short) 4:
        case (short) 9:
          int offsetFromCurrentPosition1 = this.tripleNameOffset;
          byte num5;
          while ((int) (num5 = this.PeekByte(offsetFromCurrentPosition1)) != 0)
            ++offsetFromCurrentPosition1;
          this.tripleNameLength = offsetFromCurrentPosition1 - this.tripleNameOffset;
          this.tripleAddressTypeOffset = 8 + (int) num2;
          byte num6;
          while ((int) (num6 = this.PeekByte(this.tripleAddressTypeOffset)) == 32 || (int) num6 == 9)
            ++this.tripleAddressTypeOffset;
          this.tripleAddressTypeLength = 0;
          this.tripleAddressOffset = 0;
          this.tripleAddressLength = 0;
          int offsetFromCurrentPosition2 = this.tripleAddressTypeOffset;
          byte num7;
          while ((int) (num7 = this.PeekByte(offsetFromCurrentPosition2)) != 0 && (int) num7 != 58)
            ++offsetFromCurrentPosition2;
          if ((int) num7 == 58)
          {
            this.tripleAddressTypeLength = offsetFromCurrentPosition2 - this.tripleAddressTypeOffset;
            while (this.tripleAddressTypeLength > 0 && (int) (num7 = this.PeekByte(this.tripleAddressTypeOffset + this.tripleAddressTypeLength - 1)) == 32 || (int) num7 == 9)
              --this.tripleAddressTypeLength;
            this.tripleAddressOffset = offsetFromCurrentPosition2 + 1;
            byte num8;
            while ((int) (num8 = this.PeekByte(this.tripleAddressOffset)) == 32 || (int) num8 == 9)
              ++this.tripleAddressOffset;
            int offsetFromCurrentPosition3 = this.tripleAddressOffset;
            while ((int) (num5 = this.PeekByte(offsetFromCurrentPosition3)) != 0)
              ++offsetFromCurrentPosition3;
            this.tripleAddressLength = offsetFromCurrentPosition3 - this.tripleAddressOffset;
          }
          if (this.tripleAddressTypeLength != 0 && this.tripleAddressLength != 0)
            return true;
          this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidFrom);
          return false;
        default:
          this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidFrom);
          return false;
      }
    }

    private bool CrackSchedulePlusId()
    {
      short num1 = this.PeekWord(0);
      if ((int) num1 <= 0 || 2 + (int) num1 + 2 > this.attributeValueLength || (int) this.PeekByte(2 + (int) num1 - 1) != 0)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidSchedulePlus);
        return false;
      }
      this.tripleNameOffset = 2;
      byte num2;
      while ((int) (num2 = this.PeekByte(this.tripleNameOffset)) == 32 || (int) num2 == 9)
        ++this.tripleNameOffset;
      int offsetFromCurrentPosition1 = this.tripleNameOffset;
      byte num3;
      while ((int) (num3 = this.PeekByte(offsetFromCurrentPosition1)) != 0)
        ++offsetFromCurrentPosition1;
      this.tripleNameLength = offsetFromCurrentPosition1 - this.tripleNameOffset;
      short num4 = this.PeekWord(2 + (int) num1);
      if ((int) num4 <= 0 || 2 + (int) num1 + 2 + (int) num4 > this.attributeValueLength || (int) this.PeekByte(2 + (int) num1 + 2 + (int) num4 - 1) != 0)
      {
        this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidSchedulePlus);
        return false;
      }
      this.tripleAddressTypeOffset = 2 + (int) num1 + 2;
      byte num5;
      while ((int) (num5 = this.PeekByte(this.tripleAddressTypeOffset)) == 32 || (int) num5 == 9)
        ++this.tripleAddressTypeOffset;
      this.tripleAddressTypeLength = 0;
      this.tripleAddressOffset = 0;
      this.tripleAddressLength = 0;
      int offsetFromCurrentPosition2 = this.tripleAddressTypeOffset;
      byte num6;
      while ((int) (num6 = this.PeekByte(offsetFromCurrentPosition2)) != 0 && (int) num6 != 58)
        ++offsetFromCurrentPosition2;
      if ((int) num6 == 58)
      {
        this.tripleAddressTypeLength = offsetFromCurrentPosition2 - this.tripleAddressTypeOffset;
        while (this.tripleAddressTypeLength > 0 && (int) (num6 = this.PeekByte(this.tripleAddressTypeOffset + this.tripleAddressTypeLength - 1)) == 32 || (int) num6 == 9)
          --this.tripleAddressTypeLength;
        this.tripleAddressOffset = offsetFromCurrentPosition2 + 1;
        byte num7;
        while ((int) (num7 = this.PeekByte(this.tripleAddressOffset)) == 32 || (int) num7 == 9)
          ++this.tripleAddressOffset;
        int offsetFromCurrentPosition3 = this.tripleAddressOffset;
        while ((int) (num3 = this.PeekByte(offsetFromCurrentPosition3)) != 0)
          ++offsetFromCurrentPosition3;
        this.tripleAddressLength = offsetFromCurrentPosition3 - this.tripleAddressOffset;
      }
      if (this.tripleNameLength != 0 && this.tripleAddressTypeLength != 0 && this.tripleAddressLength != 0)
        return true;
      this.SetComplianceStatus(TnefComplianceStatus.InvalidAttributeValue, CtsResources.TnefStrings.ReaderComplianceInvalidSchedulePlus);
      return false;
    }

    internal void AssertGoodToUse(bool affectsChild)
    {
      if (this.InputStream == null)
        throw new ObjectDisposedException("TnefReader");
      if (affectsChild && this.Child != null)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationChildActive);
    }

    internal void AssertInAttribute()
    {
      if (this.ReadStateValue < TnefReader.ReadState.EndAttribute)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeInAttribute);
    }

    internal void AssertInProperty()
    {
      if (this.ReadStateValue < TnefReader.ReadState.EndProperty)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeInProperty);
    }

    internal void AssertInPropertyValue()
    {
      if (this.ReadStateValue >= TnefReader.ReadState.EndPropertyValue)
        return;
      if (this.ReadStateValue != TnefReader.ReadState.BeginProperty || this.propertyTag.IsMultiValued || this.propertyTag.ValueTnefType == TnefPropertyType.Null)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeInPropertyValue);
      this.ReadNextPropertyValue();
    }

    private void AssertAtTheBeginningOfPropertyValue()
    {
      this.AssertGoodToUse(true);
      if (this.ReadStateValue == TnefReader.ReadState.BeginPropertyValue)
        return;
      if (this.ReadStateValue != TnefReader.ReadState.BeginProperty || this.propertyTag.IsMultiValued || this.propertyTag.ValueTnefType == TnefPropertyType.Null)
        throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeInPropertyValue);
      this.ReadNextPropertyValue();
    }

    private static DateTime FromOADate(double value)
    {
      return DateTime.FromOADate(value);
    }

    internal enum ReadState
    {
      EndOfFile,
      Begin,
      EndAttribute,
      BeginAttribute,
      ReadAttributeValue,
      EndRow,
      BeginRow,
      EndProperty,
      BeginProperty,
      EndPropertyValue,
      BeginPropertyValue,
      ReadPropertyValue,
    }
  }
}
