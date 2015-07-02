using System;
using System;
using System.Linq;
using IntrospectionExtensions = System.Reflection.IntrospectionExtensions;

namespace Butler.Schema.Data.ContentTypes.Tnef {

    public class TnefWriter : IDisposable {

        private TnefWriter(TnefWriter parent, int messageCodePage) {
            canFlush = true;
            this.parent = parent;
            this.parent.Child = this;
            flags = parent.flags;
            attachmentKey = parent.attachmentKey;
            unicodeEncoder = parent.unicodeEncoder;
            this.SetMessageCodePage(messageCodePage);
            outputStream = parent.outputStream;
            streamOffset = parent.streamOffset;
            writeStream = parent.writeStream;
            writeStreamOffset = parent.writeStreamOffset;
            writeBuffer = parent.writeBuffer;
            writeOffset = parent.writeOffset;
            charBuffer = parent.charBuffer;
            byteBuffer = parent.byteBuffer;
            fabricatedBuffer = parent.fabricatedBuffer;
            directWrite = true;
            this.WriteDword(0x223e9f78);
            this.WriteWord(attachmentKey);
            totalLength += 6;
            writeState = WriteState.BeforeAttribute;
            if ((flags & TnefWriterFlags.NoStandardAttributes) == 0) {
                this.WriteTnefVersion();
                this.WriteOemCodePage(messageCodePage);
            } else
                this.SetMessageCodePage(messageCodePage);
            if (string8Encoder == null) {
                string8Encoder = Globalization.Charset.DefaultWindowsCharset.GetEncoding()
                                              .GetEncoder();
            }
        }

        public TnefWriter(System.IO.Stream outputStream, short attachmentKey) : this(outputStream, attachmentKey, 0, TnefWriterFlags.NoStandardAttributes) {}

        public TnefWriter(System.IO.Stream outputStream, short attachmentKey, int messageCodePage) : this(outputStream, attachmentKey, messageCodePage, 0) {}

        public TnefWriter(System.IO.Stream outputStream, short attachmentKey, int messageCodePage, TnefWriterFlags flags) {
            canFlush = true;
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            if (!outputStream.CanWrite)
                throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportWrite);
            this.flags = flags;
            this.outputStream = outputStream;
            writeStream = outputStream;
            if (!outputStream.CanSeek)
                writeStream = Data.Internal.ApplicationServices.Provider.CreateTemporaryStorage();
            unicodeEncoder = System.Text.Encoding.Unicode.GetEncoder();
            writeBuffer = new byte[0x1000];
            charBuffer = new char[0x400];
            byteBuffer = new byte[0x400];
            fabricatedBuffer = new byte[0x200];
            this.attachmentKey = attachmentKey;
            directWrite = true;
            this.WriteDword(0x223e9f78);
            this.WriteWord(this.attachmentKey);
            totalLength += 6;
            if (!this.outputStream.CanSeek)
                this.FlushWriteStreamToOutput();
            writeState = WriteState.BeforeAttribute;
            if ((this.flags & TnefWriterFlags.NoStandardAttributes) == 0) {
                this.WriteTnefVersion();
                this.WriteOemCodePage(messageCodePage);
            } else
                this.SetMessageCodePage(messageCodePage);
            if (string8Encoder == null) {
                string8Encoder = Globalization.Charset.DefaultWindowsCharset.GetEncoding()
                                              .GetEncoder();
            }
        }

        public int MessageCodePage { get; private set; }

        private int StreamOffset {
            get {
                return (streamOffset + writeOffset);
            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void AssertGoodToUse(bool affectsChild) {
            if (outputStream == null)
                throw new ObjectDisposedException("TnefWriter");
            if (affectsChild && (Child != null))
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationChildActive);
        }

        private void Checksum(byte[] buffer, int offset, int count) {
            while (count-- > 0)
                checksum = (ushort) (checksum + buffer[offset++]);
        }

        public void Close() {
            this.Dispose();
        }

        private void CompleteLegacyAttribute() {
            directWrite = true;
            switch (this.attributeTag) {
                case TnefAttributeTag.AttachRenderData: {
                    short num6 = 1;
                    var num7 = 0;
                    var num8 = -1;
                    short num9 = -1;
                    short num10 = -1;
                    if (attachMethodOffset != -1) {
                        if (BitConverter.ToInt32(fabricatedBuffer, attachMethodOffset) == 6)
                            num6 = 2;
                        else {
                            if ((attachEncodingOffset != -1) && TnefCommon.BytesEqualToPattern(fabricatedBuffer, attachEncodingOffset, TnefCommon.OidMacBinary))
                                num7 = 1;
                            num9 = 0x20;
                            num10 = 0x20;
                        }
                    }
                    if (renderingPositionOffset != -1)
                        num8 = BitConverter.ToInt32(fabricatedBuffer, renderingPositionOffset);
                    this.WriteWord(num6);
                    this.WriteDword(num8);
                    this.WriteWord(num9);
                    this.WriteWord(num10);
                    this.WriteDword(num7);
                    return;
                }
                case TnefAttributeTag.OriginalMessageClass:
                case TnefAttributeTag.MessageClass: {
                    if ((fabricatedOffset == 0) || (fabricatedBuffer[0] == 0))
                        throw new ExchangeDataException(CtsResources.TnefStrings.WriterNotSupportedInvalidMessageClass);
                    var index = 0;
                    while (index < TnefCommon.MessageClassMappingTable.Length) {
                        if (((fabricatedOffset == (TnefCommon.MessageClassMappingTable[index].MapiName.Length + 1)) && TnefCommon.BytesEqualToPattern(fabricatedBuffer, 0, TnefCommon.MessageClassMappingTable[index].MapiName)) &&
                            (fabricatedBuffer[fabricatedOffset - 1] == 0)) {
                            var attributeTag = this.attributeTag;
                            var bytes = CTSGlobals.AsciiEncoding.GetBytes(TnefCommon.MessageClassMappingTable[index].LegacyName);
                            this.WriteBinary(bytes, 0, bytes.Length);
                            this.WriteByte(0);
                            break;
                        }
                        index++;
                    }
                    if (index == TnefCommon.MessageClassMappingTable.Length) {
                        this.WriteBinary(fabricatedBuffer, 0, fabricatedOffset);
                        if (fabricatedBuffer[fabricatedOffset - 1] == 0)
                            return;
                        this.WriteByte(0);
                    }
                    return;
                }
                case TnefAttributeTag.Owner:
                case TnefAttributeTag.SentFor: {
                    if (((nameOffset == -1) || (addrTypeOffset == -1)) || (addressOffset == -1)) {
                        if (entryIdOffset != -1)
                            this.CrackEntryId();
                        if (((nameOffset == -1) || (addrTypeOffset == -1)) || (addressOffset == -1)) {
                            if (!disposing)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedNotEnoughInformationForAttribute);
                            return;
                        }
                    }
                    var count = TnefCommon.StrZLength(fabricatedBuffer, nameOffset, fabricatedOffset);
                    var num19 = TnefCommon.StrZLength(fabricatedBuffer, addrTypeOffset, fabricatedOffset);
                    var num20 = TnefCommon.StrZLength(fabricatedBuffer, addressOffset, fabricatedOffset);
                    if (((count == 0) || (num19 == 0)) || (num20 == 0)) {
                        if (!disposing)
                            throw new iCalendar.InvalidCalendarDataException(CtsResources.TnefStrings.WriterNotSupportedInvalidRecipientInformation);
                    } else {
                        this.WriteWord((short) (count + 1));
                        this.WriteBinary(fabricatedBuffer, nameOffset, count);
                        this.WriteByte(0);
                        this.WriteWord((short) (((num19 + 1) + num20) + 1));
                        this.WriteBinary(fabricatedBuffer, addrTypeOffset, num19);
                        this.WriteByte(0x3a);
                        this.WriteBinary(fabricatedBuffer, addressOffset, num20);
                        this.WriteByte(0);
                    }
                    return;
                }
                case TnefAttributeTag.MessageStatus: {
                    if (fabricatedOffset != 4) {
                        if (!disposing)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
                        while (fabricatedOffset < 4) {
                            this.EnsureFabricatedSpace(1);
                            fabricatedBuffer[fabricatedOffset++] = 0;
                        }
                    }
                    var num2 = BitConverter.ToInt32(fabricatedBuffer, 0);
                    var num3 = 0;
                    num3 |= ((num2 & 1) != 0) ? 0x20 : 0;
                    num3 |= ((num2 & 2) != 0) ? 0 : 1;
                    num3 |= ((num2 & 4) != 0) ? 4 : 0;
                    num3 |= ((num2 & 0x10) != 0) ? 0x80 : 0;
                    num3 |= ((num2 & 8) != 0) ? 2 : 0;
                    this.WriteByte((byte) num3);
                    return;
                }
                case TnefAttributeTag.Priority: {
                    if (fabricatedOffset != 4) {
                        if (!disposing)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
                        while (fabricatedOffset < 4) {
                            this.EnsureFabricatedSpace(1);
                            fabricatedBuffer[fabricatedOffset++] = 0;
                        }
                    }
                    var num4 = BitConverter.ToInt32(fabricatedBuffer, 0);
                    short num5 = 2;
                    switch (num4) {
                        case 0:
                            num5 = 3;
                            break;

                        case 2:
                            num5 = 1;
                            break;
                    }
                    this.WriteWord(num5);
                    return;
                }
                case TnefAttributeTag.DateSent:
                case TnefAttributeTag.DateReceived:
                case TnefAttributeTag.AttachCreateDate:
                case TnefAttributeTag.AttachModifyDate:
                case TnefAttributeTag.DateModified:
                case TnefAttributeTag.DateStart:
                case TnefAttributeTag.DateEnd:
                    if (fabricatedOffset != 8) {
                        if (!disposing)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
                        while (fabricatedOffset < 8) {
                            this.EnsureFabricatedSpace(1);
                            fabricatedBuffer[fabricatedOffset++] = 0;
                        }
                    }
                    break;

                case TnefAttributeTag.From: {
                    if (((nameOffset == -1) || (addrTypeOffset == -1)) || (addressOffset == -1)) {
                        if (entryIdOffset != -1)
                            this.CrackEntryId();
                        if (((nameOffset == -1) || (addrTypeOffset == -1)) || (addressOffset == -1)) {
                            if (!disposing)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedNotEnoughInformationForAttribute);
                            return;
                        }
                    }
                    var num12 = TnefCommon.StrZLength(fabricatedBuffer, nameOffset, fabricatedOffset);
                    var num13 = TnefCommon.StrZLength(fabricatedBuffer, addrTypeOffset, fabricatedOffset);
                    var num14 = TnefCommon.StrZLength(fabricatedBuffer, addressOffset, fabricatedOffset);
                    if (((num12 == 0) || (num13 == 0)) || (num14 == 0)) {
                        if (!disposing)
                            throw new iCalendar.InvalidCalendarDataException(CtsResources.TnefStrings.WriterNotSupportedInvalidRecipientInformation);
                        return;
                    }
                    var num15 = (short) (num12 + 1);
                    if ((num15 & 1) != 0)
                        num15 = (short) (num15 + 1);
                    var num16 = (short) (((num13 + 1) + num14) + 1);
                    var num17 = (short) ((0x10 + num15) + num16);
                    this.WriteWord(4);
                    this.WriteWord(num17);
                    this.WriteWord(num15);
                    this.WriteWord(num16);
                    this.WriteBinary(fabricatedBuffer, nameOffset, num12);
                    this.WriteByte(0);
                    if (((num12 + 1) & 1) != 0)
                        this.WriteByte(0);
                    this.WriteBinary(fabricatedBuffer, addrTypeOffset, num13);
                    this.WriteByte(0x3a);
                    this.WriteBinary(fabricatedBuffer, addressOffset, num14);
                    this.WriteByte(0);
                    this.WriteQword(0L);
                    return;
                }
                default:
                    return;
            }
            var time = DateTime.FromFileTimeUtc(BitConverter.ToInt64(fabricatedBuffer, 0));
            this.WriteWord((short) time.Year);
            this.WriteWord((short) time.Month);
            this.WriteWord((short) time.Day);
            this.WriteWord((short) time.Hour);
            this.WriteWord((short) time.Minute);
            this.WriteWord((short) time.Second);
            this.WriteWord((short) time.DayOfWeek);
        }

        private void CompleteStartProperty() {
            if (propertyTag.IsMultiValued) {
                valueCountOffset = this.StreamOffset;
                this.WriteDword(0);
            } else if (((propertyTag.ValueTnefType == TnefPropertyType.String8) || (propertyTag.ValueTnefType == TnefPropertyType.Unicode)) ||
                       ((propertyTag.ValueTnefType == TnefPropertyType.Binary) || (propertyTag.ValueTnefType == TnefPropertyType.Object)))
                this.WriteDword(1);
            writeState = WriteState.BeforePropertyValue;
        }

        private void CopyStreamContentAsRawValue(System.IO.Stream stream) {
            int num;
            while ((num = stream.Read(byteBuffer, 0, byteBuffer.Length)) != 0)
                this.WriteBinary(byteBuffer, 0, num);
        }

        private void CrackEntryId() {
            if ((fabricatedOffset - entryIdOffset) < 0x1b)
                throw new ArgumentException(CtsResources.TnefStrings.WriterNotSupportedMallformedEntryId);
            if (!TnefCommon.BytesEqualToPattern(fabricatedBuffer, entryIdOffset + 4, TnefCommon.MuidOOP))
                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedNotOneOffEntryId);
            if ((BitConverter.ToInt32(fabricatedBuffer, entryIdOffset + 20) & 0x80000000L) != 0L)
                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedUnicodeOneOffEntryId);
            var offset = entryIdOffset + 0x18;
            var num3 = TnefCommon.StrZLength(fabricatedBuffer, offset, fabricatedOffset);
            var num4 = (offset + num3) + 1;
            if (num4 >= fabricatedOffset)
                throw new ArgumentException(CtsResources.TnefStrings.WriterNotSupportedMallformedEntryId);
            var num5 = TnefCommon.StrZLength(fabricatedBuffer, num4, fabricatedOffset);
            var num6 = (num4 + num5) + 1;
            if (num6 >= fabricatedOffset)
                throw new ArgumentException(CtsResources.TnefStrings.WriterNotSupportedMallformedEntryId);
            if ((TnefCommon.StrZLength(fabricatedBuffer, num6, fabricatedOffset) + 1) > fabricatedOffset)
                throw new ArgumentException(CtsResources.TnefStrings.WriterNotSupportedMallformedEntryId);
            if (nameOffset == -1)
                nameOffset = offset;
            if (addrTypeOffset == -1)
                addrTypeOffset = num4;
            if (addressOffset == -1)
                addressOffset = num6;
        }

        private void DirectWrite(byte[] buffer, int offset, int count) {
            canFlush = false;
            writeStream.Write(buffer, offset, count);
            canFlush = true;
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this.disposing = true;
                if (outputStream == null)
                    return;
                if (canFlush) {
                    if (Child != null) {
                        if (Child is TnefWriterStreamWrapper)
                            (Child as TnefWriterStreamWrapper).Dispose();
                        else if (Child is TnefWriter)
                            (Child as TnefWriter).Dispose();
                    }
                    this.Flush();
                    if (parent != null) {
                        parent.streamOffset = streamOffset;
                        parent.attributeLength += totalLength;
                        parent.valueLength += totalLength;
                        parent.checksum = (ushort) (parent.checksum + checksum);
                        parent.writeOffset = writeOffset;
                        parent.Child = null;
                    } else {
                        if (!outputStream.CanSeek)
                            writeStream.Dispose();
                        outputStream.Dispose();
                    }
                }
            }
            outputStream = null;
            writeStream = null;
            writeBuffer = null;
            byteBuffer = null;
            charBuffer = null;
            fabricatedBuffer = null;
            unicodeEncoder = null;
            string8Encoder = null;
        }

        private void EndAttribute() {
            if ((writeState != WriteState.BeginAttribute) && (writeState != WriteState.WriteAttributeValue)) {
                if (writeState >= WriteState.BeforePropertyValue)
                    this.EndProperty();
                if (!legacyAttribute) {
                    if (writeState == WriteState.BeforeProperty) {
                        if (attributeTag == TnefAttributeTag.RecipientTable)
                            this.EndRow();
                        else if (propertyCount != 0)
                            this.ReWriteDword(propertyCountOffset, propertyCount);
                    }
                    if ((writeState == WriteState.BeforeRow) && (rowCount != 0))
                        this.ReWriteDword(rowCountOffset, rowCount);
                } else
                    this.CompleteLegacyAttribute();
            }
            var attributeLength = this.attributeLength;
            var num2 = (ushort) (checksum - attributeStartChecksum);
            this.WriteWord((short) num2);
            if (attributeLength != 0) {
                totalLength += attributeLength;
                this.ReWriteDword(attributeLengthOffset, attributeLength);
            }
            totalLength += 2;
            if ((parent == null) && !outputStream.CanSeek)
                this.FlushWriteStreamToOutput();
            writeState = WriteState.BeforeAttribute;
        }

        private void EndProperty() {
            if (writeState >= WriteState.BeginPropertyValue)
                this.EndPropertyValue();
            if (!legacyAttribute) {
                if (propertyTag.IsMultiValued && (valueCount != 0))
                    this.ReWriteDword(valueCountOffset, valueCount);
                var count = (4 - ((valueCount*valueFixedLength)%4))%4;
                if (count != 0)
                    this.WriteBinary(TnefCommon.Padding, 0, count);
            }
            writeState = WriteState.BeforeProperty;
        }

        private void EndPropertyValue() {
            if ((valueFixedLength != 0) && (this.valueLength != valueFixedLength)) {
                if (!disposing || (valueLength > valueFixedLength))
                    throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationValueSizeInvalidForType);
                while (valueLength != valueFixedLength)
                    this.WriteByte(0);
            }
            if (valueAsText) {
                if (propertyTag.ValueTnefType == TnefPropertyType.String8) {
                    this.WriteString8Text(charBuffer, 0, 0, true);
                    this.WriteByte(0);
                } else if (propertyTag.ValueTnefType == TnefPropertyType.Unicode) {
                    this.WriteUnicodeText(charBuffer, 0, 0, true);
                    this.WriteWord(0);
                }
            } else if (legacyAttribute && hexEncodeBinary) {
                directWrite = true;
                this.WriteByte(0);
            }
            if (!legacyAttribute &&
                (((propertyTag.ValueTnefType == TnefPropertyType.String8) || (propertyTag.ValueTnefType == TnefPropertyType.Unicode)) ||
                 ((propertyTag.ValueTnefType == TnefPropertyType.Binary) || (propertyTag.ValueTnefType == TnefPropertyType.Object)))) {
                var valueLength = this.valueLength;
                var count = (4 - (valueLength%4))%4;
                if (count != 0)
                    this.WriteBinary(TnefCommon.Padding, 0, count);
                if (valueLength != 0)
                    this.ReWriteDword(valueLengthOffset, valueLength);
            }
            writeState = WriteState.BeforePropertyValue;
        }

        private void EndRow() {
            if (writeState >= WriteState.BeforePropertyValue)
                this.EndProperty();
            if (propertyCount != 0)
                this.ReWriteDword(propertyCountOffset, propertyCount);
            writeState = WriteState.BeforeRow;
        }

        private void EnsureFabricatedSpace(int count) {
            if ((fabricatedOffset + count) > fabricatedBuffer.Length) {
                if ((fabricatedOffset + count) > 0x8000)
                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedLegacyAttributeTooLong);
                var num = fabricatedBuffer.Length*2;
                while ((num - fabricatedOffset) < count)
                    num *= 2;
                var dst = new byte[num];
                Buffer.BlockCopy(fabricatedBuffer, 0, dst, 0, fabricatedOffset);
                fabricatedBuffer = dst;
            }
        }

        private void EnsureSpace(int count) {
            if ((writeOffset + count) > writeBuffer.Length)
                this.FlushBuffer();
        }

        public void Flush() {
            this.AssertGoodToUse(true);
            if (writeState > WriteState.BeforeAttribute)
                this.EndAttribute();
            if (parent == null)
                this.FlushBuffer();
        }

        private void FlushBuffer() {
            var writeOffset = this.writeOffset;
            this.writeOffset = 0;
            this.DirectWrite(writeBuffer, 0, writeOffset);
            streamOffset += writeOffset;
        }

        private void FlushWriteStreamToOutput() {
            canFlush = false;
            if (writeStream.Length != 0L) {
                int num;
                writeStream.Position = 0L;
                while ((num = writeStream.Read(byteBuffer, 0, byteBuffer.Length)) != 0) {
                    outputStream.Write(byteBuffer, 0, num);
                    writeStreamOffset += num;
                }
                writeStream.Position = 0L;
                writeStream.SetLength(0L);
            }
            if (writeOffset != 0) {
                outputStream.Write(writeBuffer, 0, writeOffset);
                streamOffset += writeOffset;
                writeStreamOffset += writeOffset;
                writeOffset = 0;
            }
            canFlush = true;
        }

        public TnefWriter GetEmbeddedMessageWriter() {
            this.AssertGoodToUse(true);
            return this.GetEmbeddedMessageWriter(this.MessageCodePage);
        }

        public TnefWriter GetEmbeddedMessageWriter(int messageCodePage) {
            this.AssertGoodToUse(true);
            if ((writeState != WriteState.BeginPropertyValue) && (writeState != WriteState.WritePropertyValue))
                this.StartPropertyValue();
            if (writeState == WriteState.BeginPropertyValue)
                this.WriteGuid(TnefCommon.MessageIID);
            return new TnefWriter(this, messageCodePage);
        }

        public System.IO.Stream GetRawPropertyValueWriteStream() {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            return new TnefWriterStreamWrapper(this);
        }

        private static TypeCode GetTypeCode(object value) {
            return Type.GetTypeCode(value.GetType());
        }

        private void HexEncodeBinary(byte[] buffer, int offset, int count) {
            var num = 0;
            while (count != 0) {
                this.EnsureSpace(Math.Min(0x80, count*2));
                var num2 = Math.Min(count*2, writeBuffer.Length - writeOffset) & -2;
                for (var i = 0; i < num2; i += 2) {
                    writeBuffer[writeOffset + i] = TnefCommon.HexDigit[buffer[offset] >> 4];
                    writeBuffer[(writeOffset + i) + 1] = TnefCommon.HexDigit[buffer[offset] & 15];
                    offset++;
                }
                this.Checksum(writeBuffer, writeOffset, num2);
                writeOffset += num2;
                num += num2;
                count -= num2/2;
            }
            valueLength += num;
            attributeLength += num;
        }

        private bool IsLegacyAttribute() {
            return (((attributeTag != TnefAttributeTag.MapiProperties) && (attributeTag != TnefAttributeTag.Attachment)) && (attributeTag != TnefAttributeTag.RecipientTable));
        }

        private void PrepareToStartProperty(TnefPropertyTag tag, bool nameAvailable) {
            if (tag.IsNamed != nameAvailable)
                throw new InvalidOperationException(tag.IsNamed ? CtsResources.TnefStrings.WriterInvalidOperationStartNamedPropertyNoName : CtsResources.TnefStrings.WriterInvalidOperationStartNormalPropertyWithName);
            if (legacyAttribute && tag.IsNamed)
                throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationNamedPropertyInLegacyAttribute);
            if (writeState != WriteState.BeforeProperty) {
                if ((writeState == WriteState.BeginAttribute) && (attributeTag != TnefAttributeTag.RecipientTable)) {
                    if (!legacyAttribute) {
                        propertyCountOffset = this.StreamOffset;
                        this.WriteDword(0);
                        propertyCount = 0;
                    } else
                        this.StartLegacyAttribute();
                    writeState = WriteState.BeforeProperty;
                } else {
                    if (writeState < WriteState.BeforeProperty)
                        throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperation);
                    this.EndProperty();
                }
            }
            propertyTag = tag;
            propertyCount++;
            valueCount = 0;
            valueLength = 0;
            directWrite = true;
            string8AsUnicode = false;
            var valueTnefType = tag.ValueTnefType;
            if (valueTnefType <= TnefPropertyType.Unicode) {
                switch (valueTnefType) {
                    case TnefPropertyType.Null:
                        if (tag.IsMultiValued)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationInvalidPropertyType);
                        valueFixedLength = 0;
                        return;

                    case TnefPropertyType.I2:
                        valueFixedLength = 2;
                        return;

                    case TnefPropertyType.Long:
                        valueFixedLength = 4;
                        return;

                    case TnefPropertyType.R4:
                        valueFixedLength = 4;
                        return;

                    case TnefPropertyType.Double:
                        valueFixedLength = 8;
                        return;

                    case TnefPropertyType.Currency:
                        valueFixedLength = 8;
                        return;

                    case TnefPropertyType.AppTime:
                        valueFixedLength = 8;
                        return;

                    case TnefPropertyType.Error:
                        valueFixedLength = 4;
                        return;

                    case TnefPropertyType.Boolean:
                        valueFixedLength = 2;
                        return;

                    case TnefPropertyType.Object:
                        if (tag.IsMultiValued)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationMvObject);
                        valueFixedLength = 0;
                        return;

                    case TnefPropertyType.I8:
                        valueFixedLength = 8;
                        return;

                    case TnefPropertyType.String8:
                    case TnefPropertyType.Unicode:
                        goto Label_01C9;
                }
                goto Label_0209;
            }
            if (valueTnefType != TnefPropertyType.SysTime) {
                if (valueTnefType == TnefPropertyType.ClassId) {
                    valueFixedLength = 0x10;
                    return;
                }
                if (valueTnefType == TnefPropertyType.Binary)
                    goto Label_01C9;
                goto Label_0209;
            }
            valueFixedLength = 8;
            return;
            Label_01C9:
            valueFixedLength = 0;
            return;
            Label_0209:
            throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationInvalidPropertyType);
        }

        private void ReWriteDword(int streamOffset, int value) {
            var num = this.StreamOffset - streamOffset;
            if (num <= writeOffset) {
                TnefBitConverter.GetBytes(writeBuffer, writeOffset - num, value);
                this.Checksum(writeBuffer, writeOffset - num, 4);
            } else {
                TnefBitConverter.GetBytes(byteBuffer, 0, value);
                this.Checksum(byteBuffer, 0, 4);
                canFlush = false;
                writeStream.Position = streamOffset - writeStreamOffset;
                writeStream.Write(byteBuffer, 0, 4);
                writeStream.Position = this.streamOffset - writeStreamOffset;
                canFlush = true;
            }
        }

        public void SetMessageCodePage(int messageCodePage) {
            if (this.MessageCodePage != messageCodePage) {
                System.Text.Encoding encoding = null;
                if (messageCodePage == 0) {
                    encoding = Globalization.Charset.DefaultWindowsCharset.GetEncoding();
                    messageCodePage = Globalization.CodePageMap.GetCodePage(encoding);
                } else
                    encoding = Globalization.Charset.GetEncoding(messageCodePage);
                if (TnefCommon.IsUnicodeCodepage(Globalization.CodePageMap.GetCodePage(encoding))) {
                    messageCodePage = 0x4e4;
                    encoding = Globalization.Charset.GetEncoding(messageCodePage);
                }
                string8Encoder = encoding.GetEncoder();
                this.MessageCodePage = messageCodePage;
            }
        }

        public void StartAttribute(TnefAttributeTag tag, TnefAttributeLevel level) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeforeAttribute)
                this.EndAttribute();
            attributeLevel = level;
            attributeTag = tag;
            legacyAttribute = this.IsLegacyAttribute();
            directWrite = true;
            this.WriteByte((byte) attributeLevel);
            this.WriteDword((int) attributeTag);
            attributeLengthOffset = this.StreamOffset;
            this.WriteDword(0);
            attributeLength = 0;
            attributeStartChecksum = checksum;
            totalLength += 9;
            writeState = WriteState.BeginAttribute;
        }

        private void StartLegacyAttribute() {
            fabricatedOffset = 0;
            switch (attributeTag) {
                case TnefAttributeTag.Owner:
                case TnefAttributeTag.SentFor:
                case TnefAttributeTag.From:
                    entryIdOffset = -1;
                    nameOffset = -1;
                    addrTypeOffset = -1;
                    addressOffset = -1;
                    return;

                case TnefAttributeTag.AttachRenderData:
                    renderingPositionOffset = -1;
                    attachMethodOffset = -1;
                    attachEncodingOffset = -1;
                    return;
            }
        }

        private void StartLegacyAttributeProperty() {
            directWrite = false;
            string8AsUnicode = false;
            hexEncodeBinary = false;
            var attributeTag = this.attributeTag;
            if (attributeTag <= TnefAttributeTag.DateModified) {
                if (attributeTag <= TnefAttributeTag.AttachTitle) {
                    if (attributeTag <= TnefAttributeTag.From) {
                        if ((attributeTag != TnefAttributeTag.Null) && (attributeTag == TnefAttributeTag.From)) {
                            if (propertyTag.Id == TnefPropertyId.SenderEntryId) {
                                if (entryIdOffset != -1)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if (propertyTag.TnefType != TnefPropertyType.Binary)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                entryIdOffset = fabricatedOffset;
                                return;
                            }
                            if (propertyTag.Id == TnefPropertyId.SenderName) {
                                if (nameOffset != -1)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                    string8AsUnicode = true;
                                    propertyTag = TnefPropertyTag.SenderNameA;
                                }
                                nameOffset = fabricatedOffset;
                                return;
                            }
                            if (propertyTag.Id == TnefPropertyId.SenderAddrtype) {
                                if (addrTypeOffset != -1)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                    string8AsUnicode = true;
                                    propertyTag = TnefPropertyTag.SenderAddrtypeA;
                                }
                                addrTypeOffset = fabricatedOffset;
                                return;
                            }
                            if (propertyTag.Id != TnefPropertyId.SenderEmailAddress)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (addressOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.SenderEmailAddressA;
                            }
                            addressOffset = fabricatedOffset;
                            return;
                        }
                    } else {
                        switch (attributeTag) {
                            case TnefAttributeTag.MessageId:
                                if (propertyTag.Id != TnefPropertyId.SearchKey)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                                if (attributeLength != 0)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if (propertyTag.TnefType != TnefPropertyType.Binary)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                hexEncodeBinary = true;
                                return;

                            case TnefAttributeTag.ParentId:
                                if (propertyTag.Id != TnefPropertyId.ParentKey)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                                if (attributeLength != 0)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if (propertyTag.TnefType != TnefPropertyType.Binary)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                hexEncodeBinary = true;
                                return;

                            case TnefAttributeTag.ConversationId:
                                if (propertyTag.Id != TnefPropertyId.ConversationKey)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                                if (attributeLength != 0)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if (propertyTag.TnefType != TnefPropertyType.Binary)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                hexEncodeBinary = true;
                                return;

                            case TnefAttributeTag.AttachTitle:
                                if (propertyTag.Id != TnefPropertyId.AttachFilename)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                                if (attributeLength != 0)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                    string8AsUnicode = true;
                                    propertyTag = TnefPropertyTag.AttachFilenameA;
                                }
                                directWrite = true;
                                return;

                            case TnefAttributeTag.Subject:
                                if (propertyTag.Id != TnefPropertyId.Subject)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                                if (attributeLength != 0)
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                                if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                    throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                                if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                    string8AsUnicode = true;
                                    propertyTag = TnefPropertyTag.SubjectA;
                                }
                                directWrite = true;
                                return;
                        }
                    }
                } else {
                    switch (attributeTag) {
                        case TnefAttributeTag.DateStart:
                            if (propertyTag.Id != TnefPropertyId.StartDate)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (fabricatedOffset != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.SysTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            return;

                        case TnefAttributeTag.DateEnd:
                            if (propertyTag.Id != TnefPropertyId.EndDate)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (fabricatedOffset != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.SysTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            return;

                        case TnefAttributeTag.Body:
                            if (propertyTag.Id != TnefPropertyId.Body)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (attributeLength != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.BodyA;
                            }
                            directWrite = true;
                            return;

                        case TnefAttributeTag.DateSent:
                            if (propertyTag.Id != TnefPropertyId.ClientSubmitTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (fabricatedOffset != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.SysTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            return;

                        case TnefAttributeTag.DateReceived:
                            if (propertyTag.Id != TnefPropertyId.MessageDeliveryTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (fabricatedOffset != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.SysTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            return;

                        case TnefAttributeTag.AttachCreateDate:
                            if (propertyTag.Id != TnefPropertyId.CreationTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (fabricatedOffset != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.SysTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            return;

                        case TnefAttributeTag.AttachModifyDate:
                            if (propertyTag.Id != TnefPropertyId.LastModificationTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (fabricatedOffset != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.SysTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            return;

                        case TnefAttributeTag.DateModified:
                            if (propertyTag.Id != TnefPropertyId.LastModificationTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                            if (fabricatedOffset != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.SysTime)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            return;
                    }
                }
            } else {
                switch (attributeTag) {
                    case TnefAttributeTag.Owner:
                        if (propertyTag.Id == TnefPropertyId.SentRepresentingEntryId) {
                            if (entryIdOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.Binary)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            entryIdOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.SentRepresentingName) {
                            if (nameOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.SentRepresentingNameA;
                            }
                            nameOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.SentRepresentingAddrtype) {
                            if (addrTypeOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.SentRepresentingAddrtypeA;
                            }
                            addrTypeOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.SentRepresentingEmailAddress) {
                            if (addressOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.SentRepresentingEmailAddressA;
                            }
                            addressOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.RcvdRepresentingEntryId) {
                            if (entryIdOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.Binary)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            entryIdOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.RcvdRepresentingName) {
                            if (nameOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.RcvdRepresentingNameA;
                            }
                            nameOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.RcvdRepresentingAddrtype) {
                            if (addrTypeOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.RcvdRepresentingAddrtypeA;
                            }
                            addrTypeOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id != TnefPropertyId.RcvdRepresentingEmailAddress)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (addressOffset != -1)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                            string8AsUnicode = true;
                            propertyTag = TnefPropertyTag.RcvdRepresentingEmailAddressA;
                        }
                        addressOffset = fabricatedOffset;
                        return;

                    case TnefAttributeTag.SentFor:
                        if (propertyTag.Id == TnefPropertyId.SentRepresentingEntryId) {
                            if (entryIdOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.Binary)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            entryIdOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.SentRepresentingName) {
                            if (nameOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.SentRepresentingNameA;
                            }
                            nameOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.SentRepresentingAddrtype) {
                            if (addrTypeOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                                string8AsUnicode = true;
                                propertyTag = TnefPropertyTag.SentRepresentingAddrtypeA;
                            }
                            addrTypeOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id != TnefPropertyId.SentRepresentingEmailAddress)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (addressOffset != -1)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                            string8AsUnicode = true;
                            propertyTag = TnefPropertyTag.SentRepresentingEmailAddressA;
                        }
                        addressOffset = fabricatedOffset;
                        return;

                    case TnefAttributeTag.Delegate:
                        if (propertyTag.Id != TnefPropertyId.Delegation)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (attributeLength != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Binary)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        directWrite = true;
                        return;

                    case TnefAttributeTag.MessageStatus:
                        if (propertyTag.Id != TnefPropertyId.MessageFlags)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (fabricatedOffset != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Long)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        return;

                    case TnefAttributeTag.AidOwner:
                        if (propertyTag.Id != TnefPropertyId.OwnerApptId)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (fabricatedOffset != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Long)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        directWrite = true;
                        return;

                    case TnefAttributeTag.RequestResponse:
                        if (propertyTag.Id != TnefPropertyId.ResponseRequested)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (fabricatedOffset != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Boolean)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        directWrite = true;
                        return;

                    case TnefAttributeTag.Priority:
                        if (propertyTag.Id != TnefPropertyId.Importance)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (fabricatedOffset != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Long)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        return;

                    case TnefAttributeTag.AttachData:
                        if (propertyTag.Id == TnefPropertyId.AttachData) {
                            if (attributeLength != 0)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if ((propertyTag.TnefType != TnefPropertyType.Binary) && (propertyTag.TnefType != TnefPropertyType.Object))
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            directWrite = true;
                            return;
                        }
                        if (propertyTag.Id != TnefPropertyId.AttachTag)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (fabricatedOffset != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Binary)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        return;

                    case TnefAttributeTag.AttachMetaFile:
                        if (propertyTag.Id != TnefPropertyId.AttachRendering)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (attributeLength != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Binary)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        directWrite = true;
                        return;

                    case TnefAttributeTag.AttachTransportFilename:
                        if (propertyTag.Id != TnefPropertyId.AttachTransportName)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (attributeLength != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                            string8AsUnicode = true;
                            propertyTag = TnefPropertyTag.AttachTransportNameA;
                        }
                        directWrite = true;
                        return;

                    case TnefAttributeTag.AttachRenderData:
                        if (propertyTag.Id == TnefPropertyId.RenderingPosition) {
                            if (renderingPositionOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.Long)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            renderingPositionOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id == TnefPropertyId.AttachMethod) {
                            if (attachMethodOffset != -1)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                            if (propertyTag.TnefType != TnefPropertyType.Long)
                                throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                            attachMethodOffset = fabricatedOffset;
                            return;
                        }
                        if (propertyTag.Id != TnefPropertyId.AttachEncoding)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (attachEncodingOffset != -1)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if (propertyTag.TnefType != TnefPropertyType.Binary)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        attachEncodingOffset = fabricatedOffset;
                        return;

                    case TnefAttributeTag.OriginalMessageClass:
                        if (propertyTag.Id != TnefPropertyId.OrigMessageClass)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (fabricatedOffset != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                            string8AsUnicode = true;
                            propertyTag = TnefPropertyTag.OrigMessageClassA;
                        }
                        return;

                    case TnefAttributeTag.MessageClass:
                        if (propertyTag.Id != TnefPropertyId.MessageClass)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttribute);
                        if (fabricatedOffset != 0)
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce);
                        if ((propertyTag.TnefType != TnefPropertyType.String8) && (propertyTag.TnefType != TnefPropertyType.Unicode))
                            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedInvalidPropertyType);
                        if (propertyTag.TnefType == TnefPropertyType.Unicode) {
                            string8AsUnicode = true;
                            propertyTag = TnefPropertyTag.MessageClassA;
                        }
                        return;
                }
            }
            throw new NotSupportedException(CtsResources.TnefStrings.WriterNotSupportedCannotAddAnyPropertyToAttribute);
        }

        public void StartProperty(TnefPropertyTag tag) {
            this.AssertGoodToUse(true);
            this.PrepareToStartProperty(tag, false);
            if (!legacyAttribute) {
                this.WriteDword(tag);
                this.CompleteStartProperty();
            } else {
                this.StartLegacyAttributeProperty();
                writeState = WriteState.BeforePropertyValue;
            }
        }

        public void StartProperty(TnefPropertyTag tag, Guid propSetGuid, int nameId) {
            this.AssertGoodToUse(true);
            this.PrepareToStartProperty(tag, true);
            this.WriteDword(tag);
            this.WriteGuid(propSetGuid);
            this.WriteDword(0);
            this.WriteDword(nameId);
            this.CompleteStartProperty();
        }

        public void StartProperty(TnefPropertyTag tag, Guid propSetGuid, string name) {
            this.AssertGoodToUse(true);
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length > 0x27fe)
                throw new ArgumentException(CtsResources.TnefStrings.WriterPropertyNameEmptyOrTooLong, "name");
            this.PrepareToStartProperty(tag, true);
            this.WriteDword(tag);
            this.WriteGuid(propSetGuid);
            this.WriteDword(1);
            var streamOffset = this.StreamOffset;
            this.WriteDword(0);
            this.WriteUnicodeString(name, true);
            this.WriteWord(0);
            var num2 = (this.StreamOffset - streamOffset) - 4;
            if ((num2%4) != 0)
                this.WriteBinary(TnefCommon.Padding, 0, (4 - (num2%4))%4);
            this.ReWriteDword(streamOffset, num2);
            this.CompleteStartProperty();
        }

        public void StartPropertyValue() {
            this.AssertGoodToUse(true);
            if (writeState <= WriteState.BeforeProperty)
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperation);
            if (!propertyTag.IsMultiValued) {
                if (valueCount != 0)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationMoreThanOneValueForSingleValuedProperty);
            } else if (writeState >= WriteState.BeginPropertyValue)
                this.EndPropertyValue();
            if (!legacyAttribute &&
                (((propertyTag.ValueTnefType == TnefPropertyType.String8) || (propertyTag.ValueTnefType == TnefPropertyType.Unicode)) ||
                 ((propertyTag.ValueTnefType == TnefPropertyType.Binary) || (propertyTag.ValueTnefType == TnefPropertyType.Object)))) {
                valueLengthOffset = this.StreamOffset;
                this.WriteDword(0);
            }
            valueAsText = true;
            valueCount++;
            valueLength = 0;
            writeState = WriteState.BeginPropertyValue;
        }

        public void StartRow() {
            this.AssertGoodToUse(true);
            if (attributeTag != TnefAttributeTag.RecipientTable)
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationStartRowNotInRecipientTable);
            if (writeState != WriteState.BeforeRow) {
                if (writeState == WriteState.BeginAttribute) {
                    rowCountOffset = this.StreamOffset;
                    this.WriteDword(0);
                    rowCount = 0;
                    writeState = WriteState.BeforeRow;
                } else {
                    if (writeState == WriteState.BeforeAttribute)
                        throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperation);
                    this.EndRow();
                }
            }
            rowCount++;
            propertyCountOffset = this.StreamOffset;
            this.WriteDword(0);
            propertyCount = 0;
            writeState = WriteState.BeforeProperty;
        }

        private static double ToOADate(DateTime value) {
            return value.ToOADate();
        }

        public void WriteAllProperties(TnefPropertyReader reader) {
            while (reader.ReadNextProperty())
                this.WriteProperty(reader);
        }

        public void WriteAttribute(TnefReader reader) {
            int num;
            this.AssertGoodToUse(true);
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (reader.InputStream == null)
                throw new ObjectDisposedException("TnefReader");
            if (reader.ReadStateValue != TnefReader.ReadState.BeginAttribute)
                throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeAtTheBeginningOfAttribute);
            this.StartAttribute(reader.AttributeTag, reader.AttributeLevel);
            while ((num = reader.ReadAttributeRawValue(byteBuffer, 0, byteBuffer.Length)) != 0)
                this.WriteAttributeRawValue(byteBuffer, 0, num);
            this.EndAttribute();
        }

        public void WriteAttributeRawValue(byte[] buffer, int offset, int count) {
            this.AssertGoodToUse(true);
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if ((offset > buffer.Length) || (offset < 0))
                throw new ArgumentOutOfRangeException("offset", CtsResources.TnefStrings.OffsetOutOfRange);
            if ((count > buffer.Length) || (count < 0))
                throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountOutOfRange);
            if ((count + offset) > buffer.Length)
                throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountTooLarge);
            if (writeState == WriteState.BeginAttribute)
                writeState = WriteState.WriteAttributeValue;
            if (writeState != WriteState.WriteAttributeValue)
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperation);
            writeState = WriteState.WriteAttributeValue;
            this.WriteBinary(buffer, offset, count);
        }

        private void WriteBinary(byte[] buffer, int offset, int count) {
            if (!directWrite) {
                if (hexEncodeBinary)
                    this.HexEncodeBinary(buffer, offset, count);
                else {
                    this.EnsureFabricatedSpace(count);
                    Buffer.BlockCopy(buffer, offset, fabricatedBuffer, fabricatedOffset, count);
                    fabricatedOffset += count;
                    valueLength += count;
                }
            } else {
                this.Checksum(buffer, offset, count);
                valueLength += count;
                attributeLength += count;
                if ((writeOffset + count) > writeBuffer.Length) {
                    this.FlushBuffer();
                    this.DirectWrite(buffer, offset, count);
                    streamOffset += count;
                } else {
                    Buffer.BlockCopy(buffer, offset, writeBuffer, writeOffset, count);
                    writeOffset += count;
                }
            }
        }

        private void WriteByte(byte value) {
            if (!directWrite) {
                this.EnsureFabricatedSpace(1);
                fabricatedBuffer[fabricatedOffset++] = value;
                valueLength++;
            } else {
                this.EnsureSpace(1);
                writeBuffer[writeOffset++] = value;
                this.Checksum(writeBuffer, writeOffset - 1, 1);
                valueLength++;
                attributeLength++;
            }
        }

        private void WriteDouble(double value) {
            this.EnsureSpace(8);
            TnefBitConverter.GetBytes(writeBuffer, writeOffset, value);
            this.Checksum(writeBuffer, writeOffset, 8);
            writeOffset += 8;
            valueLength += 8;
            attributeLength += 8;
        }

        private void WriteDword(int value) {
            if (!directWrite) {
                this.EnsureFabricatedSpace(4);
                TnefBitConverter.GetBytes(fabricatedBuffer, fabricatedOffset, value);
                fabricatedOffset += 4;
                valueLength += 4;
            } else {
                this.EnsureSpace(4);
                TnefBitConverter.GetBytes(writeBuffer, writeOffset, value);
                this.Checksum(writeBuffer, writeOffset, 4);
                writeOffset += 4;
                valueLength += 4;
                attributeLength += 4;
            }
        }

        private void WriteFloat(float value) {
            this.EnsureSpace(4);
            TnefBitConverter.GetBytes(writeBuffer, writeOffset, value);
            this.Checksum(writeBuffer, writeOffset, 4);
            writeOffset += 4;
            valueLength += 4;
            attributeLength += 4;
        }

        private void WriteGuid(Guid value) {
            this.EnsureSpace(0x10);
            TnefBitConverter.GetBytes(writeBuffer, writeOffset, value);
            this.Checksum(writeBuffer, writeOffset, 0x10);
            writeOffset += 0x10;
            valueLength += 0x10;
            attributeLength += 0x10;
        }

        public void WriteOemCodePage(int messageCodePage) {
            this.AssertGoodToUse(true);
            this.SetMessageCodePage(messageCodePage);
            TnefBitConverter.GetBytes(byteBuffer, 0, this.MessageCodePage);
            TnefBitConverter.GetBytes(byteBuffer, 4, 0);
            this.StartAttribute(TnefAttributeTag.OemCodepage, TnefAttributeLevel.Message);
            this.WriteAttributeRawValue(byteBuffer, 0, 8);
            this.EndAttribute();
        }

        public void WriteProperty(TnefPropertyReader propertyReader) {
            int num;
            this.AssertGoodToUse(true);
            if (propertyReader.Reader == null)
                throw new ArgumentException("TnefPropertyReader object is not properly initialized", "propertyReader");
            var reader = propertyReader.Reader;
            if (reader.InputStream == null)
                throw new ObjectDisposedException("TnefReader");
            if ((reader.ReadStateValue != TnefReader.ReadState.BeginProperty) && ((reader.ReadStateValue != TnefReader.ReadState.BeginPropertyValue) || reader.PropertyTag.IsMultiValued))
                throw new InvalidOperationException(CtsResources.TnefStrings.ReaderInvalidOperationMustBeAtTheBeginningOfProperty);
            if (!reader.PropertyTag.IsNamed)
                this.StartProperty(reader.PropertyTag);
            else if (reader.PropertyNameId.Kind == TnefNameIdKind.Id)
                this.StartProperty(reader.PropertyTag, reader.PropertyNameId.PropertySetGuid, reader.PropertyNameId.Id);
            else
                this.StartProperty(reader.PropertyTag, reader.PropertyNameId.PropertySetGuid, reader.PropertyNameId.Name);
            if (reader.PropertyTag.IsMultiValued) {
                while (reader.ReadNextPropertyValue()) {
                    this.StartPropertyValue();
                    while ((num = reader.ReadPropertyRawValue(byteBuffer, 0, byteBuffer.Length, false)) != 0)
                        this.WritePropertyRawValue(byteBuffer, 0, num);
                }
            } else if (reader.PropertyTag.TnefType != TnefPropertyType.Null) {
                this.StartPropertyValue();
                if (reader.PropertyTag.TnefType == TnefPropertyType.Object)
                    this.WriteGuid(reader.PropertyValueOleIID);
                while ((num = reader.ReadPropertyRawValue(byteBuffer, 0, byteBuffer.Length, false)) != 0)
                    this.WritePropertyRawValue(byteBuffer, 0, num);
            }
            this.EndProperty();
        }

        public void WriteProperty(TnefPropertyTag tag, bool value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, DateTime value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, double value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, Guid value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, short value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, int value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, long value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, float value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, string value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, byte[] value) {
            this.StartProperty(tag);
            this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, System.IO.Stream stream) {
            this.StartProperty(tag);
            this.WritePropertyValue(stream);
        }

        public void WriteProperty(TnefPropertyTag tag, System.IO.TextReader reader) {
            this.StartProperty(tag);
            this.WritePropertyValue(reader);
        }

        public void WriteProperty(TnefPropertyTag tag, object value) {
            this.StartProperty(tag);
            if ((tag.TnefType != TnefPropertyType.Null) || (value != null))
                this.WritePropertyValue(value);
        }

        public void WriteProperty(TnefPropertyTag tag, Guid guid, System.IO.Stream stream) {
            this.StartProperty(tag);
            this.WritePropertyValue(guid, stream);
        }

        public void WritePropertyRawValue(byte[] buffer, int offset, int count) {
            this.WritePropertyRawValueImpl(buffer, offset, count, false);
        }

        internal void WritePropertyRawValueImpl(byte[] buffer, int offset, int count, bool fromWrapper) {
            this.AssertGoodToUse(!fromWrapper);
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if ((offset > buffer.Length) || (offset < 0))
                throw new ArgumentOutOfRangeException("offset", CtsResources.TnefStrings.OffsetOutOfRange);
            if ((count > buffer.Length) || (count < 0))
                throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountOutOfRange);
            if ((count + offset) > buffer.Length)
                throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountTooLarge);
            if (writeState < WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if ((valueFixedLength != 0) && ((valueLength + count) > valueFixedLength))
                throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationValueTooLongForType);
            if (legacyAttribute && string8AsUnicode)
                throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationUnicodeRawValueForLegacyAttribute);
            if (writeState == WriteState.BeginPropertyValue)
                valueAsText = false;
            if (valueAsText)
                throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationRawDataAfterText);
            this.WriteBinary(buffer, offset, count);
            writeState = WriteState.WritePropertyValue;
        }

        public void WritePropertyTextValue(char[] buffer, int offset, int count) {
            this.AssertGoodToUse(true);
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if ((offset > buffer.Length) || (offset < 0))
                throw new ArgumentOutOfRangeException("offset", CtsResources.TnefStrings.OffsetOutOfRange);
            if ((count > buffer.Length) || (count < 0))
                throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountOutOfRange);
            if ((count + offset) > buffer.Length)
                throw new ArgumentOutOfRangeException("count", CtsResources.TnefStrings.CountTooLarge);
            if (writeState < WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (writeState == WriteState.BeginPropertyValue)
                valueAsText = true;
            if (!valueAsText)
                throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationTextAfterRawData);
            if (propertyTag.ValueTnefType == TnefPropertyType.String8)
                this.WriteString8Text(buffer, offset, count, false);
            else {
                if (propertyTag.ValueTnefType != TnefPropertyType.Unicode)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
                this.WriteUnicodeText(buffer, offset, count, false);
            }
            writeState = WriteState.WritePropertyValue;
        }

        public void WritePropertyValue(bool value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (propertyTag.ValueTnefType != TnefPropertyType.Boolean)
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
            this.WriteWord(value ? ((short) 1) : ((short) 0));
            this.EndPropertyValue();
        }

        public void WritePropertyValue(DateTime value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (propertyTag.ValueTnefType == TnefPropertyType.SysTime)
                this.WriteQword((value == DateTime.MinValue) ? 0L : value.ToFileTimeUtc());
            else {
                if (propertyTag.ValueTnefType != TnefPropertyType.AppTime)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
                this.WriteDouble((value == DateTime.MinValue) ? 0.0 : TnefWriter.ToOADate(value));
            }
            this.EndPropertyValue();
        }

        public void WritePropertyValue(double value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if ((propertyTag.ValueTnefType != TnefPropertyType.Double) && (propertyTag.ValueTnefType != TnefPropertyType.AppTime))
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
            this.WriteDouble(value);
            this.EndPropertyValue();
        }

        public void WritePropertyValue(Guid value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if ((propertyTag.ValueTnefType != TnefPropertyType.ClassId) && (propertyTag.ValueTnefType != TnefPropertyType.Object))
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
            this.WriteGuid(value);
            if (propertyTag.ValueTnefType == TnefPropertyType.Object) {
                writeState = WriteState.WritePropertyValue;
                valueAsText = false;
            } else
                this.EndPropertyValue();
        }

        public void WritePropertyValue(short value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if ((propertyTag.ValueTnefType == TnefPropertyType.I2) || (propertyTag.ValueTnefType == TnefPropertyType.Boolean))
                this.WriteWord(value);
            else if (propertyTag.ValueTnefType == TnefPropertyType.Long)
                this.WriteDword(value);
            else {
                if (propertyTag.ValueTnefType != TnefPropertyType.I8)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
                this.WriteQword(value);
            }
            this.EndPropertyValue();
        }

        public void WritePropertyValue(int value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if ((propertyTag.ValueTnefType == TnefPropertyType.Long) || (propertyTag.ValueTnefType == TnefPropertyType.Error))
                this.WriteDword(value);
            else if (((propertyTag.ValueTnefType == TnefPropertyType.Boolean) || (propertyTag.ValueTnefType == TnefPropertyType.I2)) && !propertyTag.IsMultiValued) {
                if (!legacyAttribute) {
                    valueFixedLength = 4;
                    this.WriteDword(value);
                } else
                    this.WriteWord((short) value);
            } else {
                if (propertyTag.ValueTnefType != TnefPropertyType.I8)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
                this.WriteQword(value);
            }
            this.EndPropertyValue();
        }

        public void WritePropertyValue(long value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (((propertyTag.ValueTnefType != TnefPropertyType.I8) && (propertyTag.ValueTnefType != TnefPropertyType.Currency)) && (propertyTag.ValueTnefType != TnefPropertyType.SysTime))
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
            this.WriteQword(value);
            this.EndPropertyValue();
        }

        public void WritePropertyValue(System.IO.Stream stream) {
            this.AssertGoodToUse(true);
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if ((propertyTag.ValueTnefType != TnefPropertyType.Binary) && (propertyTag.ValueTnefType != TnefPropertyType.Object))
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
            this.CopyStreamContentAsRawValue(stream);
            this.EndPropertyValue();
        }

        public void WritePropertyValue(System.IO.TextReader reader) {
            this.AssertGoodToUse(true);
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (propertyTag.ValueTnefType == TnefPropertyType.String8) {
                this.WriteString8TextReader(reader, true);
                this.WriteByte(0);
            } else {
                if (propertyTag.ValueTnefType != TnefPropertyType.Unicode)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationNotStringProperty);
                this.WriteUnicodeTextReader(reader, true);
                this.WriteWord(0);
            }
            valueAsText = false;
            this.EndPropertyValue();
        }

        public void WritePropertyValue(object value) {
            this.AssertGoodToUse(true);
            if (value == null)
                throw new ArgumentNullException("object");
            var type = value.GetType();
            switch (TnefWriter.GetTypeCode(value)) {
                case TypeCode.Boolean:
                    this.WritePropertyValue((bool) value);
                    return;

                case TypeCode.Char:
                    throw new NotSupportedException("Char type is not supported");

                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                    this.WritePropertyValue((short) value);
                    return;

                case TypeCode.UInt16:
                    this.WritePropertyValue((short) ((ushort) value));
                    return;

                case TypeCode.Int32:
                    this.WritePropertyValue((int) value);
                    return;

                case TypeCode.UInt32:
                    this.WritePropertyValue((int) ((uint) value));
                    return;

                case TypeCode.Int64:
                    this.WritePropertyValue((long) value);
                    return;

                case TypeCode.UInt64:
                    this.WritePropertyValue((long) ((ulong) value));
                    return;

                case TypeCode.Single:
                    this.WritePropertyValue((float) value);
                    return;

                case TypeCode.Double:
                    this.WritePropertyValue((double) value);
                    return;

                case TypeCode.Decimal:
                    throw new NotSupportedException("Decimal type is not supported");

                case TypeCode.DateTime:
                    this.WritePropertyValue((DateTime) value);
                    return;

                case TypeCode.String:
                    this.WritePropertyValue((string) value);
                    return;
            }
            if (type == typeof (Guid))
                this.WritePropertyValue((Guid) value);
            else if (type == typeof (byte[]))
                this.WritePropertyValue((byte[]) value);
            else if (type.IsEnum) {
                if (typeof (short).IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(type)))
                    this.WritePropertyValue((short) value);
                else if (typeof (int).IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(type)))
                    this.WritePropertyValue((int) value);
                else {
                    if (!typeof (long).IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(type)))
                        throw new NotSupportedException("enum is not supported");
                    this.WritePropertyValue((long) value);
                }
            } else {
                var reader = value as System.IO.TextReader;
                if (reader != null)
                    this.WritePropertyValue(reader);
                else {
                    var stream = value as System.IO.Stream;
                    if (stream == null)
                        throw new NotSupportedException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
                    this.WritePropertyValue(stream);
                }
            }
        }

        public void WritePropertyValue(float value) {
            this.AssertGoodToUse(true);
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (propertyTag.ValueTnefType == TnefPropertyType.R4)
                this.WriteFloat(value);
            else {
                if (propertyTag.ValueTnefType != TnefPropertyType.Double)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
                this.WriteDouble(value);
            }
            this.EndPropertyValue();
        }

        public void WritePropertyValue(string value) {
            this.AssertGoodToUse(true);
            if (value == null)
                throw new ArgumentNullException("value");
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (propertyTag.ValueTnefType == TnefPropertyType.String8) {
                this.WriteString8String(value, true);
                this.WriteByte(0);
            } else {
                if (propertyTag.ValueTnefType != TnefPropertyType.Unicode)
                    throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationInvalidValueType);
                this.WriteUnicodeString(value, true);
                this.WriteWord(0);
            }
            valueAsText = false;
            this.EndPropertyValue();
        }

        public void WritePropertyValue(byte[] value) {
            this.AssertGoodToUse(true);
            if (value == null)
                throw new ArgumentNullException("value");
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            this.WritePropertyRawValue(value, 0, value.Length);
            this.EndPropertyValue();
        }

        public void WritePropertyValue(Guid iid, System.IO.Stream stream) {
            this.AssertGoodToUse(true);
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (writeState != WriteState.BeginPropertyValue)
                this.StartPropertyValue();
            if (propertyTag.ValueTnefType != TnefPropertyType.Object)
                throw new InvalidOperationException(CtsResources.TnefStrings.WriterInvalidOperationNotObjectProperty);
            this.WriteGuid(iid);
            this.CopyStreamContentAsRawValue(stream);
            this.EndPropertyValue();
        }

        private void WriteQword(long value) {
            if (!directWrite) {
                this.EnsureFabricatedSpace(8);
                TnefBitConverter.GetBytes(fabricatedBuffer, fabricatedOffset, value);
                fabricatedOffset += 8;
                valueLength += 8;
            } else {
                this.EnsureSpace(8);
                TnefBitConverter.GetBytes(writeBuffer, writeOffset, value);
                this.Checksum(writeBuffer, writeOffset, 8);
                writeOffset += 8;
                valueLength += 8;
                attributeLength += 8;
            }
        }

        private void WriteString8String(string value, bool flush) {
            var sourceIndex = 0;
            while (sourceIndex != value.Length) {
                var count = Math.Min(charBuffer.Length, value.Length - sourceIndex);
                value.CopyTo(sourceIndex, charBuffer, 0, count);
                sourceIndex += count;
                this.WriteString8Text(charBuffer, 0, count, flush && (sourceIndex == value.Length));
            }
        }

        private void WriteString8Text(char[] buffer, int offset, int count, bool flush) {
            int num;
            int num2;
            var completed = false;
            var num3 = 0;
            if (directWrite) {
                while ((count != 0) || (flush && !completed)) {
                    this.EnsureSpace(0x80);
                    string8Encoder.Convert(buffer, offset, count, writeBuffer, writeOffset, writeBuffer.Length - writeOffset, flush, out num, out num2, out completed);
                    this.Checksum(writeBuffer, writeOffset, num2);
                    writeOffset += num2;
                    num3 += num2;
                    offset += num;
                    count -= num;
                }
                valueLength += num3;
                attributeLength += num3;
            } else {
                while ((count != 0) || (flush && !completed)) {
                    this.EnsureFabricatedSpace(0x10);
                    string8Encoder.Convert(buffer, offset, count, fabricatedBuffer, fabricatedOffset, fabricatedBuffer.Length - fabricatedOffset, flush, out num, out num2, out completed);
                    fabricatedOffset += num2;
                    num3 += num2;
                    offset += num;
                    count -= num;
                }
                valueLength += num3;
            }
        }

        private void WriteString8TextReader(System.IO.TextReader reader, bool flush) {
            int num;
            while ((num = reader.Read(charBuffer, 0, charBuffer.Length)) != 0)
                this.WriteString8Text(charBuffer, 0, num, false);
            if (flush)
                this.WriteString8Text(charBuffer, 0, 0, true);
        }

        public void WriteTnefVersion() {
            this.AssertGoodToUse(true);
            TnefBitConverter.GetBytes(byteBuffer, 0, 0x10000);
            this.StartAttribute(TnefAttributeTag.TnefVersion, TnefAttributeLevel.Message);
            this.WriteAttributeRawValue(byteBuffer, 0, 4);
            this.EndAttribute();
        }

        private void WriteUnicodeString(string value, bool flush) {
            var sourceIndex = 0;
            while (sourceIndex != value.Length) {
                var count = Math.Min(charBuffer.Length, value.Length - sourceIndex);
                value.CopyTo(sourceIndex, charBuffer, 0, count);
                sourceIndex += count;
                this.WriteUnicodeText(charBuffer, 0, count, flush && (sourceIndex == value.Length));
            }
        }

        private void WriteUnicodeText(char[] buffer, int offset, int count, bool flush) {
            var completed = false;
            var num3 = 0;
            while ((count != 0) || (flush && !completed)) {
                int num;
                int num2;
                this.EnsureSpace(0x80);
                unicodeEncoder.Convert(buffer, offset, count, writeBuffer, writeOffset, writeBuffer.Length - writeOffset, flush, out num, out num2, out completed);
                this.Checksum(writeBuffer, writeOffset, num2);
                writeOffset += num2;
                num3 += num2;
                offset += num;
                count -= num;
            }
            valueLength += num3;
            attributeLength += num3;
        }

        private void WriteUnicodeTextReader(System.IO.TextReader reader, bool flush) {
            int num;
            while ((num = reader.Read(charBuffer, 0, charBuffer.Length)) != 0)
                this.WriteUnicodeText(charBuffer, 0, num, false);
            if (flush)
                this.WriteUnicodeText(charBuffer, 0, 0, true);
        }

        private void WriteWord(short value) {
            if (!directWrite) {
                this.EnsureFabricatedSpace(2);
                TnefBitConverter.GetBytes(fabricatedBuffer, fabricatedOffset, value);
                fabricatedOffset += 2;
                valueLength += 2;
            } else {
                this.EnsureSpace(2);
                TnefBitConverter.GetBytes(writeBuffer, writeOffset, value);
                this.Checksum(writeBuffer, writeOffset, 2);
                writeOffset += 2;
                valueLength += 2;
                attributeLength += 2;
            }
        }

        private const int WriteBufferSize = 0x1000;
        private readonly short attachmentKey;
        private readonly TnefWriterFlags flags;
        private readonly TnefWriter parent;
        private int addressOffset;
        private int addrTypeOffset;
        private int attachEncodingOffset;
        private int attachMethodOffset;
        private int attributeLength;
        private int attributeLengthOffset;
        private TnefAttributeLevel attributeLevel;
        private ushort attributeStartChecksum;
        private TnefAttributeTag attributeTag;
        private byte[] byteBuffer;
        private bool canFlush;
        private char[] charBuffer;
        private ushort checksum;
        internal object Child;
        private bool directWrite;
        private bool disposing;
        private int entryIdOffset;
        private byte[] fabricatedBuffer;
        private int fabricatedOffset;
        private bool hexEncodeBinary;
        private bool legacyAttribute;
        private int nameOffset;
        private System.IO.Stream outputStream;
        private int propertyCount;
        private int propertyCountOffset;
        private TnefPropertyTag propertyTag;
        private int renderingPositionOffset;
        private int rowCount;
        private int rowCountOffset;
        private int streamOffset;
        private bool string8AsUnicode;
        private System.Text.Encoder string8Encoder;
        private int totalLength;
        private System.Text.Encoder unicodeEncoder;
        private bool valueAsText;
        private int valueCount;
        private int valueCountOffset;
        private int valueFixedLength;
        private int valueLength;
        private int valueLengthOffset;
        private byte[] writeBuffer;
        private int writeOffset;
        private WriteState writeState;
        private System.IO.Stream writeStream;
        private int writeStreamOffset;


        private enum WriteState {

            BeforeAttribute,
            BeginAttribute,
            WriteAttributeValue,
            BeforeRow,
            BeforeProperty,
            BeforePropertyValue,
            BeginPropertyValue,
            WritePropertyValue

        }

    }

}