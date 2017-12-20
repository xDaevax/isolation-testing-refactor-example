namespace TestingDependencyIsolation.Modified.Reporting {
    /// <summary>
    ///  Class used to store loaded or processed settings for loading / displaying a report.  This type helps to create a layer of separation
    ///  between the code that loads / presents the report and the code that gathers the data from the user.
    /// </summary>
    public class ReportSettings {
        #region --Fields--

        private string _fileName;
        private bool _sortAscending;

        #endregion

        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportSettings"/> class.
        /// </summary>
        public ReportSettings() {
            this._fileName = string.Empty;
            this._sortAscending = false;
        } // end default constructor

        #endregion

        #region --Properties--

        /// <summary>
        /// Gets or sets the full path of the report file.  Will not allow an empty string to be assigned.
        /// </summary>
        public virtual string FileName {
            get {
                return this._fileName;
            } set {
                if (!string.IsNullOrWhiteSpace(value)) {
                    this._fileName = value;
                }
            }
        } // end property FileName

        /// <summary>
        /// Gets or sets a value indicating whether the report data should be sorted in ascending or descending order based on time stamp.
        /// </summary>
        public virtual bool SortAscending {
            get {
                return this._sortAscending;
            } set {
                this._sortAscending = value;
            }
        } // end property SortAscending

        #endregion
    } // end class ReportSettings
} // end namespace
