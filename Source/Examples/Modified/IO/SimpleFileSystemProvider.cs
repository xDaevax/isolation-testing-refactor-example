using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestingDependencyIsolation.Modified.IO {
    /// <summary>
    /// Type that provides very simple read access to the file system.
    /// </summary>
    public class SimpleFileSystemProvider : IFileSystemProvider {
        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFileSystemProvider"/> class.
        /// </summary>
        public SimpleFileSystemProvider() {
        } // end default constructor

        #endregion

        #region --Functions--

        /// <summary>
        /// Attempts to build a fully qualified path to a file based on the given <paramref name="parts"/>.
        /// </summary>
        /// <param name="parts">The parts of a path to build.</param>
        /// <returns>A fully qualified file-system path</returns>
        public virtual string BuildPath(params string[] parts) {
            return Path.Combine(parts);
        } // end function BuildPath

        /// <summary>
        /// Determins whether or not the given path exists.
        /// </summary>
        /// <param name="itemType">The <see cref="FileSystemType"/> that specifies whether the given path is a directory or a file.</param>
        /// <param name="path">The path to check</param>
        /// <returns>true if the given <paramref name="path"/> exists; false otherwise</returns>
        public virtual bool Exists(FileSystemType itemType, string path) {
            bool returnValue = false;
            if (!string.IsNullOrWhiteSpace(path)) {
                switch (itemType) {
                    case FileSystemType.File:
                        returnValue = File.Exists(path);
                        break;
                    case FileSystemType.Directory:
                        returnValue = Directory.Exists(path);
                        break;
                    default:
                        break;
                }
            }

            return returnValue;
        } // end function Exists

        /// <summary>
        /// Returns an enumerable list of <see cref="FileInfo"/> that exist in the given <paramref name="path"/> and have contents matching the specified <paramref name="criteria"/>.
        /// </summary>
        /// <param name="path">The directory path containing the files to find</param>
        /// <param name="criteria">The file type / name fitlers used to limit results.</param>
        /// <returns>An enumerable set of <see cref="FileInfo"/> if the criteria is matched; an empty set otherwise</returns>
        public virtual IEnumerable<FileInfo> FindFiles(string path, string criteria) {
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetFiles(criteria).AsEnumerable();
        } // end function FindFiles

        /// <summary>
        /// Returns a <see cref="DirectoryInfo"/> instance for the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The fully qualified directory path</param>
        /// <returns>A new <see cref="DirectoryInfo"/> instance if the path was found; null if the path was not found</returns>
        public virtual DirectoryInfo FindDirectory(string path) {
            DirectoryInfo returnValue = null;
            if (this.Exists(FileSystemType.Directory, path)) {
                returnValue = new DirectoryInfo(path);
            }

            return returnValue;
        } // end function FindDirectory

        /// <summary>
        /// Opens the file at the given <paramref name="fileName"/> and returns the read-only stream.
        /// It is important when using this method to ENSURE that the stream is disposed of properly when complete.
        /// </summary>
        /// <param name="fileName">The name of the file to open.</param>
        /// <returns>A <see cref="FileStream"/> instance that allows the contents of the given file to be read.</returns>
        public virtual FileStream OpenRead(string fileName) {
            FileStream fs = null;
            try {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            } catch (Exception) {
                if (fs != null) {
                    try {
                        fs.Close();
                    } catch (Exception) {
                        // Do Nothing
                    }
                }
            }

            return fs;
        } // end function OpenRead

        #endregion
    } // end class SimpleFileSystemProvider
} // end namespace
