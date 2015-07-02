namespace Butler.Schema.Internal {

    internal static class InternalDebug {

        internal static bool UseSystemDiagnostics { get; set; }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Trace(long traceType, string format, params object[] traceObjects) {}

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string formatString) {}

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition) {}


        internal class DebugAssertionViolationException : System.Exception {

            public DebugAssertionViolationException() {}

            public DebugAssertionViolationException(string message)
                : base(message) {}

        }

    }

}