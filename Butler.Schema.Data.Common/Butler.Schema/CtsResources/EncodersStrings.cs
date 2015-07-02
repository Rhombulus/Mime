// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.CtsResources.EncodersStrings
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
  internal static class EncodersStrings
  {
    private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(29);
    private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.EncodersStrings", ((Type) IntrospectionExtensions.GetTypeInfo(typeof (EncodersStrings))).Assembly);

    public static string MacBinFileNameTooLong => EncodersStrings.ResourceManager.GetString("MacBinFileNameTooLong");

      public static string BinHexEncoderInternalError => EncodersStrings.ResourceManager.GetString("BinHexEncoderInternalError");

      public static string BinHexHeaderBadFileNameLength => EncodersStrings.ResourceManager.GetString("BinHexHeaderBadFileNameLength");

      public static string EncStrCannotCloneWriteableStream => EncodersStrings.ResourceManager.GetString("EncStrCannotCloneWriteableStream");

      public static string BinHexDecoderLineTooLong => EncodersStrings.ResourceManager.GetString("BinHexDecoderLineTooLong");

      public static string BinHexDecoderInternalError => EncodersStrings.ResourceManager.GetString("BinHexDecoderInternalError");

      public static string BinHexDecoderDataCorrupt => EncodersStrings.ResourceManager.GetString("BinHexDecoderDataCorrupt");

      public static string BinHexDecoderFoundInvalidCharacter => EncodersStrings.ResourceManager.GetString("BinHexDecoderFoundInvalidCharacter");

      public static string BinHexHeaderInvalidNameLength => EncodersStrings.ResourceManager.GetString("BinHexHeaderInvalidNameLength");

      public static string MacBinBadVersion => EncodersStrings.ResourceManager.GetString("MacBinBadVersion");

      public static string BinHexHeaderIncomplete => EncodersStrings.ResourceManager.GetString("BinHexHeaderIncomplete");

      public static string BinHexDecoderFileNameTooLong => EncodersStrings.ResourceManager.GetString("BinHexDecoderFileNameTooLong");

      public static string UUDecoderInvalidData => EncodersStrings.ResourceManager.GetString("UUDecoderInvalidData");

      public static string EncStrCannotRead => EncodersStrings.ResourceManager.GetString("EncStrCannotRead");

      public static string BinHexHeaderUnsupportedVersion => EncodersStrings.ResourceManager.GetString("BinHexHeaderUnsupportedVersion");

      public static string UUDecoderInvalidDataBadLine => EncodersStrings.ResourceManager.GetString("UUDecoderInvalidDataBadLine");

      public static string BinHexHeaderInvalidCrc => EncodersStrings.ResourceManager.GetString("BinHexHeaderInvalidCrc");

      public static string BinHexDecoderFirstNonWhitespaceMustBeColon => EncodersStrings.ResourceManager.GetString("BinHexDecoderFirstNonWhitespaceMustBeColon");

      public static string MacBinHeaderMustBe128Long => EncodersStrings.ResourceManager.GetString("MacBinHeaderMustBe128Long");

      public static string EncStrCannotSeek => EncodersStrings.ResourceManager.GetString("EncStrCannotSeek");

      public static string BinHexEncoderDoesNotSupportResourceFork => EncodersStrings.ResourceManager.GetString("BinHexEncoderDoesNotSupportResourceFork");

      public static string BinHexEncoderDataCorruptCannotFinishEncoding => EncodersStrings.ResourceManager.GetString("BinHexEncoderDataCorruptCannotFinishEncoding");

      public static string MacBinInvalidData => EncodersStrings.ResourceManager.GetString("MacBinInvalidData");

      public static string BinHexHeaderTooSmall => EncodersStrings.ResourceManager.GetString("BinHexHeaderTooSmall");

      public static string QPEncoderNoSpaceForLineBreak => EncodersStrings.ResourceManager.GetString("QPEncoderNoSpaceForLineBreak");

      public static string BinHexDecoderBadResourceForkCrc => EncodersStrings.ResourceManager.GetString("BinHexDecoderBadResourceForkCrc");

      public static string BinHexDecoderLineCorrupt => EncodersStrings.ResourceManager.GetString("BinHexDecoderLineCorrupt");

      public static string BinHexDecoderBadCrc => EncodersStrings.ResourceManager.GetString("BinHexDecoderBadCrc");

      public static string EncStrCannotWrite => EncodersStrings.ResourceManager.GetString("EncStrCannotWrite");

      static EncodersStrings()
    {
      EncodersStrings.stringIDs.Add(3966390553U, "MacBinFileNameTooLong");
      EncodersStrings.stringIDs.Add(25286151U, "BinHexEncoderInternalError");
      EncodersStrings.stringIDs.Add(217720121U, "BinHexHeaderBadFileNameLength");
      EncodersStrings.stringIDs.Add(1288990070U, "EncStrCannotCloneWriteableStream");
      EncodersStrings.stringIDs.Add(812742886U, "BinHexDecoderLineTooLong");
      EncodersStrings.stringIDs.Add(810486593U, "BinHexDecoderInternalError");
      EncodersStrings.stringIDs.Add(2498997775U, "BinHexDecoderDataCorrupt");
      EncodersStrings.stringIDs.Add(2916696602U, "BinHexDecoderFoundInvalidCharacter");
      EncodersStrings.stringIDs.Add(804376101U, "BinHexHeaderInvalidNameLength");
      EncodersStrings.stringIDs.Add(2503051247U, "MacBinBadVersion");
      EncodersStrings.stringIDs.Add(2749850901U, "BinHexHeaderIncomplete");
      EncodersStrings.stringIDs.Add(450079973U, "BinHexDecoderFileNameTooLong");
      EncodersStrings.stringIDs.Add(2959384831U, "UUDecoderInvalidData");
      EncodersStrings.stringIDs.Add(2345827606U, "EncStrCannotRead");
      EncodersStrings.stringIDs.Add(4253113056U, "BinHexHeaderUnsupportedVersion");
      EncodersStrings.stringIDs.Add(2009660788U, "UUDecoderInvalidDataBadLine");
      EncodersStrings.stringIDs.Add(1330362610U, "BinHexHeaderInvalidCrc");
      EncodersStrings.stringIDs.Add(2479018069U, "BinHexDecoderFirstNonWhitespaceMustBeColon");
      EncodersStrings.stringIDs.Add(3717726462U, "MacBinHeaderMustBe128Long");
      EncodersStrings.stringIDs.Add(67282974U, "EncStrCannotSeek");
      EncodersStrings.stringIDs.Add(3114903713U, "BinHexEncoderDoesNotSupportResourceFork");
      EncodersStrings.stringIDs.Add(3183910392U, "BinHexEncoderDataCorruptCannotFinishEncoding");
      EncodersStrings.stringIDs.Add(2984114443U, "MacBinInvalidData");
      EncodersStrings.stringIDs.Add(2549049936U, "BinHexHeaderTooSmall");
      EncodersStrings.stringIDs.Add(1639523968U, "QPEncoderNoSpaceForLineBreak");
      EncodersStrings.stringIDs.Add(2739587701U, "BinHexDecoderBadResourceForkCrc");
      EncodersStrings.stringIDs.Add(62154753U, "BinHexDecoderLineCorrupt");
      EncodersStrings.stringIDs.Add(2823726815U, "BinHexDecoderBadCrc");
      EncodersStrings.stringIDs.Add(1415490657U, "EncStrCannotWrite");
    }

    public static string EncStrLengthExceeded(int sum, int length)
    {
      return string.Format(EncodersStrings.ResourceManager.GetString("EncStrLengthExceeded"), (object) sum, (object) length);
    }

    public static string ThisEncoderDoesNotSupportCloning(string type)
    {
      return string.Format(EncodersStrings.ResourceManager.GetString("ThisEncoderDoesNotSupportCloning"), (object) type);
    }

    public static string UUEncoderFileNameTooLong(int maxChars)
    {
      return string.Format(EncodersStrings.ResourceManager.GetString("UUEncoderFileNameTooLong"), (object) maxChars);
    }

    public static string EncStrCannotCloneChildStream(string className)
    {
      return string.Format(EncodersStrings.ResourceManager.GetString("EncStrCannotCloneChildStream"), (object) className);
    }

    public static string MacBinIconOffsetTooLarge(int max)
    {
      return string.Format(EncodersStrings.ResourceManager.GetString("MacBinIconOffsetTooLarge"), (object) max);
    }

    public static string GetLocalizedString(EncodersStrings.IDs key)
    {
      return EncodersStrings.ResourceManager.GetString(EncodersStrings.stringIDs[(uint) key]);
    }

    public enum IDs : uint
    {
      BinHexEncoderInternalError = 25286151U,
      BinHexDecoderLineCorrupt = 62154753U,
      EncStrCannotSeek = 67282974U,
      BinHexHeaderBadFileNameLength = 217720121U,
      BinHexDecoderFileNameTooLong = 450079973U,
      BinHexHeaderInvalidNameLength = 804376101U,
      BinHexDecoderInternalError = 810486593U,
      BinHexDecoderLineTooLong = 812742886U,
      EncStrCannotCloneWriteableStream = 1288990070U,
      BinHexHeaderInvalidCrc = 1330362610U,
      EncStrCannotWrite = 1415490657U,
      QPEncoderNoSpaceForLineBreak = 1639523968U,
      UUDecoderInvalidDataBadLine = 2009660788U,
      EncStrCannotRead = 2345827606U,
      BinHexDecoderFirstNonWhitespaceMustBeColon = 2479018069U,
      BinHexDecoderDataCorrupt = 2498997775U,
      MacBinBadVersion = 2503051247U,
      BinHexHeaderTooSmall = 2549049936U,
      BinHexDecoderBadResourceForkCrc = 2739587701U,
      BinHexHeaderIncomplete = 2749850901U,
      BinHexDecoderBadCrc = 2823726815U,
      BinHexDecoderFoundInvalidCharacter = 2916696602U,
      UUDecoderInvalidData = 2959384831U,
      MacBinInvalidData = 2984114443U,
      BinHexEncoderDoesNotSupportResourceFork = 3114903713U,
      BinHexEncoderDataCorruptCannotFinishEncoding = 3183910392U,
      MacBinHeaderMustBe128Long = 3717726462U,
      MacBinFileNameTooLong = 3966390553U,
      BinHexHeaderUnsupportedVersion = 4253113056U,
    }

    private enum ParamIDs
    {
      EncStrLengthExceeded,
      ThisEncoderDoesNotSupportCloning,
      UUEncoderFileNameTooLong,
      EncStrCannotCloneChildStream,
      MacBinIconOffsetTooLarge,
    }
  }
}
