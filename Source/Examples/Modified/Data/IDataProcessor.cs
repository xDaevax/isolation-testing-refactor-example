using System.Collections.Generic;
using System.IO;
using TestingDependencyIsolation.Modified.Core;

namespace TestingDependencyIsolation.Modified.Data {
    /// <summary>
    /// Interface that exposes functionality of a type that can process data from an input source for reporting.
    /// </summary>
    public interface IDataProcessor {
        #region --Functions--

        /// <summary>
        /// Parses the given <paramref name="data"/> into a set of strongly-typed <see cref="DataItem"/> instances.
        /// </summary>
        /// <param name="data">The raw data to process</param>
        /// <returns>A new <see cref="IEnumerable{DataItem}"/> instance with the parsed results of the <paramref name="data"/>.</returns>
        IEnumerable<DataItem> ParseData(string data);

        /// <summary>
        /// Parses the given <paramref name="data"/> stream into a set of strongly-typed <see cref="DataItem"/> instances.
        /// </summary>
        /// <param name="data">The raw data stream to process</param>
        /// <returns>A new <see cref="IEnumerable{DataItem}"/> instance with the parsed results of the <paramref name="data"/> stream.</returns>
        IEnumerable<DataItem> ParseData(Stream data);

        /// <summary>
        /// Attempts to safely parse the given <paramref name="data"/> with the results stored in the <paramref name="results"/> parameter.
        /// This method follows the conventional Try{some operation} method behavior and will not throw exceptions.
        /// </summary>
        /// <param name="data">The raw data to process</param>
        /// <param name="results">The strongly typed set of <see cref="DataItem"/> instances that resulted from the parsing if successful; null if it was not.</param>
        /// <returns>true if the data was successfully parsed; false otherwise</returns>
        bool TryParseData(string data, out IEnumerable<DataItem> results);

        /// <summary>
        /// Attempts to safely parse the given <paramref name="data"/> stream with the results stored in the <paramref name="results"/> parameter.
        /// This method follows the conventional Try{some operation} method behavior and will not throw exceptions.
        /// </summary>
        /// <param name="data">The raw data stream to process</param>
        /// <param name="results">The strongly typed set of <see cref="DataItem"/> instances that resulted from the parsing if successful; null if it was not.</param>
        /// <returns>true if the data was successfully parsed; false otherwise</returns>
        bool TryParseData(Stream data, out IEnumerable<DataItem> results);

        #endregion
    } // end interface IDataProcessor
} // end namespace
