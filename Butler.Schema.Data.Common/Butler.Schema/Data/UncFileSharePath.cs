using System;
using System.Linq;

namespace Butler.Schema.Data {

    [Serializable]
    public sealed class UncFileSharePath : LongPath {

        public string ShareName { get; private set; }

        public new static UncFileSharePath Parse(string pathName) {
            return (UncFileSharePath) LongPath.ParseInternal(pathName, new UncFileSharePath());
        }

        public static bool TryParse(string path, out UncFileSharePath resultObject) {
            resultObject = (UncFileSharePath) LongPath.TryParseInternal(path, new UncFileSharePath());
            return null != resultObject;
        }

        protected override bool ParseCore(string path, bool nothrow) {
            if (base.ParseCore(path, nothrow)) {
                if (!this.IsUnc) {
                    this.IsValid = false;
                    if (!nothrow)
                        throw new ArgumentException(CtsResources.DataStrings.ErrorUncPathMustBeUncPath(path), nameof(path));
                } else {
                    if (path.Length >= 260)
                        throw new System.IO.PathTooLongException(CtsResources.DataStrings.ErrorUncPathTooLong(path));
                    var match = _regexUncShare.Match(this.PathName);
                    if (!match.Success) {
                        this.IsValid = false;
                        if (!nothrow)
                            throw new ArgumentException(CtsResources.DataStrings.ErrorUncPathMustBeUncPathOnly(path), nameof(path));
                    } else {
                        System.Net.IPAddress address;
                        if (System.Net.IPAddress.TryParse(match.Groups[1].ToString(), out address)) {
                            this.IsValid = false;
                            if (!nothrow)
                                throw new ArgumentException(CtsResources.DataStrings.ErrorUncPathMustUseServerName(path), nameof(path));
                        }
                        this.ShareName = match.Groups[2].ToString();
                    }
                    var success = _regexLeadingWhitespace.Match(this.PathName)
                                                         .Success;
                    if (_regexTrailingWhitespace.Match(this.PathName)
                                                .Success || success) {
                        this.IsValid = false;
                        if (!nothrow)
                            throw new ArgumentException(CtsResources.DataStrings.ConstraintViolationNoLeadingOrTrailingWhitespace, nameof(path));
                    }
                }
            }
            return this.IsValid;
        }

        private const int MaxPath = 260;
        private static readonly System.Text.RegularExpressions.Regex _regexUncShare = new System.Text.RegularExpressions.Regex(@"^\\\\([^\\]+)\\([^\\]+)$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        private static readonly System.Text.RegularExpressions.Regex _regexLeadingWhitespace = new System.Text.RegularExpressions.Regex(@"^\s+", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        private static readonly System.Text.RegularExpressions.Regex _regexTrailingWhitespace = new System.Text.RegularExpressions.Regex(@"\s+$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    }

}