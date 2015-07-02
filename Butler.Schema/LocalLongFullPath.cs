// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.LocalLongFullPath
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data
{
  [Serializable]
  public class LocalLongFullPath : LongPath
  {
    private const int MaxDirectoryPath = 248;
    private const int MaxPath = 260;

    protected LocalLongFullPath()
    {
    }

    public static LocalLongFullPath Parse(string path)
    {
      return (LocalLongFullPath) LongPath.ParseInternal(path, (LongPath) new LocalLongFullPath());
    }

    public static bool TryParse(string path, out LocalLongFullPath resultObject)
    {
      resultObject = (LocalLongFullPath) LongPath.TryParseInternal(path, (LongPath) new LocalLongFullPath());
      return (LongPath) null != (LongPath) resultObject;
    }

    public static LocalLongFullPath ParseFromPathNameAndFileName(string pathName, string fileName)
    {
      LocalLongFullPath localLongFullPath = LocalLongFullPath.Parse(Path.Combine(pathName, fileName));
      try
      {
        localLongFullPath.ValidateFilePathLength();
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex);
      }
      return localLongFullPath;
    }

    public static string ConvertInvalidCharactersInPathName(string fileName)
    {
      return LocalLongFullPath.ConvertInvalidCharactersInternal(fileName, Path.GetInvalidPathChars());
    }

    public static string ConvertInvalidCharactersInFileName(string fileName)
    {
      return LocalLongFullPath.ConvertInvalidCharactersInternal(fileName, Path.GetInvalidFileNameChars());
    }

    protected static string ConvertInvalidCharactersInternal(string fileName, char[] invalidChars)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException(nameof(fileName));
      fileName = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(fileName));
      Array.Sort<char>(invalidChars);
      StringBuilder stringBuilder = new StringBuilder(fileName.Length + 1);
      foreach (char ch in fileName)
      {
        if (Array.BinarySearch<char>(invalidChars, ch) < 0 && (int) ch != 126)
          stringBuilder.Append(ch);
        else
          stringBuilder.Append('_');
      }
      return stringBuilder.ToString().TrimEnd(' ', '.');
    }

    protected static void ValidatePathWithSpecifiedExtension(LocalLongFullPath path, string specifiedExtension)
    {
      if (string.IsNullOrEmpty(specifiedExtension))
        specifiedExtension = string.Empty;
      else if ((int) specifiedExtension[0] != 46 || specifiedExtension.Length == 1 || -1 != specifiedExtension.IndexOfAny(("." + new string(Path.GetInvalidFileNameChars())).ToCharArray(), 1))
        throw new FormatException(Resources.DataStrings.ErrorInvalidExtension(specifiedExtension));
      if (specifiedExtension != null && string.Compare(Path.GetExtension(path.PathName), specifiedExtension, StringComparison.OrdinalIgnoreCase) != 0)
        throw new FormatException(Resources.DataStrings.ErrorFilePathMismatchExpectedExtension(path.PathName, specifiedExtension));
    }

    public void ValidateDirectoryPathLength()
    {
      LocalLongFullPath.ValidateDirectoryPath(this.PathName);
    }

    protected static void ValidateDirectoryPath(string input)
    {
      try
      {
        string path = Path.GetFullPath(input);
        if ((int) Path.DirectorySeparatorChar == (int) path[path.Length - 1] || (int) Path.AltDirectorySeparatorChar == (int) path[path.Length - 1])
          path = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(path) && 248 <= path.Length)
          throw new PathTooLongException(Resources.DataStrings.ErrorLocalLongFullPathTooLong(input));
      }
      catch (IOException ex)
      {
        throw new FormatException(ex.Message, (Exception) ex);
      }
    }

    protected static void ValidateFilePath(string input)
    {
      try
      {
        string fullPath = Path.GetFullPath(input);
        if ((int) Path.DirectorySeparatorChar == (int) fullPath[fullPath.Length - 1] || (int) Path.AltDirectorySeparatorChar == (int) fullPath[fullPath.Length - 1])
          throw new FormatException(Resources.DataStrings.ErrorInvalidFullyQualifiedFileName(input));
        if (Path.IsPathRooted(fullPath) && 259 <= fullPath.Length)
          throw new PathTooLongException(Resources.DataStrings.ErrorLocalLongFullPathTooLong(input));
        string directoryName = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directoryName) && 248 <= directoryName.Length)
          throw new PathTooLongException(Resources.DataStrings.ErrorLocalLongFullPathTooLong(input));
      }
      catch (IOException ex)
      {
        throw new FormatException(ex.Message, (Exception) ex);
      }
    }

    protected override bool ParseCore(string path, bool nothrow)
    {
      if (base.ParseCore(path, nothrow))
      {
        if (!this.IsLocalFull)
        {
          this.IsValid = false;
        }
        else
        {
          try
          {
            Path.GetFullPath(this.PathName);
          }
          catch (IOException ex)
          {
            this.IsValid = false;
            if (!nothrow)
              throw new ArgumentException(ex.Message, (Exception) ex);
          }
        }
      }
      if (!this.IsValid && !nothrow)
        throw new ArgumentException(Resources.DataStrings.ErrorLocalLongFullPathCannotConvert(path), nameof(path));
      return this.IsValid;
    }

    public void ValidateFilePathLength()
    {
      LocalLongFullPath.ValidateFilePath(this.PathName);
    }
  }
}
