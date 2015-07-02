namespace Butler.Schema.Data.Common {

    [System.AttributeUsage(System.AttributeTargets.All)]
    [System.Serializable]
    public class LocalizedDescriptionAttribute : System.ComponentModel.DescriptionAttribute, ILocalizedString {

        public LocalizedDescriptionAttribute() {}

        public LocalizedDescriptionAttribute(LocalizedString description) {
            this.LocalizedString = description;
        }

        public override sealed string Description {
            [System.Security.Permissions.HostProtection(System.Security.Permissions.SecurityAction.LinkDemand, SharedState = true)]
            get {
                return this.LocalizedString;
            }
        }

        public LocalizedString LocalizedString { get; }

        public static string FromEnum(System.Type enumType, object value) {
            return LocalizedDescriptionAttribute.FromEnum(enumType, value, null);
        }

        public static string FromEnum(System.Type enumType, object value, System.Globalization.CultureInfo culture) {
            if (enumType == null)
                throw new System.ArgumentNullException("enumType");
            if (!System.Reflection.IntrospectionExtensions.GetTypeInfo(enumType)
                       .IsEnum)
                throw new System.ArgumentException("enumType must be an enum.", "enumType");
            if (value == null)
                throw new System.ArgumentNullException("value");
            var key = System.Enum.ToObject(enumType, value);
            if (locEnumStringTable == null) {
                var dictionary = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<object, string>>();
                System.Threading.Interlocked.CompareExchange(ref locEnumStringTable, dictionary, null);
            }
            string str;
            lock (locEnumStringTable) {
                if (culture == null)
                    culture = System.Globalization.CultureInfo.CurrentCulture;
                System.Collections.Generic.Dictionary<object, string> local_3;
                if (!locEnumStringTable.TryGetValue(culture.Name, out local_3)) {
                    local_3 = new System.Collections.Generic.Dictionary<object, string>();
                    locEnumStringTable.Add(culture.Name, local_3);
                }
                if (local_3.TryGetValue(key, out str))
                    return str;
                var local_4 = key.ToString()
                                 .Split(',');
                var local_5 = new System.Text.StringBuilder();
                for (var local_6 = 0; local_6 < local_4.Length; ++local_6) {
                    var fieldName = local_4[local_6].Trim();
                    var local_7 = fieldName;
                    foreach (System.Reflection.MemberInfo item_0 in System.Linq.Enumerable.Where(
                        System.Reflection.IntrospectionExtensions.GetTypeInfo(enumType)
                              .DeclaredFields,
                        x => x.Name == fieldName)) {
                        using (var resource_0 = System.Linq.Enumerable.Where(item_0.GetCustomAttributes(false), x => x is System.ComponentModel.DescriptionAttribute)
                                                      .GetEnumerator()) {
                            if (resource_0.MoveNext()) {
                                var local_9 = resource_0.Current;
                                local_7 = !(local_9 is LocalizedDescriptionAttribute) ? ((System.ComponentModel.DescriptionAttribute) local_9).Description : ((LocalizedDescriptionAttribute) local_9).LocalizedString.ToString(culture);
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

        private static System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<object, string>> locEnumStringTable;

    }

}