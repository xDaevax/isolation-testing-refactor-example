using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TestingDependencyIsolation.Modified.Core;

namespace TestingDependencyIsolation.Modified.Data {
    /// <summary>
    /// Type used to read data from an input stream and parse it into strongly typed items.  This type assumes a text file
    /// is being processed.  Introduce additional types if different processing instructions or different file formats are needed.
    /// </summary>
    public class TextFileProcessor : IDataProcessor {
        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFileProcessor"/> class.
        /// </summary>
        public TextFileProcessor() {
        } // end default constructor

        #endregion

        #region --Functions--

        public virtual DataItem ParseLine(string line) {
            if (string.IsNullOrWhiteSpace(line)) {
                throw new ArgumentNullException(nameof(line), "No line information provided to parse.");
            }

            DataItem returnValue = null;
            var headerData = LineParsing.HeaderPattern.Match(line).Value; // matches anything in the form of [ any number of any character / space followed by ]
            DateTime timeStamp = DateTime.Parse(LineParsing.TimestampPattern.Match(headerData).Value);
            string severity = Enum.GetNames(typeof(ItemSeverity)).FirstOrDefault(x => headerData.Contains(x)); // Load the header value matching the enum name
            var operation = headerData.Split(new string[] { severity }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("]", string.Empty).Replace(" - ", string.Empty); // load the second half of the split removing the ending ]
            if (timeStamp != DateTime.MinValue && !string.IsNullOrWhiteSpace(severity) && !string.IsNullOrWhiteSpace(operation)) {
                var contents = string.Concat(line.Split(new string[] { "]" }, StringSplitOptions.None)[1], string.Empty);
                returnValue = new DataItem() {
                    Severity = (ItemSeverity)Enum.Parse(typeof(ItemSeverity), severity), // Parse it from a string to the actual enum
                    TimeStamp = timeStamp,
                    Data = contents.Trim(),
                    Operation = operation
                };
            } else {
                throw new DataLoadException(string.Format("Invalid line data found at: {0}", line));
            }

            return returnValue;
        } // end function ParseLine

        public virtual IEnumerable<DataItem> ParseData(string data) {
            if(string.IsNullOrWhiteSpace(data)) {
                throw new DataLoadException("No data provided to parse.");
            }

            List<DataItem> returnValue = new List<DataItem>();

            if (data.Contains(LineParsing.LineDelimeter)) {
                var splitData = data.Split(new string[] { LineParsing.LineDelimeter }, StringSplitOptions.RemoveEmptyEntries);
                splitData.ToList().ForEach(x => {
                    returnValue.Add(this.ParseLine(x));
                });
            } else {
                returnValue.Add(this.ParseLine(data));
            }

            return returnValue.AsEnumerable();
        } // end overloaded function ParseData

        public virtual IEnumerable<DataItem> ParseData(Stream data) {
            IEnumerable<DataItem> returnValue = null;
            string contents = string.Empty;
            using (var reader = new StreamReader(data)) {
                contents = reader.ReadToEnd();
                returnValue = this.ParseData(contents);
            }

            return returnValue;
        } // end overloaded function ParseData

        public bool TryParseData(string data, out IEnumerable<DataItem> results) {
            bool returnValue = false;

            try {
                results = this.ParseData(data);
                returnValue = true;
            } catch(Exception) {
                results = null;
            }

            return returnValue;
        } // end overloaded function TryParseData

        public bool TryParseData(Stream data, out IEnumerable<DataItem> results) {
            bool returnValue = false;
            try {
                results = this.ParseData(data);
                returnValue = true;
            } catch (Exception) {
                results = null;
            }

            return returnValue;
        } // end overloaded function TryParseData

        #endregion

        #region --Nested Types--

        internal static class LineParsing {
            public static Regex HeaderPattern = new Regex(@"\[.+\]");
            public static Regex TimestampPattern = new Regex(@"(?!\[)+.+\.\d+");
            public static string LineDelimeter = Environment.NewLine; // TODO: This could be configurable if we wanted to
        } // end class LineParsing

        #endregion
    } // end class TextFileProcessor
} // end namespace
