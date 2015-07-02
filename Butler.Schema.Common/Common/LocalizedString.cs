using System.Linq;
namespace Butler.Schema.Data.Common {

    [System.Serializable]
    public struct LocalizedString : System.Runtime.Serialization.ISerializable, ILocalizedString, System.IFormattable, System.IEquatable<LocalizedString> {

        public LocalizedString(string id, ExchangeResourceManager resourceManager, params object[] inserts) {
            this = new LocalizedString(id, null, false, false, resourceManager, inserts);
        }

        public LocalizedString(string id, string stringId, bool showStringIdIfError, bool showAssistanceInfoIfError, ExchangeResourceManager resourceManager, params object[] inserts) {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));
            if (resourceManager == null)
                throw new System.ArgumentNullException(nameof(resourceManager));
            Id = id;
            this._stringId = stringId;
            this.ShowStringIdInUIIfError = showStringIdIfError;
            this.ShowAssistanceInfoInUIIfError = showAssistanceInfoIfError;
            _resourceManager = resourceManager;
            _deserializedFallback = null;
            _inserts = inserts == null || inserts.Length <= 0 ? null : inserts;
            this.FormatParameters = _inserts != null ? new System.Collections.ObjectModel.ReadOnlyCollection<object>(_inserts) : null;
        }

        public LocalizedString(string value) {
            Id = value;
            _stringId = null;
            this.ShowStringIdInUIIfError = false;
            this.ShowAssistanceInfoInUIIfError = false;
            _inserts = null;
            _resourceManager = null;
            _deserializedFallback = null;
            this.FormatParameters = null;
        }

        private LocalizedString(string format, object[] inserts) {
            Id = format;
            _stringId = null;
            this.ShowStringIdInUIIfError = false;
            this.ShowAssistanceInfoInUIIfError = false;
            _inserts = inserts;
            _resourceManager = null;
            _deserializedFallback = null;
            this.FormatParameters = _inserts != null ? new System.Collections.ObjectModel.ReadOnlyCollection<object>(_inserts) : null;
        }

        private LocalizedString(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            _inserts = (object[]) info.GetValue("inserts", typeof (object[]));
            this.FormatParameters = _inserts != null ? new System.Collections.ObjectModel.ReadOnlyCollection<object>(_inserts) : null;
            _resourceManager = null;
            Id = null;
            _stringId = null;
            this.ShowStringIdInUIIfError = false;
            this.ShowAssistanceInfoInUIIfError = false;
            _deserializedFallback = null;
            string str = null;
            try {
                var @string = info.GetString("baseName");
                str = info.GetString("fallback");
                if (!string.IsNullOrEmpty(@string)) {
                    var assembly = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(info.GetString("assemblyName")));
                    _resourceManager = ExchangeResourceManager.GetResourceManager(@string, assembly);
                    Id = info.GetString("id");
                    if (_resourceManager.GetString(Id) == null)
                        _resourceManager = null;
                    else {
                        _deserializedFallback = str;
                        try {
                            _stringId = info.GetString("stringId");
                            this.ShowStringIdInUIIfError = info.GetBoolean("showStringIdInUIIfError");
                            this.ShowAssistanceInfoInUIIfError = info.GetBoolean("showAssistanceInfoInUIIfError");
                        } catch (System.Runtime.Serialization.SerializationException ex) {
                            _stringId = null;
                            this.ShowStringIdInUIIfError = false;
                            this.ShowAssistanceInfoInUIIfError = false;
                        }
                    }
                }
            } catch (System.Runtime.Serialization.SerializationException ex) {
                _resourceManager = null;
            } catch (System.IO.FileNotFoundException ex) {
                _resourceManager = null;
            } catch (System.IO.FileLoadException ex) {
                _resourceManager = null;
            } catch (System.Resources.MissingManifestResourceException ex) {
                _resourceManager = null;
            }
            if (_resourceManager != null)
                return;
            Id = str;
        }

        public bool IsEmpty => null == Id;
        public string FullId => (_resourceManager != null ? _resourceManager.BaseName : string.Empty) + Id;
        public int BaseId => this.FullId.GetHashCode();

        public string StringId {
            get {
                if (_stringId != null)
                    return _stringId;
                return string.Empty;
            }
        }

        public bool ShowStringIdInUIIfError { get; }
        public bool ShowAssistanceInfoInUIIfError { get; }
        public System.Collections.ObjectModel.ReadOnlyCollection<object> FormatParameters { get; }

        public bool Equals(LocalizedString that) {
            if (!string.Equals(Id, that.Id, System.StringComparison.OrdinalIgnoreCase) || !string.Equals(_stringId, that._stringId, System.StringComparison.OrdinalIgnoreCase) ||
                (this.ShowStringIdInUIIfError != that.ShowStringIdInUIIfError || this.ShowAssistanceInfoInUIIfError != that.ShowAssistanceInfoInUIIfError) ||
                (null != _resourceManager ^ null != that._resourceManager || _resourceManager != null && !_resourceManager.Equals(that._resourceManager)) || null != _inserts ^ null != that._inserts)
                return false;
            if (_inserts == null || that._inserts == null)
                return true;
            if (_inserts.Length != that._inserts.Length)
                return false;
            return !_inserts.Where((t, index) => null != t ^ null != that._inserts[index] || t != null && that._inserts[index] != null && !t.Equals(that._inserts[index]))
                           .Any();
        }

        string System.IFormattable.ToString(string format, System.IFormatProvider formatProvider) {
            if (this.IsEmpty)
                return string.Empty;
            format = _resourceManager != null ? _resourceManager.GetString(Id, formatProvider as System.Globalization.CultureInfo) : Id;
            if (_inserts == null || _inserts.Length <= 0)
                return format;
            var objArray = new object[_inserts.Length];
            for (var index = 0; index < _inserts.Length; ++index) {
                var badObject = _inserts[index];
                var obj = (badObject as ILocalizedString)?.LocalizedString ?? LocalizedString.TranslateObject(badObject, formatProvider);
                objArray[index] = obj;
            }
            try {
                return string.Format(formatProvider, format, objArray);
            } catch (System.FormatException ex) {
                if (_deserializedFallback != null)
                    return string.Format(formatProvider, _deserializedFallback, objArray);
                throw;
            }
        }

        LocalizedString ILocalizedString.LocalizedString => this;

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            object[] objArray = null;
            if (_inserts != null && _inserts.Length > 0) {
                objArray = new object[_inserts.Length];
                for (var index = 0; index < _inserts.Length; ++index) {
                    var badObject = _inserts[index];
                    if (badObject != null) {
                        if (badObject is ILocalizedString)
                            badObject = ((ILocalizedString) badObject).LocalizedString;
                        else if (!System.Reflection.IntrospectionExtensions.GetTypeInfo(badObject.GetType())
                                        .IsSerializable && !(badObject is System.Runtime.Serialization.ISerializable)) {
                            var obj = LocalizedString.TranslateObject(badObject, System.Globalization.CultureInfo.InvariantCulture);
                            badObject = obj != badObject ? obj : badObject.ToString();
                        }
                    }
                    objArray[index] = badObject;
                }
            }
            info.AddValue("inserts", objArray);
            if (_resourceManager != null) {
                info.AddValue("baseName", _resourceManager.BaseName);
                info.AddValue("assemblyName", _resourceManager.AssemblyName);
                info.AddValue("id", Id);
                info.AddValue("stringId", _stringId);
                info.AddValue("showStringIdInUIIfError", this.ShowStringIdInUIIfError);
                info.AddValue("showAssistanceInfoInUIIfError", this.ShowAssistanceInfoInUIIfError);
                info.AddValue("fallback", _deserializedFallback ?? _resourceManager.GetString(Id, System.Globalization.CultureInfo.InvariantCulture));
            } else {
                info.AddValue("fallback", _deserializedFallback ?? Id);
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
            System.Array.Copy(value, 0, inserts, 1, value.Length);
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
            return new LocalizedString(Id, this.StringId, this.ShowStringIdInUIIfError, this.ShowAssistanceInfoInUIIfError, _resourceManager, inserts);
        }

        public override string ToString() {
            return ((System.IFormattable) this).ToString(null, null);
        }

        public string ToString(System.IFormatProvider formatProvider) {
            return ((System.IFormattable) this).ToString(null, formatProvider);
        }

        public override int GetHashCode() {
            var num = (Id?.GetHashCode() ?? 0) ^ (_resourceManager?.GetHashCode() ?? 0);
            if (_inserts == null)
                return num;
            for (var index = 0; index < _inserts.Length; ++index) {
                num = num ^ index ^ (_inserts[index] != null ? _inserts[index].GetHashCode() : 0);
            }
            return num;
        }

        public override bool Equals(object obj) {
            if (obj is LocalizedString)
                return this.Equals((LocalizedString) obj);
            return false;
        }

        private static object TranslateObject(object badObject, System.IFormatProvider formatProvider) {
            if (badObject is System.Exception)
                return ((System.Exception) badObject).Message;
            return badObject;
        }

        public static readonly LocalizedString Empty = new LocalizedString();
        private readonly string _deserializedFallback;
        internal readonly string Id;
        private readonly object[] _inserts;
        private readonly ExchangeResourceManager _resourceManager;
        private readonly string _stringId;

    }

}