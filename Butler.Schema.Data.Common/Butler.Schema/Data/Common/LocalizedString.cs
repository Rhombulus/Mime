// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.LocalizedString
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Butler.Schema.Data.Common
{
  [Serializable]
  public struct LocalizedString : ISerializable, ILocalizedString, IFormattable, IEquatable<LocalizedString>
  {
    public static readonly LocalizedString Empty = new LocalizedString();
    internal readonly string Id;
    private readonly object[] Inserts;
    private readonly string stringId;
    private readonly bool showStringIdInUIIfError;
    private readonly bool showAssistanceInfoInUIIfError;
    private readonly ExchangeResourceManager ResourceManager;
    private readonly string DeserializedFallback;
    private ReadOnlyCollection<object> formatParameters;

    public bool IsEmpty
    {
      get
      {
        return null == this.Id;
      }
    }

    public string FullId
    {
      get
      {
        return (this.ResourceManager != null ? this.ResourceManager.BaseName : string.Empty) + this.Id;
      }
    }

    public int BaseId
    {
      get
      {
        return this.FullId.GetHashCode();
      }
    }

    public string StringId
    {
      get
      {
        if (this.stringId != null)
          return this.stringId;
        return string.Empty;
      }
    }

    public bool ShowStringIdInUIIfError
    {
      get
      {
        return this.showStringIdInUIIfError;
      }
    }

    public bool ShowAssistanceInfoInUIIfError
    {
      get
      {
        return this.showAssistanceInfoInUIIfError;
      }
    }

    LocalizedString ILocalizedString.LocalizedString
    {
      get
      {
        return this;
      }
    }

    public ReadOnlyCollection<object> FormatParameters
    {
      get
      {
        return this.formatParameters;
      }
    }

    public LocalizedString(string id, ExchangeResourceManager resourceManager, params object[] inserts)
    {
      this = new LocalizedString(id, (string) null, false, false, resourceManager, inserts);
    }

    public LocalizedString(string id, string stringId, bool showStringIdIfError, bool showAssistanceInfoIfError, ExchangeResourceManager resourceManager, params object[] inserts)
    {
      if (id == null)
        throw new ArgumentNullException(nameof(id));
      if (resourceManager == null)
        throw new ArgumentNullException(nameof(resourceManager));
      this.Id = id;
      this.stringId = stringId;
      this.showStringIdInUIIfError = showStringIdIfError;
      this.showAssistanceInfoInUIIfError = showAssistanceInfoIfError;
      this.ResourceManager = resourceManager;
      this.DeserializedFallback = (string) null;
      this.Inserts = inserts == null || inserts.Length <= 0 ? (object[]) null : inserts;
      this.formatParameters = this.Inserts != null ? new ReadOnlyCollection<object>((IList<object>) this.Inserts) : (ReadOnlyCollection<object>) null;
    }

    public LocalizedString(string value)
    {
      this.Id = value;
      this.stringId = (string) null;
      this.showStringIdInUIIfError = false;
      this.showAssistanceInfoInUIIfError = false;
      this.Inserts = (object[]) null;
      this.ResourceManager = (ExchangeResourceManager) null;
      this.DeserializedFallback = (string) null;
      this.formatParameters = (ReadOnlyCollection<object>) null;
    }

    private LocalizedString(string format, object[] inserts)
    {
      this.Id = format;
      this.stringId = (string) null;
      this.showStringIdInUIIfError = false;
      this.showAssistanceInfoInUIIfError = false;
      this.Inserts = inserts;
      this.ResourceManager = (ExchangeResourceManager) null;
      this.DeserializedFallback = (string) null;
      this.formatParameters = this.Inserts != null ? new ReadOnlyCollection<object>((IList<object>) this.Inserts) : (ReadOnlyCollection<object>) null;
    }

    private LocalizedString(SerializationInfo info, StreamingContext context)
    {
      this.Inserts = (object[]) info.GetValue("inserts", typeof (object[]));
      this.formatParameters = this.Inserts != null ? new ReadOnlyCollection<object>((IList<object>) this.Inserts) : (ReadOnlyCollection<object>) null;
      this.ResourceManager = (ExchangeResourceManager) null;
      this.Id = (string) null;
      this.stringId = (string) null;
      this.showStringIdInUIIfError = false;
      this.showAssistanceInfoInUIIfError = false;
      this.DeserializedFallback = (string) null;
      string str = (string) null;
      try
      {
        string @string = info.GetString("baseName");
        str = info.GetString("fallback");
        if (!string.IsNullOrEmpty(@string))
        {
          Assembly assembly = Assembly.Load(new AssemblyName(info.GetString("assemblyName")));
          this.ResourceManager = ExchangeResourceManager.GetResourceManager(@string, assembly);
          this.Id = info.GetString("id");
          if (this.ResourceManager.GetString(this.Id) == null)
          {
            this.ResourceManager = (ExchangeResourceManager) null;
          }
          else
          {
            this.DeserializedFallback = str;
            try
            {
              this.stringId = info.GetString("stringId");
              this.showStringIdInUIIfError = info.GetBoolean("showStringIdInUIIfError");
              this.showAssistanceInfoInUIIfError = info.GetBoolean("showAssistanceInfoInUIIfError");
            }
            catch (SerializationException ex)
            {
              this.stringId = (string) null;
              this.showStringIdInUIIfError = false;
              this.showAssistanceInfoInUIIfError = false;
            }
          }
        }
      }
      catch (SerializationException ex)
      {
        this.ResourceManager = (ExchangeResourceManager) null;
      }
      catch (FileNotFoundException ex)
      {
        this.ResourceManager = (ExchangeResourceManager) null;
      }
      catch (FileLoadException ex)
      {
        this.ResourceManager = (ExchangeResourceManager) null;
      }
      catch (MissingManifestResourceException ex)
      {
        this.ResourceManager = (ExchangeResourceManager) null;
      }
      if (this.ResourceManager != null)
        return;
      this.Id = str;
    }

    public static implicit operator string(LocalizedString value)
    {
      return value.ToString();
    }

    public static bool operator ==(LocalizedString s1, LocalizedString s2)
    {
      return s1.Equals(s2);
    }

    public static bool operator !=(LocalizedString s1, LocalizedString s2)
    {
      return !s1.Equals(s2);
    }

    public static LocalizedString Join(object separator, object[] value)
    {
      if (value == null || value.Length == 0)
        return LocalizedString.Empty;
      if (separator == null)
        separator = (object) string.Empty;
      object[] inserts = new object[value.Length + 1];
      inserts[0] = separator;
      Array.Copy((Array) value, 0, (Array) inserts, 1, value.Length);
      StringBuilder stringBuilder = new StringBuilder(6 * value.Length);
      stringBuilder.Append("{");
      for (int index = 1; index < value.Length; ++index)
      {
        stringBuilder.Append(index);
        stringBuilder.Append("}{0}{");
      }
      stringBuilder.Append((string) (object) value.Length + (object) "}");
      return new LocalizedString(stringBuilder.ToString(), inserts);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      object[] objArray = (object[]) null;
      if (this.Inserts != null && this.Inserts.Length > 0)
      {
        objArray = new object[this.Inserts.Length];
        for (int index = 0; index < this.Inserts.Length; ++index)
        {
          object badObject = this.Inserts[index];
          if (badObject != null)
          {
            if (badObject is ILocalizedString)
              badObject = (object) ((ILocalizedString) badObject).LocalizedString;
            else if (!((Type) IntrospectionExtensions.GetTypeInfo(badObject.GetType())).IsSerializable && !(badObject is ISerializable))
            {
              object obj = LocalizedString.TranslateObject(badObject, (IFormatProvider) CultureInfo.InvariantCulture);
              badObject = obj != badObject ? obj : (object) badObject.ToString();
            }
          }
          objArray[index] = badObject;
        }
      }
      info.AddValue("inserts", (object) objArray);
      if (this.ResourceManager != null)
      {
        info.AddValue("baseName", (object) this.ResourceManager.BaseName);
        info.AddValue("assemblyName", (object) this.ResourceManager.AssemblyName);
        info.AddValue("id", (object) this.Id);
        info.AddValue("stringId", (object) this.stringId);
        info.AddValue("showStringIdInUIIfError", this.showStringIdInUIIfError);
        info.AddValue("showAssistanceInfoInUIIfError", this.showAssistanceInfoInUIIfError);
        if (this.DeserializedFallback == null)
          info.AddValue("fallback", (object) this.ResourceManager.GetString(this.Id, CultureInfo.InvariantCulture));
        else
          info.AddValue("fallback", (object) this.DeserializedFallback);
      }
      else
      {
        if (this.DeserializedFallback == null)
          info.AddValue("fallback", (object) this.Id);
        else
          info.AddValue("fallback", (object) this.DeserializedFallback);
        info.AddValue("baseName", (object) string.Empty);
      }
    }

    public LocalizedString RecreateWithNewParams(params object[] inserts)
    {
      return new LocalizedString(this.Id, this.StringId, this.ShowStringIdInUIIfError, this.ShowAssistanceInfoInUIIfError, this.ResourceManager, inserts);
    }

    public override string ToString()
    {
      return ((IFormattable) this).ToString((string) null, (IFormatProvider) null);
    }

    public string ToString(IFormatProvider formatProvider)
    {
      return ((IFormattable) this).ToString((string) null, formatProvider);
    }

    string IFormattable.ToString(string format, IFormatProvider formatProvider)
    {
      if (this.IsEmpty)
        return string.Empty;
      format = this.ResourceManager != null ? this.ResourceManager.GetString(this.Id, formatProvider as CultureInfo) : this.Id;
      if (this.Inserts == null || this.Inserts.Length <= 0)
        return format;
      object[] objArray = new object[this.Inserts.Length];
      for (int index = 0; index < this.Inserts.Length; ++index)
      {
        object badObject = this.Inserts[index];
        object obj = !(badObject is ILocalizedString) ? LocalizedString.TranslateObject(badObject, formatProvider) : (object) ((ILocalizedString) badObject).LocalizedString;
        objArray[index] = obj;
      }
      try
      {
        return string.Format(formatProvider, format, objArray);
      }
      catch (FormatException ex)
      {
        if (this.DeserializedFallback != null)
          return string.Format(formatProvider, this.DeserializedFallback, objArray);
        throw;
      }
    }

    public override int GetHashCode()
    {
      int num = (this.Id != null ? this.Id.GetHashCode() : 0) ^ (this.ResourceManager != null ? this.ResourceManager.GetHashCode() : 0);
      if (this.Inserts != null)
      {
        for (int index = 0; index < this.Inserts.Length; ++index)
          num = num ^ index ^ (this.Inserts[index] != null ? this.Inserts[index].GetHashCode() : 0);
      }
      return num;
    }

    public override bool Equals(object obj)
    {
      if (obj is LocalizedString)
        return this.Equals((LocalizedString) obj);
      return false;
    }

    public bool Equals(LocalizedString that)
    {
      if (!string.Equals(this.Id, that.Id, StringComparison.OrdinalIgnoreCase) || !string.Equals(this.stringId, that.stringId, StringComparison.OrdinalIgnoreCase) || (this.showStringIdInUIIfError != that.showStringIdInUIIfError || this.showAssistanceInfoInUIIfError != that.showAssistanceInfoInUIIfError) || (null != this.ResourceManager ^ null != that.ResourceManager || this.ResourceManager != null && !this.ResourceManager.Equals((object) that.ResourceManager)) || null != this.Inserts ^ null != that.Inserts)
        return false;
      if (this.Inserts != null && that.Inserts != null)
      {
        if (this.Inserts.Length != that.Inserts.Length)
          return false;
        for (int index = 0; index < this.Inserts.Length; ++index)
        {
          if (null != this.Inserts[index] ^ null != that.Inserts[index] || this.Inserts[index] != null && that.Inserts[index] != null && !this.Inserts[index].Equals(that.Inserts[index]))
            return false;
        }
      }
      return true;
    }

    private static object TranslateObject(object badObject, IFormatProvider formatProvider)
    {
      if (badObject is Exception)
        return (object) ((Exception) badObject).Message;
      return badObject;
    }
  }
}
