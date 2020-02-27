# Integration Testing Options With .NET

## QuickStart
Ensure that you have the .net core3 framework SDK installed and dotnet.exe in your path variable, then, in the TestAutomationDemo.IntegrationTests folder:
```c#
dotnet test
```

This solution contains a few different ways of integration testing the AutomationDemo.Website web application:

### Using the M$ TestServer
This allows us to start up the web application *inside* our test and query it using a HttpClient.
Benefits:
- FAST. You will not find a faster integration test running in .net - this will often be 10 times faster than the other methods;
- Parallelisable. Our tests are highly isolated, we can run lots of these at once without side effects;
- Configurable. We can easily inject our own config, or replace the services running within the app to suit our tests.

Drawbacks:
- HttpClient ONLY. We do not run our tests against a browser implementation, meaning the person writing the tests is responsible for the exact requests being made, rather than clicking buttons etc.

### Using a deployed Web Application
I have supplied 2 versions using a deployed version of our web application. The first communicates via HttpClient as with the above, the second uses a headless
chrome browser via selenium.
Benefits:
- Ease of use. From a developers point of view, they can simply start interacting with selenium (or more likely a page model over selenium);
- Slow(er). We have to spin up an actual version of the web application, deploy it, then hit it. This all adds up;
- Potential side effects. Because we are running all our tests against the same web deployment, they may interact with each other with unintented consequences;
- Trickier mocking of functionality. If we wish to switch out a component for a test double, we need to build the functionality into the web application specifically, or use external fakes.

