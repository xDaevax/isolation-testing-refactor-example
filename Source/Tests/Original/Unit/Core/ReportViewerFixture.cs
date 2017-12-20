using NUnit.Framework;
using TestingDependencyIsolation.Original.ExternalDependencies.Core;

namespace TestingDependencyIsolation.Original.Tests.Unit.Core {
    [TestFixture, Category("Unit"), Description("TestFixture used to test the functionality of the ReportViewer type.")]
    public class ReportViewerFixture {
        #region --Tests--

        [Test, Category("Unit"), Ignore("This code can't be unit-tested due to MANY reasons.")]
        public void ReportViewer_Constructor_WithNoArguments_ShouldLoadConfigAndInitializeReportFileAndParseContents() {
            // ARRANGE
            ReportViewer viewer = null;

            // ACT
            viewer = new ReportViewer();

            // ASSERT
        } // end test

        #endregion
    } // end class ReportViewerFixture
} // end test
