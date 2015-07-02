namespace Butler.Schema.Data.Internal {

    internal class CtsConfigurationArgument {

        internal CtsConfigurationArgument(string name, string value) {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }
        public string Value { get; }

    }

}