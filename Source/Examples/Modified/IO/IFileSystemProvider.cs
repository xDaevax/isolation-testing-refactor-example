using System.Collections.Generic;
using System.IO;

namespace TestingDependencyIsolation.Modified.IO {
    /// <summary>
    /// Provider that abstracts most of the file-system interaction away from code.  Provides methods
    /// for interacting with the file system while removing direct dependencies.
    /// </summary>
    public interface IFileSystemProvider {
        #region --Functions--
        /// <summary>
        /// Attempts to build a fully qualified path to a file based on the given <paramref name="parts"/>.
        /// </summary>
        /// <param name="parts">The parts of a path to build.</param>
        /// <returns>A fully qualified file-system path</returns>
        string BuildPath(params string[] parts);

        /// <summary>
        /// Returns an enumerable list of <see cref="FileInfo"/> that exist in the given <paramref name="path"/> and have contents matching the specified <paramref name="criteria"/>.
        /// </summary>
        /// <param name="path">The directory path containing the files to find</param>
        /// <param name="criteria">The file type / name fitlers used to limit results.</param>
        /// <returns>An enumerable set of <see cref="FileInfo"/> if the criteria is matched; an empty set otherwise</returns>
        IEnumerable<FileInfo> FindFiles(string path, string criteria);

        /// <summary>
        /// Returns a <see cref="DirectoryInfo"/> instance for the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The fully qualified directory path</param>
        /// <returns>A new <see cref="DirectoryInfo"/> instance if the path was found; null if the path was not found</returns>
        DirectoryInfo FindDirectory(string path);

        /// <summary>
        /// Opens the file at the given <paramref name="fileName"/> and returns the read-only stream.
        /// It is important when using this method to ENSURE that the stream is disposed of properly when complete.
        /// </summary>
        /// <param name="fileName">The name of the file to open.</param>
        /// <returns>A <see cref="FileStream"/> instance that allows the contents of the given file to be read.</returns>
        FileStream OpenRead(string fileName);

        /// <summary>
        /// Determins whether or not the given path exists.
        /// </summary>
        /// <param name="itemType">The <see cref="FileSystemType"/> that specifies whether the given path is a directory or a file.</param>
        /// <param name="path">The path to check</param>
        /// <returns>true if the given <paramref name="path"/> exists; false otherwise</returns>
        bool Exists(FileSystemType itemType, string path);
        #endregion
    } // end interface IFileSystemProvider
} // end namespace
