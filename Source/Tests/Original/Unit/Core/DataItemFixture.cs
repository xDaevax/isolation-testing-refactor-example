using System;
using NUnit.Framework;
using TestingDependencyIsolation.Original.ExternalDependencies.Core;

namespace TestingDependencyIsolation.Original.Tests.Unit.Core {
    /// <summary>
    /// A fixture (particularly when referring to unit tests) is a semi-standard way of referring to tests that share a similar setup routine.
    /// By grouping tests with similar setup, the code and complexity required to test those methods can be greatly reduced.
    /// Typically, a fixture is named according to the type it is testing suffixed with the word: "Fixture".
    /// </summary>
    [TestFixture, Category("Unit"), Description("TestFixture that contains methods used to test the functionality of the DataItem type.")]
    public class DataItemFixture {
        #region --Tests--

        /// <summary>
        /// The name of the test is significant as it should be descriptive of what is being tested and what is expected.
        /// I opt for the following format:
        /// {TypeName}_{MethodName}_(Given|With|When){ArgumentsOrConditions}_(Should|Does|Then){ExpectedResult)
        /// </summary>
        /// <remarks>
        /// This type of test is referred to as a "state" test.  It isn't concerned with the underlying behavior of the SUT (indeed, there is no behavior in this case)
        /// rather, it is concerned with the outputs only or the "state" of the program only.
        /// 
        /// In reality, you would most likely not bother with a constructor default test unless there were specific conditions that must
        /// not be changed.  This is the only test in this fixture because checking the setters of the properties is not an effective use
        /// of testing time.
        /// </remarks>
        [Test, Category("Unit")]
        public void DataItem_Constructor_WithNoParameters_ShouldInitializePropertyDefaults() {
            // ARRANGE
            DataItem item = null; // Perform any dependency or parameter setup.  In this case, farily minimal

            // ACT
            item = new DataItem(); // ACT is usually only one line and executes the SUT (System Under Test) that we are interested in

            // ASSERT
            // In NUnit, an assertion is a truth statement that either does nothing, or throws an exception if the assertion fails
            // The exception is handled by the NUnit framework, and the test that threw the exception is marked as having "failed" or "not passed".
            // Tests that run with no exceptions (regardless of whether or not they have Assert statements) are marked as "passed".
            // This behavior illustrates the IMPORTANCE of clearly defining your SUT and writing appropriate Assert statements
            // That is one of the key differences between good test coverage and bad test coverage that leads to false senses of security.
            Assert.IsEmpty(item.Data, "The default for data should be an empty string"); // Using the .IsEmpty syntax (good for strings and collections)
            Assert.That(item.Operation == string.Empty, "The operation should be empty by default"); // Using the .That syntax which is one of the most flexible options for more complex assertions
            Assert.AreEqual(DateTime.MinValue, item.TimeStamp, "The TimeStamp property should default to DateTime.MinValue"); // AreEqual tests for equality using the default equality comparer available to the type
            Assert.True(item.Severity == ItemSeverity.INFO, "The default value for the Severity property should be INFO.");  // Use the boolean true, most commonly used to simplify boolean assertions
        } // end test

        #endregion
    } // end class DataItemFixture
} // end namespace
