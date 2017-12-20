using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestingDependencyIsolation.Modified.Configuration;
using TestingDependencyIsolation.Modified.Core;
using TestingDependencyIsolation.Modified.Data;
using TestingDependencyIsolation.Modified.IO;

namespace TestingDependencyIsolation.Modified.Reporting {
    /// <summary>
    /// Engine that is responsible for bringing all of the necessary elements together to simplify the process of printing a report.
    /// </summary>
    public class ReportingEngine {
        #region --Fields--

        private IConfigurationProvider _configurationProvider;
        private IDataProcessor _dataProcessor;
        private IFileSystemProvider _fileSystemProvider;
        private IReportViewer _reportViewer;

        #endregion

        #region --Constructors--

        /// <summary>
        /// Prevents a default instance of the <see cref="ReportingEngine"/> class from being created.
        /// </summary>
        private ReportingEngine() {
        } // end default constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingEngine"/> class.
        /// </summary>
        /// <param name="configurationProvider">The <see cref="IConfigurationProvider"/> instance used to load configuration data.</param>
        /// <param name="dataProcessor">The <see cref="IDataProcessor"/> instance used to process the raw data into a format the report can use.</param>
        /// <param name="fileSystemProvider">The <see cref="IFileSystemProvider"/> instance used to load files into memory.</param>
        /// <param name="reportViewer">The <see cref="IReportViewer"/> instance used to display reports.</param>
        public ReportingEngine(IConfigurationProvider configurationProvider, IDataProcessor dataProcessor, IFileSystemProvider fileSystemProvider, IReportViewer reportViewer) {
            this._configurationProvider = configurationProvider;
            this._dataProcessor = dataProcessor;
            this._fileSystemProvider = fileSystemProvider;
            this._reportViewer = reportViewer;
        } // end overloaded constructor

        #endregion

        #region --Methods--

        public void GenerateReport() {
            string reportFile = this.LoadReportFilePath();
            this.GenerateReport(reportFile);
        } // end overloaded method GenerateReport

        public void GenerateReport(string reportFileName) {
            if (string.IsNullOrWhiteSpace(reportFileName)) {
                throw new ArgumentNullException(nameof(reportFileName), "No report file specified to load.");
            }

            if (!this.FileSystemProvider.Exists(FileSystemType.File, reportFileName)) {
                throw new FileNotFoundException("Unable to find the specified report file.");
            }

            ReportSettings settings = new ReportSettings() { FileName = reportFileName };
            var reportData = this.PrepareReportData(settings);

            if (reportData != null && reportData.Any()) {
                this.ReportViewer.PrintReport(settings, reportData);
            } else {
                // TODO: Add Logging
            }
        } // end overloaded method GenerateReport

        #endregion

        #region --Functions--

        public string LoadReportFilePath() {
            string returnValue = string.Empty;

            if (!this.ConfigurationProvider.HasKey(ConfigurationSettings.ReportFile)) {
                throw new ArgumentException("Could not find the report file setting: " + ConfigurationSettings.ReportFile);
            }

            returnValue = this.FileSystemProvider.BuildPath(Environment.CurrentDirectory, this.ConfigurationProvider.GetValue<string>(ConfigurationSettings.ReportFile));
            
            return returnValue;
        } // end function LoadReportFilePath

        public IEnumerable<DataItem> PrepareReportData(ReportSettings settings) {
            IEnumerable<DataItem> reportData = null;
            bool safeModeEnabled = false;

            this.ConfigurationProvider.TryGetValue<bool>(ConfigurationSettings.SafeMode, out safeModeEnabled);
            if (safeModeEnabled) {
                try {
                    using (var stream = this.FileSystemProvider.OpenRead(settings.FileName)) {
                        if (!this.DataProcessor.TryParseData(stream, out reportData)) {
                            // TODO: Handle logging
                        }
                    }
                } catch(Exception) {
                    // TODO: Handle logging
                    // This catch is here because we're in safe-mode and we know that the OpenRead method can sometimes return null, which
                    // would cause some problems here.
                }
            } else {
                using (var stream = this.FileSystemProvider.OpenRead(settings.FileName)) {
                    try {
                        reportData = this.DataProcessor.ParseData(stream);
                    } catch (Exception) {
                        throw;
                    }
                }
            }

            return reportData;
        } // end function PrepareReportData

        #endregion

        #region --Properties--

        /// <summary>
        /// Gets the injected <see cref="IConfigurationProvider"/> instance used to load configuration data.
        /// </summary>
        protected IConfigurationProvider ConfigurationProvider {
            get {
                return this._configurationProvider;
            }
        } // end property ConfigurationProvider

        /// <summary>
        /// Gets the injected <see cref="IDataProcessor"/> instance used to parse raw data into a standard format for reporting.
        /// </summary>
        protected IDataProcessor DataProcessor {
            get {
                return this._dataProcessor;
            }
        } // end property DataProcessor

        /// <summary>
        /// Gets the injected <see cref="IFileSystemProvider"/> instance used to load files into memory.
        /// </summary>
        protected IFileSystemProvider FileSystemProvider {
            get {
                return this._fileSystemProvider;
            }
        } // end property FileSystemProvider

        /// <summary>
        /// Gets the injected <see cref="IReportViewer"/> instance used to display reports.
        /// </summary>
        protected IReportViewer ReportViewer {
            get {
                return this._reportViewer;
            }
        } // end property ReportViewer

        #endregion
    } // end class ReportingEngine
} // end namespace
