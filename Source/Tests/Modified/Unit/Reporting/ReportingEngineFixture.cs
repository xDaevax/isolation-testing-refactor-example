using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using TestingDependencyIsolation.Modified.Configuration;
using TestingDependencyIsolation.Modified.Core;
using TestingDependencyIsolation.Modified.Data;
using TestingDependencyIsolation.Modified.IO;
using TestingDependencyIsolation.Modified.Reporting;

namespace TestingDependencyIsolation.Modified.Tests.Unit.Reporting {
    [TestFixture, Category("Unit"), TestOf(typeof(ReportingEngine)), Description("TestFixture that contains methods used to test the functionality of the ReportingEngine type")]
    public class ReportingEngineFixture {
        #region --Fields--

        private Mock<IConfigurationProvider> _mockConfigurationProvider;
        private Mock<IDataProcessor> _mockDataProcessor;
        private Mock<IFileSystemProvider> _mockFileSystemProvider;
        private Mock<IReportViewer> _mockReportViewer;

        #endregion

        #region --Methods--

        [SetUp]
        protected void Setup() {
            this._mockConfigurationProvider = new Mock<IConfigurationProvider>();
            this._mockDataProcessor = new Mock<IDataProcessor>();
            this._mockFileSystemProvider = new Mock<IFileSystemProvider>();
            this._mockReportViewer = new Mock<IReportViewer>();
        } // end test setup

        [TearDown]
        protected void TearDown() {
            this._mockConfigurationProvider = null;
            this._mockDataProcessor = null;
            this._mockFileSystemProvider = null;
            this._mockReportViewer = null;
        } // end test teardown

        #endregion

        #region --Functions--

        protected ReportingEngine CreateEngineUsingFakes() {
            ReportingEngine engine = null;

            engine = new ReportingEngine(this._mockConfigurationProvider.Object, this._mockDataProcessor.Object, this._mockFileSystemProvider.Object, this._mockReportViewer.Object);

            return engine;
        } // end function CreateEngineUsingFakes

        #endregion

        #region --Tests--

        [Test, Category("Unit"), TestOf("LoadReportFilePath")]
        public void ReportingEngine_LoadReportFilePath_WithValidConfigEntry_ShouldLoadFullPath() {
            // ARRANGE
            ReportingEngine engine = null;
            string resultPath = string.Empty;
            string expectedPath = Path.Combine(Environment.CurrentDirectory, "myfile.txt");

            this._mockConfigurationProvider.Setup(x => x.HasKey("ReportFile")).Returns(true);
            this._mockConfigurationProvider.Setup(x => x.GetValue<string>("ReportFile")).Returns("myfile.txt");
            this._mockFileSystemProvider.Setup(x => x.BuildPath(It.IsAny<string[]>())).Returns(expectedPath);

            engine = this.CreateEngineUsingFakes();

            // ACT
            resultPath = engine.LoadReportFilePath();

            // ASSERT
            Assert.IsNotEmpty(resultPath, "The path should have been returned.");
            Assert.AreEqual(expectedPath, resultPath, "The returned path wasn't what it should have been.");
        } // end test

        [Test, Category("Unit"), TestOf("LoadReportFilePath")]
        public void ReportingEngine_LoadReportFilePath_MissingAppConfigKey_ShouldThrowException() {
            // ARRANGE
            ReportingEngine engine = null;
            string resultPath = string.Empty;

            this._mockConfigurationProvider.Setup(x => x.HasKey("ReportFile")).Returns(false);

            engine = this.CreateEngineUsingFakes();

            // ACT
            // No act because we're checking an exception

            // ASSERT
            Assert.Throws<ArgumentException>(() => resultPath = engine.LoadReportFilePath(), "The method should have thrown an exception.");
            Assert.IsEmpty(resultPath, "No path should have been assigned.");
        } // end test

        [Test, Category("Unit"), TestOf("PrepareReportData")]
        public void ReportingEngine_PrepareReportData_WithValidInput_ShouldReturnReportData() {
            // ARRANGE
            ReportingEngine engine = null;
            var fakeReportSettings = Mock.Of<ReportSettings>(); // Special Moq function that lets us automatically create a mock object, and invoke it's .Object property.  This is especially useful when we don't care about the behavior of the mocked object really.
            IEnumerable<DataItem> resultData;
            bool safeModeEnabled = false;

            this._mockConfigurationProvider.Setup(x => x.TryGetValue<bool>(It.IsAny<string>(), out safeModeEnabled)).Returns(false);
            this._mockFileSystemProvider.Setup(x => x.OpenRead(It.IsAny<string>())).Returns(new Mock<FileStream>("FakeFile", FileMode.Open) { CallBase = false }.Object);

            engine = this.CreateEngineUsingFakes();

            // ACT
            resultData = engine.PrepareReportData(fakeReportSettings);

            // ASSERT
        } // end test

        #endregion
    } // end class ReportingEngineFixture
} // end namespace
