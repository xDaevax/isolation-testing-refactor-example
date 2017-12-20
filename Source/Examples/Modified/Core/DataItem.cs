using System;

namespace TestingDependencyIsolation.Modified.Core {
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

        public DataItem() {
            this._data = string.Empty;
            this._operation = string.Empty;
            this._severity = ItemSeverity.INFO;
            this._timeStamp = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Local);
        } // end default constructor

        #endregion

        #region --Properties--

        public virtual string Data {
            get {
                return this._data;
            } set {
                this._data = value;
            }
        }

        public virtual string Operation {
            get {
                return this._operation;
            } set {
                this._operation = value;
            }
        }

        public virtual ItemSeverity Severity {
            get {
                return this._severity;
            } set {
                this._severity = value;
            }
        }

        public virtual DateTime TimeStamp {
            get {
                return this._timeStamp;
            } set {
                this._timeStamp = value;
            }
        }

        #endregion
    } // end class DataItem
} // end namespace
