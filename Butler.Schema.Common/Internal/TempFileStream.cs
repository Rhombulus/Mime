namespace Butler.Schema.Internal {

    internal class TempFileStream : System.IO.FileStream {

        private TempFileStream(Microsoft.Win32.SafeHandles.SafeFileHandle handle)
            : base(handle, System.IO.FileAccess.ReadWrite) {}

        internal static string Path => TempFileStream.GetTempPath();
        public string FilePath { get; private set; }

        public static TempFileStream CreateInstance() {
            return TempFileStream.CreateInstance("Cts");
        }

        public static TempFileStream CreateInstance(string prefix) {
            return TempFileStream.CreateInstance(prefix, true);
        }

        public static TempFileStream CreateInstance(string prefix, bool deleteOnClose) {
            var securityAttributes = new NativeMethods.SecurityAttributes(false);
            var path = TempFileStream.Path;
            new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Write, path).Demand();
            var error = 0;
            var num1 = 10;
            string str;
            Microsoft.Win32.SafeHandles.SafeFileHandle file;
            do {
                var num2 = (uint) System.Threading.Interlocked.Increment(ref nextId);
                str = System.IO.Path.Combine(path, prefix + num2.ToString("X5") + ".tmp");
                var num3 = deleteOnClose ? 67108864U : 0U;
                file = NativeMethods.CreateFile(str, 1180063U, 0U, ref securityAttributes, 1U, (uint) (256 | (int) num3 | 8192), System.IntPtr.Zero);
                --num1;
                if (file.IsInvalid) {
                    error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    if (error == 80)
                        ++num1;
                    using (var currentProcess = System.Diagnostics.Process.GetCurrentProcess())
                        System.Threading.Interlocked.Add(ref nextId, currentProcess.Id);
                } else
                    num1 = 0;
            } while (num1 > 0);
            if (file.IsInvalid) {
                var fileFailed = Resources.SharedStrings.CreateFileFailed(str);
                throw new System.IO.IOException(fileFailed, new System.ComponentModel.Win32Exception(error, fileFailed));
            }
            return new TempFileStream(file) {
                FilePath = str
            };
        }

        internal static void SetTemporaryPath(string path) {
            tempPath = path;
        }

        private static string GetTempPath() {
            return tempPath ?? (tempPath = System.IO.Path.GetTempPath());
        }

        protected override void Dispose(bool disposing) {
            try {
                base.Dispose(disposing);
            } catch (System.IO.IOException ex) {}
        }

        private static int nextId = System.Environment.TickCount ^ System.Diagnostics.Process.GetCurrentProcess()
                                                                         .Id;

        private static string tempPath;

    }

}