namespace Butler.Schema.Internal {

    internal class CtsConfigurationSetting {

        internal CtsConfigurationSetting(string name) {
            this.Name = name;
            this.Arguments = new System.Collections.Generic.List<CtsConfigurationArgument>();
        }

        public string Name { get; }
        public System.Collections.Generic.IList<CtsConfigurationArgument> Arguments { get; }

        internal void AddArgument(string name, string value) {
            this.Arguments.Add(new CtsConfigurationArgument(name, value));
        }

    }

}