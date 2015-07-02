// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.LocalizedDescriptionAttribute
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Butler.Schema.Data.Common
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  [Serializable]
  public class LocalizedDescriptionAttribute : DescriptionAttribute, ILocalizedString
  {
    private static Dictionary<string, Dictionary<object, string>> locEnumStringTable;

      public LocalizedString LocalizedString { get; }

      public override sealed string Description
    {
      [HostProtection(SecurityAction.LinkDemand, SharedState = true)] get
      {
        return (string) this.LocalizedString;
      }
    }

    public LocalizedDescriptionAttribute()
    {
    }

    public LocalizedDescriptionAttribute(LocalizedString description)
    {
      this.LocalizedString = description;
    }

    public static string FromEnum(Type enumType, object value)
    {
      return LocalizedDescriptionAttribute.FromEnum(enumType, value, (CultureInfo) null);
    }

    public static string FromEnum(Type enumType, object value, CultureInfo culture)
    {
      if (enumType == (Type) null)
        throw new ArgumentNullException("enumType");
      if (!((Type) IntrospectionExtensions.GetTypeInfo(enumType)).IsEnum)
        throw new ArgumentException("enumType must be an enum.", "enumType");
      if (value == null)
        throw new ArgumentNullException("value");
      object key = Enum.ToObject(enumType, value);
      if (LocalizedDescriptionAttribute.locEnumStringTable == null)
      {
        Dictionary<string, Dictionary<object, string>> dictionary = new Dictionary<string, Dictionary<object, string>>();
        Interlocked.CompareExchange<Dictionary<string, Dictionary<object, string>>>(ref LocalizedDescriptionAttribute.locEnumStringTable, dictionary, (Dictionary<string, Dictionary<object, string>>) null);
      }
      string str;
      lock (LocalizedDescriptionAttribute.locEnumStringTable)
      {
        if (culture == null)
          culture = CultureInfo.CurrentCulture;
        Dictionary<object, string> local_3;
        if (!LocalizedDescriptionAttribute.locEnumStringTable.TryGetValue(culture.Name, out local_3))
        {
          local_3 = new Dictionary<object, string>();
          LocalizedDescriptionAttribute.locEnumStringTable.Add(culture.Name, local_3);
        }
        if (local_3.TryGetValue(key, out str))
          return str;
        string[] local_4 = key.ToString().Split(',');
        StringBuilder local_5 = new StringBuilder();
        for (int local_6 = 0; local_6 < local_4.Length; ++local_6)
        {
          string fieldName = local_4[local_6].Trim();
          string local_7 = fieldName;
          foreach (MemberInfo item_0 in Enumerable.Where<FieldInfo>(IntrospectionExtensions.GetTypeInfo(enumType).DeclaredFields, (Func<FieldInfo, bool>) (x => x.Name == fieldName)))
          {
            using (IEnumerator<object> resource_0 = Enumerable.Where<object>((IEnumerable<object>) item_0.GetCustomAttributes(false), (Func<object, bool>) (x => x is DescriptionAttribute)).GetEnumerator())
            {
              if (resource_0.MoveNext())
              {
                object local_9 = resource_0.Current;
                local_7 = !(local_9 is LocalizedDescriptionAttribute) ? ((DescriptionAttribute) local_9).Description : ((LocalizedDescriptionAttribute) local_9).LocalizedString.ToString((IFormatProvider) culture);
              }
            }
          }
          if (local_6 != 0)
            local_5.Append(", ");
          local_5.Append(local_7);
        }
        str = local_5.ToString();
        local_3.Add(key, str);
      }
      return str;
    }
  }
}
