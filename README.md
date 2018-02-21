# isolation-testing-refactor-example
Demonstrates concepts of tight vs. loose coupling, single responsibility, dependency injection, and writing automated tests

# TestingDependencyIsolation

This solution contains code samples for demonstrating unit testing concepts and refactoring strategies that can be used when attempting to update code to be unit-tested.

## Structure

There are two real examples in this solution:

 - **Original**
 - **Modified**

Both examples have three associated projects:

 - **TestingDependencyIsolation.***
 - **TestingDependencyIsolation.*.Tests**
 - **TestingDependencyIsolation.*.Driver**

## Usage

The `*.Tests` file contain any unit or integration tests for the corresponding class library project while the `*.Driver` files contain the "real-world" runnable applications in which the class library code is used.

The [`ReportViewer.cs`][source_1] file in the Original class library contains notes at the bottom that indicate the problems involved with writing tests for the code and general code smells.  The Modified series of projects has a re-factored version of the Original projects along with corresponding unit tests.

### Walkthrough

Start by opening the `Executables\TestingDependencyIsolation.Original.Driver` project and running the program.

Indicate either `0` or `1` to determine the sort order.
> Which you choose is arbitrary, but you must pick on or the other to continue

![Original Program Output][original_output]
Note the program output and press a key to close the program.

Next, have a look at the [`Inputs\ReportFile.txt`][source_2].

This is the sample file (which you can modify) that is used by both the *Original* and *Modified* versions of the program.

> To view how this is done, open the project properties of either executable and go to the **Build Events** tab.  The command listed under the *post build events* input is: `xcopy $(ProjectDir)..\..\..\Inputs\ReportFile.txt $(TargetDir) /y`

#### Program Logic

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

The above logic is primarily contained in the [`ReportViewer.cs`][source_1] file.

After you're comfortable with the logic, open the [`TestingDependencyIsolation.Original.Tests\Unit\Core\DataItemFixture.cs`][source_3] file and make note of the comments.

#### Testing - First Pass

> Run the unit test

After reviewing the comments and running the unit test open the [`ReportViewerFixture.cs`][source_4] file.

Notice the `[Ignore()]` attribution on the first test.  This attribute is used to tell NUnit to not run the test.  This allows tests that are in development or are temporarily broken to be excluded from the results.  This is particularly useful when used in conjunction with a build server, where tests are run on every commit and failed tests result in failed builds.

In this case, the code smells first start in defining what is being tested.  Unit tests should have a clearly definable, succinct SUT.  In this case, the constructor initializes a call chain that is so complex, the actual naming of the test becomes a challenge.

In addition to naming, we don't even get past step 1.  A good unit test should be isolated.  This means that the test (like the code it's testing) should only ever have **ONE** reason to fail: The code is broken or was changed.
The primary (and perhaps only) concern of any constructor should be to initialize default values such that the bare-minimum amount of information required to create an instance of a class is performed.  In contradiction to this idea in the case of the `ReportViewer` type, it initiates part of the processing logic and therefore makes an assumption about what the type *should* do, instead of what it *could* do.

This is poor OO design; the class has lost any versatility and makes too many assumptions about its own use.  In some cases, this type of behavior is *intended* by the developer, especially in the case of some very specific, opinionated frameworks.  As a rule, however, this should be avoided in OO design.  This is a `procedural` style of programming that is common in procedural languages and was prevalent in the early days of classic ASP, VB, and VBA / VBScript.
This program requires a significant refactor both from a code quality standpoint, and a test-ability standpoint.  Go ahead and attempt to run this test by removing the `[Ignore()]` attribute.

The test fails when attempting to load the configuration file from configuration because... We're in a test, we don't have the `app.config` file that the executable has.  This should also spark a chain reaction of thoughts in your mind: "Oh, if that isn't available, that means that the report file input may also not be available...", "What if this library were being used in a web-application or other way where Console operations were not the best way to perform input / output?", "What if configuration doesn't come from a config file at all?"

This is an example of tight-coupling.  Now, you could write an integration test for this situation, and add the app setting to the app.config of the test file but that is a different type of testing all together.

Unit tests should be devoid of this kind of environment configuration because a unit test should test logic in isolation.  The test now breaks for a reason that is completely unrelated to what we want to test.

Even if we decided to add the app.config to the test, certainly the case where the config isn't present is a case we want to test as well.  Now, we need to setup the configuration as part of the test setup, and make sure to clear it afterwards so other tests don't break.

If that weren't bad enough, we would quickly run into issue after issue with the code as it is (illustrated by the "chain-reaction" questions above).

Review the [`ReportViewer.cs`][source_1] file's comment section after the class.

Once you've reviewed that information, it's time to answer the question:  **How can we fix it?**

#### Fixing it - Overview

Now that *most* of the code smells have been defined, we need to bring any relevant design patterns and / or sound OO concepts to bear for the necessary re-structure of the code.  When doing this, we are attempting to balance the following aspects of the code:

 - **Readability**
 - **Maintainability**
 - **Complexity**
 - **Testability**
 - **Performance**

These are not listed in order; some projects will put a higher priority on maintainability if many changes are expected, others will emphasize performance.  Often, certain sub-sections of code will have different priorities within the same project, so it's important to make sure we're grounded in the bigger picture as testing is not an end itself, but a means to more well-rounded code.  Testing begins to become too costly and too cumbersome if the goal of the testing is the completion of tests, rather than the verification of the actual software.

In this case, we'll begin by identifying the responsibilities of program to see if we can extract meaningful, but well-defined boundaries that match the various tasks being performed by the code.

Perhaps the most obvious, is the actual printing of the report.

##### Refactoring

We need to build a context for refactoring, because the printing or the output of the program is the PRIMARY *business* goal of the software, this is a logical place to start.  First, we need to ask a series of questions in order to help guide our refactoring process.  The questions below are grouped by a design-pattern or principle that the questions are designed to help identify and answer.


1) YAGNI (You ain't gonna' need it)
   a) Is there a clear business-case for changing the report output?
   b) How likely is it that different forms of output (other than to the console) will be requested?
2) SRP (Single Responsibility Principle)
   a) Can the output only be generated from a flat-file, or can it come from other sources?
   b) Is the format for the output always the same?
   c) Is the format for the input always the same?
3) IoC (Inverson of Control)
   a) Is it possible to "refresh" the report?
   b) Could additional transformations be applied before printing the output?

Together, these questions (and their respective answers) help to frame our refactoring effort.  Some of them could be elevated to business owners and help to define scope and requirements, while others are decisions to be made by the developer.

Let us assume the following answers to the questions:

> 1.i -- Is there a clear business-case for changing the report output?

In the future, we'd like to be able to support additional output options such as PDF, HTML, and possibly Excel but we haven't gone into much detail about when.  For now we just want something simple.

> 1.ii -- How likely is it that different forms of output (other than to the console) will be requested?

Based on the business answer to the first question, we can reasonably assume that the need for different outputs will be on the way.  At the same time, we don't know what those are or when (if) they will actually happen.  As a result, we won't spend a great deal of time writing those now, but we'll set ourselves up so that they will be easy to introduce later.

> 2.i -- Can the output only be generated from a flat-file, or can it come from other sources?

Eventually we need to pull in data from our `Data Warehouse` but our analysts are still massaging the information.

> 2.ii -- Is the format for the output always the same?

Probably not, but we don't have enough information about *how* it could possibly change, and it could add considerable complexity to add in formatting customization, so we'll leave this out for now.

> 2.iii -- Is the format for the input always the same?

Based on 2.i, no but this can be easily abstracted away, so we'll set it up now in preparation.

> 3.i -- Is it possible to "refresh" the report?

Yes, new data comes in all the time. <-- This answer from the business tells us that our constructor as it exists doesn't make sense because we have to create new instances of the entire `ReportViewer` EVERY time if they want to refresh the data.  Best to separate this behavior so it can be called as needed.

> 3.ii -- Could additional transformations be applied before printing the output?

Possibly, but at the very least we can assume that it will NOT always follow that immediate printing should follow loading the data. We can set that up as one path, while allowing for intervention along another path.


With answers to these questions, and new context we can now identify the following areas of responsibility.

Responsibility|Layer|Notes
:--|:--:|:--:|
|Data Loading|Data|Loads serial report data, could be various sources|
|Customization|System|Controls application flow, probably a config file, but not necessarily|
|Data Processing|Business|Transforming Input data into a standardized structure, means that report inputs will always be the same, but how it arrives in that form can change|
|File Reading|System|Can interact with the files in the file-system, for our standard flat-file input, but could come from Data Warehouse|
|Report Output|Business|Reads from standard input to display to console, console is one of many potential outputs|


For the purpose of this code, we'll assign special "suffixes" to the types we create that contain logic to give hints as to what they do.  This helps to improve readability when used in a consistent manner.  This is significant since we can already see we'll be creating many more types than the original version had.

The suffixes are as follows:

- Data --> *Processor
- System --> *Provider (spin-off of MS conventions.  Think: MembershipProvider, LogProvider, IdentityProvider, SqlProvider <-- All support non-business logic related concerns)
- Business --> *Viewer, *Engine

This way of using suffixes for type names is helpful when designing a tiered application (think MVC --> *Controller, *Model, *Repository, *ViewModel, *Service, etc.)


At the core, one of the largest deficiencies of the original code was it was too tightly coupled.  To address that, we'll define interfaces in places where functionality could change in the future (based on the answers to our questions).

The following interfaces are created:

- [`IConfigurationProvider`][1] <-- Because configuration could come from an app.config, web.config, or even SQL.
  - Our library will not define a concrete implementation for this interface because we're in context of a class library and we don't know where this class library will be used.
- `IDataProcessor` <-- Because for now we are processing data from a flat file with a known structure, but unsure how this will happen moving forward
- `IFileSystemProvider` <-- Because we want to abstract away file-system calls to improve testability
- `IReportViewer` <-- Because we may have different outputs in the future (HTML, PDF, etc.) but for now we want Console.

The foundation of our application is based on these interfaces.  Have a look at these interfaces now.

Also note how the `DataItem` and `ItemSeverity` types were moved over without modification.  These don't need to be refactored.

Let's look more specifically at the implementation for the `IFileSystemProvider` interface first, the `SimpleFileSystemProvider` type.
Because this type "crosses the boundary" between our code and an external dependency, we know that we will not *really* be able to write unit-tests for it.  The goal is to move this "boundary-crossing" code or "external dependency" to a single place within our code-base, which should enable any code the needs to cross this boundary to be more easily unit-tested.  This promotes loose-coupling and can help to minimize bugs as the file-system is always accessed consistently, with non-business related concerns addressed, such as is the file there, do we have permission to read the file, how do we read the contents of the file, etc.).  This has the added benefit of making the portion of the code we are most interested in testing, the business logic, is free from the "problematic" code that has been isolated so more of the important behaviors can be verified.

Note that the XML comments for this type and its methods provide insight into HOW it was intended to be used, not as much WHAT the methods do.  The names of the methods should be enough to figure out what they do in *MOST* cases.  This type of documentation is critical when breaking out application components into more abstract layers that may not initially be seen.

By nature, loosely coupled code can be more difficult to read.  The solution is not to put it all in one file or reduce the overall number of files.  In the past, programs were written in files that could and often did exceed 10s of thousands of lines.  There is a balance to be struck for readability between single-file and 1 file per method.  Providing adequate documentation, consistent naming conventions, and intuitive type / method names means that the code will be more well understood and used more effectively by those other than the author.

The `SimpleFileSystemProvider` type is named aptly because it isn't designed for complex file-system interactions such as thread synchronization with multiple threads accessing a single file system resource, or buffering large contents.  For those behaviors a more advanced provider could be implemented and provided when necessary.

Firstly, we'll look at the constructor.  Because this constructor has no dependencies, it can be deduced that creating tests for methods should require less up-front setup because there are no code dependencies.  There is however a single data dependency (on the file-system) so most of our testing will be integration testing and most of that will require some setup that deals with the file-system.

Now looking at the `BuildPath` method, we see that this implementation is little more than a thin wrapper that simply calls the system method: `Path.Combine(params string)`.  In many cases, this might be considered a "waste" or at the very least, overkill.  Even something this small can dramatically improve the testability of the code that uses this provider.  Alternatively, calling code would need to provide a method for easily creating correct file-system paths.  Use of the `Path.Combine` method elsewhere has several implications:

 - `Path.Combine` is a static method.  While this isn't a *bad* thing, it means that we have **NO** way to alter the behavior, which is problematic when writing unit tests for types that deal with the file-system.
 - `Path.Combine` *should* be isolated away from our business logic because it solves a programming problem, not a business problem.
 - Most of the times where `Path.Combine` would be needed would be in building file-system paths for reading, verification, or writing, all of which happen in this provider so its inclusion is logical.

Next we'll consider the `Exists(FileSystemType, string)` method.  This function has more logic in it than the `BuildPath` method but it's ultimate goal is to wrap either the `Directory.Exists` method, or the `File.Exists` method.  These methods are again isolated and put in this provider so that code that needs to verif whether a file exists or not before performing an operation can be tested independent of whether the file-system exists.  When testing the `ReportingEngine` type, the `IFileSystemProvider` injected into the constructor can be mocked or stubbed to provide a specific value for the purposes of exercising a code-path on the `ReportingEngine` class.  Without the `IFileSystemProvider` interface wrapping the `Exists` method, the `ReportingEngine` couldn't be unit tested as the file would have to ACTUALLY exist.  Depdency on the interface means that we don't actually have to have the file and can "pretend" that the `IFileSystemProvider.Exists(FileSystemType, string)` method just returns `true` or just returns `false`.


[source_1]: Source/Examples/Original/ExternalDependencies/Core/ReportViewer.cs
[source_2]: Inputs/ReportFile.txt
[source_3]: Source/Tests/Original/Unit/Core/DataItemFixture.cs
[source_4]: Source/Tests/Original/Unit/Core/ReportViewerFixture.cs
[source_9]: Source/Examples/Modified/Configuration/IConfigurationProvider.cs

[original_output]: images/original_output_descending.jpg