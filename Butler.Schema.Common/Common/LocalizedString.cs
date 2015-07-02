using System;
using System.Collections.Generic;
using System.Linq;
using IntrospectionExtensions = System.Reflection.IntrospectionExtensions;

namespace Butler.Schema.Data.Common {

    [Serializable]
    public struct LocalizedString : System.Runtime.Serialization.ISerializable, ILocalizedString, IFormattable, IEquatable<LocalizedString> {

        public LocalizedString(string id, ExchangeResourceManager resourceManager, params object[] inserts) {
            this = new LocalizedString(id, null, false, false, resourceManager, inserts);
        }

        public LocalizedString(string id, string stringId, bool showStringIdIfError, bool showAssistanceInfoIfError, ExchangeResourceManager resourceManager, params object[] inserts) {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (resourceManager == null)
                throw new ArgumentNullException(nameof(resourceManager));
            Id = id;
            this.stringId = stringId;
            this.ShowStringIdInUIIfError = showStringIdIfError;
            this.ShowAssistanceInfoInUIIfError = showAssistanceInfoIfError;
            ResourceManager = resourceManager;
            DeserializedFallback = null;
            Inserts = inserts == null || inserts.Length <= 0 ? null : inserts;
            this.FormatParameters = Inserts != null ? new System.Collections.ObjectModel.ReadOnlyCollection<object>(Inserts) : null;
        }

        public LocalizedString(string value) {
            Id = value;
            stringId = null;
            this.ShowStringIdInUIIfError = false;
            this.ShowAssistanceInfoInUIIfError = false;
            Inserts = null;
            ResourceManager = null;
            DeserializedFallback = null;
            this.FormatParameters = null;
        }

        private LocalizedString(string format, object[] inserts) {
            Id = format;
            stringId = null;
            this.ShowStringIdInUIIfError = false;
            this.ShowAssistanceInfoInUIIfError = false;
            Inserts = inserts;
            ResourceManager = null;
            DeserializedFallback = null;
            this.FormatParameters = Inserts != null ? new System.Collections.ObjectModel.ReadOnlyCollection<object>(Inserts) : null;
        }

        private LocalizedString(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            Inserts = (object[]) info.GetValue("inserts", typeof (object[]));
            this.FormatParameters = Inserts != null ? new System.Collections.ObjectModel.ReadOnlyCollection<object>(Inserts) : null;
            ResourceManager = null;
            Id = null;
            stringId = null;
            this.ShowStringIdInUIIfError = false;
            this.ShowAssistanceInfoInUIIfError = false;
            DeserializedFallback = null;
            string str = null;
            try {
                var @string = info.GetString("baseName");
                str = info.GetString("fallback");
                if (!string.IsNullOrEmpty(@string)) {
                    var assembly = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(info.GetString("assemblyName")));
                    ResourceManager = ExchangeResourceManager.GetResourceManager(@string, assembly);
                    Id = info.GetString("id");
                    if (ResourceManager.GetString(Id) == null)
                        ResourceManager = null;
                    else {
                        DeserializedFallback = str;
                        try {
                            stringId = info.GetString("stringId");
                            this.ShowStringIdInUIIfError = info.GetBoolean("showStringIdInUIIfError");
                            this.ShowAssistanceInfoInUIIfError = info.GetBoolean("showAssistanceInfoInUIIfError");
                        } catch (System.Runtime.Serialization.SerializationException ex) {
                            stringId = null;
                            this.ShowStringIdInUIIfError = false;
                            this.ShowAssistanceInfoInUIIfError = false;
                        }
                    }
                }
            } catch (System.Runtime.Serialization.SerializationException ex) {
                ResourceManager = null;
            } catch (System.IO.FileNotFoundException ex) {
                ResourceManager = null;
            } catch (System.IO.FileLoadException ex) {
                ResourceManager = null;
            } catch (System.Resources.MissingManifestResourceException ex) {
                ResourceManager = null;
            }
            if (ResourceManager != null)
                return;
            Id = str;
        }

        public bool IsEmpty => null == Id;
        public string FullId => (ResourceManager != null ? ResourceManager.BaseName : string.Empty) + Id;
        public int BaseId => this.FullId.GetHashCode();

        public string StringId {
            get {
                if (stringId != null)
                    return stringId;
                return string.Empty;
            }
        }

        public bool ShowStringIdInUIIfError { get; }
        public bool ShowAssistanceInfoInUIIfError { get; }
        public System.Collections.ObjectModel.ReadOnlyCollection<object> FormatParameters { get; }

        public bool Equals(LocalizedString that) {
            if (!string.Equals(Id, that.Id, StringComparison.OrdinalIgnoreCase) || !string.Equals(stringId, that.stringId, StringComparison.OrdinalIgnoreCase) ||
                (this.ShowStringIdInUIIfError != that.ShowStringIdInUIIfError || this.ShowAssistanceInfoInUIIfError != that.ShowAssistanceInfoInUIIfError) ||
                (null != ResourceManager ^ null != that.ResourceManager || ResourceManager != null && !ResourceManager.Equals(that.ResourceManager)) || null != Inserts ^ null != that.Inserts)
                return false;
            if (Inserts != null && that.Inserts != null) {
                if (Inserts.Length != that.Inserts.Length)
                    return false;
                for (var index = 0; index < Inserts.Length; ++index) {
                    if (null != Inserts[index] ^ null != that.Inserts[index] || Inserts[index] != null && that.Inserts[index] != null && !Inserts[index].Equals(that.Inserts[index]))
                        return false;
                }
            }
            return true;
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider) {
            if (this.IsEmpty)
                return string.Empty;
            format = ResourceManager != null ? ResourceManager.GetString(Id, formatProvider as System.Globalization.CultureInfo) : Id;
            if (Inserts == null || Inserts.Length <= 0)
                return format;
            var objArray = new object[Inserts.Length];
            for (var index = 0; index < Inserts.Length; ++index) {
                var badObject = Inserts[index];
                var obj = !(badObject is ILocalizedString) ? LocalizedString.TranslateObject(badObject, formatProvider) : ((ILocalizedString) badObject).LocalizedString;
                objArray[index] = obj;
            }
            try {
                return string.Format(formatProvider, format, objArray);
            } catch (FormatException ex) {
                if (DeserializedFallback != null)
                    return string.Format(formatProvider, DeserializedFallback, objArray);
                throw;
            }
        }

        LocalizedString ILocalizedString.LocalizedString => this;

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            object[] objArray = null;
            if (Inserts != null && Inserts.Length > 0) {
                objArray = new object[Inserts.Length];
                for (var index = 0; index < Inserts.Length; ++index) {
                    var badObject = Inserts[index];
                    if (badObject != null) {
                        if (badObject is ILocalizedString)
                            badObject = ((ILocalizedString) badObject).LocalizedString;
                        else if (!IntrospectionExtensions.GetTypeInfo(badObject.GetType())
                                                         .IsSerializable && !(badObject is System.Runtime.Serialization.ISerializable)) {
                            var obj = LocalizedString.TranslateObject(badObject, System.Globalization.CultureInfo.InvariantCulture);
                            badObject = obj != badObject ? obj : badObject.ToString();
                        }
                    }
                    objArray[index] = badObject;
                }
            }
            info.AddValue("inserts", objArray);
            if (ResourceManager != null) {
                info.AddValue("baseName", ResourceManager.BaseName);
                info.AddValue("assemblyName", ResourceManager.AssemblyName);
                info.AddValue("id", Id);
                info.AddValue("stringId", stringId);
                info.AddValue("showStringIdInUIIfError", this.ShowStringIdInUIIfError);
                info.AddValue("showAssistanceInfoInUIIfError", this.ShowAssistanceInfoInUIIfError);
                if (DeserializedFallback == null)
                    info.AddValue("fallback", ResourceManager.GetString(Id, System.Globalization.CultureInfo.InvariantCulture));
                else
                    info.AddValue("fallback", DeserializedFallback);
            } else {
                if (DeserializedFallback == null)
                    info.AddValue("fallback", Id);
                else
                    info.AddValue("fallback", DeserializedFallback);
                info.AddValue("baseName", string.Empty);
            }
        }

        public static implicit operator string(LocalizedString value) {
            return value.ToString();
        }

        public static bool operator ==(LocalizedString s1, LocalizedString s2) {
            return s1.Equals(s2);
        }

        public static bool operator !=(LocalizedString s1, LocalizedString s2) {
            return !s1.Equals(s2);
        }

        public static LocalizedString Join(object separator, object[] value) {
            if (value == null || value.Length == 0)
                return Empty;
            if (separator == null)
                separator = string.Empty;
            var inserts = new object[value.Length + 1];
            inserts[0] = separator;
            Array.Copy(value, 0, inserts, 1, value.Length);
            var stringBuilder = new System.Text.StringBuilder(6*value.Length);
            stringBuilder.Append("{");
            for (var index = 1; index < value.Length; ++index) {
                stringBuilder.Append(index);
                stringBuilder.Append("}{0}{");
            }
            stringBuilder.Append((string) (object) value.Length + (object) "}");
            return new LocalizedString(stringBuilder.ToString(), inserts);
        }

        public LocalizedString RecreateWithNewParams(params object[] inserts) {
            return new LocalizedString(Id, this.StringId, this.ShowStringIdInUIIfError, this.ShowAssistanceInfoInUIIfError, ResourceManager, inserts);
        }

        public override string ToString() {
            return ((IFormattable) this).ToString(null, null);
        }

        public string ToString(IFormatProvider formatProvider) {
            return ((IFormattable) this).ToString(null, formatProvider);
        }

        public override int GetHashCode() {
            var num = (Id != null ? Id.GetHashCode() : 0) ^ (ResourceManager != null ? ResourceManager.GetHashCode() : 0);
            if (Inserts != null) {
                for (var index = 0; index < Inserts.Length; ++index) {
                    num = num ^ index ^ (Inserts[index] != null ? Inserts[index].GetHashCode() : 0);
                }
            }
            return num;
        }

        public override bool Equals(object obj) {
            if (obj is LocalizedString)
                return this.Equals((LocalizedString) obj);
            return false;
        }

        private static object TranslateObject(object badObject, IFormatProvider formatProvider) {
            if (badObject is Exception)
                return ((Exception) badObject).Message;
            return badObject;
        }

        public static readonly LocalizedString Empty = new LocalizedString();
        private readonly string DeserializedFallback;
        internal readonly string Id;
        private readonly object[] Inserts;
        private readonly ExchangeResourceManager ResourceManager;
        private readonly string stringId;

    }

}