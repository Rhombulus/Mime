// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.EdbFilePath
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data
{
  [Serializable]
  public sealed class EdbFilePath : LocalLongFullPath
  {
    public static readonly string DefaultEdbFilePath = Path.GetFullPath("X:\\Program Files\\");
    private const string EdbFileExtensionString = ".edb";
    public const string TemporaryDatabaseFileName = "tmp.edb";
    public const string DefaultLocalCopyDirectoryName = "LocalCopies";
    public const string MaximumRetrySuffix = "0000";
    private bool? isPathInRootDirectory;

    private bool IsTemporaryEdbFile => 0 == string.Compare(Path.GetFileName(this.PathName), "tmp.edb", StringComparison.OrdinalIgnoreCase);

      public bool IsPathInRootDirectory
    {
      get
      {
        if (!this.IsValid)
          throw new NotSupportedException("IsPathInRootDirectory");
        if (!this.isPathInRootDirectory.HasValue)
        {
          string directoryName = Path.GetDirectoryName(this.PathName);
          this.isPathInRootDirectory = string.IsNullOrEmpty(directoryName) || string.IsNullOrEmpty(Path.GetDirectoryName(directoryName)) ? new bool?(true) : new bool?(false);
        }
        return this.isPathInRootDirectory.Value;
      }
    }

    public static EdbFilePath Parse(string pathName)
    {
      return (EdbFilePath) LongPath.ParseInternal(pathName, (LongPath) new EdbFilePath());
    }

    public static bool TryParse(string path, out EdbFilePath resultObject)
    {
      resultObject = (EdbFilePath) LongPath.TryParseInternal(path, (LongPath) new EdbFilePath());
      return (LongPath) null != (LongPath) resultObject;
    }

    protected override bool ParseCore(string path, bool nothrow)
    {
      if (base.ParseCore(path, nothrow))
      {
        if (this.IsTemporaryEdbFile)
        {
          this.IsValid = false;
          if (!nothrow)
            throw new ArgumentException(CtsResources.DataStrings.ErrorEdbFileCannotBeTmp(path), nameof(path));
        }
        else
        {
          string fileName = Path.GetFileName(this.PathName);
          try
          {
            LocalLongFullPath.ValidateFilePath(Path.Combine(Path.Combine(EdbFilePath.DefaultEdbFilePath, "LocalCopies"), fileName) + "0000");
          }
          catch (FormatException ex)
          {
            this.IsValid = false;
            if (!nothrow)
              throw new ArgumentException(CtsResources.DataStrings.ErrorEdbFileNameTooLong(fileName), (Exception) ex);
          }
        }
      }
      if (!this.IsValid && !nothrow)
        throw new ArgumentException(CtsResources.DataStrings.ErrorEdbFilePathCannotConvert(path));
      return this.IsValid;
    }

    public static EdbFilePath ParseFromPathNameAndFileName(string pathName, string fileName)
    {
      return EdbFilePath.Parse(Path.Combine(pathName, fileName));
    }

    public void ValidateEdbFileExtension()
    {
      LocalLongFullPath.ValidatePathWithSpecifiedExtension((LocalLongFullPath) this, ".edb");
    }
  }
}
