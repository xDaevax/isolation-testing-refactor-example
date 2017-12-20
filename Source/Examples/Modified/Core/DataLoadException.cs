using System;

namespace TestingDependencyIsolation.Modified.Core {
    /// <summary>
    /// Exception thrown when report data cannot be loaded.
    /// </summary>
    [Serializable]
    public class DataLoadException : Exception {
        #region --Fields--

        private string _dataSource;

        #endregion

        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadException"/> class.
        /// </summary>
        public DataLoadException() : this (string.Empty) {
        } // end default constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadException"/> class.
        /// </summary>
        /// <param name="message">The message describing the reason for the exception.</param>
        public DataLoadException(string message) : this(message, null as string) {
        } // end overloaded constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadException"/> class.
        /// </summary>
        /// <param name="message">The message describing the reason for the exception.</param>
        /// <param name="dataSource">The data source that was being loaded when the exception occurred.</param>
        public DataLoadException(string message, string dataSource) : this(message, dataSource, null) {
        } // end overloaded constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadException"/> class.
        /// </summary>
        /// <param name="message">The message describing the reason for the exception.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this exception to be thrown.</param>
        public DataLoadException(string message, Exception innerException) : this(message, string.Empty, innerException) {
        } // end overloaded constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadException"/> class.
        /// </summary>
        /// <param name="message">The message describing the reason for the exception.</param>
        /// <param name="dataSource">The data source that was being loaded when the exception occurred.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this exception to be thrown.</param>
        public DataLoadException(string message, string dataSource, Exception innerException) : base(message, innerException) {
            this._dataSource = dataSource;
        } // end overloaded constructor

        #endregion

        #region --Properties--

        /// <summary>
        /// Gets or sets the data source that was being loaded when this exception was thrown.
        /// </summary>
        public string DataSource {
            get {
                return this._dataSource;
            } set {
                this._dataSource = value;
            }
        } // end property DataSource

        /// <summary>
        /// Gets a message describing the exception.
        /// </summary>
        public override string Message {
            get {
                return string.Concat(base.Message, " Data Source: ", this.DataSource);
            }
        } // end property Message

        #endregion
    } // end class DataLoadException
} // end namespace
