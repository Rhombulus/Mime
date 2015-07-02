// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.CtsResources.DataStrings
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
  internal static class DataStrings
  {
    private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);
    private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.DataStrings", ((Type) IntrospectionExtensions.GetTypeInfo(typeof (DataStrings))).Assembly);

    public static string ErrorPathCanNotBeRoot => DataStrings.ResourceManager.GetString("ErrorPathCanNotBeRoot");

      public static string ConstraintViolationNoLeadingOrTrailingWhitespace => DataStrings.ResourceManager.GetString("ConstraintViolationNoLeadingOrTrailingWhitespace");

      static DataStrings()
    {
      DataStrings.stringIDs.Add(1256740561U, "ErrorPathCanNotBeRoot");
      DataStrings.stringIDs.Add(2058499689U, "ConstraintViolationNoLeadingOrTrailingWhitespace");
    }

    public static string ErrorInvalidFullyQualifiedFileName(string path)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorInvalidFullyQualifiedFileName"), (object) path);
    }

    public static string ErrorFilePathMismatchExpectedExtension(string path, string extension)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorFilePathMismatchExpectedExtension"), (object) path, (object) extension);
    }

    public static string ErrorEdbFileCannotBeUncPath(string pathName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFileCannotBeUncPath"), (object) pathName);
    }

    public static string ErrorUncPathMustUseServerName(string path)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathMustUseServerName"), (object) path);
    }

    public static string ErrorEdbFilePathCannotConvert(string pathName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFilePathCannotConvert"), (object) pathName);
    }

    public static string ErrorEdbFileNameTooLong(string fileName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFileNameTooLong"), (object) fileName);
    }

    public static string ErrorUncPathMustBeUncPath(string path)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathMustBeUncPath"), (object) path);
    }

    public static string ErrorEdbFileCannotBeTmp(string pathName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFileCannotBeTmp"), (object) pathName);
    }

    public static string ErrorLocalLongFullPathTooLong(string path)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorLocalLongFullPathTooLong"), (object) path);
    }

    public static string ErrorUncPathMustBeUncPathOnly(string path)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathMustBeUncPathOnly"), (object) path);
    }

    public static string ErrorLocalLongFullPathCannotConvert(string pathName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorLocalLongFullPathCannotConvert"), (object) pathName);
    }

    public static string ErrorUncPathTooLong(string path)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathTooLong"), (object) path);
    }

    public static string ErrorLongPathCannotConvert(string pathName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorLongPathCannotConvert"), (object) pathName);
    }

    public static string ErrorInvalidExtension(string extension)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorInvalidExtension"), (object) extension);
    }

    public static string ErrorLocalLongFullAsciiPathCannotConvert(string pathName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorLocalLongFullAsciiPathCannotConvert"), (object) pathName);
    }

    public static string ErrorStmFilePathCannotConvert(string pathName)
    {
      return string.Format(DataStrings.ResourceManager.GetString("ErrorStmFilePathCannotConvert"), (object) pathName);
    }

    public static string GetLocalizedString(DataStrings.IDs key)
    {
      return DataStrings.ResourceManager.GetString(DataStrings.stringIDs[(uint) key]);
    }

    public enum IDs : uint
    {
      ErrorPathCanNotBeRoot = 1256740561U,
      ConstraintViolationNoLeadingOrTrailingWhitespace = 2058499689U,
    }

    private enum ParamIDs
    {
      ErrorInvalidFullyQualifiedFileName,
      ErrorFilePathMismatchExpectedExtension,
      ErrorEdbFileCannotBeUncPath,
      ErrorUncPathMustUseServerName,
      ErrorEdbFilePathCannotConvert,
      ErrorEdbFileNameTooLong,
      ErrorUncPathMustBeUncPath,
      ErrorEdbFileCannotBeTmp,
      ErrorLocalLongFullPathTooLong,
      ErrorUncPathMustBeUncPathOnly,
      ErrorLocalLongFullPathCannotConvert,
      ErrorUncPathTooLong,
      ErrorLongPathCannotConvert,
      ErrorInvalidExtension,
      ErrorLocalLongFullAsciiPathCannotConvert,
      ErrorStmFilePathCannotConvert,
    }
  }
}
