using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestingDependencyIsolation.Original.ExternalDependencies.Core {
    public class ReportViewer {
        #region --Fields--

        private bool _initialized;
        private string _reportFileName;
        private List<DataItem> _reportData;
        private bool _sortAscending = false; // Defaults to most recent first

        #endregion

        #region --Constructors--

        public ReportViewer() : this(FindFile()) {
        } // end default constructor

        public ReportViewer(string reportFile) {
            this._reportData = new List<DataItem>();
            this._reportFileName = reportFile;
            this.Initialize();
        } // end overloaded constructor

        #endregion

        #region --Methods--

        protected void Initialize() {
            if (File.Exists(this._reportFileName)) {
                this.LoadReportData();
            } else {
                Console.WriteLine("Report file {0} not found.", this._reportFileName);
            }
        }

        protected void LoadReportData() {
            string contents = File.ReadAllText(this._reportFileName);
            if (!string.IsNullOrWhiteSpace(contents)) {
                var lines = contents.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines) {
                    this._reportData.Add(this.ParseLine(line));
                }
                this._initialized = true;
            } else {
                Console.WriteLine("No data found.");
            }
        }

        public void PrintReport() {
            if (!this._initialized) {
                Console.WriteLine("The data was never initialized.");
            } else {
                if (this._reportData == null || !this._reportData.Any()) {
                    Console.WriteLine("No data to view.");
                } else {
                    
                    int? desiredSortOrder = null;
                    while (desiredSortOrder == null) {
                        int inputSortOrder = -2;
                        Console.WriteLine("Please indicate the sort order to use for report data. (0 for descending, 1 for ascending)");
                        if(int.TryParse(Console.ReadLine(), out inputSortOrder)) {
                            if (inputSortOrder < 2 && inputSortOrder > -1) {
                                desiredSortOrder = inputSortOrder;
                            }
                        }
                    }

                    this._sortAscending = desiredSortOrder.Value == 1;
                    Console.WriteLine("Writing report based on file: {0}.", this._reportFileName);
                    Console.WriteLine("-------------------------------------------------");
                    Console.WriteLine("Time Stamp\t\tSeverity\t\tOperation\t\tData");
                    Console.WriteLine("----------\t\t--------\t\t---------\t\t----");
                    List<DataItem> sortedData = null;
                    if(this._sortAscending) {
                        sortedData = this._reportData.OrderBy(x => x.TimeStamp).ToList();
                    } else {
                        sortedData = this._reportData.OrderByDescending(x => x.TimeStamp).ToList();
                    }

                    sortedData.ForEach(x => {
                        var lineData = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                            "{0}\t\t{1}\t\t{2}\t\t{3}",
                            x.TimeStamp,
                            x.Severity.ToString(),
                            x.Operation,
                            x.Data);
                        Console.WriteLine(lineData);
                    });
                    Console.WriteLine("-------------------------------------------------");
                    Console.WriteLine("Totals");
                    Console.WriteLine("Errors:\t\t{0}", this._reportData.Count(x => x.Severity == ItemSeverity.ERROR));
                    Console.WriteLine("Warns:\t\t{0}", this._reportData.Count(x => x.Severity == ItemSeverity.WARN));
                    Console.WriteLine("Info:\t\t{0}", this._reportData.Count(x => x.Severity == ItemSeverity.INFO));
                    Console.WriteLine("First Entry:\t\t{0}", this._reportData.OrderBy(x => x.TimeStamp).FirstOrDefault().TimeStamp);
                    Console.WriteLine("Last Entry:\t\t{0}", this._reportData.OrderByDescending(x => x.TimeStamp).FirstOrDefault().TimeStamp);
                }
            }
        }

        #endregion

        #region --Functions--

        internal static string FindFile() {
            var file = ConfigurationManager.AppSettings["ReportFile"];
            return Path.Combine(Environment.CurrentDirectory, file);
        }

        protected DataItem ParseLine(string line) {
            DataItem returnValue = null;
            Regex headerRegex = new Regex(@"\[.+\]");
            var headerData = headerRegex.Match(line).Value; // matches anything in the form of [ any number of any character / space followed by ]
            var dateRegex = new Regex(@"(?!\[)+.+\.\d+"); // matches the form [ followed by any characters in any number, followed by a literal "." followed by any number of digits but no spaces, starting after the [
            DateTime timeStamp = DateTime.Parse(dateRegex.Match(headerData).Value);
            string severity = Enum.GetNames(typeof(ItemSeverity)).FirstOrDefault(x => headerData.Contains(x)); // Load the header value matching the enum name
            var operation = headerData.Split(new string[] { severity }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("]", string.Empty); // load the second half of the split removing the ending ]
            if (timeStamp != DateTime.MinValue && !string.IsNullOrWhiteSpace(severity) && !string.IsNullOrWhiteSpace(operation)) {
                var contents = string.Concat(line.Split(new string[] { "] " }, StringSplitOptions.None)[1], string.Empty);
                returnValue = new DataItem() {
                    Severity = (ItemSeverity)Enum.Parse(typeof(ItemSeverity), severity), // Parse it from a string to the actual enum
                    TimeStamp = timeStamp,
                    Data = contents.Trim(),
                    Operation = operation
                };
            } else {
                Console.WriteLine("Invalid line data: {0}", line);
                throw new FormatException(string.Format("Invalid line data found at: {0}", line));
            }

            return returnValue;
        }

        #endregion

        #region --Properties--

        // IEnumerable helps to ensure it can't be modified externally
        public virtual IEnumerable<DataItem> ReportData {
            get {
                return this._reportData.AsEnumerable();
            }
        }

        #endregion
    } // end class ReportViewer
} // end namespace

#region --Code Smells--

/*
 * // Many code smells introduce test pain (which is a measure of how difficult code is to test) so identifying those smells
 * // is a good first step in refactoring to allow for more testable (and as a side-effect, better designed) code.
 * SRP (Single Responsibility Principal)
 *  - The Report viewer is responsible for
 *      - Displaying the report
 *      - Loading the report data
 *      - Loading configuration values
 *      - Prompting the user for report inputs
 *      - Validating the user input
 * DI (Dependency Inversion or Inversion of Control [IoC])
 *  - Instead of being given the file or the results of the file, the code must know how to load the file
 *  - Instead of the code being passed the user inputs, it is responsible for gathering them
 *  - The code MUST have specific knowledge of how / where configuration is kept (what if configuration gets moved to SQL?)
 * Tight Coupling
 *  - The current implementation only allows output to console.  If this is the ONLY type of output, the file should be renamed to ConsoleReportViewer
 *    This ensures that future additions don't require a rename or re-explanation of terms like: MoreBetterReportViewerButNotConsole
 *  - The current implementation only allows data to come from a text file
 * Constructor Logic
 *  - Constructors should perform bare minimum setup required to instantiate a class, not perform processing logic
 * Method Availability
 *  - Most of the members defined in this class are protected or more restrictive.  This makes unit-testing harder.
 *    If the goal of the design is to prevent those from being called, a better strategy is to make an interface for this class and ONLY
 *    expose the methods you want to be available from the interface.  Then, make the code that depends on this class instead depend on this
 *    class' interface so only the known methods are shown.  Make the other non-interface functionality (where appropriate) public.
 *    This allows you to test the functionality in a unit test without the application code having access to the public methods.  This tactic
 *    is basic OOP and is referred to as information hiding.
 * 
 * */

#endregion