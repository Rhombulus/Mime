using System.Linq;

namespace Butler.Schema.Mime {

    internal class ThreadAccessGuard : System.IDisposable {

        private ThreadAccessGuard(ObjectThreadAccessToken token) {}

        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        internal static System.IDisposable EnterPublic(ObjectThreadAccessToken token) {
            return null;
        }

        internal static System.IDisposable EnterPrivate(ObjectThreadAccessToken token) {
            return null;
        }

        protected virtual void Dispose(bool disposing) {
            if (isDisposed)
                return;
            isDisposed = true;
        }

        private bool isDisposed;

    }

}