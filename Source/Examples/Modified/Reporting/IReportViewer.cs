using System.Collections.Generic;
using TestingDependencyIsolation.Modified.Core;

namespace TestingDependencyIsolation.Modified.Reporting {
    /// <summary>
    /// Interface used to define an external contract for types that can allow users to view report data.
    /// </summary>
    public interface IReportViewer {
        #region --Methods--

        void PrintReport(ReportSettings settings, IEnumerable<DataItem> contents);

        #endregion

        #region --Functions--

        #endregion
    } // end interface IReportViewer
} // end namespace
