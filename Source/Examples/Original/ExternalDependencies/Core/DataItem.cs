using System;

namespace TestingDependencyIsolation.Original.ExternalDependencies.Core {
    /// <summary>
    /// Class that represents an individual item in a source file being reported on.
    /// </summary>
    public class DataItem {
        #region --Fields--

        private string _data;
        private string _operation;
        private ItemSeverity _severity;
        private DateTime _timeStamp;

        #endregion

        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItem"/> class.
        /// </summary>
        public DataItem() {
            this._data = string.Empty;
            this._operation = string.Empty;
            this._severity = ItemSeverity.INFO;
            this._timeStamp = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Local);
        } // end default constructor

        #endregion

        #region --Properties--

        /// <summary>
        /// Gets or sets the actual data or detail of the item.
        /// </summary>
        public virtual string Data {
            get {
                return this._data;
            } set {
                this._data = value;
            }
        } // end property Data

        /// <summary>
        /// Gets or sets the operation that was recorded for this item.
        /// </summary>
        public virtual string Operation {
            get {
                return this._operation;
            } set {
                this._operation = value;
            }
        } // end property Operation

        /// <summary>
        /// Gets or sets the severity of action that was performed.
        /// </summary>
        public virtual ItemSeverity Severity {
            get {
                return this._severity;
            } set {
                this._severity = value;
            }
        } // end property Severity

        /// <summary>
        /// Gets or sets the date and time that the item occurred.
        /// </summary>
        public virtual DateTime TimeStamp {
            get {
                return this._timeStamp;
            } set {
                this._timeStamp = value;
            }
        } // end property TimeStamp

        #endregion
    } // end class DataItem
} // end namespace
