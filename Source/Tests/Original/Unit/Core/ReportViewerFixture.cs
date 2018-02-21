using NUnit.Framework;
using TestingDependencyIsolation.Original.ExternalDependencies.Core;

namespace TestingDependencyIsolation.Original.Tests.Unit.Core {
    [TestFixture, Category("Unit"), TestOf(typeof(ReportViewer)), Description("TestFixture used to test the functionality of the ReportViewer type.")]
    public class ReportViewerFixture {
        #region --Tests--

        [Test, Category("Unit"), TestOf("ReportViewer"), Ignore("For MANY reasons, this code cannot be unit-tested. Most of these are outlined at the end of the class file for the ReportViewer type.")]
        public void ReportViewer_Constructor_WithNoArguments_ShouldLoadConfigAndInitializeReportFileAndParseContents() {
            // ARRANGE
            ReportViewer viewer = null;

            // ACT
            viewer = new ReportViewer();

            // ASSERT
        } // end test

        #endregion
    } // end class ReportViewerFixture
} // end namespace
