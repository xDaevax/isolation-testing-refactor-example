using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using TestingDependencyIsolation.Modified.Core;
using TestingDependencyIsolation.Modified.Reporting;

namespace TestingDependencyIsolation.Modified.Tests.Unit.Reporting {
    [TestFixture, Category("Unit"), TestOf(typeof(ConsoleReportViewer)), Description("Fixture that contains methods used to test the functionality of the ConsoleReportViewer type.")]
    public class ConsoleReportViewerFixture {
        #region --Fields--

        private Mock<TextWriter> _mockTextWriter;

        #endregion

        #region --Methods--

        [SetUp]
        protected void Setup() {
            this._mockTextWriter = new Mock<TextWriter>() { CallBase = false };
        } // end method Setup

        [TearDown]
        protected void Teardown() {
            this._mockTextWriter = null;
        } // end method Teardown

        #endregion

        #region --Tests--

        [Test, Category("Unit"), Category("Reporting"), TestOf("SortContents")]
        public void ConsoleReportViewer_SortContents_OrderAscendingWithContents_ShouldReturnSortedContents() {
            // ARRANGE
            ConsoleReportViewer viewer = null;
            List<DataItem> fakeUnorderedData = new List<DataItem>();
            Mock<ReportSettings> mockReportSettings = new Mock<ReportSettings>() { CallBase = false };
            IEnumerable<DataItem> results = null;

            mockReportSettings.Setup(x => x.SortAscending).Returns(true);

            // Make sure the first entry is the newest to make it easier to verify the sorting took place
            var mockFirstItem = new Mock<DataItem>() { CallBase = false };
            mockFirstItem.Setup(x => x.TimeStamp).Returns(DateTime.Now.AddDays(-1));
            mockFirstItem.Setup(x => x.Operation).Returns("Test 1");
            var mockSecondItem = new Mock<DataItem>() { CallBase = false };
            mockSecondItem.Setup(x => x.TimeStamp).Returns(DateTime.Now.AddDays(-2));
            mockSecondItem.Setup(x => x.Operation).Returns("Test 2");

            fakeUnorderedData.Add(mockFirstItem.Object);
            fakeUnorderedData.Add(mockSecondItem.Object);

            viewer = new ConsoleReportViewer(this._mockTextWriter.Object);

            // ACT
            results = viewer.SortContents(mockReportSettings.Object, fakeUnorderedData);

            // ASSERT
            Assert.NotNull(results, "The SortContents method should have returned results.");
            Assert.AreEqual(2, results.Count(), "The method didn't return the same number of results that went in.");
            Assert.AreEqual("Test 2", results.FirstOrDefault().Operation, "The sort ascending did not take place.");
        } // end test

        [Test, Category("Unit"), Category("Reporting"), TestOf("SortContents")]
        public void ConsoleReportViewer_SortContents_OrderDescendingWithContents_ShouldReturnSortedContents() {
            // ARRANGE
            ConsoleReportViewer viewer = null;
            List<DataItem> fakeUnorderedData = new List<DataItem>();
            Mock<ReportSettings> mockReportSettings = new Mock<ReportSettings>() { CallBase = false };
            IEnumerable<DataItem> results = null;

            mockReportSettings.Setup(x => x.SortAscending).Returns(false);

            // Make sure the first entry is the oldest to make it easier to verify the sorting took place
            var mockFirstItem = new Mock<DataItem>() { CallBase = false };
            mockFirstItem.Setup(x => x.TimeStamp).Returns(DateTime.Now.AddDays(-4));
            mockFirstItem.Setup(x => x.Operation).Returns("Test 1");
            var mockSecondItem = new Mock<DataItem>() { CallBase = false };
            mockSecondItem.Setup(x => x.TimeStamp).Returns(DateTime.Now.AddDays(-2));
            mockSecondItem.Setup(x => x.Operation).Returns("Test 2");

            fakeUnorderedData.Add(mockFirstItem.Object);
            fakeUnorderedData.Add(mockSecondItem.Object);

            viewer = new ConsoleReportViewer(this._mockTextWriter.Object);

            // ACT
            results = viewer.SortContents(mockReportSettings.Object, fakeUnorderedData);

            // ASSERT
            Assert.NotNull(results, "The SortContents method should have returned results.");
            Assert.AreEqual(2, results.Count(), "The method didn't return the same number of results that went in.");
            Assert.AreEqual("Test 2", results.FirstOrDefault().Operation, "The sort descending did not take place.");
        } // end test

        #endregion
    } // end class ConsoleReportViewerFixture
} // end namespace
