using NUnit.Framework;
using TestingDependencyIsolation.Modified.Reporting;

namespace TestingDependencyIsolation.Modified.Tests.Unit.Reporting {
    [TestFixture, Category("Unit"), Description("TestFixture that contains methods used to test the functionality of the ReportSettings type")]
    public class ReportSettingsFixture {
        #region --Tests--

        [Test, Category("Unit"), Category("Regression")]
        public void ReportSettings_FileNameSet_AssignedNull_ShouldNotAssignValue() {
            // ARRANGE
            ReportSettings settings = new ReportSettings();

            // ACT
            settings.FileName = null;

            // ASSERT
            Assert.IsEmpty(settings.FileName, "The null assignment should not have been allowed and the constructor default should have been used.");
        } // end test

        #endregion
    } // end class ReportSettingsFixture
} // end namespace
