namespace Butler.Schema.Internal {

    internal interface IApplicationServices {

        System.IO.Stream CreateTemporaryStorage();
        System.Collections.Generic.IList<CtsConfigurationSetting> GetConfiguration(string subSectionName);
        void RefreshConfiguration();
        void LogConfigurationErrorEvent();

    }

}