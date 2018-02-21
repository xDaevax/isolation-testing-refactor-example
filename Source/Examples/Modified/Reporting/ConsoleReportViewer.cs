using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestingDependencyIsolation.Modified.Core;

namespace TestingDependencyIsolation.Modified.Reporting {
    /// <summary>
    /// Implementation of the <see cref="IReportViewer"/> interface that can print reports to a <see cref="TextWriter"/> output (often a Console.Output).
    /// </summary>
    public class ConsoleReportViewer : IReportViewer {
        #region --Fields--

        private TextWriter _output;

        #endregion

        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleReportViewer"/> class.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> instance that the report output will be written to.</param>
        public ConsoleReportViewer(TextWriter output) {
            this._output = output;
        } // end overloaded constructor

        #endregion

        #region --Methods--

        /// <summary>
        /// Prints a report to the output given in this instances constructor.  The report will print the information in the given <paramref name="contents"/>, ordering it based on the <paramref name="settings"/> specified.
        /// </summary>
        /// <param name="settings">The <see cref="ReportSettings"/> instance that contains sorting information.</param>
        /// <param name="contents">The <see cref="IEnumerable{DataItem}"/> with items to be printed to the report output.</param>
        public void PrintReport(ReportSettings settings, IEnumerable<DataItem> contents) {
            this._output.WriteLine("Writing report based on file: {0}.", settings.FileName);
            this._output.WriteLine("-------------------------------------------------");
            this._output.WriteLine("Time Stamp\t\tSeverity\t\tOperation\t\tData");
            this._output.WriteLine("----------\t\t--------\t\t---------\t\t----");

            var sortedContents = this.SortContents(settings, contents);
            
            foreach (var item in contents) {
                var lineData = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                        "{0}\t\t{1}\t\t{2}\t\t{3}",
                        item.TimeStamp,
                        item.Severity.ToString(),
                        item.Operation,
                        item.Data);
                this._output.WriteLine(lineData);
            }

            this._output.WriteLine("-------------------------------------------------");
            this._output.WriteLine("Totals");
            this._output.WriteLine("Errors:\t\t{0}", contents.Count(x => x.Severity == ItemSeverity.ERROR));
            this._output.WriteLine("Warns:\t\t{0}", contents.Count(x => x.Severity == ItemSeverity.WARN));
            this._output.WriteLine("Info:\t\t{0}", contents.Count(x => x.Severity == ItemSeverity.INFO));
            this._output.WriteLine("First Entry:\t\t{0}", contents.OrderBy(x => x.TimeStamp).FirstOrDefault().TimeStamp);
            this._output.WriteLine("Last Entry:\t\t{0}", contents.OrderByDescending(x => x.TimeStamp).FirstOrDefault().TimeStamp);
        } // end method PrintReport

        #endregion

        #region --Functions--

        /// <summary>
        /// Sorts the given <paramref name="contents"/> in order by timestamp in the order specified by the <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">The <see cref="ReportSettings"/> instance that contains sorting information.</param>
        /// <param name="contents">The <see cref="IEnumerable{DataItem}"/> with items to be sorted.</param>
        /// <returns>An new <see cref="IEnumerable{DataItem}"/> with its contents ordered according to the sorting specified in the <paramref name="settings"/>.</returns>
        public IEnumerable<DataItem> SortContents(ReportSettings settings, IEnumerable<DataItem> contents) {
            List<DataItem> returnValue = new List<DataItem>();
            if (settings.SortAscending) {
                returnValue = contents.OrderBy(x => x.TimeStamp).ToList();
            } else {
                returnValue = contents.OrderByDescending(x => x.TimeStamp).ToList();
            }
            return returnValue.AsEnumerable();
        } // end function SortContents

        #endregion

        #region --Properties--

        #endregion
    } // end class ConsoleReportViewer
} // end namespace
