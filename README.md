# isolation-testing-refactor-example
Demonstrates concepts of tight vs. loose coupling, single responsibility, dependency injection, and writing automated tests

# TestingDependencyIsolation

This solution contains code samples for demonstrating unit testing concepts and refactoring strategies that can be used when attempting to update code to be unit-tested.

## Structure

There are two real examples in this solution:

 - Original
 - Modified

Both examples have three associated projects:

 - TestingDependencyIsolation.*
 - TestingDependencyIsolation.*.Tests
 - TestingDependencyIsolation.*.Driver

## Usage

The `*.Tests` file contain any unit or integration tests for the corresponding class library project while the `*.Driver` files contain the "real-world" runnable applications in which the class library code is used.

The `ReportViewer.cs` file in the Original class library contains notes at the bottom that indicate the problems involved with writing tests for the code and general code smells.  The Modified series of projects has a re-factored version of the Original projects along with corresponding unit tests.

### Walkthrough

Start by opening the `Executables\TestingDependencyIsolation.Original.Driver` project and running the program.

Indicate either 0 or 1 to determine the sort order.
> Which you choose is arbitrary, but you must pick on or the other to continue

Note the program output and press a key to close the program.

Next, have a look at the `TestingDependencyIsolation.Original\ReportFile.txt`.

This is the sample file (which you can modify) that is used by both the *Original* and *Modified* versions of the program.

After you've reviewed the file, consider the following program flow for the Original example:

1) Program Start
2) If the overloaded constructor is called, proceed to initialize
3) If the default constructor is called, attempt to load the full path to the report file
	1) Use the app.settings file in the executable to read the name of the report file
4) Initialize the report
   1) Check for the existence of the given file name
      1) Print a message if it isn't found
      2) Continue if it was
   2) Read the contents of the file
      1) Print a message if no contents were found
      2) Continue if there were contents
   3) Split the contents based on a new line: `\r\n` or carriage return, line feed, ignoring empty lines
   4) Loop over every line in the file
      1) Perform string and regex splits on the various parts of the line
         1) If all header elements were found, the line is well-formatted, so continue
         2) Print a message and thrown an exception otherwise
      2) Build and return a new strongly typed instance
      3) Add the new instance to the `_reportData` list.
   5) Set the initialized flag
5) Invoke the Print method for the report
   1) Continue if the data was initialized, print a message and exit if not
   2) Continue if data was loaded, print a message and exit if not
   3) Prompt for valid user input indicating sort order
   4) Print the results to the Console

The above logic is primarily contained in the `ReportViewer.cs` file.

After you're comfortable with the logic, open the `TestingDependencyIsolation.Original.Tests\Unit\Core\DataItemFixture.cs` file and make note of the comments.

Run the unit test

After reviewing the comments and running the unit test open the `ReportViewerFixture.cs` file.

Notice the `[Ignore()]` attribution on the First test.  This attribute is used to tell NUnit to not run the test.  This allows tests that are in development or are temporarily broken to be excluded from the results.  This is particularly useful when used in conjunction with a build server, where tests are run on every commit and failed tests result in failed builds.

In this case, the code smells first start in defining what is being tested.  Unit tests should have a clearly definable, succinct SUT.  In this case, the constructor initializes a call chain that is so complex, the actual naming of the test becomes a challenge.

In addition to naming, we don't even get past step 1.  A good unit test should be isolated.  This means that the test (like the code it's testing) should only ever have **ONE** reason to fail: The code is broken.
In the case of the constructor, a constructors only responsibility should be to initialize default values or the bare-minimum amount of information required to create an instance of a class.  In the case of the `ReportViewer` type, it actually initiates part of the processing logic.

This is poor OO design at a minimum, as the class has lost any versatility.  This is a `procedural` style of programming that is common in procedural languages and was prevalent in the early days of classic ASP, VB, and VBA / VBScript.
This design requires a significant refactor both from a code quality standpoint, and a test-ability standpoint.  Go ahead and attempt to run this test by removing the `[Ignore()]` attribute.

The test fails when attempting to load the configuration file from configuration because... We're in a test, we don't have the app.config file that the driver has.

This is an example of tight-coupling.  Now, you could write an integration test for this situation, and add the app setting to the app.config of the test file but that is a different type of testing all together.

Unit tests should be devoid of this kind of environment configuration because a unit test should test logic in isolation.  The test now breaks for a reason that is completely unrelated to what we want to test.

Even if we decided to add the app.config to the test, certainly the case where the config isn't present is a case we want to test as well.  Now, we need to setup the configuration as part of the test setup, and make sure to clear it afterwords so others tests don't break.

If that weren't bad enough, we would quickly run into issue after issue with the code as it is.

Review the `ReportViewer.cs` file's comment section after the class.

Once you've reviewed that information, it's time to answer the question:  How can we fix it?

> TODO: Finish ReadMe