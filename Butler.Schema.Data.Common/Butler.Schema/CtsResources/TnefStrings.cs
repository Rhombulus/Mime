// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.CtsResources.TnefStrings
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Butler.Schema.CtsResources
{
  internal static class TnefStrings
  {
    private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(92);
    private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.TnefStrings", ((Type) IntrospectionExtensions.GetTypeInfo(typeof (TnefStrings))).Assembly);

    public static string WriterPropertyNameEmptyOrTooLong
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterPropertyNameEmptyOrTooLong");
      }
    }

    public static string ReaderComplianceInvalidMessageClassNotZeroTerminated
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidMessageClassNotZeroTerminated");
      }
    }

    public static string ReaderInvalidOperationReadNextRowOnlyInRecipientTable
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationReadNextRowOnlyInRecipientTable");
      }
    }

    public static string ReaderInvalidOperationCannotCloseParentWhileChildOpen
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationCannotCloseParentWhileChildOpen");
      }
    }

    public static string ReaderComplianceInvalidAttributeChecksum
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidAttributeChecksum");
      }
    }

    public static string WriterInvalidOperationTextAfterRawData
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationTextAfterRawData");
      }
    }

    public static string WriterInvalidOperationStartRowNotInRecipientTable
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationStartRowNotInRecipientTable");
      }
    }

    public static string CountTooLarge
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("CountTooLarge");
      }
    }

    public static string WriterInvalidOperationInvalidPropertyType
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationInvalidPropertyType");
      }
    }

    public static string ReaderInvalidOperationMustBeInARow
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationMustBeInARow");
      }
    }

    public static string ReaderComplianceInvalidTnefVersion
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidTnefVersion");
      }
    }

    public static string StreamDoesNotSupportSeek
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("StreamDoesNotSupportSeek");
      }
    }

    public static string ReaderInvalidOperationMustBeAtTheBeginningOfProperty
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationMustBeAtTheBeginningOfProperty");
      }
    }

    public static string ReaderComplianceInvalidPropertyTypeObjectInRecipientTable
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyTypeObjectInRecipientTable");
      }
    }

    public static string WriterInvalidOperationUnicodeRawValueForLegacyAttribute
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationUnicodeRawValueForLegacyAttribute");
      }
    }

    public static string ReaderComplianceInvalidPropertyValueDate
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyValueDate");
      }
    }

    public static string ReaderComplianceInvalidPropertyValueCount
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyValueCount");
      }
    }

    public static string ReaderInvalidOperationCannotConvertValue
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationCannotConvertValue");
      }
    }

    public static string ReaderComplianceInvalidAttributeLength
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidAttributeLength");
      }
    }

    public static string ReaderComplianceTooDeepEmbedding
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceTooDeepEmbedding");
      }
    }

    public static string ReaderInvalidOperationTextPropertyTooLong
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationTextPropertyTooLong");
      }
    }

    public static string WriterInvalidOperationNotObjectProperty
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationNotObjectProperty");
      }
    }

    public static string ReaderComplianceInvalidFrom
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidFrom");
      }
    }

    public static string WriterInvalidOperationInvalidValueType
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationInvalidValueType");
      }
    }

    public static string ReaderComplianceInvalidNamedPropertyNameLength
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidNamedPropertyNameLength");
      }
    }

    public static string WriterInvalidOperationObjectInRecipientTable
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationObjectInRecipientTable");
      }
    }

    public static string ReaderComplianceInvalidPropertyTypeMvBoolean
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyTypeMvBoolean");
      }
    }

    public static string WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce");
      }
    }

    public static string ReaderComplianceInvalidPropertyCount
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyCount");
      }
    }

    public static string WriterNotSupportedNotOneOffEntryId
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedNotOneOffEntryId");
      }
    }

    public static string ReaderInvalidOperationRawAfterProp
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationRawAfterProp");
      }
    }

    public static string ReaderComplianceInvalidPropertyLengthObject
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyLengthObject");
      }
    }

    public static string WriterInvalidMessageCodepage
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidMessageCodepage");
      }
    }

    public static string ReaderComplianceInvalidTnefSignature
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidTnefSignature");
      }
    }

    public static string StreamDoesNotSupportRead
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("StreamDoesNotSupportRead");
      }
    }

    public static string ReaderInvalidOperationNotObjectProperty
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationNotObjectProperty");
      }
    }

    public static string StreamDoesNotSupportWrite
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("StreamDoesNotSupportWrite");
      }
    }

    public static string WriterNotSupportedInvalidRecipientInformation
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedInvalidRecipientInformation");
      }
    }

    public static string ReaderComplianceInvalidPropertyType
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyType");
      }
    }

    public static string CountOutOfRange
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("CountOutOfRange");
      }
    }

    public static string WriterInvalidOperationMoreThanOneValueForSingleValuedProperty
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationMoreThanOneValueForSingleValuedProperty");
      }
    }

    public static string ReaderComplianceInvalidPropertyTypeError
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyTypeError");
      }
    }

    public static string ReaderComplianceInvalidSchedulePlus
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidSchedulePlus");
      }
    }

    public static string WriterInvalidOperationMvObject
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationMvObject");
      }
    }

    public static string ReaderComplianceInvalidMessageCodepage
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidMessageCodepage");
      }
    }

    public static string ReaderComplianceInvalidOemCodepageAttributeLength
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidOemCodepageAttributeLength");
      }
    }

    public static string ReaderComplianceInvalidConversationId
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidConversationId");
      }
    }

    public static string ReaderComplianceInvalidRowCount
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidRowCount");
      }
    }

    public static string WriterInvalidOperationNotStringProperty
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationNotStringProperty");
      }
    }

    public static string ReaderInvalidOperationMustBeAtTheBeginningOfAttribute
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationMustBeAtTheBeginningOfAttribute");
      }
    }

    public static string ReaderComplianceInvalidPropertyLength
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyLength");
      }
    }

    public static string ReaderComplianceInvalidDateOrTimeValue
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidDateOrTimeValue");
      }
    }

    public static string ReaderComplianceInvalidComputedPropertyLength
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidComputedPropertyLength");
      }
    }

    public static string WriterInvalidOperationStartNormalPropertyWithName
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationStartNormalPropertyWithName");
      }
    }

    public static string ReaderInvalidOperationMustBeInAttribute
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationMustBeInAttribute");
      }
    }

    public static string ReaderComplianceInvalidNamedPropertyNameNotZeroTerminated
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidNamedPropertyNameNotZeroTerminated");
      }
    }

    public static string WriterInvalidOperationValueSizeInvalidForType
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationValueSizeInvalidForType");
      }
    }

    public static string WriterInvalidOperation
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperation");
      }
    }

    public static string WriterNotSupportedCannotAddAnyPropertyToAttribute
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedCannotAddAnyPropertyToAttribute");
      }
    }

    public static string IndexOutOfRange
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("IndexOutOfRange");
      }
    }

    public static string ReaderInvalidOperationNotNamedProperty
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationNotNamedProperty");
      }
    }

    public static string ReaderComplianceAttributeValueOverflow
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceAttributeValueOverflow");
      }
    }

    public static string ReaderInvalidOperationPropAfterRaw
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationPropAfterRaw");
      }
    }

    public static string WriterNotSupportedCannotAddThisPropertyToAttribute
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedCannotAddThisPropertyToAttribute");
      }
    }

    public static string WriterNotSupportedInvalidPropertyType
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedInvalidPropertyType");
      }
    }

    public static string ReaderInvalidOperationPropRawAfterText
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationPropRawAfterText");
      }
    }

    public static string ReaderInvalidOperationNotSeekableCannotUseRewind
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationNotSeekableCannotUseRewind");
      }
    }

    public static string ReaderComplianceInvalidPropertyTypeMvObject
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidPropertyTypeMvObject");
      }
    }

    public static string ReaderComplianceInvalidAttributeLevel
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidAttributeLevel");
      }
    }

    public static string WriterNotSupportedUnicodeOneOffEntryId
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedUnicodeOneOffEntryId");
      }
    }

    public static string ReaderInvalidOperationStreamOffsetForAComputedValue
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationStreamOffsetForAComputedValue");
      }
    }

    public static string ReaderInvalidOperationRowsOnlyInRecipientTable
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationRowsOnlyInRecipientTable");
      }
    }

    public static string ReaderInvalidOperationPropTextAfterRaw
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationPropTextAfterRaw");
      }
    }

    public static string WriterNotSupportedLegacyAttributeTooLong
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedLegacyAttributeTooLong");
      }
    }

    public static string ReaderInvalidOperationPropertyRawValueTooLong
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationPropertyRawValueTooLong");
      }
    }

    public static string WriterNotSupportedMallformedEntryId
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedMallformedEntryId");
      }
    }

    public static string WriterInvalidOperationRawDataAfterText
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationRawDataAfterText");
      }
    }

    public static string ReaderInvalidOperationMustBeInPropertyValue
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationMustBeInPropertyValue");
      }
    }

    public static string ReaderInvalidOperationChildActive
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationChildActive");
      }
    }

    public static string ReaderComplianceInvalidTnefVersionAttributeLength
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidTnefVersionAttributeLength");
      }
    }

    public static string ReaderInvalidOperationNotEmbeddedMessage
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationNotEmbeddedMessage");
      }
    }

    public static string WriterInvalidOperationStartNamedPropertyNoName
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationStartNamedPropertyNoName");
      }
    }

    public static string WriterInvalidOperationChildActive
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationChildActive");
      }
    }

    public static string InvalidMessageCodePage
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("InvalidMessageCodePage");
      }
    }

    public static string WriterInvalidOperationNamedPropertyInLegacyAttribute
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationNamedPropertyInLegacyAttribute");
      }
    }

    public static string ReaderInvalidOperationMustBeInProperty
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderInvalidOperationMustBeInProperty");
      }
    }

    public static string OffsetOutOfRange
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("OffsetOutOfRange");
      }
    }

    public static string ReaderComplianceInvalidMessageClassLength
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceInvalidMessageClassLength");
      }
    }

    public static string WriterInvalidOperationValueTooLongForType
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterInvalidOperationValueTooLongForType");
      }
    }

    public static string WriterNotSupportedInvalidMessageClass
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedInvalidMessageClass");
      }
    }

    public static string WriterNotSupportedNotEnoughInformationForAttribute
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("WriterNotSupportedNotEnoughInformationForAttribute");
      }
    }

    public static string ReaderComplianceTnefTruncated
    {
      get
      {
        return TnefStrings.ResourceManager.GetString("ReaderComplianceTnefTruncated");
      }
    }

    static TnefStrings()
    {
      TnefStrings.stringIDs.Add(149645149U, "WriterPropertyNameEmptyOrTooLong");
      TnefStrings.stringIDs.Add(2720608910U, "ReaderComplianceInvalidMessageClassNotZeroTerminated");
      TnefStrings.stringIDs.Add(718178560U, "ReaderInvalidOperationReadNextRowOnlyInRecipientTable");
      TnefStrings.stringIDs.Add(2814570743U, "ReaderInvalidOperationCannotCloseParentWhileChildOpen");
      TnefStrings.stringIDs.Add(2548454096U, "ReaderComplianceInvalidAttributeChecksum");
      TnefStrings.stringIDs.Add(2472716582U, "WriterInvalidOperationTextAfterRawData");
      TnefStrings.stringIDs.Add(2826628516U, "WriterInvalidOperationStartRowNotInRecipientTable");
      TnefStrings.stringIDs.Add(2746482960U, "CountTooLarge");
      TnefStrings.stringIDs.Add(3854182917U, "WriterInvalidOperationInvalidPropertyType");
      TnefStrings.stringIDs.Add(3999265701U, "ReaderInvalidOperationMustBeInARow");
      TnefStrings.stringIDs.Add(116892624U, "ReaderComplianceInvalidTnefVersion");
      TnefStrings.stringIDs.Add(3194581355U, "StreamDoesNotSupportSeek");
      TnefStrings.stringIDs.Add(2802240034U, "ReaderInvalidOperationMustBeAtTheBeginningOfProperty");
      TnefStrings.stringIDs.Add(2125705981U, "ReaderComplianceInvalidPropertyTypeObjectInRecipientTable");
      TnefStrings.stringIDs.Add(3713767861U, "WriterInvalidOperationUnicodeRawValueForLegacyAttribute");
      TnefStrings.stringIDs.Add(3183309601U, "ReaderComplianceInvalidPropertyValueDate");
      TnefStrings.stringIDs.Add(2193753292U, "ReaderComplianceInvalidPropertyValueCount");
      TnefStrings.stringIDs.Add(1030605270U, "ReaderInvalidOperationCannotConvertValue");
      TnefStrings.stringIDs.Add(45144793U, "ReaderComplianceInvalidAttributeLength");
      TnefStrings.stringIDs.Add(4260198087U, "ReaderComplianceTooDeepEmbedding");
      TnefStrings.stringIDs.Add(1889035737U, "ReaderInvalidOperationTextPropertyTooLong");
      TnefStrings.stringIDs.Add(585424572U, "WriterInvalidOperationNotObjectProperty");
      TnefStrings.stringIDs.Add(1579220719U, "ReaderComplianceInvalidFrom");
      TnefStrings.stringIDs.Add(340503501U, "WriterInvalidOperationInvalidValueType");
      TnefStrings.stringIDs.Add(1982026044U, "ReaderComplianceInvalidNamedPropertyNameLength");
      TnefStrings.stringIDs.Add(8456632U, "WriterInvalidOperationObjectInRecipientTable");
      TnefStrings.stringIDs.Add(138477889U, "ReaderComplianceInvalidPropertyTypeMvBoolean");
      TnefStrings.stringIDs.Add(2301033583U, "WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce");
      TnefStrings.stringIDs.Add(1676581361U, "ReaderComplianceInvalidPropertyCount");
      TnefStrings.stringIDs.Add(32688031U, "WriterNotSupportedNotOneOffEntryId");
      TnefStrings.stringIDs.Add(3093568444U, "ReaderInvalidOperationRawAfterProp");
      TnefStrings.stringIDs.Add(3871797491U, "ReaderComplianceInvalidPropertyLengthObject");
      TnefStrings.stringIDs.Add(354067927U, "WriterInvalidMessageCodepage");
      TnefStrings.stringIDs.Add(3347133450U, "ReaderComplianceInvalidTnefSignature");
      TnefStrings.stringIDs.Add(1984727737U, "StreamDoesNotSupportRead");
      TnefStrings.stringIDs.Add(3184266628U, "ReaderInvalidOperationNotObjectProperty");
      TnefStrings.stringIDs.Add(3282557718U, "StreamDoesNotSupportWrite");
      TnefStrings.stringIDs.Add(560728928U, "WriterNotSupportedInvalidRecipientInformation");
      TnefStrings.stringIDs.Add(1718248948U, "ReaderComplianceInvalidPropertyType");
      TnefStrings.stringIDs.Add(1590522975U, "CountOutOfRange");
      TnefStrings.stringIDs.Add(251475407U, "WriterInvalidOperationMoreThanOneValueForSingleValuedProperty");
      TnefStrings.stringIDs.Add(801076374U, "ReaderComplianceInvalidPropertyTypeError");
      TnefStrings.stringIDs.Add(2890237940U, "ReaderComplianceInvalidSchedulePlus");
      TnefStrings.stringIDs.Add(2874206193U, "WriterInvalidOperationMvObject");
      TnefStrings.stringIDs.Add(18124002U, "ReaderComplianceInvalidMessageCodepage");
      TnefStrings.stringIDs.Add(278456090U, "ReaderComplianceInvalidOemCodepageAttributeLength");
      TnefStrings.stringIDs.Add(4267063105U, "ReaderComplianceInvalidConversationId");
      TnefStrings.stringIDs.Add(1375655382U, "ReaderComplianceInvalidRowCount");
      TnefStrings.stringIDs.Add(1658289410U, "WriterInvalidOperationNotStringProperty");
      TnefStrings.stringIDs.Add(2086337489U, "ReaderInvalidOperationMustBeAtTheBeginningOfAttribute");
      TnefStrings.stringIDs.Add(924619888U, "ReaderComplianceInvalidPropertyLength");
      TnefStrings.stringIDs.Add(1481845710U, "ReaderComplianceInvalidDateOrTimeValue");
      TnefStrings.stringIDs.Add(1473863903U, "ReaderComplianceInvalidComputedPropertyLength");
      TnefStrings.stringIDs.Add(3713247674U, "WriterInvalidOperationStartNormalPropertyWithName");
      TnefStrings.stringIDs.Add(1090996482U, "ReaderInvalidOperationMustBeInAttribute");
      TnefStrings.stringIDs.Add(3553141442U, "ReaderComplianceInvalidNamedPropertyNameNotZeroTerminated");
      TnefStrings.stringIDs.Add(684398287U, "WriterInvalidOperationValueSizeInvalidForType");
      TnefStrings.stringIDs.Add(5681399U, "WriterInvalidOperation");
      TnefStrings.stringIDs.Add(2819832296U, "WriterNotSupportedCannotAddAnyPropertyToAttribute");
      TnefStrings.stringIDs.Add(1226301788U, "IndexOutOfRange");
      TnefStrings.stringIDs.Add(2425723870U, "ReaderInvalidOperationNotNamedProperty");
      TnefStrings.stringIDs.Add(82425455U, "ReaderComplianceAttributeValueOverflow");
      TnefStrings.stringIDs.Add(3129121212U, "ReaderInvalidOperationPropAfterRaw");
      TnefStrings.stringIDs.Add(3752416298U, "WriterNotSupportedCannotAddThisPropertyToAttribute");
      TnefStrings.stringIDs.Add(3721266330U, "WriterNotSupportedInvalidPropertyType");
      TnefStrings.stringIDs.Add(2410231701U, "ReaderInvalidOperationPropRawAfterText");
      TnefStrings.stringIDs.Add(151827623U, "ReaderInvalidOperationNotSeekableCannotUseRewind");
      TnefStrings.stringIDs.Add(3822473392U, "ReaderComplianceInvalidPropertyTypeMvObject");
      TnefStrings.stringIDs.Add(3882862631U, "ReaderComplianceInvalidAttributeLevel");
      TnefStrings.stringIDs.Add(373431081U, "WriterNotSupportedUnicodeOneOffEntryId");
      TnefStrings.stringIDs.Add(2522592264U, "ReaderInvalidOperationStreamOffsetForAComputedValue");
      TnefStrings.stringIDs.Add(2591161396U, "ReaderInvalidOperationRowsOnlyInRecipientTable");
      TnefStrings.stringIDs.Add(184454741U, "ReaderInvalidOperationPropTextAfterRaw");
      TnefStrings.stringIDs.Add(3432269739U, "WriterNotSupportedLegacyAttributeTooLong");
      TnefStrings.stringIDs.Add(2216356239U, "ReaderInvalidOperationPropertyRawValueTooLong");
      TnefStrings.stringIDs.Add(476021462U, "WriterNotSupportedMallformedEntryId");
      TnefStrings.stringIDs.Add(2801890310U, "WriterInvalidOperationRawDataAfterText");
      TnefStrings.stringIDs.Add(4002053520U, "ReaderInvalidOperationMustBeInPropertyValue");
      TnefStrings.stringIDs.Add(3597881315U, "ReaderInvalidOperationChildActive");
      TnefStrings.stringIDs.Add(1547596720U, "ReaderComplianceInvalidTnefVersionAttributeLength");
      TnefStrings.stringIDs.Add(1208559273U, "ReaderInvalidOperationNotEmbeddedMessage");
      TnefStrings.stringIDs.Add(585789129U, "WriterInvalidOperationStartNamedPropertyNoName");
      TnefStrings.stringIDs.Add(861262429U, "WriterInvalidOperationChildActive");
      TnefStrings.stringIDs.Add(1975716038U, "InvalidMessageCodePage");
      TnefStrings.stringIDs.Add(1236142761U, "WriterInvalidOperationNamedPropertyInLegacyAttribute");
      TnefStrings.stringIDs.Add(2236203585U, "ReaderInvalidOperationMustBeInProperty");
      TnefStrings.stringIDs.Add(3590683541U, "OffsetOutOfRange");
      TnefStrings.stringIDs.Add(2393972388U, "ReaderComplianceInvalidMessageClassLength");
      TnefStrings.stringIDs.Add(332698909U, "WriterInvalidOperationValueTooLongForType");
      TnefStrings.stringIDs.Add(1784846278U, "WriterNotSupportedInvalidMessageClass");
      TnefStrings.stringIDs.Add(690917892U, "WriterNotSupportedNotEnoughInformationForAttribute");
      TnefStrings.stringIDs.Add(2998725655U, "ReaderComplianceTnefTruncated");
    }

    public static string GetLocalizedString(TnefStrings.IDs key)
    {
      return TnefStrings.ResourceManager.GetString(TnefStrings.stringIDs[(uint) key]);
    }

    public enum IDs : uint
    {
      WriterInvalidOperation = 5681399U,
      WriterInvalidOperationObjectInRecipientTable = 8456632U,
      ReaderComplianceInvalidMessageCodepage = 18124002U,
      WriterNotSupportedNotOneOffEntryId = 32688031U,
      ReaderComplianceInvalidAttributeLength = 45144793U,
      ReaderComplianceAttributeValueOverflow = 82425455U,
      ReaderComplianceInvalidTnefVersion = 116892624U,
      ReaderComplianceInvalidPropertyTypeMvBoolean = 138477889U,
      WriterPropertyNameEmptyOrTooLong = 149645149U,
      ReaderInvalidOperationNotSeekableCannotUseRewind = 151827623U,
      ReaderInvalidOperationPropTextAfterRaw = 184454741U,
      WriterInvalidOperationMoreThanOneValueForSingleValuedProperty = 251475407U,
      ReaderComplianceInvalidOemCodepageAttributeLength = 278456090U,
      WriterInvalidOperationValueTooLongForType = 332698909U,
      WriterInvalidOperationInvalidValueType = 340503501U,
      WriterInvalidMessageCodepage = 354067927U,
      WriterNotSupportedUnicodeOneOffEntryId = 373431081U,
      WriterNotSupportedMallformedEntryId = 476021462U,
      WriterNotSupportedInvalidRecipientInformation = 560728928U,
      WriterInvalidOperationNotObjectProperty = 585424572U,
      WriterInvalidOperationStartNamedPropertyNoName = 585789129U,
      WriterInvalidOperationValueSizeInvalidForType = 684398287U,
      WriterNotSupportedNotEnoughInformationForAttribute = 690917892U,
      ReaderInvalidOperationReadNextRowOnlyInRecipientTable = 718178560U,
      ReaderComplianceInvalidPropertyTypeError = 801076374U,
      WriterInvalidOperationChildActive = 861262429U,
      ReaderComplianceInvalidPropertyLength = 924619888U,
      ReaderInvalidOperationCannotConvertValue = 1030605270U,
      ReaderInvalidOperationMustBeInAttribute = 1090996482U,
      ReaderInvalidOperationNotEmbeddedMessage = 1208559273U,
      IndexOutOfRange = 1226301788U,
      WriterInvalidOperationNamedPropertyInLegacyAttribute = 1236142761U,
      ReaderComplianceInvalidRowCount = 1375655382U,
      ReaderComplianceInvalidComputedPropertyLength = 1473863903U,
      ReaderComplianceInvalidDateOrTimeValue = 1481845710U,
      ReaderComplianceInvalidTnefVersionAttributeLength = 1547596720U,
      ReaderComplianceInvalidFrom = 1579220719U,
      CountOutOfRange = 1590522975U,
      WriterInvalidOperationNotStringProperty = 1658289410U,
      ReaderComplianceInvalidPropertyCount = 1676581361U,
      ReaderComplianceInvalidPropertyType = 1718248948U,
      WriterNotSupportedInvalidMessageClass = 1784846278U,
      ReaderInvalidOperationTextPropertyTooLong = 1889035737U,
      InvalidMessageCodePage = 1975716038U,
      ReaderComplianceInvalidNamedPropertyNameLength = 1982026044U,
      StreamDoesNotSupportRead = 1984727737U,
      ReaderInvalidOperationMustBeAtTheBeginningOfAttribute = 2086337489U,
      ReaderComplianceInvalidPropertyTypeObjectInRecipientTable = 2125705981U,
      ReaderComplianceInvalidPropertyValueCount = 2193753292U,
      ReaderInvalidOperationPropertyRawValueTooLong = 2216356239U,
      ReaderInvalidOperationMustBeInProperty = 2236203585U,
      WriterNotSupportedCannotAddThisPropertyToAttributeMoreThanOnce = 2301033583U,
      ReaderComplianceInvalidMessageClassLength = 2393972388U,
      ReaderInvalidOperationPropRawAfterText = 2410231701U,
      ReaderInvalidOperationNotNamedProperty = 2425723870U,
      WriterInvalidOperationTextAfterRawData = 2472716582U,
      ReaderInvalidOperationStreamOffsetForAComputedValue = 2522592264U,
      ReaderComplianceInvalidAttributeChecksum = 2548454096U,
      ReaderInvalidOperationRowsOnlyInRecipientTable = 2591161396U,
      ReaderComplianceInvalidMessageClassNotZeroTerminated = 2720608910U,
      CountTooLarge = 2746482960U,
      WriterInvalidOperationRawDataAfterText = 2801890310U,
      ReaderInvalidOperationMustBeAtTheBeginningOfProperty = 2802240034U,
      ReaderInvalidOperationCannotCloseParentWhileChildOpen = 2814570743U,
      WriterNotSupportedCannotAddAnyPropertyToAttribute = 2819832296U,
      WriterInvalidOperationStartRowNotInRecipientTable = 2826628516U,
      WriterInvalidOperationMvObject = 2874206193U,
      ReaderComplianceInvalidSchedulePlus = 2890237940U,
      ReaderComplianceTnefTruncated = 2998725655U,
      ReaderInvalidOperationRawAfterProp = 3093568444U,
      ReaderInvalidOperationPropAfterRaw = 3129121212U,
      ReaderComplianceInvalidPropertyValueDate = 3183309601U,
      ReaderInvalidOperationNotObjectProperty = 3184266628U,
      StreamDoesNotSupportSeek = 3194581355U,
      StreamDoesNotSupportWrite = 3282557718U,
      ReaderComplianceInvalidTnefSignature = 3347133450U,
      WriterNotSupportedLegacyAttributeTooLong = 3432269739U,
      ReaderComplianceInvalidNamedPropertyNameNotZeroTerminated = 3553141442U,
      OffsetOutOfRange = 3590683541U,
      ReaderInvalidOperationChildActive = 3597881315U,
      WriterInvalidOperationStartNormalPropertyWithName = 3713247674U,
      WriterInvalidOperationUnicodeRawValueForLegacyAttribute = 3713767861U,
      WriterNotSupportedInvalidPropertyType = 3721266330U,
      WriterNotSupportedCannotAddThisPropertyToAttribute = 3752416298U,
      ReaderComplianceInvalidPropertyTypeMvObject = 3822473392U,
      WriterInvalidOperationInvalidPropertyType = 3854182917U,
      ReaderComplianceInvalidPropertyLengthObject = 3871797491U,
      ReaderComplianceInvalidAttributeLevel = 3882862631U,
      ReaderInvalidOperationMustBeInARow = 3999265701U,
      ReaderInvalidOperationMustBeInPropertyValue = 4002053520U,
      ReaderComplianceTooDeepEmbedding = 4260198087U,
      ReaderComplianceInvalidConversationId = 4267063105U,
    }
  }
}
