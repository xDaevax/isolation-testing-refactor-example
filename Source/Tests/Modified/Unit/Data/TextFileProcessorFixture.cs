using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TestingDependencyIsolation.Modified.Core;
using TestingDependencyIsolation.Modified.Data;

namespace TestingDependencyIsolation.Modified.Tests.Unit.Data {
    [TestFixture, Category("Unit"), TestOf(typeof(TextFileProcessor)), Description("TestFixture that contains methods used to test the functionality of the TextFileProcessor type")]
    public class TextFileProcessorFixture {
        #region --Tests--
        
        [Test, Category("Unit"), Category("Regression"), TestOf("ParseLine")]
        public void TextFileProcessor_ParseLine_WithEmptyString_ShouldThrowException() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();

            // ACT
            // No act as we're checking an exception

            // ASSERT
            Assert.Throws<ArgumentNullException>(() => processor.ParseLine(string.Empty), "The method should have thrown an exception.");
        } // end test

        [Test, Category("Unit"), Category("Regression"), TestOf("ParseData")]
        public void TextFileProcessor_ParseData_GivenEmptyStream_ShouldThrowException() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            Stream emptyStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(string.Empty));

            // ACT
            // No act as we're checking an exception

            // ASSERT
            Assert.Throws<DataLoadException>(() => processor.ParseData(emptyStream), "The method should have thrown an exception.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseData")]
        public void TextFileProcessor_ParseData_GivenValidSingleLineStream_ShouldReturnParsedLine() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            Stream fakeStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("[2017-12-13 09:42:23.244 INFO - Data Updated] Something fun happened."));

            // ACT
            results = processor.ParseData(fakeStream);

            // ASSERT
            Assert.AreEqual(1, results.Count(), "There should only have been one result.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseData")]
        public void TextFileProcessor_ParseData_GivenValidMultiLineStream_ShouldReturnParsedLines() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            Stream fakeStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("[2017-12-13 09:42:23.244 INFO - Data Updated] Something fun happened." + Environment.NewLine + "[2017-12-13 11:26:19.583 INFO - Data Updated] Something different happened later."));

            // ACT
            results = processor.ParseData(fakeStream);

            // ASSERT
            Assert.AreEqual(2, results.Count(), "There should have been exactly two results.");
        } // end test

        [Test, Category("Unit"), Category("Regression"), TestOf("ParseLine")]
        public void TextFileProcessor_ParseLine_GivenStringWithInvalidDateFormat_ShouldThrowException() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            string fakeString = "[2017-12-13 09:df:23.244 INFO - Data Updated]";

            // ACT
            // No act as we're checking an exception

            // ASSERT
            Assert.Throws<FormatException>(() => processor.ParseLine(fakeString), "The method should have thrown an exception.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseLine")]
        public void TextFileProcessor_ParseLine_GivenStringWithMissingSeverity_ShouldThrowException() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            string fakeString = "[2017-12-13 09:14:23.244 - Data Updated]";

            // ACT
            // No act as we're checking an exception

            // ASSERT
            Assert.Throws<IndexOutOfRangeException>(() => processor.ParseLine(fakeString), "The method should have thrown an exception.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseLine")]
        public void TextFileProcessor_ParseLine_GivenStringWithMissingOperation_ShouldThrowException() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            string fakeString = "[2017-12-13 09:df:23.244 INFO]";

            // ACT
            // No act as we're checking an exception

            // ASSERT
            Assert.Throws<FormatException>(() => processor.ParseLine(fakeString), "The method should have thrown an exception.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseLine")]
        public void TextFileProcessor_ParseLine_GivenValidStringWithNoMessage_ShouldBuildObjectWithEmptyMessage() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            DataItem resultItem = null;
            string fakeString = "[2017-12-13 09:42:23.244 INFO - Data Updated]";

            // ACT
            resultItem = processor.ParseLine(fakeString);

            // ASSERT
            Assert.NotNull(resultItem, "The function should have built a data item.");
            Assert.IsEmpty(resultItem.Data, "No data should have been populated.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseLine")]
        [TestCase("Data Updated", "2017-12-13 09:42:23.244", ItemSeverity.WARN, "Updated Volume to 3", Description = "Case 1")]
        [TestCase("Data Inserted", "2017-12-15 03:46:22.544", ItemSeverity.ERROR, "Added Diameter", Description = "Case 2")]
        public void TextFileProcessor_ParseLine_GivenValidEntry_ShouldBuildNewObjectWithValues(string inputOperation, string inputDateTime, ItemSeverity inputSeverity, string inputMessage) {
            // ARRANGE
            DateTime expectedTime = DateTime.Parse(inputDateTime);
            string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            TextFileProcessor processor = new TextFileProcessor();
            DataItem resultItem = null;
            string fakeString = string.Concat("[", expectedTime.ToString(timeFormat), " ", inputSeverity.ToString(), " - ", inputOperation, "] ", inputMessage);

            // ACT
            resultItem = processor.ParseLine(fakeString);

            // ASSERT
            Assert.NotNull(resultItem, "The method should have built a data item.");
            Assert.AreEqual(inputOperation, resultItem.Operation, "The operation was not built properly.");
            Assert.AreEqual(expectedTime.ToString(timeFormat), resultItem.TimeStamp.ToString(timeFormat), "The date / time was not built properly.");
            Assert.AreEqual(inputSeverity, resultItem.Severity, "The severity was not built properly.");
            Assert.AreEqual(inputMessage, resultItem.Data, "The data was not build properly.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseData")]
        public void TextFileProcessor_ParseData_GivenMultiLineString_ShouldBuildMultipleResults() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            string fakeString = "[2017-12-13 09:42:23.244 INFO - Data Updated] Something fun happened." + Environment.NewLine + "[2017-12-13 11:26:19.583 INFO - Data Updated] Something different happened later.";

            // ACT
            results = processor.ParseData(fakeString);

            // ASSERT
            Assert.AreEqual(2, results.Count(), "There should have been two results built from the input.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseData")]
        public void TextFileProcessor_ParseData_GivenMultiLineStringWithTrailingEmptyLine_ShouldTrimExtraLine() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            string fakeString = "[2017-12-13 09:42:23.244 INFO - Data Updated] Something fun happened." + Environment.NewLine + "[2017-12-13 11:26:19.583 INFO - Data Updated] Something different happened later." + Environment.NewLine;

            // ACT
            results = processor.ParseData(fakeString);

            // ASSERT
            Assert.AreEqual(2, results.Count(), "There should only be two results and the trailing newline ignored.");
        } // end test

        [Test, Category("Unit"), TestOf("ParseData")]
        public void TextFileProcessor_ParseData_GivenSingleLineString_ShouldReturnOneResultOnly() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            string fakeString = "[2017-12-13 09:42:23.244 INFO - Data Updated] Something fun happened.";

            // ACT
            results = processor.ParseData(fakeString);

            // ASSERT
            Assert.AreEqual(1, results.Count(), "There should only have been one result.");
        } // end test

        [Test, Category("Unit"), TestOf("TryParseData")]
        public void TextFileProcessor_TryParseData_WithInvalidStringInput_ShouldReturnFalseAndNullOutput() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            string fakeString = "[2017-12-13 09:42:2- Data Updated] Something fun happened.";
            bool actualResult;

            // ACT
            actualResult = processor.TryParseData(fakeString, out results);

            // ASSERT
            Assert.Null(results, "No results should have come back.");
            Assert.False(actualResult, "The result of the TryParse should have been false.");
        } // end test

        [Test, Category("Unit"), TestOf("TryParseData")]
        public void TextFileProcessor_TryParseData_WithInvalidStreamInput_ShouldReturnFalseAndNullOutput() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            Stream fakeStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("[2017-12-13 09:42:2- thing fun happened."));
            bool actualResult;

            // ACT
            actualResult = processor.TryParseData(fakeStream, out results);

            // ASSERT
            Assert.Null(results, "No results should have come back.");
            Assert.False(actualResult, "The result of the TryParse should have been false.");
        } // end test

        [Test, Category("Unit"), TestOf("TryParseData")]
        public void TextFileProcessor_TryParseData_WithValidStringInput_ShouldReturnTrueAndOutputResults() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            string fakeString = "[2017-12-13 09:42:23.244 INFO - Data Updated] Something fun happened.";
            bool actualResult;

            // ACT
            actualResult = processor.TryParseData(fakeString, out results);

            // ASSERT
            Assert.AreEqual(1, results.Count(), "One result should have come out of the method.");
            Assert.True(actualResult, "The result of the TryParse should have been true.");
        } // end test

        [Test, Category("Unit"), TestOf("TryParseData")]
        public void TextFileProcessor_TryParseData_WithValidStreamInput_ShouldReturnTrueAndOutputResults() {
            // ARRANGE
            TextFileProcessor processor = new TextFileProcessor();
            IEnumerable<DataItem> results = null;
            Stream fakeStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("[2017-12-13 09:42:23.244 INFO - Data Updated] Something fun happened."));
            bool actualResult;

            // ACT
            actualResult = processor.TryParseData(fakeStream, out results);

            // ASSERT
            Assert.AreEqual(1, results.Count(), "One result should have come out of the method.");
            Assert.True(actualResult, "The result of the TryParse should have been true.");
        } // end test

        #endregion
    } // end class TextFileProcessorFixture
} // end namespace
