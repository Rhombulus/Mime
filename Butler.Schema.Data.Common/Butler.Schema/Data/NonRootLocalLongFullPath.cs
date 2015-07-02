using System;
using System.Linq;

namespace Butler.Schema.Data {

    [Serializable]
    public sealed class NonRootLocalLongFullPath : LocalLongFullPath {

        private NonRootLocalLongFullPath() {}

        public static NonRootLocalLongFullPath Parse(string pathName) {
            return (NonRootLocalLongFullPath) LongPath.ParseInternal(pathName, new NonRootLocalLongFullPath());
        }

        protected override bool ParseCore(string path, bool nothrow) {
            if (base.ParseCore(path, nothrow) && NonRootLocalLongFullPath.IsRootDirectory(this.PathName)) {
                this.IsValid = false;
                if (!nothrow)
                    throw new ArgumentException(CtsResources.DataStrings.ErrorPathCanNotBeRoot, nameof(path));
            }
            return this.IsValid;
        }

        private static bool IsRootDirectory(string path) {
            var flag = false;
            if (!string.IsNullOrEmpty(path))
                flag = path.Equals(System.IO.Path.GetPathRoot(path), StringComparison.OrdinalIgnoreCase);
            return flag;
        }

    }

}