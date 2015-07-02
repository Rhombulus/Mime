// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.LongPath
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;

namespace Butler.Schema.Data
{
  [Serializable]
  public class LongPath : IComparable, IComparable<LongPath>, IEquatable<LongPath>
  {
    private string pathName;
    private bool isValid;
    private bool isLocalFull;
    private bool isUnc;
    private string driveName;
    private string serverName;

    public string PathName
    {
      get
      {
        return this.pathName;
      }
      protected set
      {
        this.pathName = value;
      }
    }

    protected bool IsValid
    {
      get
      {
        return this.isValid;
      }
      set
      {
        this.isValid = value;
      }
    }

    public bool IsLocalFull => this.isLocalFull;

      public bool IsUnc => this.isUnc;

      public string DriveName
    {
      get
      {
        if (!this.IsLocalFull)
          throw new NotSupportedException("DriveName");
        return this.driveName;
      }
    }

    public string ServerName
    {
      get
      {
        if (!this.IsUnc)
          throw new NotSupportedException("ServerName");
        return this.serverName;
      }
    }

    protected LongPath()
    {
    }

    public static explicit operator LongPath(FileInfo file)
    {
      if (file == null)
        return (LongPath) null;
      return LongPath.Parse(file.FullName);
    }

    public static explicit operator LongPath(DirectoryInfo dir)
    {
      if (dir == null)
        return (LongPath) null;
      return LongPath.Parse(dir.FullName);
    }

    public static bool operator ==(LongPath a, LongPath b)
    {
      return object.Equals((object) a, (object) b);
    }

    public static bool operator !=(LongPath a, LongPath b)
    {
      return !object.Equals((object) a, (object) b);
    }

    public static LongPath Parse(string path)
    {
      return LongPath.ParseInternal(path, new LongPath());
    }

    public static bool TryParse(string path, out LongPath resultObject)
    {
      resultObject = LongPath.TryParseInternal(path, new LongPath());
      return (LongPath) null != resultObject;
    }

    protected static LongPath ParseInternal(string path, LongPath pathObject)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));
      if (!pathObject.ParseCore(path, false))
        throw new ArgumentException(CtsResources.DataStrings.ErrorLongPathCannotConvert(path), nameof(path));
      return pathObject;
    }

    protected static LongPath TryParseInternal(string path, LongPath pathObject)
    {
      if (pathObject.ParseCore(path, true))
        return pathObject;
      return (LongPath) null;
    }

    protected virtual bool ParseCore(string path, bool nothrow)
    {
      try
      {
        if (!string.IsNullOrEmpty(path))
        {
          if (path.Length == 2 && (int) path[1] == (int) Path.VolumeSeparatorChar)
            path += (string) (object) Path.DirectorySeparatorChar;
          if (LongPath.IsLongPath(path))
          {
            if (!(this.isLocalFull = LongPath.IsLocalFullPath(path, out this.driveName)))
            {
              if (!(this.isUnc = LongPath.IsUncPath(path, out this.serverName)))
                goto label_14;
            }
            string path1 = Path.GetFullPath(path);
            if (this.IsUnc || this.IsLocalFull && !path1.Equals(Path.GetPathRoot(path1), StringComparison.OrdinalIgnoreCase))
              path1 = path1.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            this.PathName = path1;
            this.IsValid = true;
          }
        }
      }
      catch (ArgumentException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
      catch (PathTooLongException ex)
      {
      }
      catch (SecurityException ex)
      {
        if (!nothrow)
          throw new ArgumentException(ex.Message, (Exception) ex);
      }
label_14:
      return this.IsValid;
    }

    private static bool IsLongPath(string path)
    {
      if (path != null)
        return -1 == path.IndexOf('~');
      return false;
    }

    private static bool IsLocalFullPath(string path, out string drive)
    {
      drive = (string) null;
      bool flag;
      try
      {
        flag = !new Uri(path).IsUnc;
      }
      catch (UriFormatException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex);
      }
      if (flag)
      {
        if (!Path.IsPathRooted(path) || -1 == path.IndexOf(Path.VolumeSeparatorChar))
          flag = false;
        else
          drive = path.Substring(0, path.IndexOf(Path.VolumeSeparatorChar) + 1);
      }
      return flag;
    }

    private static bool IsUncPath(string path, out string server)
    {
      server = (string) null;
      try
      {
        Uri uri = new Uri(path);
        if (uri.IsUnc)
          server = uri.Host;
      }
      catch (UriFormatException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex);
      }
      return null != server;
    }

    public override string ToString()
    {
      return this.PathName;
    }

    public override int GetHashCode()
    {
      return this.PathName.ToLower(CultureInfo.InvariantCulture).GetHashCode();
    }

    public int CompareTo(LongPath value)
    {
      if ((LongPath) null == value)
        return 1;
      if (object.ReferenceEquals((object) this, (object) value))
        return 0;
      Type type = this.GetType();
      if (type != value.GetType())
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "object must be of type {0}", new object[1]
        {
          (object) type.Name
        }));
      return string.Compare(this.PathName, value.PathName, StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo(object value)
    {
      LongPath longPath = value as LongPath;
      if ((LongPath) null == longPath && value != null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "object must be of type {0}", new object[1]
        {
          (object) this.GetType().Name
        }));
      return this.CompareTo(longPath);
    }

    public override bool Equals(object value)
    {
      return this.Equals(value as LongPath);
    }

    public bool Equals(LongPath value)
    {
      if (object.ReferenceEquals((object) this, (object) value))
        return true;
      bool flag = false;
      if ((LongPath) null != value && this.GetType() == value.GetType())
        flag = string.Equals(this.PathName, value.PathName, StringComparison.OrdinalIgnoreCase);
      return flag;
    }
  }
}
