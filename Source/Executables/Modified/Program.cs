using System;
using TestingDependencyIsolation.Modified.Configuration;
using TestingDependencyIsolation.Modified.Data;
using TestingDependencyIsolation.Modified.Driver.Configuration;
using TestingDependencyIsolation.Modified.IO;
using TestingDependencyIsolation.Modified.Reporting;

namespace TestingDependencyIsolation.Modified.Driver {
    class Program {
        static void Main(string[] args) {
            // This section serves as the composition root.
            // In Dependency Injection or Inversion of Control, the composition root is the "entry" point of the application.
            // For web-applications it is the App_Start or AppStart, for windows services it is the OnStart method.
            // For console applications, it is the main method.
            // Only executables have a REAL composition root.  Class libraries are by definition a library of types to be used by
            // calling code to accomplish a goal.  The class library cannot (and should not) make any assumptions beyond it's own behavior
            // regarding how it will be used.

            // Take note of the difference between the IConfigurationProvider and the AppConfigProvider.
            // The interface is defined in the class library, while the implementation is defined in the console application project.
            // This setup allows the console application to be able to "read" configuration, without having to know how that particular configuration
            // should be accessed.  You COULD provide a default provider in the class library for configuration (as we did with the FileSystem)
            // but this gives us maximum flexibility.  This same class library used in a web.application would load config from a web.config
            // file instead of an app.config.  Or it could be a SQLConfigurationProvider as long as they implement the class library interface.
            IConfigurationProvider provider = new AppConfigProvider();
            IDataProcessor processor = new TextFileProcessor();
            IFileSystemProvider fileSystem = new SimpleFileSystemProvider();

            // Notice the .Out property of the static Console class being passed as an argument.
            // The constructor dependency for the ConsoleReportViewer requires a TextWriter instance (which the .Out property happens to be).
            // We could really pass in ANY TextWriter instance, so you COULD rename the ConsoleReportViewer to TextReportViewer to be more specific.
            // It is this injected dependency that allows us to
            // A) Actually test the output in tests
            // B) De-couple the ConsoleReportViewer from NEEDING to run in a place where it has access to the actual Console.
            // In this case, we don't have to worry about disposing of the TextWriter (like you would normally) because it's lifetime is managed
            // by the Console Application
            IReportViewer viewer = new ConsoleReportViewer(Console.Out);

            // Because we have modified the constructor to take interface arguments, and moved the primary logic from the actual viewer
            // to a new "layer" called an engine, our code-base has become larger, but also more loosely coupled.
            ReportingEngine engine = new ReportingEngine(provider, processor, fileSystem, viewer);

            engine.GenerateReport();
            Console.ReadLine();
        }
    }
}
