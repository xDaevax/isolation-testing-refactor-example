using System;
using System.IO;
using NUnit.Framework;
using TestingDependencyIsolation.Modified.IO;

namespace TestingDependencyIsolation.Modified.Tests.Integration.IO {
    [TestFixture, TestOf(typeof(SimpleFileSystemProvider)), Category("Integration"), Category("IO"), Description("Fixture used to test the functionality of the SimpleFileSystemProvider type as well as it's interaction with the file system.")]
    public class SimpleFileSystemProviderFixture {
        #region --Constants--

        private static string TestFileName = "aFile.txt";
        private static bool SkipTests = true;

        #endregion

        #region --Fields--

        private string _targetFileName;

        #endregion

        #region --Methods--

        [OneTimeSetUp]
        protected void OneTimeSetup() {
            string workingPath = TestContext.CurrentContext.TestDirectory;
            string targetFilePath = Path.Combine(workingPath, TestFileName);
            TestContext.Progress.WriteLine("Performing One-time setup for the file system with file: {0}.", targetFilePath);
            try {
                var exists = File.Exists(targetFilePath);
                // If we're here, we have permission to read from the FS
                if (exists) {
                    // file already exists, could have been a cleanup failure of some kind
                    TestContext.Progress.WriteLine("The file: " + targetFilePath + " wasn't cleaned up after the previous test run and will be overwritten.");
                }

                using (var stream = File.Create(targetFilePath)) { // See if we have write access
                   // Don't write anything yet, just see if we can
                }

                if (!exists && File.Exists(targetFilePath)) {
                    // We successfully created the file where it didn't exist before, so we can continue with tests.
                    SkipTests = false;
                    this._targetFileName = targetFilePath;
                }
            } catch(Exception ex) {
                TestContext.Progress.WriteLine("Unable to move forward with tests.  One-time setup of File System failed.");
                TestContext.Progress.WriteLine("OneTimeSetup Failure: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            }
        } // end method OneTimeSetup

        [OneTimeTearDown]
        protected void OneTimeTeardown() {
            try {
                File.Delete(this._targetFileName);
                TestContext.Progress.WriteLine("OneTimeTeardown completed successfully.");
                this._targetFileName = string.Empty;
            } catch(Exception ex) {
                TestContext.Progress.WriteLine("Unable to delete the file: {0}.{1}{2}{3}{4}", this._targetFileName, Environment.NewLine, ex.Message, Environment.NewLine, ex.StackTrace);
            }
        } // end method OneTimeTeardown

        [SetUp]
        protected void Setup() {
            if (!SkipTests) {
                File.WriteAllLines(this._targetFileName, new string[] { "Line 1", "Line 2", "Line 3" });
            } 
        } // end method Setup

        [TearDown]
        protected void Teardown() {
            if (!SkipTests) {
                using (var stream = File.Create(this._targetFileName)) { // Overwrites and "clears the contents of the file
                }
            }
        } // end method Teardown

        #endregion

        #region --Tests--

        [Test, Category("Integration"), Category("IO"), TestOf("BuildPath")]
        public void SimpleFileSystemProvider_BuildPathGivenValidArguments_ShouldBuildCombinedFilePath() {
            SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();
            string directory = TestContext.CurrentContext.TestDirectory;
            string combinedResult = string.Empty;

            combinedResult = sfs.BuildPath(directory, TestFileName);

            Assert.AreEqual(this._targetFileName, combinedResult, "The combined path wasn't built correctly.");
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("Exists")]
        public void SimpleFileSystemProvider_Exists_WithValidDirectory_ShouldReturnTrue() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();
                string directory = TestContext.CurrentContext.TestDirectory;

                bool exists = sfs.Exists(FileSystemType.Directory, directory);

                Assert.True(exists, "The directory should have been found and the method should have returned true.");
            }
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("Exists")]
        public void SimpleFileSystemProvider_Exists_WithValidFile_ShouldReturnTrue() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();

                bool exists = sfs.Exists(FileSystemType.File, this._targetFileName);

                Assert.True(exists, "The file should have existed and the method should have returned true.");
            }
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("Exists")]
        public void SimpleFileSystemProvider_Exists_GivenInvalidDirectory_ShouldReturnFalse() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();
                string directory = TestContext.CurrentContext.TestDirectory + "123";

                bool exists = sfs.Exists(FileSystemType.Directory, directory);

                Assert.False(exists, "The directory should not have been found and the method should have returned false.");
            }
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("Exists")]
        public void SimpleFileSystemProvider_Exists_GivenInvalidFile_ShouldReturnFalse() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();

                bool exists = sfs.Exists(FileSystemType.File, this._targetFileName + "test");

                Assert.False(exists, "The file should not have existed and the method should have returned false.");
            }
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("OpenRead")]
        public void SimpleFileSystemProvider_OpenRead_WithValidFile_ShouldReturnFileContentsAsStream() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();
                Stream fileStream = null;
                string expectedContents = string.Concat("Line 1", Environment.NewLine, "Line 2", Environment.NewLine, "Line 3", Environment.NewLine);
                string actualContents = string.Empty;
                using(var reader = new StreamReader(fileStream = sfs.OpenRead(this._targetFileName))) {
                    actualContents = reader.ReadToEnd();
                }

                Assert.AreEqual(expectedContents, actualContents, "The method did not open or read the file correctly.");
            }
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("OpenRead")]
        public void SimpleFileSystemProvider_OpenRead_WithNoFile_ShouldSwallowExceptionAndReturnNullStream() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();
                Stream fileStream = null;

                fileStream = sfs.OpenRead(string.Empty);

                Assert.IsNull(fileStream, "No stream should have been returned for a file that doesn't exist.");
            }
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("FindDirectory")]
        public void SimpleFileSystemProvider_FindDirectory_WithValidDirectory_ShouldReturnDirectoryInfo() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();
                DirectoryInfo info = null;

                info = sfs.FindDirectory(TestContext.CurrentContext.TestDirectory);

                Assert.NotNull(info, "Directory information should have been returned.");
                Assert.AreEqual(TestContext.CurrentContext.TestDirectory, info.ToString(), "The directory was not the same as the one requested.");
            }
        } // end test

        [Test, Category("Integration"), Category("IO"), TestOf("FindDirectory")]
        public void SimpleFileSystemProvider_FindDirectory_WithInvalidDirectory_ShouldNotReturnDirectoryInfo() {
            if (SkipTests) {
                Assert.Inconclusive("Problems with one-time setup prevented the test from running.");
            } else {
                SimpleFileSystemProvider sfs = new SimpleFileSystemProvider();
                DirectoryInfo info = null;

                info = sfs.FindDirectory("Empty");

                Assert.Null(info, "Directory information should have been returned.");
            }
        } // end test

        #endregion
    } // end class SimpleFileSystemProviderFixture
} // end namespace
