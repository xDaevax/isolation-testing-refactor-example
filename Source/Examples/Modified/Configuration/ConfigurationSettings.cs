namespace TestingDependencyIsolation.Modified.Configuration {
    /// <summary>
    /// Type that provides static access to the names of configuration items to avoid magic strings throughout the library.
    /// </summary>
    public static class ConfigurationSettings {
        #region --Static Fields--

        /// <summary>
        /// Gets the key that stores the path to the report file.
        /// </summary>
        public static string ReportFile = "ReportFile";

        /// <summary>
        /// Gets the key that stores information about whether or not safe mode is enabled.
        /// </summary>
        public static string SafeMode = "SafeMode_Enabled";

        #endregion
    } // end class ConfigurationSettings
} // end namespace
