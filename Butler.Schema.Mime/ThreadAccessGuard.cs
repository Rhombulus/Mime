using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal class ThreadAccessGuard : IDisposable {

        private ThreadAccessGuard(ObjectThreadAccessToken token) {}

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal static IDisposable EnterPublic(ObjectThreadAccessToken token) {
            return null;
        }

        internal static IDisposable EnterPrivate(ObjectThreadAccessToken token) {
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