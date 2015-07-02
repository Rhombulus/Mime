// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.CtsResources.SharedStrings
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
  internal static class SharedStrings
  {
    private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(8);
    private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.SharedStrings", ((Type) IntrospectionExtensions.GetTypeInfo(typeof (SharedStrings))).Assembly);

    public static string InvalidFactory => SharedStrings.ResourceManager.GetString("InvalidFactory");

      public static string CannotSetNegativelength => SharedStrings.ResourceManager.GetString("CannotSetNegativelength");

      public static string CountOutOfRange => SharedStrings.ResourceManager.GetString("CountOutOfRange");

      public static string CannotSeekBeforeBeginning => SharedStrings.ResourceManager.GetString("CannotSeekBeforeBeginning");

      public static string StringArgumentMustBeUTF8 => SharedStrings.ResourceManager.GetString("StringArgumentMustBeUTF8");

      public static string OffsetOutOfRange => SharedStrings.ResourceManager.GetString("OffsetOutOfRange");

      public static string CountTooLarge => SharedStrings.ResourceManager.GetString("CountTooLarge");

      public static string StringArgumentMustBeAscii => SharedStrings.ResourceManager.GetString("StringArgumentMustBeAscii");

      static SharedStrings()
    {
      SharedStrings.stringIDs.Add(3996289637U, "InvalidFactory");
      SharedStrings.stringIDs.Add(1551326176U, "CannotSetNegativelength");
      SharedStrings.stringIDs.Add(1590522975U, "CountOutOfRange");
      SharedStrings.stringIDs.Add(2864662625U, "CannotSeekBeforeBeginning");
      SharedStrings.stringIDs.Add(2489963781U, "StringArgumentMustBeUTF8");
      SharedStrings.stringIDs.Add(3590683541U, "OffsetOutOfRange");
      SharedStrings.stringIDs.Add(2746482960U, "CountTooLarge");
      SharedStrings.stringIDs.Add(431486251U, "StringArgumentMustBeAscii");
    }

    public static string CreateFileFailed(string filePath)
    {
      return string.Format(SharedStrings.ResourceManager.GetString("CreateFileFailed"), (object) filePath);
    }

    public static string GetLocalizedString(SharedStrings.IDs key)
    {
      return SharedStrings.ResourceManager.GetString(SharedStrings.stringIDs[(uint) key]);
    }

    public enum IDs : uint
    {
      StringArgumentMustBeAscii = 431486251U,
      CannotSetNegativelength = 1551326176U,
      CountOutOfRange = 1590522975U,
      StringArgumentMustBeUTF8 = 2489963781U,
      CountTooLarge = 2746482960U,
      CannotSeekBeforeBeginning = 2864662625U,
      OffsetOutOfRange = 3590683541U,
      InvalidFactory = 3996289637U,
    }

    private enum ParamIDs
    {
      CreateFileFailed,
    }
  }
}
