namespace Butler.Schema.Data.Internal {

    internal abstract class RefCountable : System.IDisposable {

        protected RefCountable() {
            _refCount = 1;
        }

        protected bool IsDisposed { get; private set; }
        public int RefCount => _refCount;

        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected void ThrowIfDisposed() {
            if (this.IsDisposed)
                throw new System.ObjectDisposedException("RefCountable");
        }

        public void AddRef() {
            this.ThrowIfDisposed();
            System.Threading.Interlocked.Increment(ref _refCount);
        }

        public void Release() {
            this.ThrowIfDisposed();
            if (System.Threading.Interlocked.Decrement(ref _refCount) != 0)
                return;
            this.Dispose();
        }

        protected virtual void Dispose(bool disposing) {
            this.IsDisposed = true;
        }

        private int _refCount;

    }

}