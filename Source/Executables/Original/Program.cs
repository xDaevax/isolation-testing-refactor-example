using System;
using TestingDependencyIsolation.Original.ExternalDependencies.Core;

namespace TestingDependencyIsolation.Original.Driver {
    class Program {
        static void Main(string[] args) {
            ReportViewer viewer = new ReportViewer();

            viewer.PrintReport();
            Console.ReadLine(); // Halt the exit
        }
    }
}
