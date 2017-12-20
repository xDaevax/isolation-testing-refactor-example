using NUnit.Framework;
using TestingDependencyIsolation.Modified.Core;

namespace TestingDependencyIsolation.Modified.Tests.Unit.Core {
    [TestFixture, Category("Unit"), Description("TestFixture used to test the functionality of the DataLoadException type")]
    public class DataLoadExceptionFixture {
        #region --Tests--

        [Test, Category("Unit")]
        public void DataLoadException_OverloadedConstructor_ShouldAssignDataSourceProperty() {
            // ARRANGE
            DataLoadException ex = null;
            DataLoadException ex2 = null;
            string expectedMessage = "Unit Test";

            // ACT
            ex = new DataLoadException("Test Data Load Exception", "Unit Test", null);
            ex2 = new DataLoadException("Test Data Load Exception", "Unit Test");

            // ASSERT
            Assert.NotNull(ex, "The constructor failed to initialize an instance of the exception.");
            Assert.NotNull(ex2, "The constructor failed to initialize an instance of the exception.");
            Assert.AreEqual(expectedMessage, ex.DataSource, "The data source property was not initialized properly.");
            Assert.AreEqual(expectedMessage, ex2.DataSource, "The data source property was not initialized properly.");
        } // end test

        [Test, Category("Unit")]
        public void DataLoadException_Message_ShouldContainDataSource() {
            // ARRANGE
            string expectedDataSource = "Unit Test";
            DataLoadException ex = new DataLoadException("An exception", expectedDataSource);
            string expectedMessage = "An exception Data Source: " + expectedDataSource;
            string actualMessage = string.Empty;

            // ACT
            actualMessage = ex.Message;

            // ASSERT
            Assert.That(ex.Message.Contains(expectedDataSource), "The data source was not contained in the message.");
            Assert.AreEqual(expectedMessage, actualMessage, "The message didn't match what was expected.");
        } // end test

        #endregion
    } // end class DataLoadExceptionFixture
} // end namespace
