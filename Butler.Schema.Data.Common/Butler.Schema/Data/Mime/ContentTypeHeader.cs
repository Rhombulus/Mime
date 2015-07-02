using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class ContentTypeHeader : ComplexHeader {

        public ContentTypeHeader()
            : base("Content-Type", HeaderId.ContentType) {
            value = "text/plain";
            type = "text";
            subType = "plain";
            parsed = true;
        }

        public ContentTypeHeader(string value)
            : base("Content-Type", HeaderId.ContentType) {
            this.Value = value;
        }

        public string MediaType {
            get {
                if (!parsed)
                    this.Parse();
                return type;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (ValueParser.ParseToken(value, 0, false) != value.Length)
                    throw new ArgumentException("Value should be valid MIME token", nameof(value));
                if (!parsed)
                    this.Parse();
                if (value == type)
                    return;
                var str = subType;
                this.SetRawValue(null, true);
                parsed = true;
                type = Header.NormalizeString(value);
                subType = str;
                this.value = ContentTypeHeader.ComposeContentTypeValue(type, subType);
                if (type != "multipart")
                    return;
                this.EnsureBoundary();
            }
        }

        public string SubType {
            get {
                if (!parsed)
                    this.Parse();
                return subType;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (ValueParser.ParseToken(value, 0, false) != value.Length)
                    throw new ArgumentException("Value should be valid MIME token", nameof(value));
                if (!parsed)
                    this.Parse();
                if (value == subType)
                    return;
                var str = type;
                this.SetRawValue(null, true);
                parsed = true;
                type = str;
                subType = Header.NormalizeString(value);
                this.value = ContentTypeHeader.ComposeContentTypeValue(type, subType);
            }
        }

        public override sealed string Value {
            get {
                if (!parsed)
                    this.Parse();
                return value;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                var length = ValueParser.ParseToken(value, 0, false);
                if (length == 0 || length > value.Length - 2 || (value[length] != 47 || ValueParser.ParseToken(value, length + 1, false) != value.Length - length - 1))
                    throw new ArgumentException("Value should be a valid content type in the form 'token/token'", nameof(value));
                if (!parsed)
                    this.Parse();
                if (value == this.value)
                    return;
                this.SetRawValue(null, true);
                parsed = true;
                this.value = Header.NormalizeString(value);
                type = Header.NormalizeString(this.value, 0, length);
                subType = Header.NormalizeString(this.value, length + 1, this.value.Length - length - 1);
                if (type != "multipart")
                    return;
                this.EnsureBoundary();
            }
        }

        internal bool IsMultipart {
            get {
                if (!parsed)
                    this.Parse();
                return type == "multipart";
            }
        }

        internal bool IsEmbeddedMessage {
            get {
                if (!parsed)
                    this.Parse();
                return value == "message/rfc822";
            }
        }

        internal bool IsAnyMessage {
            get {
                if (!parsed)
                    this.Parse();
                return type == "message";
            }
        }

        internal override byte[] RawValue {
            get {
                if (!parsed)
                    this.Parse();
                return Internal.ByteString.StringToBytes(value, false) ?? MimeString.EmptyByteArray;
            }
            set {
                base.RawValue = value;
                this.Parse();
                if (type != "multipart")
                    return;
                this.EnsureBoundary();
            }
        }

        public override sealed MimeNode Clone() {
            var contentTypeHeader = new ContentTypeHeader();
            this.CopyTo(contentTypeHeader);
            return contentTypeHeader;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var contentTypeHeader = destination as ContentTypeHeader;
            if (contentTypeHeader == null)
                throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType, nameof(destination));
            base.CopyTo(destination);
            contentTypeHeader.type = type;
            contentTypeHeader.subType = subType;
            contentTypeHeader.value = value;
            contentTypeHeader.parsed = parsed;
        }

        public override sealed bool IsValueValid(string value) {
            if (value == null)
                return false;
            var index = ValueParser.ParseToken(value, 0, false);
            return index != 0 && index <= value.Length - 2 && (value[index] == 47 && ValueParser.ParseToken(value, index + 1, false) == value.Length - index - 1);
        }

        internal static byte[] CreateBoundary() {
            var str = Guid.NewGuid()
                          .ToString();
            var bytes = new byte[str.Length + 2];
            bytes[0] = 95;
            Internal.ByteString.StringToBytes(str, bytes, 1, false);
            bytes[1 + str.Length] = 95;
            return bytes;
        }

        internal override void RawValueAboutToChange() {
            this.Reset();
        }

        internal override void ParseValue(ValueParser parser, bool storeValue) {
            var phrase = MimeStringList.Empty;
            parser.ParseCFWS(false, ref phrase, true);
            var mimeString1 = parser.ParseToken();
            var mimeString2 = MimeString.Empty;
            parser.ParseCFWS(false, ref phrase, true);
            switch (parser.ParseGet()) {
                case 47:
                    parser.ParseCFWS(false, ref phrase, true);
                    mimeString2 = parser.ParseToken();
                    goto case (byte) 0;
                case 0:
                    if (storeValue) {
                        type = mimeString1.Length != 0 ? Header.NormalizeString(mimeString1.Data, mimeString1.Offset, mimeString1.Length, false) : "text";
                        if (mimeString2.Length == 0) {
                            if (type == "multipart")
                                subType = "mixed";
                            else if (type == "text")
                                subType = "plain";
                            else {
                                type = "application";
                                subType = "octet-stream";
                            }
                        } else
                            subType = Header.NormalizeString(mimeString2.Data, mimeString2.Offset, mimeString2.Length, false);
                        value = ContentTypeHeader.ComposeContentTypeValue(type, subType);
                    }
                    if (type != "multipart")
                        break;
                    handleISO2022 = false;
                    break;
                default:
                    parser.ParseUnget();
                    goto case (byte) 0;
            }
        }

        internal override void AppendLine(MimeString line, bool markDirty) {
            if (parsed)
                this.Reset();
            base.AppendLine(line, markDirty);
        }

        private static string ComposeContentTypeValue(string type, string subType) {
            var num = type.Length + 1 + subType.Length;
            if (num >= 2 && num <= 32) {
                var index1 = MimeData.HashValueFinish(MimeData.HashValueAdd(MimeData.HashValueAdd(MimeData.HashValueAdd(0, type), "/"), subType));
                var index2 = MimeData.valueHashTable[index1];
                if (index2 > 0) {
                    do {
                        var str = MimeData.values[index2].value;
                        if (str.Length == num && str.StartsWith(type, StringComparison.OrdinalIgnoreCase) && (str[type.Length] == 47 && str.EndsWith(subType, StringComparison.OrdinalIgnoreCase)))
                            return str;
                        ++index2;
                    } while (MimeData.values[index2].hash == index1);
                }
            }
            return type + "/" + subType;
        }

        private void EnsureBoundary() {
            if (this["boundary"] != null)
                return;
            var mimeParameter = new MimeParameter("boundary");
            this.InternalAppendChild(mimeParameter);
            mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
        }

        private void Reset() {
            this.InternalRemoveAll();
            parsed = false;
            value = null;
            type = null;
            subType = null;
        }

        internal const bool AllowUTF8Value = false;
        internal const bool AllowUTF8Boundary = false;
        internal const bool AllowUTF8Charset = false;
        private string subType;
        private string type;
        private string value;

    }

}