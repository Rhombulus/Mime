namespace Butler.Schema.Internal {

    internal abstract class StreamOnDataStorage : System.IO.Stream {

        public abstract DataStorage Storage { get; }
        public abstract long Start { get; }
        public abstract long End { get; }

    }

}